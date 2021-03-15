// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (_WinJS, _PI) {
    "use strict";

    _WinJS.Namespace.defineWithParent(_PI, "Logging", {

        RuleService: {
            getAllAsync: function (options, thisArg) {
                /// <signature>
                /// <param name="options" type="Object" optional="true" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "GET",
                    path: "logging/rules",
                    data: options
                }, thisArg);
            },
            getByIdAsync: function (id, fields, thisArg) {
                /// <signature>
                /// <param name="id" type="Number" />
                /// <param name="fields" type="String" optional="true" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "GET",
                    path: "project/rules/{id}",
                    data: { id: id, fields: fields }
                }, thisArg);
            },
            createAsync: function (rule, thisArg) {
                /// <signature>
                /// <param name="rule" type="Object" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "POST",
                    path: "logging/rules",
                    data: rule
                }, thisArg);
            },
            updateAsync: function (id, rule, thisArg) {
                /// <signature>
                /// <param name="id" type="Number" />
                /// <param name="rule" type="Object" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "PUT",
                    path: "logging/rules/" + id,
                    data: rule
                }, thisArg);
            },
            deleteAsync: function (id, thisArg) {
                /// <signature>
                /// <param name="id" type="Number" />
                /// <returns type="WinJS.Promise" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// </signature>
                return _PI.api({
                    method: "DELETE",
                    path: "logging/rules/" + id
                }, thisArg);
            }
        }

    });

})(WinJS, PI);