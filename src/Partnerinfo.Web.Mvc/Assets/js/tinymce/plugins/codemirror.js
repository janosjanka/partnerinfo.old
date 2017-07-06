// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="../../base/tinymce/tinymce.min.js" />
/// <reference path="../../base/beautify/beautify.js" />
/// <reference path="../../base/beautify/beautify-html.js" />
/// <reference path="http://cdnjs.cloudflare.com/ajax/libs/codemirror/4.8.0/codemirror.js" />

tinymce.PluginManager.add("codemirror", WinJS.Class.define(function (editor, url) {
    /// <signature>
    /// <param name="editor" type="tinymce.Editor" />
    /// </signature>
    var that = this;
    this.editor = editor;
    this.editor.addButton("codemirror", {
        image: PI.Server.mapPath("/ss/tinymce/plugins/codemirror.png"),
        tooltip: "HTML",
        onclick: this.showDialog.bind(this, this.editor)
    });
    this.editor.addMenuItem("codemirror", {
        image: PI.Server.mapPath("/ss/tinymce/plugins/codemirror.png"),
        text: "HTML",
        context: "tools",
        onclick: this.showDialog.bind(this, this.editor)
    });
}, {
    showDialog: function (editor) {
        /// <signature>
        /// <summary>Shows a dialog window</summary>
        /// </signature>
        require(["codemirror", "codemirror.css"]).done(function () {
            var codeEditor;
            var textarea = document.createElement("textarea");
            var dialog = $.WinJS.dialog({
                width: 850,
                height: window.innerHeight,
                overflow: "hidden",
                position: { my: "center top", at: "center top" },
                resizable: true,
                title: "HTML",
                content: textarea,
                buttons: [{
                    "class": "ui-btn ui-btn-primary",
                    "text": _T("command.done"),
                    "click": function () {
                        codeEditor && editor.setContent(codeEditor.getValue());
                        $(this).dialog("close");
                    }
                }, {
                    "class": "ui-btn",
                    "text": _T("command.cancel"),
                    "click": function () {
                        $(this).dialog("close");
                    }
                }],
                open: function () {
                    $(this).dialog("progress", WinJS.Promise.timeout().then(function () {
                        return WinJS.Promise(function (completeDispatch) {
                            textarea.innerHTML = html_beautify(editor.getContent(), {
                                "indent_size": 4,
                                "indent_char": " ",
                                "indent_level": 0,
                                "indent_with_tabs": false,
                                "preserve_newlines": false,
                                "max_preserve_newlines": 10,
                                "jslint_happy": false,
                                "space_after_anon_function": false,
                                "brace_style": "collapse",
                                "keep_array_indentation": false,
                                "keep_function_indentation": false,
                                "space_before_conditional": true,
                                "break_chained_methods": false,
                                "eval_code": false,
                                "unescape_strings": false,
                                "wrap_line_length": 0
                            });
                            codeEditor = CodeMirror.fromTextArea(textarea, {
                                theme: "visual-studio visual-studio-html",
                                mode: "text/html",
                                htmlMode: true,
                                lineNumbers: true,
                                styleActiveLine: true,
                                autoCloseBrackets: true,
                                autoCloseTags: true,
                                matchBrackets: true,
                                matchTags: { bothTags: true },
                                indentUnit: 4
                            });
                            codeEditor.setSize("100%", "100%");
                            completeDispatch();
                        })
                    }));
                },
                close: function () {
                    editor.focus();
                }
            });
        });
    }
}));