// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

PI.Portal.Modules.register("scroller", {
    options: {
        container: true
    },
    createModuleOptions: function (options) {
        return $.extend(this._superApply(arguments), {
            property: "bottom",
            cycle: 1000,
            duration: 60000,
            sensitivity: 50
        }, options);
    },
    _activate: function () {
        /// <signature>
        /// <summary>Activates this module.</summary>
        /// </signature>
        var options = this.getModuleOptions();
        this.scroller = new Scroller(this.element, {
            autoPlay: true,
            cycle: +options.cycle,
            duration: +options.duration,
            sensitivity: +options.sensitivity
        });
    },
    _deactivate: function () {
        /// <signature>
        /// <summary>Deactivates this module.</summary>
        /// </signature>
        this.scroller && this.scroller.dispose();
        this.scroller = null;
    }
});

var Scroller = WinJS.Class.define(function Scroller_ctor(element, options) {
    options || (options = {});
    this.element = element
        .attr("tabIndex", -1)
        .bind("click.Scroller", this.onclick.bind(this))
        .bind("keydown.Scroller", this.onkeydown.bind(this))
        .bind("mousewheel.Scroller DOMMouseScroll.Scroller", this.onmousewheel.bind(this));
    this.content = element.children(":first-child");
    this.cycle = options.cycle || 0;
    this.duration = options.duration || 60000;
    this.sensitivity = options.sensitivity || 50;
    this.reset();
    options.autoPlay && this.play();
}, {
    oncomplete: function () {
        this.cycle && async(function () {
            this.stop();
            this.play();
        }, this, this.cycle);
    },
    onclick: function () {
        this.toggle();
    },
    onkeydown: function (event) {
        switch (event.which) {
            case 32: // SPACE
                this.toggle();
                return false;
            case 36: // HOME PAGE
                this.stop();
                return false;
            case 38: // ARROW UP
                this.pause();
                this.scroll(this.sensitivity);
                return false;
            case 40: // ARROW DOWN
                this.pause();
                this.scroll(-this.sensitivity);
                return false;
        }
    },
    onmousewheel: function (event) {
        this.pause();
        var delta = event.originalEvent.wheelDelta || -event.originalEvent.detail;
        this.scroll(delta > 0 ? this.sensitivity : -this.sensitivity);
        return false;
    },
    isPlaying: function () {
        return this.content.is(":animated");
    },
    play: function () {
        var elHeight = this.element.height();
        var coHeight = this.content.outerHeight(true);
        var top = this.cycle ? -coHeight : elHeight - coHeight;
        this.content.animate(
            { top: top + "px" },
            this.duration,
            "linear",
            this.oncomplete.bind(this));
    },
    pause: function () {
        this.content.stop(true);
    },
    stop: function () {
        this.pause();
        this.reset();
    },
    toggle: function () {
        this.isPlaying() ? this.pause() : this.play();
    },
    reset: function () {
        this.content.css({
            position: "relative",
            top: this.element.height() + "px",
            bottom: null
        });
    },
    scroll: function (relPosition) {
        relPosition = relPosition || 0;
        if (relPosition) {
            var top = parseInt(this.content.css("top")) + relPosition;
            var elHeight = this.element.height();
            var coHeight = this.content.outerHeight(true);
            if (top < -coHeight) {
                top = -coHeight;
            } else if (top > elHeight) {
                top = elHeight;
            }
            this.content.css("top", top + "px");
        }
    },
    dispose: function () {
        this.stop();
        if (this.element) {
            this.element.unbind(".Scroller");
            this.content = null;
            this.element = null;
        }
    }
});