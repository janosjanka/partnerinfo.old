// Copyright (c) Partnerinfo Ltd. All Rights Reserved.
/// <reference path="engine.js" />

(function (_KO, _PI) {
    "use strict";

    _KO.components.register("pi-project-search", {
        viewModel: function (params) {
            /// <signature>
            /// <summary></summary>
            /// </signature>
            this.engine = new _PI.Project.Search.Engine(params);
        },
        template: {
            path: "project/search/search.html"
        }
    });

    _PI.component({
        name: "search",
        model: function (options) {
            return new _PI.Project.Search.Engine(options.model);
        },
        view: function (model, options) {
            return _PI.bind(options.element, model, "koSearch");
        }
    });

})(ko, PI);
