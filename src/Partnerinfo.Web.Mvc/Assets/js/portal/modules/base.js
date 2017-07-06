// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="../engine.js" />

(function (_WinJS, _Portal) {
    "use strict";

    var modulePerms = _Portal.ModulePerms;
    var moduleState = _Portal.ModuleState;
    var moduleAttributes = _Portal.ModuleAttributes;
    var moduleClasses = _Portal.ModuleClasses;

    $.widget("PI.PortalModule", {
        options: {
            /// <field type="$.PI.PortalDesigner" />
            designer: null,
            /// <field type="PI.Portal.Engine" />
            engine: null,
            /// <field type="String" />
            typeClass: null,
            /// <field type="String" />
            typeName: null,
            /// <field type="Boolean" />
            active: true,
            /// <field type="Number" integer="true" />
            mode: moduleState.active,
            /// <field type="Number" integer="true" />
            perm: modulePerms.everyone,
            /// <field type="Boolean" />
            container: false,
            /// <field type="Boolean" />
            liveEdit: false
        },
        _create: function () {
            /// <signature>
            /// <summary>Initializes a new instance of the page module.</summary>
            /// </signature>
            var moduleOptions = this.getModuleOptions();
            this.options.active = !!moduleOptions.active;
            this.options.perm = moduleOptions.perm === undefined ? modulePerms.everyone : +moduleOptions.perm;
            this.authorize();
            this._setupMode(this.options.mode);
        },
        refresh: function () {
            /// <signature>
            /// <summary>Refreshes this module.</summary>
            /// </signature>
            this.authorize();
        },
        isAuthenticated: function () {
            /// <signature>
            /// <summary>Returns true if the user is logged in.</summary>
            /// <returns type="Boolean" />
            /// </signature>
            var identity = this.options.engine.security.identity();
            return !!(identity && identity.accessToken);
        },
        hasPermission: function () {
            /// <signature>
            /// <summary>Returns true if the current user has permission for this module.</summary>
            /// <returns type="Boolean" />
            /// </signature>
            if (this.options.perm === modulePerms.none) {
                return false;
            }
            var permission = this.options.perm;
            var identity = this.options.engine.security.identity();
            return permission === modulePerms.everyone
                || permission === modulePerms.impersonated && identity
                || permission === modulePerms.authenticated && this.isAuthenticated()
                || permission === modulePerms.unauthenticated && (!identity || identity && !this.isAuthenticated());
        },
        authorize: function () {
            /// <signature>
            /// <summary>Authorizes this module.</summary>
            /// </signature>
            var state;
            if (this.hasPermission()) {
                this.element.removeClass(moduleClasses.unauthorized);
                state = moduleState.active;
            } else {
                this.element.addClass(moduleClasses.unauthorized);
                state = moduleState.inactive;
            }
            // Do not change module state in design mode
            if (this.options.mode === moduleState.active ||
                this.options.mode === moduleState.inactive) {
                //this._setOption("mode", state);
                this.options.mode = state;
                this._setupMode(this.options.mode);
            }
        },

        //
        // Activation Processes
        //

        _activate: $.noop,
        _deactivate: $.noop,

        //
        // Module Options
        //

        getModuleOptions: function () {
            /// <signature>
            /// <summary>Retrieves the module's current feature settings.</summary>
            /// <returns type="Object" />
            /// </signature>
            if (this._moduleOptions) {
                return this._moduleOptions;
            }
            var moduleOptions;
            var moduleOptionsJSON = this.element.attr(moduleAttributes.options);
            if (moduleOptionsJSON) {
                try {
                    moduleOptions = JSON.parse(moduleOptionsJSON);
                } catch (ex) {
                    _WinJS.DEBUG && _WinJS.log({
                        category: "PortalModule",
                        type: "error",
                        message: "JSON error: %s ( id: %s, class: %s ) >> %s"
                    },
                    ex.message,
                    this.element.attr("id"),
                    this.element.attr("class"),
                    moduleOptionsJSON);
                }
            }
            var options = this.createModuleOptions(moduleOptions);
            this.setModuleOptions(options);
            return options;
        },
        setModuleOptions: function (options) {
            /// <signature>
            /// <param name="options" type="Object" optional="true" />
            /// </signature>
            this._moduleOptions = options;
        },
        saveModuleOptions: function (options) {
            /// <signature>
            /// <summary>Saves module options to a data attirubte of the element.</summary>
            /// <param name="options" type="Object" optional="true" />
            /// </signature>
            options
                ? this.element.attr(moduleAttributes.options, JSON.stringify(options))
                : this.element.removeAttr(moduleAttributes.options);

            this.setModuleOptions(options);
        },
        updateModuleOptions: function (options) {
            /// <signature>
            /// <summary>Saves module options to a data attirubte of the element.</summary>
            /// <param name="options" type="Object" />
            /// </signature>
            this.saveModuleOptions($.extend(this.getModuleOptions(), options));
            if (options.active !== undefined) {
                this._setOption("active", options.active);
            }
            else if (options.perm !== undefined) {
                this._setOption("perm", options.perm);
            }
        },
        createModuleOptions: function (options) {
            /// <signature>
            /// <summary>Creates a new object that extends the given options with the module's default options.</summary>
            /// <param name="options" type="Object" optional="true" />
            /// <returns type="Object" />
            /// </signature>
            var o = {
                active: true,
                perm: modulePerms.everyone
            };
            if (options) {
                $.extend(o, options);
            }
            return o;
        },

        //
        // Module Mode Operations
        //

        _setOption: function (key, value) {
            /// <signature>
            /// <param name="key" type="String" />
            /// <param name="value" type="Object" />
            /// <returns type="Object" />
            /// </signature>
            if (this.options[key] !== value) {
                this._super(key, value);
                if (key === "mode") {
                    this._setupMode(value);
                } else if (key === "active" || key === "perm") {
                    this.authorize();
                }
            }
            return this;
        },
        _setupMode: function (value) {
            /// <signature>
            /// <param name="value" type="PI.Portal.ModuleState" />
            /// </signature>
            if (this.options.active
                && value === moduleState.active
                && this.hasPermission()) {
                if (this._actived) {
                    return;
                }
                this._activate();
                this._actived = true;
            } else {
                this._deactivate();
                this._actived = false;
            }
        }
    });

})(WinJS, PI.Portal);
