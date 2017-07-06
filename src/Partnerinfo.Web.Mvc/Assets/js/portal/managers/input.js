// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (_WinJS, _PI, _Portal) {
    "use strict";

    function checkInput(element) {
        /// <signature>
        /// <summary>Returns a value indicating whether the element is an input element.</summary>
        /// <returns type="Boolean" />
        /// </signature>
        if (!element) {
            return false;
        }
        var tagName = element.tagName.toUpperCase();
        return tagName === "INPUT" || tagName === "TEXTAREA" || tagName === "SELECT";
    }

    var ns = _WinJS.Namespace.defineWithParent(_PI, "Portal", {

        InputManager: _WinJS.Class.define(function InputManager_ctor(designer, options) {
            /// <signature>
            /// <summary>Initializes a new instance of the MenuManager class.</summary>
            /// <param name="designer" type="$.PI.PortalDesigner" optional="true" />
            /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the MenuManager.</param>
            /// <returns type="InputManager" />
            /// </signature>
            options = options || {};
            this._disposed = false;
            this._mouseInfo = { dragSource: null, clientX: -1, clientY: -1, timeStamp: 0 };

            this._onKeyDownBound = this._onKeyDown.bind(this);
            this._onMouseDownBound = this._onMouseDown.bind(this);
            this._onMouseMoveBound = this._onMouseMove.bind(this);
            this._onMouseUpBound = this._onMouseUp.bind(this);
            this._onClickBound = this._onClick.bind(this);

            this.designer = designer;
            this.element = null;
            this.window1 = $(window).on("keydown", this._onKeyDownBound);
            this.window2 = window.parent && $(window.parent).on("keydown", this._onKeyDownBound);

            this.dblClickSensitivity = options.dblClickSensitivity || 400;
        }, {
            on: function (element) {
                /// <signature>
                /// <summary>Adds event handlers to the given element.</summary>
                /// <param name="element" type="jQuery" />
                /// </signature>
                this.off();
                this.element = element
                    .on("mousedown", this._onMouseDownBound)
                    .on("mousemove", this._onMouseMoveBound)
                    .on("mouseup", this._onMouseUpBound)
                    .on("click", this._onClickBound);
            },
            off: function () {
                /// <signature>
                /// <summary>Removes the attached event handlers.</summary>
                /// </signature>
                this.element && this.element
                    .off("click", this._onClickBound)
                    .off("mouseup", this._onMouseUpBound)
                    .off("mousemove", this._onMouseMoveBound)
                    .off("mousedown", this._onMouseDownBound);
                this.element = null;
            },
            _onMouseDown: function (event) {
                /// <signature>
                /// <param name="event" type="MouseEvent" />
                /// </signature>
                event.preventDefault();

                // This type of events is the most convenient for selection
                // because this event occurs immediately before an element is moved.
                // In practice, it means that the jQuery.draggable widget works fine with it.

                var module;
                if ((this.designer.options.mode === _Portal.ModuleState.edit) &&
                    (module = this.designer.options.engine.context.findOwner(event.target, true))) {

                    // 1. Select the element (Single Click)
                    this.designer.options.selection.select(module.element);

                    // 2.1 Drag & Drop
                    if (event.ctrlKey) {
                        this._mouseInfo.dragSource = module.element[0];
                        this._mouseInfo.dragSourceX = (parseInt(module.element.css("left"), 10) | 0) - event.clientX;
                        this._mouseInfo.dragSourceY = (parseInt(module.element.css("top"), 10) | 0) - event.clientY;
                    } else {
                        // 2.2 Edit the module (Double Click)
                        if ((this._mouseInfo.clientX === event.clientX) &&
                            (this._mouseInfo.clientY === event.clientY) &&
                            (event.timeStamp - this._mouseInfo.timeStamp) < this.dblClickSensitivity) {
                            this.designer.options.module.editAsync(module.element);
                        }
                    }

                    this._mouseInfo.clientX = event.clientX;
                    this._mouseInfo.clientY = event.clientY;
                    this._mouseInfo.timeStamp = event.timeStamp || Date.now();

                    return false;
                }

                return true;
            },
            _onMouseMove: function (event) {
                /// <signature>
                /// <param name="event" type="MouseEvent" />
                /// </signature>
                event.preventDefault();

                // This method is performance critical so we avoid to use jQuery wrappers.
                if (this._mouseInfo.dragSource) {
                    this._mouseInfo.dragSource.style.left = (event.clientX + this._mouseInfo.dragSourceX) + "px";
                    this._mouseInfo.dragSource.style.top = (event.clientY + this._mouseInfo.dragSourceY) + "px";
                    return false;
                }

                return true;
            },
            _onMouseUp: function (event) {
                /// <signature>
                /// <param name="event" type="MouseEvent" />
                /// </signature>
                event.preventDefault();

                if (this._mouseInfo.dragSource) {
                    this._mouseInfo.dragSource = null;
                    return false;
                }

                return true;
            },
            _onClick: function (event) {
                /// <signature>
                /// <param name="event" type="MouseEvent" />
                /// </signature>
                event.preventDefault();

                // Do not allow the user to navigate to a link located within the current page.
                // Unfortunately, we can't do this in the mousedown event handler below.

                return false;
            },
            _onKeyDown: function (event) {
                /// <signature>
                /// <summary>Fires when the user presses a key.</summary>
                /// <param name="event" type="KeyboardEvent" />
                /// </signature>
                var handled = false;
                var editing = this.designer.options.mode === _Portal.ModuleState.edit;
                if (!editing) {
                    if (event.which === 113) {
                        this.designer.editAsync();
                        handled = true;
                    }
                } else {
                    var module = this.designer.options.module;
                    var selection = this.designer.options.selection;
                    var altKey = event.altKey;
                    var ctrlKey = event.ctrlKey;
                    switch (event.which) {
                        case 113: // F2
                            selection.hasSelection() && module.editAsync(selection.current);
                            handled = true;
                            break;
                        case 83: // CTRL + S
                            if (ctrlKey && !checkInput(event.target)) {
                                this.designer.saveAsync();
                                handled = true;
                            }
                            break;
                        case 46: // DELETE
                            if (!ctrlKey && !checkInput(event.target) && selection.hasSelection()) {
                                module.removeAsync(selection.current);
                                handled = true;
                            }
                            break;
                        case 67: // CTRL + C
                        case 86: // CTRL + V
                        case 88: // CTRL + X
                            if (ctrlKey && !checkInput(event.target) && selection.hasSelection()) {
                                if (event.which === 86) {
                                    module.pasteAsync(selection.current); // CTRL + V
                                } else {
                                    module.copyAsync(selection.current); // CTRL + C                                    
                                    if (event.which === 88) {
                                        module.removeAsync(selection.current); // CTRL + X
                                    }
                                }
                                handled = true;
                            }
                            break;
                        case 38: // Alt + Up
                            if (altKey) {
                                module.moveAsync(selection.current, -1);
                                handled = true;
                            }
                            break;
                        case 40: // Alt + Down
                            if (altKey) {
                                module.moveAsync(selection.current, +1);
                                handled = true;
                            }
                            break;
                    }
                }
                if (handled) {
                    event.preventDefault();
                    event.stopPropagation();
                    return false;
                }
                return true;
            },
            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                if (this._disposed) {
                    return;
                }
                this.off();
                this.window1 && this.window1.off("keydown", this._onKeyDownBound);
                this.window2 && this.window2.off("keydown", this._onKeyDownBound);
                this.window1 = null;
                this.window2 = null;
                this._disposed = true;
            }
        })
    });

    _WinJS.Class.mix(ns.MenuManager, _WinJS.Utilities.eventMixin);

})(WinJS, PI, PI.Portal);