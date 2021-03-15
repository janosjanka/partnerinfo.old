// Copyright (c) Partnerinfo Ltd. All Rights Reserved.
/// <reference path="../viewmodels/mail.js" />

(function (WinJS, PI, Project) {
    "use strict";

    PI.component({
        name: "project.mail",
        model: function (options) {
            /// <signature>
            /// <param name="options" type="Object" />
            /// <returns type="PI.Project.MailMessage" />
            /// </signature>
            var params = options.params || {};
            var mailMessage = new Project.MailMessage(params.mailMessage, params);
            mailMessage.exists() && mailMessage.loadAsync();
            return mailMessage;
        },
        view: function (model, options) {
            /// <signature>
            /// <param name="model" type="PI.Project.MailMessage" />
            /// <param name="options" type="Object" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            return WinJS.Promise.join(
                this.render(options.element, model, "koMailMessage"),
                this.render(options.menu, model, "koMailMessageMenu"));
        },
        dialog: function (model, options, response, done) {
            /// <signature>
            /// <param name="model" type="PI.Project.MailMessage" />
            /// <param name="options" type="Object" />
            /// <param name="response" type="Object" />
            /// <param name="done" type="Function" />
            /// <returns type="$.WinJS.dialog" />
            /// </signature>
            return $.WinJS.dialog({
                width: 800,
                buttons: [{
                    "class": "ui-btn ui-btn-primary",
                    "text": _T("ui/close"),
                    "click": done.bind(model, "cancel")
                }]
            });
        }
    });

    PI.component({
        name: "project.mails",
        model: function (options) {
            /// <signature>
            /// <param name="options" type="Object" />
            /// <returns type="PI.Project.MailMessageList" />
            /// </signature>
            return new Project.MailMessageList(options.params);
        },
        view: function (model, options) {
            /// <signature>
            /// <param name="model" type="PI.Project.MailMessageList" />
            /// <param name="options" type="Object" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            return this.render(options.element, model, "koMailMessageList");
        },
        dialog: function (model, options, response, done) {
            /// <signature>
            /// <param name="model" type="PI.Project.MailMessageList" />
            /// <param name="options" type="Object" />
            /// <param name="response" type="Object" />
            /// <param name="done" type="Function" />
            /// <returns type="$.WinJS.dialog" />
            /// </signature>
            return $.WinJS.dialog({
                width: 640,
                buttons: [{
                    "class": "ui-btn ui-btn-primary",
                    "text": _T("ui/close"),
                    "click": done.bind(model, "cancel")
                }]
            });
        }
    });

})(WinJS, PI, PI.Project);
