// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (_WinJS, _PI) {
    "use strict";

    _WinJS.Namespace.defineWithParent(_PI, "Project", {

        MailMessageService: {
            getAllAsync: function (params, thisArg) {
                /// <signature>
                /// <summary>Gets all emails</summary>
                /// <param name="params" type="Object">
                ///     <para>projectId: Number</para>
                ///     <para>subject?: String</para>
                ///     <para>page?: Number</para>
                ///     <para>count?: Number</para>
                /// </param>
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "GET",
                    path: "projects/{projectId}/mails",
                    data: params
                }, thisArg);
            },
            getByIdAsync: function (id, fields, thisArg) {
                /// <signature>
                /// <summary>Gets an email by ID</summary>
                /// <param name="id" type="Number" />
                /// <param name="fields" type="String" optional="true" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "GET",
                    path: "project/mails/" + id,
                    data: { fields: fields }
                }, thisArg);
            },
            createAsync: function (projectId, email, thisArg) {
                /// <signature>
                /// <summary>Creates a new email</summary>
                /// <param name="projectId" type="Number" />
                /// <param name="email" type="Object" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "POST",
                    path: "projects/" + projectId + "/mails",
                    data: email
                }, thisArg);
            },
            updateAsync: function (id, email, thisArg) {
                /// <signature>
                /// <summary>Updates an email</summary>
                /// <param name="id" type="Number" />
                /// <param name="email" type="Object" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "PUT",
                    path: "project/mails/" + id,
                    data: email
                }, thisArg);
            },
            sendAsync: function (id, header, thisArg) {
                /// <signature>
                /// <summary>Updates an email</summary>
                /// <param name="id" type="Number" />
                /// <param name="header" type="Object" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "POST",
                    path: "project/mails/" + id + "/send",
                    data: header
                }, thisArg);
            },
            deleteAsync: function (id, thisArg) {
                /// <signature>
                /// <summary>Deletes an email</summary>
                /// <param name="id" type="Number" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "DELETE",
                    path: "project/mails/" + id
                }, thisArg);
            }
        }

    });

})(WinJS, PI);
