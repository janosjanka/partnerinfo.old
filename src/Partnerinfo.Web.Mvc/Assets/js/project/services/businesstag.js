// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (_WinJS, _PI) {
    "use strict";

    _WinJS.Namespace.defineWithParent(_PI, "Project", {

        BusinessTagService: {
            getAllAsync: function (params, thisArg) {
                /// <signature>
                /// <param name="params" type="Object">
                ///     <para>projectId: Number - Project ID</para>
                /// </param>
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "GET",
                    path: "projects/{projectId}/business-tags",
                    data: params
                }, thisArg);
            },
            getByIdAsync: function (id, thisArg) {
                /// <signature>
                /// <param name="id" type="Number" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "GET",
                    path: "project/business-tags/" + id
                }, thisArg);
            },
            createAsync: function (projectId, businessTag, thisArg) {
                /// <signature>
                /// <param name="projectId" type="Number" />
                /// <param name="businessTag" type="Object" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "POST",
                    path: "projects/" + projectId + "/business-tags",
                    data: businessTag
                }, thisArg);
            },
            updateAsync: function (id, businessTag, thisArg) {
                /// <signature>
                /// <param name="id" type="Number" />
                /// <param name="businessTag" type="Object" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "PUT",
                    path: "project/business-tags/" + id,
                    data: businessTag
                }, thisArg);
            },
            deleteAsync: function (id, thisArg) {
                /// <signature>
                /// <param name="id" type="Number" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "DELETE",
                    path: "project/business-tags/" + id
                }, thisArg);
            }
        }

    });

})(WinJS, PI);
