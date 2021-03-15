// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

PI.Portal.Modules.extend("image", {
    _onCreateEditDialog: function (complete) {
        /// <signature>
        /// <param name="complete" type="Function" />
        /// </signature>
        this._createEditDialog({
            model: {
                fillThumbnailUrl: function () {
                    var descriptor = WinJS.Utilities.parseMediaLink(ko.unwrap(this.module.url));
                    descriptor && this.module.thumbnailUrl(descriptor.thumbnailUrls.hq);
                }
            },
            complete: complete
        }, {
            width: 700
        });
    }
});