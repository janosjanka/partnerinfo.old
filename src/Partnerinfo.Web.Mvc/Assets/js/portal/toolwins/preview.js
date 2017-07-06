// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="base.js" />

$.widget("PI.PortalPreviewToolWin", $.PI.PortalToolWin, {
    options: {
        key: "portal.toolwin.preview"
    },
    _create: function () {
        /// <signature>
        /// <summary>Initializes a new instance of the widget.</summary>
        /// </signature>
        this._super();
        this.toolWinBody.css("overflow", "auto");
    },
    _init: function () {
        /// <signature>
        /// <summary>Initializes an existing instance of the tool window.</summary>
        /// </signature>
        this._super();
        this._showPreview(this.options.designer.options.selection.current);
    },
    _onRefresh: function () {
        /// <signature>
        /// <summary>Refreshes the snippet list.</summary>
        /// </signature>
        this._super();
        this._showPreview(this.options.designer.options.selection.current);
    },
    _onSelectionChanged: function (event) {
        /// <signature>
        /// <param name="event" type="Event" />
        /// </signature>
        this._super(event);
        this._showPreview(event.detail.element);
    },
    _showPreview: function (moduleElement) {
        /// <signature>
        /// <summary>Displays the preview of the specified module element.</summary>
        /// </signature>
        this._hidePreview();
        if (moduleElement) {
            var module = this.options.designer.options.engine.context.getInstanceOf(moduleElement);
            if (module) {
                module.showPreview(this.toolWinBody);
            }
        }
    },
    _hidePreview: function () {
        /// <signature>
        /// <summary>Clears the toolwindow.</summary>
        /// </signature>
        this.toolWinBody.empty();
    },
    _destroy: function () {
        /// <signature>
        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        /// </signature>
        this._hidePreview();
        this._super();
    }
});