// Copyright (c) Partnerinfo TV. All Rights Reserved.

(function (_WinJS) {
    "use strict";

    var _Class = _WinJS.Class;
    var _Resources = _WinJS.Resources;
    var _Utilities = _WinJS.Utilities;

    var DocumentType = {
        unknown: "unknown",
        folder: "folder",
        file: "file"
    },

    Document = _Class.derive(PI.Entity, function Document_ctor(options) {
        /// <signature>
        /// <summary>Represents a view that can be used to define entity operations.</summary>
        /// <param name="options" type="Object">A set of key/value pairs that can be used to configure entity operations.</param>
        /// <returns type="Document" />
        /// </signature>
        options = options || {};
        options.item = Document.createItem(options.item);
        options = Document.createOptions(options);
        PI.Entity.call(this, options);
        this.lengthFmt = ko.pureComputed(this._formatLength, this);
    }, {
        _formatLength: function () {
            /// <signature>
            /// <summary>Returns string representation of the length.</summary>
            /// <returns type="String" />
            /// </signature>
            if (this.isFolder()) {
                return "<DIR>";
            }
            var len = ko.unwrap(this.item.length);
            if (len >= 1048576) {
                return _Resources.format(len / 1048576, "n0") + " MB";
            }
            return (_Resources.format(len / 1024, "n0") + " KB");
        },
        loadById: function (id) {
            /// <signature>
            /// <summary>Loads a document using the specified unique identifier.</summary>
            /// <returns type="$.Deferred" />
            /// </signature>
            return this.reload({ path: "drive/files/" + id });
        },
        loadBySlug: function (slug) {
            /// <signature>
            /// <summary>Loads a page using the specified URI parts.</summary>
            /// <returns type="$.Deferred" />
            /// </signature>
            return this.reload({ path: "drive/files/" + slug });
        },
        getType: function () {
            /// <signature>
            /// <summary>Gets the type of the current document.</summary>
            /// <returns type="PI.Drive.DocumentType" />
            /// </signature>
            return ko.unwrap(this.item.type);
        },
        isFolder: function () {
            /// <signature>
            /// <summary>Gets a value indicating whether the current document is a folder.</summary>
            /// <returns type="Boolean" />
            /// </signature>
            return this.getType() === DocumentType.folder;
        },
        share: function () {
            /// <signature>
            /// <summary>Prompts the user to share a document with somebody.</summary>
            /// </signature>
            PI.dialog({
                name: "security.acl",
                params: {
                    objectType: "file",
                    objectId: ko.unwrap(this.item.id)
                }
            });
        },
        rename: function (name) {
            /// <signature>
            /// <summary>Renames the current document.</summary>
            /// <param name="name" type="String">The name of the document.</param>
            /// <returns type="WinJS.Promise" />
            /// </signature>
            return PI.api({
                method: "put",
                path: String.format("drive/files/{0}/name", this.entityKey()),
                data: name
            }, this).done(function () {
                this.item.name(name);
            });
        },
        showRenameDialog: function () {
            var self = this;
            var dialog = $.WinJS.dialog({
                buttons: [{
                    "class": "ui-btn ui-btn-primary",
                    "text": _T("ui/done"),
                    "click": function () {
                        self.rename(ko.unwrap(self.item.name));
                        self.endEdit();
                        dialog.close();
                    }
                }, {
                    "class": "ui-btn",
                    "text": _T("ui/close"),
                    "click": function () {
                        dialog.close();
                    }
                }],
                open: function () {
                    PI.bind(this, self, "koDriveRename");
                    self.beginEdit();
                },
                close: function () {
                    self.cancelEdit();
                }
            });
        }
    }, {
        createOptions: function (/* options */) {
            /// <signature>
            /// <summary>Creates an options object using default options.</summary>
            /// <param name="options" type="Object" optional="true">A set of key/value pairs that can be used to configure options.</param>
            /// <returns type="Object" />
            /// </signature>
            var options = this._options;
            if (!options) {
                this._options = options = {
                    mapping: {
                        copy: ["id", "parent", "type", "length", "slug", "publicLink", "privateLink", "owners"],
                        createdDate: {
                            create: function (options) {
                                return options.data ? new Date(options.data) : null;
                            }
                        },
                        modifiedDate: {
                            create: function (options) {
                                return options.data ? ko.observable(new Date(options.data)) : ko.observable();
                            },
                            update: function (options) {
                                return options.data ? new Date(options.data) : null;
                            }
                        }
                    },
                    urls: {
                        query: "drive/files/{id}",
                        create: "drive/files",
                        update: "drive/files/{id}",
                        remove: "drive/files/{id}"
                    }
                };
            }
            return ko.utils.extend(ko.utils.extend({}, options), arguments[0]);
        },
        createItem: function (item) {
            /// <signature>
            /// <param name="item" type="Object" optional="true" />
            /// <returns type="Object" />
            /// </signature>
            return ko.utils.extend({
                id: null,
                parentId: null,
                parent: null,
                type: DocumentType.unknown,
                name: null,
                length: null,
                slug: null,
                publicLink: null,
                privateLink: null,
                schema: null,
                createdDate: null,
                modifiedDate: null
            }, item);
        }
    }),

    DocumentFilter = _Class.derive(PI.EntityFilter, function DocumentFilterCtor(options) {
        /// <signature>
        /// <summary>An object that can be used to filter data.</summary>
        /// <param name="options" type="Object">The name of the document.</param>
        /// <returns type="DocumentFilter" />
        /// </signature>
        options = options || {};
        options.required = options.required || ["parentId"];
        this.type = ko.observable(options.type);
        this.parentId = ko.observable(options.parentId);
        this.name = ko.observable(options.name);
        PI.EntityFilter.apply(this, [options]);
    }),

    DocumentList = _Class.derive(PI.EntityCollection, function DocumentListCtor(options) {
        /// <signature>
        /// <param name="options" type="Object" />
        /// <returns type="DocumentList" />
        /// </signature>
        options = options || {};
        options.autoOpen = false;
        options.cacheKey = options.cacheKey || "files";
        options.urls = options.urls || { query: "drive/files" };
        options.filter = options.filter || new DocumentFilter();
        options.oncurrentchanged = options.oncurrentchanged || this.currentChanged;

        PI.EntityCollection.apply(this, [options]);

        var that = this;

        this.folder = new Document({
            item: {
                type: DocumentType.folder
            },
            oncreated: function (event) {
                event.target.item.name(null);
                event.target.item.parent = null;
                event.target.reset();
                that.refresh(true);
            }
        });

        this.currentFolder = ko.observable();
        this.currentFolderSn = this.currentFolder.subscribe(this.currentFolderChanged, this);

        this.uploader = {
            makeUrl: function () {
                var folder = that.currentFolder();
                return _Utilities.actionLink("filestore/{parentId}", {
                    parentId: folder && ko.unwrap(folder.item.id) || null
                });
            }
        };

        // Loads the specified folder using URL hash parameters.
        this.openFolder(_Utilities.currentLinkHash().folder);
    }, {
        mapItem: function (item) {
            /// <signature>
            /// <param name="item" type="Object" />
            /// <returns type="Object" />
            /// </signature>
            return new Document({ item: item });
        },
        currentFolderChanged: function (file) {
            /// <signature>
            /// <param name="file" type="Document" />
            /// </signature>
            _Utilities.currentLinkHash(file ? { folder: ko.unwrap(file.item.id) } : null);
        },
        openFolder: function (idAccessor) {
            /// <signature>
            /// <summary>Opens the specified folder.</summary>
            /// </signature>
            var id = ko.unwrap(idAccessor);
            if (id) {
                var that = this;
                var document = new Document({ item: { id: id }, mapping: null });
                document.reload().then(
                    function () {
                        that.currentFolder(document);
                        that.options.filter.parentId(id);
                        that.folder.item.parentId(id);
                        that.refresh(true);
                    },
                    function () {
                        that.openFolder();
                    });
            } else {
                this.currentFolder(null);
                this.options.filter.parentId(null);
                this.folder.item.parentId(null);
                this.refresh(true);
            }
        },
        openFile: function (file) {
            /// <signature>
            /// <summary>Opens the specified document regarding to type.</summary>
            /// </signature>
            if (file.isFolder()) {
                this.openFolder(file.item.id);
            } else {
                window.open(ko.unwrap(file.item.publicLink));
            }
        }
    });

    //
    // Public Namespaces & Classes
    //

    _WinJS.Namespace.defineWithParent(PI, "Drive", {
        DocumentType: DocumentType,
        Document: Document,
        DocumentFilter: DocumentFilter,
        DocumentList: DocumentList
    });

    //
    // Public UI & Dialog Extensions
    //

    PI.component({
        name: "drive",
        model: function (options) {
            return new DocumentList({
                autoOpen: true,
                filter: new DocumentFilter(options.filter),
                onopened: options.onopened,
                oncurrentchanged: options.oncurrentchanged,
                onselectionchanged: options.onselectionchanged
            });
        },
        view: function (model, options) {
            return $.when(
                PI.bind(options.element, model, "koDriveList"),
                PI.bind(options.menu, model, "koDriveMenu"));
        }
    });

    PI.component({
        name: "drive.filepicker",
        model: function (options) {
            return new DocumentList({
                autoOpen: true,
                filter: new DocumentFilter(options.filter)
            });
        },
        view: function (model, options) {
            return PI.bind(options.element, model, "koDriveDialog");
        },
        dialog: function (model, options, response, done) {
            return $.WinJS.dialog({
                width: 900,
                height: 500,
                title: _T("documents"),
                buttons: [{
                    "class": "ui-btn ui-btn-primary",
                    "text": _T("ui/done"),
                    "click": function () {
                        response.items = ko.unwrap(model.selection);
                        done.apply(model, ["ok"]);
                    }
                }, {
                    "class": "ui-btn",
                    "text": _T("ui/close"),
                    "click": done.bind(model, "cancel")
                }]
            });
        }
    });

})(WinJS);
