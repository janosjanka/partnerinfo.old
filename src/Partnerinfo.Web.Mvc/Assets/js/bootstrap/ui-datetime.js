// DateTime Picker plugin v1.0
// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (globalize, ko, undefined) {
    "use strict";

    //console.log(new Date(2015, 12, 5, 0, 0, 0).getTimezoneOffset());

    var _observable = ko.observable;
    var _observableArray = ko.observableArray;

    function localizeMonths(culture, format) {
        var months;
        if (globalize && format !== "short") {
            var culture = globalize.culture(culture);
            if (culture) {
                months = culture.calendar.months[format === "long" ? "namesAbbr" : "names"];
            }
        }
        return months || [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];
    }

    function isDate(obj) {
        return Object.prototype.toString.call(obj) === "[object Date]";
    }

    function DateTime(options) {
        /// <signature>
        /// <summary>Represents a point in time, typically expressed as a date and time of day, relative to Coordinated Universal Time (UTC).</summary>
        /// <param name="options" type="Object" optional="true">A set of key/value pairs that can be used to configure date settings.</param>
        /// <returns type="DateTime" />
        /// </signature>
        this._disposed = false;

        options = options || {};
        this.culture = options.culture;
        this.date = undefined;
        this.mode = options.mode || "date";
        this.format = options.format || "longlong";
        this.onchanged = options.onchanged;        

        this.year = _observable();
        this.month = _observable();
        this.day = _observable();
        this.hours = _observable();
        this.minutes = _observable();
        this.seconds = _observable();
        this.months = _observableArray();
        this.days = _observableArray();

        this.setFormat(this.format);

        this._yearSn = this.year.subscribe(this._dateChanged.bind(this, "year"));
        this._monthSn = this.month.subscribe(this._dateChanged.bind(this, "month"));
        this._daySn = this.day.subscribe(this._dateChanged.bind(this, "day"));
        this._hoursSn = this.hours.subscribe(this._dateChanged.bind(this, "hours"));
        this._minutesSn = this.minutes.subscribe(this._dateChanged.bind(this, "minutes"));
        this._secondsSn = this.seconds.subscribe(this._dateChanged.bind(this, "seconds"));

        this.setDate(options.date);
    }
    DateTime.prototype.toDate = function () {
        /// <signature>
        /// <returns type="Object" />
        /// </signature>
        var year = this.year();
        var month = this.months.indexOf(this.month());
        var day = this.day();
        var hours = this.hours();
        var minutes = this.minutes();
        var seconds = this.seconds();
        if (month < 0) {
            month = undefined;
        }
        if (this.mode.substr(0, 4) === "date" && (year === undefined || month === undefined || day === undefined)) {
            return;
        }
        return new Date(year || 1, month || 0, day || 1, hours || 0, minutes || 0, seconds || 0, 0);
    };
    DateTime.prototype.toString = function () {
        /// <signature>
        /// <returns type="String" />
        /// </signature>
        var date = this.toDate();
        if (date) {
            return date.toString();
        }
    };
    DateTime.prototype.toISOString = function () {
        /// <signature>
        /// <returns type="String" />
        /// </signature>
        var date = this.toDate();
        if (date) {
            return date.toISOString();
        }
    };
    DateTime.prototype.setFormat = function (format) {
        /// <signature>
        /// <param name="format" type="String" />
        /// </signature>
        this.format = format;
        this.months(localizeMonths(this.culture, format));
    };
    DateTime.prototype.setDate = function (value) {
        /// <signature>
        /// <param name="value" type="String" />
        /// </signature>
        /// <signature>
        /// <param name="value" type="Date" />
        /// </signature>
        var date = value ? isDate(value) ? value : new Date(value) : undefined;
        this.date = date;
        this._updating = true;
        if (date) {
            this.year(date.getFullYear());
            this.month(this.months()[date.getMonth()]);
            this.day(date.getDate());
            this.hours(date.getHours());
            this.minutes(date.getMinutes());
            this.seconds(date.getSeconds());
        } else {
            this.year(undefined);
            this.month(undefined);
            this.day(undefined);
            this.hours(undefined);
            this.minutes(undefined);
            this.seconds(undefined);
        }
        this._updating = false;
    };
    DateTime.prototype._updateDays = function () {
        /// <signature>
        /// <summary>Updates the date</summary>
        /// </signature>
        var year = this.year();
        var month = this.month();
        if (year && month) {
            var daysInMonth = new Date(year, this.months.indexOf(month) + 1, 0).getDate();
            var days = new Array(daysInMonth);
            for (var i = 0; i < daysInMonth; ++i) {
                days[i] = i + 1;
            }
            this.days(days);
        } else {
            this.days.removeAll();
        }
    };
    DateTime.prototype._dateChanged = function (timePart) {
        /// <signature>
        /// <param name="timePart" type="String" />
        /// </signature>
        if (timePart === "year" || timePart === "month") {
            this._updateDays();
        }
        if (!this._updating) {
            var newDate = this.toDate();
            if (Number(this.date) !== Number(newDate)) {
                this.date = newDate;
                this.onchanged && this.onchanged.call(this);
            }
        }
    };
    DateTime.prototype.dispose = function () {
        /// <signature>
        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        /// </signature>
        if (this._disposed) {
            return;
        }
        this._yearSn && this._yearSn.dispose();
        this._yearSn = null;
        this._monthSn && this._monthSn.dispose();
        this._monthSn = null;
        this._daySn && this._daySn.dispose();
        this._daySn = null;
        this._hoursSn && this._hoursSn.dispose();
        this._hoursSn = null;
        this._minutesSn && this._minutesSn.dispose();
        this._minutesSn = null;
        this._secondsSn && this._secondsSn.dispose();
        this._secondsSn = null;
        this._disposed = true;
    };

    function DateTimeViewModel(params) {
        /// <signature>
        /// <summary>Initializes a new instance of the DateTimeViewModel class.</summary>
        /// <returns type="DateTimeViewModel" />
        /// </signature>
        this._disposed = false;
        var value = ko.unwrap(params.value);
        this.value = params.value;
        this.mode = params.mode || "datetime";
        this.text = params.text || {};
        this.outputFormat = params.outputFormat || (isDate(value) ? "date" : "string");
        this.date = new DateTime({
            date: value,
            mode: this.mode,
            format: params.format,
            onchanged: this._dateChanged.bind(this)
        });
        this._valueSn = this.value.subscribe(this._valueChanged, this);
    }
    DateTimeViewModel.prototype._dateChanged = function () {
        /// <signature>
        /// <summary>Fires when the date value changes</summary>
        /// </signature>
        if (ko.isWriteableObservable(this.value)) {
            if (this.outputFormat === "date") {
                this.value(this.date.toDate());
            } else {
                this.value(this.date.toISOString());
            }
        }
    };
    DateTimeViewModel.prototype._valueChanged = function (value) {
        /// <signature>
        /// <summary>Fires when the source value changes</summary>
        /// </signature>
        this.date.setDate(value);
    };
    DateTimeViewModel.prototype.dispose = function () {
        /// <signature>
        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        /// </signature>
        if (this._disposed) {
            return;
        }
        this._valueSn && this._valueSn.dispose();
        this._valueSn = null;
        this.date && this.date.dispose();
        this.date = undefined;
        this._disposed = true;
    };

    ko.components.register("ui-datetime", {
        viewModel: DateTimeViewModel,
        template:
            '<div class="ui-datetime">' +
                '<!-- ko if: mode !== "time" -->' +
                '<div class="ui-datetime-date">' +
                    '<input class="ui-datetime-year" type="number" min="0" data-bind="value: date.year, attr: { placeholder: text.year }" />' +
                    '<select class="ui-datetime-month" data-bind="options: date.months, value: date.month, optionsCaption: text.month, css: { \'ui-input-default\': !date.month() }" />' +
                    '<select class="ui-datetime-day" data-bind="options: date.days, value: date.day, optionsCaption: text.day, css: { \'ui-input-default\': !date.day() }" />' +
                '</div>' +
                '<!-- /ko -->' +
                '<!-- ko if: mode !== "date" -->' +
                '<div class="ui-datetime-time">' +
                    '<input class="ui-datetime-hours" type="number" min="0" max="24" data-bind="value: date.hours, attr: { placeholder: text.hour }" />' +
                    '<span class="ui-datetime-separator">:</span>' +
                    '<input class="ui-datetime-minutes" type="number" min="0" max="60" data-bind="value: date.minutes, attr: { placeholder: text.minute }" />' +
                '</div>' +
                '<!-- /ko -->' +
            '</div>'
    });

})(Globalize, ko);