// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="base.js" />

$.widget("PI.PortalEventToolWin", $.PI.PortalToolWin, {
    options: {
        key: "portal.toolwin.event",
        css: "ui-portal-toolwin-event"
    },
    _initAsync: function () {
        /// <signature>
        /// <summary>Initializes a new instance of the tool window.</summary>
        /// <returns type="Boolean" />
        /// </signature>
        this._createModule();
        return WinJS.Promise.complete();
    },
    _onRefresh: function () {
        /// <signature>
        /// <summary>Refreshes the document tree.</summary>
        /// </signature>
        this._super();
        this._destroyModule();
        this._createModule();
    },
    _createModule: function () {
        /// <signature>
        /// <summary>Creates a new event module.</summary>
        /// </signature>
        this.eventModule = this.options.designer.options.module.getEvent();
        this.eventModule && this.eventModule.PortalEventModule({ editor: { menu: this.toolWinMenu, content: this.toolWinBody } });
    },
    _destroyModule: function () {
        /// <signature>
        /// <summary>Destroys the event module.</summary>
        /// </signature>
        if (this.eventModule && this.eventModule.is(":data(PIPortalEventModule)")) {
            this.eventModule.PortalEventModule("destroy");
            this.eventModule = null;
        }
    },
    _destroy: function () {
        /// <signature>
        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        /// </signature>
        this._destroyModule();
        this._super();
    }
});