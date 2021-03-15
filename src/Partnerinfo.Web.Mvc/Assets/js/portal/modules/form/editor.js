// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="module.js" />

(function (Portal) {
    "use strict";

    PI.Portal.Modules.extend("form", {
        _onCreateEditDialog: function (callback) {
            /// <signature>
            /// <param name="callback" type="Function" />
            /// </signature>
            this._createEditDialog({
                model: {
                    action: ko.observable(),
                    actionSalt: ko.observable(),
                    task: {
                        action: ko.observable(),
                        salt: ko.observable()
                    },
                    content: ko.observable(this.element.html()),
                    create: function (module) {
                        this.actionSn = this.action.subscribe(function (value) {
                            value && this.module.postback(false);
                        }, this);
                        if (module.action && module.action && module.action.id) {
                            this.action(module.action);
                            this.actionSalt(module.action.salt);
                        }
                        if (module.task && module.task.action && module.task.action.id) {
                            this.task.action(module.task.action);
                            this.task.salt(module.task.action.salt);
                        }
                    },
                    submit: function (module) {
                        var action = this.action();
                        if (action) {
                            module.postback = false;
                            module.action = {
                                id: action.id,
                                name: action.name,
                                salt: this.actionSalt()
                            };
                        }
                        action = this.task.action();
                        if (action) {
                            module.task.action = {
                                id: action.id,
                                name: action.name,
                                salt: this.task.salt()
                            };
                        }
                    },
                    dispose: function () {
                        this.actionSn && this.actionSn.dispose();
                        this.actionSn = null;
                    }
                },
                complete: callback
            }, {
                width: 640
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

})(PI.Portal);