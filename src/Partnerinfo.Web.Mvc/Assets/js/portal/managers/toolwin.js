// Copyright (c) Partnerinfo Ltd. All Rights Reserved.
/// <reference path="../designer.js" />

(function (_WinJS, _PI, _Portal) {
    "use strict";

    var _Class = _WinJS.Class;
    var _Utilities = _WinJS.Utilities;

    var winDisplay = _Portal.WinDisplay;

    var ns = _WinJS.Namespace.defineWithParent(_PI, "Portal", {
        ToolWinState: _Class.define(function ToolWinState_ctor(options) {
            /// <signature>
            /// <summary>Initializes a new instance of the ToolWinState class.</summary>
            /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the ToolWinState.</param>
            /// <returns type="ToolWinState" />
            /// </signature>
            options = options || {};
            options = _Utilities.extend(options, _PI.userCache(_PI.Storage.local, options.key));
            _Utilities.setOptions(this, options);

            this._disposed = false;
            this._spinCount = 0;

            this.key = options.key;
            this.display = options.display || winDisplay.none;
            this.enabled = ko.observable(this.display !== winDisplay.none);
            this.offsetX = options.offsetX || 0;
            this.offsetY = options.offsetY || 0;
            this.width = options.width || 300;
            this.height = options.height || 400;

            this._enabledSn = this.enabled.subscribe(this._enabledChanged, this);
        }, {
            save: function () {
                /// <signature>
                /// <summary>Save this object to a storage.</summary>
                /// </signature>
                _PI.userCache(_PI.Storage.local, this.key, this.toObject());
                this.dispatchEvent("changed");
            },
            setOption: function (key, value) {
                /// <signature>
                /// <summary>Sets the given value for the given key.</summary>
                /// <param name="key" type="String" />
                /// <param name="value" type="Object" />
                /// </signature>
                if (key !== "enabled") {
                    this[key] = value;
                }
                if (key === "display") {
                    this.enabled(this.display !== winDisplay.none);
                }
                return this;
            },
            setOptions: function (options) {
                /// <signature>
                /// <summary>Updates the given settings.</summary>
                /// <param name="options" type="Object" />
                /// </signature>
                for (var key in options) {
                    if (options.hasOwnProperty(key)) {
                        this.setOption(key, options[key]);
                    }
                }
                return this;
            },
            toObject: function () {
                /// <signature>
                /// <summary>Gets the native representation of this object.</summary>
                /// <returns type="Object" />
                /// </signature>
                return {
                    display: this.enabled() ? this.display : winDisplay.none,
                    offsetX: this.offsetX,
                    offsetY: this.offsetY,
                    width: this.width,
                    height: this.height
                };
            },
            _enabledChanged: function (value) {
                /// <signature>
                /// <param name="value" type="Boolean" />
                /// </signature>
                this.display = value ? winDisplay.opaque : winDisplay.none;
                this.save();
            },
            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                if (this._disposed) {
                    return;
                }
                this._enabledSn && this._enabledSn.dispose();
                this._enabledSn = null;
                this._disposed = true;
            }
        }),

        ToolWinManager: _Class.define(function ToolWinManager_ctor(designer, options) {
            /// <signature>
            /// <summary>Initializes a new instance of the ToolWinManager class.</summary>
            /// <param name="designer" type="$.PI.PortalDesigner" optional="true" />
            /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the ToolWinManager.</param>
            /// <returns type="ToolWinManager" />
            /// </signature>
            options = options || {};
            this._disposed = false;
            this.designer = designer;
            this.element = options.element;
            this.toolWins = [];
            options.toolWins && options.toolWins.forEach(this.add, this);
        }, {
            /// <field type="Array">
            /// An array of registered tool windows.
            /// </field>
            toolWins: null,

            add: function (toolWin) {
                /// <signature>
                /// <summary>Adds a new tool window.</summary>
                /// <param name="toolWin" type="Object" />
                /// </signature>
                this.toolWins.push(toolWin);
                toolWin.resources = toolWin.resources || {};
                toolWin.state = toolWin.state || new ns.ToolWinState();
                toolWin.state.addEventListener("changed", this.init.bind(this, toolWin), false);
                this.init(toolWin);
            },
            init: function (toolWin) {
                /// <signature>
                /// <summary>Initializes the given tool window.</summary>
                /// <param name="toolWin" type="Object" />
                /// </signature>
                toolWin.state.display === winDisplay.none
                    ? this._destroy(toolWin)
                    : this._create(toolWin);
            },
            find: function (key) {
                /// <signature>
                /// <summary>Finds a tool window definition by cache key.</summary>
                /// <param name="key" type="String" />
                /// <returns type="Object" />
                /// </signature>
                for (var i = 0, len = this.toolWins.length; i < len; ++i) {
                    var toolWin = this.toolWins[i];
                    if ($.PI[toolWin.type].prototype.options.key === key) {
                        return toolWin;
                    }
                }
            },
            displayAll: function (visible) {
                /// <signature>
                /// <summary>Displays or hides tool windows.</summary>
                /// <param name="visible" type="Boolean" />
                /// </signature>
                for (var i = 0, len = this.toolWins.length; i < len; ++i) {
                    var toolWin = this.toolWins[i];
                    toolWin.element && toolWin.element.css("visibility", visible ? "visible" : "hidden");
                }
            },
            refreshAll: function () {
                /// <signature>
                /// <summary>Sends a designer message to registered recipients.</summary>
                /// </signature>
                var toolWins = this.toolWins;
                for (var i = 0, len = toolWins.length; i < len; ++i) {
                    var toolWin = toolWins[i];
                    toolWin.element && toolWin.element[toolWin.type]("refresh");
                }
            },
            _create: function (toolWin) {
                /// <signature>
                /// <summary>Creates a new tool window using the specified toolWindow definition.</summary>
                /// <param name="toolWin" type="Object" />
                /// <returns type="jQuery" />
                /// </signature>
                if (!toolWin.element) {
                    toolWin.element = $("<div>").appendTo(this.element)[toolWin.type]({
                        designer: this.designer,
                        state: toolWin.state,
                        icon: toolWin.icon,
                        resources: toolWin.resources,
                        title: toolWin.title
                    });
                }
                return toolWin.element;
            },
            _destroy: function (toolWin) {
                /// <signature>
                /// <summary>Removes the specified tool window.</summary>
                /// </signature>
                toolWin.element && toolWin.element.remove();
                toolWin.element = null;
            },
            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                if (this._disposed) {
                    return;
                }
                if (this.toolWins) {
                    for (var i = 0, len = this.toolWins.length; i < len; ++i) {
                        this.toolWins[i].dispose();
                    }
                    this.toolWins = null;
                }
                this._disposed = true;
            }
        })
    });

    _Class.mix(ns.ToolWinState, _Utilities.createEventProperties("changed"));
    _Class.mix(ns.ToolWinState, _Utilities.eventMixin);

})(WinJS, PI, PI.Portal);
