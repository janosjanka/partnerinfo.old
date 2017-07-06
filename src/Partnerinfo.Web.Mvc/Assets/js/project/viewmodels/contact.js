// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="../services/contact.js" />

(function (_Global, _KO, _WinJS, _PI) {
    "use strict";

    var _Class = _WinJS.Class;
    var _Utilities = _WinJS.Utilities;
    var _Promise = _WinJS.Promise;
    var _Knockout = _WinJS.Knockout;
    var _Resources = _WinJS.Resources;

    var _observable = _KO.observable;
    var _observableArray = _KO.observableArray;
    var _pureComputed = _KO.pureComputed;

    function idMapper(obj) {
        return obj.id;
    }

    var ns = _WinJS.Namespace.defineWithParent(_PI, "Project", {

        ContactResources: {
            createLink: function (projectId) { return _Utilities.actionLink("/admin/projects/{projectId}/#/contacts/create", { projectId: projectId }); },
            updateLink: function (projectId, contactId) { return _Utilities.actionLink("/admin/projects/{projectId}/#/contacts/{contactId}", { projectId: projectId, contactId: contactId }); }
        },

        ContactSortOrder: {
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

        ContactField: {
            /// <field>
            /// No extra fields included in the result set.
            /// </field>
            none: 0,

            /// <field>
            /// The project is included in the result set. 
            /// </field>
            project: 1 << 0,

            /// <field>
            /// The sponsor is included in the result set.
            /// </field>
            sponsor: 1 << 1,

            /// <field>
            /// Body belongs to the mail message is included in the result set.
            /// </field>
            businessTags: 1 << 2
        },

        ContactMailAddress: _Class.define(function ContactMailAddress_ctor(mailAddress) {
            /// <signature>
            /// <summary>Initializes a new instance of the ContactPhones class.</summary>
            /// <param name="mailAddress" type="Object" optional="true">The set of options to be applied initially to the ContactPhones.</param>
            /// <returns type="ContactMailAddress" />
            /// </signature>
            this.address = _observable();
            this.name = _observable();
            this.update(mailAddress);
        }, {
            update: function (mailAddress) {
                /// <signature>
                /// <param name="mailAddress" type="Object" />
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

        ContactPhones: _Class.define(function ContactPhones_ctor(phones) {
            /// <signature>
            /// <summary>Initializes a new instance of the ContactPhones class.</summary>
            /// <param name="phones" type="Object" optional="true">The set of options to be applied initially to the ContactPhones.</param>
            /// <returns type="ContactPhones" />
            /// </signature>
            this.personal = _observable();
            this.business = _observable();
            this.mobile = _observable();
            this.other = _observable();
            this.update(phones);
        }, {
            update: function (phones) {
                /// <signature>
                /// <param name="phones" type="Object" />
                /// </signature>
                phones = phones || {};
                this.personal(phones.personal);
                this.business(phones.business);
                this.mobile(phones.mobile);
                this.other(phones.other);
            },
            toObject: function () {
                /// <signature>
                /// <returns type="Object" />
                /// </signature>
                return {
                    personal: this.personal(),
                    business: this.business(),
                    mobile: this.mobile(),
                    other: this.other()
                };
            }
        }),

        Contact: _Class.define(function Contact_ctor(project, contact, options) {
            /// <signature>
            /// <summary>Initializes a new instance of the Contact class.</summary>
            /// <param name="project" type="Object" />
            /// <param name="contact" type="Object" optional="true" />
            /// <param name="options" type="Object" optional="true">A set of key/value pairs that can be used to configure entity operations.</param>
            /// <returns type="PI.Project.Contact" />
            /// </signature>
            this._disposed = false;
            this._contact = contact || (contact = {});
            this._options = options || (options = {});

            _Utilities.setOptions(this, options);
            _Promise.tasks(this);

            this.service = options.service || ns.ContactService;
            this.project = project;

            this.id = _observable();
            this.sponsors = _observableArray();
            this.sponsor = _pureComputed(this._getSponsor, this);
            this.facebookId = _observable();
            this.email = new ns.ContactMailAddress();
            this.firstName = _observable();
            this.lastName = _observable();
            this.nickName = _observable();
            this.gender = _observable();
            this.birthday = _observable();
            this.modifiedDate = _observable();
            this.phones = new ns.ContactPhones();
            this.comment = _observable();
            this.properties = _observableArray();
            this.businessTags = _observableArray();
            this.businessTagsView = this.businessTags.map(function (businessTag) { return new ns.ContactTagItem(businessTag); });
            this.update(contact);

            this.errors = _KO.validation.group(this);
            this.editing = _observable(false);
            this.exists = _pureComputed(this._exists, this);
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
                this._editSession = new _KO.editSession(this, { fields: ["name", "color"] });
                this.editing(true);
            },
            cancelEdit: function () {
                /// <signature>
                /// <summary>Discards changes since the last beginEdit call.</summary>
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

            update: function (contact) {
                /// <signature>
                /// <param name="contact" type="Object" />
                /// </signature>
                this._contact = contact = contact || {};
                this.project = contact.project || this.project;

                this.id(contact.id);
                this.sponsors(contact.sponsor ? [contact.sponsor] : []);
                this.facebookId(contact.facebookId);
                this.email.update(contact.email);
                this.firstName(contact.firstName);
                this.lastName(contact.lastName);
                this.nickName(contact.nickName);
                this.gender(contact.gender);
                this.birthday(contact.birthday);
                this.modifiedDate(contact.modifiedDate);
                this.phones.update(contact.phones);
                this.comment(contact.comment);
                this.properties(contact.properties || []);
                this.businessTags(contact.businessTags || []);
            },
            toObject: function () {
                /// <signature>
                /// <returns type="Object" />
                /// </signature>
                var sponsor = this.sponsor();
                return {
                    id: this.id(),
                    sponsorId: sponsor ? sponsor.id : null,
                    facebookId: this.facebookId(),
                    email: this.email.toObject(),
                    firstName: this.firstName(),
                    lastName: this.lastName(),
                    nickName: this.nickName(),
                    gender: this.gender(),
                    birthday: this.birthday(),
                    modifiedDate: this.modifiedDate(),
                    phones: this.phones.toObject(),
                    comment: this.comment(),
                    properties: this.properties()
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
                    this.cancelEdit();
                    return this.service.getByIdAsync(this.id(), "project,sponsor,businessTags", this)
                        .then(function (contact) {
                            this.update(contact);
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
                var contact = this.toObject();
                if (this.exists()) {
                    return this.service.updateAsync(contact.id, contact, this)
                        .then(function (contact) {
                            this.endEdit();
                            this.update(contact);
                            this.dispatchEvent("saved", { state: "modified", data: contact });
                        });
                }
                return this.service.createAsync(this.project.id, contact, this)
                    .then(function (contact) {
                        this.endEdit();
                        this.update(contact);
                        this.dispatchEvent("saved", { state: "added", data: contact });
                    });
            }),
            deleteAsync: _Promise.tasks.watch(function () {
                /// <signature>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                this.cancelEdit();
                if (!this.exists()) {
                    this.remove();
                    return _Promise.complete();
                }
                return this.service.deleteAsync(this.id(), this)
                    .then(function () {
                        this.remove();
                        this.dispatchEvent("deleted");
                    });
            }),
            discard: function () {
                /// <signature>
                /// <summary>Discards data changes and raises an ondiscarded event that can be handled by views.</summary>
                /// </signature>
                this.cancelEdit();
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
            _getSponsor: function () {
                /// <signature>
                /// <summary>Returns the first sponsor.</summary>
                /// </signature>
                return this.sponsors()[0];
            },
            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                if (this._disposed) {
                    return;
                }
                this.errors && this.errors.dispose();
                this.errors = null;
                this.exists && this.exists.dispose();
                this.exists = null;
                this._disposed = true;
            }
        }),

        ContactFilter: _Class.define(function ContactFilter_ctor(list, options) {
            /// <signature>
            /// <summary>Initializes a new instance of the ContactFilter class.</summary>
            /// <param name="list" type="PI.Project.ContactList" />
            /// <param name="options" type="Object" optional="true" />
            /// <returns type="PI.Project.ContactFilter" />
            /// </signature>
            options = options || {};
            this._disposed = false;
            this._list = list;

            this.project = options.project;
            this.name = _observable(options.name);
            this.includeWithTags = _observableArray(options.includeWithTags || []);
            this.excludeWithTags = _observableArray(options.excludeWithTags || []);
            this.orderBy = _observable(options.orderBy || ns.ContactSortOrder.none);
            this.fields = _observable(options.fields || ns.ContactField.none);
            this.tagExpression = _observable();

            this._session = _KO.editSession(this, { fields: ["name", "includeWithTags", "excludeWithTags", "orderBy", "offset", "limit"] });
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
                /// <summary>Returns a pure JS object without knockout observable functions</summary>
                /// <returns type="Object" />
                /// </signature>
                return {
                    projectId: this.project && this.project.id,
                    name: this.name(),
                    includeWithTags: this.includeWithTags().map(idMapper),
                    excludeWithTags: this.excludeWithTags().map(idMapper),
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

        ContactTagItem: _Class.define(function ContactTagItem_ctor(contactTag) {
            /// <signature>
            /// <summary>Initializes a new instance of the ContactTagItem class.</summary>
            /// <param name="options" type="contactTag" />
            /// <returns type="PI.Project.ContactTagItem" />
            /// </signature>
            this.id = contactTag.id;
            this.name = contactTag.name;
            this.shortName = this.name.substr(0, 10);
            this.color = contactTag.color;
            this.createdDate = new Date(contactTag.createdDate);
            this.createdDateText = _Resources.format(this.createdDate, "F");
        }),

        ContactListItem: _Class.define(function ContactListItem_ctor(project, contact) {
            /// <signature>
            /// <summary>Initializes a new instance of the ContactListItem class. This is an immutable type.</summary>
            /// <param name="contact" type="Object"/>
            /// <param name="project" type="Object"/>
            /// <returns type="PI.Project.ContactListItem"/>
            /// </signature>
            this.project = project;
            this.contact = contact;

            this.id = contact.id;
            this.sponsor = contact.sponsor && new ns.ContactListItem(project, contact.sponsor);
            this.facebookId = contact.facebookId;
            this.email = contact.email || {};
            this.firstName = contact.firstName;
            this.lastName = contact.lastName;
            this.nickName = contact.nickName;
            this.gender = contact.gender;
            this.birthday = contact.birthday;
            this.modifiedDate = contact.modifiedDate;
            this.phones = contact.phones || {};
            this.comment = contact.comment;
            this.businessTags = contact.businessTags || [];
            this.businessTagsView = this.businessTags.map(function (tag) { return new ns.ContactTagItem(tag); });

            this.displayName = this.email.address || this.phones.personal || this.phones.business || this.phones.mobile || this.phones.other;
        }, {
            hasBusinessTagId: function (id) {
                /// <signature>
                /// <summary>Returns true if the contact has a tag with the given ID.</summary>
                /// <returns type="Boolean" />
                /// </signature>
                return this.businessTags.some(function (businessTag) {
                    return businessTag.id === id;
                });
            },
            shareWith: function () {
                /// <signature>
                /// <summary>Opens a dialog window for sharing an action link with the current contact.</summary>
                /// </signature>
                _PI.dialog({
                    name: "project.actionlink",
                    actionlink: {
                        project: this.project,
                        contact: this.contact
                    }
                });
            }
        }),

        ContactListCommands: _Class.define(function ContactListCommands_ctor(list) {
            /// <signature>
            /// <summary>Initializes a new instance of the ContactListCommands class.</summary>
            /// <param name="list" type="PI.Project.ContactList" />
            /// <returns type="PI.Project.ContactListCommands" />
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
                /// <summary>Navigates to a page where a contact can be added to the list.</summary>
                /// </signature>
                var project = this._list.filter.project;
                project && _Global.open(ns.ContactResources.createLink(project.id), "_self");
            },
            "selection.update": function () {
                /// <signature>
                /// <summary>Deletes the current item.</summary>
                /// </signature>
                var project = this._list.filter.project;
                var item = this._list.selection()[0];
                project && item && _Global.open(ns.ContactResources.updateLink(project.id, item.id), "_self");
            },
            "selection.delete": function () {
                /// <signature>
                /// <summary>Deletes the current item.</summary>
                /// </signature>
                var selection = this._list.selection().slice(0);
                if (selection.length === 0) {
                    return;
                }
                var that = this;
                _PI.dialog({
                    name: "confirm",
                    type: "remove",
                    done: function (event) {
                        if (event.result === "yes") {
                            that._list.service
                                .deleteAllAsync(that._list.filter.project.id, selection.map(idMapper), that)
                                .then(function () {
                                    that._list.refresh();
                                });
                        }
                    }
                });
            },
            "selection.setBusinessTags": _Promise.debounce(function (tagsToAdd, tagsToRemove) {
                /// <signature>
                /// <summary>Sets the business tags for the selected contacts.</summary>
                /// <param name="tagsToAdd" type="Array" />
                /// <param name="tagsToRemove" type="Array" />
                /// </signature>
                var selection = this._list.selection().slice(0);
                if (selection.length === 0 || (tagsToAdd.length | tagsToRemove.length) === 0) {
                    return;
                }
                var contactIds = selection.map(idMapper);
                var tagsToAddIds = tagsToAdd.map(idMapper);
                var tagsToRemoveIds = tagsToRemove.map(idMapper);
                this._list.service.setBusinessTagsAsync(contactIds, tagsToAddIds, tagsToRemoveIds, this)
                    .then(function () {
                        var newSelection = [];
                        selection.forEach(function (contact) {
                            /// <param name="contact" type="PI.Project.ContactListItem" />
                            var oldBusinessTags = contact.businessTags;
                            var newBusinessTags = tagsToAdd.slice(0);
                            for (var i = 0, len = oldBusinessTags.length; i < len; ++i) {
                                var oldBusinessTag = oldBusinessTags[i];
                                if (tagsToRemoveIds.indexOf(oldBusinessTag.id) < 0 && tagsToAddIds.indexOf(oldBusinessTag.id) < 0) {
                                    newBusinessTags.push(oldBusinessTag);
                                }
                            }
                            contact.contact.businessTags = newBusinessTags
                                .sort(function (t1, t2) {
                                    return t1.name.localeCompare(t2.name);
                                });
                            newSelection.push(this._list.getAt(this._list.replace(contact, new ns.ContactListItem(contact.project, contact.contact))));
                        }, this);
                        this._list.selection(newSelection);
                    });
            }, 1000)
        }),

        ContactList: _Class.derive(_Knockout.PagedList, function ContactList_ctor(options) {
            /// <signature>
            /// <summary>Initializes a new instance of the ContactList class.</summary>
            /// <param name="options" type="Object" />
            /// <returns type="PI.Project.ContactList" />
            /// </signature>
            options = options || {};
            options.autoLoad = options.autoLoad !== false;
            options.pageIndex = options.pageIndex || 1;
            options.pageSize = options.pageSize || 50;
            _Knockout.PagedList.call(this, options);

            this._disposed = false;
            this.service = options.service || ns.ContactService;
            this.commands = new ns.ContactListCommands(this);
            this.filter = new ns.ContactFilter(this, options.filter);
            this.refresh = _Promise.tasks.watch(_Promise.debounce(this._refreshAsync, options.refreshTimeout || 1000));
            this.options.autoLoad && this._refreshAsync();
        }, {
            mapItem: function (item) {
                /// <signature>
                /// <param name="item" type="Object" />
                /// <returns type="PI.Project.ContactListItem" />
                /// </signature>
                return new ns.ContactListItem(this.filter.project, item);
            },
            _refreshAsync: function () {
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
                _Knockout.PagedList.prototype.dispose.apply(this, arguments);
                this._disposed = true;
            }
        })

    });

    _Class.mix(ns.Contact, _Utilities.createEventProperties("loaded", "saved", "deleted", "discarded"));
    _Class.mix(ns.Contact, _Utilities.eventMixin);

})(window, ko, WinJS, PI);
