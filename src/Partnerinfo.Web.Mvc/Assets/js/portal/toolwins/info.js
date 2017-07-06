// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="base.js" />

(function (window, $, _WinJS, _PI, undefined) {
    "use strict";

    function createElement(tagName, className, innerHtml) {
        var element = document.createElement(tagName);
        className && (element.className = className);
        innerHtml && (element.innerHTML = innerHtml);
        return element;
    }

    $.widget("PI.PortalInfoToolWin", $.PI.PortalToolWin, {
        options: {
            key: "portal.toolwin.info"
        },
        _init: function () {
            /// <signature>
            /// <summary>Initializes an existing instance of the tool window.</summary>
            /// </signature>
            this._super();
            this._properties = _T("portal.properties") || {};
            this._loadInfo(this.options.designer.options.selection.current);
        },
        _onRefresh: function () {
            /// <signature>
            /// <summary>Refreshes the snippet list.</summary>
            /// </signature>
            this._super();
            this._loadInfo(this.options.designer.options.selection.current);
        },
        _onSelectionChanged: function (event) {
            /// <signature>
            /// <param name="event" type="Event" />
            /// </signature>
            this._super(event);
            this._loadInfo(event.detail.element);
        },
        _loadInfo: function ($element) {
            /// <signature>
            /// <summary>Loads information about the given element.</summary>
            /// <param name="$element" type="jQuery" />
            /// </signature>
            this.toolWinBody.empty();
            var element = $element && $element[0];
            element && this._writeElement(this.toolWinBody[0], element);
        },
        _writeElement: function (element, what) {
            /// <signature>
            /// <param name="element" type="HTMLElement" />
            /// <param name="what" type="HTMLElement" />
            /// <returns type="String" />
            /// </signature>
            var manager = this.options.designer.options.style;
            var classes = manager.getClassList(what);
            var root = createElement("div", "ui-portal-toolwin-info-css");
            var title, item;
            for (var i = 0, len = classes.length; i < len; ++i) {
                var selector = "." + classes[i];
                title = createElement("div", "ui-portal-toolwin-info-title", selector + " {");
                item = createElement("div", "ui-portal-toolwin-info-item");
                this._writeCssRules(item, selector);
                item.innerHTML += "}";
                root.appendChild(title);
                root.appendChild(item);
            }
            if (what.style.length) {
                title = createElement("div", "ui-portal-toolwin-info-title", this.options.resources.inline + " {");
                item = createElement("div", "ui-portal-toolwin-info-item");
                this._writeStyleDecl(item, what.style);
                item.innerHTML += "}";
                root.appendChild(title);
                root.appendChild(item);
            }
            element.appendChild(root);
        },
        _writeCssRules: function (element, selector) {
            /// <signature>
            /// <param name="element" type="HTMLElement" />
            /// <param name="selector" type="Array" />
            /// </signature>
            this.options.designer.options.style.forEachRules(
                function (cssRule) {
                    if (cssRule.selectorText === selector) {
                        this._writeStyleDecl(element, cssRule.style);
                    }
                }, this);
        },
        _writeStyleDecl: function (element, style) {
            /// <signature>
            /// <param name="element" type="HTMLElement" />
            /// <param name="style" type="CSSStyleDeclaration" />
            /// <returns type="String" />
            /// </signature>
            for (var p in this._properties) {
                var v = style[p];
                if (v !== "") {
                    var item = element.appendChild(createElement("div", "ui-portal-toolwin-info-item"));
                    var title = item.appendChild(createElement("span", "ui-portal-toolwin-info-css-prop", this._properties[p]));
                    var value = item.appendChild(createElement("span", "ui-portal-toolwin-info-css-val", v));

                    title.setAttribute("title", p);
                    value.setAttribute("title", v);
                }
            }
        }
    });

})(window, jQuery, WinJS, PI);
