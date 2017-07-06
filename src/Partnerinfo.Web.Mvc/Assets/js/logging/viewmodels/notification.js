// Copyright (c) Partnerinfo TV. All Rights Reserved.
/// <reference path="viewmodels/event.js" />

(function (_WinJS, _PI) {
    "use strict";

    var _Class = _WinJS.Class;
    var _Utilities = _WinJS.Utilities;

    var ns = _WinJS.Namespace.defineWithParent(_PI, "Logging", {
        EventWatcher: _Class.define(function (options) {
            /// <signature>
            /// <summary>Initializes a new instance of the EventWatcher class.</summary>
            /// <param name="options" type="Object" />
            /// <returns type="PI.Logging.EventWatcher" />
            /// </signature>
            _Utilities.setOptions(this, options = options || {});
            this._notifyEventReceivedBound = this._notifyEventReceived.bind(this);
            var connection = $.hubConnection("/signalr", { useDefaultPath: false });
            this.service = options.service || new ns.NotificationService(connection);
            this.service.on("eventReceived", this._notifyEventReceivedBound);
            connection.start();
        }, {
            _notifyEventReceived: function (logEvent) {
                /// <signature>
                /// <summary>Raises an eventreceived event.</summary>
                /// <param name="logEvent" type="Object" />
                /// </signature>
                this.dispatchEvent("eventreceived", logEvent);
            },
            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                if (this._disposed) {
                    return;
                }
                this.service.off("eventReceived", this._notifyEventReceivedBound);
                this._disposed = true;
            }
        }, {
            getInstance: function () {
                /// <signature>
                /// <summary>Gets a singleton instance.</summary>
                /// <returns type="PI.Logging.EventWatcher" />
                /// </signature>
                return this._instance || (this._instance = new ns.EventWatcher());
            }
        }),

        EventTracker: _Class.define(function EventTracker_ctor(options) {
            /// <signature>
            /// <summary>Represents a view that can be used to define entity operations.</summary>
            /// <param name="options" type="Object" optional="true">A set of key/value pairs that can be used to configure entity operations.</param>
            /// <returns type="PI.Logging.EventTracker" />
            /// </signature>
            if (options == null || !options.eventId) {
                throw new TypeError("eventId is required.");
            }

            this.eventId = options.eventId;
            this.signalInternal = options.signalInternal || 30000;  // 30 sec
            this.restartTimeout = options.restartTimeout || 120000; // 2 mins
            this.failCount = 0;
            this.maxFailCount = options.maxFailCount || 3;
            this.signalOnWindowClose = options.signalOnWindowClose !== false;

            if (this.signalOnWindowClose) {
                var that = this;
                $(window).bind("beforeunload.EvenTracker", function () {
                    that.signal(false);
                });
            }

            this.start();
        }, {
            start: function () {
                /// <signature>
                /// <summary>Creates a timer for tracking the current user.</summary>
                /// </signature>
                this.stop();
                this.failCount = 0;
                var that = this;
                this._handle = window.setInterval(function () {
                    that.signal(true);
                    that._check();
                }, this.signalInternal);
            },
            stop: function () {
                /// <signature>
                /// <summary>Stops the timer.</summary>
                /// </signature>
                if (this._handle) {
                    window.clearInterval(this._handle);
                    this._handle = null;
                }
            },
            signal: function (async) {
                /// <signature>
                /// <summary>Sends a signal indicating the user is online.</summary>
                /// <param name="async" type="Boolean" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _PI.api({
                    method: "POST",
                    path: "logging/events/signal/finish",
                    data: this.eventId,
                    async: async !== false
                }, this).fail(function () {
                    if (++this.failCount >= this.maxFailCount) {
                        this.stop();
                    }
                });
            },
            _check: function () {
                /// <signature>
                /// <summary>Stops the timer if the number of errors is greater than the maxFailCount's value.</summary>
                /// </signature>
                var that = this;
                if (this.failCount >= this.maxFailCount) {
                    this.stop();
                    var timeout = window.setTimeout(function () {
                        window.clearTimeout(timeout);
                        that.start();
                    }, this.restartTimeout);
                }
            },
            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                this.stop();
                $(window).unbind("beforeunload.EvenTracker");
            }
        }),

        EventManager: _Class.define(null, null, {
            _handle: null,
            eventId: null,
            cacheKeyTime: "events.lastreadtime",
            cacheKeyUnread: "events.unreadcount",
            pollingInterval: 30000, // Long-polling interval
            refreshTimeout: 120000, // Refresh interval
            title: document.title, // Original document title
            unread: ko.observable(0), // Number of unread events
            initialize: function () {
                /// <signature>
                /// <summary>Starts a new timer (long-polling scenario).</summary>
                /// </signature>
                this.setUnread(_PI.userCache(_PI.Storage.local, this.cacheKeyUnread) | 0);
                this._handle = window.setInterval(this.refresh.bind(this), this.pollingInterval);
            },
            refresh: function () {
                /// <signature>
                /// <summary>Refresh the event collection.</summary>
                /// </signature>
                if (this._checkRefreshTimeout()) {
                    this._resetRefreshTimeout();
                    _PI.api({ path: "logging/events/unread/count" }).always(this.setUnread.bind(this));
                }
            },
            markAsRead: function () {
                /// <signature>
                /// <summary>Reads all events.</summary>
                /// </signature>
                this.unread() && _PI.api({
                    method: "POST",
                    path: "logging/events/signal/read"
                }).done(this.setUnread.bind(this, 0));
            },
            setUnread: function (unread) {
                /// <signature>
                /// <param name="unread" type="Number" />
                /// </signature>
                unread = unread | 0;
                this.unread(unread);
                this.setTitle(unread);
                _PI.userCache(_PI.Storage.local, this.cacheKeyUnread, unread);
            },
            setTitle: function (unread) {
                /// <signature>
                /// <param name="unread" type="Number" />
                /// </signature>
                document.title = unread ? "(" + unread + ") " + this.title : this.title;
            },
            setEventId: function (eventId) {
                /// <signature>
                /// <param name="eventId" type="Number" />
                /// </signature>
                this.eventId = eventId;
                if (eventId) {
                    this._eventTracker = new ns.EventTracker({ eventId: eventId });
                } else {
                    this._eventTracker && this._eventTracker.dispose();
                    this._eventTracker = null;
                }
            },
            _checkRefreshTimeout: function () {
                /// <signature>
                /// <summary>Checks whether the elapsed time is greater than the current refreshTimeout value.</summary>
                /// <returns type="Boolean" />
                /// </signature>
                var time = parseInt(_PI.userCache(_PI.Storage.local, this.cacheKeyTime), 10);
                return isNaN(time) ? true : new Date().getTime() - time > this.refreshTimeout;
            },
            _resetRefreshTimeout: function () {
                /// <signature>
                /// <summary>Saves the current time to the cache.</summary>
                /// </signature>
                _PI.userCache(_PI.Storage.local, this.cacheKeyTime, new Date().getTime());
            }
        })
    });

    _Class.mix(ns.EventWatcher, _Utilities.createEventProperties("eventreceived"));
    _Class.mix(ns.EventWatcher, _Utilities.eventMixin);

    _PI.identity && ns.EventManager.initialize();

})(WinJS, PI);
