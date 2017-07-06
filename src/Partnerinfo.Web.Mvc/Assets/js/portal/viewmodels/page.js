// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="../services/page.js" />

(function (_Global, _KO, _WinJS, _PI) {
    "use strict";

    var _Class = _WinJS.Class;
    var _Utilities = _WinJS.Utilities;
    var _Promise = _WinJS.Promise;
    var _Knockout = _WinJS.Knockout;

    var _observable = _KO.observable;
    var _observableArray = _KO.observableArray;
    var _pureComputed = _KO.pureComputed;

    var ns = _WinJS.Namespace.defineWithParent(_PI, "Portal", {

        PageUrlHelper: {
            itemAction: function (portalUri, pageUri, action) { return _Utilities.actionLink("/admin/portals/{portalUri}/#/{action}/{pageUri...}", { portalUri: portalUri, pageUri: pageUri, action: action }); },
            viewAction: function (portalUri, pageUri, preview) { return _Utilities.actionLink("{portalUri}/{pageUri...}", { portalUri: portalUri, pageUri: pageUri, preview: preview }); }
        },

        PageField: {
            /// <summary>
            /// No extra fields included in the result set.
            /// </summary>
            none: "none",

            /// <summary>
            /// The project is included in the result set. 
            /// </summary>
            portal: "portal",

            /// <summary>
            /// The master is included in the result set. 
            /// </summary>
            master: "master",

            /// <summary>
            /// The content is included in the result set.
            /// </summary>
            content: "content"
        },

        PageItem: _Class.define(function PageItem_ctor(portal, page, options) {
            /// <signature>
            /// <summary>Initializes a new instance of the PageItem class.</summary>
            /// <param name="portal" type="Object" />
            /// <param name="page" type="Object" optional="true" />
            /// <param name="options" type="Object" optional="true">A set of key/value pairs that can be used to configure entity operations.</param>
            /// <returns type="PI.Page.PageItem" />
            /// </signature>
            _Utilities.setOptions(this, options = options || {});
            _Promise.tasks(this);

            this.service = options.service || ns.PageService;

            this.portal = portal;
            this.originalUri = null;
            this.uri = _observable().extend({ required: { message: _T("pi/portal/pageUriRequired") } });
            this.name = _observable().extend({ required: { message: _T("pi/portal/pageNameRequired") } });
            this.description = _observable();
            this.htmlContent = _observable();
            this.styleContent = _observable();

            this.errors = _KO.validation.group(this);
            this.editing = _observable(false);
            this.exists = _observable(false);

            this.update(page);
            this.autoGenerateUri = options.autoGenerateUri !== false;
        }, {
            /// <field type="Boolean">
            /// If true, automatically generates a URI for this page.
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
                /// <summary>Returns true if this page is valid.</summary>
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
                this._editSession = new _KO.editSession(this, { fields: ["uri", "name", "description"] });
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

            update: function (page) {
                /// <signature>
                /// <param name="page" type="Object" />
                /// </signature>
                page = page || {};
                this.originalUri = page.uri;
                this.uri(page.uri);
                this.name(page.name);
                this.description(page.description);
            },
            updateContent: function (page) {
                /// <signature>
                /// <param name="page" type="Object" />
                /// </signature>
                page = page || {};
                this.htmlContent(page.htmlContent);
                this.styleContent(page.styleContent);
            },
            toObject: function () {
                /// <signature>
                /// <returns type="Object" />
                /// </signature>
                return {
                    uri: this.uri(),
                    name: this.name(),
                    description: this.description(),
                    htmlContent: this.htmlContent(),
                    styleContent: this.styleContent()
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
                return this.service.getByUriAsync(this.portal.uri, this.originalUri, null, this)
                    .then(function (page) {
                        this.autoGenerateUri = false;
                        this.update(page);
                        this.updateContent(page);
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
                var page = this.toObject();
                if (this.originalUri) {
                    return this.service.updateAsync(this.portal.uri, this.originalUri, page, this)
                        .then(function (page) {
                            this.update(page);
                            this.dispatchEvent("saved", { state: "modified", data: page });
                        });
                }
                return this.service.createAsync(this.portal.uri, null, page, this)
                    .then(function (page) {
                        this.update(page);
                        this.dispatchEvent("saved", { state: "added", data: page });
                    });
            }),
            saveContentAsync: _Promise.tasks.watch(function () {
                /// <signature>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                if (!this.originalUri) {
                    return _Promise.error();
                }
                var page = this.toObject();
                return this.service.setContentAsync(this.portal.uri, this.originalUri, page, this)
                  .then(function (page) {
                      this.updateContent(page);
                      this.dispatchEvent("saved", { state: "modified", data: page });
                  });
            }),
            copyAsync: _Promise.tasks.watch(function () {
                /// <signature>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                var that = this;
                var newUri = this.originalUri === this.uri() ? this.originalUri + "-copy" : this.uri();
                return this.service.copyAsync(this.portal.uri, this.originalUri, { uri: newUri }, this)
                    .then(function (page) {
                        return _Promise.complete(new ns.PageItem(that.portal, page, { service: that.service }));
                    });
            }),
            deleteAsync: _Promise.tasks.watch(function () {
                /// <signature>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return this.service.deleteAsync(this.portal.uri, this.originalUri, this)
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

            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                this.errors && this.errors.dispose();
                this.errors = null;
                this.exists && this.exists.dispose();
                this.exists = null;
            }
        })

    });

    _Class.mix(ns.PageItem, _Utilities.createEventProperties("loaded", "saved", "deleted", "discarded"));
    _Class.mix(ns.PageItem, _Utilities.eventMixin);

})(window, ko, WinJS, PI);
