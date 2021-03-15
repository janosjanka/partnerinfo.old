// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="../../types.js" />
/// <reference path="../engine.js" />

(function (ko, _WinJS, _PI, undefined) {
    "use strict";

    var _Class = _WinJS.Class;
    var _Promise = _WinJS.Promise;
    var _Resources = _WinJS.Resources;
    var _Utilities = _WinJS.Utilities;
    var _metaTagRoot = document.getElementsByTagName("head")[0];
    var _observable = ko.observable;
    var _pureComputed = ko.pureComputed;
    var _unwrap = ko.unwrap;

    function padLeft(text, totalWidth, paddingChar) {
        /// <signature>
        /// <param name="text" type="String" />
        /// <param name="totalWidth" type="Number" />
        /// <param name="paddingChar" type="String" optional="true" value=" " />
        /// <returns type="String" />
        /// </signature>
        var s = "";
        var c = totalWidth - text.length;
        paddingChar || (paddingChar = " ");
        while (--c >= 0 && (s += paddingChar));
        return s + text;
    }

    var ns = _WinJS.Namespace.defineWithParent(_PI, "Project.Search", {

        PlaylistModule: _Class.define(function PlaylistModule_ctor(engine) {
            /// <signature>
            /// <param name="engine" type="PI.Project.Search.Engine" />
            /// <returns type="PI.Project.Search.PlaylistModule" />
            /// </signature>
            this._disposed = false;

            this.engine = engine;
            this.visible = _observable(_unwrap(engine.playlistEnabled) && engine.playlistVisible);
            this.playMode = _observable(engine.playlistMode || ns.PlayMode.repeatAll);
            this.saveInfoVisible = _observable(false);

            // Represents a reserved playlist that is similar to a shop cart.
            // It can be seen on the right hand side of the screen.
            this.defaultPlaylist = new ns.Playlist(this.engine, {
                autoReload: false,
                onsaved: this._defaultPlaylistSaved.bind(this)
            });

            // Represents a mutable playlist that is loaded by URL or manually.
            // It can be seen at the bottom of a search module as a list of thumbnails by default.
            this.sharedPlaylist = _observable();

            // This is a list of playlist items come from different playlists
            // using a filter expression. In contrast to the playlist.itemList.filter member,
            // it does not depends on owner playlists.
            this.mixedPlaylistItemList = new ns.PlaylistItemList(this.engine, {
                filter: new ns.PlaylistItemFilter({ playlistId: -1 }) // No playlistId filter specified.
            });

            // Represents a list of playlists that can be seen on the right hand side of the screen
            // when a user is authenticated, in a dropdown menu by default.
            this.playlistList = new ns.PlaylistList(this.engine, {
                autoOpen: false,
                oncurrentchanged: this._playlistListItemChanged.bind(this)
            });

            // Represents a list of playlists that can be seen on the left hand side of the screen
            // next to the main searchbox, in a dropdown menu. It is similar to the playlistList above,
            // but all playlist items will be loaded into the shared playlist.
            this.sharedPlaylistList = new ns.PlaylistList(this.engine, {
                autoOpen: false,
                oncurrentchanged: this._sharedListItemChanged.bind(this)
            });

            this._sharedOrCurrent = _pureComputed(this._getSharedOrDefault, this).extend({ deferred: true });
            this._currentOrDefault = _pureComputed(this._getCurrentOrDefault, this).extend({ deferred: true });
            this._defaultVisible = _pureComputed(this._isDefaultVisible, this);
        }, {
            /// <field type="PI.Project.Search.Playlist">
            /// Gets the active, current, or default (cart) playlists.
            /// </field>
            sharedOrCurrent: {
                enumerable: false,
                get: function () {
                    return this._sharedOrCurrent();
                }
            },

            /// <field type="PI.Project.Search.Playlist">
            /// Gets the current or default (cart) playlists.
            /// </field>
            currentOrDefault: {
                enumerable: false,
                get: function () {
                    return this._currentOrDefault();
                }
            },

            /// <field type="Boolean">
            /// Gets a value whether the default playlist is visible.
            /// </field>
            defaultVisible: {
                enumerable: false,
                get: function () {
                    return this._defaultVisible();
                }
            },

            /// <field type="Boolean">
            /// Gets a value whether the default playlist has already been saved.
            /// </field>
            defaultExists: {
                enumerable: false,
                get: function () {
                    return this.defaultVisible ? this.defaultPlaylist.exists() : true;
                }
            },

            /// <field type="PI.Project.Search.EditMode">
            /// Gets or sets a URI hash can be used to share the specified playlist with a user.
            /// </field>
            editMode: {
                enumerable: false,
                get: function () {
                    return this.engine.getHashParam(ns.HashParam.editMode);
                },
                set: function (value) {
                    this.engine.setHashParam(ns.HashParam.editMode, value);
                }
            },

            /// <field type="String">
            /// Gets or sets a URI hash can be used to share the specified playlist with a user.
            /// </field>
            playlistUri: {
                enumerable: false,
                get: function () {
                    return this.engine.getHashParam(ns.HashParam.playlistUri);
                },
                set: function (value) {
                    this.engine.setHashParam(ns.HashParam.playlistUri, value);
                }
            },

            _getSharedOrDefault: function () {
                /// <signature>
                /// <returns type="PI.Project.Search.Playlist" />
                /// </signature>
                return this.sharedPlaylist() || this._getCurrentOrDefault();
            },
            _getCurrentOrDefault: function () {
                /// <signature>
                /// <returns type="PI.Project.Search.Playlist" />
                /// </signature>
                return this.playlistList.current() || this.defaultPlaylist;
            },
            _isDefaultVisible: function () {
                /// <signature>
                /// <returns type="Boolean" />
                /// </signature>
                return !this.defaultPlaylist.itemList.isEmpty || !this.playlistList.current();
            },

            //
            // Event Handlers
            //

            _playlistListItemChanged: function (event) {
                /// <signature>
                /// <summary>Occurs when the current playlist changes.</summary>
                /// </signature>
                var that = this;
                var playlist = event.detail.newItem;
                playlist && ko.tasks.schedule(function () {
                    that.setPlaylist(playlist).then(function () {
                        var playlistItem = playlist.itemList.getAt(0);
                        if (playlistItem) {
                            playlist.itemList.moveToIndex(0);
                            that.engine.playItem(playlistItem);
                        }
                    });
                });
            },
            _defaultPlaylistSaved: function () {
                /// <signature>
                /// <summary>Fires when the default playlist has been saved successfully.</summary>
                /// </signature>
                var that = this;
                var playlistData = this.defaultPlaylist.toObject();
                this.playlistList.refresh().then(function () {
                    var playlist = that.playlistList.findById(playlistData.id);
                    if (playlist) {
                        that.playlistList.moveTo(playlist);
                    } else {
                        that.playlistList.unshift(playlistData);
                        that.playlistList.moveToIndex(0);
                    }
                });
            },

            //
            // Playlist mutation functions
            // These functions edit the current playlist can be a user || a default playlist
            // Each call to these functions will be dispatched to the corresponding playlist
            //

            addNewItems: function (playlistItems, playFirst) {
                /// <signature>
                /// <param name="playlistItems" type="Array" />
                /// <param name="playFirst" type="Boolean" optional="true" />
                /// </signature>
                var itemsToAdd = playlistItems.map(function (it) {
                    var playlistItem = it.toObject ? it.toObject() : it;
                    playlistItem.id = 0;
                    playlistItem.playlistId = 0;
                    return playlistItem;
                });
                var playlist = this.playlistList.current();
                if (playlist) {
                    playlist.itemList.unshift.apply(playlist.itemList, itemsToAdd);
                    //playlist.itemList.push.apply(playlist.itemList, itemsToAdd);
                    playlist.itemList.save().then(function () {
                        playlist.itemList.refresh();
                    });
                } else {
                    // Add the specified item(s) to the list 
                    // and move the cursor to the first element of the added items.
                    this.defaultPlaylist.itemList.unshift.apply(this.defaultPlaylist.itemList, itemsToAdd);
                    this.defaultPlaylist.itemList.moveToIndex(0);
                    //var newLength = this.defaultPlaylist.itemList.push.apply(this.defaultPlaylist.itemList, itemsToAdd);
                    //this.defaultPlaylist.itemList.moveToIndex(newLength - playlistItems.length);
                    if (playFirst === true) {
                        this.engine.playItem(this.defaultPlaylist.itemList.current());
                    }
                }
                this.visible(true);
            },
            addMediaItems: function (mediaItems, playFirst) {
                /// <signature>
                /// <param name="playFirst" type="Boolean" optional="true" />
                /// <param name="mediaItems" type="Array" />
                /// </signature>
                var playlist = this.playlistList.current();
                if (playlist) {
                    playlist.itemList.addMediaItems(mediaItems)
                        .then(function () {
                            playlist.itemList.save().then(function () {
                                playlist.itemList.refresh();
                            });
                        });
                } else {
                    this.defaultPlaylist.itemList.addMediaItems(mediaItems);
                    if (playFirst === true) {
                        this.engine.playItem(this.defaultPlaylist.itemList.current());
                    }
                }
                this.visible(true);
            },
            addMediaLinks: function (mediaLinks) {
                /// <signature>
                /// <summary>Adds media links to the active playlist.</summary>
                /// <param name="mediaLinks" type="Array" />
                /// </signature>
                var playlist = this.playlistList.current();
                if (playlist) {
                    playlist.itemList.addMediaLinks(mediaLinks)
                        .then(function () {
                            playlist.itemList.save().then(function () {
                                playlist.itemList.refresh();
                            })
                        });
                } else {
                    this.defaultPlaylist.itemList.addMediaLinks(mediaLinks);
                }
                this.visible(true);
            },

            //
            // User Playlist
            //

            openPlaylistList: function () {
                /// <signature>
                /// <summary>Loads the playlists of the current user.</summary>
                /// </signature>
                if (_PI.Project.Security.identity() &&
                    this.playlistList.state() === _PI.EntityCollectionState.closed) {
                    this.playlistList.open();
                }
            },
            setPlaylist: function (playlist) {
                /// <signature>
                /// <param name="playlist" type="PI.Project.Search.Playlist" optional="true" />
                /// <returns type="$.Deferred" />
                /// </signature>
                if (playlist) {
                    this.playlistList.moveTo(playlist);
                    this.playlistUri = ko.unwrap(playlist.item.uri);
                    this.editMode = ko.unwrap(playlist.item.editMode);

                    var that = this;
                    var exists = playlist.exists();
                    var promise = exists
                        && (playlist.itemList.state() === _PI.EntityCollectionState.closed)
                        && (playlist.itemList.open());

                    return $.when(promise).always(function () {
                        that.engine.setView({
                            view: ns.ViewMode.playlist,
                            list: playlist.itemList
                        });
                        that.visible(true);
                    });
                } else {
                    this.playlistList.moveTo(null);
                    this.playlistUri = void 0;
                    this.editMode = ns.EditMode.editable;
                    this.engine.setView({ view: ns.ViewMode.medialist });
                    return _Promise.complete();
                }
            },
            setPlaylistById: function (id) {
                /// <signature>
                /// <param name="id" type="Number" />
                /// <returns type="$.Deferred" />
                /// </signature>
                /// <signature>
                /// <param name="id" type="String" />
                /// <returns type="$.Deferred" />
                /// </signature>
                var that = this;
                var playlist = that.playlistList.findById(id);
                if (playlist) {
                    return that.setPlaylist(playlist);
                }
                playlist = new ns.Playlist(this.engine, { autoReload: false });
                return playlist.loadByIdAsync(id, true).then(
                    function _setPlaylistByIdDone() {
                        that.playlistList.push.apply(that.playlistList, playlist);
                        return that.setPlaylist(playlist);
                    },
                    function _setPlaylistByIdFail() {
                        return that.setPlaylist(null);
                    });
            },

            //
            // Shared Playlist
            //

            loadDefaultByContactAsync: function (contactId, options) {
                /// <signature>
                /// <param name="contactId" type="Number" />
                /// <param name="options" type="Object" optional="true">
                ///     <para name="target" type="String" optional="true" />
                ///     <para name="setView" type="Boolean" optional="true" />
                ///     <para name="asDefault" type="Boolean" optional="true" />
                /// </param>
                /// <returns type="$.Deferred" />
                /// </signature>
                var that = this;
                var asDefault = options && options.asDefault === true;
                var setView = options && options.setView === true;
                var playlist = new ns.Playlist(this.engine, { autoReload: false });
                return playlist.loadDefaultByContactAsync(contactId, true).then(
                    function _loadDefaultByContactDone() {
                        that.sharedPlaylist(playlist);
                        asDefault && that.defaultPlaylist.itemList.replaceAll.apply(
                            that.defaultPlaylist.itemList,
                            playlist.itemList.toObject());
                        setView && that.engine.setView({
                            view: ns.ViewMode.playlist,
                            list: playlist.itemList
                        });
                    },
                    function _loadDefaultByContactFail() {
                        that.sharedPlaylist(null);
                    });
            },
            loadSharedByUriAsync: function (uri, options) {
                /// <signature>
                /// <param name="uri" type="String" />
                /// <param name="options" type="Object" optional="true">
                ///     <para name="target" type="String" optional="true" />
                ///     <para name="setView" type="Boolean" optional="true" />
                ///     <para name="asDefault" type="Boolean" optional="true" />
                /// </param>
                /// <returns type="$.Deferred" />
                /// </signature>
                var that = this;
                var asDefault = options && options.asDefault === true;
                var setView = options && options.setView === true;
                var playlist = new ns.Playlist(this.engine, { autoReload: false });
                return playlist.loadByUriAsync(uri, true).then(
                    function _loadSharedByUriDone() {
                        that.sharedPlaylist(playlist);
                        asDefault && that.defaultPlaylist.itemList.replaceAll.apply(
                            that.defaultPlaylist.itemList,
                            playlist.itemList.toObject());
                        setView && that.engine.setView({
                            view: ns.ViewMode.playlist,
                            list: playlist.itemList
                        });
                    },
                    function _loadSharedByUriFail() {
                        that.sharedPlaylist(null);
                    });
            },
            _sharedListItemChanged: function (event) {
                /// <signature>
                /// <summary>Raised when the current playlist changes.</summary>
                /// </signature>
                var playlist = event.detail.newItem;
                playlist && this.loadSharedByUriAsync(ko.unwrap(playlist.item.uri));
            },

            //
            // Shared Playlist Functions
            //

            resetAll: function () {
                /// <signature>
                /// <summary>Resets all playlists.</summary>
                /// </signature>
                this.defaultPlaylist.reset();
                this.playlistList.moveTo(null);
            },

            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                if (this._disposed) {
                    return;
                }
                this._sharedOrCurrent && this._sharedOrCurrent.dispose(), this._sharedOrCurrent = null;
                this._currentOrDefault && this._currentOrDefault.dispose(), this._currentOrDefault = null;
                this._defaultVisible && this._defaultVisible.dispose(), this._defaultVisible = null;
                this._disposed = true;
            }
        }),

        PlaylistItem: _Class.derive(_PI.Entity, function PlaylistItem_ctor(engine, options) {
            /// <signature>
            /// <summary>Represents a view that can be used to define entity operations.</summary>
            /// <param name="engine" type="PI.Project.Search.Engine" />
            /// <param name="options" type="Object" optional="true" />
            /// <returns type="PlaylistItem" />
            /// </signature>
            options = options || {};
            options.urls = options.urls || {
                query: "project/playlist-items/{id}",
                create: "project/playlist-items",
                update: "project/playlist-items/{id}",
                move: "project/playlist-items/{id}/{sortOrderId}",
                remove: "project/playlist-items/{id}"
            };
            options.mapping = options.mapping || {
                copy: ["id", "playlistId", "duration"],
                publishDate: {
                    create: function (options) {
                        return options.data ? new Date(options.data) : new Date();
                    }
                }
            };
            options.item = ns.PlaylistItem.createItem(options.item);

            this.engine = engine;
            this.isActive = _observable(false);
            this.durationParts = null;
            this.thumbnailUrl = null;

            _PI.Entity.call(this, options);
        }, {
            getAuthToken: function () {
                /// <signature>
                /// <returns type="String" />
                /// </signature>
                return _PI.Project.Security.getToken();
            },
            /// <field type="String">
            /// Gets the external video playler link for this video.
            /// </field>
            mediaLink: {
                get: function () {
                    var mediaType = ko.unwrap(this.item.mediaType);
                    var mediaId = ko.unwrap(this.item.mediaId);
                    if (mediaType === "dailymotion") {
                        return "http://www.dailymotion.com/video/" + mediaId;
                    }
                    return "https://www.youtube.com/watch?v=" + mediaId;
                }
            },
            mapItem: function (item) {
                /// <signature>
                /// <summary>Maps the current item to a new object. This method will be called from the base constructor.
                /// Unlike the setItem method, you can't access derived members from this method.</summary>
                /// <param name="item" type="Object">The native JS object to convert.</param>
                /// <param name="mapping" type="Object" optional="true">A set of key/value pairs that can be used to configure KO mapping options.</param>
                /// </signature>
                _PI.Entity.prototype.mapItem.call(this, item);
                if (!item) {
                    return;
                }
                if (item.duration) {
                    var hours = Math.floor(item.duration / 3600);
                    var mins = Math.floor((item.duration - hours * 3600) / 60);
                    var secs = item.duration - hours * 3600 - mins * 60;
                    this.durationParts = {
                        hours: padLeft(String(hours), 2, "0"),
                        mins: padLeft(String(mins), 2, "0"),
                        secs: padLeft(String(secs), 2, "0")
                    };
                }
                var provider = this.engine.providers.find(item.mediaType);
                if (provider) {
                    var service = provider.service();
                    this.thumbnailUrl = service.getThumbnail(item.mediaId);
                }
            },
            exists: function () {
                /// <signature>
                /// <summary>Returns true if the current item has been saved</summary>
                /// <returns type="Boolean" />
                /// </signature>
                var id = +ko.unwrap(this.item.id);
                return !isNaN(id) && (id > 0);
            },
            reset: function () {
                /// <signature>
                /// <summary>Resets the entity.</summary>
                /// </signature>
                this.setItem(ns.PlaylistItem.createItem());
                _PI.Entity.prototype.reset.call(this);
            },
            moveAsync: function (sortOrderId) {
                /// <signature>
                /// <summary>Moves the playlist item to the specified position.</summary>
                /// <param name="sortOrderId" type="Number">The new position.</param>
                /// <returns type="$.Deferred" />
                /// </signature>
                if (!this.exists()) {
                    return $.Deferred().reject().promise();
                }
                return _PI.api({
                    auth: this.getAuthToken(),
                    method: "move",
                    path: this.makeUrl("move", { id: ko.unwrap(this.item.id), sortOrderId: sortOrderId })
                });
            },
            toMediaItem: function () {
                /// <signature>
                /// <returns type="PI.Project.Search.MediaItem" />
                /// </signature>
                var obj = this.toObject();
                return new ns.MediaItem({
                    id: obj.mediaId,
                    type: obj.mediaType,
                    title: obj.name,
                    thumbnailUrl: obj.thumbnailUrl,
                    duration: obj.duration,
                    published: obj.publishDate,
                    updated: obj.updateDate
                });
            },
            toObject: function () {
                /// <signature>
                /// <returns type="Object" />
                /// </signature>
                return {
                    id: ko.unwrap(this.item.id),
                    playlistId: ko.unwrap(this.item.playlistId),
                    sortOrderId: ko.unwrap(this.item.sortOrderId),
                    name: ko.unwrap(this.item.name),
                    mediaType: ko.unwrap(this.item.mediaType),
                    mediaId: ko.unwrap(this.item.mediaId),
                    duration: ko.unwrap(this.item.duration) | 0,
                    publishDate: new Date(ko.unwrap(this.item.publishDate)).toISOString()
                };
            }
        }, {
            createItem: function (item) {
                /// <signature>
                /// <summary>Creates a native playlist item object.</summary>
                /// <param name="item" type="Object" optional="true">A native JS object to extend.</param>
                /// <returns type="Object" />
                /// </signature>
                item = item || {};
                return {
                    id: item.id | 0,
                    playlistId: item.playlistId | 0,
                    playlist: item.playlist,
                    sortOrderId: item.sortOrderId | 0,
                    name: item.name,
                    mediaType: item.mediaType,
                    mediaId: item.mediaId,
                    duration: item.duration | 0,
                    publishDate: item.publishDate || new Date().toISOString()
                };
            }
        }),

        PlaylistItemFilter: _Class.derive(_PI.EntityFilter, function PlaylistItemFilter_ctor(options) {
            /// <signature>
            /// <summary>Represents a view that can be used to define AJAX query parameters.</summary>
            /// <param name="options" type="Object" optional="true">A set of key/value pairs that contains filter parameters.</param>
            /// <returns type="PlaylistItemFilter" />
            /// </signature>
            options = options || {};
            this.playlistId = _observable(options.playlistId);
            this.name = _observable(options.name);
            _PI.EntityFilter.call(this, options);
        }),

        PlaylistItemList: _Class.derive(_PI.EntityCollection, function PlaylistItemList_ctor(engine, options) {
            /// <signature>
            /// <summary>Represents a view for grouping, sorting, filtering, and navigating a paged data collection.</summary>
            /// <param name="engine" type="PI.Project.Search.Engine" />
            /// <param name="options" type="Object" optional="true" />
            /// <returns type="PlaylistItemList" />
            /// </signature>
            options = options || {};
            options.urls = options.urls || {
                query: "project/playlists/{playlistId}/items",
                create: "project/playlists/{playlistId}/items"
            };
            options.filter = options.filter || new ns.PlaylistItemFilter();
            options.pageSize = options.pageSize || engine.playlistItemLimit;

            this.engine = engine;
            this.service = options.service;
            this.playlist = options.playlist;

            _PI.EntityCollection.call(this, options);
        }, {
            getAuthToken: function () {
                /// <signature>
                /// <returns type="String" />
                /// </signature>
                return _PI.Project.Security.getToken();
            },
            getService: function () {
                /// <signature>
                /// <returns type="PI.Project.Search.PlaylistService" />
                /// </signature>
                return this.service || new ns.PlaylistService(this.getAuthToken());
            },
            mapItem: function (item) {
                /// <signature>
                /// <param name="item" type="Object" />
                /// <returns type="Object" />
                /// </signature>
                return item instanceof ns.PlaylistItem
                    ? item
                    : new ns.PlaylistItem(this.engine, { item: item, mapping: null });
            },
            findById: function (id) {
                /// <signature>
                /// <param name="id" type="Number" />
                /// <returns type="PI.Project.Search.PlaylistItem" />
                /// </signature>
                /// <signature>
                /// <param name="id" type="String" />
                /// <returns type="PI.Project.Search.PlaylistItem" />
                /// </signature>
                return this.find(function (it) {
                    return ko.unwrap(it.item.id) == id;
                });
            },
            findByMedia: function (mediaType, mediaId) {
                /// <signature>
                /// <param name="mediaType" type="String" />
                /// <param name="mediaId" type="String" />
                /// <returns type="PI.Project.Search.PlaylistItem" />
                /// </signature>
                return this.find(function (it) {
                    return ko.unwrap(it.item.mediaType) === mediaType && ko.unwrap(it.item.mediaId) === mediaId;
                });
            },
            moveById: function (id, sortOrderId) {
                /// <signature>
                /// <summary>Moves an item to the given position</summary>
                /// <param name="id" type="Number" />
                /// <param name="sortOrderId" type="Number" />
                /// </signature>
                var that = this;
                var oldItem = null;
                var newItem = null;
                var oldIndex = -1;
                var newIndex = -1;
                this.some(function (playlistItem, i) {
                    if (ko.unwrap(playlistItem.item.id) === id) {
                        oldItem = playlistItem;
                        oldIndex = i;
                    }
                    if (ko.unwrap(playlistItem.item.sortOrderId) === sortOrderId) {
                        newItem = playlistItem;
                        newIndex = i;
                    }
                    return oldItem && newItem;
                });
                if (oldItem && newItem) {
                    oldItem.moveAsync(sortOrderId).then(function () {
                        that.refresh();
                    });
                }
            },
            moveItemTo: function (curIndex, newIndex) {
                /// <signature>
                /// <param name="curIndex" type="Number" />
                /// <param name="newIndex" type="Number" />
                /// </signature>
                var that = this;
                var curItem = this.getAt(curIndex);
                var newItem = this.getAt(newIndex);
                if (curItem && newItem) {
                    this.move(curIndex, newIndex);
                    if (curItem.exists()) {
                        curItem.moveAsync(ko.unwrap(newItem.item.sortOrderId))
                            .then(function () {
                                that.refresh();
                            });
                    }
                }
            },
            getMediaLinks: function () {
                /// <signature>
                /// <summary>Gets all the media items as an array of media links.</summary>
                /// <returns type="Array" />
                /// </signature>
                return this.items().map(function (plsIte) {
                    return plsIte.mediaLink;
                });
            },
            getNewItems: function () {
                /// <signature>
                /// <summary>Gets all the unsaved playlist items.</summary>
                /// <returns type="Array" />
                /// </signature>
                var filter = this.getFilter();
                var playlistId = +filter.playlistId;
                return this.items().filter(function (playlistItem) {
                    var id = +ko.unwrap(playlistItem.item.id);
                    if (isNaN(id) || id > 0) {
                        return false;
                    }
                    playlistItem.item.playlistId(playlistId);
                    return true;
                });
            },
            addMediaItems: function (mediaItems) {
                /// <signature>
                /// <param name="mediaItems" type="Array" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                var that = this;
                var filter = that.getFilter();
                return _Promise(function (completeDispatch) {
                    var playlistId = +filter.playlistId;
                    that.unshift.apply(that, mediaItems.map(function (mediaItem) {
                        var playlistItem = mediaItem.toPlaylistItem();
                        playlistItem.playlistId = playlistId;
                        return playlistItem;
                    }));
                    completeDispatch();
                });
            },
            addMediaLinks: function (mediaLinks) {
                /// <signature>
                /// <param name="mediaLinks" type="Array" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                var chunkSize = 50;
                if (mediaLinks.length > chunkSize) {
                    var that = this;
                    var chunks = [];
                    for (var i = 0, n = mediaLinks.length; i < n;) {
                        chunks.push(mediaLinks.slice(i, i += chunkSize));
                    }
                    return _Promise.join(chunks.map(function (mediaLinks) {
                        return that._addMediaLinks(mediaLinks);
                    }));
                }
                return this._addMediaLinks(mediaLinks);
            },
            detachItems: function () {
                /// <signature>
                /// <summary>Sets null for all playlist properties in the list.</summary>
                /// </signature>
                this.forEach(function (item) {
                    item.playlistId(null);
                });
            },
            deleteItem: function (item) {
                /// <signature>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.EntityCollection.prototype[item && item.exists() ? "deleteItem" : "remove"].call(this, item);
            },
            loadByContact: function (contactId) {
                /// <signature>
                /// <param name="contactId" type="Number" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                var that = this;
                var filter = this.getFilter() || {};
                if (this.supportsPaging) {
                    filter.page = this.pageIndex || 1;
                    filter.count = this.pageSize || _PI.options.pageSize;
                }
                this.state(_PI.EntityCollectionState.loading);
                return this.engine.authorize().then(function () {
                    return that.getService().getAllItemsByContact(contactId, filter).then(
                        function (response) {
                            that.afterOpen(response);
                        },
                        function () {
                            that.state(_PI.EntityCollectionState.closed);
                            that.removeAll();
                            that.total = 0;
                        });
                });
            },
            save: function () {
                /// <signature>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                var that = this;
                return this.engine.authorize().then(function () {
                    var filter = that.getFilter();
                    var playlistId = +ko.unwrap(filter.playlistId);
                    if (isNaN(playlistId) || playlistId <= 0) {
                        return _Promise.error("playlistId: " + playlistId);
                    }
                    var playlistItems = that.getNewItems().map(function (plsIte) {
                        var obj = plsIte.toObject();
                        return {
                            name: obj.name,
                            mediaType: obj.mediaType,
                            mediaId: obj.mediaId,
                            duration: obj.duration,
                            publishDate: obj.publishDate
                        };
                    });
                    return that.getService().saveAllItemsAsync(playlistId, playlistItems);
                });
            },
            _addMediaLinks: function (mediaLinks) {
                /// <signature>
                /// <param name="mediaLinks" type="Array" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                var that = this;
                return _Promise(function (completeDispatch, errorDispatch) {
                    var filter = that.getFilter();
                    var playlistId = +filter.playlistId;
                    var mediaProvs = {};
                    var mediaProvExists = false;

                    // First of all, categorize the specified links by provider types.
                    mediaLinks.forEach(function (mediaLink) {
                        var media = mediaLink && _Utilities.parseMediaLink(mediaLink);
                        if (media) {
                            mediaProvs[media.type] = mediaProvs[media.type] || [];
                            mediaProvs[media.type].push(media);
                        }
                    });

                    ko.utils.objectForEach(mediaProvs, function (mediaType) {
                        mediaProvExists = true;
                        var mediaInfos = mediaProvs[mediaType];
                        var provider = that.engine.providers.find(mediaType);
                        var service = provider && provider.service();
                        if (service && typeof service.getVideoItems === "function") {
                            service.getVideoItems(mediaInfos.map(function (m) { return m.id; }))
                                .then(function (data) {
                                    data.items.forEach(function (mediaItem) {
                                        that.push(ns.PlaylistItem.createItem({
                                            playlistId: playlistId,
                                            mediaType: mediaType,
                                            mediaId: mediaItem.id,
                                            name: mediaItem.title,
                                            duration: mediaItem.duration,
                                            publishDate: mediaItem.published
                                        }));
                                    });
                                    completeDispatch();
                                },
                                errorDispatch);
                        } else {
                            mediaInfos.forEach(function (mediaInfo) {
                                that.push(ns.PlaylistItem.createItem({
                                    playlistId: playlistId,
                                    mediaType: mediaInfo.type,
                                    mediaId: mediaInfo.id,
                                    name: null,
                                    duration: 0,
                                    publishDate: Date.now()
                                }));
                            });
                            completeDispatch();
                        }
                    });

                    if (!mediaProvExists) {
                        errorDispatch();
                    }
                });
            }
        }),

        Playlist: _Class.derive(_PI.Entity, function Playlist_ctor(engine, options) {
            /// <signature>
            /// <param name="engine" type="PI.Project.Search.Engine" />
            /// <param name="options" type="Object" optional="true" />
            /// <returns type="Playlist" />
            /// </signature>
            options = options || {};
            options.urls = options.urls || {
                query: "project/playlists/{id}",
                create: "project/playlists",
                update: "project/playlists/{id}",
                remove: "project/playlists/{id}"
            };
            options.mapping = options.mapping || { modifiedDate: { create: function (options) { return options.data && new Date(options.data); } } };
            options.item = ns.Playlist.createItem(options.item);

            this._disposed = false;
            this.engine = engine;
            this.service = options.service;
            this.itemList = options.itemList || new ns.PlaylistItemList(this.engine, {
                autoOpen: false,
                autoSave: false,
                playlist: this,
                filter: new ns.PlaylistItemFilter({ playlistId: options.item.id })
            });

            _PI.Entity.call(this, options);

            this.item.name.extend({ required: true, maxLength: 256 });
        }, {
            /// <field type="Boolean">
            /// Returns true if this is the default playlist.
            /// </field>
            isDefault: {
                get: function () {
                    return this.item.defaultList() || (this.item.uri() === this.engine.playlistUri);
                }
            },
            getAuthToken: function () {
                /// <signature>
                /// <returns type="String" />
                /// </signature>
                return _PI.Project.Security.getToken();
            },
            getService: function () {
                /// <signature>
                /// <returns type="PI.Project.Search.PlaylistService" />
                /// </signature>
                return this.service || new ns.PlaylistService(this.getAuthToken());
            },
            equals: function (playlist) {
                /// <signature>
                /// <param name="playlist" type="PI.Project.Search.Playlist" />
                /// <returns type="Boolean" />
                /// </signature>                
                return playlist && (+ko.unwrap(this.item.id) === +ko.unwrap(playlist.item.id));
            },
            exists: function () {
                /// <signature>
                /// <returns type="Boolean" />
                /// </signature>
                var id = +ko.unwrap(this.item.id);
                return !isNaN(id) && (id > 0);
            },
            setItem: function (item) {
                /// <signature>
                /// <param name="item" type="Object" />
                /// </signature>
                _PI.Entity.prototype.setItem.call(this, item);
                this.itemList.filter.deferRefresh(function () {
                    this.setValue("playlistId", item && item.id);
                }, true);
            },
            loadByIdAsync: function (id, includeItems) {
                /// <signature>
                /// <param name="id" type="Number" />
                /// <param name="includeItems" type="Boolean" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                this.reset();
                if (!id) {
                    return _Promise.error();
                }

                var service = this.getService();
                return service.getByIdAsync(id, this).then(function (playlist) {
                    this.setItem(playlist);
                    if (includeItems !== false) {
                        return this.loadItemListAsync(playlist.id);
                    }
                });
            },
            loadByUriAsync: function (uri, includeItems) {
                /// <signature>
                /// <param name="uri" type="String" />
                /// <param name="includeItems" type="Boolean" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                this.reset();
                if (!uri) {
                    return _Promise.error();
                }

                var service = this.getService();
                return service.getByUriAsync(uri, this).then(function (playlist) {
                    this.setItem(playlist);
                    if (includeItems !== false) {
                        return this.loadItemListAsync(playlist.id);
                    }
                });
            },
            loadDefaultByContactAsync: function (contactId, includeItems) {
                /// <signature>
                /// <param name="contactId" type="Number" />
                /// <param name="includeItems" type="Boolean" optional="true" />
                /// </signature>
                this.reset();
                if (!contactId) {
                    return _Promise.error();
                }

                var service = this.getService();
                return service.getDefaultByContactAsync(contactId, this).then(function (playlist) {
                    this.setItem(playlist);
                    if (includeItems !== false) {
                        return this.loadItemListAsync(playlist.id);
                    }
                });
            },
            loadItemListAsync: function (playlistId) {
                /// <signature>
                /// <param name="playlistId" type="Number" />
                /// <returns type="$.Deferred" />
                /// </signature>
                var id = +playlistId;
                if (isNaN(id) || id <= 0) {
                    return _Promise.error("playlistId: " + playlistId);
                }
                this.itemList.close();
                this.itemList.filter.deferRefresh(function () {
                    this.clear();
                    this.setValue("playlistId", id);
                }, true);
                return this.itemList.open();
            },
            reset: function () {
                /// <signature>
                /// <summary>Removes all playlist items and sets an empty playlist.</summary>
                /// </signature>
                this.itemList.removeAll();
                this.setItem(ns.Playlist.createItem());
                _PI.Entity.prototype.reset.call(this);
            },
            save: function (saveItemList, saveAsDefault) {
                /// <signature>
                /// <param name="saveItemList" type="Boolean" optional="true" />
                /// <param name="saveAsDefault" type="Boolean" optional="true" />
                /// <returns type="$.Deferred" />
                /// </signature>
                var that = this;
                return this.engine.authorize().then(function () {
                    var service = that.getService();
                    var currPls = that.engine.playlist.playlistList.current();
                    var currPlsObj = currPls && currPls.toObject();
                    var thisPlsObj = that.toObject();

                    if (saveAsDefault === true) {
                        thisPlsObj.defaultList = true;
                    }

                    return (that.exists() ? service.updateAsync(thisPlsObj.id, thisPlsObj) : service.createAsync(thisPlsObj)).then(
                        function (playlist) {
                            that.setItem(playlist);

                            // If the selected playlist equals to the current playlist,
                            // update the playlist's editMode URI without having to reload
                            // this playlist of the user.
                            if (currPlsObj && playlist && (currPlsObj.id === playlist.id)) {
                                that.engine.playlist.editMode = playlist.editMode;
                            }

                            // Set the playlistId as playlist items filter.
                            that.itemList.filter.setValue("playlistId", playlist.id);

                            // Save the playlist items too.
                            return saveItemList ? that.itemList.save() : _Promise.complete();
                        }).then(function () {
                            // Log this operation too.
                            that.engine.logging.logAction(
                                "save.playlist." + thisPlsObj.name + "-" + that.itemList.items().length + "db");

                            that.dispatchEvent("saved");
                        });
                });
            },
            setAsDefault: function () {
                /// <signature>
                /// <returns type="$.Deferred" />
                /// </signature>
                var that = this;
                var plsData = this.toObject();
                return this.engine.authorize().then(function () {
                    var service = that.getService();
                    return service.setAsDefaultAsync(plsData.id)
                        .then(function () {
                            plsData.defaultList = true;
                            that.setItem(plsData);
                            that.dispatchEvent("saved");
                        });
                });
            },
            toggleEditModeAndSave: function () {
                /// <signature>
                /// <returns type="$.Deferred" />
                /// </signature>
                var editMode = this.item.editMode();
                this.item.editMode((editMode === ns.EditMode.readonly) ? ns.EditMode.editable : ns.EditMode.readonly);
                return this.save(false);
            }
        }, {
            createItem: function (item) {
                /// <signature>
                /// <param name="item" type="Object" optional="true" />
                /// <returns type="Object" />
                /// </signature>
                item = item || {};
                return {
                    id: item.id | 0,
                    uri: item.uri,
                    name: item.name || ("Playlist-" + _Resources.format(new Date(), "yyyyMMdd-HHmm")),
                    defaultList: !!item.defaultList,
                    editMode: item.editMode || ns.EditMode.readonly,
                    itemCount: item.itemCount | 0,
                    modifiedDate: item.modifiedDate || new Date().toISOString()
                };
            }
        }),

        PlaylistFilter: _Class.derive(_PI.EntityFilter, function PlaylistFilter_ctor(options) {
            /// <signature>
            /// <param name="options" type="Object" optional="true" />
            /// <returns type="PlaylistFilter" />
            /// </signature>
            options = options || {};
            _PI.EntityFilter.call(this, options);
            this.name = _observable(options.name);
        }),

        PlaylistList: _Class.derive(_PI.EntityCollection, function PlaylistList_ctor(engine, options) {
            /// <signature>
            /// <param name="engine" type="PI.Project.Search.Engine" />
            /// <param name="options" type="Object" optional="true" />
            /// <returns type="PlaylistList" />
            /// </signature>
            options = options || {};
            options.urls = options.urls || { query: "project/playlists" };
            options.filter = options.filter || new ns.PlaylistFilter();
            options.pageSize = options.pageSize || engine.playlistLimit;

            this.loaded = false;
            this.engine = engine;

            _PI.EntityCollection.call(this, options);
        }, {
            getAuthToken: function () {
                /// <signature>
                /// <returns type="String" />
                /// </signature>
                return _PI.Project.Security.getToken();
            },
            mapItem: function (item) {
                /// <signature>
                /// <param name="item" type="Object" />
                /// <returns type="Object" />
                /// </signature>
                return item instanceof ns.Playlist
                    ? item
                    : new ns.Playlist(this.engine, { item: item /*, mapping: null*/ });
            },
            open: function () {
                /// <signature>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                if (this.loaded) {
                    return _Promise.complete();
                }
                var that = this;
                return _PI.EntityCollection.prototype.open.call(this)
                    .then(function () {
                        that.loaded = true;
                    });
            },
            setAsDefault: function (playlist) {
                /// <signature>
                /// <param name="playlist" type="PI.Project.Search.Playlist" />
                /// <returns type="$.Deferred" />
                /// </signature>
                var that = this;
                playlist.setAsDefault().then(function () {
                    that.forEach(function (it) {
                        if (it !== playlist) {
                            it.item.defaultList(false);
                        }
                    });
                });
            },
            findById: function (id) {
                /// <signature>
                /// <param name="id" type="Number" />
                /// <returns type="PI.Project.Search.PlaylistItem" />
                /// </signature>
                /// <signature>
                /// <param name="id" type="String" />
                /// <returns type="PI.Project.Search.PlaylistItem" />
                /// </signature>
                return this.find(function (it) {
                    return ko.unwrap(it.item.id) == id;
                });
            }
        })

    });

    _Class.mix(ns.Playlist, _Utilities.createEventProperties("saved"));

})(ko, WinJS, PI);
