// Knockout Events plugin v1.0
// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (ko) {
    "use strict";

    ko.events = function (target, options) {
        function events() {
            return this;
        }
        ko.subscribable.call(events);

        events.addEventListeners = function (options) {
            for (var key in options) {
                var ch1 = key[0];
                var ch2 = key[1];
                if ((ch1 === 'o' || ch1 === 'O') && (ch2 === 'n' || ch2 === 'N')) {
                    var value = options[key];
                    if (typeof value === "function") {
                        this.subscribe(value, target, key.substr(2));
                    }
                }
            }
        };

        events.dispose = function () {
            /// <signature>
            /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            /// </signature>
            for (var key in this._subscriptions) {
                if (this._subscriptions.hasOwnProperty(key)) {
                    var subscriptions = this._subscriptions[key];
                    for (var i = 0, len = subscriptions.length; i < len; ++i) {
                        subscriptions[i].dispose();
                    }
                }
            }
        };

        ko.exportProperty(target, "subscribe", events.subscribe.bind(events));
        ko.exportProperty(target, "notifySubscribers", events.notifySubscribers.bind(events));

        options && events.addEventListeners(options);

        return events;
    };

})(ko);