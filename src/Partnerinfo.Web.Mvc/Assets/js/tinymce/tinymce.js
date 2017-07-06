// Copyright (c) Partnerinfo Ltd. All rights reserved.
/// <reference path="../base/tinymce/tinymce.min.js" />

$.widget("PI.tinymce", {
    options: {
        project: null,       // Project
        container: null,     // A container element used to calculate positions
        bootstrap: false,    // Load the bootstrap CSS library
        content_style: null, // Inline CSS styles
        autoresize: false,   // Allows to change sizes dynamically
        width: "100%",       // Width
        height: "100%",      // Height
        language: "hu_HU",   // Language
        relative_urls: true, // Converts URLs to relative
        inline: false,
        oninit: $.noop,
        onchange: $.noop
    },
    _create: function () {
        /// <signature>
        /// <summary>Creates a new tinymce widget</summary>
        /// </signature>
        var that = this;
        var fireChange = this.fireChange.bind(this);
        var config = {
            project: this.options.project,
            content_css: PI.Server.mapPath("/tinymce/content.css?v=" + PI.config.version),
            content_style: this.options.content_style,
            plugins: "noneditable link placeholders projectforms advlist lists textcolor colorpicker paste table image media charmap searchreplace contextmenu codemirror actionlink",
            toolbar1: "undo redo | bold italic underline fontselect fontsizeselect forecolor backcolor numlist bullist",
            toolbar2: "table | link actionlink placeholders projectforms image media charmap | indent outdent alignleft aligncenter alignright alignjustify | styleselect | removeformat codemirror",
            width: this.options.width,
            height: this.options.container ? (this.options.container.outerHeight() - this.element.position().top - 72) : this.options.height,
            language: this.options.language,
            theme: "modern",
            menubar: false,
            statusbar: false,
            schema: "html5",
            entity_encoding: "raw",
            force_hex_style_colors: true,
            forced_root_block: false,
            convert_urls: true,
            relative_urls: this.options.relative_urls,
            image_advtab: true,
            inline: this.options.inline,
            noneditable_regexp: PI.Expression.regexpVisual,
            setup: function (editor) {
                editor.on("init", function () {
                    that.options.oninit();
                    editor
                        .on("blur", fireChange)
                        //.on("change", fireChange)
                        //.on("SetAttrib", fireChange)
                        .on("setContent", fireChange);
                });
            }
        };
        if (this.options.bootstrap) {
            config.content_css += config.content_css + "," + document.getElementById("pi-bootstrap-css").href;
        }
        if (this.options.autoresize) {
            config.plugins += " autoresize";
        }
        this.editor = new tinymce.Editor(this.element[0].id, config, tinymce.EditorManager);
        async(this.editor.render, this.editor);
    },
    fireChange: function () {
        /// <signature>
        /// <param name="context" type="String" />
        /// </signature>
        this.options.onchange(this.editor.getContent());
        WinJS.DEBUG && WinJS.log({ type: "info", category: "PI.tinymce", message: "change" });
    },
    getContent: function (options) {
        /// <signature>
        /// <param name="options" type="Object" />
        /// <returns type="String" />
        /// </signature>
        return this.editor.initialized
            ? this.editor.getContent(options)
            : this.element.val();
    },
    setContent: function (content) {
        /// <signature>
        /// <param name="content" type="String" />
        /// </signature>
        this.element.val(content);
        this.editor.initialized && this.editor.setContent(content || "");
    },
    _destroy: function () {
        /// <signature>
        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        /// </signature>
        if (this.editor) {
            this.editor
                .off("setContent")
                //.off("SetAttrib")
                //.off("change")
                .off("blur")
                .off("init");
            this.editor.remove();
        }
        this.editor = null;
    }
});

ko.components.register("pi-tinymce", {
    viewModel: WinJS.Class.define(function TinyMceViewModel_ctor(params) {
        /// <signature>
        /// <summary>Initializes a new instance of the TinyMceViewModel class.</summary>
        /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the TinyMceViewModel.</param>
        /// <returns type="TinyMceViewModel" />
        /// </signature>
        params = params || {};
        this._updating = false;
        this.element = undefined;
        this.id = params.id || "tinymce_editor";
        this.name = params.name || "html";
        this.value = params.value;
        this.options = params.options;

        if (ko.isObservable(this.value)) {
            this.valueSn = this.value.subscribe(this._valueChanged, this);
            this.options.oninit = this._editorInit.bind(this);
            this.options.onchange = this._editorChanged.bind(this);
        }
    }, {
        _valueChanged: function (value) {
            /// <signature>
            /// <summary>Raised immediately after the value is changed</summary>
            /// <param name="value" type="String" />
            /// </signature>
            if (this.element && !this._updating) {
                this.element.tinymce("setContent", value);
            }
        },
        _editorInit: function () {
            /// <signature>
            /// <summary>Raised immediately after the editor is initialized</summary>
            /// </signature>
            this.element = this.element || $(document.getElementById(this.id));
            this.element.tinymce("setContent", this.value());
        },
        _editorChanged: function (content) {
            /// <signature>
            /// <summary>Raised immediately after the editor's content is changed</summary>
            /// <param name="content" type="String" />
            /// </signature>
            this._updating = true;
            this.value(content);
            this._updating = false;
        },
        dispose: function () {
            /// <signature>
            /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            /// </signature>
            this.valueSn && this.valueSn.dispose();
            this.valueSn = null;
        }
    }),
    template: '<textarea data-bind=\'attr: { id: id, name: name }, jQuery: "PI.tinymce", jQueryOptions: options\'></textarea>'
});