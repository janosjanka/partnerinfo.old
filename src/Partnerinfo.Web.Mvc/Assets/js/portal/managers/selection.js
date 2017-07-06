// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="../common.js" />

(function (_WinJS, _Portal) {
    "use strict";

    function JQFirst(element) {
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
        return element.length ? element.first() : $(element);
    }
    function JQEquals($element1, $element2) {
        /// <signature>
        /// <summary>Checks whether the two jQuery elements equal.</summary>
        /// <param name="$element1" type="jQuery" />
        /// <param name="$element2" type="jQuery" />
        /// <returns type="Boolean" />
        /// </signature>
        if ($element1 && !$element2 ||
            $element2 && !$element1) {
            return false;
        }
        return $element1 === $element2
            || $element1[0] === $element2[0];
    }

    var eventNames = {
        changing: "changing",
        changed: "changed"
    };
    var createEvent = _WinJS.Utilities._createEventProperty;
    var ns = _WinJS.Namespace.defineWithParent(PI, "Portal", {
        SelectionManager: _WinJS.Class.define(function SelectionManager_ctor(designer, options) {
            /// <signature>
            /// <summary>Initializes a new instance of the ModuleManager class.</summary>
            /// <param name="designer" type="$.PI.PortalDesigner" optional="true" />
            /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the ModuleManager.</param>
            /// <returns type="ModuleManager" />
            /// </signature>
            options = options || {};
            _WinJS.Utilities.setOptions(this, options);
            this._disposed = false;
            this._designer = designer;
            this._engine = designer.options.engine;
            this._className = options.className || _Portal.ModuleClasses.selected;
            this.current = options.current;
        }, {
            /// <field type="Function">
            /// Raised before modules are selected or deselected.
            /// </field>
            changing: createEvent(eventNames.changing),

            /// <field type="Function">
            /// Raised after modules are selected or deselected.
            /// </field>
            changed: createEvent(eventNames.changed),

            _isEditing: function () {
                /// <signature>
                /// <summary>Returns true if all modules are in edit mode.</summary>
                /// <returns type="Boolean" />
                /// </signature>
                return this._designer.options.mode === _Portal.ModuleState.edit;
            },
            hasSelection: function () {
                /// <signature>
                /// <summary>Returns true if there is at least one selected module.</summary>
                /// <returns type="Boolean" />
                /// </signature>
                return !!this.current;
            },
            equals: function (element) {
                /// <signature>
                /// <summary>Returns true if the current element equals to the given element.</summary>
                /// <param name="element" type="HTMLElement" />
                /// <returns type="Boolean" />
                /// </signature>
                /// <signature>
                /// <summary>Returns true if the current element equals to the given element.</summary>
                /// <param name="element" type="jQuery" />
                /// <returns type="Boolean" />
                /// </signature>
                return JQEquals(this.current, JQFirst(element));
            },
            select: function (element, sourceElement) {
                /// <signature>
                /// <summary>Sets the given element as selected.</summary>
                /// <param name="element" type="HTMLElement" />
                /// <param name="sourceElement" type="HTMLElement" optional="true" />
                /// <returns type="Boolean" />
                /// </signature>
                /// <signature>
                /// <summary>Sets the given element as selected.</summary>
                /// <param name="element" type="jQuery" />
                /// <param name="sourceElement" type="jQuery" optional="true" />
                /// <returns type="Boolean" />
                /// </signature>
                if (!this._isEditing()) {
                    // Do not allow to select modules in view mode.
                    return false;
                }
                var $element = JQFirst(element);
                /* This code is not used because the ToolTip module must be realigned after selection changes
                if (JQEquals(this.current, $element)) {
                    // Do not raise unnecessary 'selectionchanged' events.
                    return false;
                }
                */
                var eventDetails = { element: $element, sourceElement: JQFirst(sourceElement) };
                this.dispatchEvent(eventNames.changing, eventDetails);
                this.current && this.current.removeClass(this._className);
                this.current = $element && $element.addClass(this._className).focus();
                this.dispatchEvent(eventNames.changed, eventDetails);
                return true;
            },
            clear: function () {
                /// <signature>
                /// <summary>Clears the contents of the selection.</summary>
                /// <returns type="Boolean" />
                /// </signature>
                return this.select();
            },
            scroll: function (element) {
                /// <signature>
                /// </signature>
                /// <signature>
                /// <param name="element" type="HTMLElement" />
                /// </signature>
                /// <signature>
                /// <param name="element" type="jQuery" />
                /// </signature>
                var $element = arguments.length === 0 ? this.current : JQFirst(element);
                $element && (this._designer.element.scrollTop(0).scrollTop($element.offset().top - 48));
            },
            moveUp: function () {
                /// <signature>
                /// <summary>Selects the parent of the current module.</summary>
                /// </signature>
                if (!this._isEditing()) {
                    return;
                }
                if (this.current) {
                    var parent = this._engine.context.findOwner(this.current);
                    parent && this.select(parent.element);
                }
            },
            load: function () {
                /// <signature>
                /// <summary>Restores selection was last saved.</summary>
                /// </signature>
                if (!this._isEditing()) {
                    return;
                }
                this._currentId && this.select($("#" + this._currentId, this._engine.context.activeContainer));
            },
            save: function () {
                /// <signature>
                /// <summary>Memorizes selection if the element has an ID.</summary>
                /// </signature>
                if (!this._isEditing()) {
                    return;
                }
                this._currentId = this.current && this.current.attr("id");
            },
            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                if (this._disposed) {
                    return;
                }
                this.clear();
                this._disposed = true;
            }
        })
    });

    _WinJS.Class.mix(ns.SelectionManager, _WinJS.Utilities.eventMixin);

})(WinJS, PI.Portal);