// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="base.js" />

(function (window, $, _WinJS, _PI, undefined) {
    "use strict";
    
    $.widget("PI.PortalReferenceToolWin", $.PI.PortalToolWin, {
        options: {
            key: "portal.toolwin.reference"
        },
        _init: function () {
            /// <signature>
            /// <summary>Initializes an existing instance of the tool window.</summary>
            /// </signature>
            this._super();
        },
        _onRender: function () {
            /// <signature>
            /// <summary>Renders content.</summary>
            /// </signature>
            this.references = new _PI.Portal.ReferenceList();
            this.toolWinBody.html("<div data-bind='listView: { columns: [{ binding: \"text: uri\" }] }'></div>");
        },
        _onRefresh: function () {
            /// <signature>
            /// <summary>Refreshes the snippet list.</summary>
            /// </signature>
            this._super();
        }
    });

})(window, jQuery, WinJS, PI);
