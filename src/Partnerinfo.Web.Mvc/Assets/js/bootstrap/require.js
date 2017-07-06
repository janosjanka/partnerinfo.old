// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (_Global, $) {
    "use strict";

    _Global.loaders = {};

    (function () {

        var references = {};
        var options = {
            root: "/",
            versionParam: "d", // "v" is used by ASP.NET MVC
            version: null,
            absoluteUri: /^(f|ht)tps?:\/\//i,
            baseElement: $(document.getElementsByTagName("head")[0])
        };

        function createInfo(url) {
            /// <signature>
            /// <param name="url" type="String" />
            /// <returns type="Object" />
            /// </signature>
            var absolute = options.absoluteUri.test(url);
            var extension = url.split('.').pop();
            var get = _Global.loaders[extension];
            if (!get) {
                if (!absolute) {
                    url += ".js";
                }
                get = _Global.loaders["js"];
            }
            if (options.version) {
                url += "?";
                url += options.versionParam;
                url += "=";
                url += options.version;
            }
            return {
                url: absolute ? url : options.root + url,
                get: get,
                promise: void 0
            };
        }

        _Global.require = function (names) {
            /// <signature>
            /// <summary>Loads the specified resources.</summary>
            /// <param name="names" type="Array" />
            /// <returns type="$.Deferred" />
            /// </signature>
            var nameArray = Array.isArray(names) ? names : Array.prototype.slice.call(arguments, 0);
            var promiseArray = new Array(len);
            for (var i = 0, len = nameArray.length; i < len; ++i) {
                var name = nameArray[i];
                var info = references[name];
                if (info) {
                    promiseArray[i] = info.promise;
                } else {
                    references[name] = info = createInfo(name);
                    promiseArray[i] = info.promise = info.get(info.url, options);
                }
            }
            return $.when.apply($, promiseArray);
        };

        _Global.require.config = function (config) {
            /// <signature>
            /// <summary>Configures the module loader.</summary>
            /// <param name="config" type="Object" />
            /// </signature>
            $.extend(options, config, true);
        };

    })();

    (function () {

        //
        // JavaScript Resource Loader
        //

        _Global.loaders["js"] = function (url, options) {
            /// <signature>
            /// <param name="url" type="String" />
            /// <param name="options" type="Object" />
            /// </signature>
            var promise = $.ajax({
                dataType: "script",
                url: url,
                cache: true,
                ifModified: true
            });
            _Global.DEBUG && promise.then(
                function () {
                    console.info("requirejs: %s", url);
                },
                function (j, t, e) {
                    console.warn("requirejs: %s - %s", url, e);
                });
            return promise;
        };

        //
        // CSS Style Resource Loader
        //

        _Global.loaders["css"] = function (url, options) {
            /// <signature>
            /// <param name="url" type="String" />
            /// <param name="options" type="Object" />
            /// </signature>
            var deferred = $.Deferred();
            var timeout = _Global.setTimeout(function () {
                _Global.clearTimeout(timeout);
                try {
                    var link = document.createElement("link");
                    link.rel = "stylesheet";
                    link.type = "text/css";
                    link.href = url;
                    options.baseElement.append(link);
                    deferred.resolve();
                } catch (ex) {
                    deferred.reject(ex.message);
                }
            }, 7);
            _Global.DEBUG && deferred.then(
                function () {
                    console.info("requirejs: %s", url);
                },
                function (e) {
                    console.warn("requirejs: %s - %s", url, e);
                });
            return deferred.promise();
        };

        //
        // HTML Template Resource Loader
        //

        _Global.loaders["html"] = function (url, options) {
            /// <signature>
            /// <param name="url" type="String" />
            /// <param name="options" type="Object" />
            /// </signature>
            var promise = $.ajax({
                type: "GET",
                url: url,
                contentType: "text/html",
                cache: true,
                ifModified: true
            }).then(
                function (textResponse) {
                    options.baseElement.append(textResponse);
                });
            _Global.DEBUG && promise.then(
                function () {
                    console.info("requirejs: %s", url);
                },
                function (j, t, e) {
                    console.warn("requirejs: %s - %s", url, e);
                });
            return promise;
        };

    }());

})(window, jQuery);