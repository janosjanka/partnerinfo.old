// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="../viewmodels/businesstag.js" />

(function (_PI, _Project) {
    "use strict";

    _PI.component({
        name: "project.businesstags",
        model: function (options) {
            /// <signature>
            /// <param name="options" type="Object" />
            /// <returns type="PI.Project.BusinessTagList" />
            /// </signature>
            return new _Project.BusinessTagList(options.params);
        },
        view: function (model, options) {
            /// <signature>
            /// <param name="model" type="PI.Project.BusinessTagList" />
            /// <param name="options" type="Object" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            return this._renderView(options.element, model, "koBusinessTagList");
        }
    });

})(PI, PI.Project);