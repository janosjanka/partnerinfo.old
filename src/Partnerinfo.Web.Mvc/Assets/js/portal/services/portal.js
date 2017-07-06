// Copyright (c) Partnerinfo TV. All Rights Reserved.

(function (_Global, _WinJS, _PI) {
    "use strict";

    function listAction() {
        /// <signature>
        /// <returns type="String" />
        /// </signature>
        return "portals";
    }

    function itemAction(uri, rel) {
        /// <signature>
        /// <param name="uri" type="String" />
        /// <param name="rel" type="String" optional="true" />
        /// <returns type="String" />
        /// </signature>
        return "portals/" + _Global.encodeURIComponent(uri) + (rel ? ("/" + rel) : "");
    }

    _WinJS.Namespace.defineWithParent(_PI, "Portal", {

        PortalService: {
            getAllAsync: function (params, thisArg) {
                /// <signature>
                /// <param name="params" type="Object">
                ///     <para>name?: String</para>
                ///     <para>orderBy?: Number</para>
                ///     <para>page?: Number</para>
                ///     <para>limit?: Number</para>
                ///     <para>fields?: Number | String</para>
                /// </param>
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "GET",
                    path: listAction(),
                    data: params
                }, thisArg);
            },
            getByUriAsync: function (uri, fields, thisArg) {
                /// <signature>
                /// <param name="uri" type="String" />
                /// <param name="fields" type="String" optional="true" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "GET",
                    path: itemAction(uri),
                    data: fields ? { fields: fields } : void 0
                }, thisArg);
            },
            createAsync: function (portal, thisArg) {
                /// <signature>
                /// <param name="portal" type="Object" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "POST",
                    path: "portals",
                    data: portal
                }, thisArg);
            },
            updateAsync: function (uri, portal, thisArg) {
                /// <signature>
                /// <param name="uri" type="String" />
                /// <param name="project" type="Object" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "PUT",
                    path: itemAction(uri),
                    data: portal
                }, thisArg);
            },
            copyAsync: function (uri, portal, thisArg) {
                /// <signature>
                /// <param name="uri" type="String" />
                /// <param name="portal" type="Object" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "COPY",
                    path: itemAction(uri),
                    data: portal
                }, thisArg);
            },
            deleteAsync: function (uri, thisArg) {
                /// <signature>
                /// <param name="uri" type="String" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "DELETE",
                    path: itemAction(uri)
                }, thisArg);
            },
            setHomePageAsync: function (uri, homePageUri, thisArg) {
                /// <signature>
                /// <param name="uri" type="String" />
                /// <param name="homePageUri" type="String" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "POST",
                    path: itemAction(uri, "homepage"),
                    data: homePageUri
                }, thisArg);
            },
            setMasterPageAsync: function (uri, masterPageUri, thisArg) {
                /// <signature>
                /// <param name="uri" type="String" />
                /// <param name="masterPageUri" type="String" optional="true" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "POST",
                    path: itemAction(uri, "masterpage"),
                    data: masterPageUri
                }, thisArg);
            },
            setProjectAsync: function (uri, projectId, thisArg) {
                /// <signature>
                /// <param name="uri" type="String" />
                /// <param name="projectId" type="Number" optional="true" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "POST",
                    path: itemAction(uri, "project"),
                    data: projectId
                }, thisArg);
            },
            setDomainAsync: function (uri, domain, thisArg) {
                /// <signature>
                /// <param name="uri" type="String" />
                /// <param name="domain" type="String" optional="true" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "POST",
                    path: itemAction(uri, "domain"),
                    data: domain
                }, thisArg);
            }
        }

    });

})(window, WinJS, PI);
