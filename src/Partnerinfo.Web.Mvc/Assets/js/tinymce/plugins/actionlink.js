// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

tinymce.PluginManager.add("actionlink", WinJS.Class.define(function (editor) {
    /// <signature>
    /// <param name="editor" type="tinymce.Editor" />
    /// </signature>
    this.res = _T("tinymce.actionlink");
    this.bookmark = null;
    this.editor = editor;
    this.editor.addButton("actionlink", {
        image: PI.Server.mapPath("/ss/tinymce/plugins/actionlink.png"),
        onclick: this.edit.bind(this)
    });
    this.editor.on("click", this._onClick.bind(this));
    this.editor.on("keydown", this._onKeyDown.bind(this));
}, {
    edit: function () {
        /// <signature>
        /// <summary>Edits the action at the cursor.</summary>
        /// </signature>
        var project = this.editor.settings.project;
        if (!project) {
            $.WinJS.dialog({ content: this.res.projectRequired });
            return;
        }
        var that = this;
        this.bookmark = this.editor.selection.getBookmark();
        return require(["project"]).then(function () {
            var node = that.editor.selection.getNode();
            var linkNode = node && that.editor.dom.getParent(node, "a");
            PI.dialog({
                name: "project.actionlink-element",
                className: linkNode && linkNode.getAttribute("class"),
                href: linkNode && linkNode.getAttribute("href"),
                title: linkNode && linkNode.getAttribute("title"),
                text: linkNode && (linkNode.innerText || linkNode.textContent) || that.editor.selection.getContent({ format: "text" }),
                actionlink: {
                    project: project,
                    outputLink: that.getUrlFromNode(linkNode)
                },
                done: function (event) {
                    if (event.result === "done") {
                        that.updateLinkNode(linkNode, event.data);
                        return;
                    }
                    if (event.result === "remove") {
                        that.removeLinkNode(linkNode);
                    }
                }
            });
        });
    },
    updateLinkNode: function (node, data) {
        /// <signature>
        /// <param name="node" type="Element" />
        /// <param name="linkNodeData" type="Object" />
        /// </signature>
        var isNew = !node;
        var content = this.editor.selection.getContent() || data.text;
        node = node ? this.editor.dom.rename(node, "a") : this.editor.dom.create("a");
        this.editor.dom.setAttrib(node, "class", data.className);
        this.editor.dom.setAttrib(node, "href", data.actionlink.outputLink || "#");
        this.editor.dom.setAttrib(node, "target", data.target);
        this.editor.dom.setAttrib(node, "title", data.title);
        this.editor.dom.setHTML(node, content);
        if (isNew) {
            this.bookmark && this.editor.selection.moveToBookmark(this.bookmark);
            this.editor.selection.setNode(node);
        }
        this.editor.focus(false);
    },
    removeLinkNode: function (node) {
        /// <signature>
        /// <param name="node" type="Element" />
        /// </signature>
        if (!node) {
            return;
        }
        this.editor.dom.setOuterHTML(node, node.innerHTML, true);
        this.editor.focus(false);
    },
    getUrlFromNode: function (node) {
        /// <signature>
        /// <summary>Gets the unique identifier of the action from a DOM element.</summary>
        /// <param name="node" type="Element">A DOM element to parse.</param>
        /// <returns type="String" />
        /// </signature>
        return node && this.editor.dom.getAttrib(node, "href");
    },
    _onClick: function (event) {
        /// <summary>Occurs when user clicks on an element within the editor.</summary>
        /// <param name="editor" type="Object">TinyMCE instance.</param>
        /// <param name="element" type="Element">A DOM element.</param>
        /// <returns type="Boolean" />
        async(function () {
            // The async method fixes a tinyMCE bug when an user selects an action link with <img> tag.
            var node = event.target;
            if (node && node.nodeName === "A" || this.editor.dom.getParent(node, "A")) {
                this.edit();
            }
        }, this);
        return true;
    },
    _onKeyDown: function (event) {
        /// <summary>Occurs when the user is pressing a key or holding down a key.</summary>
        /// <param name="editor" type="Object">TinyMCE instance.</param>
        /// <param name="element" type="Element">A DOM element.</param>
        /// <returns type="Boolean" />
        var node = this.editor.selection.getNode();
        if (node && node.nodeName === "A" && this.getUrlFromNode(node)) {
            if (event.keyCode !== 33 && event.keyCode !== 34 && event.keyCode !== 35 && event.keyCode !== 36 && event.keyCode !== 37 && event.keyCode !== 38 && event.keyCode !== 39 && event.keyCode !== 40) {
                this.edit();
                event.returnValue = false;
                return false;
            }
        }
        return true;
    }
}));