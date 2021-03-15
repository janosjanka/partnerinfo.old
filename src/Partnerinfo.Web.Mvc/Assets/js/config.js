// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (_Global, _WinJS, $) {
    "use strict";

    _WinJS.DEBUG = !!_Global.DEBUG;
    _WinJS.Resources.init("hu");
    _WinJS.Namespace.define("PI", _Global.PI);

    //
    // Configures the module loader
    //

    require.config({
        root: PI.Server.mapPath("/"),
        version: PI.config.version
    });

    //
    // Knockout Validation Plugin
    //

    ko.validation.init({
        registerExtenders: true,
        errorClass: "validation-summary-errors",
        errorElementClass: "input-validation-error",
        errorMessageClass: "validation-summary-errors",
        decorateInputElement: true,
        decorateElementOnModified: false,
        messageTemplate: null,
        messagesOnModified: false,
        insertMessages: false,
        parseInputAttributes: true
    });

    ko.validation.locale("hu-HU");

    //
    // Application ProgressBar
    //

    var progress = PI.progress = {
        _counter: 0,
        _element: null,
        _setup: function (id) {
            this._element = document.getElementById(id);
            if (this._element) {
                var oldRequire = _Global.require;
                _Global.require = function require() {
                    progress.acquire();
                    return oldRequire.apply(this, arguments).always(progress.release);
                };
                $(_Global)
                    .ajaxStart(progress.acquire)
                    .ajaxStop(progress.release)
                    .error(progress.release);
            }
        },
        acquire: function () {
            ++progress._counter;
            progress._element.style.display = "block";
        },
        release: function () {
            if (--progress._counter <= 0) {
                progress._counter = 0;
                progress._element.style.display = "none";
            }
        }
    };

    progress._setup("progress");

    //
    // DOM ready event handler
    //

    $(function () {
        $.uiBackCompat = false;

        ko.applyBindings();

        async(function () {
            $(".ui-fixed").scrollToFixed();
        });
    });

})(window, WinJS, jQuery);