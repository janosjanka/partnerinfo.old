// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="../viewmodels/user.js" />

(function (_WinJS, _PI, _Identity) {
    "use strict";

    ko.components.register("identity-user-input", {
        viewModel: _Identity.UserInput,
        template:
            '<input type="email" data-bind="value: value, typeahead: {' +
                'dataSource: {' +
                    'limit: limit,' +
                    'source: onPopulateDebounced.bind($data),' +
                    'display: function (u) { return u.email.address; },' +
                    'templates: {' +
                        'suggestion: function (u) { return \'<p>\' + u.email.name + \' - <span class=\\\'ui-type-mail\\\'>\' + u.email.address + \'<\/span><\/p>\'; }' +
                    '}' +
                '}' +
            '}" />'
    });

    _PI.component({
        name: "identity.users",
        model: function (options) {
            /// <signature>
            /// <param name="options" type="Object" />
            /// <returns type="PI.Identity.UserList" />
            /// </signature>
            return new _Identity.UserList({ autoOpen: true });
        },
        view: function (model, options) {
            /// <signature>
            /// <param name="model" type="PI.Identity.UserList" />
            /// <param name="options" type="Object" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            return this._renderView(options.element, model, "koUserList");
        },
        dialog: function () {
            /// <signature>
            /// <param name="model" type="PI.Identity.UserList" />
            /// <param name="options" type="Object" />
            /// <param name="response" type="Object" />
            /// <param name="callback" type="Function" />
            /// <returns type="$.WinJS.dialog" />
            /// </signature>
            return this._renderDialog({
                width: 750,
                minHeight: 450,
                title: _T("pi/identity/users"),
                buttons: null
            });
        }
    });

})(WinJS, PI, PI.Identity);