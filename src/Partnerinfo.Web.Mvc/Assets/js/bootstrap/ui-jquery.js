// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

$.fn.outerHTML = $.fn.outerHTML || function () {
    /// <signature>
    /// <summary>Gets the HTML markup representation of the element.</summary>
    /// </signature>
    return $("<div>").append(this.clone(false, false)).html();
};

function getTypeInfo(typeName) {
    var typeNames = typeName.split('.', 2);
    if (typeNames.length <= 1) {
        return { namespace: "ui", className: typeNames[0] };
    }
    return { namespace: typeNames[0], className: typeNames[1] };
}

ko.bindingHandlers.jQuery = {
    init: function (element, valueAccessor) {
        /// <signature>
        /// <summary>This will be called when the binding is first applied to an element.
        /// Set up any initial state, event handlers, etc. here.</summary>
        /// </signature>
        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            var typeInfo = getTypeInfo(valueAccessor());
            var $element = $(element);
            if ($element.is(":data(" + typeInfo.namespace + "-" + typeInfo.className + ")")) {
                $element[typeInfo.className]("destroy");
            }
        });
    },
    update: function (element, valueAccessor, allBindingsAccessor) {
        /// <signature>
        /// <summary>This will be called once when the binding is first applied to an element
        /// and again whenever the associated observable changes value.
        /// Update the DOM element based on the supplied values here.</summary>
        /// </signature>
        var typeInfo = getTypeInfo(valueAccessor()); // plugin name cannot be observable
        $(element)[typeInfo.className](ko.unwrap(allBindingsAccessor().jQueryOptions) || {});
    }
};