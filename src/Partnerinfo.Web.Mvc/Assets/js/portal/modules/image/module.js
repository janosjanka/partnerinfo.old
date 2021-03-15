// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

PI.Portal.Modules.register("image", {
    createModuleOptions: function (options) {
        /// <signature>
        /// <param name="options" type="Object" optional="true" />
        /// <returns type="Object" />
        /// </signature>
        return $.extend(this._super(options), {
            image: { url: null, alt: null },
            link: { url: null, target: null }
        }, options);
    },
    _activate: function () {
        /// <signature>
        /// <summary>Activates this module.</summary>
        /// </signature>
        var options = this.getModuleOptions();
        var image = document.createElement("img");
        options.image.url && image.setAttribute("src", options.image.url);
        options.image.alt && image.setAttribute("alt", options.image.alt);
        if (options.link.url) {
            var link = document.createElement("a");
            link.setAttribute("href", options.link.url);
            options.link.target && link.setAttribute("target", options.link.target);
            link.appendChild(image);
            this.element.html(link);
        } else {
            this.element.html(image);
        }
    },
    _deactivate: function () {
        /// <signature>
        /// <summary>Deactivates this module.</summary>
        /// </signature>
        this.element.empty();
    }
});