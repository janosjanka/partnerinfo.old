﻿// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

PI.Portal.Modules.extend("timer", {
    _onCreateEditDialog: function (complete) {
        /// <signature>
        /// <param name="complete" type="Function" />
        /// </signature>
        this._createEditDialog({ complete: complete }, { width: 560 });
    }
});