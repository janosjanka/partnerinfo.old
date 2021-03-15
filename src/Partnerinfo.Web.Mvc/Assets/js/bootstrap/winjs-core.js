/*! Copyright (c) Microsoft Corporation.  All Rights Reserved. Licensed under the MIT License. See License.txt in the project root for license information. */

(function (_Global, _WinJS) {
    "use strict";

    var DEBUG = 0;

    function noop() { }

    function log(options) {
        /// <signature>
        /// <summary>
        /// Outputs a message to the Web Console.
        /// </summary>
        /// <param name="options" type="String">A JavaScript string containing zero or more substitution strings.</param>
        /// </signature>
        /// <signature>
        /// <summary>
        /// Outputs a message to the Web Console.
        /// </summary>
        /// <param name="options" type="Object">May contain .type, .category, .message, properties.</param>
        /// </signature>
        var lOptions;
        if (typeof options === "string") {
            lOptions = { type: "log", message: options };
        } else {
            lOptions = options || {};
            lOptions.type = lOptions.type || "log";
        }
        var message = (lOptions.category ? lOptions.category + " >> " + lOptions.message : lOptions.message) || "";
        var paramArray = [message].concat(Array.prototype.slice.call(arguments, 1));
        _Global.console[lOptions.type].apply(_Global.console, paramArray);
    }

    function defineMembers(target, members, prefix) {
        var keys = Object.keys(members);
        var isArray = Array.isArray(target);
        var properties;
        var i, len;
        for (i = 0, len = keys.length; i < len; ++i) {
            var key = keys[i];
            var enumerable = key.charCodeAt(0) !== /*_*/95;
            var member = members[key];
            if (member && typeof member === 'object') {
                if (member.value !== undefined || typeof member.get === 'function' || typeof member.set === 'function') {
                    if (member.enumerable === undefined) {
                        member.enumerable = enumerable;
                    }
                    if (prefix && member.setName && typeof member.setName === 'function') {
                        member.setName(prefix + "." + key);
                    }
                    properties = properties || {};
                    properties[key] = member;
                    continue;
                }
            }
            if (!enumerable) {
                properties = properties || {};
                properties[key] = { value: member, enumerable: enumerable, configurable: true, writable: true };
                continue;
            }
            if (isArray) {
                target.forEach(function (target) {
                    target[key] = member;
                });
            } else {
                target[key] = member;
            }
        }
        if (properties) {
            if (isArray) {
                target.forEach(function (target) {
                    Object.defineProperties(target, properties);
                });
            } else {
                Object.defineProperties(target, properties);
            }
        }
    }

    //
    // Namespace
    //

    (function () {

        var _rootNamespace = _WinJS;
        if (!_rootNamespace.Namespace) {
            _rootNamespace.Namespace = Object.create(Object.prototype);
        }

        function createNamespace(parentNamespace, name) {
            var currentNamespace = parentNamespace || {};
            if (name) {
                var namespaceFragments = name.split(".");
                if (currentNamespace === _Global && namespaceFragments[0] === "WinJS") {
                    currentNamespace = _WinJS;
                    namespaceFragments.splice(0, 1);
                }
                for (var i = 0, len = namespaceFragments.length; i < len; ++i) {
                    var namespaceName = namespaceFragments[i];
                    if (!currentNamespace[namespaceName]) {
                        Object.defineProperty(currentNamespace, namespaceName,
                            { value: {}, writable: false, enumerable: true, configurable: true }
                        );
                    }
                    currentNamespace = currentNamespace[namespaceName];
                }
            }
            return currentNamespace;
        }

        function defineWithParent(parentNamespace, name, members) {
            /// <signature helpKeyword="WinJS.Namespace.defineWithParent">
            /// <summary locid="WinJS.Namespace.defineWithParent">
            /// Defines a new namespace with the specified name under the specified parent namespace.
            /// </summary>
            /// <param name="parentNamespace" type="Object" locid="WinJS.Namespace.defineWithParent_p:parentNamespace">
            /// The parent namespace.
            /// </param>
            /// <param name="name" type="String" locid="WinJS.Namespace.defineWithParent_p:name">
            /// The name of the new namespace.
            /// </param>
            /// <param name="members" type="Object" locid="WinJS.Namespace.defineWithParent_p:members">
            /// The members of the new namespace.
            /// </param>
            /// <returns type="Object" locid="WinJS.Namespace.defineWithParent_returnValue">
            /// The newly-defined namespace.
            /// </returns>
            /// </signature>
            var currentNamespace = createNamespace(parentNamespace, name);
            if (members) {
                defineMembers(currentNamespace, members, name || "<ANONYMOUS>");
            }
            return currentNamespace;
        }

        function define(name, members) {
            /// <signature helpKeyword="WinJS.Namespace.define">
            /// <summary locid="WinJS.Namespace.define">
            /// Defines a new namespace with the specified name.
            /// </summary>
            /// <param name="name" type="String" locid="WinJS.Namespace.define_p:name">
            /// The name of the namespace. This could be a dot-separated name for nested namespaces.
            /// </param>
            /// <param name="members" type="Object" locid="WinJS.Namespace.define_p:members">
            /// The members of the new namespace.
            /// </param>
            /// <returns type="Object" locid="WinJS.Namespace.define_returnValue">
            /// The newly-defined namespace.
            /// </returns>
            /// </signature>
            return defineWithParent(_Global, name, members);
        }

        Object.defineProperties(_rootNamespace.Namespace, {
            defineWithParent: { value: defineWithParent, writable: true, enumerable: true, configurable: true },
            define: { value: define, writable: true, enumerable: true, configurable: true }
        });

    })();

    //
    // Class
    //

    (function () {

        function define(constructor, instanceMembers, staticMembers) {
            /// <signature helpKeyword="WinJS.Class.define">
            /// <summary locid="WinJS.Class.define">
            /// Defines a class using the given constructor and the specified instance members.
            /// </summary>
            /// <param name="constructor" type="Function" locid="WinJS.Class.define_p:constructor">
            /// A constructor function that is used to instantiate this class.
            /// </param>
            /// <param name="instanceMembers" type="Object" locid="WinJS.Class.define_p:instanceMembers">
            /// The set of instance fields, properties, and methods made available on the class.
            /// </param>
            /// <param name="staticMembers" type="Object" locid="WinJS.Class.define_p:staticMembers">
            /// The set of static fields, properties, and methods made available on the class.
            /// </param>
            /// <returns type="Function" locid="WinJS.Class.define_returnValue">
            /// The newly-defined class.
            /// </returns>
            /// </signature>
            constructor = constructor || function () { };
            if (instanceMembers) {
                defineMembers(constructor.prototype, instanceMembers);
            }
            if (staticMembers) {
                defineMembers(constructor, staticMembers);
            }
            return constructor;
        }

        function derive(baseClass, constructor, instanceMembers, staticMembers) {
            /// <signature helpKeyword="WinJS.Class.derive">
            /// <summary locid="WinJS.Class.derive">
            /// Creates a sub-class based on the supplied baseClass parameter, using prototypal inheritance.
            /// </summary>
            /// <param name="baseClass" type="Function" locid="WinJS.Class.derive_p:baseClass">
            /// The class to inherit from.
            /// </param>
            /// <param name="constructor" type="Function" locid="WinJS.Class.derive_p:constructor">
            /// A constructor function that is used to instantiate this class.
            /// </param>
            /// <param name="instanceMembers" type="Object" locid="WinJS.Class.derive_p:instanceMembers">
            /// The set of instance fields, properties, and methods to be made available on the class.
            /// </param>
            /// <param name="staticMembers" type="Object" locid="WinJS.Class.derive_p:staticMembers">
            /// The set of static fields, properties, and methods to be made available on the class.
            /// </param>
            /// <returns type="Function" locid="WinJS.Class.derive_returnValue">
            /// The newly-defined class.
            /// </returns>
            /// </signature>
            if (baseClass) {
                constructor = constructor || function () { };
                var basePrototype = baseClass.prototype;
                constructor.prototype = Object.create(basePrototype);
                Object.defineProperty(constructor.prototype, "constructor", { value: constructor, writable: true, configurable: true, enumerable: true });
                if (instanceMembers) {
                    defineMembers(constructor.prototype, instanceMembers);
                }
                if (staticMembers) {
                    defineMembers(constructor, staticMembers);
                }
                return constructor;
            } else {
                return define(constructor, instanceMembers, staticMembers);
            }
        }

        function mix(constructor) {
            /// <signature helpKeyword="WinJS.Class.mix">
            /// <summary locid="WinJS.Class.mix">
            /// Defines a class using the given constructor and the union of the set of instance members
            /// specified by all the mixin objects. The mixin parameter list is of variable length.
            /// </summary>
            /// <param name="constructor" locid="WinJS.Class.mix_p:constructor">
            /// A constructor function that is used to instantiate this class.
            /// </param>
            /// <returns type="Function" locid="WinJS.Class.mix_returnValue">
            /// The newly-defined class.
            /// </returns>
            /// </signature>
            constructor = constructor || function () { };
            for (var i = 1, len = arguments.length; i < len; ++i) {
                defineMembers(constructor.prototype, arguments[i]);
            }
            return constructor;
        }

        _WinJS.Namespace.define("WinJS.Class", {
            define: define,
            derive: derive,
            mix: mix
        });

    })();

    //
    // Event
    //

    (function () {

        function createEventProperty(name) {
            var eventPropStateName = "_on" + name + "state";
            return {
                get: function () {
                    var state = this[eventPropStateName];
                    return state && state.userHandler;
                },
                set: function (handler) {
                    var state = this[eventPropStateName];
                    if (handler) {
                        if (!state) {
                            state = { wrapper: function (evt) { return state.userHandler(evt); }, userHandler: handler };
                            Object.defineProperty(this, eventPropStateName, { value: state, enumerable: false, writable: true, configurable: true });
                            this.addEventListener(name, state.wrapper, false);
                        }
                        state.userHandler = handler;
                    } else if (state) {
                        this.removeEventListener(name, state.wrapper, false);
                        this[eventPropStateName] = null;
                    }
                },
                enumerable: true
            };
        }

        function createEventProperties() {
            /// <signature>
            /// <summary locid="WinJS.Utilities.createEventProperties">
            /// Creates an object that has one property for each name passed to the function.
            /// </summary>
            /// <param name="events" locid="WinJS.Utilities.createEventProperties_p:events">
            /// A variable list of property names.
            /// </param>
            /// <returns type="Object" locid="WinJS.Utilities.createEventProperties_returnValue">
            /// The object with the specified properties. The names of the properties are prefixed with 'on'.
            /// </returns>
            /// </signature>
            var props = {};
            for (var i = 0, len = arguments.length; i < len; ++i) {
                var name = arguments[i];
                props["on" + name] = createEventProperty(name);
            }
            return props;
        }

        var EventMixinEvent = _WinJS.Class.define(
            function EventMixinEvent_ctor(type, detail, target) {
                this.detail = detail;
                this.target = target;
                this.timeStamp = Date.now();
                this.type = type;
            },
            {
                bubbles: { value: false, writable: false },
                cancelable: { value: false, writable: false },
                currentTarget: { get: function () { return this.target; } },
                defaultPrevented: { get: function () { return this._preventDefaultCalled; } },
                trusted: { value: false, writable: false },
                eventPhase: { value: 0, writable: false },
                target: null,
                timeStamp: null,
                type: null,
                preventDefault: function () { this._preventDefaultCalled = true; },
                stopImmediatePropagation: function () { this._stopImmediatePropagationCalled = true; },
                stopPropagation: function () { }
            }, {
                supportedForProcessing: false
            }
        );

        var eventMixin = {
            _listeners: null,

            addEventListener: function (type, listener, useCapture) {
                /// <signature>
                /// <summary>Adds an event listener to the control.</summary>
                /// <param name="type">The type (name) of the event.</param>
                /// <param name="listener">The listener to invoke when the event is raised.</param>
                /// <param name="useCapture">If true initiates capture, otherwise false.</param>
                /// </signature>
                useCapture = useCapture || false;
                this._listeners = this._listeners || {};
                var eventListeners = this._listeners[type] = this._listeners[type] || [];
                for (var i = 0, len = eventListeners.length; i < len; ++i) {
                    var l = eventListeners[i];
                    if (l.useCapture === useCapture && l.listener === listener) {
                        return;
                    }
                }
                eventListeners.push({ listener: listener, useCapture: useCapture });
            },
            dispatchEvent: function (type, details) {
                /// <signature>
                /// <summary>Raises an event of the specified type and with the specified additional properties.</summary>
                /// <param name="type">The type (name) of the event.</param>
                /// <param name="details">The set of additional properties to be attached to the event object when the event is raised.</param>
                /// <returns type="Boolean">True if preventDefault was called on the event.</returns>
                /// </signature>
                var listeners = this._listeners && this._listeners[type];
                if (listeners) {
                    var eventValue = new EventMixinEvent(type, details, this);
                    listeners = listeners.slice(0, listeners.length);
                    for (var i = 0, len = listeners.length; i < len && !eventValue._stopImmediatePropagationCalled; ++i) {
                        listeners[i].listener(eventValue);
                    }
                    return eventValue.defaultPrevented || false;
                }
                return false;
            },
            removeEventListener: function (type, listener, useCapture) {
                /// <signature>
                /// <summary>Removes an event listener from the control.</summary>
                /// <param name="type">The type (name) of the event.</param>
                /// <param name="listener">The listener to remove.</param>
                /// <param name="useCapture">Specifies whether to initiate capture.</param>
                /// </signature>
                useCapture = useCapture || false;
                var listeners = this._listeners && this._listeners[type];
                if (listeners) {
                    for (var i = 0, len = listeners.length; i < len; ++i) {
                        var l = listeners[i];
                        if (l.listener === listener && l.useCapture === useCapture) {
                            listeners.splice(i, 1);
                            if (listeners.length === 0) {
                                delete this._listeners[type];
                            }
                            break;
                        }
                    }
                }
            }
        };

        _WinJS.Namespace.defineWithParent(_WinJS, "Utilities", {
            _createEventProperty: createEventProperty,
            createEventProperties: createEventProperties,
            eventMixin: eventMixin
        });

    })();

    _WinJS.Namespace.defineWithParent(_Global, "WinJS", {
        DEBUG: DEBUG,
        log: log,
        noop: noop
    });

})(window, window.WinJS = {});
