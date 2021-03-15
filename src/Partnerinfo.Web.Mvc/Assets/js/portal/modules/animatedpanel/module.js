// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="../base.js" />

PI.Portal.Modules.register("animatedPanel", {
    options: {
        container: true
    },
    createModuleOptions: function (options) {
        /// <signature>
        /// <summary>Creates a new object that extends the given options with the module's default options.</summary>
        /// <param name="options" type="Object" optional="true" />
        /// <returns type="Object" />
        /// </signature>
        return $.extend(this._super(options), {
            backwards: false, // true to start slideshow at last slide and move backwards through the stack 
            fx: "fade", // name of transition effect (or comma separated names, ex: 'fade,scrollUp,shuffle') 
            random: 0, // true for random, false for sequence (not applicable to shuffle fx) 
            speed: 1000,  // speed of the transition (any valid fx speed value) 
            timeout: 4000, // milliseconds between slide transitions (0 to disable auto advance) 
            pause: false, // true to enable "pause on hover"
            startEvent: null, // event which starts the transition
            pauseEvent: null, // event which pauses the transition
            prev: null, // element, jQuery object, or jQuery selector string for the element to use as event trigger for previous slide
            next: null, // element, jQuery object, or jQuery selector string for the element to use as event trigger for next slide
            prevNextEvent: null // event which drives the manual transition to the previous or next slide
        }, options);
    },
    _activate: function () {
        /// <signature>
        /// <summary>Activates this module.</summary>
        /// </signature>
        var that = this;
        var moduleOptions = this.getModuleOptions();
        this.element.cycle({
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
        if (moduleOptions.startEvent) {
            this.element.cycle("pause").bind(
                moduleOptions.startEvent + ".PortalAnimatedPanelModule", function () {
                    that.element.cycle("resume");
                });
        }
        if (moduleOptions.pauseEvent) {
            this.element.bind(
                moduleOptions.pauseEvent + ".PortalAnimatedPanelModule", function () {
                    that.element.cycle("pause");
                });
        }
    },
    _deactivate: function () {
        /// <signature>
        /// <summary>Deactivates this module.</summary>
        /// </signature>
        this.element
            .unbind(".PortalAnimatedPanelModule")
            .cycle("destroy");
    }
});