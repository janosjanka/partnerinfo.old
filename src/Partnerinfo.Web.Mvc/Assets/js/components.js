// Copyright (c) Partnerinfo TV. All Rights Reserved.

(function ($, _WinJS, _PI) {
    "use strict";

    var _Security = _PI.Security;

    //
    // Renders an action link picker control.
    //

    ko.components.register("notification-menu", {
        viewModel: function (params) {
            params = params || {};
            this.events = params.events !== false;
            this.projects = params.projects !== false;
            this.portals = params.portals !== false;
        },
        template:
            '<div role="toolbar" class="ui-btn-group">' +
                '<div data-bind=\'visible: events, dropdown: {' +
                    'align: "right",' +
                    'autoClose: false,' +
                    'render: function (event, complete) {' +
                        'require(["logging", "logging.css", "logging/events.html"]).then(function () {' +
                            'var unread = PI.Logging.EventManager.unread();' +
                            'unread && PI.Logging.EventManager.markAsRead();' +
                            'if (!PI.Logging.EventList.instance) {' +
                                'PI.bind("#nEventMenuList", PI.Logging.EventList.instance = new PI.Logging.EventList({ autoOpen: true }), "koEventMenuList");' +
                            '} else if (unread) {' +
                                'async(PI.Logging.EventList.instance.refresh.bind(PI.Logging.EventList.instance, true));' +
                            '}' +
                            'complete();' +
                        '});' +
                    '}' +
                '}\'>' +
                        '<button class="ui-btn ui-btn-flat" type="button" title="Események">' +
                            '<i class="i event"></i>' +
                            '<sup style="display: none;" data-bind=\'visible: PI.Logging.EventManager.unread, text: PI.Logging.EventManager.unread\'></sup>' +
                        '</button>' +
                        '<ul id="nEventMenuList" class="ui-menu" style="width: 600px"></ul>' +
                    '</div>' +
                    '<div data-bind=\'visible: projects, dropdown: {' +
                        'align: "right",' +
                        'autoClose: false,' +
                        'render: function (event, complete) {' +
                            'require(["project", "project/projects-menu.html"]).then(function () {' +
                                '!PI.Project.ProjectList.instance && PI.bind("#nProjectMenuList", PI.Project.ProjectList.instance = new PI.Project.ProjectList({ filter: { orderBy: PI.Project.ProjectSortOrder.recent, fields: PI.Project.ProjectField.statistics } }), "koProjectMenuList");' +
                                'complete();' +
                            '});' +
                        '}' +
                    '}\'>' +
                        '<button class="ui-btn ui-btn-flat" type="button" title="Projektek">' +
                            '<i class="i project"></i>' +
                        '</button>' +
                        '<ul id="nProjectMenuList" class="ui-menu" style="width: 600px"></ul>' +
                    '</div>' +
                    '<div data-bind=\'visible: portals, dropdown: { align: "right", autoClose: false, render: function () { PI.ui({ element: "#pi-main-portal-list", name: "portals", displayMode: "menu" }); } }\'>' +
                        '<button class="ui-btn ui-btn-flat" type="button">' +
                            '<i class="i page"></i>' +
                        '</button>' +
                        '<ul id="pi-main-portal-list" class="ui-menu" style="width: 600px"></ul>' +
                    '</div>' +
                '</div>'
    });

    ko.components.register("resource-owners", {
        template:
            "<div data-bind='dropdown'>" +
                "<button type=\"button\" class=\"ui-btn ui-btn-flat ui-btn-sm\">" +
                    "<span data-bind='text: $data.item.owners.length'><\/span>" +
                    "<i class=\"i dd\"><\/i>" +
                "<\/button>" +
                "<ul role=\"menu\" class=\"ui-menu\" data-bind='foreach: $data.item.owners'>" +
                    "<li role=\"menuitem\">" +
                        "<a data-bind='click: $parent.share.bind($parent)'>" +
                            "<i class=\"i contact\"><\/i>" +
                            "<span data-bind='text: email.name, attr: { title: email.address }'><\/span>" +
                        "<\/a>" +
                    "<\/li>" +
                "<\/ul>" +
            "<\/div>"
    });

    ko.components.register("project-action-picker", {
        viewModel: _WinJS.Class.define(function ProjectActionPicker_ctor(options) {
            /// <signature>
            /// <summary>Initializes a new instance of the ProjectActionPicker class.</summary>
            /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the ProjectActionPicker.</param>
            /// <returns type="ProjectActionPicker" />
            /// </signature>
            this.action = options.action;
            this.filter = options.filter;
            this.width = options.width || "550px";
        }, {
            reset: function () {
                if (ko.isWritableObservable(this.action)) {
                    this.action(null);
                    return;
                }
                this.action = null;
            }
        }),
        template:
            '<div data-bind=\'visible: !action(), dropdown: { align: "right", autoClose: false, render: function () { PI.ui({ element: $("li:first", $element), name: "project.action-picker", filter: filter, oncurrentchanged: function (e) { $data.action(e.detail.newItem); } }); } }\'>' +
                '<button class="ui-btn ui-btn-flat" type="button">' +
                    '<i class="i action"></i>' +
                    '<i class="i dd"></i>' +
                '</button>' +
                '<ul role="menu" class="ui-menu" data-bind=\'style: { width: width, height: "300px" }\'>' +
                    '<li role="menuitem"></li>' +
                '</ul>' +
            '</div>' +
            '<button class="ui-btn ui-btn-flat" data-bind=\'visible: action, click: reset, clickBubble: false\'><i class="i close"></i></button>'
    });

    ko.components.register("project-actionlink-button", {
        viewModel: _WinJS.Class.define(function ProjectActionLinkButton_ctor(options) {
            /// <signature>
            /// <summary>Initializes a new instance of the ProjectActionLinkButton class.</summary>
            /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the ProjectActionLinkButton.</param>
            /// <returns type="ProjectActionLinkButton" />
            /// </signature>
            options = options || {};
            this.actionlink = options.actionlink || {};
            this.width = options.width || "550px";
        }),
        template:
            '<div data-bind=\'dropdown: { align: "right", autoClose: false, render: function () { PI.ui({ element: $(".ui-menu .ui-box-group", $element), name: "project.actionlink", actionlink: actionlink }); } }\'>' +
                '<button class="ui-btn ui-btn-flat" type="button">' +
                    '<i class="i action"></i>' +
                    '<i class="i dd"></i>' +
                '</button>' +
                '<ul role="menu" class="ui-menu" data-bind=\'style: { width: width }\'>' +
                    '<li role="menuitem">' +
                        '<div class="ui-box"><div class="ui-box-group"></div></div>' +
                    '</li>' +
                '</ul>' +
            '</div>'
    });

    //
    // Renders a mail message picker control.
    //

    ko.components.register("project-mail-picker", {
        viewModel: _WinJS.Class.define(function MailPicker_ctor(options) {
            /// <signature>
            /// <summary>Initializes a new instance of the MailPicker class.</summary>
            /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the MailPicker.</param>
            /// <returns type="MailPicker" />
            /// </signature>
            options = options || {};
            this.service = options.service;
            this.project = options.project;
            this.autoLoad = options.autoLoad !== false;
            this.width = options.width || "550px";
            this.mailMessages = undefined;
            this.mailMessage = options.mailMessage;
            this.autoLoad && this.loadItemAsync();
        }, {
            loadItemAsync: function () {
                /// <signature>
                /// <summary>Asynchronously loads the mail message.</summary>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                var mail = this.mailMessage();
                if (!mail) {
                    return _WinJS.Promise.error();
                }
                var that = this;
                return require(["project"]).then(function () {
                    var service = that.service || _PI.Project.MailMessageService;
                    service.getByIdAsync(mail.id).then(function (mailMessage) {
                        that.mailMessage(mailMessage);
                    });
                });
            },
            loadListAsync: function () {
                /// <signature>
                /// <summary>Asynchronously loads the mail list.</summary>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                var that = this;
                return require(["project"]).then(function () {
                    that.mailMessages = new _PI.Project.MailMessageList({
                        autoOpen: true,
                        pageSize: 10,
                        filter: {
                            project: that.project
                        },
                        oncurrentchanged: function (event) {
                            that.mailMessage(event.detail.newItem);
                        }
                    });
                });
            },
            onRender: function (event, complete) {
                /// <signature>
                /// <summary>Raised when the dropdown menu is rendered.</summary>
                /// <param name="event" type="$.Event" />
                /// <param name="complete" type="Function" />
                /// </signature>
                var that = this;
                _WinJS.Promise.join([this.loadListAsync(), require(["project/mail-picker.html"])])
                    .then(function () {
                        ko.renderTemplate("koMailMessagePicker", that, null, event.target.querySelector(".ui-menu"));
                        complete();
                    });
            },
            reset: function () {
                /// <signature>
                /// <summary>Indicates that the mail message is not specified.</summary>
                /// </signature>
                this.mailMessages && this.mailMessages.moveTo(undefined);
                this.mailMessage(undefined);
            }
        }),
        template:
            '<div data-bind="visible: !mailMessage(), dropdown: { autoClose: false, align: \'right\', render: function (e, c) { $data.onRender(e, c); } }">' +
                '<button class="ui-btn ui-btn-flat" type="button">' +
                    '<i class="i mail"></i>' +
                    '<i class="i dd"></i>' +
                '</button>' +
                '<div role="menu" class="ui-menu" data-bind="style: { width: width }"></div>' +
            '</div>' +
            '<button class="ui-btn ui-btn-flat" data-bind="visible: mailMessage, click: reset, clickBubble: false">' +
                '<i class="i close"></i>' +
            '</button>'
    });

    //
    // Project Contact Input
    // Picks contacts from a project using a Facebook-like input control
    //

    ko.components.register("project-contact-input", {
        viewModel: _WinJS.Class.define(function (params) {
            /// <signature>
            /// <param name="params" type="Object">
            ///     <para>name: String - Input name</para>
            ///     <para>contacts: ko.observableArray - Contains selected contacts</para>
            /// </param>
            /// <returns type="ProjectContactInputViewModel" />
            /// </signature>
            params = params || {};
            this.menu = params.menu || {};
            this.projectId = ko.unwrap(params.projectId);
            this.count = ko.unwrap(params.count) || 16;
            this.token = params.contacts;
            this.tokenUrl = PI.api.action("projects/{projectId}/contacts", { projectId: this.projectId, count: this.count });
            this.tokenParams = { queryParam: "name", enableHTML: true, tokenLimit: params.limit || 100 };
        }, {
            tokenResultsMapper: function (contact) {
                /// <signature>
                /// <param name="contact" type="Object" />
                /// <returns type="Object" />
                /// </signature>
                var email = contact.email.address || "";
                var html = '<span title="';
                html += email;
                html += '">';
                html += contact.email.name || email;
                html += '</span>';
                html += '<span class="ui-type-mail" style="position: absolute; left: 200px;">';
                html += email || (contact.phones = contact.phones || {}) && (contact.phones.personal || contact.phones.business || contact.phones.mobile || contact.phones.other);
                html += '</span>';
                return {
                    id: contact.id,
                    name: html
                };
            },
            showContactDialog: function () {
                /// <signature>
                /// <summary>Displays the contact dialog</summary>
                /// </signature>
                var that = this;
                PI.dialog({
                    name: "project.contact",
                    params: {
                        project: { id: this.projectId }
                    },
                    done: function (response) {
                        if (response.result === "ok") {
                            that.token.push(response.contact);
                        }
                    }
                });
            }
        }),
        template:
            '<div class="ui-form-group">' +
                '<input class="ui-form-control" type="text" data-bind=\'token: token, tokenUrl: tokenUrl, tokenParams: tokenParams, tokenResultsMapper: tokenResultsMapper\' />' +
            '</div>' +
            '<div role="toolbar" class="ui-toolbar">' +
                '<div role="group" class="ui-btn-group">' +
                    '<button class="ui-btn ui-btn-flat" type="button" data-bind=\'click: showContactDialog, clickBubble: false\'><span>Új felhasználó</span></button>' +
                    '<button class="ui-btn ui-btn-flat" type="button" data-bind=\'enable: token().length, click: token.bind(token, []), clickBubble: false\'><span>Nincs kiválasztott</span></button>' +
                '</div>' +
                '<div role="group" class="ui-btn-group" data-bind="template: { if: menu.name, name: menu.name, data: menu.data || $data }"></div>' +
            '</div>'
    });

})(jQuery, WinJS, PI);
