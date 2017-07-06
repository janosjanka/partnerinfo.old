// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (_WinJS, _KO) {
    "use strict";

    var _Utilities = _WinJS.Utilities;

    _KO.utils.optionsMap = function (options, mapping) {
        /// <signature>
        /// <summary>Creates an options array for select elements.</summary>
        /// <param name="options" type="Object" />
        /// <param name="mapping" type="Function" optional="true" />
        /// <returns type="Array" />
        /// </signature>
        var array = [];
        for (var p in options) {
            if (options.hasOwnProperty.apply(options, [p])) {
                if (mapping) {
                    array.push(mapping(p, options[p]));
                } else {
                    array.push({ text: options[p], value: p });
                }
            }
        }
        return array;
    };

    _KO.utils.extend(_KO.bindingHandlers, {
        video: {
            update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
                /// <summary>This will be called once when the binding is first applied to an element
                /// and again whenever the associated observable changes value.
                /// Update the DOM element based on the supplied values here.</summary>
                var value = valueAccessor();
                var allBindings = allBindingsAccessor();
                var valueObj = _KO.unwrap(value);
                var options = allBindings.videoOptions || {};
                async(function () {
                    var iframe = "";
                    var descriptor = _Utilities.parseMediaLink(valueObj, options);
                    if (descriptor.id) {
                        iframe += '<iframe frameborder="0" width="100%" height="100%" src="';
                        iframe += descriptor.url;
                        iframe += '" />';
                    }
                    element.innerHTML = iframe;
                });
            }
        },
        elapsed: {
            update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
                /// <summary>This will be called once when the binding is first applied to an element
                /// and again whenever the associated observable changes value.
                /// Update the DOM element based on the supplied values here.</summary>
                var value = _KO.unwrap(valueAccessor());
                if (value) {
                    value = new Date().elapsed(new Date(Date.parse(value)));
                }
                _Utilities.setText(element, value);
            }
        }
    });

})(WinJS, ko);