// Copyright (c) Partnerinfo Ltd. All Rights Reserved.
/// <reference path="https://www.youtube.com/player_api" />
/// <reference path="https://api.dmcdn.net/all.js" />

(function (_WinJS) {
    "use strict";

    var _Class = _WinJS.Class;
    var _Utilities = _WinJS.Utilities;

    var MediaPlayerState = {
        closed: 0,
        opened: 1,
        playing: 2,
        paused: 3,
        ended: 4
    },

    MediaQuality = {
        unknown: "unknown",
        small: "small",
        medium: "medium",
        large: "large",
        hd720: "hd720",
        hd1080: "hd1080"
    },

    noop = _WinJS.noop,

    MediaPlayer = _Class.define(function MediaPlayer_ctor(elementId, options) {
        /// <signature>
        /// <param name="elementId" type="String" />
        /// <param name="options" type="Object" optional="true" />
        /// <returns type="MediaPlayer" />
        /// </signature>
        _Utilities.setOptions(this, options = options || {});
        this.elementId = elementId;
        this.autoOpen = options.autoOpen !== false;
        this.autoPlay = !!options.autoPlay;
        this.videoId = options.videoId || null;
        this.width = options.width || "100%";
        this.height = options.height || "100%";
        this.volume = options.volume || 100;
        this.params = options.params || {};
        this.state = MediaPlayerState.closed;
        this.autoOpen && this.open();
    }, {
        /// <field>
        /// <summary>Opens the video stream.</summary>
        /// </field>
        open: noop,

        /// <field>
        /// <summary>Plays the video.</summary>
        /// </field>
        play: noop,

        /// <field>
        /// <summary>Pauses the video.</summary>
        /// </field>
        pause: noop,

        /// <field>
        /// <summary>Seeks to a specified time in the video.</summary>
        /// <param name="seconds" type="Number">The seconds parameter identifies the time to which the player should advance.</param>
        /// </field>
        seek: noop,

        /// <field>
        /// <summary>Stops the video.</summary>
        /// </field>
        stop: noop,

        /// <field>
        /// <summary>Closes the video stream.</summary>
        /// </field>
        close: noop,

        /// <field>
        /// <summary>This method returns the DOM node for the embedded "iframe".</summary>
        /// <returns type="String" />
        /// </field>
        getIframe: noop,

        /// <field>
        /// <summary>Gets the embed code for the video.</summary>
        /// <returns type="String" />
        /// </field>
        getEmbedCode: noop,

        setOption: function (key, value) {
            /// <signature>
            /// <summary>Updates the value of the specified property.</summary>
            /// <param name="key" type="String" />
            /// <param name="value" type="Object" />
            /// </signature>
            switch (key) {
                case "videoId":
                    this[key] = value;
                    this.refresh();
                    return;
                case "quality":
                    this._applyQuality(value);
                    break;
                case "volume":
                    this._applyVolume(value);
                    break;
                case "width":
                    this._applySize(value, this.height);
                    break;
                case "height":
                    this._applySize(this.width, value);
                    break;
            }
            this[key] = value;
        },

        refresh: function () {
            this.close();
            this.open();
        },

        /// <field>
        /// <summary>This function sets the suggested video quality for the current video.</summary>
        /// <param name="quality" type="MediaQuality">The quality to set.</param>
        /// </field>
        _applyQuality: noop,

        /// <field>
        /// <summary>Sets the volume. Accepts an integer between 0 and 100.</summary>
        /// <param name="volume" type="Number">The value to set.</param>
        /// </field>
        _applyVolume: noop,

        /// <field>
        /// <summary>Sets the size in pixels of the "iframe" that contains the player.</summary>
        /// <param name="width" type="String">The new width.</param>
        /// <param name="height" type="String">The new height.</param>
        /// </field>
        _applySize: noop,

        _setState: function (state) {
            /// <signature>
            /// <summary>Sets a new media state value.</summary>
            /// <param name="state" type="MediaPlayerState">The state value.</param>
            /// </signature>
            var oldState = this.state;
            this.state = state;
            this.dispatchEvent("statechanged", { oldState: oldState, newState: state });
        }
    }),

    YouTubePlayer = _Class.derive(MediaPlayer, function YouTubePlayer_ctor(elementId, options) {
        /// <signature>
        /// <param name="elementId" type="String" />
        /// <param name="options" type="Object" optional="true" />
        /// <returns type="MediaPlayer" />
        /// </signature>
        MediaPlayer.call(this, elementId, options);
    }, {
        /// <field type="YT.Player" />
        _player: null,
        _playerReady: function () {
            /// <signature>
            /// <summary>This event fires whenever a player has finished loading and is ready to begin receiving API calls.</summary>
            /// <param name="event" type="Object">The event object that the API passes to the function has a target property, which identifies the player.</param>
            /// </signature>
            this._setState(MediaPlayerState.opened);
            this.quality && this._applyQuality(this.quality);
            this.volume && this._applyVolume(this.volume);
            this.autoPlay && this.play();
        },
        _playerStateChanged: function (event) {
            /// <signature>
            /// <summary>This event fires whenever the player's state changes.</summary>
            /// <param name="event" type="Object">The data property of the event object that the API passes to your event listener function will specify an integer that corresponds to the new player state.</param>
            /// </signature>
            switch (event.data) {
                case YT.PlayerState.ENDED:
                    this._setState(MediaPlayerState.ended);
                    break;
                case YT.PlayerState.PLAYING:
                    this._setState(MediaPlayerState.playing);
                    this.quality && this._setQuality(this.quality); // Workaround.
                    break;
                case YT.PlayerState.PAUSED:
                    this._setState(MediaPlayerState.paused);
                    break;
            }
        },

        open: function () {
            /// <signature>
            /// <summary>Opens the video stream.</summary>
            /// </signature>
            this._player || (this._player = new YT.Player(this.elementId, {
                videoId: this.videoId,
                height: this.width,
                width: this.height,
                playerVars: this.params,
                events: {
                    onReady: this._playerReady.bind(this),
                    onStateChange: this._playerStateChanged.bind(this)
                }
            }));
        },
        play: function () {
            /// <signature>
            /// <summary>Plays the video.</summary>
            /// </signature>
            this._player ? this._player.playVideo() : this.open();
        },
        pause: function () {
            /// <signature>
            /// <summary>Pauses the video.</summary>
            /// </signature>
            this._player && this._player.pauseVideo();
        },
        seek: function (seconds) {
            /// <signature>
            /// <summary>Seeks to a specified time in the video.</summary>
            /// <param name="seconds" type="Number">The seconds parameter identifies the time to which the player should advance.</param>
            /// </signature>
            this._player && this._player.seekTo(seconds, !1);
        },
        stop: function () {
            /// <signature>
            /// <summary>Stops the video.</summary>
            /// </signature>
            this._player && this._player.stopVideo();
        },
        close: function () {
            /// <signature>
            /// <summary>Closes the video stream.</summary>
            if (this._player) {
                try {
                    this._player.destroy();
                } finally {
                    this._player = null;
                    this._setState(MediaPlayerState.closed);
                }
            }
        },

        getEmbedCode: function () {
            /// <signature>
            /// <summary>Gets the embed code for the video.</summary>
            /// <returns type="String" />
            /// </signature>
            return this._player ? this._player.getVideoEmbedCode() : "";
        },
        getIframe: function () {
            /// <signature>
            /// <summary>This method returns the DOM node for the embedded "iframe".</summary>
            /// <returns type="String" />
            /// </signature>
            return this._player ? this._player.getIframe() : "";
        },

        _applyQuality: function (quality) {
            /// <signature>
            /// <summary>This function sets the suggested video quality for the current video.</summary>
            /// <param name="quality" type="MediaQuality">The quality to set.</param>
            /// </signature>
            if (this._player) {
                var value = YouTubePlayer.quality[quality];
                value && this._player.setPlaybackQuality(value);
            }
        },
        _applySize: function (width, height) {
            /// <signature>
            /// <summary>Sets the size in pixels of the "iframe" that contains the player.</summary>
            /// <param name="width" type="String">The new width.</param>
            /// <param name="height" type="String">The new height.</param>
            /// </signature>
            this._player && this._player.setSize(width, height);
        },
        _applyVolume: function (value) {
            /// <signature>
            /// <summary>Sets the volume. Accepts an integer between 0 and 100.</summary>
            /// <param name="value" type="Number">The value to set.</param>
            /// </signature>
            this._player && this._player.setVolume(value);
        }
    }, {
        quality: {
            unknown: null,
            small: "small",
            medium: "medium",
            large: "large",
            hd720: "hd720",
            hd1080: "hd1080"
        }
    }),

    DailymotionPlayer = _Class.derive(MediaPlayer, function DailymotionPlayer_ctor(elementId, options) {
        /// <signature>
        /// <param name="elementId" type="String" />
        /// <param name="options" type="Object" optional="true" />
        /// <returns type="MediaPlayer" />
        /// </signature>
        MediaPlayer.call(this, elementId, options);
    }, {
        /// <field type="DM.player" />
        _player: null,
        _playerReady: function () {
            /// <signature>
            /// <summary>This event fires whenever a player has finished loading and is ready to begin receiving API calls.</summary>
            /// <param name="event" type="Object">The event object that the API passes to the function has a target property, which identifies the player.</param>
            /// </signature>
            this._setState(MediaPlayerState.opened);
            this.quality && this._applyQuality(this.quality);
            this.volume && this._applyVolume(this.volume);
            this.autoPlay && this.play();
        },

        open: function () {
            /// <signature>
            /// <summary>Opens the video stream.</summary>
            /// </signature>
            if (!this._player) {
                this._player = DM.player(this.elementId, {
                    video: this.videoId,
                    height: this.width,
                    width: this.height,
                    params: this._createParams(this.params)
                });
                this._player.addEventListener("apiready", this._playerReady.bind(this), false);
            }
        },
        play: function () {
            /// <signature>
            /// <summary>Plays the video.</summary>
            /// </signature>
            if (this._player) {
                this._player.play();
            } else {
                this.open();
            }
        },
        pause: function () {
            /// <signature>
            /// <summary>Pauses the video.</summary>
            /// </signature>
            if (this._player) {
                this._player.pause();
            }
        },
        seek: function (seconds) {
            /// <signature>
            /// <summary>Seeks to a specified time in the video.</summary>
            /// <param name="seconds" type="Number">The seconds parameter identifies the time to which the player should advance.</param>
            /// </signature>
            if (this._player) {
                this._player.seek(seconds);
            }
        },
        stop: function () {
            /// <signature>
            /// <summary>Stops the video.</summary>
            /// </signature>
            if (this._player) {
                this._player.pause();
                this._player.seek(0);
            }
        },
        close: function () {
            var parent = this._player && this._player.parentNode;
            parent && (
                parent.removeChild(this._player),
                this._player = null,
                this._setState(MediaPlayerState.closed)
            );
        },

        _createParams: function (params) {
            params = params || {};
            //var quality = DailymotionPlayer.quality[this.quality];
            //quality && (params.quality = quality);
            return params;
        },
        _applyVolume: function (value) {
            /// <signature>
            /// <summary>Sets the volume. Accepts an integer between 0 and 100.</summary>
            /// <param name="value" type="Number">The value to set.</param>
            /// </signature>
            if (this._player) {
                this._player.volume = value / 100;
            }
        }
    }, {
        quality: {
            unknown: null,
            small: 240,
            medium: 380,
            large: 480,
            hd720: 720,
            hd1080: 1080
        }
    });

    //
    // Public Namespaces & Classes
    //

    _Class.mix(MediaPlayer, _Utilities.eventMixin);
    _Class.mix(MediaPlayer, _Utilities.createEventProperties("statechanged"));

    _WinJS.Namespace.defineWithParent(_WinJS, "Media", {
        MediaPlayerState: MediaPlayerState,
        MediaQuality: MediaQuality,
        MediaPlayer: MediaPlayer,
        YouTubePlayer: YouTubePlayer,
        DailymotionPlayer: DailymotionPlayer
    });

})(WinJS);
