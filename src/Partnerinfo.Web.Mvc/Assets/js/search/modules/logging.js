// Copyright (c) Partnerinfo Ltd. All Rights Reserved.
/// <reference path="../engine.js" />

(function (_KO, _WinJS, _PI) {
    "use strict";

    var ns = _WinJS.Namespace.defineWithParent(_PI, "Project.Search", {

        LoggingModule: _WinJS.Class.define(function LoggingModule_ctor(engine) {
            /// <signature>
            /// <param name="engine" type="PI.Project.Search.Engine" />
            /// <returns type="LoggingModule" />
            /// </signature>
            this.engine = engine;
        }, {
            logAction: function (name) {
                /// <signature>
                /// <param name="name" type="String" />
                /// <returns type="$.Deferred" />
                /// </signature>
                var login = this.engine.login;
                var logActionUrl = this.engine.logActionUrl;
                if (!login || !logActionUrl) {
                    return _WinJS.Promise.complete();
                }
                return this.engine.workflow.invokeAsync({
                    name: name,
                    action: logActionUrl,
                    contact: {
                        sponsorId: _KO.unwrap(this.engine.sponsorId),
                        facebookId: _KO.unwrap(login.facebookId),
                        email: {
                            address: _KO.unwrap(login.email),
                            name: _KO.unwrap(login.name),
                        },
                        avatarLink: _KO.unwrap(login.avatarLink)
                    },
                    redirect: false
                }, this);
            }
        })

    });

})(ko, WinJS, PI);