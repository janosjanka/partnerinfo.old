// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (_Global, _WinJS, _PI) {
    "use strict";

    var ProjectController = _WinJS.Class.define(function (element, options) {
        /// <signature>
        /// <summary>Creates a new ProjectController control.</summary>
        /// <param name="element" type="HTMLElement" domElement="true">
        /// The DOM element that will host the ProjectController control.
        /// </param>
        /// <param name="options" type="Object" optional="true">
        /// An object that contains one or more property/value pairs to apply to the new control.
        /// Each property of the options object corresponds to one of the control's properties or events.
        /// </param>
        /// <returns type="ProjectController" />
        /// </signature>
        this.element = $(element);
        this.options = options || {};
        this.options.menu = this.options.menu || {};
    }, {
        project: function (context) {
            /// <signature>
            /// <summary>Renders the view that matches the action result name</summary>
            /// </signature>
            this._setWidth(400);
            this._setMenu(context);
            _PI.ui({
                name: "project",
                element: this.element,
                toolbar: this.options.menu.edit,
                params: {
                    project: {
                        id: context.params.project
                    },
                    onsaved: function (event) {
                        _WinJS.Utilities.openLink("/admin/projects/{id}/#/mails", { id: event.detail.data.id });
                    },
                    ondeleted: function () {
                        _Global.history.back();
                    },
                    ondiscarded: function () {
                        _Global.history.back();
                    }
                }
            });
        },
        projects: function (context) {
            /// <signature>
            /// <summary>Renders the view that matches the action result name</summary>
            /// </signature>
            this._setWidth();
            this._setMenu(context);
            _PI.ui({
                name: "projects",
                element: this.element,
                toolbar: this.options.menu.main1
            });
        },
        action: function (context) {
            /// <signature>
            /// <summary>Renders the view that matches the action result name</summary>
            /// </signature>
            this._setWidth(800);
            this._setMenu(context);
            _PI.ui({
                name: "project.action",
                element: this.element,
                toolbar: this.options.menu.edit,
                params: {
                    project: { id: context.params.project },
                    action: { id: context.params.id, type: "redirect" }
                }
            });
        },
        actions: function (context) {
            /// <signature>
            /// <summary>Renders the view that matches the action result name</summary>
            /// </signature>
            this._setWidth();
            this._setMenu(context);
            _PI.ui({
                name: "project.actions",
                element: this.element,
                toolbar: this.options.menu.main1,
                params: {
                    filter: { project: { id: context.params.project } }
                }
            });
        },
        contacts: function (context) {
            /// <signature>
            /// <summary>Renders the view that matches the action result name</summary>
            /// </signature>
            this._setWidth();
            this._setMenu(context, { main1: false });
            _PI.ui({
                name: "project.contacts",
                element: this.element,
                menubar: this.options.menu.main1,
                params: {
                    filter: { project: { id: context.params.project } }
                }
            });
        },
        contact: function (context) {
            /// <signature>
            /// <summary>Renders the view that matches the action result name</summary>
            /// </signature>
            this._setWidth();
            this._setMenu(context);
            _PI.ui({
                name: "project.contact",
                element: this.element.css("width", 400),
                menu: this.options.menu.edit,
                params: {
                    project: { id: context.params.project },
                    contact: { id: context.params.id },
                    onsaved: function () {
                        _Global.location.hash = "#/contacts";
                    },
                    ondiscarded: function () {
                        _Global.location.hash = "#/contacts";
                    }
                }
            });
        },
        mail: function (context) {
            /// <signature>
            /// <summary>Renders the view that matches the action result name</summary>
            /// </signature>
            this._setWidth();
            this._setMenu(context);
            function complete() {
                _Global.location.hash = "#/mails";
            }
            _PI.ui({
                name: "project.mail",
                element: this.element,
                menu: this.options.menu.edit,
                params: {
                    mailMessage: { id: context.params.id },
                    project: { id: context.params.project },
                    // onsaved: complete,
                    onsent: complete,
                    ondeleted: complete,
                    ondiscarded: complete
                }
            });
        },
        mails: function (context) {
            /// <signature>
            /// <summary>Renders the view that matches the action result name</summary>
            /// </signature>
            this._setWidth();
            this._setMenu(context);
            _PI.ui({
                name: "project.mails",
                element: this.element,
                menu: this.options.menu.main1,
                params: {
                    filter: { project: { id: context.params.project } }
                }
            });
        },
        portals: function (context) {
            /// <signature>
            /// <summary>Renders the view that matches the action result name</summary>
            /// </signature>
            this._setWidth();
            this._setMenu(context);
            var that = this;
            _PI.ui({
                name: "portals",
                element: that.element,
                filter: { project: { id: context.params.project } }
            });
        },
        events: function (context) {
            /// <signature>
            /// <summary>Renders the view that matches the action result name</summary>
            /// </signature>
            this._setWidth();
            this._setMenu(context);
            var that = this;
            _PI.ui({
                name: "logging.events",
                element: this.element,
                menu: this.options.menu.main1,
                filter: { project: this.options.project },
                categories: {
                    element: this.options.menu.main2,
                    filter: { project: this.options.project }
                }
            });
        },
        _setWidth: function (width) {
            /// <signature>
            /// <summary>Sets the width of the element.</summary>
            /// <param name="width" type="String" optional="true" />
            /// </signature>
            this.element.css("width", width || "auto");
        },
        _setMenu: function (context, options) {
            /// <signature>
            /// <summary>Renders a menu based on the current context.</summary>
            /// <param name="context" type="Object" />
            /// </signature>
            var menu = this.options.menu;
            $(menu.edit).empty();
            $(menu.main1).empty();
            $(menu.main2).empty();
            $(menu.icon).empty();
            $(menu.config).empty();
            if (context.params && context.params.project) {
                options = options || {};
                if (options.main1 !== false) {
                    PI.bind(menu.main1, null, "koProjectMainMenu");
                }
                if (options.icon !== false) {
                    PI.bind(menu.icon, null, "koProjectIconMenu");
                }
                if (options.config !== false) {
                    PI.bind(menu.config, null, "koProjectConfigMenu");
                }
            }
        }
    });

    ko.components.register("project-view", {
        viewModel: {
            createViewModel: function (params, info) {
                /// <signature>
                /// <summary>Creates a viewModel for presenting data errors.</summary>
                /// </signature>
                params = params || {};
                var element = info.element.childNodes[0];
                return Sammy("#" + (element.id || (element.id = "project_view")), function () {
                    var routes = PI.Routes.Project;
                    var controller = new ProjectController(element, params);

                    this._checkFormSubmission = function (form) {
                        return false;
                    };
                    this.around(function (callback) {
                        $.ui.dropdown.closeAll();
                        callback();
                    });

                    this.get(routes.Actions.create(":project"), controller.action.bind(controller));
                    this.get(routes.Actions.item(":project", ":id"), controller.action.bind(controller));
                    this.get(routes.Actions.list(":project"), controller.actions.bind(controller));
                    this.get(routes.Contacts.create(":project"), controller.contact.bind(controller));
                    this.get(routes.Contacts.item(":project", ":id"), controller.contact.bind(controller));
                    this.get(routes.Contacts.list(":project"), controller.contacts.bind(controller));
                    this.get(routes.MailMessages.create(":project"), controller.mail.bind(controller));
                    this.get(routes.MailMessages.item(":project", ":id"), controller.mail.bind(controller));
                    this.get(routes.MailMessages.list(":project"), controller.mails.bind(controller));
                    this.get("/admin/projects/:project/#/portals", controller.portals.bind(controller));
                    this.get("/admin/projects/:project/#/events", controller.events.bind(controller));
                    this.get("/admin/projects/:project/#/config", controller.project.bind(controller));
                    this.get("/admin/projects/#/create", controller.project.bind(controller));
                    this.get("/admin/projects", controller.projects.bind(controller));
                })
                .run("/admin/projects");
            }
        },
        template: '<div class="ui-canvas-app"></div>'
    });

    /*
        function ProjectRouter(element, options) {
            /// <signature>
            /// <summary>Creates a new instance of the rooter.</summary>
            /// <param name="element" type="HTMLElement" />
            /// <param name="options" type="Object" />
            /// </signature>
            this.router = Sammy("#" + (element.id || (element.id = "project_view")), function () {
                var listPath = "/admin/projects";
                var itemPath = "/admin/projects/:project";
    
                this.get(itemPath + "/#/actions", function _actionsView(context) {
                    /// <signature>
                    /// <summary>Renders a view of a collection of actions.</summary>
                    /// </signature>
                    _PI.ui({
                        name: "project.actions",
                        element: element,
                        toolbar: options.menu.main1,
                        params: {
                            filter: { project: { id: context.params.project } }
                        }
                    });
                });
    
                this.get(itemPath + "/#/contacts", function () {
    
                });
    
                this.get(listPath, function () {
    
                });
    
            });
            this.router.run("/admin/projects");
        }
    
        ko.components.register("project-view", {
            viewModel: {
                createViewModel: function (params, info) {
                    /// <signature>
                    /// <summary>Creates a viewModel for presenting data errors.</summary>
                    /// </signature>
                    return new ProjectRouter(info.element.childNodes[0], params);
                }
            },
            template: '<div class="ui-canvas-app"></div>'
        });
    */

})(window, WinJS, PI);