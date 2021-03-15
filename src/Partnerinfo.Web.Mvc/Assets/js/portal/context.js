// Copyright (c) Partnerinfo TV. All Rights Reserved.

/// <reference path="engine.js" />

(function (_WinJS, _Portal) {
    "use strict";

    var _Promise = _WinJS.Promise;
    var _ModuleClasses = _Portal.ModuleClasses;

    function JQ(element) {
        /// <signature>
        /// <param name="element" type="HTMLElement" />
        /// <returns type="jQuery" />
        /// </signature>
        /// <signature>
        /// <param name="element" type="jQuery" />
        /// <returns type="jQuery" />
        /// </signature>
        if (!element || element.length === 0) {
            return; // Do not use the argument "undefined" because it can change meaning of the code !!!
        }
        return element.length ? element : $(element);
    }

    var ECMA6Map = window.Map ? window.Map : _WinJS.Class.define(null, {
        get: function (key) {
            return this[key];
        },
        set: function (key, value) {
            this[key] = value;
            return this;
        }
    });

    var ns = _WinJS.Namespace.defineWithParent(PI, "Portal", {
        ModuleContext: _WinJS.Class.define(function (options) {
            /// <signature>
            /// <summary>
            /// Exposes runtime services, such as module parsing, module binding, user tracking, and so on.
            /// </summary>
            /// <param name="engine" type="PI.Portal.Engine" />
            /// <param name="options" type="Object">
            ///     <para>container: Element - The root element</para>
            ///     <para>types: Array - Module type objects</para>
            /// </param>
            /// <returns type="PI.Portal.ModuleContext" />
            /// </signature>
            this._types = new ECMA6Map();
            this._options = options = options || {};
            this._engine = options.engine;
            this.container = options.container;
        }, {
            register: function (types) {
                /// <signature>
                /// <summary>Registers all the page module type objects in the current context.</summary>
                /// <param name="modules" type="Array" />
                /// </signature>
                for (var i = 0, len = types.length, t; i < len; ++i) {
                    t = types[i];
                    t.resources = _T(t.resourceKey) || {};
                    t.path = t.path || (this._engine.modulePath + "/" + t.typeName + "/");
                    t.name = t.resources.name || t.name;
                    t.menu = t.menu | ns.MenuType.none;
                    this._types.set(t.className, t);
                }
            },
            require: function (className, requires) {
                /// <signature>
                /// <summary>Loads module dependencies using the given name array.</summary>
                /// <param name="className" type="String">The CSS class name for which the type object is obtained.</param>
                /// <param name="requires" type="Array" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                var type = this.getType(className);
                if (!type) {
                    return _Promise.error();
                }
                return require(requires.map(function (req) { return type.path + req; }));
            },
            getType: function (className) {
                /// <signature>
                /// <summary>This function is used to obtain the type object for a page module.</summary>
                /// <param name="className" type="String">The CSS class name for which the type object is obtained.</param>
                /// <returns type="Object" mayBeNull="true" value="{ className: '', typeName: '', module: '', name: '', icon: '', require: [] }" />
                /// </signature>
                return this._types.get(className);
            },
            getTypeOf: function (element) {
                /// <signature>
                /// <summary>This function is used to obtain the type object for a page module.</summary>
                /// <param name="element" type="HTMLElement">The DOM element for which the type object is obtained.</param>
                /// <returns type="Object" value="{ className: '', typeName: '', module: '', name: '', icon: '', require: [] }" />
                /// </signature>
                /// <signature>
                /// <summary>This function is used to obtain the type object for a page module.</summary>
                /// <param name="element" type="jQuery">The DOM element for which the type object is obtained.</param>
                /// <returns type="Object" value="{ className: '', typeName: '', module: '', name: '', icon: '', require: [] }" />
                /// </signature>
                if (!element) {
                    return;
                }
                element = element.length ? element[0] : element;
                if (!element || !element.className) {
                    return;
                }
                var classNames = element.classList || element.className.match(/\S+/g);
                if (!classNames) {
                    return;
                }
                for (var i = 0, len = classNames.length, t; i < len; ++i) {
                    if (t = this._types.get(classNames[i])) {
                        return t;
                    }
                }
            },
            hasInstanceOf: function (element) {
                /// <signature>
                /// <summary>Checks whether the widget instance exists.</summary>
                /// <param name="element" type="HTMLElement" />
                /// <returns type="Boolean" />
                /// </signature>
                /// <signature>
                /// <summary>Checks whether the widget instance exists.</summary>
                /// <param name="element" type="jQuery" />
                /// <returns type="Boolean" />
                /// </signature>
                var type = this.getTypeOf(element);
                return type && JQ(element).is(":data(PI-" + type.module + ")");
            },
            getInstanceOf: function (element) {
                /// <signature>
                /// <summary>Gets the widget instance that associated with the element.</summary>
                /// <param name="element" type="HTMLElement" />
                /// <returns type="Object" />
                /// </signature>
                /// <signature>
                /// <summary>Gets the widget instance that associated with the element.</summary>
                /// <param name="element" type="jQuery" />
                /// <returns type="Object" />
                /// </signature>
                var type = this.getTypeOf(element);
                return type && JQ(element).data("PI-" + type.module);
            },
            loadEditorAsync: function (className) {
                /// <signature>
                /// <summary>Loads editor extensions for the given type class.</summary>
                /// <param name="className" type="String" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                var type = this.getType(className);
                if (!type) {
                    return _Promise.error();
                }
                // 1. Load all code dependencies specified
                var promise = type.editor
                    && type.editor.require
                    && require(type.editor.require)
                    || _Promise.complete();

                // 2. Load the module in a lazy way
                if (type.lazy) {
                    promise = promise.then(function () {
                        return require([type.path + "module.js"]);
                    });
                }

                // 3. Load all editor extensions
                return promise.then(function () {
                    return require([
                        type.path + "editor.js",
                        type.path + "editor.html"
                    ]);
                });
            },
            forEach: function (callback, container, thisArg) {
                /// <signature>
                /// <summary>Enumerates a collection of page modules in the container.
                /// The selector expression includes the specified container element too.</summary>                
                /// <param name="callback" type="Function">A function to be called when a corresponding operation completes.                
                ///     <para>element: jQuery - The page module element as jQuery object.</para>
                ///     <para>type: Object - The type object for the current page module.</para>
                ///     <para>widget: $.PI.PortalModule - The widget object that associated with the current element.</para>
                ///     <para>index: Number - The current index where the page module is.</para>
                ///     <para>count: Number - The number of the page modules.</para>
                /// </param>
                /// <param name="container" type="HTMLElement" optional="true">The container element.</param>
                /// <param name="thisArg" optional="true">Object to use as this when executing callback.</param>
                /// </signature>
                /// <signature>
                /// <summary>Enumerates a collection of page modules in the container.
                /// The selector expression includes the specified container element too.</summary>                
                /// <param name="callback" type="Function">A function to be called when a corresponding operation completes.                
                ///     <para>element: jQuery - The page module element as jQuery object.</para>
                ///     <para>type: Object - The type object for the current page module.</para>
                ///     <para>widget: $.PI.PortalModule - The widget object that associated with the current element.</para>
                ///     <para>index: Number - The current index where the page module is.</para>
                ///     <para>count: Number - The number of the page modules.</para>
                /// </param>
                /// <param name="container" type="jQuery" optional="true">The container element.</param>
                /// <param name="thisArg" optional="true">Object to use as this when executing callback.</param>
                /// </signature>
                var elements = $("." + _ModuleClasses.module, container || this.container).addBack("." + _ModuleClasses.module);
                for (var i = 0, len = elements.length; i < len; ++i) {
                    var element = elements[i], type;
                    if (type = this.getTypeOf(element)) {
                        var $element = $(element);
                        if (callback.apply(thisArg, [$element, type, $element.data("PI-" + type.module), i, len])) {
                            break;
                        }
                    } else {
                        this.reset(element);
                        _WinJS.DEBUG && _WinJS.log("DOM: Unknown module type '%s' at %d.", element.className, i);
                    }
                }
            },
            findFirst: function (container, className) {
                /// <signature>
                /// <summary>Returns the first child or descendant object that matches the specified CSS class.</summary>
                /// <param name="container" type="HTMLElement" />
                /// <param name="className" type="String" />
                /// <returns type="Object" value="{ element: jQuery(), typeName: '', widget: $.PI.PortalModule(), index: 0, count: 0 }" />
                /// </signature>
                /// <signature>
                /// <summary>Returns the first child or descendant object that matches the specified CSS class.</summary>
                /// <param name="container" type="jQuery" />
                /// <param name="className" type="String" />
                /// <returns type="Object" value="{ element: jQuery(), typeName: '', widget: $.PI.PortalModule(), index: 0, count: 0 }" />
                /// </signature>
                var result;
                this.forEach(function ($element, type, widget, index, count) {
                    if (type.className === className) {
                        result = {
                            element: $element,
                            type: type,
                            widget: widget,
                            index: index,
                            count: count
                        };
                        return true;
                    }
                    return false;
                }, container);
                return result;
            },
            findOwner: function (element, addBack) {
                /// <signature>
                /// <summary>Finds the owner of the specified element.</summary>
                /// <param name="element" type="HTMLElement" />
                /// <param name="addBack" type="Boolean" value="false" optional="true" />
                /// <returns type="Object" value="{ element: jQuery(), typeName: '', widget: $.PI.PortalModule() }" />
                /// </signature>
                /// <signature>
                /// <summary>Finds the owner of the specified element.</summary>
                /// <param name="element" type="jQuery" />
                /// <param name="addBack" type="Boolean" value="false" optional="true" />
                /// <returns type="Object" value="{ element: jQuery(), typeName: '', widget: $.PI.PortalModule() }" />
                /// </signature>
                if (!element) {
                    return;
                }
                var $element = addBack === true
                    ? JQ(element).closest("." + _ModuleClasses.module)
                    : JQ(element).parents("." + _ModuleClasses.module + ":first");
                return $element.length && {
                    element: $element,
                    type: this.getTypeOf($element),
                    widget: this.getInstanceOf($element)
                };
            },
            reset: function (element) {
                /// <signature>
                /// <summary>Resets the element. This function removes all module attributes and CSS classes.</summary>
                /// <param name="element" type="HTMLElement" />
                /// </signature>
                /// <signature>
                /// <summary>Resets the element. This function removes all module attributes and CSS classes.</summary>
                /// <param name="element" type="jQuery" />
                /// </signature>
                var instance = this.getInstanceOf(element);
                instance && instance.destroy();
                JQ(element).removeAttr("class", "data-module-options");
            },
            parse: function (element, options, type) {
                /// <signature>
                /// <summary>Parses the DOM element and creates the corresponding page module.
                ///     <para>done: Function - function (element, type, widget)</para>
                ///     <para>fail: Function - function (element, type)</para></summary>
                /// <param name="element" type="HTMLElement">The DOM element to parse.</param>
                /// <param name="options" type="Object" optional="true">A set of key/value pairs that can be used to configure the page module.</param>
                /// <param name="type" type="Object" optional="true">The type object for the page module.</param>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                var that = this;
                return _Promise(function ModuleContext_parse(completeDispatch, errorDispatch) {
                    if (!(type = type || that.getTypeOf(element))) {
                        errorDispatch();
                        return;
                    }
                    (type.lazy ? require([type.path + "module.js"]) : _Promise.complete()).then(
                        function () {
                            options = options || {};
                            options.engine = that._engine;
                            var $element = JQ(element)[type.module](options);
                            completeDispatch($element, type, $element.data("PI-" + type.module));
                        },
                        function () {
                            errorDispatch(element, type);
                        });
                });
            },
            reparse: function (element, options, type) {
                /// <signature>
                /// <summary>Parses the DOM element and creates the corresponding page module.
                ///     <para>done: Function - function (element, type, widget)</para>
                ///     <para>fail: Function - function (element, type)</para></summary>
                /// <param name="element" type="HTMLElement">The DOM element to parse.</param>
                /// <param name="options" type="Object" optional="true">A set of key/value pairs that can be used to configure the page module.</param>
                /// <param name="type" type="Object" optional="true">The type object for the page module.</param>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                var instance = this.getInstanceOf(element);
                if (instance) {
                    instance.destroy();
                }
                return this.parse(element, options, type);
            },
            parseAll: function (options, container) {
                /// <signature>
                /// <summary>Parses all the DOM elements and creates the corresponding page modules.</summary>
                /// <param name="options" type="Object" optional="true" />
                /// <param name="container" type="HTMLElement" optional="true" />
                /// <returns type="$.Deferred" />
                /// </signature>
                var that = this;
                var deferred = $.Deferred();
                var objects = [];
                var objectsPerThread = -1;
                var objectCounter = 0;
                var threadPoolSize = 0;
                var position = 0;
                this.forEach(function ModuleContext_parseAll($element, type, widget, index, count) {
                    if (objectsPerThread === -1) {
                        threadPoolSize = count >= 10 ? (Math.floor(Math.log(count) / Math.LN10) << 2) : 1;
                        objectsPerThread = Math.floor((count + threadPoolSize - 1) / threadPoolSize);
                    }
                    objects.push({ element: $element, type: type });
                    if (++objectCounter >= objectsPerThread || (index === count - 1)) {
                        var handler = window.setTimeout((function (objects) {
                            // Create a closure because Internet Explorer doesn't support window.setTimeout callback arguments.
                            // https://developer.mozilla.org/en-US/docs/DOM/window.setTimeout#Callback_arguments
                            return function () {
                                function doneCallback() {
                                    if (++position >= count) {
                                        deferred.resolveWith(this, arguments);
                                    }
                                }
                                window.clearTimeout(handler);
                                var source;
                                var i = 0;
                                var len = objects.length;
                                try {
                                    for (; i < len; ++i) {
                                        source = objects[i];
                                        that.parse(source.element, options, source.type).always(doneCallback);
                                    }
                                } catch (ex) {
                                    position += len - i;
                                    deferred.notify({
                                        message: ex.toString(),
                                        source: source,
                                        position: position,
                                        count: count
                                    });
                                }
                            };
                        })(objects));
                        objects = [];
                        objectCounter = 0;
                    }
                }, container);
                return deferred.promise();
            },
            authorize: function (container) {
                /// <signature>
                /// <summary>Authorizes each of the modules.</summary>
                /// <param name="container" type="HTMLElement" optional="true" />
                /// <returns type="$.Deferred" />
                /// </signature>
                this.forEach(
                  function (element, type, module) {
                      module && module.authorize();
                  },
                  container);
            },
            isContainer: function (element) {
                /// <signature>
                /// <summary>Gets a value indicating whether the element is a container (panel, content page, etc.) element.</summary>
                /// <param name="element" type="HTMLElement">The element.</param>
                /// <returns type="Boolean" />
                /// </signature>
                var $element = JQ(element);
                if ($element.is("[data-contentplaceholder]")) {
                    return true;
                }
                var type = this.getTypeOf($element);
                if (type) {
                    var widget = $.PI[type.module];
                    if (widget) {
                        return widget.prototype.options.container;
                    }
                }
                return false;
            },
            createElement: function (className, id) {
                /// <signature>
                /// <summary>Creates a new page module DOM element.</summary>
                /// <param name="className" type="String">The CSS class name for which the element is created.</param>
                /// <param name="id" type="String" optional="true">The unique identifier for the page module element.</param>
                /// <returns type="jQuery" />
                /// </signature>
                return $("<div>")
                    .attr("id", id)
                    .addClass(_ModuleClasses.module)
                    .addClass(className);
            },
            destroy: function (element) {
                /// <signature>
                /// <summary>Destroys an existing module instance.</summary>
                /// <param name="element" type="HTMLElement">The DOM element to parse.</param>
                /// </signature>
                var instance = this.getInstanceOf(element);
                instance && instance.destroy();
            },
            filterEvents: function (container) {
                /// <signature>
                /// <summary>Finds all events in the container.</summary>
                /// <param name="container" type="HTMLElement" optional="true" />
                /// </signature>
                var events = [];
                this.forEach(function ($element, type, module) {
                    if (module && (type.className === _ModuleClasses.event)) {
                        events = events.concat(module.findEvents());
                    }
                }, container);
                return events;
            },
            executeEvent: function (eventName, container) {
                /// <signature>
                /// <summary>Executes all the events that match the given event name within the given container.</summary>
                /// <param name="eventName" type="String" />
                /// <param name="container" type="HTMLElement" optional="true" />
                /// </signature>
                this.forEach(function ($element, type, widget) {
                    if (widget && (type.className === _ModuleClasses.event)) {
                        widget.execute(eventName);
                    }
                }, container);
            },
            dataBind: function (data, element) {
                /// <signature>
                /// <summary>Binds data</summary>
                /// <param name="data" type="Object" />
                /// <param name="element" type="HTMLElement" optional="true" />
                /// </signature>
                /// <signature>
                /// <summary>Binds data</summary>
                /// <param name="data" type="Object" />
                /// <param name="element" type="jQuery" optional="true" />
                /// </signature>
                if (!data) {
                    return;
                }
                $("input,select,textarea", element || this.container).each(function () {
                    if (!this.name) {
                        return;
                    }
                    var value = data;
                    var props = this.name.split('.');
                    for (var i = 0, len = props.length; i < len; ++i) {
                        if (!(value = value[props[i]])) {
                            return;
                        }
                    }
                    if (this.type === "number"
                        && "valueAsNumber" in this) {
                        this.valueAsNumber = value;
                        return;
                    }
                    if ((this.type === "date"
                        || this.type === "time"
                        || this.type === "datetime")
                        && "valueAsDate" in this) {
                        this.valueAsDate = new Date(value);
                        return;
                    }
                    this.value = value;
                });
            }
        })

    });

})(WinJS, PI.Portal);
