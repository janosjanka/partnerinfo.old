// Copyright (c) Partnerinfo Ltd. All Rights Reserved.
/// <reference path="base.js" />

(function ($, _KO, _PI) {
    "use strict";

    $.widget("PI.PortalTreeToolWin", $.PI.PortalToolWin, {
        options: {
            css: "ui-portal-toolwin-tree"
        },
        _initAsync: function () {
            /// <signature>
            /// <summary>Initializes a new instance of the tool window. Add initialization logic to this function.</summary>
            /// <returns type="WinJS.Promise" />
            /// </signature>
            var that = this;
            this._cssEnabled = true;
            this.cssEnabled = _KO.observable(this._cssEnabled);
            this.textToSearch = _KO.observable("");
            return require(["fancytree", "fancytree.css"]);
        },
        _onRender: function () {
            /// <signature>
            /// <summary>Renders content.</summary>
            /// </signature>
            this.toolWinBody.htmltree({
                autoOpen: false,
                autoEdit: true,
                autoFilter: true,
                autoPersist: true,
                autoScroll: true,
                keyPrefix: _PI.userCache.createKey(this.options.state.key + "."),
                visitor: this._treeVisitor.bind(this),
                renderNode: this._treeRenderNode.bind(this),
                click: this._treeClick.bind(this),
                dblclick: this._treeDblClick.bind(this)
            });
            this.toolWinMenu.html(
    '<div class="ui-row">\
    <div class="ui-col-sm-6">\
        <div role="group" class="ui-btn-group ui-btn-group-sm">\
            <ui-toggle params=\'checked: cssEnabled, icon: \"i portal toolwin-style\", attr: { title: \"Téma osztály nézet\" }, css: { \"ui-btn-flat\": true }, checkbox: false\'></ui-toggle>\
            <button class=\"ui-btn ui-btn-flat\" type="button" data-bind=\'click: expandNodes, clickBubble: false\'>\
                <i class=\"i portal toolwin-tree-expand\"></i>\
            </button>\
            <button class=\"ui-btn ui-btn-flat\" type=\"button\" data-bind=\'click: collapseNodes, clickBubble: false\'>\
                <i class=\"i portal toolwin-tree-collapse\"></i>\
            </button>\
            <button class=\"ui-btn ui-btn-flat\" type="button" data-bind=\'click: clearSelection, clickBubble: false\'>\
                &empty;\
            </button>\
        </div>\
    </div>\
    <div class=\"ui-col-sm-6\">\
        <ui-searchbox params=\'width: \"100%\", placeholder: "Keresés...", value: textToSearch, submit: search.bind($data), cancel: function () { $data.textToSearch(null); $data.search(); }\'></ui-searchbox>\
    </div>\
</div>');
            _KO.applyBindings(this, this.toolWinMenu[0]);
            this.cssEnabledSn = this.cssEnabled.subscribe(this._cssEnabledChanged, this);
        },
        _cssEnabledChanged: function (value) {
            /// <signature>
            /// <summary>Raised immediately after the property 'cssEnabled' is changed.</summary>
            /// <param name="value" type="Boolean" />
            /// </signature>
            this._cssEnabled = value; // Cache the native bool to avoid overhead during rendering
            this.refresh();
        },
        _onRefresh: function () {
            /// <signature>
            /// <summary>Rebuilds the tree.</summary>
            /// </signature>
            this._super();
            if (this.toolWinBody) {
                this.toolWinBody.htmltree("option", "root", this.options.designer.activeContainer);
                this.selectNode(this.options.designer.options.selection.current);
            }
        },
        _onSelectionChanged: function (event) {
            /// <signature>
            /// <param name="event" type="Event" />
            /// </signature>
            this._super(event);
            this.selectNode(event.detail.element);
        },
        clearSelection: function () {
            /// <signature>
            /// <summary>Clears selection.</summary>
            /// </signature>
            this.options.designer.options.selection.clear();
        },
        collapseNodes: function () {
            /// <signature>
            /// <summary>Collapses all the nodes.</summary>
            /// </signature>
            this.toolWinBody.htmltree("expandNodes", false);
        },
        expandNodes: function () {
            /// <signature>
            /// <summary>Expands all the nodes.</summary>
            /// </signature>
            this.toolWinBody.htmltree("expandNodes", true);
        },
        selectNode: function (element) {
            /// <signature>
            /// <summary>Selects the given element in the tree.</summary>
            /// <param name="element" type="HTMLElement" />
            /// </signature>
            /// <signature>
            /// <summary>Selects the given element in the tree.</summary>
            /// <param name="element" type="jQuery" />
            /// </signature>
            this._selectNode = true;
            try {
                var node = this.toolWinBody.htmltree("findNode", element);
                this.toolWinBody.htmltree("setActiveNode", node);
            } finally {
                this._selectNode = false;
            }
        },
        search: function () {
            /// <signature>
            /// <summary>Searches for nodes.</summary>
            /// </signature>
            var textToSearch = this.textToSearch();
            if (textToSearch) {
                this.expandNodes();
                this.toolWinBody.htmltree("filterNodes", textToSearch, false);
            } else {
                this.toolWinBody.htmltree("clearFilter");
            }
        },
        _treeVisitor: function (element) {
            /// <signature>
            /// <param name="node" type="HTMLElement" />
            /// <returns type="Object" />
            /// </signature>
            if (!element || !element.className) {
                return null;
            }
            var designer = this.options.designer;
            var type = designer.options.engine.context.getTypeOf(element);
            if (!type || type.className === "ui-module-event") {
                return null;
            }
            var title;
            if (this._cssEnabled) {
                var themeClasses = designer.options.style.getClassList(element);
                title = themeClasses && themeClasses.join(", ");
            } else {
                title = element.id;
            }
            if (title) {
                return {
                    icon: type.icon,
                    extraClasses: "ui-portal-toolwin-tree-node-" + (this._cssEnabled ? "class" : "id"),
                    title: title
                };
            }
            return {
                icon: type.icon,
                extraClasses: "ui-portal-toolwin-tree-node-noname",
                title: type.typeName
            };
        },
        _treeRenderNode: function (event, data) {
            /// <signature>
            /// <param name="event" type="Object" />
            /// <param name="data" type="Object" />
            /// </signature>
            if (!this._cssEnabled) {
                var node = data.node;
                var element = node.data.node;
                node.extraClasses = !element.id && "ui-portal-toolwin-tree-node-noname";
            }
        },
        _treeClick: function (node, event) {
            /// <signature>
            /// <param name="node" type="HTMLElement" />
            /// <param name="event" type="Object" />
            /// </signature>
            if (!this._selectNode) {
                var selection = this.options.designer.options.selection;
                selection.select(event.node.data.node, event.node.span) && selection.scroll();
            }
            //return true;
        },
        _treeDblClick: function (node, event) {
            /// <signature>
            /// <param name="node" type="HTMLElement" />
            /// <param name="event" type="Object" />
            /// </signature>
            this.options.designer.options.module.editAsync(event.node.data.node);
        },
        _destroy: function () {
            /// <signature>
            /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            /// </signature>
            this.toolWinMenu && _KO.cleanNode(this.toolWinMenu[0]);
            this.cssEnabledSn && this.cssEnabledSn.dispose();
            this.cssEnabledSn = null;
            this._super();
        }
    });


})(jQuery, ko, PI);