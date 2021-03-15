// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

var koFlagChecked = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        /// <summary>This will be called when the binding is first applied to an element.
        /// Set up any initial state, event handlers, etc. here.</summary>
        var options = ko.utils.extend({
            defaultValue: "unknown",
            separator: ", ",
            clickBubble: false
        }, allBindingsAccessor().flagCheckedOptions);
        ko.utils.registerEventHandler(element, "click", function (event) {
            if (!options.clickBubble) {
                event.stopPropagation();
            }
            var value = valueAccessor();
            var valueObj = ko.unwrap(value);
            var flagsValue = valueObj ? valueObj.split(options.separator) : [];
            var enumItemValue = this.value;
            var checked = this.getAttribute("aria-checked") === "true" ? false : true;
            var i = flagsValue.indexOf(enumItemValue);
            if (checked) {
                if (i < 0) {
                    flagsValue.push(enumItemValue);
                }
            } else {
                if (i >= 0) {
                    flagsValue.splice(i, 1);
                }
            }
            if (!flagsValue.length) {
                flagsValue.push(options.defaultValue);
            } else {
                i = flagsValue.indexOf(options.defaultValue);
                if (i >= 0) {
                    flagsValue.splice(i, 1);
                }
            }
            koFlagChecked.setAriaChecked(this, checked);
            if (ko.isWriteableObservable(value)) {
                value(flagsValue.join(options.separator));
            }
        });
    },
    update: function (element, valueAccessor, allBindingsAccessor) {
        /// <summary>This will be called once when the binding is first applied to an element
        /// and again whenever the associated observable changes value.
        /// Update the DOM element based on the supplied values here.</summary>
        var options = ko.utils.extend({
            separator: ", ",
            clickBubble: false
        }, allBindingsAccessor().flagCheckedOptions);
        var valueObj = ko.unwrap(valueAccessor());
        var enumItemValue = element.value;
        var isChecked = valueObj && enumItemValue &&
            (valueObj === enumItemValue || valueObj.split(options.separator)
            .some(function (x) { return x === enumItemValue; }));
        koFlagChecked.setAriaChecked(element, isChecked);
    },
    setAriaChecked: function (element, value) {
        element.setAttribute("aria-checked", Boolean(value));
    }
};

//
// Public Namespace Exports
//

ko.bindingHandlers.flagChecked = koFlagChecked;