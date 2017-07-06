// Copyright (c) Partnerinfo Ltd. All Rights Reserved.
/// <reference path="../engine.js" />

(function (ko, _WinJS, _KO, _PI, _Project, undefined) {
    "use strict";

    var _Class = _WinJS.Class;
    var _observable = ko.observable;

    function getDuration(ytTimeCode) {
        var match = ytTimeCode.match(/PT(\d+H)?(\d+M)?(\d+S)?/);
        var hours = parseInt(match[1]) || 0;
        var minutes = parseInt(match[2]) || 0;
        var seconds = parseInt(match[3]) || 0;
        return hours * 3600 + minutes * 60 + seconds;
    }

    var ns = _WinJS.Namespace.defineWithParent(_Project, "Search", {

        YouTubeItem: _Class.derive(_Project.Search.MediaItem, function YouTubeItem_ctor(options) {
            /// <signature>
            /// <param name="options" type="Object" optional="true" />
            /// <returns type="PI.Project.Search.YouTubeItem" />
            /// </signature>
            options = options || {};
            options.service = options.service || new ns.YouTubeService(options.apiKey);
            options.type = "youtube";
            ns.MediaItem.call(this, options);
            this.patch(options.item);
        }),

        YouTubeFilter: _Class.derive(_Project.Search.MediaFilter, function YouTubeFilter_ctor(options) {
            /// <signature>
            /// <param name="options" type="Object" optional="true" />
            /// <returns type="PI.Project.Search.YouTubeFilter" />
            /// </signature>
            ns.MediaFilter.call(this, options);
        }),

        YouTubeList: _Class.derive(_Project.Search.MediaList, function YouTubeList_ctor(engine, options) {
            /// <signature>
            /// <param name="options" type="Object" optional="true" />
            /// <returns type="PI.Project.Search.YouTubeList" />
            /// </signature>
            options = options || {};
            options.pageIndex = options.pageIndex || 1;
            options.pageSize = options.pageSize || 48;
            options.filter = options.filter || new ns.YouTubeFilter();
            options.service = options.service || new ns.YouTubeService(options.apiKey);
            ns.MediaList.call(this, engine, options);
        }, {
            mapItem: function (item) {
                /// <signature>
                /// <param name="item" type="Object" />
                /// <returns type="Object" />
                /// </signature>
                return item instanceof ns.MediaItem
                    ? item
                    : new ns.YouTubeItem({ service: this.service, item: item });
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
                var item = new ns.YouTubeItem({ id: id });
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

        YouTubeService: _Class.define(function YouTubeService_ctor(apiKey) {
            /// <signature>
            /// <param name="apiKey" type="String" />
            /// <returns type="YouTubeService" />
            /// </signature>
            this.apiKey = apiKey;
            this.apiUrl = "https://www.googleapis.com/youtube/v3/";
        }, {
            getThumbnail: function (videoId) {
                /// <signature>
                /// <param name="videoId" type="String" />
                /// <returns type="String" />
                /// </signature>
                return "http://i.ytimg.com/vi/" + videoId + "/default.jpg";
            },
            getVideoItems: function (videoIds) {
                /// <signature>
                /// <param name="ids" type="Array" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                var that = this;
                return _WinJS.Promise(function (completeDispatch, errorDispatch) {
                    $.ajax({
                        url: that.apiUrl + "videos",
                        type: "GET",
                        cache: true,
                        dataType: "json",
                        data: {
                            "part": "snippet,contentDetails",
                            "fields": "items(id,snippet,contentDetails)",
                            "id": videoIds.join(','),
                            "key": that.apiKey
                        },
                        contentType: "application/json; charset=utf-8",
                        success: function (data) {
                            completeDispatch({
                                items: data.items.map(that._mapVideoItem)
                            });
                        },
                        error: errorDispatch
                    });
                });
            },
            getVideoList: function (options) {
                /// <signature>
                /// <param name="options" type="Object" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                var that = this;
                return _WinJS.Promise(function (completeDispatch, errorDispatch) {
                    $.ajax({
                        url: that.apiUrl + "search",
                        type: "GET",
                        cache: true,
                        dataType: "json",
                        data: {
                            "type": "video",
                            "part": "snippet",
                            "fields": "items(id,snippet),pageInfo",
                            "q": options.query,
                            "regionCode": options.language,
                            "maxResults": options.maxResults,
                            "key": that.apiKey
                        },
                        contentType: "application/json; charset=utf-8",
                        success: function (data) {
                            completeDispatch({
                                items: data.items.map(that._mapVideoItem),
                                totalItemCount: data.pageInfo.totalResults | 0
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
                var result = {};
                if (!videoItem) {
                    return result;
                }

                var snippet = videoItem.snippet;
                var content = videoItem.contentDetails;

                // YouTube API returns video IDs in two different ways.
                // It depends on whether you perform a list or an unique item query :S
                result.id = videoItem.id.videoId || videoItem.id;

                if (snippet) {
                    result.channelId = snippet.channelId;
                    result.title = snippet.title;
                    result.description = snippet.description;
                    result.thumbnailUrl = snippet.thumbnails && snippet.thumbnails.default.url;
                    result.published = snippet.publishedAt;
                }

                if (content) {
                    result.duration = content.duration ? getDuration(content.duration) : 0;
                }

                return result;
            }
        })
    });

})(window.ko, WinJS, WinJS.Knockout, PI, PI.Project);
