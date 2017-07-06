// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (ko) {
    "use strict";

    var defaultColorPalette;

    function createDefaultColorPalette() {
        return [
            "#000000",
            "#660000", "#990000", "#cc0000", "#ff0000", "#ff9999",
            "#663300", "#994c00", "#cc6600", "#ff8000", "#ffcc99",
            "#666600", "#999900", "#cccc00", "#ffff00", "#ffff99",
            "#006600", "#009900", "#00cc00", "#00ff00", "#99ff99",
            "#006633", "#0099c4", "#00cc66", "#00ff80", "#99ffcc",
            "#006666", "#009999", "#00cccc", "#00ffff", "#99ffff",
            "#000066", "#000099", "#0000cc", "#0000ff", "#9999ff",
            "#660066", "#990099", "#cc00cc", "#ff00ff", "#ff99ff",
            "#660033", "#99004c", "#cc0066", "#ff007f", "#ff99cc",
            "#606060", "#808080", "#a0a0a0", "#c0c0c0", "#e0e0e0",
            "#ffffff", null
        ];
    }

    ko.components.register("ui-colorpicker", {
        viewModel: function (params) {
            /// <signature>
            /// <summary>Creates a viewModel for picking a color</summary>
            /// </signature>
            this.palette = params.palette || defaultColorPalette || (defaultColorPalette = createDefaultColorPalette());
            this.color = params.color;
            this.colorValue = ko.unwrap(this.color) || null;
            this.onclick = function (viewModel, event) {
                if (ko.isWriteableObservable(viewModel.color)) {
                    viewModel.color(ko.dataFor(event.target));
                }
            };
        },
        template:
            "<ul class='ui-colorpicker' data-bind='foreach: palette, click: onclick'>" +
                "<li data-bind='css: { selected: $data === $parent.colorValue, notset: $data === null }, style: { backgroundColor: $data }'></li>" +
            "</ul>"
    });

})(ko);