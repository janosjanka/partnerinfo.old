// Copyright (c) Partnerinfo TV. All Rights Reserved.

(function (_Global, _WinJS, _PI) {
    "use strict";

    function itemAction(portalUri, mediaUri) {
        /// <signature>
        /// <param name="portalUri" type="String" />
        /// <param name="mediaUri" type="String" optional="true" />
        /// <returns type="String" />
        /// </signature>
        var url = "portal/media/";
        url += portalUri;
        if (mediaUri) {
            url += "/";
            url += mediaUri;
        }
        return url;
    }

    _WinJS.Namespace.defineWithParent(_PI, "Portal", {

        MediaService: {
            getUploadUrl: function (portalUri) {
                /// <signature>
                /// <param name="portalUri" type="String" />
                /// <returns type="String" />
                /// </signature>
                return _PI.api.action(itemAction(portalUri));
            },
            getByUriAsync: function (params, thisArg) {
                /// <signature>
                /// <param name="params" type="Object" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "GET",
                    path: itemAction(params.portalUri, params.mediaUri),
                    data: {
                        name: params.name,
                        orderBy: params.orderBy,
                        fields: params.fields
                    }
                }, thisArg);
            },
            deleteAsync: function (portalUri, mediaUri, thisArg) {
                /// <signature>
                /// <param name="portalUri" type="String" />
                /// <param name="mediaUri" type="String" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "DELETE",
                    path: itemAction(portalUri, mediaUri)
                }, thisArg);
            }
        }

    });

})(window, WinJS, PI);
