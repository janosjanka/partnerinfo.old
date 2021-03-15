// Copyright (c) Partnerinfo Ltd. All Rights Reserved.
/// <reference path="../engine.js" />

(function (_WinJS, _PI) {
    "use strict";

    var _promise = _WinJS.Promise;
    var _observable = ko.observable;
    var _validation = ko.validation;

    var ns = _WinJS.Namespace.defineWithParent(_PI, "Project.Search", {

        LoginModule: _WinJS.Class.define(function LoginModule_ctor(engine, options) {
            /// <signature>
            /// <param name="engine" type="PI.Project.Search.Engine" />
            /// <returns type="LoginModule" />
            /// </signature>
            this.engine = engine;

            options = options || {};
            this.facebookId = _observable(options.facebookId);
            this.name = _observable(options.name);
            this.email = _observable(options.email);
            this.avatarLink = options.avatarLink;

            this.errors = _validation.group(this);
            this.errorMessage = _observable();
        }, {
            loginAsync: function () {
                /// <signature>
                /// <returns type="$.Deferred" />
                /// </signature>
                if (!this.engine.loginActionUrl || this.errors().length) {
                    return _promise.error();
                }
                return this.engine.workflow.invokeAsync({
                    action: this.engine.loginActionUrl,
                    contact: {
                        sponsorId: this.engine.sponsorId,
                        facebookId: this.facebookId(),
                        email: {
                            address: this.email(),
                            name: this.name()
                        },
                        avatarLink: this.avatarLink
                    },
                    redirect: false
                }, this);
            },
            fbLoginAsync: function () {
                /// <signature>
                /// <returns type="$.Deferred" />
                /// </signature>
                this.errorMessage(null);
                if (typeof FB === "undefined") {
                    this.errorMessage("Facebook API is not available.");
                    return _promise.error();
                }
                var that = this;
                return _promise(function (completeDispatch, errorDispatch) {
                    FB.login(function (response) {
                        if (response.authResponse) {
                            FB.api("/me", { fields: "id,name,email,picture" }, function (response) {
                                if (response.error) {
                                    that.errorMessage("Facebook error: " + response.message);
                                    errorDispatch();
                                    return;
                                }
                                that.facebookId(response.id);
                                that.name(response.name);
                                that.email(response.email);
                                that.avatarLink = response.picture && response.picture.data && response.picture.data.url;
                                that.loginAsync().then(completeDispatch, errorDispatch);
                            });
                        } else {
                            that.errorMessage("Facebook login was failed.")
                            errorDispatch();
                        }
                    }, { scope: "email,public_profile" });
                });
            },
            logout: function () {
                this.engine.user.logout();
            }
        })

    });

})(WinJS, PI);