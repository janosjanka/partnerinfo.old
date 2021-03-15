// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (_Global, _Globalize, _WinJS) {
    "use strict";

    var ns = _WinJS.Namespace.defineWithParent(_WinJS, "Resources", {

        init: function (culture) {
            /// <signature>
            /// <summary>Initializes application resources</summary>
            /// <param name="culture" type="String" optional="true" />
            /// </signature>
            _Globalize.culture(culture || "en");
        },
        define: function (lang, resources) {
            /// <signature>
            /// <summary>Defines a language resource object</summary>
            /// <param name="lang" type="String" />
            /// <param name="resources" type="Object" />
            /// </signature>
            _Globalize.addCultureInfo(lang, { messages: resources });
        },
        localize: function (key) {
            /// <signature>
            /// <summary>Gets a value for the given key</summary>
            /// <param name="key" type="String" />
            /// <returns type="Object" />
            /// </signature>
            if (!key) {
                return;
            }

            var keys = key.split('.');
            var klen = keys.length;
            if (klen === 0) {
                return;
            }

            var item = _Globalize.localize(keys[0]);

            if (klen === 1) return item;
            if (klen === 2) return item && item[keys[1]];
            if (klen === 3) return item && (item = item[keys[1]]) && item[keys[2]];

            for (var i = 1; item && i < klen; ++i) {
                item = item[keys[i]];
            }

            return item;
        },
        format: function (value, format) {
            /// <signature>
            /// <summary>Formats the given value</summary>
            /// <param name="value" type="Object" />
            /// <param name="format" type="String" optional="true" />
            /// <returns type="String" />
            /// </signature>
            return _Globalize.format(value, format);
        }

    });

    // C++ like macro for localization

    _Global._T = ns.localize;

})(window, Globalize, WinJS);
