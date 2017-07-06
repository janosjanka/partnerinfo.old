// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="types.js" />
/// <reference path="modules/login.js" />
/// <reference path="modules/banner.js" />
/// <reference path="modules/media.js" />
/// <reference path="modules/playlist.js" />
/// <reference path="modules/logging.js" />

(function (ko, _WinJS, _PI, undefined) {
    "use strict";

    var FACEBOOK_APPID = _WinJS.DEBUG ? "1747009292247294" : "1743049802643243";
    var GOOGLE_APIKEY = "AIzaSyA9bp--mb8aVCGq9iAlwrEuD9dshvhTcqw";

    var _Class = _WinJS.Class;
    var _Utilities = _WinJS.Utilities;
    var _Promise = _WinJS.Promise;

    var _observable = ko.observable;
    var _pureComputed = ko.pureComputed;

    function loadFacebookSDK(culture, appId) {
        var id = "facebook-jssdk";
        var fjs = document.getElementsByTagName("script")[0];
        if (document.getElementById(id)) {
            return;
        }
        var script = document.createElement("script");
        script.id = id;
        script.src = "//connect.facebook.net/" + culture + "/sdk.js#xfbml=1&version=v2.6&appId=" + appId;
        fjs.parentNode.insertBefore(script, fjs);
    }

    var ns = _WinJS.Namespace.defineWithParent(_PI, "Project.Search", {

        /// <field>
        /// Exposes search services, providers, and manages communication among modules
        /// </field>
        Engine: _Class.define(function Engine_ctor(options) {
            /// <signature>
            /// <summary>Represents a view that defines search providers.</summary>
            /// <param name="options" type="Object" optional="true">A set of key/value pairs that can be used to configure search settings.</param>
            /// <returns type="PI.Project.Search.Engine" />
            /// </signature>
            options = options || {};

            _Utilities.setOptions(this, options);

            this._disposed = false;

            this.workflow = options.workflow || _PI.Project.Workflow;
            this.user = options.user || _PI.Project.Security;                                             // Current user
            this.elementId = options.elementId || "koSearchMediaPlayerContent";                           // Media player element ID
            this.facebookAppId = options.facebookAppId || FACEBOOK_APPID;                                 // Facebook APP ID
            this.googleApiKey = options.googleApiKey || GOOGLE_APIKEY;                                    // Google API KEY
            this.editMode = _observable(options.editMode || ns.EditMode.readonly);                        // Can edit the playlist
            this.editModeSn = this.editMode.subscribe(this._notifyEditModeChanged, this);
            this.autoHideMenu = !!options.autoHideMenu;                                                   // Is the menu hidden by default
            this.menuVisible = _pureComputed(this.isMenuVisible, this).extend({ rateLimit: 50 });
            this.logActionUrl = options.logActionUrl;
            this.loginActionUrl = options.loginActionUrl;                                                 // HTTP POST URL
            this.loginVisible = this.loginActionUrl && (options.loginVisible !== false);                  // Is the login button visible
            this.bannerEnabled = options.bannerEnabled !== false;                                         // Is the banner module enabled
            this._providerVisible = options.providerVisible !== false;                                    // Is the provider list visible
            this.searchVisible = options.searchVisible !== false;                                         // Is the search box visible
            this.searchQuery = options.searchQuery;                                                       // Default search query
            this.filterVisible = options.filterVisible !== false;                                         // Is the filter visible
            this.pagerVisible = options.pagerVisible !== false;                                           // Is the pager visible

            this.playlistEnabled = _observable(options.playlistEnabled !== false);                        // Is the playlist module enabled
            this.playlistVisible = this.playlistEnabled() && (!!options.playlistVisible);                 // playlist visibility ( true | false )
            this.playlistUri = options.playlistUri;                                                       // it will be played at startup
            this.playlistContact = options.playlistContact;                                               // it will be played at startup
            this.playlistMode = options.playlistMode;                                                     // play mode ( normal | repeatOne | repeatAll )
            this.playlistLimit = options.playlistLimit || 48;                                             // Maximum page size for playlists
            this.playlistItemLimit = options.playlistItemLimit || 48;                                     // Maximum page size for playlist items
            this.playlistDropImgUrl = options.playlistDropImgUrl || _PI.Server.mapPath("/ss/search/basket.png");
            this.playlistDropImgVisible = _pureComputed(this.isPlaylistDropImgVisible, this).extend({ rateLimit: 50 });

            this.hashPrefix = options.hashPrefix || "search";                                            // URL hash param prefix (search-type, search-media, etc.)

            this.view = _observable(ns.ViewMode.medialist);

            this.providers = new ns.ProviderList(this);
            this.providers.register(ns.Provider.createYouTube(this.googleApiKey));
            this.providers.register(ns.Provider.createDailymotion());
            this.provider = _observable();
            this.onProviderItemClick = this._provItemClick.bind(this);

            this.login = new ns.LoginModule(this);
            this.banner = new ns.BannerModule(this);
            this.media = new ns.MediaModule(this, {
                elementId: this.elementId,
                onplay: this._mediaPlay.bind(this),
                onstop: this._mediaStop.bind(this),
                onstatechanged: this._mediaStateChanged.bind(this)
            });
            this.playlist = new ns.PlaylistModule(this);
            this.logging = new ns.LoggingModule(this);

            this.initialize();
        }, {
            /// <field type="Boolean">
            /// Returns true if a user has already authenticated; otherwise false.
            /// </field>
            authenticated: {
                get: function () {
                    return this.user ? !!this.user.identity() : false;
                }
            },

            /// <field type="Boolean">
            /// Gets or sets the Sponsor ID.
            /// </field>
            sponsorId: {
                get: function () {
                    return this.getHashParam(ns.HashParam.sponsorId);
                },
                set: function (value) {
                    this.setHashParam(ns.HashParam.sponsorId, value);
                }
            },

            /// <field type="Boolean">
            /// Returns true if the provider menu is visible; otherwise false.
            /// </field>
            providerVisible: {
                get: function () {
                    return this.authenticated && this._providerVisible;
                }
            },

            /// <field type="Boolean">
            /// Returns a value whether the playlist can be edited or is readonly.
            /// </field>
            canEdit: {
                get: function () {
                    return this.editMode() !== ns.EditMode.readonly;
                }
            },

            //
            // Event Handlers
            //

            _notifyEditModeChanged: function (value) {
                /// <signature>
                /// <param name="value" type="PI.Project.Search.EditMode" />
                /// </signature>
                this.setHashParam(ns.HashParam.editMode, value);

                if (value === ns.EditMode.readonly) {
                    this.playlist.visible(false);
                }
            },

            // UI

            initialize: function () {
                /// <signature>
                /// <summary>Initializes instance members.</summary>
                /// </signature>
                loadFacebookSDK("hu_HU", this.facebookAppId);

                var that = this;
                var typeParam = this.getHashParam(ns.HashParam.providerType);
                var mediaParam = this.getHashParam(ns.HashParam.mediaItemId);
                var listParam = this.getHashParam(ns.HashParam.playlistUri) || this.playlistUri;
                var contactIdParam = this.getHashParam(ns.HashParam.contactId) || (this.playlistContact ? this.playlistContact.id : null);
                var editModeParam = this.getHashParam(ns.HashParam.editMode) || this.editMode.peek();

                var complete = function () {
                    var playlist = that.playlist.sharedPlaylist();
                    if (playlist) {
                        if (mediaParam) {
                            that.playByQuery(mediaParam);
                        } else {
                            playlist.itemList.moveToFirst();
                            //if (!_WinJS.DEBUG) {
                            var firstPlaylistItem = playlist.itemList.items()[0];
                            firstPlaylistItem && that.playItem(firstPlaylistItem);
                            //}
                        }
                        that.setView({
                            view: ns.ViewMode.playlist,
                            list: playlist.itemList
                        });
                        that.editMode(playlist.item.editMode());
                    } else {
                        var provider = that.providers.find(typeParam) || that.providers.find("youtube");
                        var promise = that.setProvider(provider);
                        if (mediaParam) {
                            promise = promise.always(function () {
                                that.playByQuery(mediaParam);
                            });
                        }
                        return promise;
                    }
                };

                this.editMode(editModeParam);

                if (listParam) {
                    this.playlist.loadSharedByUriAsync(listParam).always(complete);
                } else if (contactIdParam) {
                    this.playlist.loadDefaultByContactAsync(contactIdParam).always(complete);
                } else {
                    complete();
                }
            },
            hasSelection: function () {
                /// <signature>
                /// <returns type="Boolean" />
                /// </signature>
                var provider = this.provider();
                if (provider
                    && provider.list
                    && provider.list.selection().length) {
                    return true;
                }
                var playlist = this.playlist.sharedOrCurrent;
                if (playlist && playlist.itemList.selection().length) {
                    return true;
                }
                return false;
            },
            isMenuVisible: function () {
                /// <signature>
                /// <returns type="Boolean" />
                /// </signature>
                if (this.autoHideMenu && !this.user.identity()) {
                    return false;
                }
                return this.providerVisible
                    || this.searchVisible
                    || this.playlistEnabled()
                    || this.loginVisible;
            },
            isPlaylistDropImgVisible: function () {
                /// <signature>
                /// <summary>Returns true if the dropImage is visible.</summary>
                /// <returns type="Boolean" />
                /// </signature>
                return this.playlistDropImgUrl && this.hasSelection();
            },

            //
            // Authentication
            //

            authorize: function () {
                /// <signature>
                /// <returns type="$.Deferred" />
                /// </signature>
                if (this.user.getToken()) {
                    return _Promise.complete();
                }
                var that = this;
                return this.login.fbLoginAsync().then(
                    function () {
                        that.sponsorId = that.user.getIdentity().id;
                        that.editMode(ns.EditMode.editable);
                        that.playlistEnabled(true);

                        // Try to load the default list of the authenticated user.
                        if (that.playlist.defaultPlaylist.itemList.isEmpty) {
                            var user = that.user.identity();
                            user && that.playlist.playlistList.open().then(function () {
                                that.playlist.playlistList.moveToFirst();
                            });
                        }
                    });
            },

            //
            // Provider
            //

            loadProvAsync: function (provider) {
                /// <signature>
                /// <param name="provider" type="Object" />
                /// <returns type="$.Deferred" />
                /// </signature>
                if (provider) {
                    return provider.loadAsync();
                }
                return _Promise.error();
            },
            setProvider: function (provider) {
                /// <signature>
                /// <param name="provider" type="Object" />
                /// <returns type="$.Deferred" />
                /// </signature>
                if (provider /*&& (this.provider() !== provider)*/) {
                    var that = this;
                    provider.initAsync(this).then(function () {
                        that._watch(null);
                        that.provider(provider);
                        that._provChanged(provider);
                    });
                }
                return provider.promise;
            },
            reset: function () {
                /// <signature>
                /// <summary>Resets the media player.</summary>
                /// </signature>
                this.media.stopMedia();
                this.playlist.sharedOrCurrent.itemList.moveTo(null);
            },
            setView: function (options) {
                /// <signature>
                /// <param name="options" type="Object" value="{ view: PI.Project.Search.ViewMode, list: WinJS.Knockout.List.prototype }">
                ///     <para>view: PI.Project.Search.ViewMode</para>
                ///     <para>list?: WinJS.Knockout.List</para>
                /// </param>
                /// <returns type="$.Deferred" />
                /// </signature>
                var that = this;
                var view = options.view || ns.ViewMode.medialist;
                var list = options.list;

                return this.loadProvAsync(this.provider()).always(function () {
                    // Do not change view if a filter expression is specified for the current search provider or
                    // an unsaved playlist is loaded.
                    var provider = that.provider();
                    if (provider && provider.list && provider.list.filter.query() || !that.playlist.sharedOrCurrent.exists()) {
                        view = ns.ViewMode.medialist;
                        list = null;
                    }
                    if (view === ns.ViewMode.playlist) {
                        list || (list = that.playlist.sharedOrCurrent.itemList);
                        list && (that.banner.loadPlaylist(list));
                    } else {
                        list || (list = provider && provider.list);
                        list && (that.banner.loadMedialist(list));
                    }
                    that.view(view);
                });
            },

            //
            // Provider Event Handlers
            //

            _watch: function (provider) {
                /// <signature>
                /// <summary>Remove event listeners from the provider</summary>
                /// </signature>
                /// <signature>
                /// <summary>Attaches event handlers to the specified provider.</summary>
                /// <param name="provider" type="Object" />
                /// </signature>
                this._provListChangedSn && this._provListChangedSn.dispose(), this._provListChangedSn = null;
                this._provListSelChangedSn && this._provListSelChangedSn.dispose(), this._provListSelChangedSn = null;

                if (provider) {
                    var list = provider.list;
                    this._provListChangedSn = list.items.subscribe(this._provListChanged, this);
                    this._provListSelChangedSn = list.selection.subscribe(this._provListSelChanged, this);
                }
            },
            _provItemClick: function (provider) {
                /// <signature>
                /// <param name="provider" type="Object" />
                /// </signature>
                this.setProvider(provider);
                this.logging.logAction(provider.name);
            },
            _provChanged: function (provider) {
                /// <signature>
                /// <param name="provider" type="Object" />
                /// </signature>
                this.reset();
                this._watch(provider);
                this.setHashParam(ns.HashParam.providerType, provider.type);
                this.setView({ view: ns.ViewMode.medialist });
            },
            _provListChanged: function () {
                /// <signature>
                /// <summary>Occurs when the current provider changes.</summary>
                /// </signature>
                this.setView({ view: ns.ViewMode.medialist });
            },
            _provListSelChanged: function (values) {
                /// <signature>
                /// <summary>Occurs when selection changes in the list.</summary>
                /// </signature>
                if (this.playlistEnabled()) {
                    values
                        && values.length
                        && this.playlist.visible(true);
                }
            },
            _provFilterChanged: function () {
                /// <signature>
                /// <summary>Occurs when a filter property changes.</summary>
                /// </signature>
                this.setView({ view: ns.ViewMode.medialist });
            },

            //
            // Media Module Dispatchers
            //

            playByQuery: function (mediaQuery) {
                /// <signature>
                /// <param name="mediaQuery" type="String" />
                /// </signature>
                if (!mediaQuery) {
                    return;
                }
                var media = mediaQuery.split("!", 2);
                if (media.length === 2) {
                    this.playMedia(new ns.MediaItem({ type: media[0], id: media[1] }));
                }
            },
            playItem: function (playlistItem) {
                /// <signature>
                /// <param name="playlistItem" type="PI.Project.Search.PlaylistItem" optional="true" />
                /// </signature>
                var that = this;
                playlistItem = playlistItem || this.playlist.sharedOrCurrent.itemList.current();
                if (!this.bannerEnabled || this.banner.scrollIntoView(playlistItem, function () {
                    var coords = that.banner.calculateViewCoords();
                    that.media.setAnimation(coords.x, coords.y, 0);
                    that.media.playItem(playlistItem);
                }) === -1) {
                    this.setView({ view: ns.ViewMode.playlist });
                    this.media.playItem(playlistItem);
                }
            },
            playMedia: function (mediaItem, synchronizeWithPlaylist) {
                /// <signature>
                /// <param name="mediaItem" type="PI.Project.Search.MediaItem" optional="true" />
                /// <param name="synchronizeWithPlaylist" type="Boolean" optional="true" />
                /// </signature>
                mediaItem = mediaItem || this.provider().list.current();
                if (!mediaItem) {
                    return;
                }

                // If the specified media is in the playlist, try to find it there.
                if (synchronizeWithPlaylist !== false) {
                    var pls = this.playlist.sharedOrCurrent;
                    var plsItem = pls ? pls.itemList.findByMedia(mediaItem.type, mediaItem.id) : void 0;
                    if (plsItem) {
                        pls.itemList.moveTo(plsItem);
                        this.playItem(plsItem);
                        return;
                    }
                }

                var that = this;
                if (!this.bannerEnabled || this.banner.scrollIntoView(mediaItem, function () {
                    var coords = that.banner.calculateViewCoords();
                    that.media.setAnimation(coords.x, coords.y, 0);
                    that.media.playMedia(mediaItem);
                }) === -1) {
                    this.setView({ view: ns.ViewMode.medialist });
                    this.media.playMedia(mediaItem);
                }
            },
            playNext: function (direction) {
                /// <signature>
                /// <param name="direction" type="Number" value="0" />
                /// </signature>
                var list;
                var name = (direction < 0) ? "moveToPrev" : "moveToNext";
                if (this.view() === ns.ViewMode.medialist) {
                    list = this.provider().list;
                } else {
                    list = this.playlist.currentOrDefault.itemList.isEmpty
                         ? this.playlist.sharedOrCurrent.itemList
                         : this.playlist.currentOrDefault.itemList;
                }
                list && list[name]() && this.playItem(list.current());
            },

            //
            // Share
            //

            shareWithFacebook: function () {
                /// <signature>
                /// <summary>Shares the list by Facebook.</summary>
                /// </signature>
                var playlist = this.playlist.currentOrDefault;
                if (!playlist) {
                    return;
                }
                var playlistData = playlist.toObject();
                var playlistItem = playlist.itemList.getAt(0);
                if (playlistItem) {
                    var mediaItemId = this.getHashParam(ns.HashParam.mediaItemId);
                    try {
                        this.setHashParam(ns.HashParam.mediaItemId);
                        FB.ui({
                            method: "share",
                            href: window.location.href,
                            title: playlistData.name,
                            picture: playlistItem.thumbnailUrl
                        });
                    } finally {
                        this.setHashParam(ns.HashParam.mediaItemId, mediaItemId);
                    }
                }
            },

            //
            // URL helpers
            //

            makeParamName: function (name) {
                /// <signature>
                /// <param name="name" type="PI.Project.Search.HashParam" />
                /// <returns type="String" />
                /// </signature>
                return this.hashPrefix + "-" + name;
            },
            getHashParam: function (name) {
                /// <signature>
                /// <param name="name" type="PI.Project.Search.HashParam" />
                /// <returns type="String" />
                /// </signature>
                this._hash = this._hash || _Utilities.currentLinkHash();
                var paramName = this.makeParamName(name);
                return this._hash[paramName];
            },
            setHashParam: function (name, value) {
                /// <signature>
                /// <param name="name" type="PI.Project.Search.HashParam" />
                /// <param name="value" type="Object" optional="true" />
                /// </signature>
                this._hash = this._hash || _Utilities.currentLinkHash();
                var paramName = this.makeParamName(name);
                if (value !== undefined) {
                    this._hash[paramName] = value;
                } else {
                    delete this._hash[paramName];
                }
                _Utilities.currentLinkHash(this._hash, false);
            },

            _mediaPlay: function (event) {
                /// <signature>
                /// <param name="event" type="Event" />
                /// </signature>
                var mediaItem = event.detail.mediaItem;
                this.setHashParam(ns.HashParam.mediaItemId, mediaItem && (mediaItem.type + "!" + mediaItem.id));
            },
            _mediaStop: function () {
                /// <signature>
                /// <param name="event" type="Event" />
                /// </signature>
                this.setHashParam(ns.HashParam.mediaItemId);
            },
            _mediaStateChanged: function (event) {
                /// <signature>
                /// <param name="event" type="Event" />
                /// </signature>
                var state = event.detail.newState;
                if (state === _WinJS.Media.MediaPlayerState.ended) {
                    var mode = this.playlist.playMode();
                    if (mode === ns.PlayMode.repeatOne) {
                        this.media.mediaPlayer.play();
                    } else if (mode === ns.PlayMode.repeatAll) {
                        this.playNext();
                    }
                }
            },
            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                if (this._disposed) {
                    return;
                }

                this._watch(null);

                this.menuVisible && this.menuVisible.dispose(), this.menuVisible = null;
                this.playlistDropImgVisible && this.playlistDropImgVisible.dispose(), this.playlistDropImgVisible = null;
                this.playlist && this.playlist.dispose && this.playlist.dispose(), this.playlist = null;
                this.media && this.media.dispose && this.media.dispose(), this.media = null;
                this.banner && this.banner.dispose && this.banner.dispose(), this.banner = null;
                this.login && this.login.dispose && this.login.dispose(), this.login = null;
                this.logging && this.logging.dispose && this.logging.dispose(), this.logging = null;

                this._disposed = true;
            }
        })
    });

    //
    // Defines a class using the given constructor and the union of the set of instance members specified by all the mixin objects.
    //

    _Class.mix(ns.Engine, _Utilities.eventMixin);

})(ko, WinJS, PI);
