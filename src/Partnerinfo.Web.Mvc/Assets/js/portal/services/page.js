// Copyright (c) Partnerinfo TV. All Rights Reserved.

(function (_Global, _WinJS, _PI) {
    "use strict";

    function listAction(portalUri) {
        /// <signature>
        /// <param name="portalUri" type="String" />
        /// <returns type="String" />
        /// </signature>
        return "portal/pages/" + portalUri;
    }

    function itemAction(portalUri, pageUri, detail) {
        /// <signature>
        /// <param name="portalUri" type="String" />
        /// <param name="pageUri" type="String" optional="true" />
        /// <param name="detail" type="String" optional="true" />
        /// <returns type="String" />
        /// </signature>
        var url = "portal/pages";
        if (detail) {
            url += "/";
            url += detail;
        }
        url += "/";
        url += portalUri;
        if (pageUri) {
            url += "/";
            url += pageUri;
        }
        return url;
    }

    _WinJS.Namespace.defineWithParent(_PI, "Portal", {

        PageService: {
            getAllAsync: function (portalUri, thisArg) {
                /// <signature>
                /// <param name="portalUri" type="String" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "GET",
                    path: listAction(portalUri)
                }, thisArg);
            },
            getByUriAsync: function (portalUri, pageUri, fields, thisArg) {
                /// <signature>
                /// <param name="portalUri" type="String" />
                /// <param name="pageUri" type="String" />
                /// <param name="fields" type="String" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "GET",
                    path: itemAction(portalUri, pageUri),
                    data: fields && { fields: fields }
                }, thisArg);
            },
            getLayersByUriAsync: function (portalUri, pageUri, thisArg) {
                /// <signature>
                /// <param name="portalUri" type="String" />
                /// <param name="pageUri" type="String" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "GET",
                    path: itemAction(portalUri, pageUri, "layers")
                }, thisArg);
            },
            createAsync: function (portalUri, pageUri, page, thisArg) {
                /// <signature>
                /// <param name="portalUri" type="String" />
                /// <param name="pageUri" type="String" />
                /// <param name="page" type="Object" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "POST",
                    path: itemAction(portalUri, pageUri),
                    data: page
                }, thisArg);
            },
            updateAsync: function (portalUri, pageUri, page, thisArg) {
                /// <signature>
                /// <param name="portalUri" type="String" />
                /// <param name="pageUri" type="String" />
                /// <param name="page" type="Object" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "PUT",
                    path: itemAction(portalUri, pageUri),
                    data: page
                }, thisArg);
            },
            copyAsync: function (portalUri, pageUri, options, thisArg) {
                /// <signature>
                /// <param name="portalUri" type="String" />
                /// <param name="pageUri" type="String" />
                /// <param name="options" type="Object" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "COPY",
                    path: itemAction(portalUri, pageUri),
                    data: options
                }, thisArg);
            },
            moveAsync: function (portalUri, pageUri, uri, thisArg) {
                /// <signature>
                /// <param name="portalUri" type="String" />
                /// <param name="pageUri" type="String" />
                /// <param name="uri" type="String" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "MOVE",
                    path: itemAction(portalUri, pageUri),
                    data: uri
                }, thisArg);
            },
            deleteAsync: function (portalUri, pageUri, thisArg) {
                /// <signature>
                /// <param name="portalUri" type="String" />
                /// <param name="pageUri" type="String" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "DELETE",
                    path: itemAction(portalUri, pageUri)
                });
            },
            setContentAsync: function (portalUri, pageUri, content, thisArg) {
                /// <signature>
                /// <param name="portalUri" type="String" />
                /// <param name="pageUri" type="String" />
                /// <param name="content" type="Object" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "POST",
                    path: itemAction(portalUri, pageUri, "content"),
                    data: content
                }, thisArg);
            },
            setMasterAsync: function (portalUri, pageUri, uri, thisArg) {
                /// <signature>
                /// <param name="portalUri" type="String" />
                /// <param name="pageUri" type="String" />
                /// <param name="uri" type="String" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "POST",
                    path: itemAction(portalUri, pageUri, "master"),
                    data: uri
                }, thisArg);
            },
            setReferencesAsync: function (portalUri, pageUri, references, thisArg) {
                /// <signature>
                /// <param name="portalUri" type="String" />
                /// <param name="pageUri" type="String" />
                /// <param name="references" type="Array" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "POST",
                    path: itemAction(portalUri, pageUri, "references"),
                    data: references
                }, thisArg);
            },
            setModuleAsync: function (id, moduleId, htmlContent, styleContent, moduleOptions, thisArg) {
                /// <signature>
                /// <summary>Sets the content of the given page.</summary>
                /// <param name="id" type="Number" />
                /// <param name="moduleId" type="String" />
                /// <param name="htmlContent" type="String" />
                /// <param name="styleContent" type="String" />
                /// <param name="moduleOptions" type="Object" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "POST",
                    path: "portal/pages/" + id + "/module/" + moduleId,
                    data: {
                        htmlContent: htmlContent,
                        styleContent: styleContent,
                        moduleOption: moduleOptions
                    }
                }, thisArg);
            }
        }

    });

})(window, WinJS, PI);