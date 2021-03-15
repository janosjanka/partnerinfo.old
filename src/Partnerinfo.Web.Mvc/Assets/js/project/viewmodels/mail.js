// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="../services/mail.js" />

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

        /// <field>
        /// Localizable message resources.
        /// </field>
        MailMessageResources: {
            createLink: function (projectId) { return _Utilities.actionLink("/admin/projects/{projectId}/#/mails/compose", { projectId: projectId }); },
            updateLink: function (projectId, id) { return _Utilities.actionLink("/admin/projects/{projectId}/#/mails/{id}/compose", { projectId: projectId, id: id }); },
            headerSummary: function (recipients, tagExpression) {
                var summary = String.format(_T("pi/project/mailMessageHeaderSummaryById"), recipients);
                if (tagExpression) {
                    summary += " + ";
                    summary += tagExpression;
                }
                return summary;
            },
            mailMessageSubjectRequired: function () { return _T("pi/project/mailMessageSubjectRequired"); }
        },

        MailMessageField: {
            /// <summary>
            /// No extra fields included in the result set.
            /// </summary>
            none: 0,

            /// <summary>
            /// The project is included in the result set. 
            /// </summary>
            project: 1 << 0,

            /// <summary>
            /// Body belongs to the mail message is included in the result set.
            /// </summary>
            body: 1 << 1
        },

        MailMessageHeader: _Class.define(function MailMessageHeader_ctor(mailMessage, options) {
            /// <signature>
            /// <summary>Initializes a new instance of the MailMessageHeader class.</summary>
            /// <param name="mailMessage" type="PI.Project.MailMessage" />
            /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the MailMessageHeader.</param>
            /// <returns type="MailMessageHeader" />
            /// </signature>
            var that = this;
            options = options || {};
            this._disposed = false;

            this.mailMessage = mailMessage;
            this.to = _observableArray(options.to || []);
            this.includeWithTags = _observableArray(options.includeWithTags || []);
            this.excludeWithTags = _observableArray(options.excludeWithTags || []);
            this.tagExpression = _observable();

            this.to.extend({ required: { onlyIf: function () { return !that._hasPredicate(); }, message: "Legalább egy címzettre vagy szűrési kifejezésre szükség van." } });

            this.errors = _KO.validation.group(this);
            this.summary = _pureComputed(this._getSummary, this);
        }, {
            _getSummary: function () {
                /// <signature>
                /// <summary>Writes a summary of this message header.</summary>
                /// </signature>
                return ns.MailMessageResources.headerSummary(this.to().length, this.tagExpression());
            },
            _hasPredicate: function () {
                /// <signature>
                /// <summary>Returns true if at leas one predicate is specified.</summary>
                /// <returns type="Boolean" />
                /// </signature>
                return (this.to().length | this.includeWithTags().length | this.excludeWithTags().length) > 0;
            },
            validate: function () {
                /// <signature>
                /// <summary>Returns true if this project is valid.</summary>
                /// <returns type="Boolean" />
                /// </signature>
                return this.errors().length === 0;
            },
            toObject: function () {
                /// <signature>
                /// <summary>Returns a pure JS object without KO observable functions</summary>
                /// <returns type="Object" />
                /// </signature>
                return {
                    to: this.to().map(function (c) { return c.id; }),
                    includeWithTags: this.includeWithTags().map(function (t) { return t.id; }),
                    excludeWithTags: this.excludeWithTags().map(function (t) { return t.id; })
                };
            },
            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                if (this._disposed) {
                    return;
                }
                this.summary && this.summary.dispose();
                this.summary = null;
                this.errors && this.errors.dispose();
                this.errors = null;
                this._disposed = true;
            }
        }),

        MailMessage: _Class.define(function MailMessage_ctor(mailMessage, options) {
            /// <signature>
            /// <summary>Initializes a new instance of the MailMessage class.</summary>
            /// <param name="mailMessage" type="Object" optional="true" />
            /// <param name="options" type="Object" optional="true" />
            /// <returns type="MailMessage" />
            /// </signature>
            mailMessage = mailMessage || {};
            options = options || {};
            _Utilities.setOptions(this, options);
            _Promise.tasks(this);

            this._disposed = false;
            this._options = options;

            this.service = options.service || ns.MailMessageService;
            this.project = mailMessage.project || options.project;
            this.header = new ns.MailMessageHeader(this, options.header);

            this.id = _observable();
            this.subject = _observable().extend({ required: { message: ns.MailMessageResources.mailMessageSubjectRequired() } });
            this.body = _observable();

            this.errors = _KO.validation.group(this);
            this.editing = _observable(false);
            this.exists = _pureComputed(this._exists, this);
            this.canSave = _pureComputed(this._canSave, this);
            this.canSend = _pureComputed(this._canSend, this);

            this.update(mailMessage);
        }, {
            validate: function (validateHeader) {
                /// <signature>
                /// <summary>Returns true if this project is valid.</summary>
                /// <param name="validateHeader" type="Boolean" optional="true" />
                /// <returns type="Boolean" />
                /// </signature>
                if (validateHeader !== false) {
                    return (this.header.errors().length | this.errors().length) === 0;
                }
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
                this._editSession = new _KO.editSession(this, { fields: ["id", "subject", "body"] });
                this.editing(true);
            },
            cancelEdit: function () {
                /// <signature>
                /// <summary>Discards changes since the last BeginEdit call.</summary>
                /// </signature>
                if (this._editSession) {
                    this._editSession.cancel();
                    this._editSession = null;
                    this.editing(false);
                }
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
            // Memory Operations
            //

            update: function (mailMessage) {
                /// <signature>
                /// <param name="action" type="Object" />
                /// </signature>
                mailMessage = mailMessage || {};
                this.project = mailMessage.project || this.project;
                this.id(mailMessage.id);
                this.subject(mailMessage.subject);
                this.body(mailMessage.body);
            },
            toObject: function () {
                /// <signature>
                /// <returns type="Object" />
                /// </signature>
                return {
                    id: this.id(),
                    subject: this.subject(),
                    body: this.body()
                };
            },

            //
            // Storage Operations
            //

            _canSave: function () {
                /// <signature>
                /// <summary>Returns true if this mail can be saved.</summary>
                /// <returns type="Boolean" />
                /// </signature>
                return !this.busy() && this.validate(false);
            },
            _canSend: function () {
                /// <signature>
                /// <summary>Returns true if this mail can be sent.</summary>
                /// <returns type="Boolean" />
                /// </signature>
                return !this.busy() && this.validate();
            },
            loadAsync: _Promise.tasks.watch(function () {
                /// <signature>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                if (this.exists()) {
                    return this.service.getByIdAsync(this.id(), ns.MailMessageField.project | ns.MailMessageField.body, this)
                        .then(function (mailMessage) {
                            this.update(mailMessage);
                            this.dispatchEvent("loaded");
                        });
                }
                return _Promise.error();
            }),
            saveAsync: _Promise.tasks.watch(function () {
                /// <signature>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                if (!this.canSave()) {
                    return _Promise.error();
                }
                var mailMessage = this.toObject();
                if (this.exists()) {
                    return this.service.updateAsync(mailMessage.id, mailMessage, this)
                        .then(function (mailMessage) {
                            this.update(mailMessage);
                            this.dispatchEvent("saved", { state: "modified", data: mailMessage });
                        });
                }
                return this.service.createAsync(this.project.id, mailMessage, this)
                    .then(function (mailMessage) {
                        this.update(mailMessage);
                        this.dispatchEvent("saved", { state: "added", data: mailMessage });
                    });
            }),
            sendAsync: _Promise.tasks.watch(function MailMessage_sendAsync() {
                /// <signature>
                /// <summary>Sends this mail message</summary>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                if (!this.canSend()) {
                    return _Promise.error();
                }
                var id = this.id();
                var header = this.header.toObject();
                return this.saveAsync().then(function () {
                    return this.service.sendAsync(id, header, this).then(function () {
                        this.dispatchEvent("sent");
                    });
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

            //
            // Disposable
            //

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
                this.canSave && this.canSave.dispose();
                this.canSave = null;
                this.canSend && this.canSend.dispose();
                this.canSend = null;
                this.header && this.header.dispose();
                this.header = null;
                this._disposed = true;
            }
        }),

        MailMessageListItem: _Class.define(function MailMessageListItem_ctor(mailMessage) {
            /// <signature>
            /// <summary>Initializes a new instance of the MailMessageListItem class.</summary>
            /// <param name="mailMessage" type="Object" optional="true">The set of options to be applied initially to the MailMessageListItem.</param>
            /// <returns type="MailMessageListItem" />
            /// </signature>
            mailMessage = mailMessage || {};
            var now = new Date();
            this.id = mailMessage.id;
            this.project = mailMessage.project;
            this.subject = mailMessage.subject;
            this.modifiedDate = new Date(mailMessage.modifiedDate);
            this.modifiedDateFmt = now.elapsed(this.modifiedDate).toString();
        }),

        MailMessageListFilter: _Class.define(function MailMessageListFilter_ctor(list, options) {
            /// <signature>
            /// <summary>Initializes a new instance of the MailMessageListFilter class.</summary>
            /// <param name="list" type="PI.Project.MailMessageList" />
            /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the MailMessageListFilter.</param>
            /// <returns type="MailMessageListFilter" />
            /// </signature>
            options = options || {};
            this._disposed = false;
            this._list = list;
            this.project = _observable(options.project);
            this.subject = _observable(options.subject);
            this._session = _KO.editSession(this, { fields: ["subject"] });
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
                /// <summary>Returns a pure JS object without KO observable functions</summary>
                /// <returns type="Object" />
                /// </signature>
                var project = this.project();
                return {
                    projectId: project && project.id,
                    subject: this.subject()
                };
            },
            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                if (this._disposed) {
                    return;
                }
                this._session && this._session.dispose();
                this._session = null;
                this._disposed = true;
            }
        }),

        MailMessageListCommands: _Class.define(function MailMessageListCommands_ctor(list) {
            /// <signature>
            /// <summary>Initializes a new instance of the MailMessageListCommands class.</summary>
            /// <param name="list" type="Object" optional="true">The set of options to be applied initially to the MailMessageListCommands.</param>
            /// <returns type="MailMessageListCommands" />
            /// </signature>
            this._list = list;
        }, {
            "list.create": function () {
                /// <signature>
                /// <summary>Copies the current item.</summary>
                /// </signature>
                var project = this._list.filter.project();
                project && _Global.open(ns.MailMessageResources.createLink(project.id), "_self");
            },
            "list.refresh": function () {
                /// <signature>
                /// <summary>Refreshes the list.</summary>
                /// </signature>
                this._list.refresh();
            },
            "selection.update": function () {
                /// <signature>
                /// <summary>Copies the current item.</summary>
                /// </signature>
                var project = this._list.filter.project();
                var item = this._list.selection()[0];
                project && item && _Global.open(ns.MailMessageResources.updateLink(project.id, item.id), "_self");
            },
            "selection.delete": function () {
                /// <signature>
                /// <summary>Deletes the first selected item in the selection collection.</summary>
                /// </signature>
                var item = this._list.selection()[0];
                item && this._list.service.deleteAsync(item.id, this)
                    .then(function () {
                        this._list.remove(item);
                    });
            }
        }),

        MailMessageList: _Class.derive(_Knockout.PagedList, function MailMessageList_ctor(options) {
            /// <signature>
            /// <summary>Initializes a new instance of the MailMessageList class.</summary>
            /// <param name="options" type="Object" />
            /// <returns type="MailMessageList" />
            /// </signature>
            options = options || {};
            options.autoLoad = options.autoLoad !== false;
            options.pageIndex = options.pageIndex || 1;
            options.pageSize = options.pageSize || 50;
            _Knockout.PagedList.call(this, options);

            this._disposed = false;
            this.service = options.service || ns.MailMessageService;
            this.commands = new ns.MailMessageListCommands(this);
            this.filter = new ns.MailMessageListFilter(this, options.filter);
            this.fields = options.fields || ns.MailMessageField.none;
            this.options.autoLoad && this.refresh();
        }, {
            mapItem: function (item) {
                /// <signature>
                /// <param name="item" type="Object" />
                /// <returns type="PI.Project.MailMessageListItem" />
                /// </signature>
                return new ns.MailMessageListItem(item);
            },
            refresh: _Promise.tasks.watch(function () {
                /// <signature>
                /// <summary>Refreshes the list.</summary>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                var params = this._createParams();
                return this.service.getAllAsync(params, this).then(
                    function (response) {
                        this.total = response.total;
                        this.replaceAll.apply(this, response.data);
                    },
                    function () {
                        this.total = 0;
                        this.removeAll();
                    });
            }),
            _createParams: function () {
                /// <signature>
                /// <returns type="Object" />
                /// </signature>
                var params = this.filter.toObject() || {};
                params.page = this.pageIndex;
                params.limit = this.pageSize;
                params.fields = this.fields;
                return params;
            },
            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                if (this._disposed) {
                    return;
                }
                _Knockout.PagedList.prototype.dispose.apply(this, arguments);
                this._disposed = true;
            }
        })

    });

    _Class.mix(ns.MailMessage, _Utilities.createEventProperties("loaded", "saved", "sent", "deleted", "discarded"));
    _Class.mix(ns.MailMessage, _Utilities.eventMixin);

})(window, ko, WinJS, PI);