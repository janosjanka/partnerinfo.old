// SearchBox plugin v1.0
// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (globalize, ko) {
    "use strict";

    function delValue() {
        ko.isWritableObservable(this.value) ? this.value(null) : this.value = null;
    }
    function hasValue() {
        return !!ko.unwrap(this.value);
    }

    ko.components.register("ui-searchbox", {
        viewModel: function (params) {
            /// <signature>
            /// <summary>Creates a viewModel for a search box</summary>
            /// </signature>
            this.value = params.value;
            this.attr = params.attr || {};
            this.attr.tooltip = this.attr.tooltip || params.tooltip;
            this.attr.placeholder = this.attr.placeholder || params.placeholder || Globalize.localize("ui/searchBoxPlaceholder");
            this.width = params.width || "241px";
            this.menu = params.menu;
            this.hasFocus = params.hasFocus;
            this.canSubmit = params.canSubmit || ko.pureComputed(hasValue, this);
            this.submit = params.submit || delValue.bind(this);
            this.canCancel = params.canCancel || ko.pureComputed(hasValue, this);
            this.cancel = params.cancel || delValue.bind(this);
            if (this.menu) {
                this.menu.width = this.menu.width || "316px";
                this.menu.options = this.menu.options || {};
                this.menu.options.align = this.menu.options.align || "left";
                this.menu.options.autoClose = !!this.menu.options.autoClose;
            }
        },
        template:
            '<div role="search" class="ui-search" data-bind=\'css: { "ui-state-focused": hasFocus, "ui-state-filtered": canCancel }\'>' +
                '<!-- ko if: menu -->' +
                '<span class="ui-search-menu">' +
                    '<div role="menu" class="ui-dropdown" data-bind=\'dropdown: menu.options\'>' +
                        '<button class="ui-btn" type="button"><i></i></button>' +
                        '<ul role="menu" class="ui-menu" data-bind="style: { width: menu.width }">' +
                            '<li role="menuitem">' +
                                '<form class="ui-search-form" data-bind=\'submit: submit\'>' +
                                    '<div data-bind="template: { name: menu.content, data: menu.data }"></div>' +
                                    '<input class="ui-search-form-submit" type="submit" />' +
                                '</form>' +
                            '</li>' +
                            '<!-- ko if: menu.footer -->' +
                            '<li role="menuitem" class="menuFooter" data-bind="template: { name: menu.footer }"></li>' +
                            '<!-- /ko -->' +
                        '</ul>' +
                    '</div>' +
                '</span>' +
                '<!-- /ko -->' +
                '<span class="ui-search-input">' +
                    '<form data-bind=\'submit: submit\'>' +
                        '<input class="ui-search-value" type="text" data-bind=\'value: value, attr: attr, style: { width: width }, hasFocus: hasFocus\' />' +
                        '<input class="ui-search-submit" type="submit" value="" />' +
                        '<input class="ui-search-clear" type="button" value="" data-bind=\'click: cancel, clickBubble: false\' />' +
                    '</form>' +
                '</span>' +
              '</div>'
    });

})(Globalize, ko);