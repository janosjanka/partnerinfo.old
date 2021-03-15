// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (global, ko, $) {
    "use strict";

    // Inspired by twitter.com's autocomplete search functionality,
    // typeahead.js is a flexible JavaScript library that provides a strong foundation for building robust typeaheads
    // https://github.com/twitter/typeahead.js/blob/master/doc/jquery_typeahead.md  

    function JQueryCtx(element) {
        /// <signature>
        /// <param name="element" type="HTMLElement" />
        /// <returns type="jQuery" />
        /// </signature>
        return ((element.ownerDocument.defaultView || element.ownerDocument.parentWindow || global).$ || $)(element);
    }

    ko.bindingHandlers.typeahead = {
        after: ["value"],
        init: function (element, valueAccessor, allBindings) {
            /// <signature>
            /// <summary>This will be called when the binding is first applied to an element.
            /// Set up any initial state, event handlers, etc. here.</summary>
            /// </signature>
            var options = valueAccessor();
            var dataSource = options.dataSource || {};
            var value = allBindings.get("value") || options.value;
            var $element = JQueryCtx(element);

            $element.typeahead({
                highlight: options.highlight !== false,
                hint: options.hint !== false,
                minLength: options.minLength || 1,
                classNames: {
                    wrapper: "ui-typeahead",
                    input: "ui-typeahead-input",
                    hint: "ui-typeahead-hint",
                    menu: "ui-typeahead-menu",
                    dataset: "ui-typeahead-dataset",
                    selectable: "ui-typeahead-selectable",
                    suggestion: "ui-typeahead-suggestion",
                    empty: "ui-typehead-empty",
                    open: "ui-typeahead-open",
                    cursor: "ui-typeahead-cursor",
                    highlight: "ui-typeahead-highlight"
                }
            }, {
                source: dataSource.source,
                name: dataSource.name,
                async: dataSource.async,
                limit: dataSource.limit,
                display: dataSource.display,
                templates: dataSource.templates
            });

            ko.isWritableObservable(value) && $element
                .bind("typeahead:change typeahead:select", function (e, suggestion) {
                    // Typehead automatically updates the input control so we can use
                    // the mapped value (display option) instead of raw object graph (suggestion)
                    // to notify Knockout about changes. An input control always works only with simple strings not complex objects.
                    // value(suggestion);
                    value($element.val());
                });

            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                // This will be called when the element is removed by Knockout or
                // if some other part of your code calls ko.removeNode(element)
                $element.typeahead("destroy");
            });
        },
        update: function (element, valueAccessor, allBindings) {
            /// <signature>
            /// <summary>This will be called once when the binding is first applied to an element
            /// and again whenever the associated observable changes value.
            /// Update the DOM element based on the supplied values here.</summary>
            /// </signature>
            if (!allBindings.has("value")) {
                JQueryCtx(element).val(ko.unwrap(valueAccessor().value));
            }
        }
    };

})(window, ko, jQuery);