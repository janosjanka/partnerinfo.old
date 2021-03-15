// Copyright (c) Partnerinfo Ltd. All Rights Reserved.
/// <reference path="../engine.js" />

(function (ko, _WinJS, PI, undefined) {
    "use strict";

    var _Class = _WinJS.Class;
    var _Knockout = _WinJS.Knockout;

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

    var ns = _WinJS.Namespace.defineWithParent(PI, "Project.Search", {

        /// <field>
        /// URL hash parameters
        /// </field>
        HashParam: {
            providerType: "type",  // provider's type ( youtube, dailymotion, etc. )
            mediaItemId: "media",  // media to play
            playlistUri: "list",   // human-readable identitifier of a playlist
            contactId: "cid",      // Contact ID
            sponsorId: "sid",      // Sponsor ID
            editMode: "mode"       // 0 = view, 1 = editable
        },

        /// <field>
        /// Represents a view
        /// </field>
        ViewMode: {
            medialist: 1,
            playlist: 2
        },

        /// <field>
        /// Edit mode.
        /// </field>
        EditMode: {
            readonly: "readonly",
            editable: "editable"
        },

        /// <field>
        /// Allows a user to choose a play mode
        /// </field>
        PlayMode: {
            normal: "normal",
            repeatOne: "repeatOne",
            repeatAll: "repeatAll"
        },

        Provider: _Class.define(function Provider_ctor(type, name, options) {
            /// <signature>
            /// <param name="type" type="String" />
            /// <param name="name" type="String" />
            /// <param name="options" type="Object" />
            /// <returns type="PI.Project.Search.Provider" />
            /// </signature>
            this.promise = null;
            this.type = type;
            this.name = name;
            this.list = null;
            this.view = options.view || null;
            this.logo = options.logo || null;
            this.require = options.require || [];
            this.service = options.service || _WinJS.noop;
            this.filter = options.filter || _WinJS.noop;
            this.factory = options.factory || _WinJS.noop;
        }, {
            loadAsync: function () {
                /// <signature>
                /// <summary>Loads all provider dependencies without calling the factory method</summary>
                /// <returns type="$.Deferred" />
                /// </signature>
                if (this.promise === null) {
                    this.promise = require(this.require);
                }
                return this.promise;
            },
            initAsync: function (engine, options) {
                /// <signature>
                /// <summary>Initializes this provider calling the factory method</summary>
                /// <returns type="$.Deferred" />
                /// </signature>
                var that = this;
                return this.loadAsync().then(
                    function _initAsyncDone() { that.list = that.factory(engine, options); },
                    function _initAsyncFail() { that.list = null; });
            }
        }, {
            createYouTube: function (googleApiKey) {
                return new ns.Provider("youtube", "YouTube", {
                    logo: "i isearch youtube",
                    view: "koYouTubeList",
                    require: ["https://www.youtube.com/player_api", "search/youtube.html"],
                    service: function () { return new ns.YouTubeService(googleApiKey); },
                    filter: function (engine) { return new ns.YouTubeFilter({ query: engine.searchQuery }); },
                    factory: function (engine, options) {
                        options = options || {};
                        options.service = this.service(engine);
                        options.filter = this.filter(engine);
                        return new ns.YouTubeList(engine, options);
                    }
                });
            },
            createDailymotion: function (dailymotionApiKey) {
                return new ns.Provider("dailymotion", "Dailymotion", {
                    logo: "i isearch dailymotion",
                    view: "koDailymotionList",
                    require: ["https://api.dmcdn.net/all.js", "search/dailymotion.html"],
                    service: function () { return new ns.DailymotionService(dailymotionApiKey); },
                    filter: function (engine) { return new ns.DailymotionFilter({ query: engine.searchQuery }); },
                    factory: function (engine, options) {
                        options = options || {};
                        options.service = this.service(engine);
                        options.filter = this.filter(engine);
                        return new ns.DailymotionList(engine, options);
                    }
                });
            }
        }),

        ProviderList: _Class.define(function ProviderList_ctor(engine) {
            /// <signature>
            /// <summary>Initializes a new instance of the ProviderList class.</summary>
            /// <param name="engine" type="PI.Project.Search.Engine" />
            /// <returns type="PI.Project.Search.ProviderList" />
            /// </signature>
            this.engine = engine;
            this._cache = {};
            this.items = [];
        }, {
            register: function (provider) {
                /// <signature>
                /// <summary>Registers a provider</summary>
                /// <param name="provider" type="PI.Project.Search.Provider" />
                /// </signature>
                this.items.push(provider);
                this._cache[provider.type] = provider;
            },
            find: function (type) {
                /// <signature>
                /// <summary>Searches for the provider with the specified type.</summary>
                /// <param name="name" type="String" />
                /// <returns type="PI.Project.Search.Provider" />
                /// </signature>
                var provider = this._cache[type];
                if (typeof provider === "undefined") {
                    for (var i = this.items.length; --i >= 0;) {
                        if (this.items[i].type === type) {
                            this._cache[type] = provider = this.items[i];
                            break;
                        }
                    }
                }
                return provider;
            },
            loadByNameAsync: function (type) {
                /// <signature>
                /// <summary>Finds a provider by name and loads that</summary>
                /// <param name="type" type="String" />
                /// <returns type="$.Deferred" />
                /// </signature>
                var provider = this.find(type);
                if (provider) {
                    return provider.loadAsync();
                }
                return $.Deferred().reject().promise();
            }
        })

    });

    //
    // MediaItem defines an interface for a media type (video, sound, html, etc.)
    // that can be played by a user. When a user creates a playlist,
    // he/she can save a collection of media item objects to the database.
    //

    var MediaItem = _Class.define(function MediaItem_ctor(options) {
        /// <signature>
        /// <summary>Initializes a new instance of the MediaItem class.</summary>
        /// <param name="options" type="Object">The set of options to be applied initially to the MediaItem.</param>
        /// <returns type="MediaItem" />
        /// </signature>        
        this.service = options.service;

        this.id = options.id;
        this.type = options.type;
        this.author = options.author;
        this.title = options.title;
        this.description = options.description;
        this.descriptionHtml = ko.observable();
        this.duration = options.duration || 0;
        this.durationParts = null;
        this.viewCount = options.viewCount;
        this.thumbnailUrl = options.thumbnailUrl;
        this.updated = options.updated;
        this.published = options.published;
        this.isActive = ko.observable(false);
        this.isLoaded = ko.observable(false);

        this.update();
    }, {
        load: function () {
            /// <signature>
            /// <returns type="WinJS.Promise" />
            /// </signature>
            return this.service.getVideoItem(this.id, this).then(
                function (videoItem) {
                    this.patch(videoItem);
                    this.isLoaded(true);
                },
                function () {
                    this.isLoaded(false);
                });
        },
        update: function () {
            /// <signature>
            /// <summary>Updates all computed values.</summary>
            /// </signature>
            if (this.duration) {
                var hours = Math.floor(this.duration / 3600);
                var mins = Math.floor((this.duration - hours * 3600) / 60);
                var secs = this.duration - hours * 3600 - mins * 60;
                this.durationParts = {
                    hours: padLeft(String(hours), 2, "0"),
                    mins: padLeft(String(mins), 2, "0"),
                    secs: padLeft(String(secs), 2, "0")
                };
            }
            this.descriptionHtml(this.description && this.description.replace(/(\r\n|\n|\r)/gm, "<br />") || "");
        },
        patch: function (item) {
            /// <signature>
            /// <param name="item" type="Object" />
            /// </signature>
            item = item || {};
            this.id = item.id;
            this.channelId = item.channelId;
            this.title = item.title;
            this.description = item.description;
            this.duration = item.duration;
            this.thumbnailUrl = item.thumbnailUrl;
            this.published = item.published;
            this.updated = item.updated;
            this.update();
        },
        toPlaylistItem: function () {
            /// <signature>
            /// <summary>Maps the current media item to a playlist item.</summary>
            /// <returns type="Object" />
            /// </signature>
            return PI.Project.Search.PlaylistItem.createItem({
                name: this.title,
                mediaType: this.type,
                mediaId: this.id,
                duration: this.duration,
                publishDate: this.published
            });
        }
    }),

    MediaFilter = _Class.derive(_Knockout.Filter, function MediaFilter_ctor(options) {
        /// <signature>
        /// <summary>Initializes a new instance of the MediaFilter class.</summary>
        /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the MediaFilter.</param>
        /// <returns type="MediaFilter" />
        /// </signature>
        options = options || {};

        this.defaultQuery = options.defaultQuery || "Teljes film";
        this.query = ko.observable(options.query);
        this.language = ko.observable(options.language);

        _Knockout.Filter.apply(this, [options]);
    }, {
        toObject: function () {
            /// <signature>
            /// <summary>Converts the current filter properties to a native object.</summary>
            /// <returns type="Object" />
            /// </signature>
            var filter = _Knockout.Filter.prototype.toObject.apply(this, arguments);
            filter.query = filter.query || filter.defaultQuery || undefined;
            filter.language = filter.language || undefined;
            return filter;
        }
    }),

    MediaList = _Class.derive(_Knockout.PagedList, function MediaList_ctor(engine, options) {
        /// <signature>
        /// <summary>Represents a view for grouping, sorting, filtering, and navigating a paged data collection.</summary>
        /// <param name="options" type="Object" optional="true">A set of key/value pairs.</param>
        /// <returns type="MediaList" />
        /// </signature>
        options = options || {};
        options.autoOpen = options.autoOpen !== false;
        options.autoClean = options.autoClean === true;

        this.engine = engine;
        this.service = options.service;
        this.filter = options.filter || new MediaFilter();
        this.filter.changed = this.refresh.bind(this, true);

        _Knockout.PagedList.apply(this, [options]);
        options.autoOpen && this.refresh(true);
    }, {
        //
        // Public Event Handlers
        //

        onListViewItemClick: function (event) {
            /// <signature>
            /// <param name="event" type="MouseEvent" />
            /// </signature>
            var eventDetail = event.detail;
            if (eventDetail.shiftKey || eventDetail.ctrlKey) {
                return;
            }
            event.preventDefault();

            /// <var type="MediaList" />
            var mediaList = eventDetail.list;
            /// <var type="MediaItem" />
            var mediaItem = eventDetail.item;

            // We just support playing in readonly mode.
            if (!mediaList.engine.canEdit) {
                mediaList.engine.playMedia(mediaItem);
                return;
            }

            // Display the playlist module when a user clicks a media item.
            mediaList.engine.playlist.visible(true);

            // In edit mode, we just move the clicked media item
            // to the current playlist.
            var $mediaListGroup = $(eventDetail.group);
            var $mediaPart = $mediaListGroup.parents(".wsProvMediaPart:first");
            var mediaListGroupCoord = $mediaListGroup.offset();
            var playListCoord = $(".wsPlaylist:first", $mediaPart).offset();

            // Do some animation to be modern.
            $mediaListGroup.animate({
                left: playListCoord.left - mediaListGroupCoord.left,
                top: playListCoord.top - mediaListGroupCoord.top,
                opacity: 0
            }, 500, function () {
                // Move the cursor to the clicked media item and add it to the current playlist.
                mediaList.moveTo(mediaItem);
                mediaList.selection.removeAll();
                mediaList.engine.playlist.addMediaItems([mediaItem]);

                // Remove all KO bindings and remove the group node from the media list.
                ko.cleanNode(eventDetail.group);
                $mediaListGroup.remove();

                // Play the video without loading the playlist into the left hand-side of the list.
                mediaList.engine.playMedia(mediaItem, false);
            });
        },

        //
        // Public Functions
        //

        mapItem: function (item) {
            /// <signature>
            /// <param name="item" type="Object" />
            /// <returns type="PI.Project.Search.MediaItem" />
            /// </signature>
            return new MediaItem(item);
        },
        moveToPage: function () {
            /// <signature>
            /// <param name="pageIndex" type="Number" />
            /// <returns type="Boolean" />
            /// </signature>
            _Knockout.Filter.prototype.moveToPage.apply(this, arguments);
            if (this.refresh) {
                this.refresh(false);
            }
        },
        refresh: function (clearItems) {
            /// <signature>
            /// <summary>Refreshes the list.</summary>
            /// </signature>
            if (clearItems === true || this.options.autoClean === true) {
                this.removeAll();
            }
        },
        reset: function () {
            /// <signature>
            /// <summary>Resets the list.</summary>
            /// </signature>
            this.removeAll();
            this.pageIndex = 1;
        },

        //
        // Playlist operations
        //

        addToPlaylist: function () {
            /// <signature>
            /// <summary>Adds all selected items to the current playlist.</summary>
            /// </signature>
            var selection = this.selection();
            if (selection.length) {
                this.engine.playlist.addMediaItems(selection);
                this.deselectAll();
            }
        }
    });

    //
    // Public Namespaces & Classes
    //

    _WinJS.Namespace.defineWithParent(PI, "Project.Search", {
        MediaItem: MediaItem,
        MediaFilter: MediaFilter,
        MediaList: MediaList
    });

})(ko, WinJS, PI);