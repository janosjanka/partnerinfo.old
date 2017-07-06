// Copyright (c) Partnerinfo Ltd. All Rights Reserved.
/// <reference path="../viewmodels/rule.js" />

(function (_WinJS, _PI, _Logging) {
    "use strict";

    var _Resources = _WinJS.Resources;

    _PI.component({
        name: "logging.rules",
        model: function (options) {
            /// <signature>
            /// <param name="options" type="Object" />
            /// <returns type="PI.Logging.RuleList" />
            /// </signature>
            return new _Logging.RuleList(options.params || {});
        },
        view: function (model, options) {
            /// <signature>
            /// <param name="model" type="PI.Logging.RuleList" />
            /// <param name="options" type="Object" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            return _WinJS.Promise.join(
                this._renderView(options.element, model, "koLogRuleList"),
                this._renderView(options.toolbar, model, "koLogRuleListToolbar")
            );
        },
        dialog: function (model, options, response, done) {
            /// <signature>
            /// <param name="model" type="PI.Project.ProjectList" />
            /// <param name="options" type="Object" />
            /// <param name="response" type="Object" />
            /// <param name="done" type="Function" />
            /// <returns type="$.WinJS.dialog" />
            /// </signature>
            return this._renderDialog({
                width: 750,
                height: 500,
                title: _T("logging/rules"),
                buttons: [{
                    "class": "ui-btn",
                    "text": _T("ui/done"),
                    "click": function () {
                        done.call(model, "ok");
                    }
                }]
            });
        }
    });

})(WinJS, PI, PI.Logging);