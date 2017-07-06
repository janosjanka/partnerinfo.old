// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (_WinJS) {
    "use strict";

    var Messenger = _WinJS.Class.define(function Messenger_ctor() {
        /// <signature>
        /// <summary>Initializes a new instance of the Messenger class.</summary>
        /// <returns type="Messenger" />
        /// </signature>
        this._recipients = {};
    }, {
        register: function (recipient, type, token, action) {
            /// <signature>
            /// <summary>Registers a recipient for a type of message. The action parameter will be executed when a corresponding message is sent.</summary>
            /// <param name="recipient" type="Object">The recipient that will receive the messages.</param>
            /// <param name="type" type="String">A type descriptor.</param>
            /// <param name="action" type="Function">The action that will be executed when a message of type is sent.</param>
            /// </signature>
            /// <signature>
            /// <summary>Registers a recipient for a type of message. The action parameter will be executed when a corresponding message is sent.</summary>
            /// <param name="recipient" type="Object">The recipient that will receive the messages.</param>
            /// <param name="type" type="String">A type descriptor.</param>
            /// <param name="token" type="String">A token for a messaging channel. If a recipient registers
            ///     using a token, and a sender sends a message using the same token, then this
            ///     message will be delivered to the recipient. Other recipients who did not
            ///     use a token when registering (or who used a different token) will not
            ///     get the message. Similarly, messages sent without any token, or with a different
            ///     token, will not be delivered to that recipient.</param>
            /// <param name="action" type="Function">The action that will be executed when a message of type is sent.</param>
            /// </signature>
            var argsLen = arguments.length;
            if (argsLen < 3) {
                throw new Error("Recipient, type, and action are required.");
            }
            if (argsLen === 3) {
                action = token;
                token = null;
            }
            if (typeof action !== "function") {
                throw new TypeError("The action is not a function.");
            }
            var obj = {
                recipient: recipient || window,
                token: token,
                action: action
            };
            type = type || "global";
            var list = this._recipients[type];
            if (list) {
                list.push(obj);
            } else {
                this._recipients[type] = [obj];
            }
            return this;
        },
        send: function (type, token, message) {
            /// <signature>
            /// <summary>Sends a message to registered recipients. The message will reach only recipients that registered for this message type using one of the Register methods, and that are of the targetType.</summary>
            /// <param name="type" type="String">A type descriptor.</param>
            /// <param name="message" type="Object">The message to send to registered recipients.</param>
            /// </signature>
            /// <signature>
            /// <summary>Sends a message to registered recipients. The message will reach only recipients that registered for this message type using one of the Register methods, and that are of the targetType.</summary>
            /// <param name="type" type="String">A type descriptor.</param>
            /// <param name="token" type="String">A token for a messaging channel. If a recipient registers
            ///     using a token, and a sender sends a message using the same token, then this
            ///     message will be delivered to the recipient. Other recipients who did not
            ///     use a token when registering (or who used a different token) will not
            ///     get the message. Similarly, messages sent without any token, or with a different
            ///     token, will not be delivered to that recipient.</param>
            /// <param name="message" type="Object">The message to send to registered recipients.</param>
            /// </signature>
            var argsLen = arguments.length;
            if (argsLen < 2) {
                throw new Error("Type and message are required.");
            }
            if (argsLen === 2) {
                message = token;
                token = null;
            }
            var list = this._recipients[type];
            if (list) {
                for (var i = 0, len = list.length; i < len; ++i) {
                    var item = list[i];
                    if (token) {
                        if (item.token === token) {
                            item.action.apply(item.recipient, [message]);
                        }
                    } else {
                        item.action.apply(item.recipient, [message]);
                    }
                }
            }
            return this;
        },
        unregister: function (recipient, type, token, action) {
            /// <signature>
            /// <summary>Unregisters a messager recipient completely. After this method is executed, the recipient will not receive any messages anymore.</summary>
            /// <param name="recipient" type="Object">The recipient that will receive the messages.</param>
            /// <param name="type" type="String">A type descriptor.</param>
            /// <param name="action" type="Function">The action that will be executed when a message of type is sent.</param>
            /// </signature>
            /// <signature>
            /// <summary>Unregisters a messager recipient completely. After this method is executed, the recipient will not receive any messages anymore.</summary>
            /// <param name="recipient" type="Object">The recipient that will receive the messages.</param>
            /// <param name="type" type="String">A type descriptor.</param>
            /// <param name="token" type="String">A token for a messaging channel. If a recipient registers
            ///     using a token, and a sender sends a message using the same token, then this
            ///     message will be delivered to the recipient. Other recipients who did not
            ///     use a token when registering (or who used a different token) will not
            ///     get the message. Similarly, messages sent without any token, or with a different
            ///     token, will not be delivered to that recipient.</param>
            /// <param name="action" type="Function">The action that will be executed when a message of type is sent.</param>
            /// </signature>
            var argsLen = arguments.length;
            if (argsLen < 3) {
                throw new Error("Recipient, type, and action are required.");
            }
            if (argsLen === 3) {
                action = token;
                token = null;
            }
            var list = this._recipients[type || "global"];
            if (!list) {
                return this;
            }
            recipient = recipient || window;
            for (var i = list.length; --i >= 0;) {
                var item = list[i];
                if (item.recipient === recipient &&
                   (!token || item.token === token) &&
                   (!action || item.action === action)) {
                    list.splice(i, 1);
                }
            }
            return this;
        }
    });

    Messenger.instance = window.messenger = new Messenger();

    _WinJS.Namespace.defineWithParent(_WinJS, "Messaging", {
        Messenger: Messenger
    });

})(WinJS);
