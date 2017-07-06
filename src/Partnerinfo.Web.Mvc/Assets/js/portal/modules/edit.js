// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="base.js" />

(function (_Portal) {
    "use strict";

    var modulePerms = _Portal.ModulePerms;
    var moduleState = _Portal.ModuleState;
    var moduleClasses = _Portal.ModuleClasses;

    $.widget("PI.PortalModule", $.PI.PortalModule, {
        _isInDesignTime: true,

        behaviors: function (enabled) {
            /// <signature>
            /// <summary>Attaches or detaches behaviors.</summary>
            /// <param name="enabled" type="Boolean" />
            /// </signature>
            this.element.is(":data(ui-resizable)") && this.element.resizable("destroy");
            enabled && this.element.resizable();
        },

        //
        // Edit Module
        //

        _createDialog: function (options) {
            /// <signature>
            /// <summary>Creates a dialog window for this module.</summary>
            /// <returns type="$.WinJS.dialog" />
            /// </signature>
            options = options || {};
            options.dialogClass = "ui-portal-dialog";
            options.title = options.title || this.options.engine.context.getType(this.options.typeClass).name;
            options.position = options.position || { my: "center top", at: "center top" };
            options.overflow = options.overflow || "visible";
            return $.WinJS.dialog(options);
        },
        _createEditDialog: function (moduleOptions, dialogOptions) {
            /// <signature>
            /// <summary>Creates a new module dialog for editing module settings.</summary>
            /// <param name="options" type="Object">A set of key/value pairs that can be used to configure module dialog settings.</param>
            /// <returns type="$.WinJS.dialog" />
            /// </signature>
            var that = this;
            var editor = { dialog: null, isCanceled: true };

            moduleOptions = moduleOptions || {};
            moduleOptions.model = moduleOptions.model || {};
            moduleOptions.model.portal = this.options.engine.portal;
            moduleOptions.model.page = this.options.engine.page;

            dialogOptions = dialogOptions || {};
            dialogOptions.dialogClass = "ui-dialog-module";
            dialogOptions.open = function () {
                editor.dialog = $(this).data("WinJS-dialog");
                that._onOpenEditDialog(editor, moduleOptions);
            };
            dialogOptions.close = this._onCloseEditDialog.bind(this, editor, moduleOptions);
            dialogOptions.buttons = dialogOptions.buttons || [{
                "class": "ui-btn ui-btn-primary",
                "text": _T("portal.actions.ok"),
                "click": this._onSubmitEditDialog.bind(this, editor, moduleOptions)
            }, {
                "class": "ui-btn",
                "text": _T("portal.actions.cancel"),
                "click": this._onCancelEditDialog.bind(this, editor, moduleOptions)
            }];

            return this._createDialog(dialogOptions);
        },
        _onCreateEditDialog: function (callback) {
            /// <signature>
            /// <param name="callback" type="Function" />
            /// </signature>
            callback("ok");
        },
        _onOpenEditDialog: function (editor, options) {
            /// <signature>
            /// <param name="editor" type="Object" />
            /// <param name="options" type="Object" />
            /// </signature>
            var module = options.model.module || this.getModuleOptions();
            var type = this.options.engine.context.getType(this.options.typeClass);

            options.model = options.model || {};
            options.model.module = ko.mapping.fromJS(module);
            options.model.events = [{ name: null }].concat(this.options.engine.context.filterEvents());
            options.view = options.view || ("ko" + type.module);

            if ($.isFunction(options.model.create)) {
                options.model.create(module);
            }

            editor.dialog.progress(PI.bind(
                editor.dialog.element,
                options.model,
                options.view,
                options.require));
        },
        _onSubmitEditDialog: function (editor) {
            /// <signature>
            /// <param name="editor" type="Object" />
            /// <param name="options" type="Object" />
            /// </signature>
            editor.isCanceled = false;
            editor.dialog.close();
        },
        _onCancelEditDialog: function (editor) {
            /// <signature>
            /// <param name="editor" type="Object" />
            /// <param name="options" type="Object" />
            /// </signature>
            editor.isCanceled = true;
            editor.dialog.close();
        },
        _onCloseEditDialog: function (editor, options) {
            /// <signature>
            /// <param name="editor" type="Object" />
            /// <param name="options" type="Object" />
            /// </signature>
            if (!editor.isCanceled && options.model.module) {
                var moduleOptions = ko.mapping.toJS(options.model.module);
                if ($.isFunction(options.model.submit)) {
                    options.model.submit(moduleOptions);
                }
                this.updateModuleOptions(moduleOptions);
            }
            if ($.isFunction(options.model.dispose)) {
                options.model.dispose(editor.isCanceled);
            }
            if ($.isFunction(options.complete)) {
                options.complete(editor.isCanceled ? "cancel" : "ok");
            }
        },
        edit: function (callback) {
            /// <signature>
            /// <summary>Opens the current module for editing with its associated editor.</summary>
            /// <param name="callback" type="Function" />
            /// </signature>
            var that = this;
            this.behaviors(false);
            this._onCreateEditDialog(function (result) {
                that.behaviors(true);
                callback(result);
            });
        },

        //
        // Activation Processes
        //

        showPreview: function (element) {
            /// <signature>
            /// <param name="element" type="HTMLElement" />
            /// <returns type="$.Deferred" />
            /// </signature>
            this.options.engine.context.parseAll(null, this.element
                .clone(false)
                .removeClass(moduleClasses.editMode)
                .removeClass(moduleClasses.selected)
                .css({
                    display: "",
                    position: "",
                    visibility: "",
                    float: "",
                    left: "",
                    top: "",
                    right: "",
                    bottom: "",
                    margin: 0,
                    opacity: "",
                    zIndex: ""
                })
                .appendTo(element));
        },

        //
        // Module Mode Operations
        //

        _setupMode: function (value) {
            /// <signature>
            /// <param name="value" type="PI.Portal.ModuleState" />
            /// </signature>
            this._deactivate();
            if (value === moduleState.edit) {
                this.element.addClass(moduleClasses.editMode);
                this.behaviors(true);
                return;
            }
            if (value === moduleState.view ||
                value === moduleState.active ||
                value === moduleState.inactive) {
                this.behaviors(false);
                this.element.removeClass(moduleClasses.editMode);
                if (value === moduleState.active
                    && this.options.active
                    && this.hasPermission()) {
                    this._activate();
                }
            }
        },
        _destroy: function () {
            /// <signature>
            /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            /// </signature>
            this.behaviors(false);
        }
    });

})(PI.Portal);
