// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (ko) {
    "use strict";

    function strToFlags(str) {
        var flags = {};
        var array = str.split(" ");
        for (var i = array.length; --i >= 0;) {
            flags[array[i]] = true;
        }
        return flags;
    }

    ko.components.register("ui-datapager", {
        viewModel: function (params) {
            /// <signature>
            /// <summary>Creates a viewModel for pagination</summary>
            /// </signature>
            this.list = params.list;
            this.ctrls = strToFlags(params.ctrls || "row first prev input next last");
        },
        template:
            "<div class='ui-datapager'>" +
                "<!--ko if: ctrls.row--><span class='ui-datapager-info' data-bind='if: list.pageCount'><span data-bind='text: list.rowStart'></span>–<span data-bind='text: list.rowEnd'></span>&nbsp;/&nbsp;<span data-bind='text: list.total'></span></span><!--/ko-->" +
                "<!--ko if: ctrls.first--><button class='ui-btn ui-btn-page' type='button' data-bind='enable: list.hasPrevPage, click: function (d) { d.list.pageIndex = 1; }, clickBubble:false'><i class='ui-ico first'></i></button><!--/ko-->" +
                "<!--ko if: ctrls.prev--><button class='ui-btn ui-btn-page' type='button' data-bind='enable: list.hasPrevPage, click: function (d) { d.list.pageIndex = list.pageIndex - 1; }, clickBubble:false'><i class='ui-ico prev'></i></button><!--/ko-->" +
                "<!--ko if: ctrls.input--><span class='ui-datapager-input'><span><input type='text' data-bind='value: list.pageIndex' /></span><span>&nbsp;/&nbsp;</span><span data-bind='text:list.pageCount'></span></span><!--/ko-->" +
                "<!--ko if: ctrls.next--><button class='ui-btn ui-btn-page' type='button' data-bind='enable: list.hasNextPage, click: function (d) { d.list.pageIndex = list.pageIndex + 1; }, clickBubble:false'><i class='ui-ico next'></i></button><!--/ko-->" +
                "<!--ko if: ctrls.last--><button class='ui-btn ui-btn-page' type='button' data-bind='enable: list.hasNextPage, click: function (d) { d.list.pageIndex = list.pageCount; }, clickBubble:false'><i class='ui-ico last'></i></button><!--/ko-->" +
            "</div>"
    });

})(ko);