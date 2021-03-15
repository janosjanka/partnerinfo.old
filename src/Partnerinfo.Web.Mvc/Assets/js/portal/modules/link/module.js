// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="../base.js" />

PI.Portal.Modules.register("link", {
    options: {
        container: true
    },
    createModuleOptions: function (options) {
        /// <signature>
        /// <summary>Creates a new object that extends the given options with the module's default options.</summary>
        /// <param name="options" type="Object" optional="true" />
        /// <returns type="Object" />
        /// </signature>
        return $.extend(this._superApply(arguments), {
            url: null,
            target: null
        }, options);
    },
    _activate: function () {
        /// <signature>
        /// <returns type="WinJS.Promise" />
        /// </signature>
        var moduleOptions = this.getModuleOptions();
        this.element.wrapInner(
            $("<a>").attr({
                href: moduleOptions.url,
                target: moduleOptions.target
            }));
    },
    _deactivate: function () {
        /// <signature>
        /// <returns type="WinJS.Promise" />
        /// </signature>
        $(">a>*", this.element).unwrap();
    }
});