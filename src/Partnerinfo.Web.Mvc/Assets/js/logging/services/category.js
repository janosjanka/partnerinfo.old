// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (_WinJS, _PI) {
    "use strict";

    _WinJS.Namespace.defineWithParent(_PI, "Logging", {

        CategoryService: {
            getAllAsync: function (options, thisArg) {
                /// <signature>
                /// <summary>Gets all event categories</summary>
                /// <param name="options" type="Object" optional="true" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "GET",
                    path: "logging/categories",
                    data: options
                }, thisArg);
            },
            createAsync: function (category, thisArg) {
                /// <signature>
                /// <summary>Creates a new category</summary>
                /// <param name="category" type="Object" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "POST",
                    path: "logging/categories",
                    data: category
                }, thisArg);
            },
            updateAsync: function (category, thisArg) {
                /// <signature>
                /// <summary>Updates a category</summary>
                /// <param name="category" type="Object" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "PUT",
                    path: "logging/categories/" + category.id,
                    data: category
                }, thisArg);
            },
            deleteAsync: function (id, thisArg) {
                /// <signature>
                /// <summary>Gets all events</summary>
                /// <param name="id" type="Number" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "DELETE",
                    path: "logging/categories/" + id
                }, thisArg);
            }
        }

    });

})(WinJS, PI);