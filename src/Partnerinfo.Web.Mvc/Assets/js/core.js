// Copyright (c) Partnerinfo TV. All Rights Reserved.

(function (_WinJS, _PI) {
    "use strict";

    var _Utilities = _WinJS.Utilities;

    var absoluteUriRx = /^(f|ht)tps?:\/\//i;
    var interpolateRx = /\{(\S*?)\}/gi;

    function interpolate(path, data) {
        /// <signature>
        /// <param name="path" type="String" />
        /// <param name="data" type="Object" />
        /// <returns type="String" />
        /// </signature>
        return path.replace(interpolateRx, function (match) {
            var k = match.substr(1, match.length - 2);
            var v = data[k];
            if (v === null || v === undefined) {
                throw new TypeError("PI.api - interpolation failed: " + path);
            }
            delete data[k];
            return window.encodeURIComponent(v);
        });
    }

    function getRouteInfo(path, data) {
        /// <signature>
        /// <param name="path" type="String" />
        /// <param name="data" type="Object" optional="true" />
        /// <returns type="String" />
        /// </signature>
        if (!path) {
            return null;
        }
        if (path.charCodeAt(0) === 126) {
            path = path.substr(1);
        } else if (!absoluteUriRx.test(path)) {
            path = "/api/" + path;
        }
        // Value and array types cannot be interpolated
        if (!data || typeof data !== "object" || Array.isArray(data)) {
            return {
                path: path,
                data: data
            };
        }
        return {
            path: interpolate(path, data = _Utilities.extend({}, data)),
            data: data
        };
    }
    
    _PI.api = function api(options, thisArg) {
        /// <signature>
        /// <summary>Performs an asynchronous HTTP request.</summary>
        /// <param name="options" type="Object">
        ///     <para>path: String - An url that identifies an API resource.</para>
        ///     <para>data?: Object - Data to be sent to the server.</para>
        ///     <para>method?: String - The HTTP data transfer method (such as GET, POST, or HEAD) used by the client.</para>
        ///     <para>context?: Object - An object to which the this keyword can refer in the callback function.</para>
        /// </param>
        /// <param name="thisArg" type="Object" optional="true">An object to which the this keyword can refer in the callback function. If thisArg is omitted, undefined is used.</param>
        /// <returns type="_WinJS.Promise" />
        /// </signature>
        if (!options) {
            throw new TypeError("options");
        }
        return _WinJS.Promise(function (completeDispatch, errorDispatch) {
            var routeInfo = getRouteInfo(options.path, options.data);
            var ajaxOptions = {
                type: options.method || "GET",
                url: routeInfo.path,
                data: routeInfo.data,
                async: options.async !== false,
                cache: options.cache !== false,
                beforeSend: function api_beforeSend(xhr) {
                    options.auth && xhr.setRequestHeader("Authorization", "Bearer " + options.auth);
                    var headers = options.headers;
                    if (headers) {
                        for (var key in headers) {
                            if (headers.hasOwnProperty(key)) {
                                xhr.setRequestHeader(key, headers[key]);
                            }
                        }
                    }
                }
            };

            if (ajaxOptions.type !== "GET"
                && ajaxOptions.data !== undefined
                && ajaxOptions.data !== null) {
                ajaxOptions.data = JSON.stringify(ajaxOptions.data);
                ajaxOptions.contentType = "application/json; charset=utf-8";
            }

            $.ajax(ajaxOptions).then(
                function (response, status, jqXHR) {
                    if (typeof response !== "string") {
                        response = response || {};
                        var anonymId = jqXHR.getResponseHeader("AID");
                        var location = jqXHR.getResponseHeader("Location");
                        anonymId && (response.anonymId = anonymId);
                        location && (response.locationLink = location);
                    }
                    completeDispatch.call(thisArg, response);
                },
                function (jqXHR) {
                    var message;
                    var error;
                    var status = jqXHR.status;
                    // HTTP 0 (Operation was canceled)
                    if (status > 0 && jqXHR.responseText) {
                        try {
                            error = JSON.parse(jqXHR.responseText);
                            if (error) {
                                if (error.members) {
                                    message = error.members.map(function (m) { return m.message; }).join('/n');
                                } else {
                                    message = error.message;
                                }
                            }
                        } catch (ex) { // Invalid JSON Data Exception
                            message = jqXHR.responseText;
                        }
                    }
                    error = error || {};
                    error.status = status;
                    error.message = message || "";
                    errorDispatch.call(thisArg, error);
                });
        }, thisArg);
    };

    _PI.api.action = function (path, routeValues) {
        /// <signature>
        /// <summary>Generates a fully qualified URL for an action method by using the specified action name, controller name, route values, protocol to use, and host name.</summary>
        /// <param name="path" type="String">An URL that can be used to identify a web resource.</param>
        /// <param name="routeValues" type="Object" optional="true">An object that contains the parameters for a route.</param>
        /// <returns type="String" />
        /// </signature>
        return getRouteInfo(path, routeValues).path;
    };

    /// <field type="Object">
    /// Represents a storage type
    /// </field>
    _PI.Storage = {
        session: 1,
        local: 2
    };

    function getCacheItem(storageKey, key) {
        /// <signature>
        /// <summary>Gets a value from the cache.</summary>
        /// <param name="storageKey" type="storage">The type of the cache storage.</param>
        /// <param name="key" type="String">The cache key.</param>
        /// <returns type="Object" />
        /// </signature>
        var sto = storageKey === _PI.Storage.session ? window.sessionStorage : window.localStorage;
        if (sto) {
            var value = sto.getItem(key);
            if (value) {
                try {
                    return JSON.parse(value);
                } catch (ex) {
                    return;
                }
            }
        }
        return null;
    }

    function setCacheItem(storageKey, key, value) {
        /// <signature>
        /// <summary>Adds or updates an item in the cache.</summary>
        /// <param name="storageKey" type="storage">The type of the cache storage.</param>
        /// <param name="key" type="String">The cache key.</param>
        /// <param name="value" type="Object">The value to update.</param>
        /// </signature>
        if (value !== undefined) {
            if (value !== null) {
                value = JSON.stringify(value);
            }
            var sto = storageKey === _PI.Storage.session ? window.sessionStorage : window.localStorage;
            if (sto) {
                if (value === null) {
                    sto.removeItem(key);
                } else {
                    sto.setItem(key, value);
                }
            }
        }
    }

    _PI.globalCache = function (storage, key, value) {
        /// <signature>
        /// <summary>Gets an object from the cache using the specified cache key.</summary>
        /// <param name="storage" type="PI.Storage">The storage.</param>
        /// <param name="key" type="String">The cache key.</param>
        /// <returns type="Object" />
        /// </signature>
        /// <signature>
        /// <summary>Sets a new cache object using the specified cache key.</summary>
        /// <param name="storage" type="PI.Storage">The storage.</param>
        /// <param name="key" type="String">The cache key.</param>
        /// <param name="value" type="Object">The value to set.</param>
        /// </signature>
        if (arguments.length <= 2) {
            return getCacheItem(storage, key);
        }
        setCacheItem(storage, key, value);
    };
    _PI.userCache = function (storage, key, value) {
        /// <signature>
        /// <summary>Gets an object from the cache using the specified cache key.</summary>
        /// <param name="storage" type="PI.Storage">The storage.</param>
        /// <param name="key" type="String">The cache key.</param>
        /// <returns type="Object" />
        /// </signature>
        /// <signature>
        /// <summary>Sets a new cache object using the specified cache key.</summary>
        /// <param name="storage" type="PI.Storage">The storage.</param>
        /// <param name="key" type="String">The cache key.</param>
        /// <param name="value" type="Object">The value to set.</param>
        /// </signature>
        var id = _PI.identity && _PI.identity.id;
        if (!id) {
            return;
        }
        var k = _PI.userCache.createKey(key);
        if (arguments.length <= 2) {
            return getCacheItem(storage, k);
        }
        setCacheItem(storage, k, value);
    };
    _PI.userCache.createKey = function (key) {
        /// <signature>
        /// <summary>Creates a user key from the given key.</summary>
        /// <param name="key" type="String" />
        /// <returns type="String" />
        /// </signature>
        if (!key) {
            return;
        }
        var id = _PI.identity && _PI.identity.id;
        if (!id) {
            return;
        }
        var k = "u";
        k += id;
        k += "/";
        k += key;
        return k;
    };

})(WinJS, PI);
