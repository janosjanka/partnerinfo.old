// Copyright (c) Partnerinfo TV. All Rights Reserved.

/// <reference path="common.js" />
/// <reference path="//www.google-analytics.com/analytics.js" />

(function (_Global, _WinJS, _PI) {
    "use strict";

    var namespace = "Portal.Analytics";
    var _Class = _WinJS.Class;

    var MapECMA6 = window.Map ? window.Map : _Class.define(null, {
        get: function (key) {
            return this[key];
        },
        set: function (key, value) {
            this[key] = value; return this;
        }
    });

    var ns = _WinJS.Namespace.defineWithParent(_PI, namespace, {

        GoogleProvider: _Class.define(function GoogleProviderProvider_ctor(trackingId) {
            /// <signature>
            /// <summary>Initializes a new instance of the GoogleProvider class.</summary>
            /// <param name="trackingId" type="String" />
            /// <returns type="PI.Portal.Analytics.GoogleProvider" />
            /// </signature>
            this._require();
            this._createTracker(trackingId);
        }, {
            _require: function () {
                /// <signature>
                /// <summary>Initializes Google Analytics provider-</summary>
                /// </signature>
                if (_Global.document.getElementById("gaAnalytics")) {
                    return;
                }
                _Global["GoogleAnalyticsObject"] = "ga";
                _Global["ga"] = _Global["ga"] || function () { (_Global["ga"].q = _Global["ga"].q || []).push(arguments); };
                _Global["ga"].l = 1 * new Date();
                var script = _Global.document.createElement("script");
                var firstScript = _Global.document.getElementsByTagName("script")[0];
                script.id = "gaAnalytics";
                script.async = true;
                script.src = "//www.google-analytics.com/analytics.js";
                firstScript.parentNode.insertBefore(script, firstScript);
            },
            _createTracker: function (trackingId) {
                /// <signature>
                /// <param name="trackingId" type="String" />
                /// </signature>
                _Global.ga("create", trackingId);
            },
            sendView: function () {
                /// <signature>
                /// <summary>Sends a pageView signal to Google Analytics.</summary>>
                /// </signature>
                _Global.ga("send", "pageView");
                _WinJS.DEBUG && _WinJS.log({ category: namespace, message: "GA - pageView" });
            },
            sendEvent: function (options) {
                /// <signature>
                /// <summary>Sends an event signal to Google Analytics.</summary>
                /// <param name="options" type="Object" />
                /// </signature>
                _Global.ga("send", {
                    hitType: "event",
                    eventCategory: options.category,
                    eventAction: options.action,
                    eventLabel: options.label,
                    eventValue: options.value
                });
                _WinJS.DEBUG && _WinJS.log({ category: namespace, message: "GA - event" }, options);
            }
        }),

        ContactEventInfo: _Class.define(function ContactEventInfo_ctor(contactId) {
            /// <signature>
            /// <summary>Initializes a new instance of the ContactEventInfo class.</summary>
            /// <param name="contactId" type="Number" />
            /// <returns type="PI.Portal.Analytics.ContactEventInfo" />
            /// </signature>
            this.contactId = contactId;
            this.clicks = 0;
            this.clickedDate = null;
            this.hovers = 0;
            this.hoverDate = null;
            this.hoverTime = 0;
        }, {
            toString: function () {
                /// <signature>
                /// <summary>Returns a string that represents the current object.</summary>
                /// <returns type="String" />
                /// </signature>
                return String.format("id: {0}, clicks: {1}, hovers: {2}", this.id, this.clicks, this.hovers);
            }
        }),

        NodeEventInfo: _Class.define(function NodeEventInfo_ctor(id) {
            /// <signature>
            /// <summary>Initializes a new instance of the NodeInfo class.</summary>
            /// <param name="id" type="String" />
            /// <returns type="PI.Portal.Analytics.NodeEventInfo" />
            /// </signature>
            this.id = id;
            this.clicks = 0;
            this.clickedDate = null;
            this.hovers = 0;
            this.hoverDate = null;
            this.hoverTime = 0;
            this.contacts = null;
        }, {
            logClick: function () {
                /// <signature>
                /// <summary>Logs a click event.</summary>
                /// </signature>
                this.clicks += 1;
                this.clickedDate = Date.now();
            },
            logHover: function () {
                /// <signature>
                /// <summary>Logs a mouse hover event.</summary>
                /// </signature>
                this.hovers += 1;
                this.hoverDate = Date.now();
            },
            toString: function () {
                /// <signature>
                /// <summary>Returns a string that represents the current object.</summary>
                /// <returns type="String" />
                /// </signature>
                return String.format("id: {0}, clicks: {1}, clickedDate: {2}, hovers: {3}, hoverDate: {4}",
                    this.id, this.clicks, this.clickedDate, this.hovers, this.hoverDate);
            }
        }),

        Watcher: _Class.define(function Watcher_ctor(engine) {
            /// <signature>
            /// <summary>Initializes a new instance of the Watcher class.</summary>
            /// <param name="engine" type="PI.Portal.Engine" />
            /// <returns type="Watcher" />
            /// </signature>
            this._disposed = false;
            this._clickBound = this._click.bind(this);
            this._hoverBound = this._hover.bind(this);

            this._engine = engine;
            this._container = $(engine.context.container);
            this._nodes = new MapECMA6();
            this._enabled = false;
        }, {
            enabled: {
                get: function () {
                    return this._enabled;
                },
                set: function (value) {
                    if (value && this._engine.portal && this._engine.portal.gaTrackingId) {
                        this._provider = new ns.GoogleProvider(this._engine.portal.gaTrackingId);
                        this._container.on("click", this._clickBound).on("mouseover", this._hoverBound);
                        this._enabled = true;
                    } else {
                        this._container.off("click", this._clickBound).off("mouseover", this._hoverBound);
                        this._provider = null;
                        this._enabled = false;
                    }
                }
            },
            sendView: function () {
                /// <signature>
                /// <summary>Sends a pageView signal to the current provider.</summary>>
                /// </signature>
                this._provider && this._provider.sendView();
            },
            sendEvent: function (options) {
                /// <signature>
                /// <summary>Sends an event signal to the current provider.</summary>
                /// <param name="options" type="Object" />
                /// </signature>
                this._provider && this._provider.sendEvent(options);
            },
            _click: function (event) {
                /// <signature>
                /// <summary>Raised immediately after an element is clicked.</summary>
                /// <param name="event" type="MouseEvent" />
                /// </signature>
                var target = this._closestNodeWithId(event.target);
                if (target.id) {
                    var info = this._getNodeEventInfo(target.id);
                    info.logClick();
                    this.sendEvent({ category: "#" + info.id, action: "click" });
                    _WinJS.DEBUG && _WinJS.log({ category: namespace, message: info.toString() });
                }
                return true;
            },
            _hover: function (event) {
                /// <signature>
                /// <summary>Raised immediately after the mouse is over an element.</summary>
                /// <param name="event" type="MouseEvent" />
                /// </signature>
                var target = event.target;
                if (target.id) {
                    var info = this._getNodeEventInfo(target.id);
                    info.logHover();
                    _WinJS.DEBUG && _WinJS.log({ category: namespace, message: info.toString() });
                }
                return true;
            },
            _closestNodeWithId: function (node) {
                /// <signature>
                /// <summary>Finds the closest node where an ID is presented.</summary>
                /// <param name="node" type="HTMLElement" />
                /// </signature>
                while (node && node !== _Global.document && !node.id) { node = node.parentNode; }
                return node;
            },
            _getNodeEventInfo: function (id) {
                /// <signature>
                /// <summary>Gets a node object for the given ID.</summary>
                /// <returns type="NodeEventInfo" />
                /// </signature>
                var node = this._nodes.get(id);
                if (node) {
                    return node;
                }
                node = new ns.NodeEventInfo(id);
                this._nodes.set(id, node);
                return node;
            },
            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                if (this._disposed) {
                    return;
                }
                this.enabled = false;
                this._disposed = true;
            }
        })

    });

})(window, WinJS, PI);
