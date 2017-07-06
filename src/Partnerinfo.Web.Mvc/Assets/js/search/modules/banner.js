// Copyright (c) Partnerinfo Ltd. All Rights Reserved.
/// <reference path="../engine.js" />

(function (ko, _WinJS, PI, undefined) {

    var _Class = _WinJS.Class;
    var _Utilities = _WinJS.Utilities;

    var ns = _WinJS.Namespace.defineWithParent(PI, "Project.Search", {
        /// <field>
        /// <summary>Represents a video banner.</summary>
        /// </field>
        BannerModule: _Class.define(function BannerModule_ctor(engine, options) {
            /// <signature>
            /// <summary>Initializes a new instance of the BannerModule class.</summary>
            /// <param name="engine" type="PI.Project.Search.Engine">Search engine.</param>
            /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the BannerModule.</param>
            /// <returns type="BannerModule" />
            /// </signature>
            if (!engine) {
                throw new TypeError("engine cannot be null or undefined");
            }
            options = options || {};
            _Utilities.setOptions(this, options);

            this.engine = engine;
            this.playlist = ko.observable();
            this.medialist = ko.observable();
            this.scrollIndex = ko.observable(0).extend({ notify: "always" });
            this.scrollIndex.subscribe(this.dispatchEvent.bind(this, "beforescroll"));
        }, {
            raiseAfterScroll: function () {
                /// <signature>
                /// <summary>Raises an afterscroll event.</summary>
                /// </signature>
                if (typeof this._scrollCallback === "function") {
                    this._scrollCallback();
                }
                this.dispatchEvent("afterscroll");
            },
            loadPlaylist: function (list) {
                /// <signature>
                /// <summary>Activates the specified list.</summary>
                /// <param name="list" type="WinJS.Knockout.PagedList">The collection to activate.</param>
                /// </signature>
                list = this._cloneList(list);
                this.medialist(null);
                this.playlist(list);
            },
            loadMedialist: function (list) {
                /// <signature>
                /// <summary>Activates the specified list.</summary>
                /// <param name="list" type="WinJS.Knockout.PagedList">The collection to activate.</param>
                /// </signature>
                list = this._cloneList(list);
                this.playlist(null);
                this.medialist(list);
            },
            scrollIntoView: function (item, callback) {
                /// <signature>
                /// <summary>Causes the object to scroll into view.</summary>
                /// <param name="item" type="Object">Object to scroll.</param>
                /// </signature> 
                var index = -1;
                var list = this.medialist() || this.playlist();
                if (list) {
                    index = list.indexOf(item);
                    if (index > -1) {
                        this._scrollCallback = callback;
                        this.scrollIndex(index);
                    }
                }
                return index;
            },
            calculateViewCoords: function () {
                /// <signature>
                /// <summary>Calculates the coordinates of the current view.</summary>
                /// <returns type="Object" value="{ x: 0, y: 0 }" />
                /// </signature>
                var index = this.scrollIndex() || 0;
                var left = (index - Math.floor(index / 8) * 8) * 121 + 60;
                return {
                    x: left,
                    y: -375
                };
            },
            _cloneList: function (list) {
                /// <signature>
                /// <summary>Makes a cloned version of the specified list.</summary>
                /// <param name="list" type="WinJS.Knockout.PagedList">The collection to clone.</param>
                /// <returns type="WinJS.Knockout.PagedList" />
                /// </signature>
                var options = ko.utils.extend({}, list.options);
                options.autoOpen = false;
                options.items = list.items();
                options.filter = list.filter && new list.filter.constructor(list.filter.toObject());
                var clonedList = new list.constructor(list.engine, options);
                clonedList.total = list.total;
                return clonedList;
            }
        })
    });

    //
    // Defines a class using the given constructor and the union of the set of instance members specified by all the mixin objects.
    //

    _Class.mix(ns.BannerModule, _Utilities.createEventProperties("beforescroll", "afterscroll"));
    _Class.mix(ns.BannerModule, _Utilities.eventMixin);

})(window.ko, window.WinJS, window.PI);
