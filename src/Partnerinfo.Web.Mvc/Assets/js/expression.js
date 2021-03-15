// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (_WinJS) {
    "use strict";

    window.tinymce = {
        base: PI.Server.mapPath("/js/base/tinymce"),
        suffix: ".min"
    };

    var ExpressionType = {
        unknown: null,
        link: "",
        from: "feladó",
        to: "címzett",
        invitation: "meghívás"
    },

    Expression = {
        regexp: /\{\{\s{0,1}(\S*?)\.(\S*?)\s{0,1}\}\}/.source, // Grouped syntax $ ( type ) . ( property ) $
        regexpVisual: /\{\{\s{0,1}\S*?\.\S*?\s{0,1}\}\}/gi, // TinyMCE syntax
        format: "{{ {0}.{1} }}",
        
        //format: "{{ {0}.{1} }}",
        make: function (type, propOrValue /*, format */) {
            /// <signature>
            /// <summary>Creates a new template expression.</summary>
            /// <param name="type" type="PI.ExpressionType">The type of the expression.</param>
            /// <param name="propOrValue" type="String">The name or value of the property.</param>
            /// </signature>
            //if (!type) {
            //    throw new Error("invalid expression: " + type);
            //}
            return String.format(arguments[2] || this.format, type, propOrValue || "");
        },
        parseAll: function (expression, callback) {
            /// <signature>
            /// <summary>Parses a template expression.</summary>
            /// </signature>
            if (typeof callback !== "function") {
                throw new TypeError("callback must be a function");
            }
            if (expression) {
                var match;
                var regexp = new RegExp(this.regexp, "gi");
                while (match = regexp.exec(expression)) {
                    if ((match.length >= 3) && callback({ type: match[1], value: match[2] })) {
                        break;
                    }
                }
            }
        },
        parseFirst: function (expression, type) {
            /// <signature>
            /// <summary>Parses the first template expression.</summary>
            /// <param name="expression" type="String" />
            /// <param name="type" type="PI.ExpressionType" optional="true" />
            /// <returns type="Object" />
            /// </signature>
            var result;
            if (expression) {
                this.parseAll(expression, function (current) {
                    if (type === undefined || current.type === type) {
                        result = current;
                        return true;
                    }
                });
            }
            return result || {
                type: PI.ExpressionType.unknown,
                value: undefined
            };
        },
        replaceAll: function (expression, callback) {
            if (typeof callback !== "function") {
                throw new TypeError("callback must be a function");
            }
            if (expression) {
                return expression.replace(new RegExp(this.regexp, "gi"), callback);
            }
            return "";
        },
        isValid: function (expression) {
            var result = this.parseFirst(expression);
            var type = PI.ExpressionType;
            for (var p in type) {
                if (type[p] === result.type) {
                    return true;
                }
            }
            return false;
        }
    };

    //
    // Public Namespace Exports
    //

    _WinJS.Namespace.define("PI", {
        ExpressionType: ExpressionType,
        Expression: Expression
    });

})(WinJS);