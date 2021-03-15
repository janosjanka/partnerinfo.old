// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (global, globalize, ko, $) {
    "use strict";

    function JQueryCtx(element) {
        return ((element.ownerDocument.defaultView || element.ownerDocument.parentWindow || global).$ || $)(element);
    }
    
    ko.bindingHandlers.fileupload = {
        init: function (element, valueAccessor, allBindings) {
            /// <signature>
            /// <summary>This will be called when the binding is first applied to an element.
            /// Set up any initial state, event handlers, etc. here.</summary>
            /// </signature>
            var options = valueAccessor();
            var $element = JQueryCtx(element);
            $element.fileupload(options);

            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                // This will be called when the element is removed by Knockout or
                // if some other part of your code calls ko.removeNode(element)
                $element.fileupload("destroy");
            });
        }
    };

    ko.components.register("ui-fileupload", {
        viewModel: function (params) {
            /// <signature>
            /// <summary>Creates a viewModel for pagination</summary>
            /// </signature>
            this.text = params.text || globalize.localize("ui/fileUploadBtn");
            this.options = params.options || {};
        },
        template:
            "<div role='button' class='fileinput-button'>" +
                '<p data-bind="text: text"></p>' +
                '<input type="file" multiple="multiple" data-bind="fileupload: options" />' +
            "</div>"
    });

})(window, Globalize, ko, jQuery);