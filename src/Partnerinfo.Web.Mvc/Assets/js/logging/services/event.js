// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (_WinJS, _PI) {
    "use strict";

    _WinJS.Namespace.defineWithParent(_PI, "Logging", {

        EventService: {
            downloadFile: function (options, thisArg) {
                /// <signature>
                /// <summary>Gets all events</summary>
                /// <param name="options" type="Object" optional="true" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                window.open(_PI.api.action("logging/events/download", options), "_blank");
            },
            getAllAsync: function (options, thisArg) {
                /// <signature>
                /// <summary>Gets all events</summary>
                /// <param name="options" type="Object" optional="true" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "GET",
                    path: "logging/events",
                    data: options
                }, thisArg);
            },
            setAllCategoriesAsync: function (options, thisArg) {
                /// <signature>
                /// <summary>Sets the given category for a group of events</summary>
                /// <param name="options" type="Object">
                ///     <para name="categoryId" type="Object" />
                ///     <para name="ids" type="Array" />
                ///     <para name="filter" type="Object" />
                /// </param>
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                options = options || {};
                options.categoryId = options.categoryId || null;
                return _PI.api({
                    method: "POST",
                    path: "logging/events/category",
                    data: options
                }, thisArg);
            },
            setAllContactsAsync: function (options, thisArg) {
                /// <signature>
                /// <summary>Sets the given category for a group of events</summary>
                /// <param name="options" type="Object">
                ///     <para name="contactId" type="Object" />
                ///     <para name="ids" type="Array" />
                ///     <para name="filter" type="Object" />
                /// </param>
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                options = options || {};
                options.contactId = options.contactId || null;
                return _PI.api({
                    method: "POST",
                    path: "logging/events/contact",
                    data: options
                }, thisArg);
            },
            deleteAllAsync: function (options, thisArg) {
                /// <signature>
                /// <summary>Gets all events</summary>
                /// <param name="options" type="Object">
                ///     <para name="ids" type="Array" />
                ///     <para name="filter" type="Object" />
                /// </param>
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "DELETE",
                    path: "logging/events",
                    data: options
                }, thisArg);
            }
        }

    });

})(WinJS, PI);