// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (_Global, _WinJS) {
    "use strict";

    var separator = "!/?";
    var absoluteUriRx = /^(f|ht)tps?:\/\//i;
    var placeholderRx = /\{(\w+)?(...)?\}/gi;

    var _pixelsRE = /^-?\d+(px)?$/i;
    var _numberRE = /^-?\d+/i;
    var textProp = "textContent" in document.createElement("div") ? "textContent" : "innerText";

    function async(callback, thisArg, delay, debounce) {
        /// <signature>
        /// <summary>Executes a code snippet or a function after specified delay.</summary>
        /// <param name="callback" type="Function">A callback you want to execute after delay milliseconds.</param>
        /// <param name="thisArg" type="Object" optional="true">Execution context.</param>
        /// <param name="delay" type="Number" optional="true">The number of milliseconds (thousandths of a second) that the function call should be delayed by. </param>
        /// <param name="debounce" type="Boolean" optional="true">The callback will execute only once, coalescing multiple sequential calls into a single execution.</param>
        /// </signature>
        if (debounce) {
            return debounce(callback, delay)();
        }
        _Global.setTimeout(function () {
            callback.apply(thisArg, []);
        }, delay | 0);
    }

    function extend(target, source) {
        /// <signature>
        /// <summary>Extends the target object with the members of the source object.</summary>
        /// <param name="target" type="Object">The target object to extend.</param>
        /// <param name="source" type="Object" optional="true">The source object containing the added members.</param>
        /// <returns type="Object" />
        /// </signature>
        if (source) {
            for (var p in source) {
                if (source.hasOwnProperty(p)) {
                    target[p] = source[p];
                }
            }
        }
        return target;
    }

    function debounce(callback, delay, immediate) {
        /// <signature>
        /// <summary>Returns a new function that will execute only once, coalescing multiple sequential calls into a single execution.</summary>
        /// <param name="callback" type="Function">A callback you want to execute after delay milliseconds.</param>
        /// <param name="delay" type="Number" optional="true">The number of milliseconds (thousandths of a second) that the function call should be delayed by. </param>
        /// <param name="immediate" type="Boolean" optional="true">If true, trigger the function on the leading edge, instead of the trailing.</param>
        /// <returns type="Function" />
        /// </signature>
        var timeout;
        return function () {
            var thisArg = this;
            var args = arguments;
            var later = function () {
                timeout = null;
                if (!immediate) {
                    callback.apply(thisArg, args);
                }
            };
            var callNow = immediate && !timeout;
            _Global.clearTimeout(timeout);
            timeout = _Global.setTimeout(later, delay);
            if (callNow) {
                callback.apply(thisArg, args);
            }
        };
    }

    function actionLink(path, route, protocol, hostName) {
        /// <signature>
        /// <summary>Generates a fully qualified URL for an action method by using the specified action name, controller name, route values, protocol to use, and host name.</summary>
        /// <param name="path" type="String">An URL that can be used to identify a web resource.</param>
        /// <param name="route" type="Object" optional="true">An object that contains the parameters for a route.</param>
        /// <param name="protocol" type="String" optional="true">The protocol for the URL, such as "http" or "https".</param>
        /// <param name="hostName" type="String" optional="true">The host name for the URL.</param>
        /// <returns type="String" />
        /// </signature>
        if (!path.match(absoluteUriRx) && path[0] !== '/') {
            path = '/' + path;
        }
        if (arguments.length > 2) {
            protocol = protocol ? protocol + ":" : _Global.location.protocol;
            hostName = hostName || _Global.location.host;
            path = protocol + "//" + hostName + path;
        }
        route = extend({}, route);
        path = path.replace(placeholderRx, function (match, name, attr) {
            var value;
            if (name === undefined || (value = route[name]) === undefined) {
                throw new TypeError("invalid route data: " + match);
            }
            delete route[name];
            if (attr === "...") {
                return value;
            }
            return _Global.encodeURIComponent(value);
        });
        var query = serializeParams(route);
        if (query) {
            path += '?';
            path += query;
        }
        return path;
    }

    function openLink(path, route, protocol, hostName, target, features, replace) {
        /// <signature>
        /// <summary>Opens a new window and loads the document specified by a given URL.</summary>
        /// <param name="path" type="String">An URL that can be used to identify a web resource.</param>
        /// <param name="route" type="Object" optional="true">An object that contains the parameters for a route.</param>
        /// <param name="protocol" type="String" optional="true">The protocol for the URL, such as "http" or "https".</param>
        /// <param name="hostName" type="String" optional="true">The host name for the URL.</param>
        /// <param name="target" type="String" optional="true">Sets the window or frame at which to target content.</param>
        /// <param name="features" type="String" optional="true">A string that contains a list of items separated by commas. Each item consists of an option and a value, separated by an equals sign (for example, "fullscreen=yes, toolbar=yes").</param>
        /// <param name="replace" type="Boolean" optional="true">A flag that specifies whether the url creates a new entry or replaces the current entry in the window's history list. This parameter only takes effect if the url is loaded into the same window.</param>
        /// <returns type="Window" />
        /// </signature>
        return _Global.open(actionLink(path, route, protocol, hostName),
            target || "_self", features || "", replace || false);
    }

    function currentLink(includePath) {
        /// <signature>
        /// <param name="includePath" type="Boolean" />
        /// <returns type="String" />
        /// </signature>
        return _Global.location.protocol + "//" + _Global.location.host + (includePath ? _Global.location.pathname : "");
    }

    function currentLinkHash(options, merge) {
        /// <signature>
        /// <summary>Gets URL hash parameters.</summary>
        /// <returns type="Object" />
        /// </signature>
        /// <signature>
        /// <summary>Sets or removes URL hash parameters.</summary>
        /// <param name="options" type="Object">A set of key/value pairs.</param>
        /// </signature>
        /// <signature>
        /// <summary>Sets or merges or removes URL hash parameters.</summary>
        /// <param name="options" type="Object">A set of key/value pairs.</param>
        /// <param name="merge" type="Boolean">Merges all URL hash parameters.</param>
        /// </signature>
        if (options === undefined) {
            return parseLink(_Global.location.href.replace("#" + separator, "#")).fparam();
        }
        if (merge) {
            options = extend(currentLinkHash() || {}, options);
        }
        var hash = options && $.param(options);
        _Global.location.hash = hash ? separator + hash : "";
    }

    function parseLink(url, strictMode) {
        /// <signature>
        /// <summary>Parses the current URL.</summary>
        /// <returns type="Object" />
        /// </signature>
        /// <signature>
        /// <summary>Parses the specified URL.</summary>
        /// <param name="url" type="String">The URL to parse.</param>
        /// <returns type="Object" />
        /// </signature>
        /// <signature>
        /// <summary>Parses the specified URL.</summary>
        /// <param name="url" type="String">The URL to parse.</param>
        /// <param name="strictMode" type="Boolean">
        /// true: Less intuitive, more accurate to the specs.
        /// false: More intuitive, fails on relative paths and deviates from specs.
        /// </param>
        /// <returns type="Object" />
        /// </signature>
        if (arguments.length === 1 && url === true) {
            strictMode = true;
            url = undefined;
        }
        strictMode = !!strictMode;
        url = url || _Global.location.toString();
        var tag2attr = {
            a: 'href',
            img: 'src',
            form: 'action',
            base: 'href',
            script: 'src',
            iframe: 'src',
            link: 'href'
        },
        key = ['source', 'protocol', 'authority', 'userInfo', 'user', 'password', 'host', 'port', 'relative', 'path', 'directory', 'file', 'query', 'fragment'],
        aliases = { 'anchor': 'fragment' },
        parser = {
            strict: /^(?:([^:\/?#]+):)?(?:\/\/((?:(([^:@]*):?([^:@]*))?@)?([^:\/?#]*)(?::(\d*))?))?((((?:[^?#\/]*\/)*)([^?#]*))(?:\?([^#]*))?(?:#(.*))?)/, // Less intuitive, more accurate to the specs.
            loose: /^(?:(?![^:@]+:[^:@\/]*@)([^:\/?#.]+):)?(?:\/\/)?((?:(([^:@]*):?([^:@]*))?@)?([^:\/?#]*)(?::(\d*))?)(((\/(?:[^?#](?![^?#\/]*\.[^?#\/.]+(?:[?#]|$)))*\/?)?([^?#\/]*))(?:\?([^#]*))?(?:#(.*))?)/ // More intuitive, fails on relative paths and deviates from specs.
        },
        querystring_parser = /(?:^|&|;)([^&=;]*)=?([^&;]*)/g,
        fragment_parser = /(?:^|&|;)([^&=;]*)=?([^&;]*)/g;
        function parseUri(url, strictMode) {
            var str = decodeURI(url);
            var res = parser[strictMode || false ? 'strict' : 'loose'].exec(str);
            var uri = { attr: {}, param: {}, seg: {} };
            var i = 14;
            while (i--) {
                uri.attr[key[i]] = res[i] || '';
            }
            uri.param.query = {};
            uri.param.fragment = {};
            uri.attr.query.replace(querystring_parser, function ($0, $1, $2) { if ($1) { uri.param.query[$1] = $2; } });
            uri.attr.fragment.replace(fragment_parser, function ($0, $1, $2) { if ($1) { uri.param.fragment[$1] = $2; } });
            uri.seg.path = uri.attr.path.replace(/^\/+|\/+$/g, '').split('/');
            uri.seg.fragment = uri.attr.fragment.replace(/^\/+|\/+$/g, '').split('/');
            uri.attr.base = uri.attr.host ? uri.attr.protocol + "://" + uri.attr.host + (uri.attr.port ? ":" + uri.attr.port : '') : '';
            return uri;
        }
        function getAttrName(elm) {
            var tn = elm.tagName;
            if (tn !== undefined) {
                return tag2attr[tn.toLowerCase()];
            }
            return tn;
        }
        return {
            data: parseUri(url, strictMode),
            attr: function (attr) {
                attr = aliases[attr] || attr;
                return attr !== undefined ? this.data.attr[attr] : this.data.attr;
            },
            param: function (param) {
                return param !== undefined ? this.data.param.query[param] : this.data.param.query;
            },
            fparam: function (param) {
                return param !== undefined ? this.data.param.fragment[param] : this.data.param.fragment;
            },
            segment: function (seg) {
                if (seg === undefined) {
                    return this.data.seg.path;
                } else {
                    seg = seg < 0 ? this.data.seg.path.length + seg : seg - 1;
                    return this.data.seg.path[seg];
                }
            },
            fsegment: function (seg) {
                if (seg === undefined) {
                    return this.data.seg.fragment;
                } else {
                    seg = seg < 0 ? this.data.seg.fragment.length + seg : seg - 1;
                    return this.data.seg.fragment[seg];
                }
            }
        };
    }

    function parseMediaLink(url, params) {
        /// <signature>
        /// <summary>Parses a video URL.</summary>
        /// <param name="url" type="String">The url.</param>
        /// <returns type="Object" />
        /// </signature>
        /// <signature>
        /// <summary>Parses a video URL.</summary>
        /// <param name="url" type="String">The url.</param>
        /// <param name="params" type="Object">Parameters.</param>
        /// <returns type="Object" />
        /// </signature>
        var parsedUrl = parseLink(url);
        var path = parsedUrl.data.seg.path;
        var host = parsedUrl.attr("host");
        var videoType, videoUrl, videoId, thumbnailUrls;
        params = extend({ wmode: "opaque" }, params);
        if (host.indexOf("youtube") >= 0 || host.indexOf("youtu.be") >= 0) {
            // http://www.youtube.com/watch?v=bx9VtSoLy3M
            // http://www.youtube.com/v/bx9VtSoLy3M
            videoType = "youtube";
            videoId = parsedUrl.param("v") || path[path.length - 1];
            videoUrl = "http://www.youtube.com/embed/" + videoId;
            thumbnailUrls = {
                sq: "http://img.youtube.com/vi/" + videoId + "/default.jpg",
                mq: "http://img.youtube.com/vi/" + videoId + "/mqdefault.jpg",
                hq: "http://img.youtube.com/vi/" + videoId + "/hqdefault.jpg"
            };
        }
        if (host.indexOf("dailymotion") >= 0) {
            // http://www.dailymotion.com/video/xse57d_gvs-of-shauna-mullin-and-zara-dampney_sport
            videoType = "dailymotion";
            videoId = path[path.length - 1],
            videoUrl = "http://www.dailymotion.com/embed/video/" + videoId;
            thumbnailUrls = {
                sq: "http://img.youtube.com/vi/" + videoId + "/default.jpg",
                mq: "http://img.youtube.com/vi/" + videoId + "/mqdefault.jpg",
                hq: "http://img.youtube.com/vi/" + videoId + "/hqdefault.jpg"
            };
        }
        if (!videoType) {
            videoType = null;
            videoId = null;
            videoUrl = url;
        }
        if (videoUrl && params) {
            videoUrl += "?" + serializeParams(params);
        }
        return {
            type: videoType,
            id: videoId,
            url: videoUrl,
            thumbnailUrls: thumbnailUrls
        };
    }

    function serializeParams(params, traditional) {
        /// <signature>
        /// <summary>Creates a serialized representation of an array or object, suitable for use in a URL query string or Ajax request.</summary>
        /// <param name="params" type="Object">An array or object to serialize.</param>
        /// <param name="traditional" type="Boolean" optional="true">A Boolean indicating whether to perform a traditional "shallow" serialization.</param>
        /// <returns type="String" />
        /// </signature>
        var newParams = {};
        for (var p in params) {
            var value = params[p];
            if (value === null) {
                newParams[p] = "";
            } else if (value !== undefined) {
                newParams[p] = value;
            }
        }
        return $.param(newParams, traditional);
    }

    function cookie(key, value, options) {
        /// <signature>
        /// <summary>Gets the value of an existing cookie.</summary>
        /// <param name="key" type="String">The key of the cookie.</param>
        /// <returns type="String" />
        /// </signature>
        /// <signature>
        /// <summary>Creates a new session cookie or deletes a cookie by passing null as value. Keep in mind that you have to use the same path and domain used when the cookie was set.</summary>
        /// <param name="key" type="String">The key of the cookie.</param>
        /// <param name="value" type="String">The value of the cookie.</param>
        /// <returns type="String" />
        /// </signature>
        /// <signature>
        /// <summary>Creates a new session or expiring cookie or deletes a cookie by passing null as value. Keep in mind that you have to use the same path and domain used when the cookie was set.</summary>
        /// <param name="key" type="String">The key of the cookie.</param>
        /// <param name="value" type="String">The value of the cookie.</param>
        /// <param name="options" type="Object">An object literal containing key/value pairs to provide optional cookie attributes.
        /// <para>raw?: Boolean - If true, the value will not be encoded by the encodeURIComponent function.</para>
        /// <para>expires?: Number - Either an integer specifying the expiration date from now on in days or a Date object.
        /// If a negative value is specified (e.g. a date in the past), the cookie will be deleted.
        /// If set to null or omitted, the cookie will be a session cookie and will not be retained when the the browser exits.</para>
        /// <para>path?: String - The value of the path atribute of the cookie (default: path of page that created the cookie).</para>
        /// <para>domain?: String - The value of the domain attribute of the cookie (default: domain of page that created the cookie).</para>
        /// <para>secure?: Boolean - If true, the secure attribute of the cookie will be set and the cookie transmission will require a secure protocol (like HTTPS).</para>
        /// </param>
        /// <returns type="String" />
        /// </signature>
        if (arguments.length > 1 && (value === null || typeof value !== "object")) {
            options = extend({}, options);
            if (value === null) {
                options.expires = -1;
            }
            if (typeof options.expires === "number") {
                var d = options.expires;
                var t = options.expires = new Date();
                t.setDate(t.getDate() + d);
            }
            return _Global.document.cookie = [
                _Global.encodeURIComponent(key), "=",
                options.raw ? String(value) : _Global.encodeURIComponent(String(value)),
                options.expires ? "; expires=" + options.expires.toUTCString() : "", // use expires attribute, max-age is not supported by IE
                options.path ? "; path=" + options.path : "",
                options.domain ? "; domain=" + options.domain : "",
                options.secure ? "; secure" : ""
            ].join("");
        }
        options = value || {};
        var result;
        var decode = options.raw ? function (s) { return s; } : _Global.decodeURIComponent;
        return (result = new RegExp("(?:^|; )" + _Global.encodeURIComponent(key) + "=([^;]*)").exec(_Global.document.cookie)) ? decode(result[1]) : null;
    }

    function setOptions(target, options) {
        /// <signature>
        /// <summary>Adds the set of declaratively specified options (properties and events) to the specified object.
        /// If name of the options property begins with "on", the property value is a function and the object supports addEventListener.</summary>
        /// <param name="target" type="Object">The object on which the properties and events are to be applied.</param>
        /// <param name="options" type="Object">The set of options that are specified declaratively.</param>
        /// </signature>
        var keys = Object.keys(options);
        for (var i = 0, len = keys.length; i < len; ++i) {
            var key = keys[i];
            var value = options[key];
            if (key.length > 2) {
                var ch1 = key[0];
                var ch2 = key[1];
                if ((ch1 === 'o' || ch1 === 'O') && (ch2 === 'n' || ch2 === 'N')) {
                    if (typeof value === "function") {
                        if (target.addEventListener) {
                            target.addEventListener(key.substr(2), value);
                            continue;
                        }
                    }
                }
            }
        }
    }

    function removeEmpties(arr) {
        var len = arr.length;
        for (var i = len - 1; i >= 0; i--) {
            if (!arr[i]) {
                arr.splice(i, 1);
                len -= 1;
            }
        }
        return len;
    }

    function getClassName(element) {
        var name = element.className || "";
        if (typeof name === "string") {
            return name;
        }
        return name.baseVal || "";
    }

    function setClassName(element, value) {
        var name = element.className || "";
        if (typeof name === "string") {
            element.className = value;
        } else {
            element.className.baseVal = value;
        }
        return element;
    }

    function getDimension(element, property) {
        return Utilities.convertToPixels(element, Utilities.currentStyle(element, property));
    }

    function currentStyle(element, name) {
        /// <signature>
        /// <summary>Gets the current style value of the specified element.</summary>
        /// <param name="element" type="HTMLElement">The element.</param>
        /// <param name="name" type="String">The name of the style property.</param>
        /// </signature>
        if (!_Global.getComputedStyle) {
            return element.currentStyle[name];
        }
        return _Global.getComputedStyle(element, null)[name];
    }

    function hasClass(element, name) {
        /// <signature>
        /// <summary>Determines whether the specified element has the specified class.</summary>
        /// <param name="element" type="HTMLElement">The element.</param>
        /// <param name="name" type="String">The name of the class.</param>
        /// <returns type="Boolean">true if the specified element contains the specified class; otherwise, false.</returns>
        /// </signature>
        if (element.classList) {
            return element.classList.contains(name);
        }
        var className = getClassName(element);
        var names = className.trim().split(" ");
        for (var i = 0, len = names.length; i < len; ++i) {
            if (names[i] === name) {
                return true;
            }
        }
        return false;
    }

    var ns = _WinJS.Namespace.defineWithParent(_WinJS, "Utilities", {
        separator: separator,
        extend: extend,
        async: async,
        debounce: debounce,
        actionLink: actionLink,
        openLink: openLink,
        currentLink: currentLink,
        currentLinkHash: currentLinkHash,
        parseLink: parseLink,
        parseMediaLink: parseMediaLink,
        serializeParams: serializeParams,
        cookie: cookie,
        setOptions: setOptions,
        currentStyle: currentStyle,
        hasClass: hasClass,
        addClass: function (element, name) {
            /// <signature>
            /// <summary>Adds the specified class(es) to the specified element. Multiple classes can be added using space delimited names.</summary>
            /// <param name="element" type="HTMLElement">The element to which to add the class.</param>
            /// <param name="name" type="String">The name of the class to add, multiple classes can be added using space delimited names</param>
            /// <returns type="HTMLElement">The element.</returns>
            /// </signature>
            var i = 0;
            if (element.classList) {
                if (name.indexOf(" ") < 0) {
                    element.classList.add(name);
                } else {
                    var namesToAdd = name.split(" ");
                    removeEmpties(namesToAdd);
                    for (var len = namesToAdd.length; i < len; ++i) {
                        element.classList.add(namesToAdd[i]);
                    }
                }
                return element;
            } else {
                var className = getClassName(element);
                var names = className.split(" ");
                var l = removeEmpties(names);
                var toAdd;
                if (name.indexOf(" ") >= 0) {
                    var namesToAdd = name.split(" ");
                    removeEmpties(namesToAdd);
                    for (; i < l; ++i) {
                        var found = namesToAdd.indexOf(names[i]);
                        if (found >= 0) {
                            namesToAdd.splice(found, 1);
                        }
                    }
                    if (namesToAdd.length > 0) {
                        toAdd = namesToAdd.join(" ");
                    }
                } else {
                    var saw = false;
                    for (; i < l; ++i) {
                        if (names[i] === name) {
                            saw = true;
                            break;
                        }
                    }
                    if (!saw) { toAdd = name; }
                }
                if (toAdd) {
                    if (l > 0 && names[0].length > 0) {
                        setClassName(element, className + " " + toAdd);
                    } else {
                        setClassName(element, toAdd);
                    }
                }
                return element;
            }
        },
        removeClass: function (element, name) {
            /// <signature>
            /// <summary>Removes the specified class from the specified element.</summary>
            /// <param name="element" type="HTMLElement">The element from which to remove the class.</param>
            /// <param name="name" type="String">The name of the class to remove.</param>
            /// <returns type="HTMLElement">The element.</returns>
            /// </signature>
            if (element.classList) {
                if (element.classList.length === 0) {
                    return element;
                }
                var namesToRemove = name.split(" ");
                removeEmpties(namesToRemove);
                for (var i = 0, len = namesToRemove.length; i < len; ++i) {
                    element.classList.remove(namesToRemove[i]);
                }
                return element;
            } else {
                var original = getClassName(element);
                var namesToRemove;
                var namesToRemoveLen;
                if (name.indexOf(" ") >= 0) {
                    namesToRemove = name.split(" ");
                    namesToRemoveLen = removeEmpties(namesToRemove);
                } else {
                    if (original.indexOf(name) < 0) { return element; }
                    namesToRemove = [name];
                    namesToRemoveLen = 1;
                }
                var removed;
                var names = original.split(" ");
                var namesLen = removeEmpties(names);
                for (var i = namesLen - 1; i >= 0; --i) {
                    if (namesToRemove.indexOf(names[i]) >= 0) {
                        names.splice(i, 1);
                        removed = true;
                    }
                }
                if (removed) {
                    setClassName(element, names.join(" "));
                }
                return element;
            }
        },
        toggleClass: function (element, name) {
            /// <signature>
            /// <summary>oggles (adds or removes) the specified class on the specified element.
            /// If the class is present, it is removed; if it is absent, it is added.</summary>
            /// <param name="element" type="HTMLElement">The element on which to toggle the class.</param>
            /// <param name="name" type="String">The name of the class to toggle.</param>
            /// <returns type="HTMLElement">The element.</returns>
            /// </signature>
            if (element.classList) {
                element.classList.toggle(name);
                return element;
            } else {
                var className = getClassName(element);
                var names = className.trim().split(" ");
                var l = names.length;
                var found = false;
                for (var i = 0; i < l; ++i) {
                    if (names[i] === name) {
                        found = true;
                    }
                }
                if (!found) {
                    if (l > 0 && names[0].length > 0) {
                        setClassName(element, className + " " + name);
                    } else {
                        setClassName(element, className + name);
                    }
                } else {
                    setClassName(element, names.reduce(function (r, element) {
                        if (element === name) {
                            return r;
                        } else if (r && r.length > 0) {
                            return r + " " + element;
                        } else {
                            return element;
                        }
                    }, ""));
                }
                return element;
            }
        },
        getRelativeLeft: function (element, parent) {
            /// <signature>
            /// <summary>Gets the left coordinate of the specified element relative to the specified parent.</summary>
            /// <param name="element">The element.</param>
            /// <param name="parent">The parent element.</param>
            /// <returns type="Number">The relative left coordinate.</returns>
            /// </signature>
            if (!element) {
                return 0;
            }
            var left = element.offsetLeft;
            var current = element.parentNode;
            while (current) {
                left -= current.offsetLeft;
                if (current === parent) {
                    break;
                }
                current = current.parentNode;
            }
            return left;
        },
        getRelativeTop: function (element, parent) {
            /// <signature>
            /// <summary>Gets the top coordinate of the element relative to the specified parent.</summary>
            /// <param name="element">The element.</param>
            /// <param name="parent">The parent element.</param>
            /// <returns type="Number">The relative top coordinate.</returns></signature>
            /// </signature>
            if (!element) {
                return 0;
            }
            var top = element.offsetTop;
            var current = element.parentNode;
            while (current) {
                top -= current.offsetTop;
                if (current === parent) {
                    break;
                }
                current = current.parentNode;
            }
            return top;
        },
        getContentWidth: function (element) {
            /// <signature>
            /// <summary>Gets the width of the content of the specified element. The content width does not include borders or padding.</summary>
            /// <param name="element" type="HTMLElement">The element.</param>
            /// <returns type="Number">The content width of the element.</returns>
            /// </signature>
            var border = getDimension(element, "borderLeftWidth") + getDimension(element, "borderRightWidth");
            var padding = getDimension(element, "paddingLeft") + getDimension(element, "paddingRight");
            return element.offsetWidth - border - padding;
        },
        getTotalWidth: function (element) {
            /// <signature>
            /// <summary>Gets the width of the element, including margins.</summary>
            /// <param name="element" type="HTMLElement">The element.</param>
            /// <returns type="Number">The width of the element including margins.</returns>
            /// </signature>
            var margin = getDimension(element, "marginLeft") + getDimension(element, "marginRight");
            return element.offsetWidth + margin;
        },
        getContentHeight: function (element) {
            /// <signature>
            /// <summary>Gets the height of the content of the specified element. The content height does not include borders or padding.</summary>
            /// <param name="element" type="HTMLElement">The element.</param>
            /// <returns type="Number" integer="true">The content height of the element.</returns>
            /// </signature>
            var border = getDimension(element, "borderTopWidth") + getDimension(element, "borderBottomWidth");
            var padding = getDimension(element, "paddingTop") + getDimension(element, "paddingBottom");
            return element.offsetHeight - border - padding;
        },
        getTotalHeight: function (element) {
            /// <signature>
            /// <summary>Gets the height of the element, including its margins.</summary>
            /// <param name="element" type="HTMLElement">The element.</param>
            /// <returns type="Number">The height of the element including margins.</returns>
            /// </signature>
            var margin = getDimension(element, "marginTop") + getDimension(element, "marginBottom");
            return element.offsetHeight + margin;
        },
        getPosition: function (element) {
            /// <signature>
            /// <summary>Gets the position of the specified element.</summary>
            /// <param name="element" type="HTMLElement">The element.</param>
            /// <returns type="Object">An object that contains the left, top, width and height properties of the element.</returns>
            /// </signature>
            var fromElement = element;
            var offsetParent = element.offsetParent;
            var top = element.offsetTop;
            var left = element.offsetLeft;
            while ((element = element.parentNode) && element !== document.body && element !== document.documentElement) {
                top -= element.scrollTop;
                var dir = document.defaultView.getComputedStyle(element, null).direction;
                left -= dir !== "rtl" ? element.scrollLeft : -element.scrollLeft;
                if (element === offsetParent) {
                    top += element.offsetTop;
                    left += element.offsetLeft;
                    offsetParent = element.offsetParent;
                }
            }
            return {
                left: left,
                top: top,
                width: fromElement.offsetWidth,
                height: fromElement.offsetHeight
            };
        },
        convertToPixels: function (element, value) {
            /// <signature>
            /// <summary>Converts a CSS positioning string for the specified element to pixels.</summary>
            /// <param name="element" type="HTMLElement">The element.</param>
            /// <param name="value" type="String">The CSS positioning string.</param>
            /// <returns type="Number">The number of pixels.</returns>
            /// </signature>
            if (!_pixelsRE.test(value) && _numberRE.test(value)) {
                var previousValue = element.style.left;
                element.style.left = value;
                value = element.style.pixelLeft;
                element.style.left = previousValue;
                return value;
            }
            return parseInt(value, 10) || 0;
        },
        getText: function (element) {
            /// <signature>
            /// <summary>Gets the width of the content of the specified element. The content width does not include borders or padding.</summary>
            /// <param name="element" type="HTMLElement">The element.</param>
            /// <returns type="String">The text between the start and end tags.</returns>
            /// </signature>
            return element[textProp] || "";
        },
        setText: function (element, value) {
            /// <signature>
            /// <summary>Sets or retrieves the text between the start and end tags of the object.</summary>
            /// <param name="element" type="HTMLElement">The element.</param>
            /// <param name="value" type="String">The text between the start and end tags.</param>
            /// </signature>
            element[textProp] = value || "";
        }
    });

    _Global.async = async;

    if (jQuery) {
        jQuery.cookie = cookie;
    }

})(window, WinJS);
