// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (_WinJS, _PI) {
    "use strict";

    _WinJS.Namespace.defineWithParent(_PI, "Project", {

        ContactService: {
            getAllAsync: function (params, thisArg) {
                /// <signature>
                /// <param name="params" type="Object">
                ///     <para>projectId: Number</para>
                ///     <para>name?: String</para>
                ///     <para>includeWithTags?: Int32Array</para>
                ///     <para>excludeWithTags?: Int32Array</para>
                ///     <para>orderBy?: String</para>
                ///     <para>offset?: Number</para>
                ///     <para>limit?: Number</para>
                ///     <para>fields?: String (flags)</para>
                /// </param>
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "GET",
                    path: "projects/{projectId}/contacts",
                    data: params
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
                    path: "project/contacts/{id}",
                    data: { id: id, fields: fields }
                }, thisArg);
            },
            getByEmailAsync: function (projectId, email, fields, thisArg) {
                /// <signature>
                /// <param name="projectId" type="Number" />
                /// <param name="email" type="String" />
                /// <param name="fields" type="String" optional="true" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "GET",
                    path: "projects/" + projectId + "/contacts",
                    data: {
                        email: email,
                        fields: fields
                    }
                }, thisArg);
            },
            createAsync: function (projectId, contact, thisArg) {
                /// <signature>
                /// <param name="projectId" type="Number" />
                /// <param name="contact" type="Object" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "POST",
                    path: "projects/" + projectId + "/contacts",
                    data: contact
                }, thisArg);
            },
            updateAsync: function (id, contact, thisArg) {
                /// <signature>
                /// <param name="id" type="Number" />
                /// <param name="contact" type="Object" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "PUT",
                    path: "project/contacts/" + id,
                    data: contact
                }, thisArg);
            },
            deleteAllAsync: function (projectId, ids, thisArg) {
                /// <signature>
                /// <param name="projectId" type="Number" />
                /// <param name="ids" type="Array" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "DELETE",
                    path: "projects/" + projectId + "/contacts",
                    data: ids
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
                    path: "project/contacts/" + id
                }, thisArg);
            },
            setBusinessTagsAsync: function (ids, tagsToAdd, tagsToRemove, thisArg) {
                /// <signature>
                /// <param name="ids" type="Array" />
                /// <param name="tagsToAdd" type="Array" optional="true" />
                /// <param name="tagsToRemove" type="Array" optional="true" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "POST",
                    path: "project/contacts/business-tags",
                    data: {
                        ids: ids,
                        tagsToAdd: tagsToAdd,
                        tagsToRemove: tagsToRemove
                    }
                }, thisArg);
            }
        }

    });

})(WinJS, PI);
