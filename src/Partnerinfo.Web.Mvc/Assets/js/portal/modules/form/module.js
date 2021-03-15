// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="../base.js" />

(function (_WinJS, _PI, _Portal) {
    "use strict";

    PI.Portal.Modules.register("form", {
        options: {
            container: true,
            onsuccess: null,
            onfailure: null
        },
        authorize: function () {
            /// <signature>
            /// <summary>Authorizes this module.</summary>
            /// </signature>
            this._super();

            // Fill all the input fields with the current contact
            var identity = this.options.engine.security.identity();
            identity && this.options.engine.context.dataBind({ contact: identity }, this.element);
        },
        submitAsync: function (options) {
            /// <signature>
            /// <summary>Submits this form to the given URL</summary>
            /// <param name="options" type="Object" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            var that = this;
            return $.ajax({
                type: options.method || "POST",
                url: options.action,
                data: this.toObject() //$("form", this.element).serialize()
            }).then(
                function (data, status, jqXHR) {
                    that._onSubmitDone(options, jqXHR.getResponseHeader("Location"));
                },
                function () {
                    that._onSubmitFail(options);
                });
        },
        toObject: function () {
            /// <signature>
            /// <returns type="Object" />
            /// </signature>
            var form;
            var formElement = $("form:first", this.element);
            var moduleOptions = this.getModuleOptions();
            if ($.isFunction($.fn.toObject)) {
                form = formElement.toObject();
            } else {
                form = {};
                formElement.serializeArray().forEach(function (item) {
                    if ((item.name) &&
                        (item.value !== undefined) &&
                        (item.value !== "")) {
                        var props = item.name.split('.');
                        var last = props.length - 1;
                        var curr = form;
                        for (var i = 0; i < last; ++i) {
                            var p = props[i];
                            curr = curr[p] = curr[p] || {};
                        }
                        curr[props[last]] = item.value;
                    }
                });
            }
            if (moduleOptions.task
                && moduleOptions.task.type
                && moduleOptions.task.type !== "none") {
                form[moduleOptions.task.type] = form[moduleOptions.task.type] || {};
                form[moduleOptions.task.type].action = moduleOptions.task.action;
            }
            return form;
        },
        createModuleOptions: function (options) {
            /// <signature>
            /// <param name="options" type="Object" optional="true" />
            /// <returns type="Object" />
            /// </signature>
            if (options
                && options.action
                && typeof options.action !== "string") {
                options.action = options.action.url;
            }
            return $.extend(this._super(options), {
                action: null,     // Form action (url)
                method: "POST",   // Form method (post, put)
                target: null,     // Form target (self)
                postback: false,  // True if postback is used instead of AJAX
                redirect: true,   // Allows HTTP redirect if a location header is specified in the response
                task: {
                    type: "none",
                    action: null
                },
                onsuccess: null,  // Raised when a form is successfully posted
                onfailure: null   // Raised when a form post fails
            }, options);
        },
        _activate: function () {
            /// <signature>
            /// <summary>Activates this module.</summary>
            /// </signature>
            var that = this;
            var options = this.getModuleOptions();
            var form = $("<form>").attr({
                action: options.action || "/",
                method: options.method || "POST",
                target: options.target || "_self"
            });
            if (!options.postback) {
                form.on("submit", function (event) {
                    event.preventDefault();
                    that.submitAsync(options);
                });
            }
            this.element.wrapInner(form);
        },
        _deactivate: function () {
            /// <signature>
            /// <summary>Deactivates this module.</summary>
            /// </signature>
            $("form>*", this.element).unwrap();
        },
        _onSubmitDone: function (options, location) {
            /// <signature>
            /// <param name="options" type="Object" />
            /// <param name="location" type="String" />
            /// </signature>
            if (options.redirect && location) {
                window.open(location, "_self");
                return;
            }
            if (options.onsuccess) {
                this.options.engine.context.executeEvent(options.onsuccess);
            }
            if ($.isFunction(this.options.onsuccess)) {
                this.options.onsuccess(location);
            }
        },
        _onSubmitFail: function (options) {
            /// <signature>
            /// <param name="options" type="Object" />
            /// </signature>
            if (options.onfailure) {
                this.options.engine.context.executeEvent(options.onfailure);
            }
            if ($.isFunction(this.options.onfailure)) {
                this.options.onfailure();
            }
        }
    });

})(WinJS, PI, PI.Portal);