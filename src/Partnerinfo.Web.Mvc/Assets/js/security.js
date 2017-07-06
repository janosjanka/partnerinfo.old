// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (_WinJS, _Knockout) {
    "use strict";

    /*
    var _Class = _WinJS.Class;

    var AccessSource = {
        unknown: "unknown",
        project: "project",
        file: "file",
        portal: "portal",
        page: "page"
    },

    AccessScope = {
        unknown: "unknown",
        anyone: "anyone",
        user: "user"
    },

    AccessPermission = {
        unknown: "unknown",
        canView: "canView",
        canEdit: "canEdit",
        isOwner: "isOwner"
    },

    AccessVisibility = {
        "unknown": "unknown",
        "public": "public",
        "anyoneWithLink": "anyoneWithLink",
        "impersonated": "impersonated",
        "private": "private"
    },

    AceEntryAnyone = _Class.derive(_Knockout.Editable, function AceEntryAnyone_ctor(options) {
        /// <signature>
        /// <summary>Represents a view that can be used to define entity operations.</summary>
        /// <param name="options" type="Object" optional="true">A set of key/value pairs that can be used to configure entity operations.</param>
        /// <returns type="AceEntryAnyone" />
        /// </signature>
        options = options || {};
        options.item = AceEntryAnyone.createItem(options.item);
        _Knockout.Editable.apply(this, [options]);
    }, null, {
        createItem: function (item) {
            /// <signature>
            /// <summary>Creates a native AceEntryUser object.</summary>
            /// <param name="item" type="Object" optional="true">A native JS object to extend.</param>
            /// <returns type="Object" />
            /// </signature>
            return ko.utils.extend({
                id: null,
                type: null,
                visibility: AccessVisibility.unknown
            }, item);
        }
    }),

    AceEntryUser = _Class.derive(_Knockout.Editable, function AceEntryUser_ctor(options) {
        /// <signature>
        /// <summary>Represents a view that can be used to define entity operations.</summary>
        /// <param name="options" type="Object" optional="true">A set of key/value pairs that can be used to configure entity operations.</param>
        /// <returns type="AceEntryUser" />
        /// </signature>
        options = options || {};
        options.item = AceEntryUser.createItem(options.item);
        _Knockout.Editable.apply(this, [options]);
    }, null, {
        createItem: function (item) {
            /// <signature>
            /// <summary>Creates a native AceEntryUser object.</summary>
            /// <param name="item" type="Object" optional="true">A native JS object to extend.</param>
            /// <returns type="Object" />
            /// </signature>
            return ko.utils.extend({
                id: null,
                type: null,
                email: null,
                permission: AccessPermission.unknown
            }, item);
        }
    }),

    AceResult = _Class.derive(PI.Entity, function AceResult_ctor(options) {
        /// <signature>
        /// <summary>Represents a view that can be used to define entity operations.</summary>
        /// <param name="options" type="Object" optional="true">A set of key/value pairs that can be used to configure entity operations.</param>
        /// <returns type="AceResult" />
        /// </signature>
        options = options || {};
        options.urls = options.urls || { query: "acl/{id}", create: "acl", update: "acl/{id}", remove: "acl/{id}" };
        options.item = AceResult.createItem(options.item);
        PI.Entity.apply(this, [options]);
    }, null, {
        createItem: function (item) {
            /// <signature>
            /// <summary>Creates a native Ace object.</summary>
            /// <param name="item" type="Object" optional="true">A native JS object to extend.</param>
            /// <returns type="Object" />
            /// </signature>
            return ko.utils.extend({
                id: null,
                scope: null,
                user: null,
                permission: null,
                visibility: null
            }, item);
        }
    }),

    AceFilter = _Class.derive(PI.EntityFilter, function AceFilter_ctor(options) {
        /// <signature>
        /// <summary>Represents a view that can be used to define AJAX query parameters.</summary>
        /// <param name="options" type="Object" optional="true">A set of key/value pairs that contains filter parameters.</param>
        /// <returns type="AceFilter" />
        /// </signature>
        options = options || {};
        options.required = options.required || ["sourceType", "sourceId"];
        PI.EntityFilter.apply(this, [options]);

        this.sourceType = ko.observable(options.sourceType);
        this.sourceId = ko.observable(options.sourceId);
    }),

    Acl = _Class.derive(PI.EntityCollection, function Acl_ctor(options) {
        /// <signature>
        /// <summary>Represents a view for grouping, sorting, filtering, and navigating a paged data collection.</summary>
        /// <param name="options" type="Object" optional="true">A set of key/value pairs.</param>
        /// <returns type="AceList" />
        /// </signature>
        options = options || {};
        options.cacheKey = options.cacheKey || "acl";
        options.supportsAnyone = options.supportsAnyone || false;
        options.urls = options.urls || { query: "acl/{sourceType}/{sourceId}", remove: "acl" };
        options.filter = new AceFilter(options.filter);
        options.supportsPaging = false;
        options.onopened = options.onopened || this.opened;
        PI.EntityCollection.apply(this, [options]);

        this.items = this.items.filter(function (item) { return !!item; });
        this.aceAnyone = ko.observable(); // Store the current ACE entry for the scope 'Anyone'.
        this.aceUser = ko.observable(); // Store the current ACE entry for the scope 'User'.

        this.ensureAceUser();
    }, {
        mapItem: function (item) {
            /// <signature>
            /// <summary>Represents a function called before adding a new item to the collection.</summary>
            /// <param name="item" type="Object">The item to add.</param>
            /// <returns type="Object" />
            /// </signature>
            var mappedItem = new AceResult({ item: item });
            if (item && item.scope === AccessScope.anyone) {
                this.ensureAceAnyone(item.visibility);
                return null;
            }
            return mappedItem;
        },
        saveAceAnyone: function () {
            /// <signature>
            /// <summary>Saves the current ACE user.</summary>
            /// </signature>
            var aceAnyone = this.aceAnyone();
            if (aceAnyone) {
                aceAnyone.endEdit();
                PI.api({
                    method: "post",
                    path: "acl/anyone",
                    data: aceAnyone.toObject()
                }, this).done(function () {
                    this.refresh(true);
                });
            }
        },
        saveAceUser: function () {
            /// <signature>
            /// <summary>Saves the current ACE user.</summary>
            /// </signature>
            var aceUser = this.aceUser();
            if (aceUser) {
                aceUser.endEdit();
                PI.api({
                    method: "post",
                    path: "acl/user",
                    data: aceUser.toObject()
                }, this).done(function () {
                    this.ensureAceUser();
                    this.refresh(true);
                });
            }
        },
        ensureAceAnyone: function (visibility) {
            /// <signature>
            /// <summary>Ensures a user ACE scope.</summary>
            /// </signature>
            if (!this.aceAnyone()) {
                var f = this.options.filter;
                this.aceAnyone(new AceEntryAnyone({
                    item: {
                        type: ko.unwrap(f.sourceType),
                        id: ko.unwrap(f.sourceId),
                        visibility: visibility || AccessVisibility.unknown
                    }
                }));
            }
        },
        ensureAceUser: function () {
            /// <signature>
            /// <summary>Ensures a user ACE scope.</summary>
            /// </signature>
            var f = this.options.filter;
            this.aceUser(new AceEntryUser({
                item: {
                    type: ko.unwrap(f.sourceType),
                    id: ko.unwrap(f.sourceId),
                    email: null,
                    permission: AccessPermission.canEdit
                }
            }));
        },
        opened: function (event) {
            /// <signature>
            /// <summary>Invoked during the transition of a collection into the opened state.</summary>
            /// <param name="event" type="Event">The event object.</param>
            /// </signature>
            event.target.ensureAceAnyone();
        }
    });

    function getPermissionId(permission) {
        /// <signature>
        /// <summary>Get a number for the specified access permission.</summary>
        /// <param name="permission" type="String">A value indicating a permission value.</param>
        /// <returns type="Number" />
        /// </signature>
        if (isNaN(permission)) {
            permission = String(permission).toLowerCase();
            switch (permission) {
                case "canview": return 10;
                case "canedit": return 20;
                case "isowner": return 30;
                default: return 0;
            }
        }
        return permission;
    }

    function checkAccess(permission, minPermission) {
        /// <signature>
        /// <summary>Compares the two access permissions.</summary>
        /// <returns type="Boolean" />
        /// </signature>
        return getPermissionId(permission) >= getPermissionId(minPermission);
    }

    //
    // Public Namespace Exports
    //

    _WinJS.Namespace.defineWithParent(PI, "Security", {
        AccessSource: AccessSource,
        AccessScope: AccessScope,
        AccessPermission: AccessPermission,
        AccessVisibility: AccessVisibility,
        AceEntryAnyone: AceEntryAnyone,
        AceEntryUser: AceEntryUser,
        AceResult: AceResult,
        Acl: Acl,
        checkAccess: checkAccess
    });
*/
})(WinJS, WinJS.Knockout);
