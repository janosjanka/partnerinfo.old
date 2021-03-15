// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="../services/project.js" />

(function (_Global, _KO, _WinJS, _PI) {
    "use strict";

    var _Class = _WinJS.Class;
    var _Utilities = _WinJS.Utilities;
    var _Promise = _WinJS.Promise;
    var _Knockout = _WinJS.Knockout;

    var _observable = _KO.observable;
    var _observableArray = _KO.observableArray;
    var _pureComputed = _KO.pureComputed;

    var ns = _WinJS.Namespace.defineWithParent(_PI, "Project", {

        ProjectResources: {
            createLink: function () { return "/admin/projects/#/create"; },
            actionLink: function (projectId, controller) { return _Utilities.actionLink("/admin/projects/{project}/#/{controller}", { project: projectId, controller: controller }); }
        },

        ProjectSortOrder: {
            /// <field>
            /// Items are returned in chronological order.
            /// </field>
            none: "none",

            /// <field>
            /// Items are returned in reverse chronological order.
            /// </field>
            recent: "recent",

            /// <field>
            /// Items are ordered alphabetically by name.
            /// </field>
            name: "name"
        },

        ProjectField: {
            /// <field>
            /// No extra fields included in the result set.
            /// </field>
            none: 0,

            /// <field>
            /// Owners are included in the result set. 
            /// </field>
            owners: 1 << 0,

            /// <field>
            /// Statistics are included in the result set.
            /// </field>
            statistics: 1 << 1
        },

        ProjectMailAddress: _Class.define(function ProjectSender_ctor(mailAddress) {
            /// <signature>
            /// <summary>Initializes a new instance of the Project class.</summary>
            /// <param name="mailAddress" type="Object" optional="true" />
            /// <returns type="PI.Project.Project" />
            /// </signature>
            mailAddress = mailAddress || {};
            this.address = _observable(mailAddress.address);
            this.name = _observable(mailAddress.name);
        }, {
            update: function (mailAddress) {
                /// <signature>
                /// <param name="project" type="Object" />
                /// </signature>
                mailAddress = mailAddress || {};
                this.address(mailAddress.address);
                this.name(mailAddress.name);
            },
            toObject: function () {
                /// <signature>
                /// <returns type="Object" />
                /// </signature>
                return {
                    address: this.address(),
                    name: this.name()
                };
            }
        }),

        ProjectItem: _Class.define(function ProjectItem_ctor(project, options) {
            /// <signature>
            /// <summary>Initializes a new instance of the ProjectItem class.</summary>
            /// <param name="project" type="Object" optional="true" />
            /// <param name="options" type="Object" optional="true" />
            /// <returns type="PI.Project.ProjectItem" />
            /// </signature>
            _Utilities.setOptions(this, options = options || {});
            _Promise.tasks(this);

            this.service = options.service || ns.ProjectService;

            this.id = _observable();
            this.name = _observable().extend({ required: { message: "A projekt nevére szükség van." } });
            this.sender = new ns.ProjectMailAddress();

            this.errors = _KO.validation.group(this);
            this.editing = _observable(false);
            this.exists = _pureComputed(this._exists, this);

            this.update(project);
        }, {
            validate: function () {
                /// <signature>
                /// <summary>Returns true if this project is valid.</summary>
                /// <returns type="Boolean" />
                /// </signature>
                return this.errors().length === 0;
            },

            //
            // Edit Session
            //

            beginEdit: function () {
                /// <signature>
                /// <summary>Begins an edit on an object.</summary>
                /// </signature>
                if (this._editSession) {
                    return;
                }
                this._editSession = new _KO.editSession(this, { fields: ["name", "sender"] });
                this.editing(true);
            },
            cancelEdit: function () {
                /// <signature>
                /// <summary>Discards changes since the last BeginEdit call.</summary>
                /// </signature>
                if (!this._editSession) {
                    return;
                }
                this._editSession.cancel();
                this._editSession = null;
                this.editing(false);
            },
            endEdit: function () {
                /// <signature>
                /// <summary>Pushes changes since the last beginEdit.</summary>
                /// </signature>
                if (!this.validate()) {
                    return;
                }
                this._editSession = null;
                this.editing(false);
            },

            //
            // Data Operations
            //

            update: function (project) {
                /// <signature>
                /// <param name="project" type="Object" />
                /// </signature>
                project = project || {};
                this.id(project.id);
                this.name(project.name);
                this.sender.update(project.sender);
            },
            toObject: function () {
                /// <signature>
                /// <returns type="Object" />
                /// </signature>
                return {
                    id: this.id(),
                    name: this.name(),
                    sender: this.sender.toObject()
                };
            },

            //
            // Storage Operations
            //

            loadAsync: _Promise.tasks.watch(function () {
                /// <signature>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                if (this.exists()) {
                    return this.service.getByIdAsync(this.id(), this)
                        .then(function (project) {
                            this.update(project);
                            this.dispatchEvent("loaded");
                        });
                }
                return _Promise.error();
            }),
            saveAsync: _Promise.tasks.watch(function () {
                /// <signature>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                if (!this.validate()) {
                    return _Promise.error();
                }
                var project = this.toObject();
                if (this.exists()) {
                    return this.service.updateAsync(project.id, project, this)
                        .then(function (project) {
                            this.update(project);
                            this.dispatchEvent("saved", { state: "modified", data: project });
                        });
                }
                return this.service.createAsync(project, this)
                    .then(function (project) {
                        this.update(project);
                        this.dispatchEvent("saved", { state: "added", data: project });
                    });
            }),
            deleteAsync: _Promise.tasks.watch(function () {
                /// <signature>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                if (!this.exists()) {
                    return _Promise.complete();
                }
                return this.service.deleteAsync(this.id(), this)
                    .then(function () {
                        this.dispatchEvent("deleted");
                    });
            }),
            discard: function () {
                /// <signature>
                /// <summary>Discards data changes and raises an ondiscarded event that can be handled by views.</summary>
                /// </signature>
                this.dispatchEvent("discarded");
            },

            //
            // Event & Computed Members
            //

            _exists: function () {
                /// <signature>
                /// <returns type="Boolean" />
                /// </signature>
                return !!this.id();
            },
            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                this.errors && this.errors.dispose();
                this.errors = null;
                this.exists && this.exists.dispose();
                this.exists = null;
            }
        }),

        ProjectFilter: _Class.define(function ProjectFilter_ctor(list, options) {
            /// <signature>
            /// <summary>Initializes a new instance of the ProjectFilter class.</summary>
            /// <param name="list" type="PI.Project.ProjectList" />
            /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the ProjectFilter.</param>
            /// <returns type="PI.Project.ProjectFilter" />
            /// </signature>
            options = options || {};
            this._disposed = false;
            this._list = list;

            this.name = _observable(options.name);
            this.orderBy = _observable(options.orderBy || ns.ProjectSortOrder.none);
            this.fields = _observable(options.fields || ns.ProjectField.none);

            this._session = _KO.editSession(this, { fields: ["name"] });
            this._orderBySn = this.orderBy.subscribe(this.submit, this);
        }, {
            submit: function () {
                /// <signature>
                /// <summary>Commits the edit session.</summary>
                /// </signature>
                this._list.refresh();
            },
            cancel: function () {
                /// <signature>
                /// <summary>Cancels the edit session.</summary>
                /// </signature>
                this._session.cancel();
                this._list.refresh();
            },
            toObject: function () {
                /// <signature>
                /// <summary>Returns a pure JS object.</summary>
                /// <returns type="Object" />
                /// </signature>
                return {
                    name: this.name(),
                    orderBy: this.orderBy(),
                    fields: this.fields()
                };
            },
            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                if (this._disposed) {
                    return;
                }
                this._orderBySn && this._orderBySn.dispose();
                this._orderBySn = null;
                this._session && this._session.dispose();
                this._session = null;
                this._disposed = true;
            }
        }),

        ProjectListItem: _Class.define(function ProjectListItem_ctor(project) {
            /// <signature>
            /// <summary>Initializes a new instance of the ProjectListItem class.</summary>
            /// <param name="options" type="project" />
            /// <returns type="PI.Project.ProjectListItem" />
            /// </signature>
            this.id = project.id;
            this.name = project.name;
            this.sender = project.sender || {};
            this.createdDate = project.createdDate;
            this.modifiedDate = project.modifiedDate;
            this.contactCount = project.contactCount | 0;
            this.owners = project.owners || [];
        }),

        ProjectListCommands: _Class.define(function ProjectListCommands_ctor(list) {
            /// <signature>
            /// <summary>Initializes a new instance of the ProjectListCommands class.</summary>
            /// <param name="list" type="Object" optional="true" />
            /// <returns type="PI.Project.ProjectListCommands" />
            /// </signature>
            this._list = list;
        }, {
            "list.refresh": function () {
                /// <signature>
                /// <summary>Refreshes the list.</summary>
                /// </signature>
                this._list.refresh();
            },
            "list.create": function () {
                /// <signature>
                /// <summary>Creates a new project.</summary>
                /// </signature>
                _Global.open(ns.ProjectResources.createLink(), "_self");
            },
            "selection.action": function (controller) {
                /// <signature>
                /// <param name="controller" type="String" />
                /// </signature>
                var item = this._list.selection()[0];
                item && _Global.open(ns.ProjectResources.actionLink(item.id, controller), "_self");
            },
            "selection.open": function () {
                /// <signature>
                /// <summary>Opens the project.</summary>
                /// </signature>
                var item = this._list.selection()[0];
                item && _Global.open(ns.ProjectResources.actionLink(item.id, "actions"), "_self");
            },
            "selection.edit": function () {
                /// <signature>
                /// <summary>Opens the project.</summary>
                /// </signature>
                var item = this._list.selection()[0];
                item && _Global.open(ns.ProjectResources.actionLink(item.id, "edit"), "_self");
            },
            "selection.delete": function () {
                /// <signature>
                /// <summary>Deletes the selected item.</summary>
                /// </signature>
                var item = this._list.selection()[0];
                item && this._list.service.deleteAsync(item.id, this)
                    .then(function () {
                        this._list.remove(item);
                    });
            },
            "selection.share": function () {
                /// <signature>
                /// <summary>Shares the selected item with other users.</summary>
                /// </signature>
                var item = this._list.selection()[0];
                item && _PI.dialog({
                    name: "security.acl",
                    params: {
                        objectType: "project",
                        objectId: item.id
                    }
                });
            }
        }),

        ProjectList: _Class.derive(_Knockout.PagedList, function ProjectList_ctor(options) {
            /// <signature>
            /// <summary>Initializes a new instance of the ProjectList class.</summary>
            /// <param name="options" type="Object" />
            /// <returns type="PI.Project.ProjectList" />
            /// </signature>
            options = options || {};
            options.autoLoad = options.autoLoad !== false;
            options.pageIndex = options.pageIndex || 1;
            options.pageSize = options.pageSize || 50;
            _Knockout.PagedList.call(this, options);

            this._disposed = false;
            this.service = options.service || ns.ProjectService;
            this.commands = new ns.ProjectListCommands(this);
            this.filter = new ns.ProjectFilter(this, options.filter);
            options.autoLoad && this.refresh();
        }, {
            mapItem: function (item) {
                /// <signature>
                /// <param name="item" type="Object" />
                /// <returns type="PI.Project.ProjectListItem" />
                /// </signature>
                return new ns.ProjectListItem(item);
            },
            refresh: function () {
                /// <signature>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                var params = this._createParams();
                return this.service.getAllAsync(params, this).then(
                    function (response) {
                        this.replaceAll.apply(this, response.data);
                        this.total = response.total;
                    },
                    function () {
                        this.removeAll();
                        this.total = 0;
                    });
            },
            _createParams: function () {
                /// <signature>
                /// <returns type="Object" />
                /// </signature>
                var params = this.filter.toObject() || {};
                params.page = this.pageIndex;
                params.limit = this.pageSize;
                return params;
            },
            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                if (this._disposed) {
                    return;
                }
                this.filter && this.filter.dispose();
                this.filter = null;
                _Knockout.PagedList.prototype.dispose.call(this);
                this._disposed = true;
            }
        })

    });

    _Class.mix(ns.ProjectItem, _Utilities.createEventProperties("loaded", "saved", "deleted", "discarded"));
    _Class.mix(ns.ProjectItem, _Utilities.eventMixin);

})(window, ko, WinJS, PI);
