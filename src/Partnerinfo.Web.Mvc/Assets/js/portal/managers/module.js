// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="../designer.js" />

(function (_WinJS, Promise, Portal) {
    "use strict";

    var resources = {
        nameAlreadyExists: "Name {0} already exists."
    };
    var ModuleClasses = Portal.ModuleClasses;

    function JQ(element) {
        /// <signature>
        /// <param name="element" type="HTMLElement" />
        /// <returns type="jQuery" />
        /// </signature>
        /// <signature>
        /// <param name="element" type="jQuery" />
        /// <returns type="jQuery" />
        /// </signature>
        if (!element) {
            return;
        }
        return element.length ? element : $(element);
    }

    var ns = _WinJS.Namespace.defineWithParent(PI, "Portal", {
        ModuleManager: _WinJS.Class.define(function ModuleManager_ctor(designer, options) {
            /// <signature>
            /// <summary>Initializes a new instance of the ModuleManager class.</summary>
            /// <param name="designer" type="$.PI.PortalDesigner" optional="true" />
            /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the ModuleManager.</param>
            /// <returns type="ModuleManager" />
            /// </signature>
            options = options || {};
            this._disposed = false;
            this._designer = designer;
            this._engine = designer.options.engine;
        }, {
            parseAllAsync: function (options, container, filter) {
                /// <signature>
                /// <summary>Parses all the modules in the container.</summary>
                /// <param name="options" type="Object" optional="true" />
                /// <param name="container" type="Element" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return this._engine.context.parseAll(
                    this._createOptions(options),
                    container || this._designer.activeContainer);
            },
            getEvent: function (container) {
                /// <signature>
                /// <summary>Ensures an event module within the given container.</summary>
                /// <param name="container" type="HTMLElement" optional="true" />
                /// <returns type="jQuery" />
                /// </signature>
                /// <signature>
                /// <summary>Ensures an event module within the given container.</summary>
                /// <param name="container" type="jQuery" optional="true" />
                /// <returns type="jQuery" />
                /// </signature>
                container = JQ(container || this._designer.activeContainer);
                if (!container) {
                    return;
                }
                var module = $(".ui-module-event:first", container);
                if (module.length) {
                    return module;
                }
                module = this._engine.context.createElement("ui-module-event");
                container.append(module);
                return module;
            },
            getContent: function (element, addBack) {
                /// <signature>
                /// <summary>Returns the content of the given module element.</summary>
                /// <param name="element" type="HTMLElement" />
                /// <param name="addBack" type="Boolean" />
                /// <returns type="String" />
                /// </signature>
                var clone = JQ(element).clone(false);
                var html = addBack ? clone.outerHTML() : clone.html();
                //html = html.replace("data-module-options")
                return html;
            },
            addAsync: function (options) {
                /// <signature>
                /// <summary>Adds a new module with the given values.</summary>
                /// <param name="options" type="Object">
                ///     <para>container?: HTMLElement - The container element. If null, the selectedModule or the active container is used.</para>
                ///     <para>className: String - The type class.</para>
                ///     <para>options: Object - A set of key/value pairs that configure the new module instance.</para>
                /// </param>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                var container = options.container
                    || this._designer.options.selection.current
                    || this._designer.activeContainer;
                if (!this._engine.context.isContainer(container)) {
                    return Promise.error();
                }
                var that = this;
                var element = this._engine.context.createElement(options.className);
                return this._editAsync(element, options.options).then(function (instance) {
                    container.append(element);
                    that._designer.options.selection.select(element);
                });
            },
            editAsync: function (element) {
                /// <signature>
                /// <summary>Edits the given module associated with the given element.</summary>
                /// <param name="element" type="HTMLElement" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                var that = this;
                return this._editAsync(element).then(function (instance) {
                    that._designer.options.selection.select(element);
                });
            },
            removeAsync: function (element) {
                /// <signature>
                /// <summary>Deletes the given module element in an async way.</summary>
                /// <param name="element" type="HTMLElement" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                /// <signature>
                /// <summary>Deletes the given module element in an async way.</summary>
                /// <param name="element" type="jQuery" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return Promise(function (completeDispatch) {
                    var $element = JQ(element);
                    if (this._designer.options.selection.equals($element)) {
                        this._designer.options.selection.clear();
                    }
                    $element.remove();
                    completeDispatch();
                }, this);
            },
            emptyAsync: function (element) {
                /// <signature>
                /// <summary>Deletes the given module element's children in an async way.</summary>
                /// <param name="element" type="HTMLElement" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                /// <signature>
                /// <summary>Deletes the given module element's children in an async way.</summary>
                /// <param name="element" type="jQuery" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return Promise(function (completeDispatch) {
                    JQ(element).empty();
                    completeDispatch();
                }, this);
            },
            renameAsync: function (element, name) {
                /// <signature>
                /// <summary>Renames the given module element in an async way.</summary>
                /// <param name="element" type="HTMLElement" />
                /// <param name="name" type="String" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                /// <signature>
                /// <summary>Renames the given module element in an async way.</summary>
                /// <param name="element" type="jQuery" />
                /// <param name="name" type="String" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return Promise(function (completeDispatch, errorDispatch) {
                    if (name) {
                        name = name.uri(false, '-');
                        if (this._designer.container &&
                            this._designer.container.find("#" + name).length) {
                            errorDispatch(String.format(resources.nameAlreadyExists, name));
                            return;
                        }
                        JQ(element).attr("id", name);
                    } else {
                        JQ(element).removeAttr("id");
                    }
                    completeDispatch(name);
                }, this);
            },
            alignAsync: function (element, direction) {
                /// <signature>
                /// <summary>Aligns the elment to the given position.</summary>
                /// <param name="element" type="HTMLElement" />
                /// <param name="direction" type="Number" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                /// <signature>
                /// <summary>Aligns the elment to the given position.</summary>
                /// <param name="element" type="jQuery" />
                /// <param name="direction" type="Number" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return Promise(function (completeDisptach, errorDispatch) {
                    var $element = JQ(element).css("position", "relative");
                    if (direction === "center") {
                        $element.css({
                            "float": "",
                            "margin-left": "auto",
                            "margin-right": "auto"
                        });
                    } else {
                        $element.css({
                            "float": direction
                        });
                    }
                }, this);
            },
            splitAsync: function (element, numParts) {
                /// <signature>
                /// <summary>Splits the given module element into equal parts.</summary>
                /// <param name="element" type="HTMLElement" />
                /// <param name="numParts" type="Number" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                /// <signature>
                /// <summary>Splits the given module element into equal parts.</summary>
                /// <param name="element" type="jQuery" />
                /// <param name="numParts" type="Number" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return Promise(function (completeDispatch, errorDispatch) {
                    var $element = JQ(element);
                    if (numParts < 1 || !this._engine.context.isContainer($element)) {
                        errorDispatch();
                        return;
                    }

                    var content = $element.html();
                    $element.empty();
                    var width = 100.0 / numParts;
                    var panel;
                    for (var i = 1; i <= numParts; ++i) {
                        var div = this._engine.context.createElement("ui-module-panel")
                            .css({
                                "float": "left",
                                "width": width + "%"
                            })
                            .appendTo($element);
                        panel = panel || div;
                    }
                    panel.html(content);

                    var that = this;
                    this.parseAllAsync($element).then(
                        function () {
                            that._designer.options.selection.select(panel);
                            completeDispatch();
                        },
                        function () {
                            errorDispatch();
                        });
                }, this);
            },
            moveAsync: function (element, direction) {
                /// <signature>
                /// <summary>Moves the element up/down.</summary>
                /// <param name="element" type="HTMLElement" />
                /// <param name="direction" type="Number" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                /// <signature>
                /// <summary>Moves the element up/down.</summary>
                /// <param name="element" type="jQuery" />
                /// <param name="direction" type="Number" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return Promise(function (completeDispatch) {
                    var $element = JQ(element);
                    var target;
                    if (direction === -1) {
                        target = $element.prev();
                        target.length && $element.insertBefore(target);
                    }
                    else if (direction === 1) {
                        target = $element.next();
                        target.length && $element.insertAfter(target);
                    }
                    completeDispatch();
                });
            },
            copyAsync: function (sourceElement) {
                /// <signature>
                /// <summary>Copies the given module to the clipboard.</summary>
                /// <param name="sourceElement" type="HTMLElement" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                /// <signature>
                /// <summary>Copies the given module to the clipboard.</summary>
                /// <param name="sourceElement" type="jQuery" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return Promise(function (completeDispatch) {
                    PI.userCache(
                        PI.Storage.local,
                        "portal.clipboard",
                        JQ(sourceElement).clone(false).outerHTML());
                    completeDispatch();
                });
            },
            pasteAsync: function (targetElement) {
                /// <signature>
                /// <summary>Pastes a module from the clipboard.</summary>
                /// <param name="targetElement" type="HTMLElement" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                /// <signature>
                /// <summary>Pastes a module from the clipboard.</summary>
                /// <param name="targetElement" type="jQuery" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return this.pasteHtmlAsync(targetElement, PI.userCache(PI.Storage.local, "portal.clipboard"));
            },
            pasteHtmlAsync: function (targetElement, htmlContent) {
                /// <signature>
                /// <summary>Pastes a module from the clipboard.</summary>
                /// <param name="targetElement" type="HTMLElement" />
                /// <param name="htmlContent" type="String" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                /// <signature>
                /// <summary>Pastes a module from the clipboard.</summary>
                /// <param name="targetElement" type="jQuery" />
                /// <param name="htmlContent" type="String" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                var that = this;
                return Promise(function (completeDispatch, errorDispatch) {
                    if (!that._engine.context.isContainer(targetElement)) {
                        errorDispatch();
                        return;
                    }
                    var $element = $(htmlContent);
                    var type = that._engine.context.getTypeOf($element);
                    if (!type) {
                        errorDispatch();
                        return;
                    }
                    // Remove each of the state classes, and add the element to the target container.
                    that._removeState($element).appendTo(targetElement);

                    // Parse the content of the target element.
                    that.parseAllAsync(targetElement).then(
                        function () {
                            that._designer.options.selection.select($element);
                            completeDispatch($element);
                        },
                        function () { errorDispatch($element); });
                });
            },
            _editAsync: function (element, options) {
                /// <signature>
                /// <summary>Edits the given module associated with the given element.</summary>
                /// <param name="element" type="HTMLElement" />
                /// <param name="options" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                var typeInfo = this._engine.context.getTypeOf(element);
                if (!typeInfo) {
                    return Promise.error();
                }
                var that = this;
                return Promise(function (completeDispatch, errorDispatch) {
                    // First of all, load editor extensions for the given module type.
                    that._engine.context.loadEditorAsync(typeInfo.className).then(
                        function () { // (Re)parse the module.
                            that._engine.context.reparse(element, that._createOptions(options)).then(
                                function (element, type, instance) {
                                    that._designer.options.menu.setEditing(true);
                                    instance.edit(function (result) {
                                        try {
                                            if (result === "ok") {
                                                completeDispatch(instance);
                                            } else {
                                                errorDispatch();
                                            }
                                        } finally {
                                            that._designer.options.menu.setEditing(false);
                                        }
                                    });
                                });
                        });
                });
            },
            _removeState: function ($element) {
                /// <signature>
                /// <summary>Removes state flags, classes, etc.</summary>
                /// <param name="$element" type="jQuery" />
                /// <returns type="jQuery" />
                /// </signature>
                $(ModuleClasses.selected, $element).removeClass(ModuleClasses.selected);
                return $element.removeClass(ModuleClasses.selected);
            },
            _createOptions: function (options) {
                /// <signature>
                /// <summary>Extends default module options with the specified options.</summary>
                /// <param name="options" type="Object">A set of key/value pairs that can be used to configure module options.</param>
                /// <returns type="Object" />
                /// </signature>
                options = options || {};
                options.designer = this._designer;
                options.container = this._designer.activeContainer;
                options.mode = this._designer.options.mode;
                options.title = this._designer.options.title;
                return options;
            },
            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                if (this._disposed) {
                    return;
                }
                this._engine = null;
                this._designer = null;
                this._disposed = true;
            }
        })
    });

})(WinJS, WinJS.Promise, PI.Portal);