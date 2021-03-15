// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="base.js" />

(function (Resources, Portal) {
    "use strict";

    var liveEditorResources = {
        editByMail: function () { return "Weboldalának szerkesztése emaillel"; },
        editByMailDescription: function () { return "FONTOS! Arról az email címéről tudja szerkeszteni az oldalát, amellyel regisztrált a rendszerbe!"; },
        deleteConfirmText: function () { return "Biztos, hogy törölni szeretné a modult?"; }
    };

    var MailDialog = WinJS.Class.define(function MailDialog_ctor(options) {
        /// <signature>
        /// <summary>Initializes a new instance of the MailDialog class.</summary>
        /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the MailDialog.</param>
        /// <returns type="MailDialog" />
        /// </signature>
        options = options || {};
        this.autoOpen = options.autoOpen !== false;
        this.element = document.createElement("div");
        this.saveText = options.saveText || "Küldés";
        this.cancelText = options.cancelText || "Mégse";
        this.onsave = options.onsave || WinJS.noop;
        this.oncancel = options.oncancel || WinJS.noop;
        this.mail = ko.observable(options.mail);
        this.autoOpen && this.show();
    }, {
        show: function (options) {
            /// <signature>
            /// <summary>Shows a dialog that prompts the user to give us his email address.</summary>
            /// <param name="options" type="Object" />
            /// </signature>
            if (MailDialog.mail) {
                this.onsave(MailDialog.mail);
                return;
            }
            var that = this;
            var canceled = true;
            $.WinJS.dialog({
                width: 450,
                position: { my: "center top", at: "center top", of: window },
                title: liveEditorResources.editByMail(),
                content: $(that.element).append(
                    $("<div>").addClass("conVerXS ui-text-center ui-type-note").text(liveEditorResources.editByMailDescription()),
                    $("<div>").addClass("conVerXS").css({ "padding": "4px", "background": "#eee" }).append(
                        $("<div>").css({ "margin": "0 0 4px 0", "padding": "4px", "background": "#000", "color": "#fff" }).text("1. Írja (másolja) ide e-mail címét!"),
                        $("<input>").attr({ "type": "email", "data-bind": "value: mail" })
                    )
                ),
                open: function () {
                    ko.applyBindings(that, that.element);
                },
                close: function () {
                    ko.cleanNode(that.element);
                    canceled && that.oncancel();
                },
                buttons: [{
                    "class": "ui-btn ui-btn-primary",
                    "text": this.saveText,
                    "click": function () {
                        that.onsave(MailDialog.mail = that.mail());
                        canceled = false;
                        $(this).dialog("close");
                    }
                }, {
                    "class": "ui-btn",
                    "text": this.cancelText,
                    "click": function () {
                        $(this).dialog("close");
                    }
                }]
            });
        }
    }, {
        /// <field type="String">
        /// A mail address that is shared with all the editor instances.
        /// </field>
        mail: null
    });

    var LiveEditor = WinJS.Class.define(function LiveEditor_ctor(module, options) {
        /// <signature>
        /// <summary>Initializes a new instance of the LiveEditor class.</summary>
        /// <param name="module" type="jQuery" />
        /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the LiveEditor.</param>
        /// <returns type="LiveEditor" />
        /// </signature>
        options = options || {};
        this.module = module;
        this.element = module.element.addClass("ui-module-liveedit");
        this._onEditClickBound = this._onEditClick.bind(this);
        this._onDeleteClickBound = this._onDeleteClick.bind(this);
        this._menu = $('<div class="ui-module-liveedit-menu">')
            .append($('<a class="ui-btn ui-btn-inverse ui-btn-sm">').on("click", this._onEditClickBound).append($('<span>').text(this.element.attr("id"))))
            .append($('<a class="ui-btn ui-btn-primary ui-btn-sm">').on("click", this._onDeleteClickBound).append($('<i>').addClass("i delete")));
        this.showMenu(true);
    }, {
        _onEditClick: function (event) {
            /// <signature>
            /// <param name="event" type="MouseEvent" />
            /// <returns type="Boolean" />
            /// </signature>
            event.stopImmediatePropagation();
            var typeClass = this.module.options.typeClass;
            if (typeClass === "ui-module-html") {
                this.showTextDialog();
                return false;
            }
            if (typeClass === "ui-module-image") {
                this.showImageDialog();
                return false;
            }
            return false;
        },
        _onDeleteClick: function (event) {
            /// <signature>
            /// <param name="event" type="MouseEvent" />
            /// <returns type="Boolean" />
            /// </signature>
            event.stopImmediatePropagation();
            this.showDeleteDialog();
            return false;
        },
        showMenu: function (enable) {
            /// <signature>
            /// <summary>Enables/disables the live editor.</summary>
            /// <param name="enable" type="Boolean" />
            /// </signature>
            if (!this._menu) {
                return;
            }
            if (enable) {
                this._menu.appendTo(this.element);
            } else {
                this._menu.detach();
            }
        },
        showTextDialog: function (options) {
            /// <signature>
            /// <summary>Shows a dialog that enables a user to edit a text module.</summary>
            /// <param name="options" type="Object" />
            /// <returns type="$.WinJS.dialog" />
            /// </signature>
            this.showMenu(false);
            var that = this;
            var saved = false;
            var model = { module: this.module, content: ko.observable(this.element.html()) };
            var hostElement = document.createElement("div");
            return $.WinJS.dialog({
                title: "Szerkesztés",
                width: 640,
                position: { my: "center top", at: "center top", of: window },
                resizable: true,
                content: $(hostElement).attr({
                    "id": "piLiveHtmlEditor",
                    "data-bind": 'component: { name: "pi-tinymce", params: {' +
                        'id: "koPortalHtmlModuleInput",' +
                        'value: content,' +
                        'options: {' +
                            'autoresize: true,' +
                            'cssMode: "system",' +
                            'config: {' +
                                'project: module.options.engine.portal && module.options.engine.portal.project,' +
                            '}' +
                        '}' +
                    '} }'
                }),
                open: function () {
                    var $this = $(this);
                    $this.dialog("progress", require(["tinymce", "tinymce.css"]).then(function () {
                        ko.applyBindings(model, hostElement);
                    }));
                },
                close: function () {
                    hostElement && ko.cleanNode(hostElement);
                    async(function () { that.showMenu(true); }, this, 4000);
                },
                buttons: [{
                    "class": "ui-btn ui-btn-primary",
                    "text": "Mentés",
                    "click": function () {
                        var $dialog = $(this);
                        var oldContent = that.element.html();
                        var newContent = model.content();
                        that.element.html(newContent);
                        new MailDialog({
                            onsave: function (mail) {
                                $dialog.dialog("progress",
                                    that.requestAsync("UPDATE", { address: mail }, newContent).done(
                                        function () {
                                            $dialog.dialog("close");
                                        }));
                            },
                            oncancel: function () {
                                that.element.html(oldContent);
                            }
                        });
                        saved = true;
                    }
                }, {
                    "class": "ui-btn",
                    "text": "Bezár",
                    "click": function () {
                        $(this).dialog("close");
                    }
                }]
            });
        },
        showImageDialog: function () {
            /// <signature>
            /// <summary>Shows a dialog that enables a user to edit an image module.</summary>
            /// <param name="options" type="Object" />
            /// <returns type="$.WinJS.dialog" />
            /// </signature>
            var that = this;
            var moduleOptions = this.module.getModuleOptions();
            var model = {
                $dialog: null,
                url: ko.observable(moduleOptions.image.url),
                onSubmit: function (e, d) {
                    new MailDialog({
                        onsave: function (mail) {
                            return $(e.target)
                                .fileupload("option", "url", "/filestore?mail=" + window.encodeURIComponent(mail))
                                .fileupload("send", d)
                                .then(function (response) {
                                    var url = response.data[0].link;
                                    model.url(url);
                                    model.$dialog.dialog("progress",
                                        that.requestAsync("UPDATE", { address: mail }, url).done(
                                            function () {
                                                model.$dialog.dialog("close");
                                            }));
                                });
                        }
                    });
                    return false;
                }
            };
            var hostElement = document.createElement("div");
            hostElement.innerHTML =
'<div class="conVerXS" style="padding: 4px; background: #eee;">' +
    '<div style="margin: 0 0 4px 0; padding: 4px; background: #000; color: #fff;">Másolja ide a képfájl elérési útvonalát:<\/div>' +
    '<input type="url" data-bind="value: url" \/>' +
'<\/div>' +
'<div class="conVerXS">' +
    '<div class="conXS">' +
        '<div role="button" class="fileinput-button">' +
            '<div class="conVerS"><i class="i droparea"><\/i><\/div>' +
            '<div class="conVerS">Válassza ki kattintással vagy húzza be ide a képet<\/div>' +
            '<input id="fileupload" type="file" name="files" data-bind=\'' +
        'jQuery: "fileupload",' +
        'jQueryOptions: {' +
            'dataType: "json",' +
            'dropZone: $($element).parent(),' +
            'submit: onSubmit' +
    '}\' \/><\/div>' +
    '<\/div>' +
'<\/div>';
            this.showMenu(false);
            $.WinJS.dialog({
                width: 500,
                height: "auto",
                position: { my: "center top", at: "center top", of: window },
                title: "Kép szerkesztése",
                content: hostElement,
                open: function () {
                    model.$dialog = $(this);
                    require(["fileupload", "fileupload.css"]).then(function () {
                        ko.applyBindings(model, hostElement);
                    });
                },
                close: function () {
                    ko.cleanNode(hostElement);
                    async(function () { that.showMenu(true); }, this, 4000);
                },
                buttons: [{
                    "class": "ui-btn ui-btn-primary",
                    "text": "Mentés",
                    "click": function () {
                        var $dialog = $(this);
                        var oldImageUrl = moduleOptions.image.url;
                        var newImageUrl = model.url();
                        that.module.updateModuleOptions({ image: { url: newImageUrl } });
                        new MailDialog({
                            onsave: function (mail) {
                                $dialog.dialog("progress",
                                    that.requestAsync("UPDATE", { address: mail }, newImageUrl).done(
                                        function () {
                                            $dialog.dialog("close");
                                        }));
                            },
                            oncancel: function () {
                                that.module.updateModuleOptions({ image: { url: oldImageUrl } });
                            }
                        });
                    }
                }, {
                    "class": "ui-btn",
                    "text": "Bezár",
                    "click": function () {
                        $(this).dialog("close");
                    }
                }]
            });
        },
        showDeleteDialog: function () {
            /// <signature>
            /// <summary>Shows a dialog that prompts the user to give us his email address.</summary>
            /// <param name="options" type="Object" optional="true" />
            /// <returns type="$.WinJS.dialog" />
            /// </signature>
            var that = this;
            this.showMenu(false);
            return $.WinJS.dialog({
                content: liveEditorResources.deleteConfirmText(),
                close: function () {
                    that.showMenu(true);
                },
                buttons: [{
                    "class": "ui-btn ui-btn-primary",
                    "text": "Igen",
                    "click": function () {
                        var $dialog = $(this);
                        new MailDialog({
                            onsave: function (mail) {
                                $dialog.dialog("progress", that.requestAsync("DELETE", { address: mail }).done(
                                    function () {
                                        $dialog.dialog("close");
                                        that.element.remove();
                                    }));
                            }
                        });
                    }
                }, {
                    "class": "ui-btn",
                    "text": "Mégse",
                    "click": function () {
                        $(this).dialog("close");
                    }
                }]
            });
        },
        requestAsync: function (operation, mail, data) {
            /// <signature>
            /// <param name="operation" type="String" />
            /// <param name="mail" type="Object" />
            /// <param name="data" type="String" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            var that = this;
            return require(["input"]).then(function () {
                return PI.Input.CommandService.createAsync({
                    mail: mail,
                    command: {
                        line: PI.Input.CommandService.generateLine(operation || "UPDATE", [{
                            type: "PAGE",
                            id: that.module.options.engine.page.id
                        }, {
                            type: "MODULE",
                            id: that.element.attr("id")
                        }]),
                        data: data
                    },
                    returnUrl: window.location.href
                }).then(
                    function () {
                        $.WinJS.dialog({
                            title: "Módosítás visszaigazolása",
                            content: String.format(
                                "Elküldtünk egy levelet a megadott címre: <strong>{0}</strong>. " +
                                "A levélben talál egy linket amivel véglegesítheti a módosításait. " +
                                "Kérjük ellenőrizze beérkező leveleit és/vagy <strong>spam mappáját</strong>!", mail.address)
                        });
                    },
                    function (error) {
                        $.WinJS.dialog({ content: error.message });
                    });
            });
        },
        dispose: function () {
            /// <signature>
            /// <summary>Disposes the editor.</summary>
            /// </signature>
            this.element.removeClass("ui-module-liveedit");
            this._menu && this._menu.remove();
            this._menu = null;
        }
    }, {
        isEditable: function (element, options) {
            /// <signature>
            /// <summary>Returns true if the given element is enabled.</summary>
            /// </signature>
            return element.attr("id") &&
                ((options.typeClass === "ui-module-html") ||
                (options.typeClass === "ui-module-image"));
        }
    });

    //
    // This widget extension adds edit functions to all the modules at run-time.
    // In contrast with editors available at design-time, these functions are restricted
    // and just adds some marketing value to Partnerinfo TV.
    //

    $.widget("PI.PortalModule", $.PI.PortalModule, {
        _create: function () {
            /// <signature>
            /// <summary>Initializes a new instance of the page module.</summary>
            /// </signature>
            this._super();
            this._setupLiveEdit(this.options.liveEdit);
        },
        _setOption: function (key, value) {
            /// <signature>
            /// <param name="key" type="String" />
            /// <param name="value" type="Object" />
            /// <returns type="Object" />
            /// </signature>
            this._super(key, value);
            if (key === "liveEdit") {
                this._setupLiveEdit(value);
            }
            return this;
        },
        _setupLiveEdit: function (value) {
            /// <signature>
            /// <param name="value" type="Boolean" />
            /// </signature>
            if (!LiveEditor.isEditable(this.element, this.options)) {
                return;
            }
            if (value) {
                this.liveEditor = new LiveEditor(this, { page: this.options.engine.page });
            } else {
                this.liveEditor && this.liveEditor.dispose();
                this.liveEditor = null;
            }
        },
        _destroy: function () {
            /// <signature>
            /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            /// </signature>
            this._setOption("liveEdit", false);
            this._super();
        }
    });

})(WinJS.Resources, PI.Portal);
