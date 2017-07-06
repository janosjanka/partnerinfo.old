// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (ko) {
    "use strict";

    ko.components.register("ui-dataerrors", {
        viewModel: function (params) {
            /// <signature>
            /// <summary>Creates a viewModel for presenting data errors</summary>
            /// </signature>
            this.errors = params.errors;
            this.errorsClass = {};
            this.errorsClass[params.errorsClass || "validation-summary-errors"] = true;
        },
        template:
            "<div data-bind='css: errorsClass, visible: errors().length'><ul data-bind='foreach: errors'><li data-bind='text: $data'></li></ul></div>"
    });

})(ko);
