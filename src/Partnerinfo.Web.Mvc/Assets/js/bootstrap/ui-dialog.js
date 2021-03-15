// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function ($) {
    "use strict";

    $.widget("WinJS.dialog", $.ui.dialog, {
        options: {
            progressClass: "ajax-loader",
            content: null,
            autoOpen: true,
            closeOnEscape: true,
            /* closeOnOverlayClick: true, */
            destroyOnClose: true,
            modal: true,
            resizable: false,
            overflow: "auto",
            minHeight: 150,
            minWidth: 267,
            height: "auto"
        },
        _create: function () {
            /// <signature>
            /// <summary>Constructor</summary>
            /// </signature>
            this.options.title = this.options.title || window.document.title;
            this.options.content && this.element.html(this.options.content);
            this._ensureButton();
            this._super();
            this._setupOverflow(this.options.overflow);
            this._on({ keydown: this._onkeydown });
        },
        _allowInteraction: function (event) {
            /// <signature>
            /// <summary>Allows interaction in modal dialogs</summary>
            /// <returns type="Boolean" />
            /// </signature>
            if ($(event.target).closest(".mce-window").length) { // TinyMCE hack
                return true;
            }
            return this._super(event);
        },
        /*
        _createOverlay: function () {
            /// <signature>
            /// <summary>Creates an overlay element for modal dialogs</summary>
            /// </signature>
            this._super();
            if (this.options.closeOnOverlayClick) {
                var that = this;
                this._on(this.overlay, {
                    click: function () {
                        that.close();
                    }
                });
            }
        },
        */
        _ensureButton: function () {
            /// <signature>
            /// <summary>Ensures at least one OK button for the dialog</summary>
            /// </signature>
            var that = this;
            var buttons = this.options.buttons;
            if ($.isArray(buttons) && !buttons.length) {
                this.options.buttons = [{
                    "class": "ui-btn-primary",
                    "text": "OK",
                    "click": function () { that.close(); }
                }];
            }
        },
        _setOption: function (key, value) {
            /// <signature>
            /// <summary>Updates a dialog option</summary>
            /// </signature>
            this._super(key, value);
            if (key === "overflow") {
                this._setupOverflow(value);
            }
        },
        _setupOverflow: function (value) {
            /// <signature>
            /// <summary>Allows scrolling</summary>
            /// </signature>
            this.element.css("overflow", value);
        },
        _onkeydown: function (event) {
            /// <signature>
            /// <summary>Raised when a user presses a key.</summary>
            /// <param name="event" type="$.Event" />
            /// <returns type="Boolean" />
            /// </signature>
            if (event.keyCode === $.ui.keyCode.ENTER && !(event.altKey || event.ctrlKey || event.shiftKey)) {
                var button = $(".ui-dialog-buttonpane button.ui-btn-primary:first", this.element.closest(".ui-dialog"));
                if (button.length) {
                    event.stopPropagation();
                    var activeElement = document.activeElement;
                    activeElement && activeElement.blur(); // Update binding (angular, knockout, etc.)
                    button.trigger("click");
                    activeElement && activeElement.focus(); // Revert focus (it does not work with async event handlers)
                    return false;
                }
            }
        },
        focus: function (selector) {
            /// <signature>
            /// <summary>Causes the element to receive the focus and executes the code specified by the onfocus event.</summary>
            /// </signature>
            /// <signature>
            /// <summary>Causes the element to receive the focus and executes the code specified by the onfocus event.</summary>
            /// <param name="selector" type="String">jQuery selector.</param>
            /// </signature>
            if (this.element) {
                selector = selector || "[name]";
                if (!$(selector + ":not(:hidden):focus", this.element).length) {
                    var input = $(selector + ":not(:hidden):first", this.element);
                    if (input.length) {
                        input.focus();
                        return true;
                    }
                }
            }
            return false;
        },
        progress: function (valueOrPromise) {
            /// <signature>
            /// <summary>Displays a progress bar on the dialog.</summary>
            /// <param name="value" type="Boolean">A value indicating whether the progress bar is visible.</param>
            /// </signature>
            /// <signature>
            /// <summary>Displays a progress bar on the dialog.</summary>
            /// <param name="promise" type="$.Deferred">A value indicating whether the progress bar is visible.</param>
            /// </signature>
            if ($.isFunction(valueOrPromise.always)) {
                this.progress(true);
                var that = this;
                valueOrPromise.always(function () {
                    that.progress(false);
                });
            } else {
                this._progress = this._progress || $("<div style='hidden'>")
                    .addClass(this.options.progressClass)
                    .appendTo(this.element.parent(".ui-dialog"));
                valueOrPromise
                    ? this._progress.show()
                    : this._progress.hide();
            }
        },
        realign: function () {
            /// <signature>
            /// <summary>Realigns the dialog window according to the given position</summary>
            /// </signature>
            this.widget().position(this.options.position);
        },
        close: function () {
            /// <signature>
            /// <summary>Closes this dialog</summary>
            /// </signature>
            this._super();
            this.options.destroyOnClose && this._destroy();
        }
    });

})(jQuery);