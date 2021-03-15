// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="http://cdnjs.cloudflare.com/ajax/libs/codemirror/4.8.0/codemirror.js" />
/// <reference path="base.js" />
/// <reference path="../../base/beautify/beautify-css.js" />

(function (_WinJS, _KO, _App) {
    "use strict";

    var ClassList = _WinJS.Class.derive(_KO.List, function ClassList_ctor(options) {
        /// <signature>
        /// <summary>Initializes a new instance of the ClassList class.</summary>
        /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the ClassList.</param>
        /// <returns type="ClassList" />
        /// </signature>
        options = options || {};
        this._designer = options.designer;
        this._element = options.element;
        this.reserved = ko.observable();
        _KO.List.call(this, options);
    }, {
        getDocumentClasses: function (query) {
            /// <signature>
            /// <summary>Returns an array of available document classes.</summary>
            /// <param name="query" type="String" />
            /// <returns type="Array" />
            /// </signature>
            return this._designer.options.style.getClassListByDocument(document, query, 25);
        },
        element: function (element) {
            /// <signature>
            /// <summary>Gets the element.</summary>
            /// <returns type="HTMLElement" />
            /// </signature>
            /// <signature>
            /// <summary>Sets the given element.</summary>
            /// <param name="element" type="HTMLElement" />
            /// </signature>
            if (arguments.length === 0) {
                return this._element;
            }
            this._element = element;
            this.refresh();
        },
        submit: function () {
            /// <signature>
            /// <summary>Adds the reserved class name.</summary>
            /// </signature>
            var reserved = this.reserved();
            if (reserved) {
                var className = this._designer.options.style.addClass(this._element, reserved);
                className && this.push(className);
                this.reserved(null);
            }
        },
        deleteName: function (className) {
            /// <signature>
            /// <summary>Deletes the given class name.</summary>
            /// <param name="item" type="ClassName" />
            /// </signature>
            if (className) {
                this._designer.options.style.removeClass(this._element, className);
                this.remove(className);
            }
        },
        refresh: function () {
            /// <signature>
            /// <summary>Refreshes the list.</summary>
            /// </signature>
            this.replaceAll.apply(this, this._designer.options.style.getClassList(this._element));
        }
    });

    $.widget("PI.PortalStyleToolWin", $.PI.PortalToolWin, {
        options: {
            css: "ui-portal-toolwin-style",
            key: "portal.toolwin.style",
            state: {
                left: 0,
                top: 0
            }
        },
        viewType: {
            inline: 1,
            global: 2
        },
        loadStyleCode: function () {
            /// <signature>
            /// <summary>Loads CSS class names into the code editor.</summary>
            /// </signature>
            this.styleCode(this.options.designer.options.style.getContentCode() || null);
        },
        formatStyleCode: function () {
            /// <signature>
            /// <summary>Formats the style code.</summary>
            /// </signature>
            var css = this.styleCode();
            this.styleCode(css ? css_beautify(css) : null);
        },
        saveStyleCode: function () {
            /// <signature>
            /// <summary>Loads CSS class names into the code editor.</summary>
            /// </signature>
            this.options.designer.options.style.setContentCode(this.styleCode() || null);
        },
        _initAsync: function () {
            /// <signature>
            /// <summary>Initializes a new instance of the tool window. Add initialization logic to this function.</summary>
            /// <returns type="WinJS.Promise" />
            /// </signature>
            this.viewId = ko.observable().extend({ rateLimit: 1 });
            this.view = ko.observable();
            this.viewSn = this.view.subscribe(this._viewChanged, this);
            this.view(this.viewType.inline);
            this.classList = new ClassList({ designer: this.options.designer });
            this.styleCode = ko.observable();
            this.styleCodeSn = this.styleCode.subscribe(this._styleCodeChanged, this);
            this.cssEditorOptions = ko.observable({ selected: this.options.designer.options.selection.current });

            var that = this;
            return require(["portal/toolwins/style.html"]).then(function () {
                ko.renderTemplate("koPageToolStyleMenu", that, null, that.toolWinMenu[0]);
                ko.renderTemplate("koPageToolStyleBody", that, null, that.toolWinBody[0]);
            });
        },
        _viewChanged: function (value) {
            /// <signature>
            /// <summary>Raised immediately after the view is changed.</summary>
            /// <param name="value" type="Number" />
            /// </signature>
            if (value === this.viewType.inline) {
                this.viewId("koPageToolStyleInlineView");
            } else {
                var that = this;
                require(["codemirror", "codemirror.css"]).then(function () {
                    that.loadStyleCode();
                    that.viewId("koPageToolStyleGlobalView");
                });
            }
        },
        _styleCodeChanged: function () {
            /// <signature>
            /// <summary>Raised immediately after the style code is changed.</summary>
            /// </signature>
            this.saveStyleCode();
        },
        _onRefresh: function () {
            /// <signature>
            /// <summary>Refreshes the control.</summary>
            /// </signature>
            this._super();
            var selected = this.options.designer.options.selection.current;
            this.classList.element(selected);
            this.cssEditorOptions({ selected: selected });
            this.loadStyleCode();
        },
        _onSelectionChanged: function (event) {
            /// <signature>
            /// <param name="event" type="Event" />
            /// </signature>
            this._super(event);
            var selected = event.detail.element;
            this.classList.element(selected);
            this.cssEditorOptions({ selected: selected });
        },
        _destroy: function () {
            /// <signature>
            /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            /// </signature>
            this.styleCodeSn && this.styleCodeSn.dispose();
            this.styleCodeSn = null;
            this.viewSn && this.viewSn.dispose();
            this.viewSn = null;

            ko.cleanNode(this.toolWinBody[0]);

            this._super();
        }
    });

})(WinJS, WinJS.Knockout, PI);
