// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

PI.Portal.Modules.extend("content", {
    _create: function () {
        /// <signature>
        /// <summary>Initializes a new instance of this module.</summary>
        /// </signature>
        if (this.element.children().length === 0) {
            this.element.html("<!-- content-place-holder -->");
        }
        this._super();
    }
});