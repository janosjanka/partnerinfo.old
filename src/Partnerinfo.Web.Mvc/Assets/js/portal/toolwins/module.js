// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (Portal) {
    "use strict";

    var ElementRectSizer = WinJS.Class.define(function ElementRectSizer_ctor(element, styleName) {
        /// <signature>
        /// <summary>A helper class that can be used to resize a property of the given element.</summary>
        /// </signature>
        this._disposed = false;

        this.element = element[0];
        this.styleName = styleName;
        this.value = ko.observable();
        this.loadValue();
        this.valueSn = this.value.subscribe(this.saveValue, this);
    }, {
        loadValue: function () {
            /// <signature>
            /// <summary>Get CSS rect size info.</summary>
            /// </signature>
            this.value([
                this.element.style[this.styleName + "-left"] || ElementRectSizer.unspecified,
                this.element.style[this.styleName + "-top"] || ElementRectSizer.unspecified,
                this.element.style[this.styleName + "-right"] || ElementRectSizer.unspecified,
                this.element.style[this.styleName + "-bottom"] || ElementRectSizer.unspecified
            ].join(" "));
        },
        saveValue: function (value) {
            /// <signature>
            /// <summary>Occurs when margin changes.</summary>
            /// </signature>
            var rect = this._getSizeRect(value);
            this.element.style[this.styleName + "-left"] = rect.left;
            this.element.style[this.styleName + "-top"] = rect.top;
            this.element.style[this.styleName + "-right"] = rect.right;
            this.element.style[this.styleName + "-bottom"] = rect.bottom;
        },
        _getSizeValue: function (text) {
            /// <signature>
            /// <param name="text" type="String" />
            /// <returns type="String" />
            /// </signature>
            if (!text || text === ElementRectSizer.unspecified) {
                return "";
            }
            var parts = text.match(/^([+-]?(?:\d+|\d*\.\d+))([a-z]*|%)$/);
            if (parts && !parts[2]) {
                text += "px"; // Add a unit (px) to the number if that is not specified.
            }
            return text;
        },
        _getSizeRect: function (text) {
            /// <signature>
            /// <summary>Calculates rectangle sizes parsing the given text ( left, top, right, bottom )</summary>
            /// <param name="text" type="String" />
            /// <returns type="Object" />
            /// </signature>
            var rect = {
                left: null,
                top: null,
                right: null,
                bottom: null
            };

            if (!text) {
                return rect;
            }

            // Margin: 1px
            // Margin: 2px 4px
            // Margin: 3px 3px 5px 5px
            var segments = text.split(/\s+/g);

            switch (segments.length) {
                case 1:
                    rect.left = rect.top =
                    rect.right = rect.bottom = this._getSizeValue(segments[0]);
                    break;
                case 2:
                    rect.left = rect.right = this._getSizeValue(segments[0]);
                    rect.top = rect.bottom = this._getSizeValue(segments[1]);
                    break;
                default:
                    rect.left = this._getSizeValue(segments[0]);
                    rect.top = this._getSizeValue(segments[1]);
                    rect.right = this._getSizeValue(segments[2]);
                    rect.bottom = this._getSizeValue(segments[3]);
                    break;
            }

            return rect;
        },
        dispose: function () {
            /// <signature>
            /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            /// </signature>
            if (this._disposed) {
                return;
            }
            this.valueSn && this.valueSn.dispose();
            this.valueSn = null;
            this._disposed = true;
        }
    }, {
        unspecified: "-"
    });

    var ModuleOptions = WinJS.Class.define(function ModuleOptions_ctor(designer, element) {
        /// <signature>
        /// <summary>Initializes a new instance of the ModuleOptions class.</summary>
        /// <param name="designer" type="$.PI.PortalDesigner" />
        /// <returns type="ModuleOptions" />
        /// </signature>
        this._disposed = false;

        this.designer = designer;
        this.element = element;
        this.type = designer.options.engine.context.getTypeOf(this.element);
        this.instance = designer.options.engine.context.getInstanceOf(this.element);
        this.container = designer.options.engine.context.isContainer(this.element);

        var domElement = element[0];
        var moduleOptions = this.instance && this.instance.getModuleOptions() || {};

        this.id = ko.observable(domElement.id);
        this.active = ko.observable(moduleOptions.active);
        this.perm = ko.observable(moduleOptions.perm);
        this.permOptions = _T("portal.permOptions") || [];
        this.width = ko.observable(domElement.style.width);
        this.height = ko.observable(domElement.style.height);
        this.margin = new ElementRectSizer(this.element, "margin");
        this.padding = new ElementRectSizer(this.element, "padding");
        this.splitOptions = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

        this._locked = false;
        this._idSn = this.id.subscribe(this._onIdChanged, this);
        this._activeSn = this.active.subscribe(this._onActiveChanged, this);
        this._permSn = this.perm.subscribe(this._onPermChanged, this);
        this._widthSn = this.width.subscribe(this._onSizeChanged, this);
        this._heightSn = this.height.subscribe(this._onSizeChanged, this);
    }, {
        _onIdChanged: function (value) {
            /// <signature>
            /// <summary>Raised immediately after the property 'id' is changed.</summary>
            /// </signature>
            if (this._locked) {
                return;
            }
            var that = this;
            var oldId = this.element.attr("id");
            this.designer.options.module.renameAsync(this.element, value).then(
                function (id) {
                    that._locked = true;
                    that.id(id);
                    that._locked = false;
                },
                function () {
                    that._locked = true;
                    that.id(oldId);
                    that._locked = false;
                });
        },
        _onActiveChanged: function (value) {
            /// <signature>
            /// <summary>Raised immediately after the property 'active' is changed.</summary>
            /// </signature>
            this.instance && this.instance.updateModuleOptions({ active: value });
        },
        _onPermChanged: function (value) {
            /// <signature>
            /// <summary>Raised immediately after the property 'permission' is changed.</summary>
            /// </signature>
            this.instance && this.instance.updateModuleOptions({ perm: value });
        },
        _onSizeChanged: function () {
            /// <signature>
            /// <summary>Raised immediately after the properties 'width, height' are changed.</summary>
            /// </signature>
            this.element.css({
                width: this.width(),
                height: this.height()
            });
        },
        getElementSize: function () {
            /// <signature>
            /// <summary>Returns size info.</summary>
            /// <returns type="Object" />
            /// </signature>
            return {
                width: this.element.outerWidth(),
                height: this.element.outerHeight()
            };
        },
        dispose: function () {
            /// <signature>
            /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            /// </signature>
            if (this._disposed) {
                return;
            }
            this._idSn && this._idSn.dispose();
            this._activeSn && this._activeSn.dispose();
            this._permSn && this._permSn.dispose();
            this._heightSn && this._heightSn.dispose();
            this._widthSn && this._widthSn.dispose();
            this.margin && this.margin.dispose();
            this.padding && this.padding.dispose();

            this._idSn = null;
            this._activeSn = null;
            this._permSn = null;
            this._heightSn = null;
            this._widthSn = null;
            this.margin = null;
            this.padding = null;

            this._disposed = true;
        }
    });

    $.widget("PI.PortalModuleToolWin", $.PI.PortalToolWin, {
        options: {
            css: "ui-portal-toolwin-module"
        },
        _onRender: function () {
            /// <signature>
            /// <summary>Renders content.</summary>
            /// </signature>
            this.element.hide().css("height", "200px");
            this._onDesignerScrollBound = this._onDesignerScroll.bind(this);
            this.options.designer.element.on("scroll", this._onDesignerScrollBound);
            this.toolWinMenu.html(
"<table class=\"ui-table ui-table-sm\">\
    <colgroup>\
        <col \/>\
        <col style=\"width: 10px;\" \/>\
        <col \/>\
    <\/colgroup>\
    <tbody>\
        <tr>\
            <td role=\"toolbar\" colspan=\"3\" class=\"ui-toolbar\">\
                <div role=\"group\" class=\"ui-btn-group ui-btn-group-sm\">\
                    <button class=\"ui-btn ui-btn-flat\" type=\"button\" data-bind='click: designer.options.selection.moveUp.bind(designer.options.selection), clickBubble: false'>\
                        <i class=\"i back\"><\/i>\
                    <\/button>\
                <\/div>\
                <div role=\"group\" class=\"ui-btn-group ui-btn-group-sm ui-pull-right\">\
                    <button class=\"ui-btn ui-btn-flat\" type=\"button\" title=\"Mozgatás fel\" data-bind='click: designer.options.module.moveAsync.bind(designer.options.module, element, -1), clickBubble: false'>\
                        <i class=\"i previous\"><\/i>\
                    <\/button>\
                    <button class=\"ui-btn ui-btn-flat\" type=\"button\" title=\"Mozgatás le\" data-bind='click: designer.options.module.moveAsync.bind(designer.options.module, element, +1), clickBubble: false'>\
                        <i class=\"i next\"><\/i>\
                    <\/button>\
                    <button class=\"ui-btn ui-btn-flat\" type=\"button\" title=\"Szerkesztés\" data-bind='click: designer.options.module.editAsync.bind(designer.options.module, element), clickBubble: false'>\
                        <i class=\"i edit\"><\/i>\
                    <\/button>\
                    <button class=\"ui-btn ui-btn-flat\" type=\"button\" title=\"Másolás vágólapra\" data-bind='click: designer.options.module.copyAsync.bind(designer.options.module, element), clickBubble: false'>\
                        <i class=\"i copy\"><\/i>\
                    <\/button>\
                    <button class=\"ui-btn ui-btn-flat\" type=\"button\" title=\"Beillesztés vágólapról\" data-bind='click: designer.options.module.pasteAsync.bind(designer.options.module, element), clickBubble: false'>\
                        <i class=\"i paste\"><\/i>\
                    <\/button>\
                    <button class=\"ui-btn ui-btn-flat\" type=\"button\" title=\"Tartalom törlése\" data-bind='click: designer.options.module.emptyAsync.bind(designer.options.module, element), clickBubble: false'>\
                        <i class=\"i close\"><\/i>\
                    <\/button>\
                    <button class=\"ui-btn ui-btn-flat\" type=\"button\" title=\"Modul törlése\" data-bind='click: designer.options.module.removeAsync.bind(designer.options.module, element), clickBubble: false'>\
                        <i class=\"i delete\"><\/i>\
                    <\/button>\
                <\/div>\
            <\/td>\
        <\/tr>\
    <\/tbody>\
<\/table>");
            this.toolWinBody.html(
"<table class=\"ui-table ui-table-sm\">\
    <colgroup>\
        <col \/>\
        <col style=\"width: 10px;\" \/>\
        <col \/>\
    <\/colgroup>\
    <tbody>\
        <tr>\
            <td colspan=\"3\">\
                <input type=\"text\" spellcheck=\"false\" placeholder=\"Module neve\" data-bind='value: id' \/>\
            <\/td>\
        <\/tr>\
        <tr>\
            <td colspan=\"3\">\
                <select data-bind='value: perm, options: permOptions, optionsValue: \"id\", optionsText: \"name\"'><\/select>\
            <\/td>\
        <\/tr>\
        <tr>\
            <td>\
                <input type=\"text\" spellcheck=\"false\" placeholder=\"Szélesség\" data-bind='value: width' \/>\
            <\/td>\
            <td>x<\/td>\
            <td>\
                <input type=\"text\" spellcheck=\"false\" placeholder=\"Magasság\" data-bind='value: height' \/>\
            <\/td>\
        <\/tr>\
        <tr>\
            <td>\
                <input type=\"text\" spellcheck=\"false\" placeholder=\"Margó\" data-bind='value: margin.value' \/>\
            <\/td>\
            <td>&Dagger;<\/td>\
            <td>\
                <input type=\"text\" spellcheck=\"false\" placeholder=\"Behúzás\" data-bind='value: padding.value' \/>\
            <\/td>\
        <\/tr>\
    <\/tbody>\
<\/table>");
        },
        _onSelectionChanged: function (event) {
            /// <signature>
            /// <summary>Raised after modules are selected or deselected.</summary>
            /// </signature>
            if (this.options.designer.options.selection.current) {
                this.show(event.detail.element, event.detail.sourceElement);
            } else {
                this.hide();
            }
        },
        _onDesignerScroll: function () {
            /// <signature>
            /// <summary>Description</summary>
            /// </signature>
            this.alignTo(this._moduleElement, this._sourceElement);
        },
        alignTo: function ($element, $sourceElement) {
            /// <signature>
            /// <summary>Realigns the toolwin.</summary>
            /// </signature>
            if ($sourceElement) {
                this.element.position({
                    my: "left-" + (this.element.width() + 20) + " top",
                    at: "left top",
                    of: $sourceElement,
                    collision: "flip"
                });
                this.toolWinHead.trigger("mousedown");
                return;
            }
            if ($element) {
                this.element.position({
                    my: "center top+5",
                    at: "center bottom",
                    of: $element,
                    collision: "flip"
                });
            }
        },
        show: function ($element, $sourceElement) {
            /// <signature>
            /// <summary>Shows the tooltip.</summary>
            /// <param name="$element" type="jQuery" />
            /// <param name="$sourceElement" type="jQuery" optional="true" />
            /// </signature>
            this.hide();

            this._moduleOptions = new ModuleOptions(this.options.designer, $element);
            this._moduleElement = $element;
            this._sourceElement = $sourceElement;

            var size = this._moduleOptions.getElementSize();
            this._setOption("icon", this._moduleOptions.type.icon);
            this._setOption("title", String.format("{0} ({1}x{2})", this._moduleOptions.id(), size.width, size.height));

            ko.cleanNode(this.toolWinMenu[0]);
            ko.cleanNode(this.toolWinBody[0]);
            ko.applyBindings(this._moduleOptions, this.toolWinMenu[0]);
            ko.applyBindings(this._moduleOptions, this.toolWinBody[0]);

            this.element.show();
            this.alignTo($element, $sourceElement);
        },
        hide: function () {
            /// <signature>
            /// <summary>Hides the tooltip.</summary>
            /// </signature>
            this.element.hide();
            this._moduleOptions && this._moduleOptions.dispose();
            this._moduleOptions = null;
        },
        _destroy: function () {
            /// <signature>
            /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            /// </signature>
            this._onDesignerScrollBound && this.options.designer.element.off("scroll", this._onDesignerScrollBound);
            this._onDesignerScrollBound = null;

            ko.cleanNode(this.toolWinMenu[0]);
            ko.cleanNode(this.toolWinBody[0]);

            this._super();
        }
    });

})(PI.Portal);