// TokenInput plugin v1.0
// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (ko, $, undefined) {

    "use strict";

    function tokenMapper(tokenResults, tokenResultsMapper) {
        var len = tokenResults.length;
        var list = new Array(len);
        for (var i = 0; i < len; ++i) {
            var result = ko.mapping.toJS(tokenResults[i]);
            if (tokenResultsMapper) {
                var data = tokenResultsMapper.apply(this, [result]);
                if (data) {
                    result["__ko_token"] = data.id;
                    data.result = result;
                    list[i] = data;
                }
            } else {
                result["__ko_token"] = result.id;
                list[i] = {
                    id: result.id,
                    name: result.name,
                    result: result
                };
            }
        }
        return list;
    }

    ko.bindingHandlers.token = {
        init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            /// <summary>This will be called when the binding is first applied to an element.
            /// Set up any initial state, event handlers, etc. here.</summary>
            var allBindings = allBindingsAccessor();
            var params = ko.utils.extend({
                queryParam: "q",
                preventDuplicates: true,
                searchDelay: 300,
                animateDropdown: false,
                hintText: null,
                noResultsText: null,
                searchingText: null
            }, allBindings.tokenParams);
            params.onResult = function (response) {
                var allBindings = allBindingsAccessor();
                var results = response[allBindings.tokenResults || "data"];
                return tokenMapper(results || response, allBindings.tokenResultsMapper);
            };
            params.onAdd = function (item) {
                var value = valueAccessor();
                if (ko.isWriteableObservable(value)) {
                    var array = ko.unwrap(value);
                    if (array === null || array === undefined) {
                        value([item.result]);
                    } else {
                        if (!ko.utils.arrayFirst(array, function (x) { return x["__ko_token"] == item.id; })) {
                            value.push(item.result);
                        }
                    }
                }
            };
            params.onDelete = function (item) {
                var value = valueAccessor();
                if (ko.isWriteableObservable(value)) {
                    value.remove(function (x) { return x["__ko_token"] == item.id; });
                }
            };
            $(element).tokenInput(allBindings.tokenUrl, params);
        },
        update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            /// <summary>This will be called once when the binding is first applied to an element
            /// and again whenever the associated observable changes value.
            /// Update the DOM element based on the supplied values here.</summary>
            var array = ko.unwrap(valueAccessor());
            if (array !== null && array !== undefined) {
                var that = $(element);
                var mapped = tokenMapper(array, allBindingsAccessor().tokenResultsMapper);
                array.splice(0, array.length);
                that.tokenInput("clear");
                for (var i = 0, len = mapped.length; i < len; ++i) {
                    var data = mapped[i];
                    array.push(data.result);
                    that.tokenInput("add", data);
                }
            }
        }
    };

})(ko, jQuery);
