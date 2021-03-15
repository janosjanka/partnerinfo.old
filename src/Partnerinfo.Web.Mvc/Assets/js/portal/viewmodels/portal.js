// Copyright (c) Partnerinfo TV. All Rights Reserved.

/// <reference path="../services/portal.js" />

(function (_Global, _KO, _WinJS, _PI) {
    "use strict";

    var _Class = _WinJS.Class;
    var _Utilities = _WinJS.Utilities;
    var _Promise = _WinJS.Promise;
    var _Resources = _WinJS.Resources;
    var _Knockout = _WinJS.Knockout;

    var _observable = _KO.observable;
    var _observableArray = _KO.observableArray;
    var _pureComputed = _KO.pureComputed;

    var ns = _WinJS.Namespace.defineWithParent(_PI, "Portal", {

        PortalResources: {
            createLink: function () { return "/admin/portals/#/create"; },
            editLink: function (portalUri, controller) { return _Utilities.actionLink("/admin/portals/{portalUri}/#/{controller}", { portalUri: portalUri, controller: controller }); }
        },

        PortalSortOrder: {
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

        PortalField: {
            /// <field>
            /// No extra fields included in the result set.
            /// </field>
            none: "none",

            /// <field>
            /// The project is incldued in the result set.
            /// </field>
            project: "project",

            /// <field>
            /// The home page is incldued in the result set.
            /// </field>
            homePage: "homepage",

            /// <field>
            /// The master page is incldued in the result set.
            /// </field>
            masterPage: "masterpage",

            /// <field>
            /// Owners are included in the result set. 
            /// </field>
            owners: "owners"
        },

        PortalItem: _Class.define(function PortalItem_ctor(portal, options) {
            /// <signature>
            /// <summary>Initializes a new instance of the PortalItem class.</summary>
            /// <param name="portal" type="Object" optional="true" />
            /// <param name="options" type="Object" optional="true" />
            /// <returns type="PI.Portal.PortalItem" />
            /// </signature>
            _Utilities.setOptions(this, options = options || {});
            _Promise.tasks(this);

            this.service = options.service || ns.PortalService;

            this.originalUri = null;
            this.uri = _observable().extend({ required: { message: "A portál címére szükség van." } });
            this.domain = _observable();
            this.name = _observable().extend({ required: { message: "A portál nevére szükség van." } });
            this.description = _observable();
            this.gaTrackingId = _observable();
            this.project = _observable();

            this.template = _observable("default");
            this.templates = [null, "default"];

            this.errors = _KO.validation.group(this);
            this.editing = _observable(false);

            this.update(portal);
            this.autoGenerateUri = options.autoGenerateUri !== false;
        }, {
            /// <field type="Boolean">
            /// If true, automatically generates a URI for this portal.
            /// </field>
            autoGenerateUri: {
                get: function () {
                    return !!this._nameSn;
                },
                set: function (value) {
                    if (value) {
                        this._nameSn = this._nameSn || this.name.subscribe(this.generateUri, this);
                    } else {
                        this._nameSn && this._nameSn.dispose();
                        this._nameSn = null;
                    }
                }
            },

            validate: function () {
                /// <signature>
                /// <summary>Returns true if this portal is valid.</summary>
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
                this._editSession = new _KO.editSession(this, { fields: ["uri", "name", "description", "gaTrackingId", "project"] });
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

            update: function (portal) {
                /// <signature>
                /// <param name="portal" type="Object" />
                /// </signature>
                portal = portal || {};

                this.project(portal.project || this.project());

                this.originalUri = portal.uri;
                this.uri(portal.uri);
                this.domain(portal.domain);
                this.name(portal.name);
                this.description(portal.description);
                this.gaTrackingId(portal.gaTrackingId);
            },
            toObject: function () {
                /// <signature>
                /// <returns type="Object" />
                /// </signature>
                return {
                    uri: this.uri(),
                    domain: this.domain(),
                    name: this.name(),
                    description: this.description(),
                    gaTrackingId: this.gaTrackingId(),
                    project: this.project()
                };
            },
            generateUri: function () {
                /// <signature>
                /// <summary>Generates a new URI.</summary>
                /// </signature>
                var name = this.name();
                this.uri(name ? name.uri() : null);
            },

            //
            // Storage Operations
            //

            loadAsync: _Promise.tasks.watch(function () {
                /// <signature>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                if (!this.originalUri) {
                    return _Promise.error();
                }
                return this.service.getByUriAsync(this.originalUri, "project", this)
                    .then(function (portal) {
                        this.autoGenerateUri = false;
                        this.update(portal);
                        this.dispatchEvent("loaded");
                    });
            }),
            saveAsync: _Promise.tasks.watch(function () {
                /// <signature>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                if (!this.validate()) {
                    return _Promise.error();
                }
                var portal = this.toObject();
                if (this.originalUri) {
                    return this.service.updateAsync(this.originalUri, portal, this)
                        .then(function (portal) {
                            this.autoGenerateUri = false;
                            this.update(portal);
                            this.dispatchEvent("saved", { state: "modified", data: portal });
                        });
                }
                portal.template = this.template();
                return this.service.createAsync(portal, this)
                    .then(function (portal) {
                        this.autoGenerateUri = false;
                        this.update(portal);
                        this.dispatchEvent("saved", { state: "added", data: portal });
                    });
            }),
            deleteAsync: _Promise.tasks.watch(function () {
                /// <signature>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return this.service.deleteAsync(this.originalUri, this)
                    .then(function () {
                        this.dispatchEvent("deleted");
                    });
            }),
            setProjectAsync: _Promise.tasks.watch(function (project) {
                /// <signature>
                /// <param name="project" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                if (!this.originalUri) {
                    this.project(project);
                    return _Promise.complete();
                }
                return this.service.setProjectAsync(this.originalUri, project && project.id, this).then(
                    function () {
                        this.project(project);
                    },
                    function () {
                        this.project(null);
                    });
            }),
            setDomainAsync: _Promise.tasks.watch(function (domain) {
                /// <signature>
                /// <param name="domain" type="String" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                if (!this.originalUri) {
                    this.domain(domain);
                    return _Promise.complete();
                }
                return this.service.setDomainAsync(this.originalUri, domain, this).then(
                    function () {
                        this.domain(domain);
                    },
                    function () {
                        this.domain(null);
                    });
            }),
            discard: function () {
                /// <signature>
                /// <summary>Discards data changes and raises an ondiscarded event that can be handled by views.</summary>
                /// </signature>
                this.dispatchEvent("discarded");
            },

            //
            // Related Actions
            //

            projectDialog: function () {
                /// <signature>
                /// <summary>Changes the project of the current portal.</summary>
                /// </signature>
                var that = this;
                _PI.dialog({
                    name: "project-picker",
                    mode: "single",
                    done: function (event) {
                        if (event.result === "ok") {
                            that.setProjectAsync(event.items[0]);
                        }
                    }
                });
            },

            //
            // Event & Computed Members
            //

            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                this.autoGenerateUri = false;
                this.errors && this.errors.dispose();
                this.errors = null;
            }
        }),

        PortalFilter: _Class.define(function PortalFilter_ctor(list, options) {
            /// <signature>
            /// <summary>Initializes a new instance of the PortalFilter class.</summary>
            /// <param name="list" type="PI.Portal.PortalList" />
            /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the PortalFilter.</param>
            /// <returns type="PI.Portal.PortalFilter" />
            /// </signature>
            options = options || {};
            this._disposed = false;
            this._list = list;

            this.project = options.project;
            this.name = _observable(options.name);
            this.orderBy = _observable(options.orderBy || ns.PortalSortOrder.none);
            this.fields = _observable(options.fields || ns.PortalField.none);

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
                    projectId: this.project && this.project.id,
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

        PortalListItem: _Class.define(function PortalListItem_ctor(portal) {
            /// <signature>
            /// <summary>Initializes a new instance of the PortalListItem class.</summary>
            /// <param name="portal" type="Object" />
            /// <returns type="PI.Portal.PortalListItem" />
            /// </signature>
            this.id = portal.id;
            this.name = portal.name;
            this.project = portal.project;
            this.uri = portal.uri;
            this.domain = portal.domain;
            this.createdDate = portal.createdDate;
            this.modifiedDate = portal.modifiedDate;
            this.owners = portal.owners || [];
        }),

        PortalListCommands: _Class.define(function PortalListCommands_ctor(list) {
            /// <signature>
            /// <param name="list" type="Object" optional="true" />
            /// <returns type="PI.Portal.PortalListCommands" />
            /// </signature>
            this._list = list;
        }, {
            current: {
                get: function () {
                    var current = this._list.selection()[0];
                    if (!current) {
                        throw new TypeError("There is no selected portal.");
                    }
                    return current;
                }
            },
            refresh: function () {
                this._list.refresh();
            },
            create: function () {
                _Global.open(ns.PortalResources.createLink(), "_self");
            },
            view: function () {
                _Global.open("/" + (this.current.domain ? this.current.domain : this.current.uri) + "?preview=true", "_blank");
            },
            edit: function () {
                _Global.open(ns.PortalResources.editLink(this.current.uri, "pages"), "_self");
            },
            copy: function () {
                this._list.service.copyAsync(this.current.uri, { uri: this.current.uri + "-" + _Resources.format(new Date(), "yyyy-MM-dd-hhmmss") }, this)
                    .then(function () {
                        this._list.refresh();
                    });
            },
            share: function () {
                _PI.dialog({
                    name: "security.acl",
                    params: {
                        objectType: "portal",
                        objectId: this.current.id
                    }
                });
            },
            remove: function () {
                var that = this;
                var current = this.current;
                _PI.dialog({
                    name: "confirm",
                    type: "remove",
                    done: function (response) {
                        if (response.result === "yes") {
                            that._list.service.deleteAsync(current.uri, this)
                                .then(function () {
                                    that._list.remove(current);
                                });
                        }
                    }
                });
            }
        }),

        PortalList: _Class.derive(_Knockout.PagedList, function PortalList_ctor(options) {
            /// <signature>
            /// <summary>Initializes a new instance of the PortalList class.</summary>
            /// <param name="options" type="Object" />
            /// <returns type="PI.Portal.PortalList" />
            /// </signature>
            options = options || {};
            options.autoLoad = options.autoLoad !== false;
            options.pageIndex = options.pageIndex || 1;
            options.pageSize = options.pageSize || 50;
            _Knockout.PagedList.call(this, options);

            this._disposed = false;
            this.service = options.service || ns.PortalService;
            this.commands = new ns.PortalListCommands(this);
            this.filter = new ns.PortalFilter(this, options.filter);
            options.autoLoad && this.refresh();
        }, {
            mapItem: function (item) {
                /// <signature>
                /// <param name="item" type="Object" />
                /// <returns type="PI.Portal.PortalListItem" />
                /// </signature>
                return new ns.PortalListItem(item);
            },
            refresh: function () {
                /// <signature>
                /// <summary>Refreshes the list.</summary>
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
                /// <summary>Deserializes the current filter object</summary>
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

    _Class.mix(ns.PortalItem, _Utilities.createEventProperties("loaded", "saved", "deleted", "discarded"));
    _Class.mix(ns.PortalItem, _Utilities.eventMixin);

})(window, ko, WinJS, PI);