// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (_WinJS) {
    "use strict";

    _WinJS.Namespace.defineWithParent(PI, "Logging", {
        NotificationService: _WinJS.Class.define(function NotificationService_ctor(connection, options) {
            /// <signature>
            /// <summary>Initializes a new instance of the NotificationService class.</summary>
            /// <param name="connection" type="$.hubConnection">The connection object.</param>
            /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the NotificationService.</param>
            /// <returns type="NotificationService" />
            /// </signature>
            options = options || {};
            options.proxyName = options.proxyName || "notification";
            this._proxy = connection.createHubProxy(options.proxyName);
        }, {
            on: function (name, fn) {
                /// <signature>
                /// <summary>Attaches an event listener to the proxy object.</summary>
                /// <param name="name" type="String" />
                /// <param name="fn" type="Function" />
                /// </signature>
                this._proxy.on(name, fn);
            },
            off: function (name, fn) {
                /// <signature>
                /// <summary>Detaches an event listener from the proxy object.</summary>
                /// <param name="name" type="String" />
                /// <param name="fn" type="Function" />
                /// </signature>
                this._proxy.off(name, fn);
            }
        })
    });

})(WinJS);
