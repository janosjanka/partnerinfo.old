// Copyright (c) Partnerinfo TV. All Rights Reserved.
/// <reference path="context.js" />
/// <reference path="analytics.js" />

(function (_WinJS) {
    "use strict";

    var menuType = {
        none: 0,
        main: 1 << 0,
        quick: 1 << 1
    };

    function createWidgetName(moduleName) {
        /// <signature>
        /// <summary>Creates a name for the jQuery widget.</summary>
        /// <param name="moduleName" type="String" />
        /// <returns type="String" />
        /// </signature>
        return "Portal" + moduleName.substr(0, 1).toUpperCase() + moduleName.substr(1) + "Module";
    }

    var ns = _WinJS.Namespace.define("PI.Portal", {
        /// <field>
        /// Defines module state.
        /// </field>
        ModuleState: {
            edit: 1,
            view: 2,
            active: 3,
            inactive: 4
        },

        /// <field>
        /// Defines module permission levels.
        /// </field>
        ModulePerms: {
            none: 0,
            everyone: 1,
            impersonated: 2,
            authenticated: 3,
            unauthenticated: 4
        },

        /// <field>
        /// Defines module attributes.
        /// </field>
        ModuleAttributes: {
            options: "data-module-options"
        },

        /// <field>
        /// Defines module CSS classes.
        /// </field>
        ModuleClasses: {
            cover: "ui-module-cover",
            event: "ui-module-event",
            content: "ui-module-content",
            editMode: "ui-module-mode-edit",
            module: "ui-module",
            selected: "ui-module-state-selected",
            unauthorized: "ui-module-unauthorized"
        },

        /// <field>
        /// Used to set a display mode for windows (tool windows, etc.).
        /// </field>
        WinDisplay: {
            /// <field type="Number" integer="true">
            /// Represents a closed window.
            /// </field>
            none: 0,

            /// <field type="Number" integer="true">
            /// Represents an opaque window.
            /// </field>
            opaque: 1,

            /// <field type="Number" integer="true">
            /// Represents a transparent window.
            /// </field>
            transparent: 2
        },
        /*
        WinMessages: {
            /// <field type="Number" integer="true">
            /// Posted to the window when a page is rendered and ready to use.
            /// </field>
            documentReady: 1,

            /// <field type="Number" integer="true">
            /// Posted to the window when a new module is inserted.
            /// </field>
            moduleInserted: 100,

            /// <field type="Number" integer="true">
            /// Posted to the window when a new module is inserted.
            /// </field>
            moduleDeleted: 101,

            /// <field type="Number" integer="true">
            /// Posted to the window when a module attribute changes (id, style, class, etc.).
            /// </field>
            moduleAttrChanged: 102,

            /// <field type="Number" integer="true">
            /// Posted to the window when the style of a module changes.
            /// </field>
            moduleStyleChanged: 103
        },
        */
        /// <field>
        /// Represents menu type flags.
        /// </field>
        MenuType: menuType,

        /// <field type="Array">
        /// An array of the built-in module types.
        /// </field>
        ModuleTypes: [{
            resourceKey: "portal.modules.events",
            className: "ui-module-event",
            typeName: "event",
            module: "PortalEventModule",
            icon: "i portal module-event",
            editor: { require: ["codemirror", "codemirror.css"] }
        }, {
            resourceKey: "portal.modules.content",
            className: "ui-module-content",
            typeName: "content",
            module: "PortalContentModule",
            icon: "i portal module-content",
            menu: menuType.main
        }, {
            resourceKey: "portal.modules.panel",
            className: "ui-module-panel",
            typeName: "panel",
            module: "PortalPanelModule",
            icon: "i portal module-panel",
            menu: menuType.main
        }, {
            resourceKey: "portal.modules.animatedpanel",
            className: "ui-module-animatedpanel",
            typeName: "animatedpanel",
            module: "PortalAnimatedPanelModule",
            icon: "i portal module-carousel",
            menu: menuType.main
        }, {
            resourceKey: "portal.modules.search",
            className: "ui-module-search",
            typeName: "search",
            module: "PortalSearchModule",
            icon: "i portal module-search",
            menu: menuType.main
        }, {
            resourceKey: "portal.modules.scroller",
            className: "ui-module-scroller",
            typeName: "scroller",
            module: "PortalScrollerModule",
            icon: "i portal module-scroller",
            menu: menuType.main
        }, {
            resourceKey: "portal.modules.html",
            className: "ui-module-html",
            typeName: "html",
            module: "PortalHtmlModule",
            icon: "i portal module-html",
            editor: { require: ["tinymce", "tinymce.css"] },
            menu: menuType.main
        }, {
            resourceKey: "portal.modules.image",
            className: "ui-module-image",
            typeName: "image",
            module: "PortalImageModule",
            icon: "i portal module-image",
            menu: menuType.main
        }, {
            resourceKey: "portal.modules.link",
            className: "ui-module-link",
            typeName: "link",
            module: "PortalLinkModule",
            icon: "i portal module-link",
            menu: menuType.main
        }, {
            resourceKey: "portal.modules.video",
            className: "ui-module-video",
            typeName: "video",
            module: "PortalVideoModule",
            icon: "i portal module-video",
            menu: menuType.main
        }, {
            resourceKey: "portal.modules.form",
            className: "ui-module-form",
            typeName: "form",
            module: "PortalFormModule",
            icon: "i portal module-form",
            editor: { require: ["tinymce", "tinymce.css"] },
            menu: menuType.main
        }, {
            resourceKey: "portal.modules.timer",
            className: "ui-module-timer",
            typeName: "timer",
            module: "PortalTimerModule",
            icon: "i portal module-timer",
            menu: menuType.main
        }, {
            resourceKey: "portal.modules.frame",
            className: "ui-module-frame",
            typeName: "frame",
            module: "PortalFrameModule",
            icon: "i portal module-frame",
            menu: menuType.main
        }, {
            resourceKey: "portal.modules.chat",
            className: "ui-module-chat",
            typeName: "chat",
            module: "PortalChatModule",
            icon: "i portal module-chat",
            menu: menuType.main,
            lazy: true
        }],

        Modules: {
            register: function (moduleName, options) {
                /// <signature>
                /// <summary>Registers a portal module with the specified name.</summary>
                /// <param name="moduleName" type="String" />
                /// <param name="options" type="Object" optional="true" />
                /// </signature>
                var normalizedName = moduleName.toLowerCase(); // animatedPanel => animatedpanel

                options = options || {};
                options.options = options.options || {};
                options.options.typeName = normalizedName;
                options.options.typeClass = ns.ModuleClasses.module + "-" + normalizedName;

                return $.widget("PI." + createWidgetName(moduleName), $.PI.PortalModule, options);
            },
            extend: function (moduleName, options) {
                /// <signature>
                /// <summary>Registers a portal module with the specified name.</summary>
                /// </signature>
                var widgetName = createWidgetName(moduleName);
                return $.widget("PI." + widgetName, $.PI[widgetName], options);
            }
        }
    });

})(WinJS);