// Copyright (c) Partnerinfo TV. All Rights Reserved.

/// <reference path="../viewmodels/page.js" />

(function (_WinJS, _PI, _Portal) {
    "use strict";

    _PI.component({
        name: "portal.page",
        model: function (options) {
            /// <signature>
            /// <param name="options" type="Object" />
            /// <returns type="PI.Portal.PageItem" />
            /// </signature>
            var params = options.params || {};
            var page = new _Portal.PageItem(params.portal, params.page, params);
            page.loadAsync();
            return page;
        },
        view: function (model, options) {
            /// <signature>
            /// <param name="model" type="PI.Portal.PageItem" />
            /// <param name="options" type="Object" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            return _WinJS.Promise.join(
                this._renderView(options.element, model, "koPageItem"),
                this._renderView(options.menu, model, "koPageMenu"));
        },
        dialog: function (model, options, response, callback) {
            /// <signature>
            /// <param name="model" type="PI.Portal.PageItem" />
            /// <param name="options" type="Object" />
            /// <param name="response" type="Object" />
            /// <param name="callback" type="Function" />
            /// <returns type="$.WinJS.dialog" />
            /// </signature>
            return this._renderDialog({
                width: 550,
                title: _T("page"),
                buttons: [{
                    "class": "ui-btn ui-btn-primary",
                    "text": _T("ui/done"),
                    "click": function () {
                        model.saveAsync().then(function () {
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

})(WinJS, PI, PI.Portal);