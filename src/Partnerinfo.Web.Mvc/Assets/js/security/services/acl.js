// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (_WinJS, _PI) {
    "use strict";

    _WinJS.Namespace.defineWithParent(_PI, "Security", {

        AccessRuleService: {
            getAllAsync: function (objectType, objectId, thisArg) {
                /// <signature>
                /// <param name="objectType" type="Object" />
                /// <param name="objectId" type="Number" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "GET",
                    path: "security/access-rules/{objectType}/{objectId}",
                    data: {
                        objectType: objectType,
                        objectId: objectId
                    }
                }, thisArg);
            },
            setAsync: function (objectType, objectId, trustee, thisArg) {
                /// <signature>
                /// <param name="objectType" type="Object" />
                /// <param name="objectId" type="Number" />
                /// <param name="trustee" type="Object" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "POST",
                    path: "security/access-rules/" + objectType + "/" + objectId,
                    data: trustee
                }, thisArg);
            },
            deleteAsync: function (objectType, objectId, email, thisArg) {
                /// <signature>
                /// <param name="objectType" type="Object" />
                /// <param name="objectId" type="Number" />
                /// <param name="email" type="String" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "DELETE",
                    path: "security/access-rules/" + objectType + "/" + objectId,
                    data: email
                }, thisArg);
            }
        }

    });

})(WinJS, PI);
