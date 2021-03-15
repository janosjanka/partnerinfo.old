// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (_WinJS) {
    "use strict";

    function JQ(element) {
        /// <signature>
        /// <param name="element" type="HTMLElement" />
        /// <returns type="jQuery" />
        /// </signature>
        /// <signature>
        /// <param name="element" type="jQuery" />
        /// <returns type="jQuery" />
        /// </signature>
        return element.length ? element : $(element);
    }
    function isThemeClass(className) {
        /// <signature>
        /// <summary>Returns true if the given class name is a system class.</summary>
        /// <param name="className" type="String" />
        /// <returns type="Boolean" />
        /// </signature>
        return className
            && className.indexOf("ui-module") === -1
            && className.indexOf("ui-resizable") === -1
            && className.indexOf("ui-draggable") === -1;
    }
    function getThemeClass(className) {
        /// <signature>
        /// <summary>Converts the given class name to a theme class name, avoiding name collision.</summary>
        /// <param name="className" type="String" />
        /// <returns type="String" />
        /// </signature>
        if (className) {
            className = className.uri(false, '-');
            // Add a prefix 'ui-theme-' to the class name 
            // if the given name is not an empty string and does not start with that.
            //if (className && className.indexOf("ui-theme-") !== 0) {
            //    return className = "ui-theme-" + className;
            //}
        }
        return className;
    }
    function createElement(masterCode, contentCode) {
        /// <signature>
        /// <summary>Creates a style element with the given style codes.</summary>
        /// <param name="masterCode" type="String" />
        /// <param name="contentCode" type="String" />
        /// </signature>
        var code = "";
        masterCode && (code += masterCode);
        contentCode && (code += contentCode);
        var element = document.getElementById("ui-style");
        if (element) {
            element.innerHTML = code;
            return element;
        }
        element = document.createElement("style");
        element.id = "ui-style";
        element.type = "text/css";
        element.innerHTML = code;
        return document.body.appendChild(element);
    }

    var ns = _WinJS.Namespace.defineWithParent(PI, "Portal", {
        StyleManager: _WinJS.Class.define(function StyleManager_ctor(designer, options) {
            /// <signature>
            /// <summary>Initializes a new instance of the MenuManager class.</summary>
            /// <param name="designer" type="$.PI.PortalDesigner" optional="true" />
            /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the MenuManager.</param>
            /// <returns type="StyleManager" />
            /// </signature>
            options = options || {};
            this._disposed = false;
            this._element = null;
            this._masterCode = null;
            this._contentCode = null;
            this.designer = designer;
        }, {
            getMasterCode: function () {
                /// <signature>
                /// <summary>Gets the code for the master style element.</summary>
                /// <returns type="String" />
                /// </signature>
                return this._masterCode;
            },
            getContentCode: function () {
                /// <signature>
                /// <summary>Gets the code for the content style element.</summary>
                /// <returns type="String" />
                /// </signature>
                return this._contentCode;
            },
            setMasterCode: function (styleCode) {
                /// <signature>
                /// <summary>Renders the CSS code of the master page.</summary>
                /// <param name="styleCode" type="String" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                this._masterCode = styleCode;
            },
            setContentCode: function (styleCode) {
                /// <signature>
                /// <summary>Renders the CSS code of the content page.</summary>
                /// <param name="styleCode" type="String" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                this._contentCode = styleCode;
                this._element = createElement(this._masterCode, styleCode);
            },
            forEachRules: function (callback, thisArg) {
                /// <signature>
                /// <param name="callback" type="Function" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// </signature>
                if (this._element) {
                    var cssRules = this._element.sheet.cssRules;
                    for (var i = 0, len = cssRules.length; i < len; ++i) {
                        if (callback.apply(thisArg, [cssRules[i], i, len])) {
                            break;
                        }
                    }
                }
            },
            getClassListByDocument: function (document, classNamePart, limit) {
                /// <signature>
                /// <summary>Gets an array of CSS class names from the specified HTMLDocument.</summary>
                /// <param name="document" type="HTMLDocument" />
                /// <param name="classNamePart" type="String" optional="true" />
                /// <param name="limit" type="Number" optional="true" />
                /// <returns type="Array" />
                /// </signature>
                return this.getClassListByRuleList(document.styleSheets[0].rules || document.styleSheets[0].cssRules, classNamePart, limit);
            },
            getClassListByRuleList: function (cssRuleList, classNamePart, limit) {
                /// <signature>
                /// <summary>Gets an array of CSS class names from the specified CSSRuleList.</summary>
                /// <param name="cssRuleList" type="CSSRuleList" />
                /// <param name="classNamePart" type="String" optional="true" />
                /// <param name="limit" type="Number" optional="true" />
                /// <returns type="Array" />
                /// </signature>
                if (!cssRuleList) {
                    return [];
                }
                limit = limit || 50;
                var classList = [];
                for (var i = 0, len = cssRuleList.length; i < len && limit > 0; ++i) {
                    var cssRule = cssRuleList[i];
                    if (!cssRule.selectorText) {
                        continue;
                    }
                    var classNames = this.getClassListBySelectorText(cssRule.selectorText);
                    for (var c = 0, clen = classNames.length; c < clen; ++c) {
                        var className = classNames[c];
                        if ((classNamePart === undefined || classNamePart === null || className.indexOf(classNamePart) >= 0)
                            && classList.indexOf(className) < 0) {
                            classList.push(className);
                            limit -= 1;
                        }
                    }
                }
                return classList;
            },
            getClassListBySelectorText: function (selectorText) {
                /// <signature>
                /// <summary>Gets an array of CSS class names parsing the specified selectors.</summary>
                /// <param name="selectorText" type="String" />
                /// <returns type="Array" />
                /// </signature>
                if (!selectorText) {
                    return [];
                }
                var matches = selectorText.match(/\.[a-zA-Z0-9_-]+/g);
                if (!matches) {
                    return [];
                }
                var classNames = new Array(matches.length);
                for (var i = 0, len = matches.length; i < len; ++i) {
                    classNames[i] = matches[i].substr(1);
                }
                return classNames;
            },
            getClassList: function (element) {
                /// <signature>
                /// <summary>Gets an array of CSS class names applied to the given element.</summary>
                /// <param name="element" type="HTMLElement" />
                /// <returns type="Array" />
                /// </signature>
                /// <signature>
                /// <summary>Gets an array of CSS class names applied to the given element.</summary>
                /// <param name="element" type="jQuery" />
                /// <returns type="Array" />
                /// </signature>
                if (!element) {
                    return [];
                }
                element = element.length ? element[0] : element;
                if (!element || !element.className) {
                    return [];
                }
                var classNames = element.classList || element.className.match(/\S+/g);
                if (!classNames) {
                    return [];
                }
                var ret = [];
                for (var i = 0, len = classNames.length, className; i < len; ++i) {
                    if (isThemeClass(className = classNames[i])) {
                        ret.push(classNames[i]);
                    }
                }
                return ret;
            },
            hasClass: function (element, className) {
                /// <signature>
                /// <summary>Determines whether the specified element has the specified class.</summary>
                /// <param name="element" type="HTMLElement" />
                /// <param name="className" type="String" />
                /// <returns type="Boolean" />
                /// </signature>
                /// <signature>
                /// <summary>Determines whether the specified element has the specified class.</summary>
                /// <param name="element" type="jQuery" />
                /// <param name="className" type="String" />
                /// <returns type="Boolean" />
                /// </signature>
                className = getThemeClass(className);
                return JQ(element).hasClass(className);
            },
            addClass: function (element, className) {
                /// <signature>
                /// <summary>Adds the given class to the given element.</summary>
                /// <param name="element" type="HTMLElement" />
                /// <param name="className" type="String" />
                /// <returns type="String" />
                /// </signature>
                /// <signature>
                /// <summary>Adds the given class to the given element.</summary>
                /// <param name="element" type="jQuery" />
                /// <param name="className" type="String" />
                /// <returns type="String" />
                /// </signature>
                className = getThemeClass(className);
                className && JQ(element).addClass(className);
                return className;
            },
            removeClass: function (element, className) {
                /// <signature>
                /// <summary>Removes the given class from the given element.</summary>
                /// <param name="element" type="HTMLElement" />
                /// <param name="className" type="String" />
                /// </signature>
                /// <signature>
                /// <summary>Removes the given class from the given element.</summary>
                /// <param name="element" type="jQuery" />
                /// <param name="className" type="String" />
                /// </signature>
                className = getThemeClass(className);
                className && JQ(element).removeClass(className);
            },
            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                if (this._disposed) {
                    return;
                }
                this._disposed = true;
            }
        })
    });

    _WinJS.Class.mix(ns.StyleManager, _WinJS.Utilities.eventMixin);

})(WinJS);