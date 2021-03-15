// ToggleButton plugin v1.0
// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function ($, ko, undefined) {
    "use strict";

    function ToggleButton(element, options) {
        /// <signature>
        /// <summary>Creates a viewModel that represents a ToggleButton control.</summary>
        /// <param name="element" type="HTMLElement" />
        /// <param name="options" type="Object" />
        /// <returns type="ToggleButton" />
        /// </signature>
        options = options || {};
        this._disposed = false;
        this.element = $(element).addClass("ui-toggle");
        this.enable = ko.isObservable(options.enable) ? options.enable : ko.isWritableObservable(options.checked);
        this.checked = options.checked;
        this.option = options.option !== undefined ? options.option : true;
        this.nullable = options.nullable;
        this.icon = options.icon;
        this.text = options.text;

        this.hasFocus = options.hasFocus;
        this.clickBubble = options.clickBubble;
        this.attr = options.attr || {};
        this.attr["role"] = this.option === true ? "checkbox" : "radio";
        this.attr["title"] = this.attr.title || options.tooltip;
        this.attr["aria-checked"] = ko.pureComputed(this.getAriaChecked, this);
        this.css = options.css || { "ui-btn-checkbox": true };
        this.checkbox = options.checkbox !== undefined ? options.checkbox : true;
    }
    ToggleButton.prototype.getAriaChecked = function () {
        /// <signature>
        /// <summary>Description</summary>
        /// </signature>
        if (!ko.unwrap(this.enable)) {
            return;
        }
        var checked = this.checked();
        if (checked === null || checked === undefined) {
            return "undefined";
        }
        return checked === this.option ? "true" : "false";
    };
    ToggleButton.prototype.toggle = function () {
        /// <signature>
        /// <summary>Description</summary>
        /// </signature>
        if (!ko.unwrap(this.enable)) {
            return;
        }
        if (this.option === true) {
            var checked = this.checked();
            if (this.nullable) {
                if (checked === null || checked === undefined) {
                    checked = true;
                } else if (checked === true) {
                    checked = false;
                } else {
                    checked = null;
                }
            } else {
                checked = !checked;
            }
            this.checked(checked);
        } else {
            this.checked(this.option);
        }
    };
    ToggleButton.prototype.dispose = function () {
        /// <signature>
        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        /// </signature>
        if (this._disposed) {
            return;
        }
        this.attr["aria-checked"] && this.attr["aria-checked"].dispose();
        this.attr["aria-checked"] = null;
        this.element = null;
        this._disposed = true;
    };

    ko.components.register("ui-toggle", {
        viewModel: {
            createViewModel: function (params, info) {
                return new ToggleButton(info.element, params);
            }
        },
        template:
            '<button class="ui-btn" type="button" data-bind="attr: attr, css: css, enable: enable, hasFocus: hasFocus, click: toggle, clickBubble: clickBubble">' +
                '<!-- ko if: checkbox --><span class="ui-checkbox"><i></i></span><!-- /ko -->' +
                '<!-- ko if: icon --><i data-bind="attr: { class: icon }"></i><!-- /ko -->' +
                '<!-- ko if: text --><span data-bind="text: text"></span><!-- /ko -->' +
            '</button>'
    });

})(jQuery, ko);