// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (_Global, _KO, _WinJS, undefined) {
    "use strict";

    var _Class = _WinJS.Class;
    var _Utilities = _WinJS.Utilities;

    var ECMA6Map = _Global.Map ? _Global.Map : _Class.define(null, {
        get: function (key) {
            return this[key];
        },
        set: function (key, value) {
            this[key] = value;
            return this;
        }
    });

    var document = _Global.document;
    var hasClass = _Utilities.hasClass;
    var eventNames = {
        render: "render",
        command: "command",
        currentchanged: "currentchanged",
        selectionchanged: "selectionchanged",
        itemrender: "itemrender",
        itemclick: "itemclick",
        itemdrag: "itemdrag",
        itemdragstart: "itemdragstart",
        itemdragend: "itemdragend",
        itemdragenter: "itemdragenter",
        itemdragleave: "itemdragleave",
        itemdragover: "itemdragover"
    };
    var templateCache = new ECMA6Map();

    function makeTemplate(template) {
        /// <signature>
        /// <summary>Parses the specified template string and returns with the content of the template.</summary>
        /// <param name="template" type="String">The name of the template script tag or the (inline) html markup.</param>
        /// <returns type="String" />
        /// </signature>
        if (template.charCodeAt(0) === /* # */ 35) { // In this case, the template refers to a DOM element.
            var elementId = template.substr(1);
            template = elementId && templateCache.get(elementId);
            if (template) {
                return template;
            }
            var element = document.getElementById(elementId);
            if (!element) {
                throw new Error("Cannot find template element: " + template);
            }
            templateCache.set(elementId, template = element.innerHTML);
        }
        return template;
    }
    function createElement(tagName, className, innerHTML) {
        /// <signature>
        /// <summary>Creates a new HTML element with the specified class name.</summary>
        /// <param name="tagName" type="String" />
        /// <param name="className" type="String" optional="true" />
        /// <param name="innerHTML" type="String" optional="true" />
        /// <returns type="HTMLElement" />
        /// </signature>
        var element = document.createElement(tagName);
        className && (element.className = className);
        innerHTML && (element.innerHTML = innerHTML);
        return element;
    }
    function forEachParent(startElement, rootElement, callback) {
        /// <signature>
        /// <summary>Finds the parent element of the specified element by className.</summary>
        /// <param name="startElement" type="HTMLElement" />
        /// <param name="rootElement" type="HTMLElement" />
        /// <param name="callback" type="Function" />
        /// </signature>
        while ((startElement) &&
               (!callback(startElement)) &&
               (startElement !== rootElement) &&
               (startElement !== document) &&
               (startElement = startElement.parentNode));
    }
    function addEventListener(element, name, listener) {
        /// <signature>
        /// <summary>Registers an event listener for the element.</summary>
        /// <param name="element" type="HTMLElement" />
        /// <param name="name" type="String" />
        /// <param name="listener" type="Function" />
        /// </signature>
        if (element.addEventListener) {
            element.addEventListener(name, listener, false);
        } else {
            element.attachEvent("on" + name, listener);
        }
    }
    function removeEventListener(element, name, listener) {
        /// <signature>
        /// <summary>Removes an event listener from the element.</summary>
        /// <param name="element" type="HTMLElement" />
        /// <param name="name" type="String" />
        /// <param name="listener" type="Function" />
        /// </signature>
        if (element.removeEventListener) {
            element.removeEventListener(name, listener, false);
        } else {
            element.detachEvent("on" + name, listener);
        }
    }
    function cancelEventBubbling(event) {
        /// <signature>
        /// <summary>Cancels event bubbling regardless of browser.</summary>
        /// <param name="event" type="Event" />
        /// </signature>
        if (event.stopPropagation) {
            event.stopPropagation();
        } else {
            event.cancelBubble = true; // IE 8
        }
    }

    var SelectionMode = {
        /// <field name="none" type="Number" integer="true">No items can be selected.</field>
        none: "none",
        /// <field name="single" type="Number" integer="true">The user can select only one item at a time.</field>
        single: "single",
        /// <field name="multi" type="Number" integer="true">The user can select multiple items.</field>
        multi: "multi"
    };

    //
    // public class ListViewColumn
    //

    var ListViewColumn = _Class.define(function ListViewColumn_ctor(options) {
        /// <signature>
        /// <summary>Initializes a new instance of the ListViewColumn class.</summary>
        /// <param name="options" type="Object">An object that contains one or more property/value pairs to apply to the new control.</param>
        /// <returns type="ListViewColumn" />
        /// </signature>
        options = options || {};
        this.binding = options.binding; // Gets or sets the data item to bind to for this column.
        this.header = options.header || "&nbsp;"; // Gets or sets the content of the header of a ListViewColumn.
        this.headerClass = "ui-listview-item" + (options.headerClass ? (" " + options.headerClass) : ""); // Gets or sets the CSS class of the header of a ListViewColumn.
        this.headerStyle = options.headerStyle; // Gets or sets the inline CSS style of the header of a ListViewColumn.
        this.headerTemplate = options.headerTemplate; // Gets or sets the template to use to display the content of the column header.
        this.cellClass = "ui-listview-item" + (options.cellClass ? (" " + options.cellClass) : ""); // Gets or sets the CSS class of the column cell.
        this.cellStyle = options.cellStyle; // Gets or sets the inline CSS style of the column cell.
        this.cellTemplate = options.cellTemplate; // Gets or sets the template to use to display the contents of a column cell.
        this.cellTemplateSelector = typeof this.cellTemplate === "function"; // Cell template selector
        this.width = options.width; // Gets or sets the width of the column.
        this.minWidth = options.minWidth; // Gets or sets the minimum width of column.

        this.compHeaderTemplate = this.headerTemplate && makeTemplate(this.headerTemplate);
        this.compCellTemplate = !this.cellTemplateSelector && this.cellTemplate && makeTemplate(this.cellTemplate);
    }),

    ListViewGroup = _Class.define(function ListViewGroup_ctor(options) {
        /// <signature>
        /// <summary>Initializes a new instance of the ListViewGroup class.</summary>
        /// <param name="options" type="Object">An object that contains one or more property/value pairs to apply to the new control.</param>
        /// <returns type="ListViewGroup" />
        /// </signature>
        options = options || {};
        this.binding = options.binding;
        this.headClass = "ui-listview-group" + (options.headClass ? (" " + options.headClass) : "");
        this.headStyle = options.headStyle;
        this.bodyClass = "ui-listview-group" + (options.bodyClass ? (" " + options.bodyClass) : "");
        this.bodyStyle = options.bodyStyle;
    }),

    ListViewBinding = _Class.define(function ListViewBinding_ctor(listContext, list, item, index) {
        /// <signature>
        /// <summary>Initializes a new instance of the ListViewBinding class.</summary>
        /// <param name="listContext" type="Object" />
        /// <param name="list" type="Object" />
        /// <param name="item" type="Object" />
        /// <param name="index" type="Number" />
        /// <returns type="ListViewBinding" />
        /// </signature>
        this.listViewGroup = this;
        this.listContext = listContext;
        this.list = list;
        this.item = item;
        this.index = ko.observable(index | 0);
    }),

    //
    // public class ListView
    //

    ListView = _Class.define(function ListView_ctor(element, options) {
        /// <signature>
        /// <summary>Initializes a new instance of the ListView class.</summary>
        /// <param name="element" type="HTMLElement">The DOM element that hosts the ListView control.</param>
        /// <param name="options" type="Object" optional="true">An object that contains one or more property/value pairs to apply to the new control.
        /// <para>context: The data context object that will exposed as $listContext.</para>
        /// <para>displayHeader: A value indicating whether the header is displayed.</para>
        /// <para>displaySelectionCheckbox: A value indicating whether the selectionbox is displayed.</para>
        /// <para>columns: An array of native column descriptor objects.</para>
        /// <para>synchronize: A value that indicates whether a list should keep the selected item synchronized with the current item.</para>
        /// <para>items: An observable array of items or a ListViewCollection instance.</para>
        /// <para>selection: An observable array of selected items.</para>
        /// <para>selectionMode: Specifies the selection behavior of a listView ( none | [ one ] | multi ).</para>
        /// <para>onrender: A function that can be called when the listView control is rendered.</para>
        /// <para>oncurrentchanged: A function that can be called when the current property has changed.</para>
        /// <para>onselectionchanged: A function that can be called when the selection array has changed.</para>
        /// </param>
        /// <returns type="ListView" />
        /// </signature>
        if (!element) {
            throw new TypeError("DOM element is required");
        }

        options = options || {};

        var items;
        var selection;
        var list;
        if (options.items instanceof _WinJS.Knockout.List) {
            list = options.items;
            items = list.items;
            selection = list.selection;
        } else {
            if (options.synchronize) {
                throw new Error("synchronize is not supported.");
            }
            items = _KO.isObservable(options.items) ? options.items : _KO.observableArray(options.items || []);
            selection = _KO.isObservable(options.selection) ? options.selection : _KO.observableArray(options.selection || []);
        }
        this._disposed = false;
        this._body = null;
        this._lastItem = null;
        this._fixedWidth = -1;
        this._onclickBound = this._onclick.bind(this);

        this.list = list;
        this.group = new ListViewGroup(options.group);
        this.columns = this._createColumns(options.columns || []);
        this.context = options.context;
        this.displayHeader = options.displayHeader !== false;
        this.displayRowIndex = !!options.displayRowIndex;
        this.displaySelectionCheckbox = options.displaySelectionCheckbox !== false;
        this.element = element;
        this.emptyTemplate = options.emptyTemplate;
        this.synchronize = !!options.synchronize;
        this.items = items;
        this.selection = selection;
        this.selectionMode = options.selectionMode || SelectionMode.single;
        this.itemsReorderable = !!options.itemsReorderable;

        this._emptyTemplateHtml = this.emptyTemplate && makeTemplate(this.emptyTemplate);

        _Utilities.setOptions(this, options);
        _Utilities.addClass(element, "ui-listview");

        this._itemsChangedSn = this.items.subscribe(this._onitemschanged, this, "arrayChange");
        this._selectionSn = this.selection.subscribe(this._onselectionchanged, this);
        this._listSn = this.synchronize && this.list ? this.list.current.subscribe(this._oncurrentchanged, this) : null;
        addEventListener(this.element, "click", this._onclickBound);

        this._render();
    }, {
        current: function () {
            /// <signature>
            /// <summary>Gets the current item from the list.</summary>
            /// <returns type="Object" />
            /// </signature>
            return this.list && this.list.current();
        },
        moveTo: function (newItem) {
            /// <signature>
            /// <summary>Sets the specified item to be the current in the view.</summary>
            /// <param name="newItem" type="Object">The item to set as the current.</param>
            /// </signature>
            if (this.list && (newItem !== undefined)) {
                this.list.moveTo(newItem);
            }
        },
        select: function (item, single) {
            /// <signature>
            /// <summary>Adds the specified item to the selected items array.</summary>
            /// <param name="item" type="Object">The item to select.</param>
            /// <param name="single" type="Boolean" optional="true">Forces single selection mode.</param>
            /// <returns type="Boolean" />
            /// </signature>
            if ((this.selectionMode === SelectionMode.single) || single) {
                this.selection([item]);
            } else if (this.selection.indexOf(item) === -1) {
                this.selection.push(item);
            }
            this._lastItem = item;
        },
        deselect: function (item) {
            /// <signature>
            /// <summary>Removes the specified item from the selected items array.</summary>
            /// <param name="item" type="Object">The item to deselect.</param>
            /// </signature>
            this.selection.remove(item);
        },
        toggleSelect: function (item) {
            /// <signature>
            /// <summary>Toggles selection of the specified item.</summary>
            /// <param name="item" type="Object">The item to select/deselect.</param>
            /// </signature>
            if (this.selection.indexOf(item) >= 0) {
                this.deselect(item);
            } else {
                this.select(item);
            }
        },
        selectRange: function (from, to) {
            /// <signature>
            /// <summary>Selects the specified range.</summary>
            /// <param name="from" type="Number" integer="true">The first element in the range.</param>
            /// <param name="to" type="Number" integer="true">The last element in the range.</param>
            /// </signature>
            var items = this.items.peek();
            var len = items.length;
            if (from > to) {
                var tmp = from;
                from = to;
                to = tmp;
            }
            if (from < 0) { from = 0; }
            if (to >= len) { to = len - 1; }
            for (; from <= to; ++from) {
                var item = items[from];
                if (item !== null && item !== undefined) {
                    this.select(items[from]);
                }
            }
        },
        selectAll: function () {
            /// <signature>
            /// <summary>Selects all the items in a listView.</summary>
            /// </signature>
            this.selection(this.items.peek().slice(0));
        },
        deselectAll: function () {
            /// <signature>
            /// <summary>Removes all selections.</summary>
            /// </signature>
            this.selection.removeAll();
        },
        toggleAllSelections: function () {
            if (this.items().length === this.selection().length) {
                this.deselectAll();
            } else {
                this.selectAll();
            }
        },

        //
        // Event
        //

        _onclick: function (event) {
            /// <signature>
            /// <param name="event" type="MouseEvent" />
            /// <returns type="Boolean" />
            /// </signature>
            //cancelEventBubbling(event);

            var eventInfo = {
                target: event.target || event.srcElement,
                checkbox: null,
                group: null,
                head: null,
                body: null,
                list: this.list || this.items,
                item: null,
                commandTarget: null,
                command: null,
                ctrlKey: event.ctrlKey,
                shiftKey: event.shiftKey
            };

            forEachParent(eventInfo.target, this.element, function (element) {
                if (element.hasAttribute("data-command")) {
                    eventInfo.commandTarget = element;
                    eventInfo.command = element.getAttribute("data-command");
                }
                if (hasClass(element, "ui-listview-toggle")) { eventInfo.checkbox = element; }
                if (hasClass(element, "ui-listview-group")) { eventInfo.group = element; }
                if (hasClass(element, "ui-listview-head")) { eventInfo.head = element; return true; }
                if (hasClass(element, "ui-listview-body")) { eventInfo.body = element; return true; }
                return false;
            });

            if (!eventInfo.group) {
                return;
            }

            eventInfo.item = _KO.dataFor(eventInfo.group);

            this._handleCheckboxClick(eventInfo)
                || this._handleItemClick(eventInfo)
                || this._handleCommand(eventInfo);
        },
        _handleCheckboxClick: function (eventInfo) {
            /// <signature>
            /// <param name="eventInfo" type="Object" />
            /// <param name="Boolean" />
            /// </signature>
            if (!eventInfo.checkbox) {
                return false;
            }
            if (this.selectionMode === SelectionMode.none) {
                return true;
            }
            if (hasClass(eventInfo.checkbox, "ui-listview-checkall")) {
                if (this.selectionMode === SelectionMode.multi) {
                    this.toggleAllSelections();
                }
                return true;
            }
            this.synchronize && this.moveTo(eventInfo.item);
            this.toggleSelect(eventInfo.item);
            return true;
        },
        _handleItemClick: function (eventInfo) {
            /// <signature>
            /// <param name="eventInfo" type="Object" />
            /// <param name="Boolean" />
            /// </signature>
            if (eventInfo.head
                || this.dispatchEvent(eventNames.itemclick, eventInfo)
                || this.selectionMode === SelectionMode.none) {
                return true;
            }
            this.synchronize && this.moveTo(eventInfo.item);
            if (this.selectionMode === SelectionMode.multi) {
                if (eventInfo.shiftKey) {
                    var items = this.items.peek();
                    this.selectRange(items.indexOf(this._lastItem), items.indexOf(eventInfo.item));
                    return true;
                }
            }
            if (eventInfo.ctrlKey) {
                this.toggleSelect(eventInfo.item);
                return true;
            }
            this.select(eventInfo.item, true);
            return false;
        },
        _handleCommand: function (eventInfo) {
            /// <signature>
            /// <param name="eventInfo" type="Object" />
            /// </signature>
            if (!eventInfo.commandTarget) {
                return false;
            }
            if (this.list && this.list.command(eventInfo.command)(eventInfo.item) === false) {
                return true;
            }
            return this.dispatchEvent(eventNames.command, eventInfo);
        },
        _onitemschanged: function (entries) {
            /// <signature>
            /// <param name="entries" type="Array" />
            /// </signature>
            /// <var type="Number" integer="true" />
            var that = this;
            _Global.setTimeout(function __onitemschanged() {
                /// <var type="Number" integer="true" />
                var diff = 0;
                /// <var type="Element" domElement="true" />
                var body = that._body;
                /// <var type="Element" domElement="true" />
                var node = null;
                /// <var type="Element" domElement="true" />
                var ref = null;
                /// <var type="DocumentFragment" domElement="true" />
                var nodeList = document.createDocumentFragment();
                /// <var type="Object" />
                var listContext = that.context;
                /// <var type="Object" />
                var list = that.list || that.items;

                var mutated = false;

                for (var i = 0, len = entries.length; i < len; ++i) {
                    var entry = entries[i];
                    var entryIndex = entry.index + diff;
                    if (entry.moved >= 0) {
                        if (entry.status === "deleted") {
                            var newIndex = entry.moved + diff;
                            var childNode = body.childNodes[entryIndex];
                            if (newIndex <= 0) {
                                body.insertBefore(childNode, body.firstChild);
                            } else if (newIndex >= body.childNodes.length - 1) {
                                body.appendChild(childNode);
                            } else {
                                body.insertBefore(childNode, body.childNodes[newIndex]);
                            }
                            mutated = true;
                        }
                    }
                    else if (entry.status === "added") {
                        ref = ref || body.childNodes[entryIndex /*+ 1*/] || null;
                        node = that._createBodyGroup(new ListViewBinding(listContext, list, entry.value, entryIndex));
                        if (node) {
                            nodeList.appendChild(node);
                            mutated = true;
                        }
                    }
                    else if (entry.status === "deleted") {
                        node = body.childNodes[entryIndex];
                        if (node) {
                            _KO.cleanNode(node);
                            body.removeChild(node);
                            diff -= 1;
                            mutated = true;
                        }
                    }
                }
                body.insertBefore(nodeList, ref);

                if (mutated) {
                    for (i = 0, len = body.childNodes.length; i < len; ++i) {
                        var ctx = ko.contextFor(body.childNodes[i]);
                        ctx && ctx.$index(i);
                    }
                }
            });
        },
        _onselectionchanged: function (value) {
            /// <signature>
            /// <param name="value" type="Array" />
            /// </signature>
            value = value || [];
            this._applyState(this.current(), value);
            this.dispatchEvent(eventNames.selectionchanged, { list: this.list || this.items, items: value });
        },
        _oncurrentchanged: function (value) {
            /// <signature>
            /// <param name="value" type="Object">The current item.</param>
            /// </signature>
            this._applyState(value, this.selection());
            this.dispatchEvent(eventNames.currentchanged, { list: this.list || this.items, item: value });
        },

        //
        // DOM
        //

        _render: function () {
            /// <signature>
            /// <summary>Renders the listView control to the host element.</summary>
            /// </signature>
            var fragment = document.createDocumentFragment();
            this.displayHeader && fragment.appendChild(this._createHead());
            this._body = fragment.appendChild(this._createBody());
            fragment.appendChild(this._createEmpty());
            this._applyState(this.current(), this.selection());
            var t = createElement("div", "ui-listview-inner");
            t.appendChild(fragment);
            this.element.appendChild(t);
            this.dispatchEvent(eventNames.render);
        },
        _applyState: function (current, selection) {
            /// <signature>
            /// <summary>Repaints the listView control adding/removing CSS class.</summary>
            /// <param name="current" type="Object" />
            /// <param name="selection" type="Array" />
            /// </signature>
            if (this.displaySelectionCheckbox && (this.selectionMode === SelectionMode.multi)) {
                var checkAllElement = this.element.querySelector(".ui-listview-checkall>.ui-btn-checkbox");
                if (checkAllElement) {
                    var selLen = selection.length;
                    checkAllElement.setAttribute("aria-checked", (selLen > 0) && (selLen === this.items().length));
                }
            }
            var groupElements = this._body.childNodes;
            for (var i = 0, len = groupElements.length; i < len; ++i) {
                var groupElement = groupElements[i];
                var groupData = _KO.dataFor(groupElement);
                var selected = selection.indexOf(groupData) >= 0;
                if (selected) {
                    _Utilities.addClass(groupElement, "ui-listview-selected");
                } else {
                    _Utilities.removeClass(groupElement, "ui-listview-selected");
                }
                if (current === groupData) {
                    _Utilities.addClass(groupElement, "ui-listview-current");
                } else {
                    _Utilities.removeClass(groupElement, "ui-listview-current");
                }
                // Update the checkbox. Note: A checkbox can be added to other columns
                // so we ignore the displaySelectionCheckbox's value.
                var selectionBox = groupElement.querySelector(".ui-listview-toggle>.ui-btn-checkbox");
                selectionBox && selectionBox.setAttribute("aria-checked", selected);
            }
        },
        _createHead: function () {
            /// <signature>
            /// <summary>Creates a header for the listView using the specified columns.</summary>
            /// <returns type="HTMLElement" />
            /// </signature>
            var multiSelection = this.selectionMode === SelectionMode.multi;
            var thisGroup = this.group;
            var he = createElement("div", "ui-listview-head");
            var gr = createElement("div", thisGroup.headClass);
            thisGroup.headStyle && gr.setAttribute("style", thisGroup.headStyle);

            this.displayRowIndex && gr.appendChild(this._createRowIndex());
            this.displaySelectionCheckbox && gr.appendChild(this._createCheckBox(
                "ui-listview-toggle ui-listview-checkall",
                multiSelection ? false : null));

            for (var i = 0, len = this.columns.length; i < len; ++i)
                gr.appendChild(this._createHeadCell(this.columns[i]));

            he.appendChild(gr);
            return he;
        },
        _createHeadCell: function (column) {
            /// <signature>
            /// <summary>Creates a header cell element for the specified column.</summary>
            /// <param name="column" type="ListViewColumn">The listView column.</param>
            /// <returns type="HTMLElement" />
            /// </signature>
            var cell = createElement("div");
            column.headerClass && (cell.className = column.headerClass);
            column.headerStyle && (cell.setAttribute("style", column.headerStyle));
            column.width && (column.width !== "*") && (cell.style.width = column.width);
            column.minWidth && (cell.style.minWidth = column.minWidth);
            column.header && (cell.setAttribute("title", column.header), cell.innerHTML = column.header);
            return cell;
        },
        _createBody: function () {
            /// <signature>
            /// <summary>Creates a body for the listView using the specified columns.</summary>
            /// <returns type="HTMLElement" />
            /// </signature>
            var body = createElement("div", "ui-listview-body");
            var items = this.items();
            var listContext = this.context;
            var list = this.list || this.items;
            for (var i = 0, len = items.length, item; i < len; ++i) {
                if ((item = items[i]) !== undefined) {
                    body.appendChild(
                        this._createBodyGroup(
                            new ListViewBinding(listContext, list, item, i)));
                }
            }
            return body;
        },
        _createBodyGroup: function (bindings) {
            /// <signature>
            /// <summary>Creates a new group element.</summary>
            /// <returns type="HTMLElement" />
            /// </signature>
            var divGroup = createElement("div", this.group.bodyClass);
            this.group.bodyStyle && divGroup.setAttribute("style", this.group.bodyStyle);
            this.group.binding && divGroup.setAttribute("data-bind", this.group.binding);
            this.displayRowIndex && divGroup.appendChild(this._createRowIndex());
            this.displaySelectionCheckbox && divGroup.appendChild(this._createCheckBox("ui-listview-toggle", false));

            for (var i = 0, len = this.columns.length; i < len; ++i)
                divGroup.appendChild(this._createBodyCell(this.columns[i], bindings, divGroup));

            _KO.applyBindingsToNode(divGroup, bindings);
            return divGroup;
        },
        _createBodyCell: function (column, bindings, group) {
            /// <signature>
            /// <summary>Creates a body cell element for the specified column.</summary>
            /// <param name="column" type="ListViewColumn">The listView column.</param>
            /// <returns type="HTMLElement" />
            /// </signature>
            var cell = createElement("div", column.cellClass);
            column.cellStyle && cell.setAttribute("style", column.cellStyle);
            column.width && (column.width !== "*") && (cell.style.width = column.width);
            column.minWidth && (cell.style.minWidth = column.minWidth);
            if (column.cellTemplateSelector) {
                cell.innerHTML = makeTemplate(column.cellTemplate(bindings, column, group));
            } else if (column.compCellTemplate) {
                cell.innerHTML = column.compCellTemplate;
            } else if (column.binding) {
                cell.setAttribute("data-bind", column.binding);
            }
            return cell;
        },
        _createRowIndex: function () {
            /// <signature>
            /// <summary>Creates a row index element.</summary>
            /// <returns type="HTMLElement" />
            /// </signature>
            var numElement = createElement("div", "ui-listview-row");
            numElement.setAttribute("data-bind", "text:$index()+1");
            return numElement;
        },
        _createCheckBox: function (className, state, radio) {
            /// <signature>
            /// <summary>Creates a new checkbox container element.</summary>
            /// <param name="className" type="String" />
            /// <param name="state" type="String" />
            /// <param name="radio" type="Boolean" />
            /// <returns type="HTMLElement" />
            /// </signature>
            var checkElement = createElement("div", className);
            var checkBoxElement = createElement("div", "ui-btn ui-btn-checkbox");
            var checkFrameElement = createElement("span");
            var checkImageElement = createElement("i");
            checkBoxElement.setAttribute("role", radio ? "radio" : "checkbox");
            state !== null && state !== undefined && checkBoxElement.setAttribute("aria-checked", state);
            checkFrameElement.className = "ui-checkbox";
            checkFrameElement.appendChild(checkImageElement);
            checkBoxElement.appendChild(checkFrameElement);
            checkElement.appendChild(checkBoxElement);
            return checkElement;
        },
        _createEmpty: function () {
            /// <signature>
            /// <returns type="HTMLElement" />
            /// </signature>
            var emptyElement = createElement("div", "ui-listview-empty", this._emptyTemplateHtml);
            emptyElement.setAttribute("data-bind", "visible: !length");
            _KO.applyBindings(this.items, emptyElement);
            return emptyElement;
        },
        _createColumns: function (columns) {
            /// <signature>
            /// <param name="columns" type="Array" />
            /// <returns type="Array" />
            /// </signature>
            var len = columns.length;
            var listViewColumns = new Array(len);

            for (var i = 0; i < len; ++i)
                listViewColumns[i] = new ListViewColumn(columns[i] || {});

            return listViewColumns;
        },

        //
        // IDisposable
        //

        dispose: function () {
            /// <signature>
            /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            /// </signature>
            if (this._disposed) {
                return;
            }
            _Utilities.removeClass(this.element, "ui-listview");
            removeEventListener(this.element, "click", this._onclickBound);
            this._itemsChangedSn && this._itemsChangedSn.dispose();
            this._selectionSn && this._selectionSn.dispose();
            this._listSn && this._listSn.dispose();
            this._itemsChangedSn = null;
            this._selectionSn = null;
            this._listSn = null;
            this.element = null;
            this._disposed = true;
        }
    });

    //
    // Defines a class using the given constructor and the union of the set of instance members specified by all the mixin objects.
    //

    _Class.mix(ListView, _Utilities.createEventProperties(
        eventNames.render,
        eventNames.command,
        eventNames.currentchanged,
        eventNames.selectionchanged,
        eventNames.itemrender,
        eventNames.itemclick,
        eventNames.itemdrag,
        eventNames.itemdragstart,
        eventNames.itemdragend,
        eventNames.itemdragenter,
        eventNames.itemdragleave,
        eventNames.itemdragover
    ));
    _Class.mix(ListView, _Utilities.eventMixin);

    _KO.bindingHandlers.listViewGroup = {
        init: function (element, valueAccessor, allBindingsAcessor, viewModel, bindingContext) {
            /// <signature>
            /// <summary>This will be called when the binding is first applied to an element.
            /// Set up any initial state, event handlers, etc. here.</summary>
            /// </signature>
            var value = valueAccessor();
            var ctx = bindingContext.createChildContext(value.item);
            ctx.$root = value.list;
            ctx.$list = value.list;
            ctx.$listContext = value.listContext;
            ctx.$index = value.index;

            _KO.applyBindings(ctx, element);
            return { controlsDescendantBindings: true };
        }
    };

    _KO.bindingHandlers.listView = {
        init: function (element, valueAccessor, allBindingsAcessor, viewModel) {
            /// <signature>
            /// <summary>This will be called when the binding is first applied to an element.
            /// Set up any initial state, event handlers, etc. here.</summary>
            /// </signature>
            var options = _KO.unwrap(valueAccessor()) || {};
            options.context = viewModel; // The data context object.
            options.items = options.items || viewModel; // Use the viewModel (context data) as item source too.
            var listView = new ListView(element, options);
            _KO.utils.domNodeDisposal.addDisposeCallback(element, function () {
                if (listView) {
                    listView.dispose();
                    listView = null;
                }
            });
            return { controlsDescendantBindings: true };
        }
    };

    //
    // Public Namespaces & Classes
    //

    _WinJS.Namespace.defineWithParent(_WinJS, "UI", {
        SelectionMode: SelectionMode,
        ListView: ListView,
        ListViewColumn: ListViewColumn
    });

})(window, ko, WinJS);
