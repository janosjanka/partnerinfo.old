// Copyright (c) Partnerinfo Ltd. All Rights Reserved.
/// <reference path="../engine.js" />

(function (_WinJS) {

    var _Class = _WinJS.Class;
    var _Utilities = _WinJS.Utilities;

    var ns = _WinJS.Namespace.defineWithParent(PI, "Project.Search", {

        MediaPlayerType: {
            youtube: _WinJS.Media.YouTubePlayer,
            dailymotion: _WinJS.Media.DailymotionPlayer
        },

        MediaModule: _Class.define(function MediaModule_ctor(engine, options) {
            /// <signature>
            /// <summary>Initializes a new instance of the MediaModule class.</summary>
            /// <param name="engine" type="PI.Project.Search.Engine">Search engine.</param>
            /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the MediaModule.</param>
            /// <returns type="MediaModule" />
            /// </signature>
            options = options || {};
            _Utilities.setOptions(this, options);

            this.engine = engine;
            this.elementId = options.elementId;
            this.delay = options.delay || 1000;
            this.mediaItem = ko.observable(options.mediaItem);
            this.playlistItem = null;
            this.animation = ko.observable(false);
            this.transform = ko.observable();
        }, {
            addToPlaylist: function () {
                /// <signature>
                /// <summary>Adds the current media to the playlist.</summary>
                /// </signature>
                var mediaItem = this.mediaItem();
                mediaItem && this.engine.playlist.addMediaItems([mediaItem]);
            },
            playItem: function (playlistItem) {
                /// <signature>
                /// <param name="playlistItem" type="PI.Project.Search.PlaylistItem" />
                /// </signature>
                this.stopMedia();
                if (playlistItem) {
                    var mediaItem = playlistItem.toMediaItem();
                    playlistItem.isActive(true);
                    this.playlistItem = playlistItem;
                    this.mediaItem(mediaItem);
                    this._playMedia(mediaItem);

                    // Adds a log entry with this action to the database.
                    this.engine.logging.logAction(mediaItem.title);
                }
            },
            playMedia: function (mediaItem) {
                /// <signature>
                /// <param name="mediaItem" type="PI.Project.Search.MediaItem" />
                /// </signature>
                this.stopMedia();
                if (mediaItem) {
                    mediaItem.isActive(true);
                    this.mediaItem(mediaItem);
                    this._playMedia(mediaItem);

                    // Adds a log entry with this action to the database.
                    this.engine.logging.logAction(mediaItem.title);
                }
            },
            stopMedia: function () {
                /// <signature>
                /// <summary>Stops the current media item.</summary>
                /// </signature>
                this.mediaPlayer && this.mediaPlayer.close();
                this.mediaPlayer = null;
                this.playlistItem && this.playlistItem.isActive(false);
                this.playlistItem = null;
                var mediaItem = this.mediaItem();
                if (mediaItem) {
                    mediaItem.isActive(false);
                    this.mediaItem(null);
                    this.dispatchEvent("stop");
                }
            },
            _getParams: function (type) {
                /// <signature>
                /// <summary>Gets player parameters based on media type.</summary>
                /// </signature>
                var options = { wmode: "opaque" };
                if (type === "youtube") {
                    options.showinfo = 0;
                    options.rel = 0;
                    options.modestbranding = 1;
                } else if (type === "dailymotion") {
                    options.info = 0;
                    options.related = 0;
                    // options.logo = 0; // Throws
                }
                return options;
            },
            _playMedia: function (mediaItem) {
                /// <signature>
                /// <summary>Plays the current media item.</summary>
                /// </signature>
                if (!mediaItem) {
                    this.stopMedia();
                    return;
                }
                var that = this;
                this.engine.providers.loadByNameAsync(mediaItem.type).done(
                    function () {
                        _Utilities.async(function () {
                            that.mediaPlayer = new PI.Project.Search.MediaPlayerType[mediaItem.type](that.elementId, {
                                autoPlay: true,
                                videoId: mediaItem.id,
                                params: that._getParams(mediaItem.type),
                                onstatechanged: that._mediaPlayerStateChanged.bind(that)
                            });
                            that.setAnimation(); // Full screen fix.
                            that.dispatchEvent("play", {
                                mediaItem: mediaItem,
                                playlistItem: that.playlistItem
                            });
                        }, that, that.delay || 0);
                    });
            },
            _mediaPlayerStateChanged: function (event) {
                /// <signature>
                /// <summary>Raised when the media player's state has been changed.</summary>
                /// </signature>
                this.dispatchEvent("statechanged", event.detail);
            },

            setAnimation: function (translateX, translateY, scale) {
                /// <signature>
                /// <summary>Sets a CSS3 media transformation for the media element.</summary>
                /// </signature>
                if (arguments.length === 0) {
                    this.animation(false);
                    this.transform(null);
                    return;
                }
                var transform = "translate(";
                transform += translateX || 0;
                transform += "px,";
                transform += translateY || 0;
                transform += "px) scale(";
                transform += scale || 0;
                transform += ")";
                this.animation(true);
                this.transform(transform);
            }
        })
    });

    //
    // Defines a class using the given constructor and the union of the set of instance members specified by all the mixin objects.
    //

    _Class.mix(ns.MediaModule, _Utilities.createEventProperties("play", "stop", "statechanged"));
    _Class.mix(ns.MediaModule, _Utilities.eventMixin);

})(WinJS);