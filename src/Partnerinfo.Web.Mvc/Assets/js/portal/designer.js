// Copyright (c) Partnerinfo TV. All Rights Reserved.

/// <reference path="engine.js" />
/// <reference path="managers/input.js" />
/// <reference path="managers/menu.js" />
/// <reference path="managers/toolwin.js" />
/// <reference path="managers/selection.js" />
/// <reference path="managers/module.js" />
/// <reference path="managers/style.js" />
/// <reference path="modules/edit.js" />
/// <reference path="modules/base.js" />
/// <reference path="services/page.js" />

(function (_Global, $, _WinJS, _PI, _Portal) {
    "use strict";

    var _Promise = _WinJS.Promise;
    var _ModuleClasses = _Portal.ModuleClasses;
    var _ModuleState = _Portal.ModuleState;

    var createEvent = _WinJS.Utilities._createEventProperty;
    var eventNames = {
        rendering: "rendering",
        rendered: "rendered"
    };

    $.widget("PI.PortalDesigner", {
        options: {
            /// <field type="PI.Portal.Engine" />
            engine: null,
            /// <field type="PI.Portal.MenuManager" />
            menu: null,
            /// <field type="PI.Portal.SelectionManager" />
            selection: null,
            /// <field type="PI.Portal.ModuleManager" />
            module: null,
            /// <field type="PI.Portal.StyleManager" />
            style: null,
            /// <field type="PI.Portal.InputManager" />
            input: null,
            /// <field type="PI.Portal.ToolWinManager" />
            toolWin: null,
            /// <field type="$" />
            toolbar1: null,
            /// <field type="$" />
            toolbar2: null,
            /// <field type="PI.Portal.ModuleState" />
            mode: _ModuleState.edit,
            /// <field type="String" />
            portalUri: null,
            /// <field type="String" />
            pageUri: null,
            /// <field type="Object" />
            links: {
                settings: "/admin/portals/{portal}/#/settings",
                sharing: "/admin/portals/{portal}/#/sharing",
                pagelist: "/admin/portals/{portal}/#/pages",
                pageview: "/{portal}/{page}?preview=true"
            }
        },
        /// <field type="$" />
        container: null,
        /// <field type="$" />
        activeContainer: null,
        /// <field type="Object" />
        contentPage: null,
        /// <field type="Object" />
        masterPage: null,
        /// <field type="Boolean" />
        isLoading: false,
        /// <field type="Boolean" />
        rendered: false,

        /// <field type="Function">
        /// Raised before modules are rendered.
        /// </field>
        onrendering: createEvent(eventNames.rendering),

        /// <field type="Function">
        /// Raised after modules are rendered.
        /// </field>
        onrendered: createEvent(eventNames.rendered),

        _create: function () {
            /// <signature>
            /// <summary>Initializes a new instance of the PortalDesigner.</summary>
            /// </signature>
            this._onMenuChangedBound = this._onMenuChanged.bind(this);

            this.container = $("<div>").addClass("ui-portal-modules ui-module-panel");
            this.element.addClass("ui-portal-designer").append(this.container);

            this._setupEngine(this.options.engine);
            this._setupSelection(this.options.selection);
            this._setupModule(this.options.module);
            this._setupStyle(this.options.style);
            this._setupMenu(this.options.menu, this.options.toolbar1, this.options.toolbar2);
            this._setupInput(this.options.input);
            this._setupToolWin(this.options.toolWin);
            this._setActiveContainer(this.container);
            this._setupMode(this.options.mode);
            this._loadSettings();

            this.loadAsync();

            _WinJS.Utilities.setOptions(this, this.options);
        },

        //
        // Site & Page
        //

        navigate: function (name) {
            /// <signature>
            /// <summary>Navigates to a resource identified by a name.</summary>
            /// <param name="name" type="String" />
            /// </signature>
            var options = this.options;
            _Global.open(options.links[name].replace(/{(portal|page)}/ig,
                function (match, name) {
                    if (name === "portal") {
                        return options.portalUri || "null";
                    }
                    if (name === "page") {
                        return options.pageUri || "null";
                    }
                    return "null";
                }));
        },
        loadAsync: function () {
            /// <signature>
            /// <returns type="WinJS.Promise" />
            /// </signature>
            if (!this.options.portalUri || !this.options.pageUri) {
                return _Promise.error();
            }
            return this.options.engine.pageService.getLayersByUriAsync(this.options.portalUri, this.options.pageUri, this)
                .then(function (data) {
                    this.options.engine.portal = data.portal;
                    this.options.engine.page = data.contentPage;
                    this.contentPage = data.contentPage;
                    this.masterPage = data.masterPage;
                    return this.renderAsync();
                });
        },
        editAsync: function () {
            /// <signature>
            /// <summary>Edits the current page.</summary>
            /// <returns type="WinJS.Promise" />
            /// </signature>
            if (this.options.mode === _ModuleState.edit) {
                return _Promise.complete();
            }
            this._setOption("mode", _ModuleState.edit);
            var that = this;
            return this.renderAsync().then(function () {
                that.options.selection.load();
            });
        },
        saveAsync: function () {
            /// <signature>
            /// <summary>Saves the content of the page.</summary>
            /// <returns type="WinJS.Promise" />
            /// </signature>
            if (!this.contentPage) {
                return _Promise.error();
            }

            this.options.selection.save();
            this.options.selection.clear();

            var oldMode = this.options.mode;
            this.options.mode = _ModuleState.view;
            this._setupMode(this.options.mode);

            var htmlContent = this.activeContainer.html();
            var styleContent = this.options.style.getContentCode();

            return this.options.engine.pageService.setContentAsync(this.options.portalUri, this.options.pageUri, {
                htmlContent: htmlContent,
                styleContent: styleContent
            }, this).then(
                function () {
                    this.contentPage.htmlContent = htmlContent;
                    this.contentPage.styleContent = styleContent;
                    this._setOption("mode", _ModuleState.active);
                },
                function () {
                    this._setOption("mode", oldMode);
                });
        },
        cancelAsync: function () {
            /// <signature>
            /// <summary>Cancels pending changes.</summary>
            /// <returns type="WinJS.Promise" />    
            /// </signature>
            this.options.selection.save();
            this.options.selection.clear();
            this._setOption("mode", _ModuleState.active);
            return this.renderAsync();
        },
        close: function () {
            /// <signature>
            /// <summary>Closes the page.</summary>
            /// </signature>
            this.empty();
            this.masterPage = null;
            this.contentPage = null;
            this.options.engine.page = null;
            this.options.engine.portal = null;
        },
        empty: function () {
            /// <signature>
            /// <summary>Remove all child nodes from the active container.</summary>
            /// </signature>
            this.options.selection.clear();
            this._setActiveContainer();
        },

        //
        // Rendering
        //

        renderAsync: function () {
            /// <signature>
            /// <summary>Changes the body of the page and parses the page modules on the page.</summary>
            /// <returns type="WinJS.Promise" />
            /// </signature>
            var that = this;
            this.rendered = false;
            return _Promise.timeout(1).then(function () {
                that.dispatchEvent(eventNames.rendering);
                that.empty();
                return _Promise.join(that.renderStylesAsync(), that.renderHtmlAsync())
                    .then(function () {
                        return that.options.module.parseAllAsync();
                    })
                    .then(function () {
                        that.rendered = true;
                        that.dispatchEvent(eventNames.rendered);
                    });
            });
        },
        renderHtmlAsync: function () {
            /// <signature>
            /// <summary>Renders HTML content.</summary>
            /// <returns type="WinJS.Promise" />
            /// </signature>
            return _Promise(function (completeDispatch) {
                var activeContainer = this.activeContainer;
                if (this.masterPage) {
                    this.container.html(this.masterPage.htmlContent);
                    activeContainer = $("." + _ModuleClasses.content + ":first", this.container);
                    if (this.options.menu.renderMaster()) {
                        this.options.engine.context.forEach(function (element, type) {
                            if (type.className !== _ModuleClasses.event &&
                                type.className !== _ModuleClasses.content) {
                                this.options.engine.context.parse(element, null, type);
                            }
                        }, this.container, this);
                    }
                }
                this.contentPage && activeContainer.html(this.contentPage.htmlContent);
                this._setActiveContainer(activeContainer);
                this.options.module.getEvent();
                completeDispatch();
            }, this);
        },
        renderStylesAsync: function (master, content) {
            /// <signature>
            /// <summary>Renders CSS style definitions.</summary>
            /// <param name="master" type="Boolean" />
            /// <param name="content" type="Boolean" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            return _Promise(function (completeDispatch) {
                master !== false
                    && this.masterPage
                    && this.options.style.setMasterCode(this.masterPage.styleContent);
                content !== false
                    && this.contentPage
                    && this.options.style.setContentCode(this.contentPage.styleContent);
                completeDispatch();
            }, this);
        },

        //
        // Page Module
        //

        _setActiveContainer: function (element) {
            /// <signature>
            /// <param name="element" type="$" optional="true" />
            /// </signature>
            if (arguments.length === 0) {
                this.options.input.off();
                this.activeContainer.empty();
                return;
            }
            this._applyHiddenVisible(false);
            this.activeContainer = this.options.engine.context.container = element;
            this.options.input.on(this.activeContainer);
            this._applyHiddenVisible(this.options.mode === _ModuleState.edit ? this.options.menu.hiddenVisible() : false);
        },

        //
        // Settings
        //

        _setOption: function (key, value) {
            /// <signature>
            /// <param name="key" type="String" />
            /// <param name="value" type="Object" />
            /// <returns type="$.PI.PortalDesigner" />
            /// </signature>
            if (this.options[key] !== value) {
                this._super(key, value);
                if (key === "mode") {
                    this._setupMode(value);
                }
            }
            return this;
        },
        _setupMode: function (mode) {
            /// <signature>
            /// <param name="mode" type="PI.Portal.ModuleState" />
            /// </signature>
            var editing = mode === _ModuleState.edit;
            this.options.engine.context.forEach(function (e, t, m) { m && m.option("mode", mode); });
            this.options.menu.mode(this.options.mode);
            this.options.toolWin.displayAll(editing);
            this._applyHiddenVisible(editing ? this.options.menu.hiddenVisible() : false);
        },

        //
        // Setup
        //

        _setupEngine: function (engine) {
            /// <signature>
            /// <param name="engine" type="PI.Portal.Engine" />
            /// </signature>
            engine = engine || new _Portal.Engine({ autoParse: false });
            engine.context.container = this.container;
            this.options.engine = engine;
        },
        _setupSelection: function (selection) {
            /// <signature>
            /// <param name="selection" type="PI.Portal.SelectionManager" optional="true" />
            /// </signature>
            selection = selection || new _Portal.SelectionManager(this);
            selection.designer = this;
            selection.context = this.options.engine.context;
            this.options.selection = selection;
        },
        _setupModule: function (module) {
            /// <signature>
            /// <param name="module" type="PI.Portal.ModuleManager" />
            /// </signature>
            module = module || new _Portal.ModuleManager(this);
            module.designer = this;
            this.options.module = module;
        },
        _setupStyle: function (style) {
            /// <signature>
            /// <param name="style" type="PI.Portal.StyleManager" />
            /// </signature>
            style = style || new _Portal.StyleManager(this);
            style.designer = this;
            this.options.style = style;
        },
        _setupMenu: function (menu, toolbar1, toolbar2) {
            /// <signature>
            /// <summary>Sets a new menu manager.</summary>
            /// <param name="menu" type="PI.Portal.MenuManager" />
            /// <param name="toolbar1" type="HTMLElement" />
            /// <param name="toolbar2" type="HTMLElement" />
            /// </signature>
            this.options.menu = menu = menu || new _Portal.MenuManager(this, { mode: this.options.mode });
            this.options.menu.addEventListener("changed", this._onMenuChangedBound, true);
            if (toolbar1 || toolbar2) {
                var that = this;
                var toolbarDoc = toolbar1.ownerDocument;
                var toolbarWin = toolbarDoc.defaultView || toolbarDoc.parentWindow;
                toolbarWin.require(["portal/designer/menu.html"])
                    .then(function () {
                        toolbar1 && ko.renderTemplate("koPortalDesignerToolbar1", that, null, toolbar1);
                        toolbar2 && ko.renderTemplate("koPortalDesignerToolbar2", that, null, toolbar2);
                    });
            }
        },
        _setupInput: function (input) {
            /// <signature>
            /// <summary>Sets a input manager.</summary>
            /// <param name="input" type="PI.Portal.InputManager" optional="true" />
            /// </signature>
            input = input || new _Portal.InputManager(this);
            input.designer = this;
            input.context = this.options.engine.context;
            this.options.input = input;
        },
        _setupToolWin: function (toolWin) {
            /// <signature>
            /// <summary>Sets a tool window manager.</summary>
            /// <param name="toolWin" type="PI.Portal.ToolWinManager" />
            /// </signature>
            var toolWinRes = _T("portal").toolWins;
            var toolWinState = _Portal.ToolWinState;
            var toolWinDisplay = _Portal.WinDisplay;

            var elHeight = _Global.innerHeight;
            var elWidth = _Global.innerWidth;
            var winW = 290;
            var winHT = elHeight / 3 | 0; /* int */
            var winHB = elHeight - winHT - 20;
            var winRL = elWidth - winW - 20;

            this.options.toolWin = toolWin || new _Portal.ToolWinManager(this, {
                element: this.element,
                toolWins: [{
                    type: "PortalToolboxToolWin",
                    icon: "i portal toolwin-toolbox",
                    resources: toolWinRes.toolbox,
                    state: new toolWinState({ key: "portal.toolwin.toolbox", display: toolWinDisplay.opaque, height: elHeight, width: 92 })
                }, {
                    type: "PortalModuleToolWin",
                    icon: "i portal toolwin-module",
                    resources: toolWinRes.module,
                    state: new toolWinState({ key: "portal.toolwin.module", display: toolWinDisplay.opaque, height: 250 })
                }, {
                    type: "PortalEventToolWin",
                    icon: "i portal toolwin-event",
                    resources: toolWinRes.event,
                    state: new toolWinState({ key: "portal.toolwin.event", display: toolWinDisplay.none })
                }, {
                    type: "PortalStyleToolWin",
                    icon: "i portal toolwin-style",
                    resources: toolWinRes.style,
                    state: new toolWinState({ key: "portal.toolwin.style", display: toolWinDisplay.opaque, offsetX: 92, width: winW, height: elHeight })
                }, {
                    type: "PortalTreeToolWin",
                    icon: "i portal toolwin-tree",
                    resources: toolWinRes.tree,
                    state: new toolWinState({ key: "portal.toolwin.tree", display: toolWinDisplay.opaque, offsetX: winRL, width: winW, height: elHeight })
                }, {
                    type: "PortalReferenceToolWin",
                    icon: "i portal toolwin-reference",
                    resources: toolWinRes.reference,
                    state: new toolWinState({ key: "portal.toolwin.reference", display: toolWinDisplay.none })
                }, {
                    type: "PortalInfoToolWin",
                    icon: "i portal toolwin-info",
                    resources: toolWinRes.info,
                    state: new toolWinState({ key: "portal.toolwin.info", display: toolWinDisplay.none })
                }, {
                    type: "PortalPreviewToolWin",
                    icon: "i portal toolwin-preview",
                    resources: toolWinRes.preview,
                    state: new toolWinState({ key: "portal.toolwin.preview", display: toolWinDisplay.none })
                }]
            });
        },

        //
        // Settings
        //

        _onMenuChanged: function (event) {
            /// <signature>
            /// <param name="event" type="Event" />
            /// </signature>
            var property = event.detail.property;
            var value = event.detail.value;
            if (property === "renderMaster") {
                this._applyRenderMaster(value);
            } else if (property === "hiddenVisible") {
                this._applyHiddenVisible(value);
            }
            !this.isLoading && this._saveSettings();
        },
        _applyRenderMaster: function () {
            /// <signature>
            /// <summary>Applies master page settings.</summary>
            /// <param name="value" type="Boolean" />
            /// </signature>
            this.renderAsync();
        },
        _applyHiddenVisible: function (value) {
            /// <signature>
            /// <summary>Applies module visibility settings.</summary>
            /// <param name="value" type="Boolean" />
            /// </signature>
            if (!this.activeContainer) {
                return;
            }
            value ?
                this.activeContainer.addClass("ui-modules-showall") :
                this.activeContainer.removeClass("ui-modules-showall");
        },
        _loadSettings: function () {
            /// <signature>
            /// <summary>Loads state information for the designer.</summary>
            /// </signature>
            this.isLoading = true;
            try {
                var menu = this.options.menu;
                var state = _PI.userCache(_PI.Storage.local, "portal.designer") || {};
                state.menu = state.menu || {};
                menu.renderMaster(!!state.menu.renderMaster);
                menu.hiddenVisible(!!state.menu.hiddenVisible);
            } finally {
                this.isLoading = false;
            }
        },
        _saveSettings: function () {
            /// <signature>
            /// <summary>Saves the current state of the designer.</summary>
            /// </signature>
            if (this.isLoading) {
                return;
            }
            var menu = this.options.menu;
            _PI.userCache(_PI.Storage.local, "portal.designer", {
                menu: {
                    renderMaster: menu.renderMaster(),
                    hiddenVisible: menu.hiddenVisible()
                }
            });
        },

        _destroy: function () {
            /// <signature>
            /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            /// </signature>
            this._setOption("mode", _ModuleState.view);

            if (this.options.menu) {
                this.options.menu.removeEventListener("changed", this._onMenuChangedBound, true);
                this.options.menu.dispose();
            }
            this.options.input && this.options.input.dispose();
            this.options.toolWin && this.options.toolWin.dispose();
            this.options.selection && this.options.selection.dispose();
            this.options.style && this.options.style.dispose();
            this.options.module && this.options.module.dispose();
            this.activeContainer && this._setActiveContainer();

            this.options.input = null;
            this.options.menu = null;
            this.options.toolWin = null;
            this.options.selection = null;
            this.options.style = null;
            this.options.module = null;

            this.activeContainer = null;
        }
    });

    _WinJS.Class.mix($.PI.PortalDesigner, _WinJS.Utilities.eventMixin);

})(window, jQuery, WinJS, PI, PI.Portal);