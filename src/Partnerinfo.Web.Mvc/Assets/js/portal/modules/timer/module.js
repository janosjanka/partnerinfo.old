// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="../base.js" />

(function (_Portal, _Resources) {
    "use strict";

    PI.Portal.Modules.register("timer", {
        createModuleOptions: function (options) {
            /// <signature>
            /// <summary>Extends default module options with the specified module options.</summary>
            /// <param name="options" type="Object" optional="true">A set of key/value pairs that can be used to configure module options.</param>
            /// <returns type="Object" />
            /// </signature>
            return $.extend(this._superApply(arguments), {
                relative: false,
                format: "{ddd} {hh}:{mm}:{ss}",
                days: null,
                hours: null,
                minutes: null,
                seconds: null,
                expireDate: null, // Used to set an absolute time value in UTC.
                expireText: null,
                event: null
            }, options);
        },
        _activate: function () {
            /// <signature>
            /// <summary>Activates this module.</summary>
            /// </signature>
            var moduleOptions = this.getModuleOptions();
            var expireDate;
            if (moduleOptions.relative) {
                expireDate = new Date()
                    .addDays(parseInt(moduleOptions.days, 10) || 0)
                    .addHours(parseInt(moduleOptions.hours, 10) || 0)
                    .addMinutes(parseInt(moduleOptions.minutes, 10) || 0)
                    .addSeconds(parseInt(moduleOptions.seconds, 10) || 0);
            } else {
                expireDate = new Date(moduleOptions.expireDate);
            }
            this._countdown({
                date: expireDate,
                tick: this._tick.bind(this, moduleOptions.format),
                complete: this._done.bind(this, moduleOptions.expireText, moduleOptions.event)
            });
        },
        _deactivate: function () {
            /// <signature>
            /// <summary>Deactivates this module.</summary>
            /// </signature>
            this.element.empty();
        },
        _countdown: function (options) {
            /// <signature>
            ///	<summary>Starts the countdown timer (TODO: optimize this function).</summary>
            /// <param name="options" type="Object">A set of key/value pairs.</param>
            /// </signature>
            var dateNow = new Date();
            var amount = options.date - dateNow;
            if (amount < 0) {
                if ($.isFunction(options.complete)) {
                    options.complete.apply(this, []);
                }
                return;
            }
            var days = 0, hours = 0, mins = 0, secs = 0;
            amount = Math.floor(amount / 1000);
            days = Math.floor(amount / 86400);
            amount = amount % 86400;
            hours = Math.floor(amount / 3600);
            amount = amount % 3600;
            mins = Math.floor(amount / 60);
            amount = amount % 60;
            secs = Math.floor(amount);
            if ($.isFunction(options.tick) &&
                options.tick.apply(this, [days, hours, mins, secs]) === true) {
                return;
            }
            var that = this;
            var timeoutId = window.setTimeout(function () {
                window.clearTimeout(timeoutId);
                that._countdown(options);
            }, 1000);
        },
        _tick: function (format, days, hours, mins, secs) {
            /// <signature>
            /// <summary>Occurs when the specified timer interval has elapsed and the timer is enabled.</summary>
            /// </signature>
            if (this.options.mode === _Portal.ModuleState.edit) {
                this._deactivate();
                return true;
            }
            if (format) {
                this.element.text(format.replace(
                    /{((d|h|m|s){1,})}/g,
                    function (item) {
                        var value = item.substring(1, item.length - 1), v;
                        switch (value[0]) {
                            case 'd': v = days; break;
                            case 'h': v = hours; break;
                            case 'm': v = mins; break;
                            case 's': v = secs; break;
                        }
                        return _Resources.format(v, "d" + value.length);
                    }));
            }
        },
        _done: function (expireText, event) {
            /// <signature>
            /// <summary>Occurs when a timer operation is complete.</summary>
            /// </signature>
            if (this.options.mode === _Portal.ModuleState.edit) {
                this._deactivate();
                return;
            }
            expireText && this.element.text(expireText);
            event && this.options.engine.context.executeEvent(event);
        }
    });

})(PI.Portal, WinJS.Resources);
