// Copyright (c) Partnerinfo TV. All Rights Reserved.

/// <reference path="../services/user.js" />

(function (_Global, _KO, _WinJS, _PI) {
    "use strict";

    var _Class = _WinJS.Class;
    var _Utilities = _WinJS.Utilities;
    var _Promise = _WinJS.Promise;
    var _Knockout = _WinJS.Knockout;

    var _observable = _KO.observable;
    var _observableArray = _KO.observableArray;
    var _pureComputed = _KO.pureComputed;

    var ns = _WinJS.Namespace.defineWithParent(_PI, "Identity", {

        UserInput: _Class.define(function UserInput_ctor(options) {
            /// <signature>
            /// <summary>Initializes a new instance of the UserInput class.</summary>
            /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the UserInput.</param>
            /// <returns type="UserInput" />
            /// </signature>
            options = options || {};
            this.service = options.service || ns.UserService;
            this.value = options.value;
            this.limit = options.limit || 10;
            this.delay = options.delay || 500;
            this.query = { name: options.name, limit: this.limit };
            this.onPopulateDebounced = _Utilities.debounce(this.onPopulate, this.delay);
        }, {
            onPopulate: function (query, sync, async) {
                /// <signature>
                /// <param name="query" type="String" />
                /// <param name="sync" type="Function" />
                /// <param name="async" type="Function" />
                /// </signature>
                this.query.name = query;
                this.service.getAllAsync(this.query, this)
                    .then(function (response) {
                        async(response.data);
                    });
            }
        })

    });

})(window, ko, WinJS, PI);


(function (window, ko, WinJS, PI, undefined) {
    "use strict";

    var Class = WinJS.Class,

    User = Class.derive(PI.Entity, function User_ctor(options) {
        /// <signature>
        /// <summary>Initializes a new instance of the User class.</summary>
        /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the User.</param>
        /// <returns type="User" />
        /// </signature>
        options = options || {};
        options.item = User.createItem(options.item);
        options = User.createOptions(options);
        PI.Entity.apply(this, [options]);
    }, {
        authorize: function (/* returnUrl */) {
            /// <signature>
            /// <summary>Authorizes the current user. This is a security critical function that cannot be called without Admin rights.</summary>
            /// <returns type="$.Deferred" />
            /// </signature>
            var d = $.post("/account/switch", {
                id: ko.unwrap(this.item.id)
            });
            if (arguments.length) {
                var returnUrl = arguments[0];
                d.done(function () {
                    window.open(returnUrl || "/", "_self");
                });
            }
            return d;
        }
    }, {
        createOptions: function (options) {
            /// <signature>
            /// <summary>Creates an options object using default options.</summary>
            /// <param name="options" type="Object" optional="true">A set of key/value pairs that can be used to configure options.</param>
            /// <returns type="Object" />
            /// </signature>
            return ko.utils.extend({
                urls: {
                    query: "{id}"
                },
                mapping: {
                    copy: ["owners", "links"],
                    birthday: {
                        create: function (options) {
                            return options.data ? new Date(options.data) : null;
                        }
                    },
                    createDate: {
                        create: function (options) {
                            return options.data ? new Date(options.data) : null;
                        }
                    },
                    updateDate: {
                        create: function (options) {
                            return options.data ? new Date(options.data) : null;
                        }
                    }
                }
            }, options);
        },
        createItem: function (item) {
            /// <signature>
            /// <summary>Creates a native project object.</summary>
            /// <param name="item" type="Object" optional="true">A native JS object to extend.</param>
            /// <returns type="Object" />
            /// </signature>
            return ko.utils.extend({
                id: null,
                email: null,
                firstName: null,
                lastName: null,
                name: null,
                gender: null,
                birthday: null,
                createDate: null,
                updateDate: null
            }, item);
        }
    }),

    UserFilter = Class.derive(PI.EntityFilter, function UserFilterCtor(options) {
        /// <signature>
        /// <summary>Initializes a new instance of the UserFilter class.</summary>
        /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the UserFilter.</param>
        /// <returns type="UserFilter" />
        /// </signature>
        options = options || {};
        PI.EntityFilter.apply(this, [options]);
        this.name = ko.observable(options.name);
    }),

    UserList = Class.derive(PI.EntityCollection, function UserListCtor(options) {
        /// <signature>
        /// <summary>Initializes a new instance of the UserList class.</summary>
        /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the UserList.</param>
        /// <returns type="UserList" />
        /// </signature>
        options = options || {};
        options.cacheKey = options.cacheKey || "user";
        options.filter = options.filter || new UserFilter();
        options.urls = options.urls || { query: "identity/users" };
        PI.EntityCollection.apply(this, [options]);
    }, {
        mapItem: function (item) {
            /// <signature>
            /// <summary>Represents a function called before adding a new item to the collection.</summary>
            /// <param name="item" type="Object">The item to add.</param>
            /// <returns type="Object" />
            /// </signature>
            return new User({ item: item, mapping: null });
        }
    });

    //
    // Public Namespaces & Classes
    //

    WinJS.Namespace.defineWithParent(PI, "Identity", {
        User: User,
        UserFilter: UserFilter,
        UserList: UserList
    });

})(window, ko, WinJS, PI);
