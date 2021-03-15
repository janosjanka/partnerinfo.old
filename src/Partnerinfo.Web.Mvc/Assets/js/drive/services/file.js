// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (_WinJS, _PI) {
    "use strict";

    _WinJS.Namespace.defineWithParent(_PI, "Drive.FileService", {

        getAllAsync: function (params, thisArg) {
            /// <signature>
            /// <summary>Gets all the actions using the given filter parameters.</summary>
            /// <param name="params" type="Object">
            ///     <para>projectId: Number - Project ID</para>
            ///     <para>name?: String - Action name</para>
            ///     <para>page?: Number - Page index</para>
            ///     <para>count?: Number - Page size</para>
            /// </param>
            /// <param name="thisArg" type="Object" optional="true" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            return _PI.api({
                method: "GET",
                path: "projects/{projectId}/actions",
                data: params
            }, thisArg);
        },
        getByIdAsync: function (id, thisArg) {
            /// <signature>
            /// <summary>Gets an action by ID.</summary>
            /// <param name="id" type="Number" />
            /// <param name="thisArg" type="Object" optional="true" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            return _PI.api({
                method: "GET",
                path: "project/actions/" + id
            }, thisArg);
        },
        createAsync: function (email, thisArg) {
            /// <signature>
            /// <summary>Creates a new action.</summary>
            /// <param name="email" type="Object" />
            /// <param name="thisArg" type="Object" optional="true" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            return _PI.api({
                method: "POST",
                path: "project/actions",
                data: email
            }, thisArg);
        },
        updateAsync: function (id, email, thisArg) {
            /// <signature>
            /// <summary>Updates an action.</summary>
            /// <param name="id" type="Number" />
            /// <param name="email" type="Object" />
            /// <param name="thisArg" type="Object" optional="true" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            return _PI.api({
                method: "PUT",
                path: "project/actions/" + id,
                data: email
            }, thisArg);
        },
        copyAsync: function (id, thisArg) {
            /// <signature>
            /// <summary>Copies an action.</summary>
            /// <param name="id" type="Number" />
            /// <param name="thisArg" type="Object" optional="true" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            return _PI.api({
                method: "COPY",
                path: "project/actions/" + id
            }, thisArg);
        },
        deleteAsync: function (id, thisArg) {
            /// <signature>
            /// <summary>Deletes an action.</summary>
            /// <param name="id" type="Number" />
            /// <param name="thisArg" type="Object" optional="true" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            return _PI.api({
                method: "DELETE",
                path: "project/actions/" + id
            }, thisArg);
        }

    });

})(WinJS, PI);