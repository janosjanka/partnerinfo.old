// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (_WinJS, _Promise, _Utilities, _Knockout, ko, undefined) {
    "use strict";

    var _Class = _WinJS.Class;

    var _observable = ko.observable;
    var _observableArray = ko.observableArray;
    var _pureComputed = ko.pureComputed;

    var eventNames = {
        itemschanged: "itemschanged",
        currentchanged: "currentchanged",
        selectionchanged: "selectionchanged"
    };

    function isFunction(fn) {
        /// <signature>
        /// <summary>Returns a value indicating whether the specified object is a function.</summary>
        /// <param name="fn" type="Object" />
        /// <returns type="Boolean" />
        /// </signature>
        return typeof fn === "function";
    }
    function isEditable(item) {
        /// <signature>
        /// <summary>Returns a value indicating whether the specified item is an editable object.</summary>
        /// <param name="item" type="Object" />
        /// <returns type="Boolean" />
        /// </signature>
        return item !== null
            && item !== undefined
            && isFunction(item.beginEdit)
            && isFunction(item.endEdit)
            && isFunction(item.cancelEdit);
    }
    function isValidable(item) {
        /// <signature>
        /// <summary>Returns a value indicating whether the specified item is a valid object.</summary>
        /// <param name="obj" type="Object" />
        /// <returns type="Boolean" />
        /// </signature>
        return item !== null
            && item !== undefined
            && isFunction(item.validate);
    }
    function asNumber(value) {
        /// <signature>
        /// <summary>Converts a value to valid number.</summary>
        /// <param name="value" type="Object" />
        /// <returns type="Number" />
        /// </signature>
        return value === undefined ? undefined : +value;
    }

    var List = _Class.define(function List_ctor(options) {
        /// <signature>
        /// <summary>Represents the list view for collections.</summary>
        /// <param name="options" type="Object" optional="true">A set of key/value pairs that can be used to configure list settings.
        /// <para>items: An array of items that will be mapped to a KO observable array.</para>
        /// <para>map: Mapping function</para>
        /// <para>ownsItems: Gets or sets whether objects in the list are owned by the list or not. If entries are owned, when an entry object is removed from the list, the entry object is disposed.</para>
        /// <para>oncurrentchanged: A function that can be called when the current property has changed.</para>
        /// </param>
        /// <returns type="WinJS.Knockout.List" />
        /// </signature>
        this.options = options = options || {};
        _Utilities.setOptions(this, options);
        _Promise.tasks(this);

        this._disposed = false;
        this._newItem = null;
        this._editItem = null;
        this._currentLast = null;
        this._refreshCycle = 0;
        this._items = _observableArray(options.items || []); // An array of items that can be tracked by KO binding engine

        this.ownsItems = options.ownsItems !== false; // true if all the deleted items should be disposed
        this.items = this._items.map(options.mapItem || this.mapItem.bind(this)); // https://github.com/SteveSanderson/knockout-projections
        this.selection = _observableArray(); // An array of selected items
        this.current = _observable(this._currentLast); // The current item that is synchronized with data controls using synchronize

        this._itemsChangedSn = this.items.subscribe(this.itemsChanged, this, "arrayChange");
        this._currentChangedSn = this.current.subscribe(this.currentChanged, this);
        this._selectionChangedSn = this.selection.subscribe(this.selectionChanged, this);
    }, {
        /// <field type="Number">
        /// Returns the length of the underlying array.
        /// </field>
        length: {
            enumerable: false,
            get: function () {
                return this.items().length;
            }
        },
        /// <field type="Boolean">
        /// Returns true if this list is empty.
        /// </field>
        isEmpty: {
            enumerable: true,
            get: function () {
                return this.length === 0;
            }
        },
        every: function (callback, thisArg) {
            /// <signature>
            /// <summary>Checks whether the specified callback function returns true for all items in a list.</summary>
            /// <param name="callback" type="Function">A function that accepts up to three arguments. This function is called for each item in the list until it returns false or the end of the list is reached.</param>
            /// <param name="thisArg" type="Object" optional="true">An object to which the this keyword can refer in the callback function. If thisArg is omitted, undefined is used.</param>
            /// <returns type="Boolean">True if the callback returns true for all items in the list.</returns>
            /// </signature>
            return this.peek().every(callback, thisArg);
        },
        filter: function (callback, thisArg) {
            /// <signature>
            /// <summary>Returns the items of a list that meet the condition specified in a callback function.</summary>
            /// <param name="callback" type="Function">A function that accepts up to three arguments. The function is called for each item in the list.</param>
            /// <param name="thisArg" type="Object" optional="true">An object to which the this keyword can refer in the callback function. If thisArg is omitted, undefined is used.</param>
            /// <returns type="Array">An array containing the items that meet the condition specified in the callback function.</returns>
            /// </signature>
            return this.peek().filter(callback, thisArg);
        },
        find: function (callback, thisArg) {
            /// <signature>
            /// <summary>Returns the item of a list that meet the condition specified in a callback function.</summary>
            /// <param name="callback" type="Function">A function that accepts up to three arguments. The function is called for each item in the list until it returns true, or until the end of the list.</param>
            /// <param name="thisArg" type="Object" optional="true">An object to which the this keyword can refer in the callback function. If thisArg is omitted, undefined is used.</param>
            /// <returns type="Object">The return value from the last call to the callback function.</returns>
            /// </signature>
            return this.peek().find(callback, thisArg);
        },
        forEach: function (callback, thisArg) {
            /// <signature>
            /// <summary>Calls the specified callback function for each item in a list.</summary>
            /// <param name="callback" type="Function">A function that accepts up to three arguments. The function is called for each item in the list.</param>
            /// <param name="thisArg" type="Object" optional="true">An object to which the this keyword can refer in the callback function. If thisArg is omitted, undefined is used.</param>
            /// </signature>
            this.peek().forEach(callback, thisArg);
        },
        getAt: function (index) {
            /// <signature>
            /// <summary>Returns the item at the specified index.</summary>
            /// <param name="index" type="Number" integer="true">The index of the value to get.</param>
            /// <returns type="Object" mayBeNull="true">The item at the specified index.</returns>
            /// </signature>
            index = asNumber(index);
            return this.peek()[index];
        },
        indexOf: function (searchItem, fromIndex) {
            /// <signature>
            /// <summary>Returns the first index at which a given item can be found in the array, or -1 if it is not present.</summary>
            /// <param name="searchItem" type="Object">Item to locate in the array.</param>
            /// <param name="fromIndex" type="Number">The index at which to begin the search. Default: 0 (Entire array is searched).</param>
            /// <returns type="Number" integer="true" />
            /// </signature>
            fromIndex = asNumber(fromIndex);
            return this.peek().indexOf(searchItem, fromIndex);
        },
        map: function (callback, thisArg) {
            /// <signature>
            /// <summary>Calls the specified callback function on each item of a list, and returns an array that contains the results.</summary>
            /// <param name="callback" type="Function">A function that accepts up to three arguments. The function is called for each item in the list.</param>
            /// <param name="thisArg" type="Object" optional="true">An object to which the this keyword can refer in the callback function. If thisArg is omitted, undefined is used.</param>
            /// <returns type="Array">An array containing the result of calling the callback function on each item in the list.</returns>
            /// </signature>
            return this.peek().map(callback, thisArg);
        },
        peek: function () {
            /// <signature>
            /// <summary>Returns the underlying array of the observableArray.</summary>
            /// <returns type="Array" />
            /// </signature>
            return this.items.peek();
        },
        some: function (callback, thisArg) {
            /// <signature>
            /// <summary>Checks whether the specified callback function returns true for any item of a list.</summary>
            /// <param name="callback" type="Function">A function that accepts up to three arguments. The function is called for each item in the list until it returns true, or until the end of the list.</param>
            /// <param name="thisArg" type="Object" optional="true">An object to which the this keyword can refer in the callback function. If thisArg is omitted, undefined is used.</param>
            /// <returns type="Boolean">True if callback returns true for any item in the list.</returns>
            /// </signature>
            return this.peek().some(callback, thisArg);
        },
        reduce: function (callback, initialValue) {
            /// <signature>
            /// <summary>Accumulates a single result by calling the specified callback function for all items in a list. The return value of the callback function is the accumulated result, and is provided as an argument in the next call to the callback function.</summary>
            /// <param name="callback" type="Function">A function that accepts up to four arguments. The function is called for each item in the list.</param>
            /// <param name="initialValue" type="Object" optional="true">If initialValue is specified, it is used as the value with which to start the accumulation. The first call to the function provides this value as an argument instead of a list value.</param>
            /// <returns type="Object">The return value from the last call to the callback function.</returns>
            /// </signature>
            if (arguments.length > 1) {
                return this.peek().reduce(callback, initialValue);
            }
            return this.peek().reduce(callback);
        },
        reduceRight: function (callback, initialValue) {
            /// <signature>
            /// <summary>Accumulates a single result by calling the specified callback function for all items in a list, in descending order. The return value of the callback function is the accumulated result, and is provided as an argument in the next call to the callback function.</summary>
            /// <param name="callback" type="Function">A function that accepts up to four arguments. The function is called for each item in the list.</param>
            /// <param name="initialValue" type="Object" optional="true">If initialValue is specified, it is used as the value with which to start the accumulation. The first call to the callbackfn function provides this value as an argument instead of a list value.</param>
            /// <returns type="Object">The return value from last call to callback function.</returns>
            /// </signature>
            if (arguments.length > 1) {
                return this.peek().reduceRight(callback, initialValue);
            }
            return this.peek().reduceRight(callback);
        },

        //
        // List mutation functions
        //

        move: function (index, newIndex) {
            /// <signature>
            /// <summary>Moves an item to a new position.</summary>
            /// <param name="index" type="Number">The current position of the item in the list.</param>
            /// <param name="newIndex" type="Number">The new position of the item in the list.</param>
            /// <returns type="Number" />
            /// </signature>
            var _items = this._items.peek();
            var lastIndex = _items.length - 1;
            index = asNumber(index);
            index = index < 0 ? 0 : index > lastIndex ? lastIndex : index;
            newIndex = asNumber(newIndex);
            newIndex = newIndex < 0 ? 0 : newIndex > lastIndex ? lastIndex : newIndex;
            if (index !== newIndex) {
                var current = this.current();
                var selection = this.selection().slice(0);
                this._items.valueWillMutate();
                _items.splice(newIndex, 0, _items.splice(index, 1)[0]);
                this._items.valueHasMutated();
                this.moveTo(current);
                this.selection(selection);
            }
            return newIndex;
        },
        pop: function () {
            /// <signature>
            /// <summary>
            /// Removes the last element from a list and returns it.
            /// </summary>
            /// <returns type="Object">Last element from the list.</returns>
            /// </signature>
            return this._items.pop.apply(this._items, arguments);
        },
        push: function () {
            /// <signature>
            /// <summary>
            /// Appends new element(s) to a list, and returns the new length of the list.
            /// </summary>
            /// <param name="value" type="Object" parameterArray="true">The element to insert at the end of the list.</param>
            /// <returns type="Number" integer="true">The new length of the list.</returns>
            /// </signature>
            return this._items.push.apply(this._items, arguments);
        },
        remove: function (itemOrPredicate) {
            /// <signature>
            /// <summary>Removes the first occurrence of a specific item from the list without saving the pending added/edited item.</summary>
            /// <param name="item" type="Object" />
            /// <returns type="Array">The removed item(s).</returns>
            /// </signature>
            /// <signature>
            /// <summary>Removes all the items that match the conditions defined by the specified predicate without saving the pending added/edited item.</summary>
            /// <param name="predicate" type="Function" />
            /// <returns type="Array">The removed item(s).</returns>
            /// </signature>
            var isPredicate = isFunction(itemOrPredicate) && !ko.isObservable(itemOrPredicate);
            var _items = this._items.peek();
            var items = this.peek();
            var removed = [];
            var valueWillMutate = false;
            for (var i = items.length; --i >= 0;) {
                var item = items[i];
                var remove = isPredicate ? itemOrPredicate(item) : item === itemOrPredicate;
                if (remove) {
                    if (!valueWillMutate) {
                        this._items.valueWillMutate();
                        valueWillMutate = true;
                    }
                    removed.push(item);
                    _items.splice(i, 1);
                }
            }
            if (valueWillMutate) {
                this._items.valueHasMutated();
            }
            return removed;
        },
        removeAll: function () {
            /// <signature>
            /// <summary>Removes all items from the list without saving the pending added/edited item.</summary>
            /// <returns type="Array">The deleted elements.</returns>
            /// </signature>
            return this._items.removeAll();
        },
        removeAt: function (index) {
            /// <signature>
            /// <summary>Removes an item at the given index without saving the pending added/edited item.</summary>
            /// <param name="index" type="Number">The index of the item to remove.</param>
            /// <returns type="Array">The deleted elements.</returns>
            /// </signature>
            index = asNumber(index);
            return this.splice(index, 1);
        },
        replace: function (oldItem, newItem) {
            /// <signature>
            /// <summary>Replaces the oldItem with the newItem, and returns the index of the new item without saving the pending added/edited item.</summary>
            /// <param name="oldItem" type="Object">The old item.</param>
            /// <param name="newItem" type="Object">The new item.</param>
            /// <returns type="Number" integer="true">The index of the newItem.</returns>
            /// </signature>
            var index = this.indexOf(oldItem);
            if (index >= 0) {
                this._items.valueWillMutate();
                this._items.peek()[index] = newItem;
                this._items.valueHasMutated();
            }
            return index;
        },
        replaceAll: function () {
            /// <signature>
            /// <summary>Sets the specified array as the items of the list without saving the pending added/edited item.</summary>
            /// <param name="value" type="Object" parameterArray="true">The element to insert at the end of the list.</param>
            /// <returns type="Number" integer="true">The new length of the list.</returns>
            /// </signature>
            this.removeAll();
            return this.push.apply(this, arguments);
        },
        shift: function () {
            /// <signature>
            /// <summary>
            /// Removes the first element from a list and returns it.
            /// </summary>
            /// <returns type="Object">First element from the list.</returns>
            /// </signature>
            return this._items.shift.apply(this._items, arguments);
        },
        unshift: function () {
            /// <signature>
            /// <summary>
            /// Appends new element(s) to a list, and returns the new length of the list.
            /// </summary>
            /// <param name="value" type="Object" parameterArray="true">The element to insert at the start of the list.</param>
            /// <returns type="Number" integer="true">The new length of the list.</returns>
            /// </signature>
            return this._items.unshift.apply(this._items, arguments);
        },
        splice: function () {
            /// <signature>
            /// <summary>
            /// Removes elements from a list and, if necessary, inserts new elements in their place, returning the deleted elements.
            /// </summary>
            /// <param name="start" type="Number" integer="true">The zero-based location in the list from which to start removing elements.</param>
            /// <param name="deleteCount" type="Number" integer="true">The number of elements to remove.</param>
            /// <param name="item" type="Object" optional="true" parameterArray="true">The elements to insert into the list in place of the deleted elements.</param>
            /// <returns type="Array">The deleted elements.</returns>
            /// </signature>
            return this._items.splice.apply(this._items, arguments);
        },
        sort: function () {
            /// <signature>
            /// <summary>
            /// Sorts the elements of an array in place and returns the array.
            /// </summary>
            /// <param name="compareFunction" type="Function" optional="true">Specifies a function that defines the sort order. If omitted, the array is sorted according to each character's Unicode code point value, according to the string conversion of each element.</param>
            /// <returns type="Array">The array.</returns>
            /// </signature>
            return this._items.sort.apply(this._items, arguments);
        },

        //
        // Commanding
        //

        command: function (name) {
            /// <signature>
            /// <param name="name" type="String" />
            /// <param name="params" type="Array" optional="true" />
            /// </signature>
            var commands = this.commands;
            var commandFn = commands && commands[name];
            var commandParams = Array.prototype.slice.call(arguments, 1);
            return function () {
                if (commandFn) {
                    commandFn.apply(commands, commandParams);
                }
            };
        },

        //
        // List mutation functions for editing items
        //

        /// <field type="Boolean">
        /// Gets a value that indicates whether an add transaction is in progress.
        /// </field>
        isAddingNew: {
            enumerable: true,
            get: function () {
                return this._newItem !== null && this._newItem !== undefined;
            }
        },
        /// <field type="Boolean">
        /// Returns true if an edit transaction is in progress.
        /// </field>
        isEditingItem: {
            enumerable: true,
            get: function () {
                return this._editItem !== null && this._editItem !== undefined;
            }
        },
        addNew: function () {
            /// <signature>
            /// <summary>Adds the specified object to the list. Calling the addNew method starts an add transaction. You should call the commitNew or cancelNew methods to end the add transaction.</summary>
            /// <returns type="Object" />
            /// </signature>
            return this.addNewItem();
        },
        addNewItem: function (item) {
            /// <signature>
            /// <summary>Adds the specified object to the list. Calling the addNewItem method starts an add transaction. You should call the commitNew or cancelNew methods to end the add transaction.</summary>
            /// <param name="item" type="Object" />
            /// </signature>
            this.commitEdit();
            if (this.isEditingItem) {
                return;
            }
            this.commitNew();
            if (this.isAddingNew) {
                return;
            }
            this._newItem = this.getAt(this.push(item) - 1);
            this.moveTo(this._newItem);
            if (isEditable(this._newItem)) {
                this._newItem.beginEdit();
            }
            return this._newItem;
        },
        editItem: function (item) {
            /// <signature>
            /// <summary>Begins an edit transaction of the specified item.</summary>
            /// <param name="item" type="Object" />
            /// </signature>
            if (item === this._newItem) {
                return;
            }
            this.commitNew();
            if (this.isAddingNew) {
                return;
            }
            this.commitEdit();
            if (this.isEditingItem) {
                return;
            }
            this._editItem = item;
            this.moveTo(this._editItem);
            if (isEditable(this._editItem)) {
                this._editItem.beginEdit();
            }
        },
        commitNew: function () {
            /// <signature>
            /// <summary>Ends the add transaction and saves the pending new item.</summary>
            /// </signature>
            if (!this.isAddingNew || this.isEditingItem) {
                return;
            }
            var newItem = this._newItem;
            if (isValidable(newItem) && !newItem.validate()) {
                return;
            }
            if (isEditable(newItem)) {
                newItem.endEdit();
            }
            this._newItem = null;
        },
        commitEdit: function () {
            /// <signature>
            /// <summary>Ends the edit transaction and saves the pending changes.</summary>
            /// </signature>
            if (!this.isEditingItem || this.isAddingNew) {
                return;
            }
            var editItem = this._editItem;
            if (isValidable(editItem) && !editItem.validate()) {
                return;
            }
            if (isEditable(editItem)) {
                editItem.endEdit();
            }
            this._editItem = null;
        },
        cancelNew: function () {
            /// <signature>
            /// <summary>Ends the add transaction and saves the pending new item.</summary>
            /// </signature>
            if (!this.isAddingNew || this.isEditingItem) {
                return;
            }
            this.remove(this._newItem);
        },
        cancelEdit: function () {
            /// <signature>
            /// <summary>Ends the edit transaction, and if possible, restores the original value to the item.</summary>
            /// </signature>
            if (!this.isEditingItem || this.isAddingNew) {
                return;
            }
            var editItem = this._editItem;
            this._editItem = null;
            if (isEditable(editItem)) {
                editItem.cancelEdit();
            }
        },
        commitAll: function () {
            /// <signature>
            /// <summary>Ends the edit transaction and saves the pending changes.</summary>
            /// </signature>
            this.isAddingNew && this.commitNew();
            this.isEditingItem && this.commitEdit();
        },
        cancelAll: function () {
            /// <signature>
            /// <summary>Ends the edit transaction, and if possible, restores the original value to the item.</summary>
            /// </signature>
            this.isAddingNew && this.cancelNew();
            this.isEditingItem && this.cancelEdit();
        },

        //
        // List selection
        //

        select: function (itemOrPredicate) {
            /// <signature>
            /// <summary>Selects the given item.</summary>
            /// <param name="item" type="Object" />
            /// <returns type="Array">The selected items.</returns>
            /// </signature>
            /// <signature>
            /// <summary>Selects all the items that match the conditions defined by the specified predicate.</summary>
            /// <param name="predicate" type="Function" />
            /// <returns type="Array">The selected items.</returns>
            /// </signature>
            var selection = [];
            var items = this.peek();
            var isPredicate = isFunction(itemOrPredicate) && !ko.isObservable(itemOrPredicate);
            for (var i = 0, len = items.length; i < len; ++i) {
                var item = items[i];
                var add = isPredicate ? itemOrPredicate(item) : item === itemOrPredicate;
                if (add && this.selection.indexOf(item) === -1) {
                    selection.push(item);
                }
            }
            if (selection.length) {
                this.selection.push.apply(this.selection, selection);
            }
            return selection;
        },
        selectRange: function (from, to) {
            /// <signature>
            /// <summary>Adds the specified range of items to the selection list.</summary>
            /// <param name="from" type="Number" integer="true">A number that specifies the first item.</param>
            /// <param name="to" type="Number" integer="true">A number that specifies the last item.</param>
            /// <returns type="Array">The selected elements.</returns>
            /// </signature>
            var items = this.peek();
            var len = items.length;
            from = asNumber(from);
            to = asNumber(to);
            if (from > to) {
                var tmp = from;
                from = to;
                to = tmp;
            }
            if (from < 0) {
                from = 0;
            }
            if (to >= len) {
                to = len - 1;
            }
            var selected = new Array(to - from);
            for (var i = 0; from <= to; ++from) {
                var item = items[from];
                selected[i++] = item;
                this.select(item);
            }
            return selected;
        },
        selectAll: function () {
            /// <signature>
            /// <summary>Selects all the items in the list.</summary>
            /// <returns type="Array">The selected elements.</returns>
            /// </signature>
            var selection = Array.prototype.slice.apply(this.items.peek(), [0]);
            this.selection(selection);
            return selection;
        },
        deselect: function (item) {
            /// <signature>
            /// <summary>Deselects an item in the list.</summary>
            /// <param name="item" type="Function">The item to deselect.</param>
            /// </signature>
            this.selection.remove(item);
        },
        deselectAll: function () {
            /// <signature>
            /// <summary>Deselects all the items in the list.</summary>
            /// </signature>
            this.selection.removeAll();
        },

        //
        // List cursor
        //

        moveTo: function (item) {
            /// <signature>
            /// <summary>Sets the specified item to be the current in the view.</summary>
            /// <param name="item" type="Object">The item to set as the current.</param>
            /// <returns type="Boolean">true if the resulting current is an item within the view; otherwise, false.</returns>
            /// </signature>
            if (item === undefined) {
                item = null;
            }
            if (item === null || this.indexOf(item) >= 0) {
                this.current(item);
                return true;
            }
            return false;
        },
        moveToIndex: function (index) {
            /// <signature>
            /// <summary>Sets the item at the specified index to be the current in the view.</summary>
            /// <param name="index" type="Number">The index to set the current to.</param>
            /// <returns type="Boolean">true if the resulting current is an item within the view; otherwise, false.</returns>
            /// </signature>
            index = asNumber(index);
            var items = this.peek();
            return (index >= 0) && (index < items.length) && this.moveTo(items[index]);
        },
        moveToFirst: function () {
            /// <signature>
            /// <summary>Sets the first item in the view as the current.</summary>
            /// <returns type="Boolean">true if the resulting current is an item within the view; otherwise, false.</returns>
            /// </signature>
            return this.moveToIndex(0);
        },
        moveToPrev: function () {
            /// <signature>
            /// <summary>Sets the item before the current in the view as the current.</summary>
            /// <returns type="Boolean">true if the resulting current is an item within the view; otherwise, false.</returns>
            /// </signature>
            var items = this.peek();
            var index = items.indexOf(this.current()) - 1;
            return this.moveToIndex((index < 0) ? (items.length - 1) : index);
        },
        moveToLast: function () {
            /// <signature>
            /// <summary>Sets the last item in the view as the current.</summary>
            /// <returns type="Boolean">true if the resulting current is an item within the view; otherwise, false.</returns>
            /// </signature>
            var items = this.peek();
            return this.moveTo(items[items.length - 1]);
        },
        moveToNext: function () {
            /// <signature>
            /// <summary>Sets the item after the current in the view as the current.</summary>
            /// <returns type="Boolean">true if the resulting current is an item within the view; otherwise, false.</returns>
            /// </signature>
            var items = this.peek();
            var index = items.indexOf(this.current()) + 1;
            return this.moveToIndex((index < items.length) ? index : 0);
        },

        //
        // List refresh
        //

        refresh: function () {
            /// <signature>
            /// <summary>Re-creates the view.</summary>
            /// </signature>
        },
        deferRefresh: function (callback, thisArg) {
            /// <signature>
            /// <summary>Enters a defer cycle that you can use to merge changes to the view and delay automatic refresh.</summary>
            /// <param name="callback" type="Function">A callback function that can be called when enters a defer cycle. 
            /// The callback function will be executed in the context of the filter object.
            /// <para>http://referencesource.microsoft.com/#PresentationFramework/Framework/System/Windows/Data/CollectionViewSource.cs,735bcfd75952a04b</para>
            /// </param>
            /// <param name="thisArg" type="Object" optional="true">Object to use as this when executing callback.</param>
            /// </signature>
            this._refreshCycle += 1;
            try {
                callback.call(thisArg);
            } finally {
                this._refreshCycle -= 1;
                if (this._refreshCycle <= 0) {
                    this.refresh();
                }
            }
        },

        //
        // List serialization
        //

        mapItem: function (item) {
            /// <signature>
            /// <summary>Represents a function called before adding a new item to the list.</summary>
            /// <param name="item" type="Object" />
            /// <returns type="Object" />
            /// </signature>
            return item;
        },
        toObject: function (options) {
            /// <signature>
            /// <summary>Serializes the list into a native JavaScript object.</summary>
            /// <param name="options" type="Object" optional="true">A set of key/value pairs that can be used to configure knockout.js mapping plugin.</param>
            /// <returns type="Object" />
            /// </signature>
            return this.items().map(function (item) {
                if (item === null || item === undefined) {
                    return item;
                }
                return isFunction(item.toObject) ? item.toObject() : item;
            });
        },

        //
        // Event handlers
        //

        itemsChanged: function (entries) {
            /// <signature>
            /// <param name="entries" type="Array" />
            /// </signature>
            var current = this.current();
            var removed = [];
            for (var entry, i = 0, len = entries.length; i < len; ++i) {
                if ((entry = entries[i]).status === "deleted") {
                    // The same entry can be presented in the list with a different status,
                    // so we need to check the last status of the entry too.
                    var deleted = true;
                    for (var j = len; --j > i;) {
                        var lastItem = entries[j];
                        if (lastItem.index === entry.index && lastItem.status === "added") {
                            deleted = false;
                            break;
                        }
                    }
                    if (deleted) {
                        removed.push(entry.value);
                        if (this._newItem === entry.value) {
                            this._newItem = null;
                        }
                        if (this._editItem === entry.value) {
                            this._editItem = null;
                        }
                        if (current === entry.value) {
                            this.current(null);
                        }
                        if (this.ownsItems && entry.value && isFunction(entry.value.dispose)) {
                            entry.value.dispose();
                        }
                    }
                }
            }
            if (removed.length) {
                this.selection.removeAll(removed);
            }
            this.dispatchEvent(eventNames.itemschanged, entries);
        },
        currentChanged: function (current) {
            /// <signature>
            /// <summary>Fires an oncurrentchanged event.</summary>
            /// <param name="current" type="Object" />
            /// </signature>
            if (this._currentLast !== current) {
                this.dispatchEvent(eventNames.currentchanged, { oldItem: this._currentLast, newItem: current });
            }
            this._currentLast = current;
        },
        selectionChanged: function (selection) {
            /// <signature>
            /// <summary>Raises an onselectionchanged event.</summary>
            /// <param name="selection" type="Array" />
            /// </signature>
            this.dispatchEvent(eventNames.selectionchanged, selection);
        },

        //
        // Disposable
        //

        dispose: function () {
            /// <signature>
            /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            /// </signature>
            if (this._disposed) {
                return;
            }
            this.removeAll();
            this._itemsChangedSn && this._itemsChangedSn.dispose();
            this._itemsChangedSn = null;
            this._currentChangedSn && this._currentChangedSn.dispose();
            this._currentChangedSn = null;
            this._selectionChangedSn && this._selectionChangedSn.dispose();
            this._selectionChangedSn = null;
            this.items && this.items.dispose && this.items.dispose();
            this.items = null;
            this._disposed = true;
        }
    });

    var PagedList = _Class.derive(List, function PagedList_ctor(options) {
        /// <signature>
        /// <summary>Represents a view for sorting, filtering, and navigating a paged data list.</summary>
        /// <param name="options" type="Object" optional="true">A set of key/value pairs that can be used to configure paged list settings.
        /// <para>items: An array of items that will be mapped to a KO observable array.</para>
        /// <para>ownsItems: Gets or sets whether objects in the list are owned by the list or not. If entries are owned, when an entry object is removed from the list, the entry object is disposed.</para>
        /// <para>pageIndex: Gets or sets the index of the currently displayed page.</para>
        /// <para>pageSize: Gets or sets the number of records to display on a page.</para>
        /// <para>pageRateLimit: Delay time in millisecs.</para>
        /// <para>oncurrentchanged: A function that can be called when the current property has changed.</para>
        /// </param>
        /// <returns type="WinJS.Knockout.PagedList" />
        /// </signature>
        options = options || {};

        this._disposed = false;
        this._pagingLocked = false;
        this._lastPageIndex = 1;

        this._pageIndex = _observable(asNumber(options.pageIndex) || 1);
        this._pageSize = _observable(asNumber(options.pageSize) || 8);
        this._pageCount = _observable(1);
        this._total = _observable(0);
        this._rowStart = _observable(0);
        this._rowEnd = _observable(0);

        List.call(this, options);

        this._pageChangedSn = ko.computed(this._pageChanged, this).extend({
            rateLimit: {
                method: "notifyWhenChangesStop",
                timeout: asNumber(options.pageRateLimit) || 600
            }
        });
        this._totalChangedSn = this._total.subscribe(this._totalChanged, this);
    }, {
        /// <field type="Boolean">
        /// The index of the first row of results to return.
        /// </field>
        hasPrevPage: {
            enumerable: true,
            get: function () {
                return this._pageIndex() > 1;
            }
        },
        /// <field type="Boolean">
        /// The index of the first row of results to return.
        /// </field>
        hasNextPage: {
            enumerable: true,
            get: function () {
                return this._pageIndex() < this._pageCount();
            }
        },
        /// <field type="Number" integer="true">
        /// The index of the page of results to return. Use 1 to indicate the first page.
        /// </field>
        pageIndex: {
            enumerable: true,
            get: function () {
                return this._pageIndex();
            },
            set: function (value) {
                this._pageIndex(value);
            }
        },
        /// <field type="Number" integer="true">
        /// The size of the page of results to return.
        /// </field>
        pageSize: {
            enumerable: true,
            get: function () {
                return this._pageSize();
            },
            set: function (value) {
                this._pageSize(value);
            }
        },
        /// <field type="Number" integer="true">
        /// The total number of pages before paging is applied.
        /// </field>
        pageCount: {
            enumerable: true,
            get: function () {
                return this._pageCount();
            }
        },
        /// <field type="Number" integer="true">
        /// The total number of results before paging is applied.
        /// </field>
        total: {
            enumerable: true,
            get: function () {
                return this._total();
            },
            set: function (value) {
                this._total(value);
            }
        },
        /// <field type="Number" integer="true">
        /// The index of the first row of results to return.
        /// </field>
        rowStart: {
            enumerable: true,
            get: function () {
                return this._rowStart();
            }
        },
        /// <field type="Number" integer="true">
        /// The index of the last row of results to return.
        /// </field>
        rowEnd: {
            enumerable: true,
            get: function () {
                return this._rowEnd();
            }
        },
        _totalChanged: function (total) {
            /// <signature>
            /// <summary>Fires when the number of items has been changed.</summary>
            /// <param name="total" type="Number" />
            /// </signature>
            if (total <= 0) {
                return;
            }
            this._pagingLocked = true;
            var pageSize = this._pageSize();
            this._pageIndex(1);
            this._pageCount(pageSize < 1 || total < 1 ? 0 : Math.floor((total + pageSize - 1) / pageSize));
            this._computeRows(1, pageSize, total);
            this._pagingLocked = false;
        },
        _pageChanged: function () {
            /// <signature>
            /// <summary>Fires when one of page values changes.</summary>
            /// </signature>
            if (this._pagingLocked) {
                return;
            }
            var pageIndex = this._pageIndex();
            var pageSize = this._pageSize();
            var pageCount = this._pageCount();
            if (this._lastPageIndex === pageIndex || pageSize <= 0 || pageCount <= 0) {
                return;
            }
            if (pageIndex < 1) {
                this._pageIndex(1);
                return;
            }
            if (pageIndex > pageCount) {
                this._pageIndex(pageCount);
                return;
            }
            this._lastPageIndex = pageIndex;
            this._computeRows(pageIndex, pageSize, this._total.peek());
            this.refresh();
        },
        _computeRows: function (pageIndex, pageSize, total) {
            /// <signature>
            /// <param name="pageIndex" type="Number" />
            /// <param name="pageSize" type="Number" />
            /// <param name="total" type="Number" />
            /// </signature>
            var rowStart = (pageIndex - 1) * pageSize;
            if (rowStart + 1 > total) {
                rowStart = total - 1;
            }
            var rowEnd = rowStart + pageSize;
            if (rowEnd > total) {
                rowEnd = total;
            }
            this._rowStart(rowStart + 1);
            this._rowEnd(rowEnd);
        },
        dispose: function () {
            /// <signature>
            /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            /// </signature>
            if (this._disposed) {
                return;
            }
            this._pageChangedSn && this._pageChangedSn.dispose();
            this._pageChangedSn = null;
            this._totalChangedSn && this._totalChangedSn.dispose();
            this._totalChangedSn = null;
            List.prototype.dispose.call(this);
            this._disposed = true;
        }
    });

    //
    // Defines a class using the given constructor and the union of the set of instance members specified by all the mixin objects.
    //

    _Class.mix(List, _Utilities.eventMixin);
    _Class.mix(List, _Utilities.createEventProperties(
        eventNames.currentchanged,
        eventNames.currentchanged,
        eventNames.selectionchanged
    ));

    _WinJS.Namespace.defineWithParent(_WinJS, "Knockout", {
        List: List,
        PagedList: PagedList
    });

})(WinJS, WinJS.Promise, WinJS.Utilities, WinJS.Knockout, window.ko);
