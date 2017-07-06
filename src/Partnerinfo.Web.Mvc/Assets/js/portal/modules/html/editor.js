// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

PI.Portal.Modules.extend("html", {
    _onCreateEditDialog: function (complete) {
        /// <signature>
        /// <param name="complete" type="Function" />
        /// </signature>
        this._createEditDialog({
            model: {
                content: ko.observable(this.element.html())
            },
            complete: complete
        }, {
            width: 850,
            height: window.innerHeight,
            resizable: true,
            overflow: "hidden"
        });
    },
    _onSubmitEditDialog: function (editor, options) {
        /// <signature>
        /// <param name="editor" type="Object" />
        /// <param name="options" type="Object" />
        /// </signature>
        this._super(editor, options);
        this.element.html(ko.unwrap(options.model.content));
    }
});