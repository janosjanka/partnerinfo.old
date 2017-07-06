// Copyright (c) Partnerinfo TV. All Rights Reserved.

(function (_WinJS) {
    "use strict";

    function formatProjectRoute() {
        /// <signature>
        /// <param name="relativePath" type="String" />
        /// <param name="projectId" type="Number" />
        /// <returns type="String" />
        /// </signature>
        var args = Array.prototype.slice.apply(arguments, [0]);
        args[0] = "/admin/projects/{0}/#/" + args[0];
        return String.format.apply(String, args);
    }
    function formatPortalRoute() {
        /// <signature>
        /// <param name="relativePath" type="String" />
        /// <param name="projectId" type="Number" />
        /// <returns type="String" />
        /// </signature>
        var args = Array.prototype.slice.apply(arguments, [0]);
        args[0] = "/admin/portals/{0}/#/" + args[0];
        return String.format.apply(String, args);
    }

    _WinJS.Namespace.define("PI.Routes", {
        Drive: {
            FileStore: {
                temp: function () {}
            }
        },
        Project: {
            Logging: {
                list: function (projectId) { return formatProjectRoute("events", projectId); }
            },
            Actions: {
                create: function (projectId) { return formatProjectRoute("actions/create", projectId); },
                list: function (projectId) { return formatProjectRoute("actions", projectId); },
                item: function (projectId, id) { return formatProjectRoute("actions/{1}", projectId, id); }
            },
            Contacts: {
                create: function (projectId) { return formatProjectRoute("contacts/create", projectId); },
                list: function (projectId) { return formatProjectRoute("contacts", projectId); },
                item: function (projectId, id) { return formatProjectRoute("contacts/{1}", projectId, id); }
            },
            MailMessages: {
                create: function (projectId) { return formatProjectRoute("mails/compose", projectId); },
                list: function (projectId) { return formatProjectRoute("mails", projectId); },
                item: function (projectId, id) { return formatProjectRoute("mails/{1}/compose", projectId, id); }
            }
        },
        Portal: {
            Sites: {
                pages: function (portalUri) { return formatPortalRoute("pages", portalUri); }
            },
            Pages: {
                preview: function (portalUri, pageUri) { return String.format("/{0}/{1}?preview=true", portalUri, pageUri); },
                designer: function (portalUri, pageUri) { return formatPortalRoute("designer/{1}", portalUri, pageUri); }
            }
        }
    });

})(WinJS);