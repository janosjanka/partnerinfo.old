// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="../viewmodels/project.js" />

(function (_WinJS, _PI, _Project) {
    "use strict";

    _PI.component({
        name: "project",
        model: function (options) {
            /// <signature>
            /// <param name="options" type="Object" />
            /// <returns type="PI.Project.ProjectItem" />
            /// </signature>
            var params = options.params || {};
            var project = new _Project.ProjectItem(params.project, params);
            project.exists() && project.loadAsync();
            return project;
        },
        view: function (model, options) {
            /// <signature>
            /// <param name="model" type="PI.Project.ProjectItem" />
            /// <param name="options" type="Object" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            return _WinJS.Promise.join(
                this._renderView(options.element, model, "koProjectItem"),
                this._renderView(options.toolbar, model, "koProjectMenu")
            );
        }
    });

    _PI.component({
        name: "project-picker",
        model: function (options) {
            /// <signature>
            /// <param name="options" type="Object" />
            /// <returns type="PI.Project.ProjectList" />
            /// </signature>
            return new _Project.ProjectList(options);
        },
        view: function (model, options) {
            /// <signature>
            /// <param name="model" type="PI.Project.ProjectList" />
            /// <param name="options" type="Object" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            return this._renderView(options.element, model, "koProjectList");
        },
        dialog: function (model, options, response, callback) {
            /// <signature>
            /// <param name="model" type="PI.Project.ProjectList" />
            /// <param name="options" type="Object" />
            /// <param name="response" type="Object" />
            /// <param name="callback" type="Function" />
            /// <returns type="$.WinJS.dialog" />
            /// </signature>
            return this._renderDialog({
                width: 950,
                height: 450,
                title: _T("projects"),
                buttons: [{
                    "class": "ui-btn ui-btn-primary",
                    "text": _T("ui/done"),
                    "click": function () {
                        response.items = ko.unwrap(model.selection);
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
        name: "projects",
        model: function (options) {
            /// <signature>
            /// <param name="options" type="Object" />
            /// <returns type="PI.Project.ProjectList" />
            /// </signature>
            var params = options.params || {};
            params.filter = params.filter || { orderBy: "recent", fields: "owners,statistics" };
            return new _Project.ProjectList(params);
        },
        view: function (model, options) {
            /// <signature>
            /// <param name="model" type="PI.Project.ProjectList" />
            /// <param name="options" type="Object" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            return _WinJS.Promise.join(
                this._renderView(options.element, model, "koProjectList"),
                this._renderView(options.toolbar, model, "koProjectListMenu"));
        }
    });

})(WinJS, PI, PI.Project);
