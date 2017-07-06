// Copyright (c) Partnerinfo TV. All Rights Reserved.

/// <reference path="../services/portal.js" />
/// <reference path="../services/page.js" />
/// <reference path="../viewmodels/page.js" />
/// <reference path="../base/fancytree/jquery.fancytree.js" />

(function (_Global, $, _WinJS, _PI, _Portal) {
    "use strict";

    var _Promise = _WinJS.Promise;
    var _Resources = _WinJS.Resources;
    var _Utilities = _WinJS.Utilities;

    var ns = _WinJS.Namespace.defineWithParent(_PI, "Portal", {
        PageNode: _WinJS.Class.define(function PageNode_ctor(portal, page, portalService, pageService) {
            /// <signature>
            /// <summary>Initializes a new instance of the PageNode class.</summary>
            /// <param name="portal" type="Object" />
            /// <param name="page" type="Object" optional="true" />
            /// <returns type="PageNode" />
            /// </signature>
            page = page || {};
            this.portalService = portalService || ns.PortalService;
            this.pageService = pageService || ns.PageService;
            this.portal = portal;
            this.master = page.master;
            this.id = page.id;
            this.uri = page.uri;
            this.name = page.name;
            this.modifiedDate = page.modifiedDate;
        }, {
            addNew: function () {
                /// <signature>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                var that = this;
                return _Promise(function (completeDispatch, errorDispatch) {
                    _PI.dialog({
                        name: "portal.page",
                        params: { portal: that.portal },
                        done: function (event) {
                            if (event.result === "ok") {
                                completeDispatch({ state: "created" });
                            } else {
                                errorDispatch();
                            }
                        }
                    });
                });
            },
            edit: function () {
                /// <signature>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                var that = this;
                return _Promise(function (completeDispatch, errorDispatch) {
                    _PI.dialog({
                        name: "portal.page",
                        params: { portal: that.portal, page: that },
                        done: function (event) {
                            if (event.result === "ok") {
                                completeDispatch({ state: "modified" });
                            } else {
                                errorDispatch();
                            }
                        }
                    });
                });
            },
            share: function () {
                /// <signature>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                var that = this;
                return _Promise(function (completeDispatch, errorDispatch) {
                    _PI.dialog({
                        name: "security.acl",
                        params: {
                            objectType: "page",
                            objectId: that.id
                        },
                        done: function (event) {
                            completeDispatch();
                        }
                    });
                });
            },
            copy: function () {
                /// <signature>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return this.pageService.copyAsync(this.portal.uri, this.uri, { uri: this.uri + "-copy" }, this)
                    .then(function (page) {
                        return _Promise.complete({ state: "created" });
                    });
            },
            move: function (parentPage) {
                /// <signature>
                /// <param name="parentPage" type="PI.Portal.PageNode" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return this.pageService.moveAsync(this.portal.uri, this.uri, parentPage.uri, this)
                    .then(function (page) {
                        return _Promise.complete({ state: "moved" });
                    });
            },
            remove: function () {
                /// <signature>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                var that = this;
                return _Promise(function (completeDispatch, errorDispatch) {
                    _PI.dialog({
                        name: "confirm",
                        type: "remove",
                        done: function (response) {
                            if (response.result === "yes") {
                                that.pageService.deleteAsync(that.portal.uri, that.uri)
                                    .then(function () {
                                        completeDispatch({ state: "deleted" });
                                    }, function () {
                                        errorDispatch();
                                    });
                            }
                        }
                    });
                });
            },
            setMaster: function (masterPage) {
                /// <signature>
                /// <param name="masterPage" type="PI.Portal.PageNode" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                var that = this;
                return this.pageService.setMasterAsync(this.portal.uri, this.uri, masterPage ? masterPage.uri : null, this).then(
                    function () {
                        that.master = masterPage;
                        return _Promise.complete({ state: "modified" });
                    });
            },
            openViewAction: function (preview) {
                window.open(ns.PageUrlHelper.viewAction(this.portal.uri, this.uri, true), "_blank");
                return _Promise.complete();
            },
            openItemAction: function (action) {
                if (action === "portal") {
                    window.open(ns.PageUrlHelper.viewAction(this.portal.uri, "", action), "_blank");
                } else if (action === "project") {
                    _Utilities.openLink("/admin/projects/{project}/#/actions", { project: this.portal.project && this.portal.project.id });
                } else {
                    window.open(ns.PageUrlHelper.itemAction(this.portal.uri, this.uri, action), "_blank");
                }
                return _Promise.complete();
            },
            setAsPortalHomePage: function () {
                /// <signature>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                var that = this;
                return this.portalService.setHomePageAsync(this.portal.uri, this.uri, this)
                    .then(function () {
                        that.portal.homePage = that;
                        return _Promise.complete({ state: "modified" });
                    });
            },
            togglePortalMasterPage: function () {
                /// <signature>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                var that = this;
                var masterPageUri = null;
                if (!this.portal.masterPage || this.portal.masterPage.uri !== this.uri) {
                    masterPageUri = this.uri;
                }
                return this.portalService.setMasterPageAsync(this.portal.uri, masterPageUri, this)
                    .then(function () {
                        that.portal.master = masterPageUri ? that : null;
                        return _Promise.complete({ state: "modified" });
                    });
            }
        })
    });

    $.widget("PI.PortalPages", {
        options: {
            treeClass: "portal-tree",
            portalService: _Portal.PortalService,
            pageService: _Portal.PageService,
            portal: null
        },
        _create: function () {
            /// <signature>
            /// <summary>Initializes a new instance of the widget</summary>
            /// </signature>
            this._super();

            this._promise = require(["fancytree", "fancytree.css"]);

            this._renderColumnsBound = this._renderColumns.bind(this);
            this._onDragStartBound = this._onDragStart.bind(this);
            this._onDragEnterBound = this._onDragEnter.bind(this);
            this._onDragLeaveBound = this._onDragLeave.bind(this);
            this._onDragDropBound = this._onDragDrop.bind(this);
            this._onActivateBound = this._onActivate.bind(this);
            this._onRemoveNodeBound = this._onRemoveNode.bind(this);

            this.uiHostOuter = $("<div>").addClass("ui-box").appendTo(this.element);
            this.uiHostInner = $("<div>").addClass("ui-box-group").appendTo(this.uiHostOuter);
            this.uiTree = $("<table>")
                .addClass(this.options.treeClass)
                .html(
                    "<colgroup>" +
                        "<col style='width: 10px;'/>" +
                        "<col style='width: 60px;'/>" +
                        "<col/>" +
                        "<col style='width: 150px;'/>" +
                        "<col style='width: 130px;'/>" +
                    "</colgroup>" +
                    "<thead>" +
                        "<th></th>" +
                        "<th></th>" +
                        "<th></th>" +
                        "<th></th>" +
                        "<th></th>" +
                    "</thead>" +
                    "<tbody></tbody>")
                .appendTo(this.uiHostInner);

            this.uiMenu = $("<div>")
                .addClass("ui-dropdown")
                .html(
                    '<button class="ui-btn ui-btn-flat ui-btn-sm" type="button" data-action="toggleMenu"><i class="i dd"></i></button>' +
                    '<ul role="menu" class="ui-menu">' +
                        '<li role="menuitem"><a data-action="addNew"><i class="i add"></i><span>Új oldal hozzáadása</span></a></li>' +
                        '<li role="separator"></li>' +
                        '<li role="menuitem"><a data-action="edit"><i class="i option"></i><span>Oldal beállítása</span></a></li>' +
                        '<li role="menuitem"><a data-action="openItemAction" data-action-params="designer"><i class="i edit"></i><span>Oldal tervezése</span></a></li>' +
                        '<li role="menuitem"><a data-action="copy"><i class="i copy"></i><span>Oldal másolása</span></a></li>' +
                        '<li role="menuitem"><a data-action="remove"><i class="i delete"></i><span>Oldal törlése</span></a></li>' +
                        '<li role="menuitem"><a data-action="setMaster"><i class="i page master"></i><span>Portál elrendezés használata</span></a></li>' +
                        '<li role="separator"></li>' +
                        '<li role="menuitem"><a data-action="share"><i class="i share"></i><span>Megosztás másokkal</span></a></li>' +
                        '<li role="menuitem"><a data-action="openViewAction" data-action-params="true"><i class="i view"></i><span>Megtekintés felhasználóként</span></a></li>' +
                        '<li role="separator"></li>' +
                        '<li role="menuitem"><a data-action="setAsPortalHomePage"><i class="i page"></i><span>Portál landolási oldala</span></a></li>' +
                        '<li role="menuitem"><a data-action="togglePortalMasterPage"><i class="i page master"></i><span>Portál elrendezési oldala</span></a></li>' +
                    '</ul>')
                .dropdown();

            this._on(this.uiTree, {
                "click a[data-action], button[data-action]": this._onClick.bind(this),
                "mouseenter tr": this._onMouseEnter.bind(this)
            });

            this.refresh();
        },
        refresh: function () {
            /// <signature>
            /// <summary>Refreshes</summary>
            /// </signature>
            var that = this;
            this._date = new Date();
            this._promise
                .then(function () {
                    if (!that.options.portal) {
                        return _Promise.error();
                    }
                    return that.options.portalService.getByUriAsync(
                        that.options.portal.uri, "project,homepage,masterpage,pages");
                })
                .then(function (portal) {
                    that.options.portal = portal;
                    that._createTree(portal);
                });
        },
        _createTree: function (portal) {
            /// <signature>
            /// <param name="pages" type="Array" />
            /// </signature>
            var title = "<p>Weboldal: ";
            title += "<a data-action='openItemAction' data-action-params='portal'>";
            title += portal.name;
            title += "</a></p>";
            if (portal.project) {
                title += "<p>Projekt: ";
                title += "<a data-action='openItemAction' data-action-params='project'>";
                title += portal.project.name;
                title += "</a></p>";
            }
            var source = [{
                icon: false,
                expanded: true,
                data: new _Portal.PageNode(portal, null, this.options.portalService, this.options.pageService),
                title: title,
                tooltip: portal.domain ? "/" + portal.domain : portal.uri,
                children: portal.pages.map(this._createNode, this)
            }];
            if (this.uiMenu) {
                this.uiMenu.detach();
            }
            if (this.uiTree.is(":data(uiFancytree)")) {
                this.uiTree.fancytree("option", "source", source);
                return;
            }
            this.uiTree.fancytree({
                extensions: ["table", "dnd"],
                minExpandLevel: 2,
                selectMode: 1,
                source: source,
                activate: this._onActivateBound,
                removeNode: this._onRemoveNodeBound,
                renderColumns: this._renderColumnsBound,
                table: {
                    indentation: 20,
                    nodeColumnIdx: 2
                },
                dnd: {
                    preventRecursiveMoves: true,
                    preventVoidMoves: true,
                    dragStart: this._onDragStartBound,
                    dragEnter: this._onDragEnterBound,
                    dragLeave: this._onDragLeaveBound,
                    dragDrop: this._onDragDropBound
                }
            });
        },
        _createNode: function (page) {
            /// <signature>
            /// <param name="page" type="Object" />
            /// <returns type="Object" />
            /// </signature>
            var pageNode = new _Portal.PageNode(
                this.options.portal,
                page,
                this.options.portalService,
                this.options.pageService);
            return {
                key: pageNode.uri,
                expanded: true,
                title: pageNode.name,
                tooltip: pageNode.uri,
                data: pageNode,
                children: page.children ? page.children.map(this._createNode, this) : null
            };
        },
        _renderColumns: function (event, data) {
            /// <signature>
            /// <summary>Raised when the tree is rendered</summary>
            /// </signature>
            var portal = this.options.portal;
            var page = data.node.data;
            var masterPage = page.master || portal.masterPage;
            var dateInfo = this._getModifiedDateInfo(page.modifiedDate);
            var index = this._getNodeIndex(data.node);

            var tr = $(data.node.tr);
            var td = tr.children().removeClass();//.find(">td");
            var tdState = td.eq(0);
            var tdIndex = td.eq(1).addClass("portal-tree-col-index");
            var tdNode = td.eq(2).addClass("portal-tree-col-title");
            var tdMaster = td.eq(3).addClass("portal-tree-col-master");
            var tdDate = td.eq(4).addClass("portal-tree-col-date");

            // Highlight pages which are up-to-date
            if (dateInfo) {
                tdState.addClass(dateInfo.css);
                tdDate.text(dateInfo.dateString);
            }

            if (index) {
                tdIndex.text(index);
            } else {
                tdState.colspan = 2;
                tdState.html('<button class="ui-btn ui-btn-primary" type="button" data-action="addNew">Új oldal</button>');
            }

            portal.masterPage
                && portal.masterPage.uri === page.uri
                && td.addClass("portal-tree-masterpage");

            portal.homePage
                && portal.homePage.uri === page.uri
                && td.addClass("portal-tree-homepage");

            // Cannot be master of itself
            if (!masterPage || masterPage.uri === page.uri) {
                return;
            }

            tdMaster.addClass(page.master
                ? "portal-tree-masterpage-modified"
                : "portal-tree-masterpage")
                .attr("title", masterPage.name)
                .text(masterPage.name);
        },
        _getNodeIndex: function (node) {
            /// <signature>
            /// <param name="node" type="Object" />
            /// <returns type="String" />
            /// </signature>
            var skip = true;
            var nums = [];
            $.each(node.getParentList(false, true), function (i, o) {
                if (skip) {
                    skip = false;
                    return;
                }
                nums.push(o.getIndex() + 1);
            });
            return nums.join('.');
        },
        _getModifiedDateInfo: function (dateString) {
            /// <signature>
            /// <summary>Gets a color code for the given date</summary>
            /// </signature>
            if (!dateString) {
                return;
            }
            var date = new Date(Date.parse(dateString));
            var info = {
                css: null,
                date: date,
                elapsed: this._date.elapsed(date),
                dateString: _Resources.format(date, "F")
            };
            if (info.elapsed.weeks) {
                return info;
            }
            if (info.elapsed.days >= 1) {
                info.css = "portal-tree-modified-earlier";
            } else if (info.elapsed.hours >= 1) {
                info.css = "portal-tree-modified-recently";
            } else {
                info.css = "portal-tree-modified-present";
            }
            return info;
        },

        //
        // Event Handlers
        //

        _onActivate: function (event, data) {
            /// <signature>
            /// <summary>Fires when a node is activated</summary>
            /// </signature>
            var page = data.node.data;
            if (!page || !page.uri) {
                return;
            }
            data.node.setActive(false);
            data.node.setFocus(false);
            page.openItemAction("designer");
        },
        _onRemoveNode: function (event, data) {
            /// <signature>
            /// <summary>Fires when a node is removed.</summary>
            /// </signature>
            if (this.uiMenu.is(":not(:data(ui-dropdown))")) {
                // TODO: We need reinitialize the dropdown menu because fancytree
                // kills it. It can be avoided if $.ui.dropdown will support jQuery positions.
                this.uiMenu.dropdown();
            }
        },
        _onClick: function (event) {
            /// <signature>
            /// <summary>Fires when the user clicks a menu item</summary>
            /// </signature>
            var node = $.ui.fancytree.getNode(event.target);
            var page = node && node.data;
            if (!page) {
                return;
            }
            var $target = $(event.currentTarget);
            var action = $target.data("action");
            if (action) {
                var actionParams = $target.data("action-params");
                if (action === "toggleMenu") {
                    this.uiMenu.dropdown("toggle");
                    return false;
                }
                var that = this;
                page[action](actionParams)
                    .then(function (data) {
                        var state = data && data.state;
                        if (state === "created" || state === "modified") {
                            that.refresh();
                            //node.render(true);
                        } else if (state === "deleted") {
                            node.remove();
                        }
                    });
            }
            return false;
        },
        _onMouseEnter: function (event) {
            /// <signature>
            /// <summary>Fires when the user moves the mouse pointer into the object</summary>
            /// </signature>
            var tr = $(event.currentTarget);
            if (tr.index() > 0) {
                tr.find(".fancytree-node:first").append(this.uiMenu);
                this.uiMenu.dropdown("realign");
            }
        },
        _onDragStart: function () {
            return true;
        },
        _onDragEnter: function (node, data) {
            // Highlight the master page column when the cursor is over it
            var target = $(data.originalEvent.target);
            if (target.hasClass("portal-tree-col-master")) {
                target.addClass("portal-tree-dragenter");
            }
            // Don't allow changing the order (before, after)
            return ["over"];
        },
        _onDragLeave: function (node) {
            $(".portal-tree-col-master", node.tr).removeClass("portal-tree-dragenter");
        },
        _onDragDrop: function (node, data) {
            /// <signature>
            /// <summary>Drops data</summary>
            /// </signature>
            var nodeSrc = data.otherNode;
            var nodeDst = data.node;

            /// <var type="PI.Portal.PageNode" />
            var pageSrc = nodeSrc.data;
            /// <var type="PI.Portal.PageNode" />
            var pageDst = nodeDst.data;

            if (pageSrc && pageDst) {

                // Change the layout (master) page when the cursor is over the master page column
                if ($(data.originalEvent.target).hasClass("portal-tree-col-master")) {

                    // If the master page equals the portal's master page, reset the page's master page
                    var newMasterUri = (
                        this.options.portal.masterPage &&
                        this.options.portal.masterPage.uri
                    ) === pageSrc.uri ? null : pageSrc.uri;

                    pageDst.setMaster(pageSrc).then(function () {
                        //pageDst.master = newMasterUri !== null ? pageSrc : null;
                        nodeDst.render(true);
                    });
                    /*
                    this.options.pageService.setMasterAsync(pageDst.uri, newMasterUri, this).then(
                        function () {
                            pageDst.master = newMasterUri !== null ? pageSrc : null;
                            nodeDst.render(true);
                        });*/

                } else {
                    // Otherwise, change the parent of the page
                    pageSrc.move(pageDst).then(function () {
                        nodeSrc.moveTo(node, data.hitMode);
                    });
                }
            }
        },
        _destroy: function () {
            /// <signature>
            /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            /// </signature>
            this.uiMenu = null;
            this.uiTree && this.uiTree.fancytree("destroy");
            this.uiTree = null;
            this.empty();
        }
    });

    //
    // Public UI & Dialog Extensions
    //

    _PI.component({
        name: "portal.pages",
        view: function (model, options) {
            /// <signature>
            /// <param name="model" type="Object" />
            /// <param name="options" type="Object" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            $(options.element).PortalPages(options.params);
        },
        dialog: function (model, options, response, callback) {
            /// <signature>
            /// <param name="model" type="Object" />
            /// <param name="options" type="Object" />
            /// <param name="response" type="Object" />
            /// <param name="callback" type="Function" />
            /// <returns type="$.WinJS.dialog" />
            /// </signature>
            return this._renderDialog({
                width: 850,
                height: 550,
                title: _T("pages"),
                buttons: null,
                scrollable: true,
                resizable: true
            });
        }
    });

})(window, jQuery, WinJS, PI, PI.Portal);