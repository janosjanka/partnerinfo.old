// TimeSpan plugin v1.0
// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (globalize, ko, undefined) {
    "use strict";

    var _observable = ko.observable;

    function asNumber(n) {
        return n === undefined ? 0 : +n;
    }
    function localize(name) {
        return globalize ? globalize.localize(name) : name;
    }

    var TimeSpan = function (options) {
        /// <signature>
        /// <param name="options" type="Object" optional="true" />
        /// </param>
        /// <returns type="TimeSpan" />
        /// </signature>
        options = options || {};

        this._disposed = false;
        this._listening = true;

        this.daysText = options.daysText || localize("ui/timeSpanDays");
        this.hoursText = options.hoursText || localize("ui/timeSpanHours");
        this.minutesText = options.minutesText || localize("ui/timeSpanMinutes");
        this.secondsText = options.secondsText || localize("ui/timeSpanSeconds");

        this.days = _observable();
        this.hours = _observable();
        this.minutes = _observable();
        this.seconds = _observable();
        this.value = options.value;

        this.loadFromString(ko.unwrap(this.value));

        this._daysSn = this.days.subscribe(this.timeChanged, this);
        this._hoursSn = this.hours.subscribe(this.timeChanged, this);
        this._minutesSn = this.minutes.subscribe(this.timeChanged, this);
        this._secondsSn = this.seconds.subscribe(this.timeChanged, this);
    };
    TimeSpan.prototype.timeChanged = function () {
        /// <signature>
        /// <summary>Raised immediately after one of the time values has changed.</summary>
        /// </signature>
        if (!this._listening) {
            return;
        }
        this._listening = false;
        var time = this.toObject();
        var rem = time.seconds % 60;
        time.minutes += (time.seconds - rem) / 60;
        time.seconds = rem;
        rem = time.minutes % 60;
        time.hours += (time.minutes - rem) / 60;
        time.minutes = rem;
        rem = time.hours % 24;
        time.days += (time.hours - rem) / 24;
        time.hours = rem;
        this.update(time.days, time.hours, time.minutes, time.seconds);
        this.value(this.toString());
        this._listening = true;
    };
    TimeSpan.prototype.loadFromString = function (value) {
        /// <signature>
        /// <param name="value" type="String" optional="true" />
        /// </signature>
        var slots = value ? value.split(":", 4) : [];
        this.update(slots[0], slots[1], slots[2], slots[3]);
    };
    TimeSpan.prototype.update = function (days, hours, minutes, seconds) {
        /// <signature>
        /// <param name="days" type="Number" />
        /// <param name="hours" type="Number" />
        /// <param name="minutes" type="Number" />
        /// <param name="seconds" type="Number" />
        /// </signature>
        this.days(asNumber(days));
        this.hours(asNumber(hours));
        this.minutes(asNumber(minutes));
        this.seconds(asNumber(seconds));
    };
    TimeSpan.prototype.toObject = function () {
        /// <signature>
        /// <returns type="Object" />
        /// </signature>
        return {
            days: asNumber(this.days()),
            hours: asNumber(this.hours()),
            minutes: asNumber(this.minutes()),
            seconds: asNumber(this.seconds())
        };
    };
    TimeSpan.prototype.toString = function () {
        /// <signature>
        /// <summary>Converts the current date to string.</summary>
        /// <returns type="String" />
        /// </signature>
        var time = this.toObject();
        var timeStr = "";
        timeStr += time.days;
        timeStr += ":";
        timeStr += time.hours;
        timeStr += ":";
        timeStr += time.minutes;
        timeStr += ":";
        timeStr += time.seconds;
        return timeStr;
    };
    TimeSpan.prototype.dispose = function () {
        /// <signature>
        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        /// </signature>
        if (this._disposed) {
            return;
        }
        this._daysSn && this._daysSn.dispose();
        this._daysSn = null;
        this._hoursSn && this._hoursSn.dispose();
        this._hoursSn = null;
        this._minutesSn && this._minutesSn.dispose();
        this._minutesSn = null;
        this._secondsSn && this._secondsSn.dispose();
        this._secondsSn = null;
        this._disposed = true;
    };

    ko.components.register("ui-timespan", {
        viewModel: TimeSpan,
        template:
            '<div class="ui-timespan">' +
                '<label data-bind="text: daysText"></label>' +
                '<input type="number" min="0" data-bind="value: days" />' +
                '<label data-bind="text: hoursText"></label>' +
                '<input type="number" min="0" max="23" data-bind="value: hours" />' +
                '<label data-bind="text: minutesText"></label>' +
                '<input type="number" min="0" max="59" data-bind="value: minutes" />' +
                '<label data-bind="text: secondsText"></label>' +
                '<input type="number" min="0" max="59" data-bind="value: seconds" />' +
            '</div>'
    });

})(Globalize, ko);