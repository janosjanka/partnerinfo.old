// Copyright (c) Partnerinfo Ltd. All Rights Reserved.
/// <reference path="../base.js" />

(function ($, _KO, _WinJS, _PI) {
    "use strict";

    var _Class = _WinJS.Class;
    var _Utilities = _WinJS.Utilities;
    var _Promise = _WinJS.Promise;
    var _Knockout = _WinJS.Knockout;
    var _observable = _KO.observable;

    var namespace = "Portal.Chat";
    var ns = _WinJS.Namespace.defineWithParent(_PI, namespace, {
        ChatState: {
            chatExpanded: 1,
            chatCollapsed: 2,
            userTyping: 3
        },

        ChatService: _Class.define(function ChatService_ctor(connection, options) {
            /// <signature>
            /// <summary>Initializes a new instance of the ChatService class.</summary>
            /// <param name="connection" type="$.hubConnection">The connection object.</param>
            /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the ChatService.</param>
            /// <returns type="ChatService" />
            /// </signature>
            options = options || {};
            options.proxyName = options.proxyName || "chat";
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
            },
            notifyStateChangedAsync: function (state, userName) {
                /// <signature>
                /// <summary>Sends a message to the given user.</summary>
                /// <param name="state" type="Number" />
                /// <param name="userName" type="String" optional="true" />
                /// <returns type="$.Deferred" />
                /// </signature>
                return this._proxy.invoke("NotifyStateChanged", state, userName);
            },
            getMessagesAsync: function (userName) {
                /// <signature>
                /// <summary>Gets an array of messages for the given user.</summary>
                /// <param name="userName" type="String" />
                /// <returns type="$.Deferred" />
                /// </signature>
                return this._proxy.invoke("GetMessages", userName);
            },
            sendMessageAsync: function (userName, message) {
                /// <signature>
                /// <summary>Sends a message to the given user.</summary>
                /// <param name="userName" type="String" />
                /// <param name="message" type="String" />
                /// <returns type="$.Deferred" />
                /// </signature>
                return this._proxy.invoke("SendMessage", userName, message);
            }
        }),

        ChatMessage: _Class.define(function ChatMessage_ctor(engine, message) {
            /// <signature>
            /// <summary>Initializes a new instance of the ChatMessage class.</summary>
            /// <param name="engine" type="PI.Portal.Chat.ChatEngine" />
            /// <param name="message" type="Object" optional="true">The set of options to be applied initially to the ChatMessage.</param>
            /// <returns type="ChatMessage" />
            /// </signature>
            message = message || {};
            this.engine = engine;
            this.from = message.from;
            this.message = message.message;
            this.createdDate = new Date(message.createdDate);
            this.createdDateText = this.createdDate.elapsed(this.engine.timeStamp).toString();

            if (this.from.admin) {
                this.from.nickName = this.engine.adminNickName;
            } else if (this.engine.me && this.engine.me.equals(this.from)) {
                this.from.nickName = String.format(this.engine.userNickName, this.from.nickName);
            }
        }),

        ChatMessageList: _Class.derive(_Knockout.List, function ChatMessageList_ctor(engine, options) {
            /// <signature>
            /// <summary>Initializes a new instance of the ChatMessageList class.</summary>
            /// <param name="engine" type="PI.Portal.Chat.ChatEngine" />
            /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the ChatMessageList.</param>
            /// <returns type="ChatMessageList" />
            /// </signature>
            options = options || {};
            this.engine = engine;
            this.unread = _observable(0);
            _Knockout.List.apply(this, [options]);
        }, {
            mapItem: function (item) {
                /// <signature>
                /// <summary>Represents a function called before adding a new item to the list.</summary>
                /// <param name="item" type="Object" />
                /// <returns type="Object" />
                /// </signature>
                return new ns.ChatMessage(this.engine, item);
            }
        }),

        ChatUser: _Class.define(function ChatUser_ctor(engine, user) {
            /// <signature>
            /// <summary>Initializes a new instance of the ChatUser class.</summary>
            /// <param name="engine" type="PI.Portal.Chat.ChatEngine" />
            /// <param name="user" type="Object" optional="true">The set of options to be applied initially to the ChatUser.</param>
            /// <returns type="ChatUser" />
            /// </signature>
            user = user || {};
            this._disposed = false;
            this._loadPromise = null;
            this._typingPromise = null;
            this._messageSent = false;

            this.engine = engine;
            this.userName = user.userName;
            this.nickName = user.nickName;
            this.admin = !!user.admin;
            this.messages = new ns.ChatMessageList(engine);
            this.message = _observable();
            this.chatCollapsed = _observable(true);
            this.typing = _observable(false);
            this.typingMessage = _KO.pureComputed(this._getTypingMessage, this);
            this._onMessageChangedDebounced = _Utilities.debounce(this._onMessageChanged, this.engine.typingSignalTimeOffs);
            this._onTypingChangedDebounced = _Utilities.debounce(this._onTypingChanged, this.engine.typingSignalDuration);
            this._messageSn = this.message.subscribe(this._onMessageChangedDebounced, this);
            this._typingSn = this.typing.subscribe(this._onTypingChangedDebounced, this);
        }, {
            initialize: function () {
                /// <signature>
                /// <summary>Executes initialization logic for the user.</summary>
                /// </signature>
                return this._loadPromise = this._loadPromise || this.loadMessagesAsync();
            },
            equals: function (otherUser) {
                /// <signature>
                /// <summary>Checks whether the two user objects are the same.</summary>
                /// <param name="otherUser" type="PI.Portal.Chat.ChatUser" optional="true" />
                /// <returns type="Boolean" />
                /// </signature>
                if (!otherUser) {
                    return false;
                }
                if (this === otherUser) {
                    return true;
                }
                return this.userName === otherUser.userName;
            },
            loadMessagesAsync: function () {
                /// <signature>
                /// <summary>Loads all messages for the given user.</summary>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                var that = this;
                return this.engine.service.getMessagesAsync(this.userName)
                    .then(function (messages) {
                        for (var i = messages.length; --i >= 0;) {
                            that.messages.push(messages[i]);
                        }
                    });
            },
            sendMessageAsync: function () {
                /// <signature>
                /// <summary>Sends the message to this user.</summary>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                var that = this;
                var message = this.message();
                if (!message) {
                    return _Promise.error();
                }
                this._messageSent = true;
                return this.engine.service.sendMessageAsync(this.userName, message)
                    .then(function () {
                        that.message(null);
                    });
            },
            _onMessageChanged: function () {
                /// <signature>
                /// <summary>Raised immediately after the message changes.</summary>
                /// </signature>
                if (this._messageSent ||
                    this._typingPromise && this._typingPromise.state() === "pending") {
                    this._messageSent = false;
                    return;
                }
                this._typingPromise =
                    this.engine.service.notifyStateChangedAsync(ns.ChatState.userTyping, this.userName);
            },
            _onTypingChanged: function (value) {
                /// <signature>
                /// <summary>Raised when this user is typing.</summary>
                /// </signature>
                value && this.typing(false);
            },
            _getTypingMessage: function () {
                /// <signature>
                /// <summary>Gets a message that can be displayed when a user is typing.</summary>
                /// <returns type="String" />
                /// </signature>
                return String.format(this.engine.typingMessage, this.nickName);
            },
            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                if (this._disposed) {
                    return;
                }
                this._typingSn && this._typingSn.dispose();
                this._typingSn = null;
                this._messageSn && this._messageSn.dispose();
                this._messageSn = null;
                this._disposed = true;
            }
        }),

        ChatUserList: _Class.derive(_Knockout.List, function ChatUserList_ctor(engine, options) {
            /// <signature>
            /// <summary>Initializes a new instance of the ChatUserList class.</summary>
            /// <param name="engine" type="PI.Portal.Chat.ChatEngine" />
            /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the ChatUserList.</param>
            /// <returns type="ChatUserList" />
            /// </signature>
            options = options || {};
            options.ownsItems = true; // Dispose user objects after removing them
            this.engine = engine;
            _Knockout.List.apply(this, [options]);
        }, {
            mapItem: function (item) {
                /// <signature>
                /// <summary>Represents a function called before adding a new item to the list.</summary>
                /// <param name="item" type="Object" />
                /// <returns type="Object" />
                /// </signature>
                return new ns.ChatUser(this.engine, item);
            },
            currentEquals: function (otherUser) {
                /// <signature>
                /// <summary>Checks whether the two user objects are the same.</summary>
                /// <param name="otherUser" type="PI.Portal.Chat.ChatUser" optional="true" />
                /// <returns type="Boolean" />
                /// </signature>
                var current = this.current();
                if (!current) {
                    return false;
                }
                return current.equals(otherUser);
            },
            findByUserName: function (userName) {
                /// <signature>
                /// <summary>Finds a user by ID.</summary>
                /// <param name="userName" type="String" />
                /// <returns type="Object" />
                /// </signature>
                return this.find(function (it) {
                    return it.userName === userName;
                });
            },
            push: function () {
                /// <signature>
                /// <summary>
                /// Appends new element(s) to a list, and returns the new length of the list.
                /// </summary>
                /// <param name="value" type="Object" parameterArray="true">The element to insert at the end of the list.</param>
                /// <returns type="Number" integer="true">The new length of the list.</returns>
                /// </signature>
                var others = [];
                var userNames = this.map(function (it) { return it.userName; });
                var me = this.engine.me && this.engine.me.userName;
                for (var i = 0, len = arguments.length; i < len; ++i) {
                    var user = arguments[i];
                    if ((user.userName !== me) && userNames.indexOf(user.userName) < 0) {
                        others.push(user);
                    }
                }
                return _Knockout.List.prototype.push.apply(this, others);
            }
        }),

        ChatEngine: _Class.define(function ChatEngine_ctor(connection, options) {
            /// <signature>
            /// <summary>Initializes a new instance of the ChatEngine class.</summary>
            /// <param name="connection" type="$.hubConnection">The connection object.</param>
            /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the ChatEngine.</param>
            /// <returns type="ChatEngine" />
            /// </signature>
            options = options || {};

            this.timeStamp = new Date();
            this.element = options.element;
            this.service = options.service || new ns.ChatService(connection);
            this.userNickName = options.userNickName || "{0}";
            this.adminNickName = options.adminNickName || "Customer Service";
            this.adminAvatarUrl = options.adminAvatarUrl;
            this.loginSoundEnabled = options.loginSoundEnabled !== false;
            this.loginSoundUrl = options.loginSoundUrl;
            this.loginSoundUrl && (_PI.Sound.register("chat-login", this.loginSoundUrl));
            this.messageSoundEnabled = options.messageSoundEnabled !== false;
            this.messageSoundUrl = options.messageSoundUrl;
            this.messageSoundUrl && (_PI.Sound.register("chat-message", this.messageSoundUrl));
            this.messageInputText = options.messageInputText || "Chat message";
            this.messageSubmitText = options.messageSubmitText || "Send";

            this.typingMessage = options.typingMessage || "{0} is typing...";
            this.typingSignalTimeOffs = options.typingSignalTimeOffs || 1000;
            this.typingSignalDuration = options.typingSignalDuration || 5000;

            this.messageListStyle = { background: this.adminAvatarUrl ? String.format("url({0}) no-repeat bottom right", this.adminAvatarUrl) : null };

            this.me = null;
            this.error = _observable();
            this.users = new ns.ChatUserList(this);
            this.collapsed = _observable(!!options.collapsed);

            this._onCurrentUserChangedBound = this._onCurrentUserChanged.bind(this);
            this._onErrorBound = this._onError.bind(this);
            this._onLoginBound = this._onLogin.bind(this);
            this._onJoinedBound = this._onJoined.bind(this);
            this._onLeftBound = this._onLeft.bind(this);
            this._onMessageReceivedBound = this._onMessageReceived.bind(this);
            this._onStateChangedBound = this._onStateChanged.bind(this);

            this._on();
        }, {
            _onCurrentUserChanged: function (event) {
                /// <signature>
                /// <summary>Raised immediately after the current user changes.</summary>
                /// <param name="event" type="Event" />
                /// </signature>
                var currentUser = event.detail.newItem;
                if (currentUser) {
                    var that = this;
                    // currentUser.messages.unread(0);
                    currentUser.initialize().then(function () {
                        currentUser.messages.items().length && that.collapsed(false);
                        // Fix: look forward to finishing rendering
                        async(function () {
                            that.scrollToLast();
                        });
                    });
                }
                this.scrollToLast();
            },
            _onCollapsed: function (value) {
                /// <signature>
                /// <summary>Raised when the chat is collapsed/expanded.</summary>
                /// <param name="value" type="Boolean" />
                /// </signature>
                if (!this.me) {
                    return;
                }
                var other = this.users.current();
                if (other && other.admin) {
                    this.service.notifyStateChangedAsync(value
                        ? ns.ChatState.chatCollapsed
                        : ns.ChatState.chatExpanded);
                }
            },
            _onError: function (error) {
                /// <signature>
                /// <summary>Raised when an error is occured.</summary>
                /// <param name="error" type="String" />
                /// </signature>
                var message = error && error.errorMessage || error;
                $.WinJS.dialog({ content: message });
            },
            _onLogin: function (me, others) {
                /// <signature>
                /// <summary>Raised immediately after a user is logged in.</summary>
                /// <param name="me" type="Object" />
                /// <param name="others" type="Array" optional="true" />
                /// </signature>
                this.me = new ns.ChatUser(this, me);
                //if (this.userNickName) {
                //    this.me.nickName = String.format(this.userNickName, this.me.nickName);
                //}
                if (others) {
                    this.users.push.apply(this.users, others);
                    var admin = this.users.find(function (it) { return it.admin; });
                    admin && this.users.moveTo(admin);
                }
            },
            _onJoined: function (user) {
                /// <signature>
                /// <summary>Raised immediately after a user is logged in.</summary>
                /// <param name="user" type="Object" />
                /// </signature>                
                var len = this.users.push(user);
                if (user.admin && !this.users.current()) {
                    this.users.moveToIndex(len - 1);
                }
                this._playLoginSound();
            },
            _onLeft: function (userName) {
                /// <signature>
                /// <summary>Raised immediately after a user is logged out.</summary>
                /// <param name="userName" type="String" />
                /// </signature>
                var user = this.users.findByUserName(userName);
                user && this.users.remove(user);
            },
            _onMessageReceived: function (message) {
                /// <signature>
                /// <summary>Raised immediately after a message is received.</summary>
                /// <param name="message" type="Object" />
                /// </signature>
                var isFrom = this.me.equals(message.from);
                var userTo = isFrom ? message.to : message.from;
                var userPos = -1;
                var user = this.users.find(function (it, i) {
                    if (it.equals(userTo)) {
                        userPos = i;
                        return true;
                    }
                    return false;
                });
                if (userPos === -1) {
                    this.users.splice(0, 0, userTo);
                    user = this.users.getAt(0);
                } else if (userPos > 0) {
                    this.users.move(userPos, 0);
                }
                if (user) {
                    user.messages.push(message);
                    user.messages.unread(this.users.currentEquals(user) ? 0 : (user.messages.unread() + 1));
                }
                if (!isFrom) {
                    this._playMessageSound();
                    if (user.admin) {
                        this.collapsed(false);
                    }
                }
                this.scrollToLast();
            },
            _onStateChanged: function (userName, state) {
                /// <signature>
                /// <summary>Raised when a user state changes.</summary>
                /// <param name="userName" type="String" />
                /// <param name="state" type="Number" />
                /// </signature>
                var user = this.users.findByUserName(userName);
                if (!user) {
                    return;
                }
                switch (state) {
                    case ns.ChatState.chatCollapsed:
                        user.chatCollapsed(true);
                        break;
                    case ns.ChatState.chatExpanded:
                        user.chatCollapsed(false);
                        break;
                    case ns.ChatState.userTyping:
                        user.typing(true);
                        break;
                }
            },
            _on: function () {
                /// <signature>
                /// <summary>Creates proxy event handlers for the given Hub events.</summary>
                /// </signature>
                this.service.on("error", this._onErrorBound);
                this.service.on("login", this._onLoginBound);
                this.service.on("joined", this._onJoinedBound);
                this.service.on("left", this._onLeftBound);
                this.service.on("statechanged", this._onStateChangedBound);
                this.service.on("messagereceived", this._onMessageReceivedBound);
                this.users.addEventListener("currentchanged", this._onCurrentUserChangedBound, true);
                this._collapsedSn = this.collapsed.subscribe(this._onCollapsed, this);
            },
            _off: function () {
                /// <signature>
                /// <summary>Disconnects the proxy handlers.</summary>
                /// </signature>
                this._collapsedSn && this._collapsedSn.dispose();
                this._collapsedSn = null;
                this.users.removeEventListener("currentchanged", this._onCurrentUserChangedBound, true);
                this.service.off("error", this._onJoinedBound);
                this.service.off("login", this._onLoginBound);
                this.service.off("joined", this._onJoinedBound);
                this.service.off("left", this._onLeftBound);
                this.service.off("statechanged", this._onStateChangedBound);
                this.service.off("messagereceived", this._onMessageReceivedBound);
            },
            _playLoginSound: function () {
                /// <signature>
                /// <summary>Plays sound.</summary>
                /// </signature>
                this.loginSoundEnabled && _PI.Sound.play(this.loginSoundUrl ? "chat-login" : "notify");
            },
            _playMessageSound: function () {
                /// <signature>
                /// <summary>Plays sound.</summary>
                /// </signature>
                this.messageSoundEnabled && _PI.Sound.play(this.messageSoundUrl ? "chat-message" : "notify");
            },
            scrollToLast: function () {
                /// <signature>
                /// <summary>Scrolls to the last message.</summary>
                /// </signature>
                var msgList = $(".ui-module-chat-msglist:first", this.element);
                var msgListBody = $(".ui-listview-body:first", msgList);
                msgList.scrollTop(msgListBody.outerHeight(true));
            },
            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                this._off();
            }
        }),

        ChatFactory: _Class.define(null, null, {
            createAsync: function (options) {
                /// <signature>
                /// <summary>Establishes a connection between the current client and the server.</summary>
                /// <param name="options" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _Promise(function (completeDispatch, errorDispatch) {
                    var connection = $.hubConnection("/signalr", {
                        useDefaultPath: false,
                        qs: {
                            "pl": options.portalUri,
                            "pg": options.pageUri,
                            "ai": options.anonymId,
                            "un": options.userName,
                            "an": options.adminNickName
                        }
                    });
                    // connection.logging = !!WinJS.DEBUG;
                    // 1. Register all the event handlers before starting.
                    var engine = new ns.ChatEngine(connection, options);
                    // 2. Now you can start the connection.
                    connection.start().then(
                        function () {
                            completeDispatch(engine);
                        },
                        function (error) {
                            errorDispatch(error);
                        });
                });
            }
        })
    });

    PI.Portal.Modules.register("chat", {
        createModuleOptions: function (options) {
            /// <signature>
            /// <param name="options" type="Object" optional="true" />
            /// <returns type="Object" />
            /// </signature>
            return $.extend(this._super(options), {
                chatTheme: "customer",
                userNickName: "{0}",
                adminNickName: "Ügyfélszolgálat",
                adminAvatarUrl: null,
                loginSoundEnabled: true,
                loginSoundUrl: null,
                messageSoundEnabled: true,
                messageSoundUrl: null,
                messageInputText: "Chat üzenet",
                messageSubmitText: null,
                typingMessage: "{0} éppen ír...",
                typingSignalTimeOffs: 1000,
                typingSignalDuration: 5000
            }, options);
        },
        _activate: function () {
            /// <signature>
            /// <summary>Activates this module.</summary>
            /// </signature>
            if (this._isInDesignTime || !this.options.engine.portal) {
                return;
            }
            var that = this;
            var engine = this.options.engine;
            var identity = engine.security.identity();
            var options = this.getModuleOptions();
            options.chatTheme && engine.context.require(this.options.typeClass, [options.chatTheme + ".css"]);
            return ns.ChatFactory.createAsync({
                element: that.element,
                collapsed: true,
                anonymId: engine.security.getAnonymId(),
                portalUri: engine.portal.uri,
                pageUri: engine.page.uri,
                userName: identity && identity.email.address,
                userNickName: options.userNickName,
                adminNickName: options.adminNickName,
                adminAvatarUrl: options.adminAvatarUrl,
                loginSoundEnabled: options.loginSoundEnabled,
                loginSoundUrl: options.loginSoundUrl,
                messageSoundEnabled: options.messageSoundEnabled,
                messageSoundUrl: options.messageSoundUrl,
                messageInputText: options.messageInputText,
                messageSubmitText: options.messageSubmitText,
                typingMessage: options.typingMessage,
                typingSignalTimeOffs: options.typingSignalTimeOffs,
                typingSignalDuration: options.typingSignalDuration
            }).then(function (chat) {
                that._chat = chat;
                that._render(chat);
            });
        },
        _deactivate: function () {
            /// <signature>
            /// <summary>Deactivates this module.</summary>
            /// </signature>
            _KO.cleanNode(this.element[0]);
            this._chat && this._chat.dispose();
            this._chat = null;
            this.element.empty();
        },
        _render: function (chat) {
            /// <signature>
            /// <param name="chat" type="PI.Portal.Chat.ChatEngine" />
            /// </signature>
            this.element.attr("data-bind", 'css: { "ui-module-chat-active": users.current, "ui-module-chat-collapsed": collapsed  }');
            this.element.html(
                '<div class="ui-module-chat-head" data-bind=\'click: function (d) { d.collapsed(!d.collapsed()); }\'>' +
                    '<span class="ui-module-chat-state"></span>' +
                    '<span class="ui-module-chat-title" data-bind=\'text: users.current() ? users.current().nickName : "Chat"\'></span>' +
                    '<button class="ui-module-chat-collapse" type="button" data-bind=\'visible: !collapsed(), click: function (d) { d.collapsed(true); }, clickBubble: false\'>x</button>' +
                '</div>' +
                '<div class="ui-module-chat-body">' +
                    '<div class="ui-module-chat-userbox-container">' +
                        '<div class="ui-module-chat-userbox">' +
                            '<div class="ui-module-chat-usersearch-container">' +
                                '<div class="ui-module-chat-usersearch"></div>' +
                            '</div>' +
                            '<div class="ui-module-chat-userlist-container">' +
                                '<div class="ui-module-chat-userlist" data-bind=\'listView: {' +
                                    'items: users,' +
                                    'synchronize: true,' +
                                    'displayHeader: false,' +
                                    'displaySelectionCheckbox: false,' +
                                    'group: {' +
                                        'binding: "css: {' +
                                            '\\"ui-module-chat-userchatexp\\": !chatCollapsed(),' +
                                            '\\"ui-module-chat-usermsgsunread\\": messages.unread,' +
                                            '\\"ui-module-chat-usertyping\\": typing' +
                                        '}",' +
                                        'bodyClass: "ui-module-chat-user"' +
                                    '},' +
                                    'columns: [' +
                                        '{ cellClass: "ui-module-chat-username", binding: "text: nickName" },' +
                                        '{ cellClass: "ui-module-chat-userstate" },' +
                                        '{ cellClass: "ui-module-chat-usermsgs", binding: "text: messages.unread" }' +
                                    ']' +
                                '}\'></div>' +
                            '</div>' +
                        '</div>' +
                    '</div>' +
                    '<div class="ui-module-chat-msgbox-container">' +
                        '<div class="ui-module-chat-msgbox" data-bind="with: users.current">' +
                            '<div class="ui-module-chat-msglist-container">' +
                                '<div class="ui-module-chat-msglist" data-bind=\'listView: {' +
                                    'items: messages,' +
                                    'displayHeader: false,' +
                                    'displaySelectionCheckbox: false,' +
                                    'selectionMode: "none",' +
                                    'group: { bodyClass: "ui-module-chat-msg" },' +
                                    'columns: [' +
                                        '{ cellClass: "ui-module-chat-msguser", binding: "text: from.nickName" },' +
                                        '{ cellClass: "ui-module-chat-msgtext", binding: "text: message, attr: { title: createdDateText }" }' +
                                    ']' +
                                '}, style: $root.messageListStyle\'></div>' +
                            '</div>' +
                            '<div class="ui-module-chat-msgstate" data-bind=\'css: { "ui-module-chat-usertyping": typing }, text: typingMessage\'></div>' +
                            '<div class="ui-module-chat-msgform-container">' +
                                '<form class="ui-module-chat-msgform">' +
                                    '<textarea class="ui-module-chat-msginput" data-bind="value: message, valueUpdate: \'keydown\', attr: { placeholder: $root.messageInputText }, event: { \'keydown\': function (d, e) { if (!e.shiftKey && e.which === 13) { d.sendMessageAsync(); return false; } return true; } }, hasFocus: true"></textarea>' +
                                    '<button class="ui-module-chat-msgsubmit" type="submit" data-bind=\'text: $root.messageSubmitText\'></button>' +
                                '</form>' +
                            '</div>' +
                        '</div>' +
                    '</div>' +
                '</div>'
            );
            _KO.applyBindings(chat, this.element[0]);
        }
    });

})(jQuery, ko, WinJS, PI);
