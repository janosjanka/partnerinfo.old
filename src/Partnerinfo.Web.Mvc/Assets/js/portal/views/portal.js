// Copyright (c) Partnerinfo TV. All Rights Reserved.

/// <reference path="../viewmodels/portal.js" />

(function (_WinJS, _PI, _Portal) {
    "use strict";

    _PI.component({
        name: "portal",
        model: function (options) {
            /// <signature>
            /// <param name="options" type="Object" />
            /// <returns type="PI.Portal.PortalItem" />
            /// </signature>
            var params = options.params || {};
            var portal = new _Portal.PortalItem(params.portal, params);
            portal.loadAsync();
            return portal;
        },
        view: function (model, options) {
            /// <signature>
            /// <param name="model" type="PI.Portal.PortalItem" />
            /// <param name="options" type="Object" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            return _WinJS.Promise.join(
                this._renderView(options.element, model, "koPortalItem"),
                this._renderView(options.toolbar, model, "koPortalMenu"));
        },
        dialog: function (model, options, response, callback) {
            /// <signature>
            /// <param name="model" type="PI.Portal.PortalItem" />
            /// <param name="options" type="Object" />
            /// <param name="response" type="Object" />
            /// <param name="callback" type="Function" />
            /// <returns type="$.WinJS.dialog" />
            /// </signature>
            return this._renderDialog({
                width: 640,
                height: 450,
                title: _T("portal"),
                buttons: [{
                    "class": "ui-btn ui-btn-primary",
                    "text": _T("ui/done"),
                    "click": function () {
                        model.saveAsync().then(callback.bind(model, "ok"));
                    }
                }, {
                    "class": "ui-btn",
                    "text": _T("ui/close"),
                    "click": callback.bind(model, "cancel")
                }]
            });
        }
    });

    _PI.component({
        name: "portal.host",
        model: function (options) {
            /// <signature>
            /// <param name="options" type="Object" />
            /// <returns type="PI.Portal.PortalItem" />
            /// </signature>
            var params = options.params || {};
            var model = {
                domain: ko.observable().extend({ required: true }),
                portal: new _Portal.PortalItem(params.portal, params),
                update: function () {
                    if (this.domain.isValid()) {
                        model.portal.setDomainAsync(this.domain())
                            .then(function () {
                                model.domain(model.portal.domain());
                            });
                    }
                },
                remove: function () {
                    model.portal.setDomainAsync(null)
                        .then(function () {
                            model.domain(null);
                        });
                }
            };
            model.portal.loadAsync();
            return model;
        },
        view: function (model, options) {
            /// <signature>
            /// <param name="model" type="PI.Portal.PortalItem" />
            /// <param name="options" type="Object" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            return this._renderView(options.element, model, "koPortalBinding");
        }
    });

    _PI.component({
        name: "portal.uri",
        model: function (options) {
            /// <signature>
            /// <param name="options" type="Object" />
            /// <returns type="Object" />
            /// </signature>
            return {
                uri: ko.observable(options.uri),
                saveAsync: function () {
                    return _Portal.PortalService.setUriAsync(options.uri, this.uri(), this)
                        .then(function (uri) {
                            this.uri(uri);
                        });
                }
            };
        },
        view: function (model, options) {
            /// <signature>
            /// <param name="model" type="Object" />
            /// <param name="options" type="Object" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            return this._renderView(options.element, model, "koPortalUri");
        },
        dialog: function (model, options, response, callback) {
            /// <signature>
            /// <param name="model" type="Object" />
            /// <param name="options" type="Object" />
            /// <param name="response" type="Object" />
            /// <param name="callback" type="Function" />
            /// <returns type="$.WinJS.dialog" />
            /// </signature>
            return this._renderDialog({
                width: 400,
                title: _T("portal"),
                buttons: [{
                    "class": "ui-btn ui-btn-primary",
                    "text": _T("ui/done"),
                    "click": function () {
                        model.saveAsync().then(function () {
                            response.uri = model.uri();
                            callback.call(model, "ok");
                        });
                    }
                }, {
                    "class": "ui-btn",
                    "text": _T("ui/close"),
                    "click": callback.bind(model, "cancel")
                }]
            });
        }
    });

    _PI.component({
        name: "portals",
        model: function (options) {
            /// <signature>
            /// <param name="options" type="Object" />
            /// <returns type="PI.Portal.PortalList" />
            /// </signature>
            var params = options.params || {};
            params.filter = params.filter || { orderBy: "recent", fields: "project,owners" };
            return new _Portal.PortalList(params);
        },
        view: function (model, options) {
            /// <signature>
            /// <param name="model" type="PI.Portal.PortalList" />
            /// <param name="options" type="Object" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            return this._renderView(options.element, model,
                options.displayMode === "menu" ? "koPortalListMenu" : "koPortalList");
        }
    });

    _PI.component({
        name: "portal.designer",
        view: function (model, options) {
            /// <signature>
            /// <param name="model" type="PI.Portal.Page" />
            /// <param name="options" type="Object" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            return require(["portal/modules", "portal/modules.css"]).then(function () {
                return require(["portal/designer", "portal/designer.css"]).then(function () {
                    $(options.element).PortalDesigner(options);
                });
            });
        }
    });

})(WinJS, PI, PI.Portal);