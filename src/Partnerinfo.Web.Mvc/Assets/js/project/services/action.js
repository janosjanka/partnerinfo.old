// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (_WinJS, _PI) {
    "use strict";

    function actionListUrl(projectId, actionId) {
        /// <signature>
        /// <returns type="String" />
        /// </signature>
        return "projects/" + projectId + "/actions/" + (actionId || "");
    }

    function actionItemUrl(id) {
        /// <signature>
        /// <returns type="String" />
        /// </signature>
        return "project/actions/" + id;
    }

    _WinJS.Namespace.defineWithParent(_PI, "Project", {

        ActionService: {
            getAllAsync: function (params, thisArg) {
                /// <signature>
                /// <param name="params" type="Object">
                ///     <para>projectId: Number - Project ID</para>
                ///     <para>name?: String - Action name</para>
                ///     <para>orderBy?: Number - Sort order</para>
                ///     <para>page?: Number - Page index</para>
                ///     <para>count?: Number - Page size</para>
                ///     <para>fields?: Number - Included fields</para>
                /// </param>
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "GET",
                    path: actionListUrl(params.projectId),
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
                    path: actionItemUrl(id)
                }, thisArg);
            },
            getByLinkIdAsync: function (id, thisArg) {
                /// <signature>
                /// <param name="id" type="Number" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "GET",
                    path: "project/actionlinks/" + id
                }, thisArg);
            },
            getByLinkAsync: function (link, thisArg) {
                /// <signature>
                /// <param name="link" type="String" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "GET",
                    path: "project/actions",
                    data: { link: link }
                }, thisArg);
            },
            getLinkAsync: function (id, params, thisArg) {
                /// <signature>
                /// <param name="id" type="Number" />
                /// <param name="params" type="Object" optional="true">
                ///     <para>contactId?: String</para>
                ///     <para>customUri?: String</para>
                /// </param>
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "GET",
                    path: "project/actions/" + id + "/link",
                    data: params
                }, thisArg);
            },
            createAsync: function (projectId, parentId, action, thisArg) {
                /// <signature>
                /// <param name="projectId" type="Number" />
                /// <param name="parentId" type="Number" optional="true" />
                /// <param name="action" type="Object" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "POST",
                    path: actionListUrl(projectId, parentId),
                    data: action
                }, thisArg);
            },
            updateAsync: function (id, action, thisArg) {
                /// <signature>
                /// <param name="id" type="Number" />
                /// <param name="action" type="Object" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "PUT",
                    path: actionItemUrl(id),
                    data: action
                }, thisArg);
            },
            copyBeforeAsync: function (id, referenceId, thisArg) {
                /// <signature>
                /// <param name="id" type="Number" />
                /// <param name="referenceId" type="Number" optional="true" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "COPY",
                    path: actionItemUrl(id),
                    data: referenceId && { referenceId: referenceId }
                }, thisArg);
            },
            moveBeforeAsync: function (id, referenceId, thisArg) {
                /// <signature>
                /// <param name="id" type="Number" />
                /// <param name="referenceId" type="Number" optional="true" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "MOVE",
                    path: actionItemUrl(id),
                    data: referenceId && { referenceId: referenceId }
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
                    path: actionItemUrl(id)
                }, thisArg);
            }
        }

    });

})(WinJS, PI);
