// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="../../base/fancytree/jquery.fancytree.js" />

(function (window, $, undefined) {
    "use strict";

    function JQ(element) {
        /// <signature>
        /// <param name="element" type="HTMLElement" />
        /// <returns type="jQuery" />
        /// </signature>
        /// <signature>
        /// <param name="element" type="jQuery" />
        /// <returns type="jQuery" />
        /// </signature>
        if (!element) {
            return;
        }
        return element.length ? element : $(element);
    }
    function unwrap(element) {
        /// <signature>
        /// <param name="element" type="HTMLElement" />
        /// <returns type="jQuery" />
        /// </signature>
        /// <signature>
        /// <param name="element" type="jQuery" />
        /// <returns type="jQuery" />
        /// </signature>
        if (!element) {
            return;
        }
        return element.length ? element[0] : element;
    }
    function visitor() { return true; }
    function canDrop(srcElement, dstElement) {
        /// <signature>
        /// <summary>Returns true if the source element can be dropped into the dest element.</summary>
        /// <returns type="Boolean" />
        /// </signature>
        var tagName = dstElement.tagName.toUpperCase();
        return (tagName === "DIV") || (tagName === "SPAN") || (tagName === "P");
    }

    $.widget("ui.htmltree", {
        options: {
            autoOpen: true,
            autoEdit: false,        // Makes DOM editable
            autoFilter: true,       // Makes DOM filterable
            autoPersist: false,     // Makes DOM persistant
            autoScroll: true,
            autoObserve: true,      // Observes DOM changes
            checkbox: false,
            icon: true,
            keyPrefix: undefined,   // ID prefix for persistance, etc.
            minExpandLevel: 2,
            quicksearch: true,
            root: undefined,        // Root element
            skipRoot: true,
            selectMode: 1,          // Selection Mode (1: Single)
            storage: "local",
            tabbable: true,
            titlesTabbable: false,
            toggleEffect: null,
            activate: undefined,    // A function called when an item is activated.
            canDrop: canDrop,       // A function that checks whether an item can be moved.
            click: undefined,       // A function called when an item is clicked.
            dblclick: undefined,    // A function called when an item is double clicked.
            renderNode: undefined,  // A function called when a node is rendered.
            visitor: visitor        // A function can be called when an element is added to the tree
        },
        findNode: function (element) {
            /// <signature>
            /// <summary>Finds a tree node using the given node.</summary>
            /// <param name="element" type="HTMLElement" />
            /// <returns type="Object" />
            /// </signature>
            /// <signature>
            /// <summary>Finds a tree node using the given node.</summary>
            /// <param name="element" type="jQuery" />
            /// <returns type="Object" />
            /// </signature>
            if (!this._isOpen) {
                return null;
            }
            if (!(element = unwrap(element))) {
                return null;
            }
            var rootNode = this.getRootNode();
            return rootNode ? rootNode.findFirst(function (it) {
                return it.data.node === element;
            }) : null;
        },
        getTree: function () {
            /// <signature>
            /// <summary>Gets the tree widget.</summary>
            /// </signature>
            if (this._isOpen) {
                return this.element.fancytree("getTree");
            }
        },
        getRootNode: function () {
            /// <signature>
            /// <summary>Gets the root node.</summary>
            /// <returns type="Object" />
            /// </signature>
            if (this._isOpen) {
                return this.element.fancytree("getRootNode");
            }
        },
        getActiveNode: function () {
            /// <signature>
            /// <summary>Gets the active node.</summary>
            /// </signature>
            if (this._isOpen) {
                return this.element.fancytree("getActiveNode");
            }
        },
        setActiveNode: function (node) {
            /// <signature>
            /// <summary>Selects the given node.</summary>
            /// <param name="node" type="Object" optional="true" />
            /// </signature>
            if (this._isOpen) {
                if (node) {
                    node.setActive(true);
                    node.setFocus(true);
                } else {
                    node = this.getActiveNode();
                    node && node.setActive(false);
                }
            }
        },
        expandNode: function (node, expanded) {
            /// <signature>
            /// <summary>Collapses the node.</summary>
            /// <param name="node" type="Object" />
            /// <param name="expanded" type="Boolean" />
            /// </signature>
            node.setExpanded(expanded);
        },
        expandNodes: function (expanded) {
            /// <signature>
            /// <summary>Collapses all the nodes.</summary>
            /// <param name="node" type="Object" />
            /// <param name="expanded" type="Boolean" />
            /// </signature>
            var rootNode = this.getRootNode();
            rootNode && rootNode.visit(function (node) {
                node.setExpanded(expanded);
            });
        },
        filterBranches: function (predicate) {
            /// <signature>
            /// <summary>Dimm or hide unmatched branches. Matching nodes are displayed together with all descendants.</summary>
            /// <param name="predicate" type="Function" />
            /// <returns type="Number" integer="true" />
            /// </signature>
            /// <signature>
            /// <summary>Dimm or hide unmatched branches. Matching nodes are displayed together with all descendants.</summary>
            /// <param name="predicate" type="String" />
            /// <returns type="Number" integer="true" />
            /// </signature>
            return this.getTree().filterBranches(predicate);
        },
        filterNodes: function (predicate) {
            /// <signature>
            /// <summary>Dimm or hide unmatched nodes.</summary>
            /// <param name="predicate" type="Function" />
            /// <returns type="Number" integer="true" />
            /// </signature>
            /// <signature>
            /// <summary>Dimm or hide unmatched nodes.</summary>
            /// <param name="predicate" type="String" />
            /// <returns type="Number" integer="true" />
            /// </signature>
            return this.getTree().filterNodes(predicate);
        },
        clearFilter: function () {
            /// <signature>
            /// <summary>Reset the filter.</summary>
            /// </signature>
            this.getTree().clearFilter();
        },
        refresh: function () {
            /// <signature>
            /// <summary>Rebuilds the tree</summary>
            /// </signature>
            this._createTree();
        },
        _create: function () {
            /// <signature>
            /// <summary>Constructor</summary>
            /// </signature>
            this._nodeInsertedBound = this._nodeInserted.bind(this);
            this._nodeRemovedBound = this._nodeRemoved.bind(this);

            if (this.options.autoOpen && this.options.root) {
                this._setOption("root", this.options.root);
            }
        },
        _createTree: function () {
            /// <signature>
            /// <summary>Creates a fancytree control</summary>
            /// </signature>
            var source = this.options.root
                ? this._createTreeNodes(
                    this.options.skipRoot
                        ? this.options.root.children()
                        : this.options.root, this.options.visitor)
                : [];

            if (this._isOpen) {
                this.element.fancytree("option", "source", source);
                return;
            }

            var options = {
                extensions: [],
                autoScroll: this.options.autoScroll,
                checkbox: this.options.checkbox,
                icon: this.options.icon,
                minExpandLevel: this.options.minExpandLevel,
                selectMode: this.options.selectMode,
                source: source,
                quicksearch: this.options.quicksearch,
                tabbable: this.options.tabbable,
                titlesTabbable: this.options.titlesTabbable,
                toggleEffect: this.options.toggleEffect,
                renderNode: this.options.renderNode,
                activate: this.options.activate,
                click: this.options.click,
                dblclick: this.options.dblclick
            };

            // Drag & Drop with inline editor for element IDs
            if (this.options.autoEdit) {
                options.extensions.push("dnd", "edit");
                options.dnd = {
                    preventRecursiveMoves: true, // Prevent dropping nodes on own descendants
                    preventVoidMoves: true, // Prevent dropping nodes 'before self', etc.
                    dragStart: this._nodeDragStart.bind(this),
                    dragEnter: this._nodeDragEnter.bind(this),
                    dragOver: this._nodeDragOver.bind(this),
                    dragDrop: this._nodeDragDrop.bind(this)
                };
                options.edit = {
                    triggerStart: ["f2", "shift+click"],
                    save: this._nodeSave.bind(this),
                    close: this._nodeClose.bind(this)
                };
            }

            // DOM filter features
            if (this.options.autoFilter) {
                options.extensions.push("filter");
                options.filter = {
                    mode: "hide",
                    autoApply: true
                };
            }

            // State persistance
            if (this.options.autoPersist) {
                options.extensions.push("persist");
                options.persist = {
                    cookieDelimiter: ".",
                    cookiePrefix: this.options.keyPrefix,
                    store: this.options.storage
                };
            }

            this.element.fancytree(options);
            this._isOpen = true;
        },
        _createTreeNodes: function (nodes, visitor) {
            /// <signature>
            /// <summary>Creates an array of fancytree nodes from DOM nodes</summary>
            /// <param name="nodes" type="NodeList" />
            /// <param name="visitor" type="Function" />
            /// <returns type="Array" />
            /// </signature>
            var treeNodes = [];
            for (var i = 0, len = nodes.length; i < len; ++i) {
                var treeNode = this._createTreeNode(nodes[i], visitor);
                treeNode && treeNodes.push(treeNode);
            }
            return treeNodes;
        },
        _createTreeNode: function (node, visitor) {
            /// <signature>
            /// <summary>Creates a fancytree node from a DOM node</summary>
            /// <param name="node" type="Node" />
            /// <param name="visitor" type="Function" />
            /// <returns type="Object" />
            /// </signature>
            var treeNode = visitor(node);
            if (treeNode === null) {
                return null;
            }
            treeNode = treeNode || {};
            treeNode.key = node.id;
            treeNode.title = treeNode.title || node.id;
            treeNode.data = treeNode.data || {};
            treeNode.data.node = node;
            treeNode.children = this._createTreeNodes(node.childNodes, visitor);
            return treeNode;
        },
        _observe: function (enabled) {
            /// <signature>
            /// <summary>Observes changes within the root element.</summary>
            /// <param name="enabled" type="Boolean" />
            /// </signature>
            if (!this.options.root) {
                return;
            }
            if (enabled) {
                this._on(this.options.root, {
                    "DOMNodeInserted": this._nodeInsertedBound,
                    "DOMNodeRemoved": this._nodeRemovedBound
                });
            } else {
                this._off(this.options.root);
            }
        },
        _nodeInserted: function (event) {
            /// <signature>
            /// <summary>Fires when a new DOM node is inserted.</summary>
            /// </signature>
            var domNode = event.originalEvent.target;
            var treeNode = this._createTreeNode(domNode, this.options.visitor);
            if (!treeNode) {
                return;
            }
            var domParentNode = this.findNode(event.originalEvent.relatedNode) || this.getRootNode();
            if (!domParentNode) {
                return;
            }
            domParentNode.addChildren([treeNode], this.findNode(domNode.nextSibling));
        },
        _nodeRemoved: function (event) {
            /// <signature>
            /// <summary>Fires when a node changes.</summary>
            /// </signature>
            var mutEvent = event.originalEvent;
            var node = this.findNode(mutEvent.target);
            node && node.remove();
        },
        _nodeDragStart: function () {
            /// <signature>
            /// <summary>Return false to cancel dragging of node.</summary>
            /// <returns type="Boolean" />
            /// </signature>
            return true;
        },
        _nodeDragEnter: function () {
            /// <signature>
            /// <summary>Return false to disallow dropping on node. In this case
            /// dragOver and dragLeave are not called. Return 'over', 'before, or 'after' to force a hitMode.
            /// Return ['before', 'after'] to restrict available hitModes.
            /// Any other return value will calc the hitMode from the cursor position.</summary>
            /// <returns type="Array" />
            /// </signature>
            return ["over"];
        },
        _nodeDragOver: function (node, data) {
            /// <signature>
            /// <summary>Returns false to disallow dropping on node.</summary>
            /// <returns type="Boolean" />
            /// </signature>
            return this.options.canDrop(node.data.node, data.otherNode.data.node);
        },
        _nodeDragDrop: function (node, data) {
            /// <signature>
            /// <summary>This function MUST be defined to enable dropping of items on the tree (hitMode is 'before', 'after', or 'over').</summary>
            /// </signature>
            var src = data.otherNode;
            var dst = data.node;
            if (this.options.canDrop(src.data.node, dst.data.node)) {
                src.moveTo(node, data.hitMode);
                $(src.data.node).appendTo(dst.data.node);
            }
        },
        _nodeSave: function (event, data) {
            /// <signature>
            /// <summary>Raised when an edit session is commited.</summary>
            /// </signature>
            var handle = window.setTimeout(function () {
                window.clearTimeout(handle);
                var node = data.node;
                var id = node.title && node.title.replace(/\s+/gi, '-').toLowerCase();
                node.data.node.id = id;
                node.setTitle(id);
                node.render();
            });
            return true;
        },
        _nodeClose: function (event, data) {
            /// <signature>
            /// <summary>Raised when the inline editor is removed.</summary>
            /// </signature>
            data.node.render(); // Fix (icon) rendering bug.
        },
        _setOption: function (key, value) {
            /// <signature>
            /// <summary>Sets the value for the given key</summary>
            /// </signature>
            var updateTree = false;
            var changeRoot = key === "root";

            if (changeRoot) {
                this._observe(false);
                value = JQ(value);
                this._rootParent = value.parent();
                updateTree = true;
            } else if (key === "visitor") {
                value = value || visitor;
                updateTree = true;
            }

            this._super(key, value);

            if (updateTree) {
                this.refresh();
            }
            if (changeRoot && this.options.autoObserve) {
                this._observe(true);
            }
        },
        _destroy: function () {
            /// <signature>
            /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            /// </signature>
            this._observe(false);
            this._isOpen && this.element.fancytree("destroy");
            this._isOpen = false;
        }
    });

})(window, $);