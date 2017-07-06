// Copyright (c) Partnerinfo Ltd. All Rights Reserved.
/// <reference path="../engine.js" />

(function (ko, _WinJS, _KO, _PI, _Project, undefined) {
    "use strict";

    var _Class = _WinJS.Class;
    var _observable = ko.observable;

    var ns = _WinJS.Namespace.defineWithParent(_Project, "Search", {

        DailymotionItem: _Class.derive(_Project.Search.MediaItem, function DailymotionItem_ctor(options) {
            /// <signature>
            /// <param name="options" type="Object" optional="true" />
            /// <returns type="PI.Project.Search.DailymotionItem" />
            /// </signature>
            options = options || {};
            options.service = options.service || new ns.DailymotionService(options.apiKey);
            options.type = "dailymotion";
            ns.MediaItem.call(this, options);
            this.patch(options.item);
        }),

        DailymotionFilter: _Class.derive(_Project.Search.MediaFilter, function DailymotionFilter_ctor(options) {
            /// <signature>
            /// <param name="options" type="Object" optional="true" />
            /// <returns type="PI.Project.Search.DailymotionFilter" />
            /// </signature>
            ns.MediaFilter.call(this, options);
        }),

        DailymotionList: _Class.derive(_Project.Search.MediaList, function DailymotionList_ctor(engine, options) {
            /// <signature>
            /// <param name="options" type="Object" optional="true" />
            /// <returns type="PI.Project.Search.DailymotionList" />
            /// </signature>
            options = options || {};
            options.pageIndex = options.pageIndex || 1;
            options.pageSize = options.pageSize || 48;
            options.filter = options.filter || new ns.DailymotionFilter();
            options.service = options.service || new ns.DailymotionService(options.apiKey);
            ns.MediaList.call(this, engine, options);
        }, {
            mapItem: function (item) {
                /// <signature>
                /// <param name="item" type="Object" />
                /// <returns type="Object" />
                /// </signature>
                return item instanceof ns.MediaItem
                    ? item
                    : new ns.DailymotionItem({ service: this.service, item: item });
            },
            findById: function (id, thisArg) {
                /// <signature>
                /// <summary>Finds a media item using the specified video id.</summary>
                /// <param name="id" type="String" />
                /// <param name="thisArg" type="Object" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                var that = thisArg || this;
                var deferred = $.Deferred();
                var item = new ns.DailymotionItem({ id: id });
                item.load().then(
                    function () {
                        deferred.resolveWith(that, [item]);
                    },
                    function () {
                        deferred.rejectWith(that, []);
                    });
                return deferred.promise();
            },
            refresh: function () {
                /// <signature>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                var that = this;
                var filter = this.filter.toObject();
                var maxResults = this.pageSize;
                var startIndex = ((this.pageIndex - 1) * maxResults) + 1;
                return this.service.getVideoList({
                    query: filter.query,
                    language: filter.language,
                    startIndex: startIndex,
                    maxResults: maxResults
                }).then(
                    function (data) {
                        that.removeAll();
                        that.total = data.total;
                        that.push.apply(that, data.items || []);
                    },
                    function () {
                        that.removeAll();
                        that.total = 0;
                    });
            }
        }),

        DailymotionService: _Class.define(function DailymotionService_ctor(apiKey) {
            /// <signature>
            /// <param name="apiKey" type="String" />
            /// <returns type="DailymotionService" />
            /// </signature>
            this.apiKey = apiKey;
            this.apiUrl = "https://api.dailymotion.com/";
        }, {
            getThumbnail: function (videoId) {
                /// <signature>
                /// <param name="videoId" type="String" />
                /// <returns type="String" />
                /// </signature>
                return "http://www.dailymotion.com/thumbnail/video/" + videoId;
            },
            getVideoItems: function (videoIds) {
                /// <signature>
                /// <param name="ids" type="Array" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _WinJS.Promise.error();
            },
            getVideoList: function (options) {
                /// <signature>
                /// <param name="options" type="Object" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                var that = this;
                return _WinJS.Promise(function (completeDispatch, errorDispatch) {
                    $.ajax({
                        url: that.apiUrl + "videos?callback=?",
                        type: "GET",
                        cache: true,
                        dataType: "json",
                        data: {
                            "search": options.query,
                            "language": options.language,
                            "page": 1,
                            "limit": options.maxResults,
                            "fields": "id,title,description,duration,thumbnail_medium_url,views_total,modified_time"
                            /* key": that.apiKey */
                        },
                        contentType: "application/json; charset=utf-8",
                        success: function (data) {
                            if (data.error) {
                                errorDispatch(data);
                                return;
                            }
                            completeDispatch({
                                items: data.list.map(that._mapVideoItem),
                                totalItemCount: data.total | 0
                            });
                        },
                        error: errorDispatch
                    });
                });
            },
            _mapVideoItem: function (videoItem) {
                /// <signature>
                /// <param name="videoItem" type="Object" />
                /// <returns type="Object" />
                /// </signature>
                return videoItem && {
                    id: videoItem.id,
                    channelId: videoItem.channelId,
                    title: videoItem.title,
                    description: videoItem.description,
                    duration: videoItem.duration,
                    thumbnailUrl: videoItem.thumbnail_medium_url,
                    published: videoItem.modified_time,
                    updated: videoItem.modified_time
                };
            }
        })
    });

})(window.ko, WinJS, WinJS.Knockout, PI, PI.Project);