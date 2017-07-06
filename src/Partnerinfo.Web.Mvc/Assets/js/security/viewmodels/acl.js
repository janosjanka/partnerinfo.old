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

    function normalize(str) {
        return str.substr(0, 1).toUpperCase() + str.substr(1);
    }

    var ns = _WinJS.Namespace.defineWithParent(_PI, "Security", {

        SecurityResources: {
            anyone: function () { return _T("pi/security/anyone"); },
            accessPermission: function (permission) { return _T("pi/security/accessPermission" + normalize(permission)); },
            accessVisibility: function (visibility) { return _T("pi/security/accessVisibility" + normalize(visibility)); }
        },

        AccessPermission: {
            /// <summary>
            /// Indicates that the role is not being used.
            /// </summary>
            unknown: "unknown",

            /// <summary>
            /// Grants permissions to view a business object.
            /// </summary>
            canView: "canView",

            /// <summary>
            /// Grants permissions to view and edit a business object.
            /// </summary>
            canEdit: "canEdit",

            /// <summary>
            /// Grants permissions to view, edit, and manage a business object.
            /// </summary>
            canManage: "canManage",

            /// <summary>
            /// Grants permissions to view, edit, manage, and delete a business object.
            /// </summary>
            isOwner: "isOwner"
        },

        AccessVisibility: {
            /// <field>
            /// Indicates that the ACL source type is not being used.
            /// </field>
            "unknown": "unknown",

            /// <field>
            /// Indicates that the ACL source object is a project.
            /// </field>
            "public": "public",

            /// <field>
            /// Indicates that the ACL source object is a blog.
            /// </field>
            "anyoneWithLink": "anyoneWithLink",

            /// <field>
            /// Indicates that the ACL source object is a blog.
            /// </field>
            "impersonated": "impersonated",

            /// <field>
            /// Indicates that the ACL source object is a blog.
            /// </field>
            "private": "private"
        },

        AccessRuleItem: _Class.define(function AccessRuleItem_ctor(objectType, objectId, rule, options) {
            /// <signature>
            /// <param name="objectType" type="String" />
            /// <param name="objectId" type="Number" />
            /// <param name="rule" type="Object" optional="true" />
            /// <param name="options" type="Object" optional="true" />
            /// <returns type="PI.Security.AccessRuleItem" />
            /// </signature>
            _Utilities.setOptions(this, options = options || {});
            _Promise.tasks(this);

            this.service = options.service || ns.AccessRuleService;

            this.objectType = objectType;
            this.objectId = objectId;

            this.anyone = _observable().extend({ required: true });
            this.email = _observable();
            this.permission = _observable().extend({ required: true });
            this.visibility = _observable();

            this.permissionOptions = ko.utils.optionsMap(ns.AccessPermission, function (p) { return { text: ns.SecurityResources.accessPermission(p), value: p }; });
            this.visibilityOptions = ko.utils.optionsMap(ns.AccessVisibility, function (p) { return { text: ns.SecurityResources.accessVisibility(p), value: p }; });

            this.update(rule);
        }, {
            update: function (rule) {
                /// <signature>
                /// <param name="ace" type="Object" />
                /// </signature>
                rule = rule || {};
                this.anyone(rule.anyone);
                this.email(rule.email);
                this.permission(rule.permission);
                this.visibility(rule.visibility);
            },
            toObject: function () {
                /// <signature>
                /// <returns type="Object" />
                /// </signature>
                var anyone = this.anyone();
                return {
                    anyone: anyone,
                    email: anyone ? null : this.email(),
                    permission: anyone ? "canView" : this.permission(),
                    visibility: anyone ? this.visibility() : "unknown"
                };
            },

            //
            // Validation
            //

            validate: function () {
                /// <signature>
                /// <summary>Returns true if this ACE is valid.</summary>
                /// <returns type="Boolean" />
                /// </signature>
                return this.errors().length === 0;
            },

            //
            // Storage Operations
            //

            setAsync: _Promise.tasks.watch(function () {
                /// <signature>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return this.service.setAsync(this.objectType, this.objectId, this.toObject(), this)
                    .then(function (rule) {
                        this.dispatchEvent("changed", rule);
                    });
            }),
            deleteAsync: _Promise.tasks.watch(function () {
                /// <signature>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return this.service.deleteAsync(this.objectType, this.objectId, this.toObject(), this)
                    .then(function () {
                        this.dispatchEvent("deleted");
                    });
            }),

            //
            // Event & Computed Members
            //

            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                this.errors && this.errors.dispose();
                this.errors = null;
            }
        }),

        AccessRuleListItem: _Class.define(function AccessRuleListItem(ace) {
            /// <signature>
            /// <param name="ace" type="Object" />
            /// <returns type="PI.Security.AccessRuleListItem" />
            /// </signature>
            this.anyone = ace.anyone;
            this.user = ace.user;
            this.permission = ace.permission;
            this.visibility = ace.visibility;

            this.userName = this.anyone ? ns.SecurityResources.anyone() : this.user.email.name;
            this.userEmail = this.anyone ? "-" : this.user.email.address;
            this.permissionText = ns.SecurityResources.accessPermission(this.permission);
            this.visibilityText = ns.SecurityResources.accessVisibility(this.visibility);
        }),

        AccessRuleListCommands: _Class.define(function AccessRuleListCommands_ctor(list) {
            /// <signature>
            /// <param name="list" type="Object" optional="true" />
            /// <returns type="PI.Security.AccessRuleListCommands" />
            /// </signature>
            this._list = list;
        }, {
            "selection.delete": function () {
                /// <signature>
                /// <summary>Deletes the selected item.</summary>
                /// </signature>
                var item = this._list.selection()[0];
                item && this._list.service.deleteAsync(this._list.objectType, this._list.objectId, item.user ? item.user.email.address : null, this)
                    .then(function () {
                        this._list.remove(item);
                    });
            }
        }),

        AccessRuleList: _Class.derive(_Knockout.List, function AccessRuleList_ctor(objectType, objectId, options) {
            /// <signature>
            /// <param name="objectType" type="String" />
            /// <param name="objectId" type="Number" />
            /// <param name="options" type="Object" />
            /// <returns type="PI.Security.AccesRuleList" />
            /// </signature>
            options = options || {};
            options.autoLoad = options.autoLoad !== false;
            _Knockout.List.call(this, options);

            this._disposed = false;
            this.service = options.service || ns.AccessRuleService;
            this.commands = new ns.AccessRuleListCommands(this);
            this.objectType = objectType;
            this.objectId = objectId;

            this.rule = new ns.AccessRuleItem(this.objectType, this.objectId);
            this.rule.addEventListener("changed", this.ruleChanged.bind(this), false);

            options.autoLoad && this.refresh();
        }, {
            ruleChanged: function (event) {
                /// <signature>
                /// <param name="event" type="Event" />
                /// </signature>
                this.refresh();
            },
            selectionChanged: function (selection) {
                /// <signature>
                /// <param name="selection" type="Array" />
                /// </signature>
                var item = selection[0];
                item && this.rule.update({
                    anyone: item.anyone,
                    email: item.user ? item.user.email.address : null,
                    permission: item.permission,
                    visibility: item.visibility
                });
                _Knockout.List.prototype.selectionChanged.call(this, selection);
            },
            mapItem: function (item) {
                /// <signature>
                /// <param name="item" type="Object" />
                /// <returns type="PI.Security.AccessRuleListItem" />
                /// </signature>
                return new ns.AccessRuleListItem(item);
            },
            refresh: function () {
                /// <signature>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return this.service.getAllAsync(this.objectType, this.objectId, this).then(
                    function (response) {
                        this.replaceAll.apply(this, response.data);
                        this.total = response.total;
                        this.ensureAnyone();
                    },
                    function () {
                        this.removeAll();
                        this.total = 0;
                        this.ensureAnyone();
                    });
            },
            ensureAnyone: function () {
                /// <signature>
                /// <summary>Ensures an Anyone user.</summary>
                /// </signature>
                if (this.objectType !== "portal" || this.objectType !== "page") {
                    // TODO: It is still hard-coded.
                    return;
                }
                var anyone = this.find(function (rule) { return rule.anyone; });
                if (!anyone) {
                    this.unshift({
                        anyone: true,
                        permission: ns.AccessPermission.canView,
                        visibility: ns.AccessVisibility.private
                    });
                }
            },
            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                if (this._disposed) {
                    return;
                }
                this.rule && this.rule.dispose();
                this.rule = null;
                _Knockout.List.prototype.dispose.call(this);
                this._disposed = true;
            }
        })

    });

    _Class.mix(ns.AccessRuleItem, _Utilities.createEventProperties("changed", "deleted"));
    _Class.mix(ns.AccessRuleItem, _Utilities.eventMixin);

})(window, ko, WinJS, PI);
