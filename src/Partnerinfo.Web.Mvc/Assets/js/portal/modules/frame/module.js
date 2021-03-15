// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="../base.js" />

PI.Portal.Modules.register("frame", {
    createModuleOptions: function (options) {
        /// <signature>
        /// <param name="options" type="Object" optional="true" />
        /// <returns type="Object" />
        /// </signature>
        return $.extend(this._super(options), {
            url: null,
            width: "100%",
            height: "100%",
            scrolling: null,
            allowTransparency: false
        }, options);
    },
    _activate: function () {
        /// <signature>
        /// <summary>Activates this module.</summary>
        /// </signature>
        var options = this.getModuleOptions();
        if (!options.url) {
            return;
        }
        this.element.html($("<iframe>")
            .attr({
                id: "frame-" + this.uuid,
                src: options.url,
                width: options.width,
                height: options.height,
                frameborder: 0,
                scrolling: options.scrolling,
                allowTransparency: options.allowTransparency,
                seamless: true
                //sandbox: "",
                //security: "restricted"
            }) // HTML 5 doesn't support the scrolling attribute.
            .css("overflow",
                options.scrolling === "no"
                    ? "hidden"
                    : options.scrolling === "yes"
                        ? "scroll"
                        : "auto"));
    },
    _deactivate: function () {
        /// <signature>
        /// <summary>Deactivates this module.</summary>
        /// </signature>
        this.element.empty();
    }
});
