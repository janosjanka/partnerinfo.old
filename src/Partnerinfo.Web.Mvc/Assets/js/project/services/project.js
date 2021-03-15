// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (_WinJS, _PI) {
    "use strict";

    _WinJS.Namespace.defineWithParent(_PI, "Project", {

        ProjectService: {
            getAllAsync: function (params, thisArg) {
                /// <signature>
                /// <param name="params" type="Object">
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
                    path: "projects",
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
                    path: "projects/" + id
                }, thisArg);
            },
            createAsync: function (project, thisArg) {
                /// <signature>
                /// <param name="project" type="Object" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "POST",
                    path: "projects",
                    data: project
                }, thisArg);
            },
            updateAsync: function (id, project, thisArg) {
                /// <signature>
                /// <param name="id" type="Number" />
                /// <param name="project" type="Object" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "PUT",
                    path: "projects/" + id,
                    data: project
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
                    path: "projects/" + id
                }, thisArg);
            }
        }

    });

})(WinJS, PI);
