// Copyright (c) Partnerinfo Ltd. All Rights Reserved.
/// <reference path="../services/event.js" />
/// <reference path="../viewmodels/event.js" />

(function () {
    "use strict";
    /*
    var ns = WinJS.Namespace.defineWithParent(PI, "Logging", {
        LogEventListViewOptions: Class.define(function LogEventListViewOptions_ctor(options) {
            /// <signature>
            /// <summary>Initializes a new instance of the LogEventListViewOptions class.</summary>
            /// <param name="options" type="Object" />
            /// <returns type="PI.Logging.LogEventListViewOptions" />
            /// </signature>
            this.listViewOptions = {
                displayRowIndex: true,
                synchronize: true,
                selectionMode: "multi",
                items: options.items,
                columns: [
                    { header: "Esemény tárgya", cellTemplate: function (bindings) { var filter = bindings.list.filter; return filter.project() ? "#koEventListSourceCell" : "#koEventListSourceAndProjectCell" }, minWidth: "150px" },
                    { header: "Egyedi webcím", cellTemplate: "#koEventListCustomUriCell", width: "150px" },
                    { header: "", cellTemplate: "#koEventListCategoryCell", width: "15px" },
                    { header: "Felhasználó", cellTemplate: "#koEventListContactCell", width: "150px" },
                    { header: "Kliens ID", cellTemplate: "#koEventListClientCell", width: "90px" },
                    { header: "Böngésző", cellTemplate: "#koEventListBrowserCell", width: "60px", headerClass: "ui-text-right", cellClass: "ui-text-right ui-type-number" },
                    { header: "Időpont", cellTemplate: "#koEventListStartDateCell", width: "125px", headerClass: "ui-text-right", cellClass: "ui-text-right ui-type-time" },
                    { header: "Hossz", binding: "text: finishElapsedTimeText, attr: { title: finishDateText }", width: "60px", headerClass: "ui-text-right", cellClass: "ui-text-right ui-type-duration" },
                    { header: "Utolsó", binding: "text: correlationElapsedTimeText, attr: { title: correlationStartDateText }", width: "60px", headerClass: "ui-text-right", cellClass: "ui-text-right ui-type-duration" },
                    { header: "", cellTemplate: "#koEventListReferrerUrlCell", width: "16px" }
                ]
            };
            this.menuEnabled = options.menuEnabled !== false;
            this.filterEnabled = options.filterEnabled !== false;
            this.pagerEnabled = options.pagerEnabled !== false;
        })
    });
    */
    PI.component({
        name: "logging.events",
        model: function (options) {
            /// <signature>
            /// <summary>Returns a bindable model object</summary>
            /// <returns type="PI.Logging.LogEventList" />
            /// </signature>
            return new PI.Logging.EventList(options);
        },
        view: function (model, options) {
            /// <signature>
            /// <summary>Returns a Promise object</summary>
            /// <returns type="WinJS.Promise" />
            /// </signature>
            var template;
            if (options.displayMode === "reduced") {
                template = "koEventListReduced";
            } else {
                template = "koEventListFull";
            }
            return WinJS.Promise.join(
                this.render(options.element, model, template),
                options.categories && this.render(options.categories.element, model.categories, "koEventCategoryList"));
        }
    });

})();