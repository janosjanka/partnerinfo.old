// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (_WinJS, _PI) {
    "use strict";

    var _Promise = _WinJS.Promise;

    var modules = {
        "confirm": {},
        "identity.users": { require: ["identity", "identity/users.html"] },
        "logging.events": { require: ["logging", "logging.css", "logging/events.html"] },
        "logging.rules": { require: ["logging", "logging/rules.html"] },
        "input.command": { require: ["input"] },
        "project": { require: ["project", "project.css", "project/project.html"] },
        "project-picker": { require: ["project", "project.css", "project/projects.html"] },
        "projects": { require: ["project", "project.css", "project/projects.html"] },
        "project.businesstags": { require: ["project", "project.css", "project/businesstags.html"] },
        "project.contact": { require: ["project", "project.css", "project/contact.html"] },
        "project.contacts": { require: ["fileupload", "fileupload.css", "project", "project.css", "project/contacts.html", "project/businesstags.html"] },
        "project.mail": { require: ["tinymce", "tinymce.css", "project", "project.css", "project/mail.html"] },
        "project.mails": { require: ["project", "project.css", "project/mails.html"] },
        "project.action": { require: ["project", "project.css", "project/action.html"] },
        "project.actions": { require: ["project", "project.css", "project/actions.html"] },
        "project.action-picker": { require: ["project", "project.css", "project/actionpicker.html"] },
        "project.actionlink": { require: ["project", "project.css", "project/actionlink.html"] },
        "project.actionlink-element": { require: ["project", "project.css", "project/actionlink.html"] },
        "drive": { require: ["fileupload", "fileupload.css", "drive", "drive.css", "drive/files.html"] },
        "drive.filepicker": { require: ["fileupload", "fileupload.css", "drive", "drive.css", "drive/files.html"] },
        "portal": { require: ["portal", "portal.css", "portal/portal.html"] },
        "portal.uri": { require: ["portal", "portal.css", "portal/portal.html"] },
        "portal.host": { require: ["portal", "portal.css", "portal/portal.html"] },
        "portals": { require: ["portal", "portal.css", "portal/portals.html"] },
        "portal.page": { require: ["portal", "portal.css", "portal/page.html"] },
        "portal.pages": { require: ["portal", "portal.css"] },
        "portal.media": { require: ["fileupload", "fileupload.css", "portal", "portal.css", "portal/media.html"] },
        "portal.media-editor": { require: ["tinymce", "tinymce.css", "portal", "portal.css"] },
        "portal.designer": { require: ["portal", "portal.css"] },
        "search": { require: ["search", "search.css", "search/search.html"] },
        "security.acl": { require: ["identity", "security", "security/acl.html"] }
    };

    var templateOptions = { templateEngine: ko.nativeTemplateEngine.instance };

    function select(selectorOrElement) {
        /// <signature>
        /// <param name="selectorOrElement" type="String">The jQuery selector expression or DOM element.</param>
        /// <returns type="HTMLElement" />
        /// </signature>
        return typeof selectorOrElement === "string"
            ? document.querySelector(selectorOrElement)
            : selectorOrElement && selectorOrElement.length
                ? selectorOrElement[0]
                : selectorOrElement;
    }

    function bind(selectorOrElement, viewModel, view, requires) {
        /// <signature>
        /// <summary>Renders a knockout template.</summary>
        /// <param name="selectorOrElement" type="String">The jQuery selector expression or DOM element.</param>
        /// <param name="viewModel" type="Object">The view model to render.</param>
        /// <param name="view" type="String" optional="true">The unique identifier of the template script element.</param>
        /// <param name="requires" type="Array" optional="true">An array of code dependencies.</param>
        /// <returns type="WinJS.Promise" />
        /// </signature>
        var element = select(selectorOrElement);
        if (!element) {
            return _Promise.error();
        }
        if (requires) {
            return require(requires).then(
                function () {
                    ko.renderTemplate(view, viewModel, templateOptions, element);
                });
        }
        ko.renderTemplate(view, viewModel, templateOptions, element);
        return _Promise.complete();
    }

    function componentDialogFactory() {
        /// <signature>
        /// <summary>Creates a dialog window.</summary>
        /// <returns type="$.WinJS.dialog" />
        /// </signature>
        return $.WinJS.dialog({
            width: 640,
            height: 480,
            resizable: true
        });
    }

    function component(options) {
        /// <signature>
        /// <summary>Registers a new component</summary>
        /// <param name="options" type="Object">
        ///     <para>name: String - Unique name</para>
        ///     <para>model: Function - A factory function that constructs a model object</para>
        ///     <para>view: Function - A factory function that renders a view for the model object</para>
        ///     <para>dialog: Function - A factory function that renders a view in a dialog window</para>
        /// </param>
        /// </signature>
        var comp = modules[options.name] || {};

        // Expose component APIs as internal functions.
        comp._renderView = comp.render = bind;
        comp._renderDialog = $.WinJS.dialog;

        comp.require = comp.require || options.require || [];
        comp.model = comp.model || options.model || _WinJS.noop;
        comp.view = comp.view || options.view || _WinJS.noop;
        comp.dialog = comp.dialog || options.dialog || componentDialogFactory;

        modules[options.name] = comp;
    }

    function ui(options) {
        /// <signature>
        /// <summary>Displays a view using the specified options.</summary>
        /// <param name="options" type="Object">A set of key/value pairs that can be used to configure the UI form.
        ///     <para>name: String - The name of the UI form.</para>
        ///     <para>element: HTMLElement - The element to bind.</para>
        ///     <para>model?: Object - The model to bind.</para>
        /// </param>
        /// <returns type="$.Deferred" />
        /// </signature>
        var module;
        if (!options || !options.name || !options.element || !(module = modules[options.name])) {
            throw new Error("PI.ui({ name: \"" + (options && options.name) + "\" })");
        }
        return require(module.require).then(function () {
            module.view(module.model(options), options, false);
        });
    }

    function dialog(options) {
        /// <signature>
        /// <summary>Displays a dialog window using the specified options.</summary>
        /// <param name="options" type="Object">A set of key/value pairs that can be used to configure the dialog.
        ///     <para>name: The name of the dialog.</para>
        ///     <para>mode: The dialog mode (optional).</para>
        ///     <para>done: A callback function that will be called after closing a dialog window (optional).</para></param>
        /// </signature>
        var module;
        if (!options || !options.name || !(module = modules[options.name])) {
            throw new Error("PI.dialog({ name: \"" + (options && options.name) + "\" })");
        }
        require(module.require).then(function () {
            var model = module.model(options);
            var response = { result: "cancel" };
            var dialog = module.dialog(model, options, response,
                function (result, data) {
                    if (dialog) {
                        response.data = response.data || $.isFunction(model.toObject) ? model.toObject() : null;
                        response.result = result || response.result;
                        dialog.close();
                    }
                });
            dialog.element.on("dialogclose", function () {
                if ($.isFunction(options.done)) {
                    options.done.call(model, response);
                }
            });
            options.element = dialog.element[0];
            var promise = module.view(model, options, true /* Dialog */);
            promise && dialog.progress(promise.always(function () { dialog.realign(); }));
        });
    }

    //
    // System UI & Dialog
    //

    component({
        name: "confirm",
        model: function (options) {
            return {
                title: options.title || _T("pi/confirm"),
                content: options.content || _T("pi/confirm-" + options.type)
            };
        },
        dialog: function (model, options, response, callback) {
            return this._renderDialog({
                title: model.title,
                content: model.content,
                buttons: [{
                    "class": "ui-btn ui-btn-primary",
                    "text": _T("ui/yes"),
                    "click": callback.bind(model, "yes")
                }, {
                    "class": "ui-btn",
                    "text": _T("ui/no"),
                    "click": callback.bind(model, "no")
                }]
            });
        }
    });

    //
    // Exports
    //

    _WinJS.Namespace.defineWithParent(_PI, null, {
        bind: bind,
        component: component,
        ui: ui,
        dialog: dialog
    });

    //
    // Knockout Binding Handlers
    //

    ko.bindingHandlers.ui = {
        init: function (element, valueAccessor) {
            /// <signature>
            /// <summary>This will be called when the binding is first applied to an element.
            /// Set up any initial state, event handlers, etc. here.</summary>
            /// </signature>
            var options = ko.unwrap(valueAccessor()) || {};
            options.element = options.element || element;
            _PI.ui(options);
            return { controlsDescendantBindings: true };
        }
    };

})(WinJS, PI);
