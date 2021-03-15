// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (window, $, _Base, undefined) {
    "use strict";
    /*
        var CSNumber = _Base.Class.define(function CSNumber_ctor(value) {
            /// <signature>
            /// <summary>Initializes a new instance of the CSNumber class.</summary>
            /// <param name="value" type="String" />
            /// <returns type="CSNumber" />
            /// </signature>
            this.value = value;
        }, {
            toString: function () {
                /// <signature>
                /// <returns type="String" />
                /// </signature>
                return this.value;
            }
        });
    
        var CSBox = _Base.Class.define(function CSBox_ctor(left, top, right, bottom) {
            /// <signature>
            /// <summary>Initializes a new instance of the CSBox class.</summary>
            /// <param name="left" type="Number" optional="true" />
            /// <param name="top" type="Number" optional="true" />
            /// <param name="right" type="Number" optional="true" />
            /// <param name="bottom" type="Number" optional="true" />
            /// <returns type="CSBox" />
            /// </signature>
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }, {
            toString: function () {
                /// <signature>
                /// <returns type="String" />
                /// </signature>
                return [this.top, this.right, this.bottom, this.left].join(" ");
            }
        });
    
        var styleMap = [{
            name: "Méret és pozíció",
            prop: {
                "width": CSNumber,
                "height": CSNumber,
                "margin": CSBox,
                "padding": CSBox
            }
        }];
    */

    $.widget("ui.cssEditor", {
        options: {
            selected: null,
            delay: 25
        },
        _create: function () {
            /// <signature>
            /// <summary>Initializes a new instance of the cssEditor widget.</summary>
            /// </signature>
            this.element.addClass("ui-csse");
            this.map = this._createCssMap();
            this._build();
        },
        _init: function () {
            /// <signature>
            /// <summary>Initializes a new instance of the cssEditor widget.</summary>
            /// </signature>
            this.refresh();
        },
        refresh: function (rebuild) {
            /// <signature>
            /// <summary>Reads all properties from the selected element.</summary>
            /// <param name="rebuild" type="Boolean" />
            /// </signature>
            _Base.Utilities.async(function () {
                if (rebuild) {
                    this._build();
                }
                var selected = this.options.selected;
                if (!selected) {
                    if (this.filterInput.val()) {
                        this.filterInput.val("").trigger("keyup");
                    }
                    this._showOverlay(true);
                    return;
                }
                this._showOverlay(false);
                this._fill();
            }, this, this.options.delay);
        },
        _build: function () {
            /// <signature>
            /// <summary>Rebuilds the entire editor.</summary>
            /// </signature>
            this.element.empty();

            // Adds an overlay element to the CSS editor.
            this.overlay = $("<div>").css({
                position: "absolute",
                left: 0,
                right: 0,
                top: 0,
                bottom: 0,
                background: "#fff",
                opacity: 0.5,
                zIndex: 1
            }).appendTo(this.element)[0];

            this._addGroups();

            // Adds a filter box to the CSS editor.
            this.filterInput = $("<input type='search' placeholder='Stílus keresése...'>")
                .bind("keyup.csse search.csse", this._filter.bind(this))
                .appendTo($("<div class='ui-csse-filter'>").prependTo(this.element));
        },
        _createCssMap: function (lang) {
            var colorValues = ["black", "blue", "green", "red", "yellow", "brown", "gray", "white"],
                widthHeightValues = ["auto", "-", "250px", "500px", "750px", "1000px", "-", "25%", "50%", "75%", "100%"],
                marginPaddingValues = ["0px", "5px", "10px", "20px", "-", "auto"],
                textValues = ["normal", "-", "5px", "10px", "20px", "-", "1em", "1.5em", "2em"],
                overflowValues = ["auto", "hidden", "scroll", "visible"],
                shadowValues = ["0px 0px 16px #000", "0px 0px 16px #ccc", "0px 0px 16px #fff", "-", "4px 4px 8px #000", "4px 4px 8px #ccc", "4px 4px 8px #fff", "-", "4px -4px 8px #000", "4px -4px 8px #ccc", "4px -4px 8px #fff", "-", "-4px -4px 8px #000", "-4px -4px 8px #ccc", "-4px -4px 8px #fff"],
                properties = [
                    { category: true, name: "Méret és pozíció" },
                    { name: "Szélesség", property: "width", values: widthHeightValues },
                    { name: "Magasság", property: "height", values: widthHeightValues },
                    { name: "Min. szélesség", property: "min-width", values: widthHeightValues },
                    { name: "Min. magasság", property: "min-height", values: widthHeightValues },
                    { name: "Pozíció", property: "position", values: ["absolute", "fixed", "relative", "static"] },
                    { name: "Balról", property: "left" },
                    { name: "Fentről", property: "top" },
                    { name: "Jobbról", property: "right" },
                    { name: "Lentről", property: "bottom" },
                    { name: "Réteg sorrend", property: "z-index", values: ["auto", "-", "0", "1", "2", "3", "4", "5"] },

                    { category: true, name: "Margó és behúzás" },
                    { name: "Margó balról", property: "margin-left", values: marginPaddingValues },
                    { name: "Margó fentről", property: "margin-top", values: marginPaddingValues },
                    { name: "Margó jobbról", property: "margin-right", values: marginPaddingValues },
                    { name: "Margó lentről", property: "margin-bottom", values: marginPaddingValues },
                    { name: "Behúzás balról", property: "padding-left", values: marginPaddingValues },
                    { name: "Behúzás fentről", property: "padding-top", values: marginPaddingValues },
                    { name: "Behúzás jobbról", property: "padding-right", values: marginPaddingValues },
                    { name: "Behúzás lentről", property: "padding-bottom", values: marginPaddingValues },

                    { category: true, name: "Megjelenés és láthatóság" },
                    { name: "Megjelenés", property: "display", values: ["block", "inline", "inline-block", "inline-table", "list-item", "none", "run-in", "table", "table-caption", "table-cell", "table-column", "table-column-group", "table-row", "table-row-group", "table-footer-group", "table-header-group"] },
                    { name: "Láthatóság", property: "visibility", values: ["collapse", "hidden", "visible"] },
                    { name: "Lebegés", property: "float", values: ["left", "none", "right"] },
                    { name: "Oldaltörés", property: "clear", values: ["both", "left", "none", "right"] },
                    { name: "Egér kurzor", property: "cursor", values: ["auto", "crosshair", "default", "e-resize", "help", "move", "n-resize", "ne-resize", "nw-resize", "pointer", "progress", "s-resize", "se-resize", "sw-resize", "text", "w-resize", "wait"] },
                    { name: "Túlcsordulás", property: "overflow", values: overflowValues },
                    { name: "Túlcsordulás X", property: "overflow-x", values: overflowValues },
                    { name: "Túlcsordulás Y", property: "overflow-y", values: overflowValues },

                    { category: true, name: "Blokk keret" },
                    { name: "Keret stílus", property: "border-style", values: ["dashed", "dotted", "double", "groove", "hidden", "inset", "none", "outset", "ridge", "solid"] },
                    { name: "Keret szélesség", property: "border-width", values: ["medium", "thick", "thin", "-", "1px", "2px", "4px", "8px", "16px", "32px", "64px"] },
                    { name: "Keret szín", property: "border-color", values: colorValues },
                    { name: "Keret sugár", property: "border-radius", values: ["1px", "2px", "4px", "8px", "16px", "32px", "64px"] },
                    { name: "Keret árnyék", property: "box-shadow", values: shadowValues },
                    { name: "Keret méretezés", property: "box-sizing", values: ["content-box", "border-box", "initial", "inherit"] },

                    { category: true, name: "Blokk háttér" },
                    { name: "Háttér szín", property: "background-color", values: colorValues },
                    { name: "Háttér kép", property: "background-image" },
                    { name: "Háttér ismétlés", property: "background-repeat", values: ["no-repeat", "repeat", "repeat-x", "repeat-y"] },
                    { name: "Háttér rögzítés", property: "background-attachment", values: ["fixed", "scroll"] },
                    { name: "Háttér pozíció", property: "background-position", values: ["bottom", "center", "left", "right", "top"] },
                    { name: "Háttér méret", property: "background-size", values: ["25%", "50%", "100%"] },
                    { name: "Háttér áttetszőség", property: "opacity", values: ["0", "0.1", "0.2", "0.3", "0.4", "0.5", "0.6", "0.7", "0.8", "0.9", "1"] },

                    { category: true, name: "Bekezdés és igazítás" },
                    { name: "Sormagasság", property: "line-height", values: textValues },
                    { name: "Függőleges igazítás", property: "vertical-align", values: ["baseline", "bottom", "middle", "sub", "super", "text-bottom", "text-top", "top"] },
                    { name: "Szöveg igazítás", property: "text-align", values: ["center", "justify", "left", "right"] },
                    { name: "Szöveg bekezdés", property: "text-indent", values: textValues },
                    { name: "Nem látszó karakterek", property: "white-space", values: ["normal", "nowrap", "pre", "pre-line", "pre-wrap"] },
                    { name: "Szó távolság", property: "word-spacing", values: textValues },
                    { name: "Betű távolság", property: "letter-spacing", values: textValues },

                    { category: true, name: "Betűtípus és szöveg" },
                    { name: "Betűtípus", property: "font-family", values: ["arial, monospace, sans-serif", "lucida grande, tahoma", "verdana, arial, sans-serif", "segoe ui, tahoma, arial, verdana, sans-serif", "verdana, helvetica, sans-serif", "times new roman, times, serif", "courier new, courier, monospace"] },
                    { name: "Betűméret", property: "font-size", values: ["large", "larger", "medium", "small", "smaller", "x-large", "x-small", "xx-large", "xx-small", "-", "10pt", "12pt", "14pt", "16pt", "18pt", "24pt", "32pt", "48pt"] },
                    { name: "Betűszín", property: "color", values: colorValues },
                    { name: "Betű vastagság", property: "font-weight", values: ["bold", "bolder", "lighter", "normal", "-", "100", "200", "300", "400", "500", "600", "700", "700", "800", "900"] },
                    { name: "Betű stílus", property: "font-style", values: ["italic", "normal", "oblique"] },
                    { name: "Betű változat", property: "font-variant", values: ["normal", "small-caps"] },
                    { name: "Szöveg transzformáció", property: "text-transform", values: ["capitalize", "lowercase", "none", "uppercase"] },
                    { name: "Szöveg dekoráció", property: "text-decoration", values: ["none", "underline", "overline", "line-through", "blink"] },
                    { name: "Szöveg árnyék", property: "text-shadow", values: shadowValues }
                ];
            return properties;
        },
        _addGroups: function () {
            /// <signature>
            /// <summary>Adds all property groups to the element.</summary>
            /// </signature>
            var group, style, fragment = $(document.createDocumentFragment());
            for (var i = 0, len = this.map.length; i < len; ++i) {
                style = this.map[i];
                if (style.category) {
                    group = this._addGroup(fragment, style);
                } else {
                    this._addProperty(group, style);
                }
            }
            this.element.append(fragment);
        },
        _addGroup: function (root, options) {
            /// <summary>Adds a new property group control to the element.</summary>
            /// <param name="root" type="Element">The root element.</param>
            /// <param name="options" type="Object">A set of key/value pairs that configure the group.</param>
            /// <returns type="$" />
            var content = $("<div>").addClass("ui-csse-grp-content");
            root.append(
                $("<div>")
                    .addClass("ui-csse-grp")
                    .append($("<h2>")
                        .addClass("ui-csse-grp-title")
                        .html(options.name))
                    .append(content));
            return content;
        },
        _addProperty: function (group, options, prepend) {
            /// <summary>Adds a new property control to the element.</summary>
            /// <param name="group" type="Element">The property group element.</param>
            /// <param name="options" type="Object">A set of key/value pairs that configure the group.</param>
            /// <returns type="Element" />
            var inputId = "csse_" + options.property;
            var input = $("<input>")
                .attr({
                    "id": inputId,
                    "type": "text",
                    "data-property": options.property,
                    "data-attribute": options.attribute
                })
                .bind("change.csse", this._change.bind(this));

            return group[prepend ? "prepend" : "append"]($("<div class='ui-csse-prop'>")
               .append($("<label>")
                   .attr({
                       "for": inputId,
                       "title": options.name.concat(" (", options.property, ")"),
                       "data-property": options.property
                   })
                   .text(options.name))
               .append($("<div>").addClass("ui-csse-input").append(input))
            );
        },
        _change: function (ev) {
            /// <summary>Occurs when the user changes the content of an input.</summary>
            /// <param name="ev" type="Object">The event object.</param>
            ev.preventDefault();
            ev.stopPropagation();
            var input = ev.currentTarget, value = String(input.value);
            if (value && (value.indexOf("javascript") > -1 || value.indexOf("expression") > -1)) {
                input.value = ""; // Anti XSS.
                return;
            }
            this._setProperty(
                input.getAttribute("data-property"),
                input.getAttribute("data-attribute") === "true",
                value);
            this._fill();
            return false;
        },
        _filter: function (ev) {
            /// <summary>Occurs when the user searches for a property.</summary>
            /// <param name="ev" type="Object">The event object.</param>
            ev.preventDefault();
            ev.stopPropagation();
            var self = this, timeout = window.setTimeout(function () {
                window.clearTimeout(timeout);
                var filterInput = ev.currentTarget,
                    filterValue = (filterInput.value || "").toLowerCase(),
                    isFilterValue = filterValue.length > 0,
                    i = 0,
                    l = self.map.length,
                    style,
                    input;
                for (; i < l; ++i) {
                    style = self.map[i];
                    input = window.document.getElementById("csse_" + style.property);
                    if (input) {
                        input = input.parentElement.parentElement;
                        if (!isFilterValue || style.name.toLowerCase().indexOf(filterValue) > -1) {
                            input.setAttribute("style", "display:block");
                        } else {
                            input.setAttribute("style", "display:none");
                        }
                    }
                }
                var titles = $("h2.ui-csse-grp-title", self.element);
                isFilterValue ? titles.hide() : titles.show();
            }, 500);
            return false;
        },
        _getProperty: function (property, attribute, element) {
            /// <summary>Gets the CSS value of the specified property.</summary>
            /// <param name="property" type="String">Name of the property.</param>
            /// <param name="element" type="Element">The DOM element.</param>
            /// <returns type="Object" />
            var selected = element || this.options.selected;
            var value;
            if (selected) {
                if (attribute) { // Attribute
                    value = selected.attr(property);
                } else { // Get a simple CSS value without getComputedStyle().
                    value = $.style(selected[0], property);
                }
            }
            return value || "";
        },
        _setProperty: function (property, attribute, value, element) {
            /// <summary>Applies the specified CSS style to the element.</summary>
            /// <param name="property" type="String">Name of the property.</param>
            /// <param name="value" type="Object">The value of the property.</param>
            /// <param name="element" type="Element">The DOM element.</param>
            var selected = element || this.options.selected;
            var node;
            if (selected) {
                if (attribute) { // Attribute
                    selected.attr(property, value);
                } else { // CSS
                    selected.css(property, value);
                }
            }
        },
        _fill: function () {
            /// <signature>
            /// <summary>Loads all CSS styles to the input fields.</summary>
            /// </signature>
            var selected = this.options.selected;
            if (!selected) {
                return;
            }
            // Create a new temp element without custom CSS styles.
            var temp = $(window.document.createElement(selected[0].tagName));
            var inputs = $("input", this.element);
            for (var i = 0, length = inputs.length; i < length; ++i) {
                var input = inputs[i];
                var property = input.getAttribute("data-property");
                if (property) {
                    var type = input.getAttribute("type");
                    var attribute = input.getAttribute("data-attribute") === "true";
                    var orgStyle = this._getProperty(property, attribute, temp);
                    var newStyle = this._getProperty(property, attribute);
                    input.value = (orgStyle !== newStyle ? newStyle : "");
                }
            }
        },
        _setOption: function (key, value) {
            /// <signature>
            /// <summary>Sets a property of the widget.</summary>
            /// <param name="key" type="String">Name of the property.</param>
            /// <param name="value" type="Object">The value of the property.</param>
            /// </signature>
            if (this.options[key] !== value) {
                if (key === "selected") {
                    this.options.selected = (value ? (value instanceof jQuery ? value : $(value)) : null);
                    this.refresh();
                } else {
                    this.options[key] = value;
                }
            }
        },
        _showOverlay: function (enabled) {
            /// <signature>
            /// <summary>Sets the visibility of the overlay control.</summary>
            /// <param name="enabled" type="bool">The visibility of the overlay control.</param>
            /// </signature>
            this.overlay.style.display = enabled ? "block" : "none";
        },
        _destroy: function () {
            /// <signature>
            /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            /// </signature>
            $("*", this.element).unbind(".csse").empty();
            this.element.removeClass("ui-csse");
        }
    });

})(window, jQuery, WinJS, undefined);
