// Copyright (c) Partnerinfo TV. All Rights Reserved.

/// <reference path="../viewmodels/media.js" />

(function (_WinJS, _PI, _Portal) {
    "use strict";

    _PI.component({
        name: "portal.media",
        model: function (options) {
            /// <signature>
            /// <param name="options" type="Object" />
            /// <returns type="PI.Portal.MediaList" />
            /// </signature>
            var params = options.params || {};
            params.query = params.query || { orderBy: "recent" };
            return new _Portal.MediaList(params);
        },
        view: function (model, options) {
            /// <signature>
            /// <param name="model" type="PI.Portal.PortalItem" />
            /// <param name="options" type="Object" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            return _WinJS.Promise.join(
                this._renderView(options.element, model, "koPortalMediaList"),
                this._renderView(options.toolbar, model, "koPortalMediaMenu"));
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

})(WinJS, PI, PI.Portal);