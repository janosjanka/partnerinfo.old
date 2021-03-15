// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (_WinJS, _PI) {
    "use strict";

    _WinJS.Namespace.defineWithParent(_PI, "Identity", {

        UserService: {
            getAllAsync: function (params, thisArg) {
                /// <signature>
                /// <param name="params" type="Object" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "GET",
                    path: "identity/users",
                    data: params
                }, thisArg);
            }            
        }

    });

})(WinJS, PI);
