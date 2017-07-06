// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (global, $, ko) {
    "use strict";

    $.widget("ui.dropdown", {
        options: {
            /// <field type="Boolean">
            /// Opens the dropdown when the widget is initialized.
            /// </field>
            autoOpen: false,

            /// <field type="Boolean">
            /// Closes the dropdown when a click occurs out of the scope.
            /// </field>
            autoClose: true,

            /// <field type="String">
            /// Menu alignment.
            /// </field>
            align: "center",

            /// <field type="String">
            /// A list of controls which are ignored and insensitive.
            /// </field>
            ignored: "input,textarea,select",

            /// <field type="Function">
            /// Occurs when the dropdown is opened.
            /// </field>
            render: null,

            /// <field type="Function">
            /// Occurs when the dropdown is opened.
            /// </field>
            open: null,

            /// <field type="Function">
            /// Occurs when the dropdown is closed.
            /// </field>
            close: null
        },
        _create: function () {
            /// <signature>
            /// <summary>Constructor.</summary>
            /// </signature>
            this.element.addClass("ui-dropdown");
            this.uiButton = $(">.ui-btn", this.element).attr("aria-haspopup", true).attr("aria-expanded", false);
            this.uiMenu = $(">.ui-menu", this.element).attr("aria-labelledby", this.uiButton.attr("id"));

            this._isOpen = false;
            this._isRendered = false;
            this._renderCompleteBound = this.realign.bind(this);
            this._createPosition(this.options.align);
        },
        _init: function () {
            /// <signature>
            /// <summary>Initializer.</summary>
            /// </signature>
            this.options.autoOpen && this.open();
        },
        _destroy: function () {
            /// <signature>
            /// <summary>Destructor.</summary>
            /// </signature>
            this.close();
            this.element.removeClass("ui-dropdown");
            this.uiMenu = null;
            this.uiButton = null;
        },
        _setOption: function (key, value) {
            /// <signature>
            /// <param name="key" type="String" />
            /// <param name="value" type="Object" />
            /// </signature>
            this._super(key, value);

            if (key === "align") {
                this._createPosition(value);
                this.realign();
            }
        },
        _createPosition: function (align) {
            /// <signature>
            /// </signature>
            if (align === "left") {
                this._position = {
                    my: "left top",
                    at: "left bottom-1",
                    of: this.uiButton,
                    collision: "fit"
                };
            } else if (align === "right") {
                this._position = {
                    my: "right top",
                    at: "right bottom-1",
                    of: this.uiButton,
                    collision: "fit"
                };
            } else {
                this._position = {
                    my: "center top",
                    at: "center bottom-1",
                    of: this.uiButton,
                    collision: "fit"
                };
            }
        },
        isOpen: function () {
            /// <signature>
            /// <summary>Returns true if this dropdown is open.</summary>
            /// <returns type="Boolean" />
            /// </signature>
            return this._isOpen;
        },
        isRendered: function () {
            /// <signature>
            /// <summary>Returns true if this dropdown is rendered.</summary>
            /// <returns type="Boolean" />
            /// </signature>
            return this._isRendered;
        },
        open: function () {
            /// <signature>
            /// <summary>Opens this dropdown.</summary>
            /// </signature>
            if (this._isOpen) {
                return;
            }
            this._isOpen = true;
            this.element.addClass("ui-open");
            this.uiButton.attr("aria-expanded", true);
            this.realign();

            this._trigger("open");

            if (this._isRendered) {
                return;
            }
            this._trigger("render", null, this._renderCompleteBound);
            this._isRendered = true;
        },
        close: function () {
            /// <signature>
            /// <summary>Closes this dropdown.</summary>
            /// </signature>
            if (!this._isOpen) {
                return;
            }
            this._isOpen = false;
            this.element.removeClass("ui-open");
            this.uiButton.attr("aria-expanded", false);
            this._trigger("close");
        },
        toggle: function () {
            /// <signature>
            /// <summary>Toggles state of this dropdown.</summary>
            /// </signature>
            this._isOpen ? this.close() : this.open();
        },
        realign: function () {
            /// <signature>
            /// <summary>Realigns the dialog window according to the given position</summary>
            /// </signature>
            this.uiMenu.position(this._position);
        },
        button: function () {
            /// <signature>
            /// <summary>Returns the button element.</summary>
            /// <returns type="jQuery" />
            /// </signature>
            return this.uiButton;
        },
        menu: function () {
            /// <signature>
            /// <summary>Returns the menu element.</summary>
            /// <returns type="jQuery" />
            /// </signature>
            return this.uiMenu;
        }
    });

    $.ui.dropdown.closeAll = function () {
        /// <signature>
        /// <summary>Closes all the dropdown menus within the document.</summary>
        /// </signature>
        $(".ui-dropdown.ui-open").dropdown("close");
    };

    $.ui.dropdown.closeAllExcept = function (selector) {
        /// <signature>
        ///   <param name="selector" type="String">Remove elements from the set of matched elements.</param>
        /// </signature>
        /// <signature>
        ///   <param name="elements" type="Array">Remove elements from the set of matched elements.</param>
        /// </signature>
        /// <signature>
        ///   <param name="function(index)" type="Function">Remove elements from the set of matched elements.</param>
        /// </signature>
        /// <signature>
        ///   <param name="jQuery object" type="PlainObject">Remove elements from the set of matched elements.</param>
        /// </signature>
        $(".ui-dropdown.ui-open").not(selector).dropdown("close");
    };

    //
    // Knockout Binding Handler
    //

    function JQueryCtx(element) {
        /// <signature>
        /// <summary>Returns a jQuery instance using the context of the specified element.</summary>
        /// <param name="element" type="HTMLElement" />
        /// <returns type="jQuery" />
        /// </signature>
        return ((element.ownerDocument.defaultView || element.ownerDocument.parentWindow || global).$ || $)(element);
    }

    ko.bindingHandlers.dropdown = {
        init: function (element, valueAccessor) {
            /// <signature>
            /// <summary>This will be called when the binding is first applied to an element.
            /// Set up any initial state, event handlers, etc. here.</summary>
            /// </signature>
            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                JQueryCtx(element).dropdown("destroy");
            });
        },
        update: function (element, valueAccessor, allBindingsAccessor) {
            /// <signature>
            /// <summary>This will be called once when the binding is first applied to an element
            /// and again whenever the associated observable changes value.
            /// Update the DOM element based on the supplied values here.</summary>
            /// <param name="element" type="HTMLElement" />
            /// </signature>
            JQueryCtx(element).dropdown(valueAccessor());
        }
    };

    //
    // Global Event Handler
    //

    function clickHandler(event) {
        /// <signature>
        /// <param name="eventObject" type="MouseEvent" />
        /// <returns type="Boolean" />
        /// </signature>
        var $target = $(event.target);
        var $dropdowns = $target.parents(".ui-dropdown");
        if ($dropdowns.length === 0) {
            $.ui.dropdown.closeAll();
            return;
        }
        $.ui.dropdown.closeAllExcept($dropdowns);
        var $dropdown = $dropdowns.eq(0);
        if ($dropdown.dropdown("option", "autoClose")) {
            if ($target.is($dropdown.dropdown("option", "ignored"))) {
                return;
            }
            $dropdown.dropdown("toggle");
            return;
        }
        var dropdownBtn = $dropdown.dropdown("button")[0];
        if (dropdownBtn === event.target || $.contains(dropdownBtn, event.target)) {
            $dropdown.dropdown("toggle");
        } else {
            $dropdown.dropdown("open");
        }
    }

    $(global.document).on("click.ui-dropdown", clickHandler);

})(window, jQuery, ko);
