// Copyright (c) Partnerinfo TV. All Rights Reserved.

/// <reference path="common.js" />
/// <reference path="context.js" />
/// <reference path="analytics.js" />

(function (_WinJS, _PI, _Project, _Portal) {
    "use strict";

    var ns = _WinJS.Namespace.defineWithParent(_Portal, null, {
        Engine: _WinJS.Class.define(function (options) {
            /// <signature>
            /// <param name="options" type="Object">A set of key/value pairs that can be used to configure UI logic.</param>
            /// <returns type="PI.Portal.Engine" />
            /// </signature>
            options = options || {};

            this._disposed = false;
            this._onLoginBound = this._onLogin.bind(this);
            this._onLogoutBound = this._onLogout.bind(this);

            this.url = _WinJS.Utilities.parseLink();
            this.registerBuiltInModules = options.registerBuiltInModules !== false;
            this.autoParse = options.autoParse !== false;

            var urlSegment = this.url.segment() || [];

            this.modulePath = options.modulePath || "/js/portal/modules";
            this.security = options.security || _Project.Security;
            this.security.setIdentity(options.identity, options.identityToken);
            this.workflow = options.workflow || _Project.Workflow;
            this.portalService = options.portalService || _Portal.PortalService;
            this.pageService = options.pageService || _Portal.PageService;
            this.messenger = options.messenger || new _WinJS.Messaging.Messenger();
            this.context = options.context || new ns.ModuleContext({ container: document.body });
            this.context._engine = this;
            this.watcher = new ns.Analytics.Watcher(this);
            this.liveEdit = options.liveEdit || (urlSegment[urlSegment.length - 1] === "info");
            this.liveEditEmail = options.liveEditEmail || "";

            this.portal = options.portal;
            this.page = options.page;

            options.anonymId && this.security.setAnonymId(options.anonymId);
            options.event && this.security.setEventId(options.event.id);

            this.registerBuiltInModules && this.context.register(ns.ModuleTypes);
            this.autoParse && this.context.parseAll({ mode: ns.ModuleState.active, liveEdit: this.liveEdit });
            this.security.addEventListener("login", this._onLoginBound, true);
            this.security.addEventListener("logout", this._onLogoutBound, true);
        }, {
            portal: {
                get: function () {
                    return this._portal;
                },
                set: function (value) {
                    this._portal = value;
                    this.watcher.enabled = !!value;
                    this.watcher.sendView();
                }
            },
            page: {
                get: function () {
                    return this._page;
                },
                set: function (value) {
                    this._page = value;
                }
            },
            _onLogin: function () {
                /// <signature>
                /// <summary>Raised immediately after a user logs in.</summary>
                /// </signature>
                this.context.authorize();
            },
            _onLogout: function () {
                /// <signature>
                /// <summary>Raised immediately after a user logs out.</summary>
                /// </signature>
                this.context.authorize();
            },
            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                if (this._disposed) {
                    return;
                }
                this.security.removeEventListener("logout", this._onLogoutBound, true);
                this.security.removeEventListener("login", this._onLoginBound, true);
                this._disposed = true;
            }
        }, {
            initialize: function (options) {
                /// <signature>
                /// <summary>Initializes the portal engine.</summary>
                /// </signature>
                ns.Engine.instance = new ns.Engine(options);
            }
        })
    });

})(WinJS, PI, PI.Project, PI.Portal);