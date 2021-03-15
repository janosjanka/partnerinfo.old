// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="../viewmodels/contact.js" />

(function (_Promise, PI, Project) {
    "use strict";

    PI.component({
        name: "project.contact",
        model: function (options) {
            /// <signature>
            /// <param name="options" type="Object" />
            /// <returns type="PI.Project.Contact" />
            /// </signature>
            var params = options.params || {};
            var contact = new Project.Contact(params.project, params.contact, params);
            contact.exists() ? contact.loadAsync() : contact.beginEdit();
            return contact;
        },
        view: function (model, options) {
            /// <signature>
            /// <param name="model" type="PI.Project.Contact" />
            /// <param name="options" type="Object" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            return _Promise.join(
                this.render(options.element, model, "koContactItem"),
                this.render(options.menu, model, "koContactMenu"));
        },
        dialog: function (model, options, response, done) {
            /// <signature>
            /// <param name="model" type="PI.Project.Contact" />
            /// <param name="options" type="Object" />
            /// <param name="response" type="Object" />
            /// <param name="done" type="Function" />
            /// <returns type="$.WinJS.dialog" />
            /// </signature>
            return $.WinJS.dialog({
                width: 500,
                //position: { my: "center top", at: "center top" },
                title: _T("contact"),
                buttons: [{
                    "class": "ui-btn ui-btn-primary",
                    "text": _T("ui/done"),
                    "click": function () {
                        model.saveAsync().then(
                            function () {
                                response.contact = model.toObject();
                                done.call(model, "ok");
                            });
                    }
                }, {
                    "class": "ui-btn",
                    "text": _T("ui/close"),
                    "click": done.bind(model, "cancel")
                }]
            });
        }
    });

    PI.component({
        name: "project.contacts",
        model: function (options) {
            /// <signature>
            /// <param name="options" type="Object" />
            /// <returns type="PI.Project.ContactList" />
            /// </signature>
            var params = options.params || {};
            params.filter = params.filter || {};
            params.filter.orderBy = params.filter.orderBy || Project.ContactSortOrder.recent;
            params.filter.fields = params.filter.fields || (Project.ContactField.sponsor | Project.ContactField.businessTags);
            return new Project.ContactList(options.params);
        },
        view: function (model, options) {
            /// <signature>
            /// <param name="model" type="PI.Project.ContactList" />
            /// <param name="options" type="Object" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            return _Promise.join(
                this.render(options.element, model, "koContactList"),
                this.render(options.menubar, model, "koContactListMenu"));
        },
        dialog: function (model, options, response, done) {
            /// <signature>
            /// <param name="model" type="PI.Project.ContactList" />
            /// <param name="options" type="Object" />
            /// <param name="response" type="Object" />
            /// <param name="done" type="Function" />
            /// <returns type="$.WinJS.dialog" />
            /// </signature>
            return $.WinJS.dialog({
                width: 980,
                height: 480,
                position: { my: "center top", at: "center top" },
                title: _T("contacts"),
                buttons: null
            });
        }
    });

})(WinJS.Promise, PI, PI.Project);
