// Copyright (c) Partnerinfo TV. All Rights Reserved.

/// <reference path="../services/media.js" />

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

        MediaSortOrder: {
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

        MediaField: {
            /// <field>
            /// No extra fields included in the result set.
            /// </field>
            none: "none",

            /// <field>
            /// The project is incldued in the result set.
            /// </field>
            portal: "portal"
        },

        MediaQuery: _Class.define(function MediaQuery_ctor(list, options) {
            /// <signature>
            /// <summary>Initializes a new instance of the PortalFilter class.</summary>
            /// <param name="list" type="PI.Portal.MediaList" />
            /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the PortalFilter.</param>
            /// <returns type="PI.Portal.MediaQuery" />
            /// </signature>
            options = options || {};
            this._disposed = false;
            this._list = list;

            this.portal = options.portal;
            this.media = _observable(options.media);
            this.name = _observable(options.name);
            this.orderBy = _observable(options.orderBy || ns.MediaSortOrder.none);
            this.fields = _observable(options.fields || ns.MediaField.none);

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
                /// <returns type="Object" />
                /// </signature>
                var portal = this.portal;
                var media = this.media();
                return {
                    portalUri: portal && portal.uri,
                    mediaUri: media && media.uri,
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

        MediaListItem: _Class.define(function MediaListItem_ctor(portal, media) {
            /// <signature>
            /// <param name="portal" type="Object" />
            /// <param name="media" type="Object" />
            /// <returns type="PI.Portal.MediaListItem" />
            /// </signature>
            this.portal = portal;
            this.id = media.id;
            this.name = media.name;
            this.uri = media.uri;
            this.modifiedDate = media.modifiedDate;
            this.link = media.link;
        }),

        MediaListCommands: _Class.define(function MediaListCommands_ctor(list) {
            /// <signature>
            /// <param name="list" type="Object" optional="true" />
            /// <returns type="PI.Portal.MediaListCommands" />
            /// </signature>
            this.list = list;
        }, {
            current: {
                get: function () {
                    var current = this.list.selection()[0];
                    if (!current) {
                        throw new TypeError("There is no selected media.");
                    }
                    return current;
                }
            },
            "refresh": function () {
                /// <signature>
                /// <summary>Refreshes the list.</summary>
                /// </signature>
                this.list.refresh();
            },
            "selection.delete": function () {
                /// <signature>
                /// <summary>Deletes the selected media items.</summary>
                /// </signature>
                var that = this;
                var current = this.current;
                _PI.dialog({
                    name: "confirm",
                    type: "remove",
                    done: function (response) {
                        (response.result === "yes") && that.list.service.deleteAsync(current.portal.uri, current.uri, this)
                            .then(function () {
                                that.list.remove(current);
                            });
                    }
                });
            }
        }),

        MediaList: _Class.derive(_Knockout.List, function MediaList_ctor(options) {
            /// <signature>
            /// <param name="options" type="Object" />
            /// <returns type="PI.Portal.MediaList" />
            /// </signature>
            options = options || {};
            options.autoLoad = options.autoLoad !== false;
            _Knockout.List.call(this, options);

            this._disposed = false;
            this.service = options.service || ns.MediaService;
            this.query = new ns.MediaQuery(this, options.query);
            this.uploadUrl = this.service.getUploadUrl(this.query.portal.uri);
            this.commands = new ns.MediaListCommands(this);

            options.autoLoad && this.refresh();
        }, {
            mapItem: function (item) {
                /// <signature>
                /// <param name="item" type="Object" />
                /// <returns type="PI.Portal.MediaListItem" />
                /// </signature>
                return new ns.MediaListItem(this.query.portal, item);
            },
            refresh: function () {
                /// <signature>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                var params = this._createParams();
                return this.service.getByUriAsync(params, this).then(
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
                return this.query.toObject() || {};
            },
            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                if (this._disposed) {
                    return;
                }
                this.query && this.query.dispose();
                this.query = null;
                _Knockout.List.prototype.dispose.call(this);
                this._disposed = true;
            }
        })

    });

})(window, ko, WinJS, PI);