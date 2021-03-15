// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

PI.Portal.Modules.extend("animatedPanel", {
    showPreview: function (element, moduleOptions) {
        moduleOptions = moduleOptions || this.getModuleOptions();
        var panel = this.element
            .clone(false)
            .removeAttr("ui-module-mode-edit ui-module-state-selected")
            .css({
                display: "",
                position: "",
                visibility: "",
                float: "",
                left: "",
                top: "",
                right: "",
                bottom: "",
                margin: 0,
                opacity: "",
                zIndex: ""
            })
            .appendTo(element);
        this.options.engine.context.parseAll(null, panel).always(function () {
            panel.cycle({
                backwards: moduleOptions.backwards,
                fx: moduleOptions.fx,
                random: moduleOptions.random,
                speed: moduleOptions.speed,
                timeout: moduleOptions.timeout,
                pause: moduleOptions.pause,
                prev: moduleOptions.prev,
                next: moduleOptions.next,
                prevNextEvent: moduleOptions.prevNextEvent
            });
        });
    },
    _onCreateEditDialog: function (complete) {
        var that = this;
        this._createEditDialog({
            model: {
                effectOptions: this._createEffectOptions(),
                selectedEffect: ko.observable("none"),
                showPreview: function () {
                    var moduleOptions = ko.mapping.toJS(this.module);
                    $.WinJS.dialog({
                        position: { my: "center top", at: "center top" },
                        width: "auto",
                        height: "auto",
                        open: function () { that.showPreview(this, moduleOptions); }
                    });
                }
            },
            complete: complete
        }, {
            width: 700,
            height: 420
        });
    },
    _createEffectOptions: function () {
        var transitions = $.fn.cycle.transitions;
        var hasOwnProperty = Object.prototype.hasOwnProperty;
        var options = [];
        for (var p in transitions) {
            if (hasOwnProperty.apply(transitions, [p])) {
                options.push(p);
            }
        }
        return options.sort();
    }
});
