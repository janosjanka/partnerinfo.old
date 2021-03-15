// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="module.js" />

PI.Portal.Modules.extend("video", {
    _onCreateEditDialog: function (complete) {
        /// <signature>
        /// <param name="complete" type="Function" />
        /// </signature>        
        this._createEditDialog({
            model: {
                fillThumbnailUrl: function () {
                    var videoUrl = ko.unwrap(this.module.url);
                    if (videoUrl) {
                        var descriptor = WinJS.Utilities.parseMediaLink(videoUrl);
                        if (descriptor && descriptor.thumbnailUrls) {
                            this.module.thumbnailUrl(descriptor.thumbnailUrls.hq);
                            return;
                        }
                    }
                    this.module.thumbnailUrl(null);
                }
            },
            complete: complete
        }, {
            width: 640
        });
    }
});