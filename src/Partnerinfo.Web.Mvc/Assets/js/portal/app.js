// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="_references.js" />

(function (_Global, _WinJS) {
    "use strict";

    var _Utilities = _WinJS.Utilities;

    var ui = PI.ui;
    var PortalController = WinJS.Class.define(function (element, options) {
        /// <signature>
        /// <summary>Creates a new PortalController control.</summary>
        /// <param name="element" type="HTMLElement" domElement="true">
        /// The DOM element that will host the PortalController control.
        /// </param>
        /// <param name="options" type="Object" optional="true">
        /// An object that contains one or more property/value pairs to apply to the new control.
        /// Each property of the options object corresponds to one of the control's properties or events.
        /// </param>
        /// <returns type="PortalController" />
        /// </signature>
        this._options = options || {};
        this.element = $(element);
        this.toolbar1 = $(this._options.toolbar1).first();
        this.toolbar2 = $(this._options.toolbar2).first();
        this.sidebar = $(this._options.sidebar).first();
        this.model = { context: null, portal: null };
    }, {
        portals: function (context) {
            /// <signature>
            /// <summary>Renders the view that matches the action result name</summary>
            /// </signature>
            var that = this;
            this._setOptions(context).then(function () {
                ui({
                    name: "portals",
                    element: that.element,
                    menu: that.sidebar
                });
            });
        },
        create: function (context) {
            /// <signature>
            /// <summary>Renders the view that matches the action result name</summary>
            /// </signature>
            var that = this;
            this._setOptions(context, { css: { width: "640px" } }).then(function () {
                ui({
                    name: "portal",
                    element: that.element,
                    toolbar: that.toolbar1,
                    params: {
                        onsaved: function (event) {
                            _Utilities.openLink("/admin/portals/{portal}/#/pages", { portal: event.detail.data.uri });
                        },
                        ondiscarded: function () {
                            _Global.history.back();
                        }
                    }
                });
            });
        },
        settings: function (context) {
            /// <signature>
            /// <summary>Renders the view that matches the action result name</summary>
            /// </signature>
            var that = this;
            this._setOptions(context, { css: { width: "640px" } }).then(function () {
                ui({
                    name: "portal",
                    element: that.element,
                    toolbar: that.toolbar1,
                    params: {
                        portal: that.model.portal,
                        onsaved: function (event) {
                            _Utilities.openLink("/admin/portals/{portal}/#/pages", { portal: event.detail.data.uri });
                        },
                        ondiscarded: function () {
                            _Global.history.back();
                        }
                    }
                });
            });
        },
        sharing: function (context) {
            /// <signature>
            /// <summary>Renders the view that matches the action result name</summary>
            /// </signature>
            var that = this;
            this._setOptions(context, { css: { width: "640px" } }).then(function () {
                ui({
                    name: "security.acl",
                    element: that.element,
                    params: {
                        objectType: "portal",
                        objectId: that.model.portal.id
                    }
                });
            });
        },
        hosting: function (context) {
            /// <signature>
            /// <summary>Renders the view that matches the action result name</summary>
            /// </signature>
            var that = this;
            this._setOptions(context, { css: { width: "640px" } }).then(function () {
                ui({
                    name: "portal.host",
                    element: that.element,
                    menu: that.toolbar1,
                    params: {
                        portal: that.model.portal
                    }
                });
            });
        },
        pages: function (context) {
            /// <signature>
            /// <summary>Renders the view that matches the action result name</summary>
            /// </signature>
            var that = this;
            this._setOptions(context, { css: { width: "940px" } }).then(function () {
                ui({
                    name: "portal.pages",
                    element: that.element,
                    menu: that.toolbar1,
                    params: {
                        portal: that.model.portal
                    }
                });
            });
        },
        media: function (context) {
            /// <signature>
            /// <summary>Renders the view that matches the action result name</summary>
            /// </signature>
            var that = this;
            this._setOptions(context, { css: { width: "940px" } }).then(function () {
                ui({
                    name: "portal.media",
                    element: that.element,
                    menu: that.toolbar1,
                    params: {
                        query: {
                            portal: that.model.portal,
                            orderBy: "recent"
                        }
                    }
                });
            });
        },
        designer: function (context) {
            /// <signature>
            /// <summary>Renders the view that matches the action result name</summary>
            /// </signature>
            var that = this;
            var url = this._options.root + '/0/designer?' + _Utilities.serializeParams({
                toolbar1: this._options.toolbar1,
                toolbar2: this._options.toolbar2,
                portalUri: context.params.splat[0],
                pageUri: context.params.splat[1]
            });
            this._setOptions(context).then(function () {
                require(["portal/designer.css"]).then(function () {
                    that.element.html($("<iframe>").attr({
                        "id": "designer",
                        "class": "ui-portal-host",
                        "src": url,
                        "frameborder": "0",
                        "marginheight": "0",
                        "marginwidth": "0",
                        "scrolling": "no",
                        "seamless": "seamless"
                    }));
                });
            });
        },
        _executeCore: function (callback) {
            /// <signature>
            /// <param name="callback" type="Function" />
            /// </signature>
            require(["portal", "portal.css"]).then(callback);
        },
        _setOptions: function (context, options) {
            /// <signature>
            /// <summary>Resets the menu, sidebar, content of the container element.</summary>
            /// <param name="context" type="Object" />
            /// <param name="options" type="Object" optional="true" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            var that = this;
            options = options || {};
            return _WinJS.Promise(
                function (completeDispatch, errorDispatch) {
                    /// <signature>
                    /// <summary>Updates the model in an async way.</summary>
                    /// </signature>
                    that.model.context = context;
                    if (!context.params.portalUri) {
                        that.model.portal = null;
                        completeDispatch();
                        return;
                    }
                    if (that.model.portal) {
                        completeDispatch();
                        return;
                    }
                    PI.Portal.PortalService.getByUriAsync(context.params.portalUri, "none").then(
                        function (portal) {
                            that.model.portal = portal;
                            completeDispatch();
                        },
                        function () {
                            that.model.portal = null;
                            errorDispatch();
                        });
                }).then(function () {
                    /// <signature>
                    /// <summary>Renders UI.</summary>
                    /// </signature>
                    var element = that.element[0];
                    var toolbar1 = that.toolbar1[0];
                    var toolbar2 = that.toolbar2[0];
                    var sidebar = that.sidebar[0];
                    toolbar1 && ko.cleanNode(toolbar1);
                    toolbar2 && ko.cleanNode(toolbar2);
                    sidebar && ko.cleanNode(sidebar);
                    element && ko.cleanNode(element);
                    that.toolbar1.empty();
                    that.toolbar2.empty();
                    that.sidebar.empty();
                    that.element.empty();
                    that.sidebar && that._renderSidebar(context, that.model, that.sidebar);
                    that.toolbar2 && that._renderInfobar(context, that.model, that.toolbar2);
                    options.css && that.element.css(options.css);
                });
        },
        _renderSidebar: function (context, model, element) {
            /// <signature>
            /// <summary>Renders the portal menu.</summary>
            /// <param name="context" type="Object" />
            /// <param name="model" type="Object" />
            /// <param name="element" type="jQuery" />
            /// </signature>
            if (context.params.portalUri) {
                element.html(
                    '<div class="ui-container">' +
                        '<ul role="menu" class="ui-navbar-vl">' +
                            '<li>' +
                                '<a data-bind=\'click: function (d) { d.context.redirect("admin/portals"); }, clickBubble: false\'>' +
                                    '<i class="i previous"></i>' +
                                    '<span>Portálok</span>' +
                                '</a>' +
                            '</li>' +
                            '<li>' +
                                '<a data-bind=\'click: function (d) { d.context.redirect("admin/portals", d.context.params.portalUri, "#/pages"); }, clickBubble: false\'>' +
                                    '<i class="i page"></i>' +
                                    '<span>Weboldalak</span>' +
                                '</a>' +
                            '</li>' +
                            '<li>' +
                                '<a data-bind=\'click: function (d) { d.context.redirect("admin/portals", d.context.params.portalUri, "#/media"); }, clickBubble: false\'>' +
                                    '<i class="i drive"></i>' +
                                    '<span>Média</span>' +
                                '</a>' +
                            '</li>' +
                            '<li>' +
                                '<a data-bind=\'click: function (d) { d.context.redirect("admin/portals", d.context.params.portalUri, "#/settings"); }, clickBubble: false\'>' +
                                    '<i class="i option"></i>' +
                                    '<span>Beállítások</span>' +
                                '</a>' +
                            '</li>' +
                            '<li>' +
                                '<a data-bind=\'click: function (d) { d.context.redirect("admin/portals", d.context.params.portalUri, "#/sharing"); }, clickBubble: false\'>' +
                                    '<i class="i share"></i>' +
                                    '<span>Megosztás</span>' +
                                '</a>' +
                            '</li>' +
                            '<li>' +
                                '<a data-bind=\'click: function (d) { d.context.redirect("admin/portals", d.context.params.portalUri, "#/hosting"); }, clickBubble: false\'>' +
                                    '<i class="i page"></i>' +
                                    '<span>Hosztolás</span>' +
                                '</a>' +
                            '</li>' +
                        '</ul>' +
                    '</div>'
                );
            } else {
                element.html(
                    '<div class="ui-container">' +
                        '<div role="group" class="ui-btn-group">' +
                            '<button class="ui-btn ui-btn-primary" type="button" data-bind=\'click: function (d) { d.context.redirect("admin/portals/#/create"); }, clickBubble: false\'>' +
                                '<span>Új portál</span>' +
                            '</button>' +
                        '</div>' +
                    '</div>'
                );
            }
            ko.applyBindings(model, element[0]);
        },
        _renderInfobar: function (context, model, element) {
            /// <signature>
            /// <summary>Renders an infobar.</summary>
            /// <param name="context" type="Object" />
            /// <param name="model" type="Object" />
            /// <param name="element" type="jQuery" />
            /// </signature>
            if (context.params.portalUri) {
                element.html(
                    '<div role="toolbar" class="ui-toolbar">' +
                        '<div role="group" class="ui-btn-group">' +
                            '<h2 data-bind="text: portal.name"></h2>' +
                        '</div>' +
                        '<div role="group" class="ui-btn-group">' +
                            '<button class="ui-btn ui-btn-flat" type="button" data-bind=\'click: function (d) { d.context.redirect("admin/portals", d.context.params.portalUri, "#/settings"); }, clickBubble: false\'>' +
                                '<i class="i option"></i>' +
                            '</button>' +
                        '</div>' +
                    '</div>'
                );
            }
            ko.applyBindings(model, element[0]);
        }
    });

    ko.components.register("pi-portal-app", {
        viewModel: {
            createViewModel: function (params, info) {
                /// <signature>
                /// <summary>Creates a viewModel for presenting data errors.</summary>
                /// </signature>
                params = params || {};
                params.root = params.root || "/admin/portals";

                return Sammy("#" + info.element.id, function () {
                    this._checkFormSubmission = function (form) { return false; };
                    var controller = new PortalController(info.element.firstChild, params);
                    this.around(function (callback) { $.ui.dropdown.closeAll(); controller._executeCore(callback); });
                    this.get(params.root + "/:portalUri/#/settings", controller.settings.bind(controller));
                    this.get(params.root + "/:portalUri/#/sharing", controller.sharing.bind(controller));
                    this.get(params.root + "/:portalUri/#/hosting", controller.hosting.bind(controller));
                    this.get(params.root + "/:portalUri/#/pages", controller.pages.bind(controller));
                    this.get(params.root + "/:portalUri/#/media", controller.media.bind(controller));
                    this.get(/\/admin\/portals\/(.*)\/#\/designer\/(.*)/, controller.designer.bind(controller));
                    this.get(params.root + "/#/create", controller.create.bind(controller));
                    this.get(params.root, controller.portals.bind(controller));
                })
                .run("/admin/portals");
            }
        },
        template: '<div class="pi-portal-app"></div>'
    });

})(window, WinJS);
