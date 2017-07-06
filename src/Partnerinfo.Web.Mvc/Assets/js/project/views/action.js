// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="../viewmodels/action.js" />

(function (_WinJS, _PI, _Project) {
    "use strict";

    _PI.component({
        name: "project.action",
        model: function (options) {
            /// <signature>
            /// <param name="options" type="Object" />
            /// <returns type="PI.Project.Action" />
            /// </signature>
            var params = options.params || {};
            var action = new _Project.Action(params.project, params.action, params);
            action.exists() ? action.loadAsync() : action.beginEdit();
            return action;
        },
        view: function (model, options) {
            /// <signature>
            /// <param name="model" type="PI.Project.Action" />
            /// <param name="options" type="Object" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            return this._renderView(options.element, model, "koActionItem");
        },
        dialog: function (model, options, result, callback) {
            /// <signature>
            /// <param name="model" type="PI.Project.Action" />
            /// <param name="options" type="Object" />
            /// <param name="result" type="Object" />
            /// <param name="callback" type="Function" />
            /// <returns type="$.WinJS.dialog" />
            /// </signature>
            return this._renderDialog({
                width: 700,
                height: 500,
                title: _T("action"),
                buttons: [{
                    "class": "ui-btn",
                    "text": _T("ui/done"),
                    "click": callback.bind(model, "ok")
                }, {
                    "class": "ui-btn",
                    "text": _T("ui/close"),
                    "click": callback.bind(model, "cancel")
                }]
            });
        }
    });

    _PI.component({
        name: "project.actions",
        model: function (options) {
            /// <signature>
            /// <param name="options" type="Object" />
            /// <returns type="PI.Project.ActionList" />
            /// </signature>
            return new _Project.ActionList(options.params);
        },
        view: function (model, options) {
            /// <signature>
            /// <param name="model" type="PI.Project.ActionList" />
            /// <param name="options" type="Object" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            return this._renderView(options.element, model, "koActionList");
        }
    });

    _PI.component({
        name: "project.action-picker",
        model: function (options) {
            /// <signature>
            /// <param name="options" type="Object" />
            /// <returns type="PI.Project.ActionList" />
            /// </signature>
            return new _Project.ActionList(options);
        },
        view: function (model, options) {
            /// <signature>
            /// <param name="model" type="PI.Project.ActionList" />
            /// <param name="options" type="Object" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            return this._renderView(options.element, model, "koActionPicker");
        },
        dialog: function (model, options, result, callback) {
            /// <signature>
            /// <param name="model" type="PI.Project.ActionList" />
            /// <param name="options" type="Object" />
            /// <param name="result" type="Object" />
            /// <param name="callback" type="Function" />
            /// <returns type="$.WinJS.dialog" />
            /// </signature>
            return this._renderDialog({
                width: 800,
                height: 480,
                title: _T("actions"),
                buttons: [{
                    "class": "ui-btn ui-btn-primary",
                    "text": _T("ui/select"),
                    "click": function () {
                        result.items = model.selection.peek();
                        callback.call(model, "ok");
                    }
                }, {
                    "class": "ui-btn",
                    "text": _T("ui/close"),
                    "click": callback.bind(model, "cancel")
                }]
            });
        }
    });

    _PI.component({
        name: "project.actionlink",
        model: function (options) {
            /// <signature>
            /// <param name="options" type="Object" />
            /// <returns type="Object" />
            /// </signature>
            return new _Project.ActionLink(options.actionlink);
        },
        view: function (model, options) {
            /// <signature>
            /// <param name="model" type="PI.Project.ActionLink" />
            /// <param name="options" type="Object" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            return this._renderView(options.element, model, "koActionLink");
        },
        dialog: function (model, options, result, callback) {
            /// <signature>
            /// <param name="model" type="PI.Project.ActionLink" />
            /// <param name="options" type="Object" />
            /// <param name="result" type="Object" />
            /// <param name="callback" type="Function" />
            /// <returns type="$.WinJS.dialog" />
            /// </signature>
            var title = _T("pi/project/actionLink");
            var dialog = this._renderDialog({
                overflow: "visible",
                minWidth: 600,
                title: title,
                buttons: [{
                    "class": "ui-btn",
                    "text": _T("ui/close"),
                    "click": callback.bind(model, "cancel")
                }, {
                    "class": "ui-btn ui-hidden",
                    "text": _T("ui/edit"),
                    "click": function () {
                        _PI.dialog({
                            name: "project.action",
                            params: {
                                action: model.action()
                            }
                        });
                    }
                }],
                close: function () {
                    actionSn.dispose();
                    model.dispose();
                }
            });
            var actionSn = model.action.subscribe(function (action) {
                var editBtn = dialog.uiButtonSet.find("button:nth-child(2)");
                if (action) {
                    dialog.option("title", title + " - " + action.name);
                    editBtn.removeClass("ui-hidden");
                } else {
                    dialog.option("title", title);
                    editBtn.addClass("ui-hidden");
                }
            });
            return dialog;
        }
    });

    _PI.component({
        name: "project.actionlink-element",
        model: function (options) {
            /// <signature>
            /// <param name="options" type="Object" />
            /// <returns type="Object" />
            /// </signature>
            return new _Project.ActionLinkNode(options);
        },
        view: function (model, options) {
            /// <signature>
            /// <param name="model" type="PI.Project.ActionLink" />
            /// <param name="options" type="Object" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            return this._renderView(options.element, model, "koActionLinkElement");
        },
        dialog: function (model, options, result, callback) {
            /// <signature>
            /// <param name="model" type="PI.Project.ActionLink" />
            /// <param name="options" type="Object" />
            /// <param name="result" type="Object" />
            /// <param name="callback" type="Function" />
            /// <returns type="$.WinJS.dialog" />
            /// </signature>
            return this._renderDialog({
                overflow: "visible",
                minWidth: 600,
                title: _T("pi/project/actionLink"),
                buttons: [{
                    "class": "ui-btn ui-btn-primary",
                    "text": _T("ui/done"),
                    "click": callback.bind(model, "done")
                }, {
                    "class": "ui-btn",
                    "text": _T("ui/cancel"),
                    "click": callback.bind(model, "cancel")
                }, {
                    "class": "ui-btn",
                    "text": _T("ui/remove"),
                    "click": callback.bind(model, "remove")
                }]
            });
        }
    });

})(WinJS, PI, PI.Project);
