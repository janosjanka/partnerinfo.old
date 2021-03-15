// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="../viewmodels/acl.js" />

(function (_WinJS, _PI, _Security) {
    "use strict";

    _PI.component({
        name: "security.acl",
        model: function (options) {
            /// <signature>
            /// <param name="options" type="Object" />
            /// <returns type="PI.Security.AceList" />
            /// </signature>
            var params = options.params || {};
            return new _Security.AccessRuleList(params.objectType, params.objectId);
        },
        view: function (model, options) {
            /// <signature>
            /// <param name="model" type="PI.Security.AceList" />
            /// <param name="options" type="Object" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            return this._renderView(options.element, model, "koAcl");
        },
        dialog: function () {
            /// <signature>
            /// <param name="model" type="PI.Security.AceList" />
            /// <param name="options" type="Object" />
            /// <param name="response" type="Object" />
            /// <param name="callback" type="Function" />
            /// <returns type="$.WinJS.dialog" />
            /// </signature>
            return this._renderDialog({
                width: 750,
                minHeight: 450,
                title: "Megosztás",
                buttons: null
            });
        }
    });

})(WinJS, PI, PI.Security);