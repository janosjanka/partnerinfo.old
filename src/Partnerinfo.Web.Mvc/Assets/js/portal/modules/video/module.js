// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="../base.js" />

PI.Portal.Modules.register("video", {
    createModuleOptions: function (options) {
        /// <signature>
        /// <summary>Extends default module options with the specified module options.</summary>
        /// <param name="options" type="Object" optional="true">A set of key/value pairs that can be used to configure module options.</param>
        /// <returns type="Object" />
        /// </signature>
        return $.extend(this._superApply(arguments), {
            url: null,
            thumbnailUrl: null,
            autoplay: false,
            autohide: true,
            controls: true,
            fs: true,
            rel: false,
            showinfo: false,
            code: null,
            loadEvent: null
        }, options);
    },
    _activate: function () {
        /// <signature>
        /// <summary>Activates this module.</summary>
        /// </signature>
        var moduleOptions = this.getModuleOptions();
        if (moduleOptions.thumbnailUrl) {
            var $img = $("<img>").attr("src", moduleOptions.thumbnailUrl).appendTo(this.element);
            if (moduleOptions.loadEvent) {
                var that = this;
                $img.bind(moduleOptions.loadEvent, function () {
                    that.element.append(that._getEmbedNode(moduleOptions));
                });
            }
            return;
        }
        this.element.append(this._getEmbedNode(moduleOptions));
    },
    _deactivate: function () {
        /// <signature>
        /// <summary>Deactivates this module.</summary>
        /// </signature>
        this.element.empty();
    },
    _getEmbedNode: function (options) {
        /// <signature>
        /// <summary>Gets the active content of the current module.</summary>
        /// <returns type="String" />
        /// </signature>
        if (!options.url) {
            return;
        }
        var descriptor = WinJS.Utilities.parseMediaLink(options.url, {
            autoplay: options.autoplay ? 1 : 0,
            autohide: options.autohide ? 1 : 0,
            controls: options.controls ? 2 : 0,
            fs: options.fs ? 1 : 0,
            rel: options.rel ? 1 : 0,
            showinfo: options.showinfo ? 1 : 0
        });
        if (!descriptor.url) {
            return;
        }
        return $("<iframe>").attr({
            src: descriptor.url,
            width: "100%",
            height: "100%",
            frameBorder: 0,
            allowfullscreen: !!options.fs
        });
    }
});