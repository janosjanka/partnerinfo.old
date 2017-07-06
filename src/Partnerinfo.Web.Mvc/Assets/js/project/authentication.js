// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (_KO, _WinJS, _PI) {
    "use strict";

    var _Class = _WinJS.Class;
    var _Utilities = _WinJS.Utilities;
    var _Promise = _WinJS.Promise;

    var _observable = _KO.observable;

    var Identity = _Class.define(function Identity_ctor(options, accessToken) {
        /// <signature>
        /// <summary>Initializes a new instance of the Identity class.</summary>
        /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the Identity.</param>
        /// <param name="accessToken" type="String" optional="true" />
        /// <returns type="PI.Project.Identity" />
        /// </signature>
        options = options || {};
        this.id = options.id;
        this.facebookId = options.facebookId;
        this.email = options.email || {};
        this.firstName = options.firstName;
        this.lastName = options.lastName;
        this.gender = options.gender;
        this.birthday = options.birthday;
        this.phones = options.phones || {};
        this.accessToken = accessToken;
        this.avatarLink = options.avatarLink;
    });

    var SecurityManager = _Class.define(function SecurityManager_ctor() {
        /// <signature>
        /// <summary>Initializes a new instance of the Security class.</summary>
        /// </signature>
        this.eventId = null;
        this.anonymId = null;
        this.identity = _observable();
        this.loadIdentity();
    }, {
        loadIdentity: function () {
            /// <signature>
            /// <summary>Loads session identity</summary>
            /// </signature>
            var identity = _PI.globalCache(_PI.Storage.session, "project.identity");
            identity && this.setIdentity(identity, identity.accessToken);
        },
        getEventId: function () {
            /// <signature>
            /// <summary>Returns the Request ID.</summary>
            /// <returns type="String" />
            /// </signature>
            return this.eventId;
        },
        getAnonymId: function () {
            /// <signature>
            /// <summary>Returns the Session ID.</summary>
            /// <returns type="String" />
            /// </signature>
            return this.anonymId;
        },
        getIdentity: function () {
            /// <signature>
            /// <summary>Gets the current identity.</summary>
            /// <returns type="Object" />
            /// </signature>
            return this.identity();
        },
        getToken: function () {
            /// <signature>
            /// <summary>Gets the current user's access token</summary>
            /// <returns type="String" />
            /// </signature>
            var identity = this.identity();
            return identity && identity.accessToken;
        },
        setEventId: function (id) {
            /// <signature>
            /// <summary>Sets a ID which can be used to track the current user.</summary>
            /// <param name="id" type="String" />
            /// </signature>
            this.eventId = id;
            _PI.Logging.EventManager.setEventId(id);
        },
        setAnonymId: function (id) {
            /// <signature>
            /// <summary>Sets a ID which can be used to track the current user.</summary>
            /// <param name="id" type="String" />
            /// </signature>
            this.anonymId = id;
        },
        setIdentity: function (identity, accessToken) {
            /// <signature>
            /// <summary>Sets the current identity.</summary>
            /// <param name="identity" type="Object" />
            /// <param name="accessToken" type="String" />
            /// </signature>
            var oldIdentity = this.identity();
            var newIdentity = identity && new Identity(identity, accessToken) || null;

            _PI.globalCache(_PI.Storage.session, "project.identity", newIdentity);

            this.identity(newIdentity);
            if (newIdentity && newIdentity.accessToken) {
                this.dispatchEvent("login");
            }
            else if (oldIdentity && oldIdentity.accessToken && !accessToken) {
                this.dispatchEvent("logout");
            }
            this.dispatchEvent("changed");
        },
        logout: function () {
            /// <signature>
            /// <summary>Logouts</summary>
            /// </signature>
            this.setIdentity(null, null);
        }
    });

    _Class.mix(SecurityManager, _Utilities.eventMixin);

    var ns = _WinJS.Namespace.defineWithParent(_PI, "Project", {
        /// <field>
        /// <summary>Represents a user identity.</summary>
        /// </field>
        Identity: Identity,

        /// <field>
        /// <summary>Manages security services</summary>
        /// </field>
        Security: new SecurityManager(),

        Workflow: {
            invokeAsync: function (options, thisArg) {
                /// <signature>
                /// <summary>Called by the workflow runtime to execute an activity</summary>
                /// <param name="options" type="Object">
                ///     <para>action: Object - id | url</para>
                ///     <para>contact: Object - { id, email, ... }</para>
                ///     <para>customUri: String</para>
                ///     <para>friends: Array - [{ id, email, ... }]</para>
                ///     <para>redirect: Boolean - false</para>
                /// </param>
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                if (!options || !options.action) {
                    return _Promise.error();
                }
                if (typeof options.action === "number") {
                    return _PI.api({
                        method: "GET",
                        path: "project/actionlinks/" + options.action + "/link",
                        data: {
                            contactId: options.contact && options.contact.id,
                            customUri: options.customUri
                        }
                    }, this).then(function (url) {
                        return this._invokeAsync({
                            action: url,
                            contact: options.contact,
                            invitation: options.invitation,
                            customUri: options.customUri,
                            redirect: options.redirect
                        }, thisArg);
                    });
                }
                return this._invokeAsync(options, thisArg);
            },
            _invokeAsync: function (options, thisArg) {
                /// <signature>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                var headers, model, anonymId, accessToken;

                // Add security headers to the request.
                if ((anonymId = ns.Security.getAnonymId()) || (accessToken = ns.Security.getToken())) {
                    headers = {};
                    anonymId && (headers["AID"] = anonymId);
                    accessToken && (headers["Authorization"] = "Bearer " + accessToken);
                }

                // Add contact and/or invitation request as model object.
                if (options.name || options.contact || options.invitation) {
                    model = {
                        name: options.name,
                        contact: options.contact,
                        invitation: options.invitation
                    };
                }

                return _PI.api({
                    path: options.action,
                    method: "POST",
                    headers: headers,
                    data: model
                }, thisArg).then(
                    function (data) {
                        if (options.redirect && data.locationLink) {
                            window.open(data.locationLink, "_self");
                            return;
                        }
                        if (data.identity) {
                            // At present avatar link is not stored on server-side so we need to
                            // store it on the client using an external provider, such as Facebook.
                            if (model && model.contact) {
                                data.identity.avatarLink = data.identity.avatarLink || model.contact.avatarLink;
                            }
                            ns.Security.setIdentity(data.identity, data.accessToken);
                        }
                    });
            },
            invokeByIdAsync: function (actionLinkId, actionLinkSalt, contact, redirect, thisArg) {
                /// <signature>
                /// <summary>Called by the workflow runtime to execute an activity (DEPRECATED)</summary>
                /// <param name="actionLinkId" type="Number" />
                /// <param name="actionLinkSalt" type="String" optional="true" />
                /// <param name="contact" type="Object" optional="true" />
                /// <param name="redirect" type="Boolean" optional="true" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return this.invokeAsync({ action: actionLinkId, contact: contact, customUri: actionLinkSalt, redirect: redirect }, thisArg);
            },
            invokeByUrlAsync: function (actionUrl, contact, redirect, thisArg) {
                /// <signature>
                /// <summary>Called by the workflow runtime to execute an activity (DEPRECATED)</summary>
                /// <param name="actionUrl" type="String" />
                /// <param name="contact" type="Object" optional="true" />
                /// <param name="redirect" type="Boolean" optional="true" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return this.invokeAsync({ action: actionUrl, contact: contact, redirect: redirect }, thisArg);
            }
        },

        /// <field>
        /// <summary>Represents a service for actions.</summary>
        /// </field>
        ActionService: _Class.define(null, null, {
            executeAsync: function (actionLink, formData, thisArg) {
                /// <signature>
                /// <summary>Executes the action behind the given URL.</summary>
                /// <param name="actionLink" type="String" />
                /// <param name="formData" type="Object" optional="true" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                if (!actionLink) {
                    return _Promise.error();
                }
                return _PI.api({
                    path: actionLink,
                    method: "POST",
                    data: formData
                }, thisArg);
            }
        })
    });

    _Class.mix(SecurityManager, _Utilities.createEventProperties("changed", "login", "logout"));
    _Class.mix(SecurityManager, _Utilities.eventMixin);

})(ko, WinJS, PI);
