// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="module.js" />

PI.Portal.Modules.extend("chat", {
    _onCreateEditDialog: function (completeDispatch) {
        /// <signature>
        /// <param name="completeDispatch" type="Function" />
        /// </signature>
        this._createEditDialog({
            model: {},
            complete: completeDispatch
        }, {
            width: 640,
            height: 480
        });
    }
});