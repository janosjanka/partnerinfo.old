// Copyright (c) Partnerinfo Ltd. All Rights Reserved.
/// <reference path="http://cdnjs.cloudflare.com/ajax/libs/codemirror/4.8.0/codemirror.js" />

(function (ko) {
    "use strict";

    var CodeViewModel = function (editor, value) {
        /// <signature>
        /// <summary>Initializes a new instance of the CodeViewModel class.</summary>
        /// <param name="editor" type="CodeMirror" />
        /// <param name="value" type="ko.observable" />
        /// <returns type="CodeViewModel" />
        /// </signature>
        this.editor = editor;
        this.value = value;
        var code = ko.unwrap(this.value);
        code && this.editor.setValue(code);
        if (ko.isWritableObservable(this.value)) {
            this.editor.on("blur", this._editorBlur.bind(this));
            this.valueSn = this.value.subscribe(this._valueChanged, this);
        }
    };

    CodeViewModel.prototype._valueChanged = function (value) {
        /// <signature>
        /// <summary>Raised immediately after the value is changed.</summary>
        /// <param name="value" type="String" />
        /// </signature>
        !this._refresh && this.editor.setValue(value || "");
    };

    CodeViewModel.prototype._editorBlur = function (editor) {
        /// <signature>
        /// <summary>Fires every time the content of the editor is changed.</summary>
        /// <param name="editor" type="CodeMirror" />
        /// </signature>
        this._refresh = true;
        this.value(editor.getValue());
        this._refresh = false;
    };

    CodeViewModel.prototype.dispose = function () {
        /// <signature>
        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        /// </signature>
        this.valueSn && this.valueSn.dispose();
        this.valueSn = null;
    };

    ko.components.register("ui-codemirror", {
        viewModel: {
            createViewModel: function (params, info) {
                /// <signature>
                /// <summary>Creates a viewModel for presenting data errors</summary>
                /// </signature>
                params = params || {};
                params.codemirror = params.codemirror || {};
                params.codemirror.lineNumbers = params.codemirror.lineNumbers !== false;
                params.codemirror.styleActiveLine = params.codemirror.styleActiveLine !== false;
                params.codemirror.autoCloseBrackets = params.codemirror.autoCloseBrackets !== false;
                params.codemirror.autoCloseTags = params.codemirror.autoCloseTags !== false;
                params.codemirror.matchBrackets = params.codemirror.matchBrackets !== false;
                params.codemirror.matchTags = params.codemirror.matchTags || { bothTags: true };
                params.codemirror.indentUnit = params.codemirror.indentUnit || 4;
                var editor = CodeMirror.fromTextArea(info.element, params.codemirror);
                editor.setSize(params.width, params.height);
                return new CodeViewModel(editor, params.value);
            }
        },
        template: " "
    });

})(ko);
