// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (_WinJS, _PI) {
    "use strict";

    var _Class = _WinJS.Class;
    var _Utilities = _WinJS.Utilities;
    var _Promise = _WinJS.Promise;
    var _Knockout = _WinJS.Knockout;
    var _PagedList = _WinJS.Knockout.PagedList;

    var options = {
        pageSize: 50
    },

    EntityState = {
        unchanged: 0,
        loaded: 1,
        created: 2,
        updated: 3,
        deleted: 4
    },

    EntityCollectionState = {
        closed: 0,
        loading: 1,
        loaded: 2
    },

    EntityErrorCode = {
        notSupported: 0,
        busy: 1,
        validationError: 2,
        exists: 3,
        deleted: 4
    };

    //
    // EntityMixin provides members for performing tasks asynchronously.
    // A class can mix these members with its own members, making those own.
    //

    var EntityMixin = {
        _taskCount: 0,
        startTask: function (callback) {
            /// <signature>
            /// <summary>Starts a new task.</summary>
            /// <param name="callback" type="Function">A function that accepts an argument.</param>
            /// <returns type="$.Deferred" />
            /// </signature>
            var deferred = $.Deferred();
            //this.busy(true);
            ++this._taskCount;
            _Utilities.async(function startTask_core() {
                var that = this;
                try {
                    callback.apply(this, [deferred]);
                    deferred.always(function startTask_complete() {
                        if (--that._taskCount <= 0) {
                            //  that.busy(false);
                            that._taskCount = 0;
                        }
                    });
                } catch (ex) {
                    if (--this._taskCount <= 0) {
                        //this.busy(false);
                        this._taskCount = 0;
                    }
                    deferred.rejectWith(this, [ex]);
                }
            }, this, 120);
            _WinJS.DEBUG && deferred.fail(function (ex) {
                _WinJS.log({
                    type: ex ? "error" : "warn",
                    category: "EntityMixin.startTask",
                    message: ex ? ex + "\n" + callback : "rejected"
                });
            });
            return deferred.promise();
        }
    },

    Entity = _Class.derive(_Knockout.Editable, function Entity_ctor(options) {
        /// <signature>
        /// <summary>Initializes a new instance of the Entity class.</summary>
        /// <param name="options" type="Object" optional="true">A set of key/value pairs that can be used to configure entity operations.
        /// <para>autoReload: Reloads the entity from the database overwriting any property values with values from the database.</para>
        /// <para>item: The original entity object to wrap.</para>
        /// <para>mapping: A set of key/value pairs that can be used to configure KO mapping settings.</para>
        /// <para>key: The name of the entity key property. Default: id</para>
        /// <para>urls: The resource URLs for CRUD { query, create, update, remove }.</para>
        /// <para>onloaded: A function that can be called when the entity is loaded.</para>
        /// <para>oncreated: A function that can be called when the entity is created.</para>
        /// <para>onupdated: A function that can be called when the entity is updated.</para>
        /// <para>onremoved: A function that can be called when the entity is removed.</para>
        /// <para>oncanceled: A function that can be called when the entity is canceled.</para>
        /// </param>
        /// <returns type="Entity" />
        /// </signature>
        options || (options = {});
        options.autoReload = options.autoReload || false; // Reloads the entity from the database overwriting property values with values from the database.
        options.key = options.key || "id"; // Entity key property.
        this.state = ko.observable(EntityState.unchanged);
        _Knockout.Editable.call(this, options);

        options.autoReload && this.reload();
    }, {
        getAuthToken: function () {
            /// <signature>
            /// <summary>Gets an auth token.</summary>
            /// <returns type="String" />
            /// </signature>
            return this.options.authToken;
        },
        descriptorFor: function (key) {
            /// <signature>
            /// <summary>Gets a descriptor object that represents information on the current entity.</summary>
            /// <param name="key" type="String">The unique identifier for the pre-defined resource URL.</param>
            /// <returns type="Object" />
            /// </signature>
            if (!key) throw new Error("You must specify a key.");
            var path = this.options.urls && this.options.urls[key];
            var data = null;
            if (path) {
                var self = this;
                // Replace URL placeholders in the path.
                data = this.toObject();
                path = path.replace(/\{(\S*?)\}/gi, function (match) {
                    var v = self.resolvePath(key, match.substr(1, match.length - 2), data);
                    if (v === undefined) return match;
                    if (v === null) return "";
                    return v;
                });
                // The HTTP GET and DELETE use URL route parameters, not HTTP body.
                if ((key === "query") || (key === "remove")) data = null;
            }
            return {
                method: null, // The HTTP method ( GET | POST | PUT | DELETE )
                path: path, // The address of the resource on the Web
                data: data // The native (deserialized) entity object
            };
        },
        resolvePath: function (key, member, data) {
            /// <signature>
            /// <summary>Resolves the current URL pattern.</summary>
            /// <param name="key" type="String">The unique identifier for the pre-defined resource URL.</param>
            /// <param name="member" type="String">The URL part to match.</param>
            /// <param name="data" type="Object">The deserialized entity data.</param>
            /// <returns type="Object" />
            /// </signature>
            return data[member];
        },
        setItemAs: function (item, state) {
            /// <signature>
            /// <summary>Set the specified item in the specified state.</summary>
            /// <param name="item" type="Object">The native JS item to set.</param>
            /// <param name="state" type="EntityState">A value of EntityState enumeration values.</param>
            /// </signature>
            if (item !== undefined) {
                this.setItem(item);
            }
            if (state !== undefined) {
                this.state(state);
            }
            this.clearErrors();
        },
        setErrors: function (members) {
            /// <signature>
            /// <summary>Removes all error messages, then adds a new array of error messages to the collection.</summary>
            /// <param name="errors" type="Array" optional="true" value="[]">The error messages to add.</param>
            /// </signature>
            this.clearErrors();
            if (members) {
                for (var i = 0, len = members.length; i < len; ++i) {
                    this.addError(members[i].message);
                }
            }
        },
        reload: function (options) {
            /// <signature>
            /// <summary>Reloads the current entity using the specified settings.</summary>
            /// <param name="options" type="Object" optional="true">A set of key/value pairs that can be used to configure the HTTP request.</param>
            /// </signature>
            if (this.state() === EntityState.deleted) {
                return _Promise.error(EntityErrorCode.deleted);
            }
            return this.startTask(function reloadAction(deferred) {
                var path;
                var data;
                // Use the specified settings object to configure the HTTP request instead of the built-in descriptor mechanism.
                if (options) {
                    path = options.path; // Request URI
                    data = options.data; // Request data
                } else {
                    // Get the descriptor object for the specified operation.
                    var descriptor = this.descriptorFor("query");
                    if (!descriptor.path) {
                        deferred.rejectWith(this, [EntityErrorCode.notSupported]);
                        return;
                    }
                    path = descriptor.path;
                    data = descriptor.data;
                }
                if (!path) {
                    deferred.rejectWith(this, ["path"]);
                    return;
                }
                _PI.api({
                    auth: this.getAuthToken(),
                    path: path,
                    data: data
                }, this).then(
                    function (data) { this.afterLoaded(deferred, data); },
                    function (error) {
                        this.setErrors(error.members);
                        deferred.rejectWith(this, []);
                    }
                );
            });
        },
        afterLoaded: function (deferred, data) {
            /// <signature>
            /// <summary>Occurs when a new data has created. This function resolves the deferred object and calls the corresponding event handlers.</summary>
            /// <param name="deferred" type="$.Deferred">The deferred object that can be used to chain more operations.</param>
            /// <param name="data" type="Object">The data.</param>            
            /// </signature>
            this.setItemAs(data, EntityState.loaded);
            // Resolve the deferred object.
            deferred.resolveWith(this, [data]);
            // Raise an onloaded event.
            this.dispatchEvent("loaded");
        },
        create: function (commit) {
            /// <signature>
            /// <summary>Creates a new entity independently of the status of the current entity.</summary>
            /// <param name="commit" type="Boolean" optional="true" value="true">Commits the current transaction after saving data.</param>
            /// <returns type="$.Deferred" />
            /// </signature>
            var descriptor = this.descriptorFor("create");
            if (!descriptor.path) {
                return _Promise.error(EntityErrorCode.notSupported);
            }
            if (this.state() !== EntityState.unchanged) {
                return _Promise.error(EntityErrorCode.exists);
            }
            if (!this.validate()) {
                return _Promise.error(EntityErrorCode.validationError);
            }
            return this.startTask(function createAction(deferred) {
                _PI.api({
                    method: "post",
                    auth: this.getAuthToken(),
                    path: descriptor.path,
                    data: descriptor.data
                }, this).then(
                    function (data) {
                        this.afterCreated(deferred, data, commit === undefined || commit === true);
                    },
                    function (error) {
                        this.setErrors(error.members);
                        deferred.rejectWith(this, []);
                    });
            });
        },
        afterCreated: function (deferred, data, commit) {
            /// <signature>
            /// <summary>Occurs when a new data has created. This function resolves the deferred object and calls the corresponding event handlers.</summary>
            /// <param name="deferred" type="$.Deferred">The deferred object that can be used to chain more operations.</param>
            /// <param name="data" type="Object">The data.</param>            
            /// <param name="commit" type="Boolean">Commits the current transaction after saving data.</param>
            /// </signature>
            this.setItemAs(data, EntityState.created);
            // If the validation was successful, commit the current edit session.
            commit && this.endEdit();
            // Resolve the deferred object.
            deferred.resolveWith(this, [data]);
            // Raise an oncreated event.
            this.dispatchEvent("created");
        },
        update: function (commit) {
            /// <signature>
            /// <summary>Updates an existing entity independently of the status of the current entity.</summary>
            /// <param name="commit" type="Boolean" optional="true" value="true">Commits the current transaction after saving data.</param>
            /// <returns type="$.Deferred" />
            /// </signature>
            var descriptor = this.descriptorFor("update");
            if (!descriptor.path) {
                return _Promise.error(EntityErrorCode.notSupported);
            }
            if (this.state() === EntityState.deleted) {
                return _Promise.error(EntityErrorCode.exists);
            }
            if (!this.validate()) {
                return _Promise.error(EntityErrorCode.validationError);
            }
            return this.startTask(function updateAction(deferred) {
                _PI.api({
                    method: "put",
                    auth: this.getAuthToken(),
                    path: descriptor.path,
                    data: descriptor.data
                }, this).then(
                    function (data) {
                        this.afterUpdated(deferred, data, commit === undefined || commit === true);
                    },
                    function (error) {
                        this.setErrors(error.members);
                        deferred.rejectWith(this, []);
                    });
            });
        },
        afterUpdated: function (deferred, data, commit) {
            /// <signature>
            /// <summary>Occurs when a new data has updated. This function resolves the deferred object and calls the corresponding event handlers.</summary>
            /// <param name="deferred" type="$.Deferred">The deferred object that can be used to chain more operations.</param>
            /// <param name="data" type="Object">The data.</param>            
            /// <param name="commit" type="Boolean">Commits the current transaction after saving data.</param>
            /// </signature>
            this.setItemAs(data, EntityState.updated);
            // If the validation was successful, commit the current edit session.
            commit && this.endEdit();
            // Resolve the deferred object.
            deferred.resolveWith(this, [data]);
            // Raise an onupdated event.
            this.dispatchEvent("updated");
        },
        remove: function (cancel) {
            /// <signature>
            /// <summary>Deletes the current entity.</summary>
            /// <param name="cancel" type="Boolean" optional="true" value="true">Cancels the current transaction before saving data.</param>
            /// <returns type="$.Deferred" />
            /// </signature>
            var descriptor = this.descriptorFor("remove");
            if (!descriptor.path) {
                return _Promise.error(EntityErrorCode.notSupported);
            }
            if (cancel === undefined || cancel === true) {
                this.cancelEdit();
            }
            if (this.state() === EntityState.deleted) {
                return _Promise.error(EntityErrorCode.deleted);
            }
            return this.startTask(function removeAction(deferred) {
                _PI.api({
                    method: "delete",
                    auth: this.getAuthToken(),
                    path: descriptor.path,
                    data: descriptor.data
                }, this).then(
                    function () {
                        this.afterRemoved(deferred, cancel === undefined || cancel === true);
                    },
                    function (error) {
                        this.setErrors(error.members);
                        deferred.rejectWith(this, []);
                    });
            });
        },
        afterRemoved: function (deferred) {
            /// <signature>
            /// <summary>Occurs when a new data has removed. This function resolves the deferred object and calls the corresponding event handlers.</summary>
            /// <param name="deferred" type="$.Deferred">The deferred object that can be used to chain more operations.</param>
            /// </signature>
            this.setItemAs(undefined, EntityState.deleted);
            // Resolve the deferred object.
            deferred.resolveWith(this, []);
            // Raise an onremoved event.
            this.dispatchEvent("removed");
        },
        save: function () {
            /// <signature>
            /// <summary>Creates or updates an entity depending on the status of the current entity.</summary>
            /// <returns type="$.Deferred" />
            /// </signature>
            if ((this.state() === EntityState.unchanged) && !this.entityKey()) {
                return this.create.apply(this, arguments);
            }
            return this.update.apply(this, arguments);
        },
        edit: function () {
            /// <signature>
            /// <summary>Begins a new edit session.</summary>
            /// </signature>
            this.beginEdit();
        },
        cancel: function () {
            /// <signature>
            /// <summary>Cancels the current session.</summary>
            /// </signature>
            this.cancelEdit();
            this.dispatchEvent("canceled");
        },
        reset: function () {
            /// <signature>
            /// <summary>Resets the entity.</summary>
            /// </signature>
            this.state(EntityState.unchanged);
            this.entityKey(null);
        },
        confirm: function (type, done, fail) {
            /// <signature>
            /// <summary>Displays a dialog to confirm the specified operation.</summary>
            /// <param name="type" type="String">The type of the dialog.</param>
            /// </signature>
            /// <signature>
            /// <summary>Displays a dialog to confirm the specified operation.</summary>
            /// <param name="type" type="String">The type of the dialog.</param>
            /// <param name="done" type="Function">A callback function that can be called when the operation is succeeded.</param>
            /// <param name="fail" type="Boolean">A callback function that can be called when the operation is failed.</param>
            /// </signature>
            var that = this;
            _PI.dialog({
                name: "confirm",
                type: type,
                done: function (response) {
                    if (response.result === "yes") {
                        if (typeof done === "function") {
                            done.apply(that, []);
                        }
                    } else {
                        if (typeof fail === "function") {
                            fail.apply(that, []);
                        }
                    }
                }
            });
        },
        entityKey: function (value) {
            /// <signature>
            /// <summary>Gets the key for the current entity.</summary>
            /// <returns type="Object" />
            /// </signature>
            /// <signature>
            /// <summary>Sets the key for the current entity.</summary>
            /// <param name="value" type="Object">The key value.</param>
            /// </signature>
            if (this.item && this.options.key) {
                var key = this.options.key;
                var p = this.item[key];
                if (!arguments.length) {
                    return ko.unwrap(p);
                }
                if (ko.isObservable(p)) {
                    p(value || null);
                } else {
                    this.item[key] = value || null;
                }
            }
        },
        makeUrl: function (key, routeValues) {
            /// <signature>
            /// <summary>Gets a fully qualified URL path.</summary>
            /// <param name="key" type="String">The name of the key.</param>
            /// <param name="routeValues" type="Object" optional="true">A set of key/value pairs that can be used to configure URL parameters.</param>
            /// </signature>
            var path = this.options.urls && this.options.urls[key];
            if (path && routeValues) {
                path = _Utilities.actionLink(path, routeValues);
            }
            return path;
        },
        navigate: function (keyOrUrl, target) {
            /// <signature>
            /// <summary>Redirects the user to the specified URL.</summary>
            /// <param name="keyOrUrl" type="String">The name of the hypermedia link or URL.</param>
            /// </signature>
            /// <signature>
            /// <summary>Redirects the user to the specified URL.</summary>
            /// <param name="keyOrUrl" type="String">The name of the hypermedia link or URL.</param>
            /// <param name="target" type="String">The window target.</param>
            /// </signature>
            if (!keyOrUrl) {
                throw new TypeError("You must specify a key or URL.");
            }
            if (this.item) {
                keyOrUrl = ko.unwrap((ko.unwrap((ko.unwrap(this.item.links) || {})[keyOrUrl]) || {}).href) || keyOrUrl;
            }
            window.open(keyOrUrl, target || "_self");
        }
    }),

    EntityFilter = _Class.derive(_Knockout.Filter, function EntityFilter_ctor(options) {
        /// <signature>
        /// <summary>Represents a view that can be used to define AJAX query parameters.</summary>
        /// <param name="options" type="Object" optional="true">A set of key/value pairs that contains filter parameters.</param>
        /// <returns type="EntityFilter" />
        /// </signature>
        options || (options = {});
        options.changed = options.changed || null;
        _Knockout.Filter.apply(this, [options]);
    }),

    EntityCollection = _Class.derive(_PagedList, function EntityCollection_ctor(options) {
        /// <signature>
        /// <summary>Represents a view for grouping, sorting, filtering, and navigating a paged data collection.</summary>
        /// <param name="options" type="Object" optional="true">A set of key/value pairs that can be used to configure entity collection options.
        /// <para>autoOpen: Executes the open method with a default of no parameters.</para>
        /// <para>autoSave: A value indicating whether data are automatically updated after editing ( commitNew | commitEdit | remove ).</para>
        /// <para>cacheKey: The unique browser cache key for the entity type.</para>
        /// <para>items: An array of items that will be mapped to a KO observable array.</para>
        /// <para>ownsItems: Gets or sets whether objects in the list are owned by the list or not. If entries are owned, when an entry object is removed from the list, the entry object is disposed.</para>
        /// <para>supportsPaging: If set to true, paging parameters will be attached to the request.</para>
        /// <para>pageIndex: Gets or sets the index of the currently displayed page.</para>
        /// <para>pageSize: Gets or sets the number of records to display on a page.</para>
        /// <para>pageRateLimit: Delay time in millisecs.</para>
        /// <para>urls: An object that describes resource addresses on the Web ( query | remove ).</para>
        /// <para>onopened: A function that can be called when the current property has changed.</para>
        /// <para>oncurrentchanged: A function that can be called when the current property has changed.</para>
        /// </param>
        /// <returns type="EntityCollection" />
        /// </signature>
        options = options || {};
        options.items = options.items || []; // An array of items that will be mapped to an observable array.
        options.ownsItems = options.ownsItems !== false; // See List class.
        options.pageIndex = options.pageIndex || 1; // The index of the page of results to return. The page is not zero-based.
        options.pageSize = options.pageSize || _PI.options.pageSize || 50; // The size of the page of results to return.

        this._disposed = false;

        _PagedList.apply(this, [options]);

        this.autoOpen = options.autoOpen !== false; // Executes the open method with a default of no parameters.
        this.autoSave = options.autoSave !== false; // A value indicating whether data are automatically updated after editing ( commitNew | commitEdit | remove ).
        this.filter = options.filter || null;
        this.state = ko.observable(EntityCollectionState.closed);
        this.supportsPaging = options.supportsPaging !== false; // If set to true, paging parameters will be attached to the request.
        this.urls = options.urls || null; // An object that describes resource addresses on the Web ( query | remove ).

        this.setFilter(this.filter);
        this.autoOpen && this.open();
    }, {
        getAuthToken: function () {
            /// <signature>
            /// <summary>Gets an auth token.</summary>
            /// <returns type="String" />
            /// </signature>
            return this.options.authToken;
        },
        setFilter: function (filter) {
            /// <signature>
            /// <summary>Sets a new filter object for the collection.</summary>
            /// <param name="filter" type="EntityFilter">The new filter object to update.</param>
            /// </signature>
            if (filter) {
                filter.changed = this.refresh.bind(this);
                filter._list = this;
            }
            this.filter = filter;
        },
        open: _Promise.tasks.watch(function () {
            /// <signature>
            /// <summary>Loads objects into the collection, using the specified options.</summary>
            /// <returns type="WinJS.Promise" />
            /// </signature>
            var path = this.urls && this.urls.query;
            var filter = this.getFilter() || {};
            if (this.supportsPaging) {
                filter.page = this.pageIndex || 1;
                filter.count = this.pageSize || _PI.options.pageSize;
            }
            this.state(EntityCollectionState.loading);
            return _PI.api({
                auth: this.getAuthToken(),
                path: path,
                data: filter
            }, this).then(
                function (response) {
                    this.afterOpen(response);
                },
                function () {
                    this.state(EntityCollectionState.closed);
                    this.removeAll();
                    this.total = 0;
                }
            );
        }),
        afterOpen: function (response) {
            /// <signature>
            /// <summary>Occurs when a new data has created. This function resolves the deferred object and calls the corresponding event handlers.</summary>
            /// <param name="deferred" type="$.Deferred">The deferred object that can be used to chain more operations.</param>
            /// <param name="data" type="Object">The data.</param>
            /// </signature>
            this.state(EntityCollectionState.loaded);
            var data = response.data || [];
            this.total = response.total || data.length;
            this.replaceAll.apply(this, data);
            this.dispatchEvent("opened");
        },
        commitNew: function () {
            /// <signature>
            /// <summary>Ends the add transaction and saves the pending new item.</summary>
            /// </signature>
            var newItem = this._newItem;
            _PagedList.prototype.commitNew.apply(this, arguments);
            if (newItem !== null && this.autoSave) {
                var saveFn = newItem.save || newItem.create;
                if (typeof saveFn === "function") {
                    saveFn.apply(newItem, [false]);
                }
            }
        },
        commitEdit: function () {
            /// <signature>
            /// <summary>Ends the edit transaction and saves the pending changes.</summary>
            /// </signature>
            var editItem = this._editItem;
            _PagedList.prototype.commitEdit.apply(this, arguments);
            if (editItem !== null && this.autoSave) {
                var saveFn = editItem.save || editItem.update;
                if (typeof saveFn === "function") {
                    saveFn.apply(editItem, [false]);
                }
            }
        },
        deleteAll: _Promise.tasks.watch(function deleteAll() {
            /// <signature>
            /// <summary>Deletes all the selected items from the database.</summary>
            /// <returns type="$.Deferred" />
            /// </signature>            
            var items = this.selection();
            var len = items.length;
            if (len === 1) {
                return this.deleteItem(items[0]);
            } else {
                return this.deleteItems(items);
            }
        }),
        deleteItem: function (item) {
            /// <signature>
            /// <summary>Deletes the specified item and remove that from the collection.</summary>
            /// <param name="item" type="Object">The item to delete.</param>
            /// <returns type="$.Deferred" />
            /// </signature>
            if (!item) {
                throw new Error("item cannot be undefined or null.");
            }
            var deferred;
            if (item instanceof Entity) {
                deferred = item.remove();
            } else {
                var options = this.createDeleteItemOptions(item);
                if (options) {
                    options.method = "delete";
                    deferred = _PI.api(options);
                }
            }
            return $.when(deferred).done(this.onDeleteItemComplete.bind(this, item));
        },
        deleteItems: function (items) {
            /// <signature>
            /// <summary>Deletes the specified items and remove those from the collection.</summary>
            /// <param name="item" type="Number">The items to delete.</param>
            /// <returns type="$.Deferred" />
            /// </signature>
            if (!items) {
                throw new Error("items cannot be undefined or null.");
            }
            var deferred;
            var options = this.createDeleteItemsOptions(items);
            if (options) {
                options.method = "delete";
                deferred = _PI.api(options);
            }
            return $.when(deferred).done(this.onDeleteItemsComplete.bind(this, items));
        },
        createDeleteItemOptions: function (item) {
            /// <signature>
            /// <summary>Creates a new request message object.</summary>
            /// <param name="item" type="Object">The item to delete.</param>
            /// <returns type="Object" />
            /// </signature>
            var id = this.getEntityKey(item);
            var urls = this.urls || {};
            return id ? { path: _Utilities.actionLink(urls.remove || urls.query, { id: id }) } : null;
        },
        createDeleteItemsOptions: function (items) {
            /// <signature>
            /// <summary>Creates a new request message object.</summary>
            /// <param name="items" type="Array">The items to delete.</param>
            /// <returns type="Object" />
            /// </signature>
            var ids = items.map(this.getEntityKey);
            var urls = this.urls || {};
            return ids.length ? { path: _Utilities.actionLink(urls.removeAll || urls.remove || urls.query, { id: "", ids: ids }) } : null;
        },
        onDeleteItemComplete: function (item) {
            /// <signature>
            /// <summary>Occurs when the specified item is deleted.</summary>
            /// </signature>
            this.remove(item);
            this.refreshEmpty();
        },
        onDeleteItemsComplete: function (items) {
            /// <signature>
            /// <summary>Occurs when the specified items are deleted.</summary>
            /// </signature>
            this.remove(function (i) { return items.indexOf(i) >= 0; });
            this.refreshEmpty();
        },
        close: function () {
            /// <signature>
            /// <summary>Closes a dataset.</summary>
            /// </signature>
            this.commitAll();
            this.removeAll();
            this.state(EntityCollectionState.closed);
        },
        refresh: function () {
            /// <signature>
            /// <summary>Refreshes a dataset.</summary>
            /// </signature>
            return this.open();
        },
        refreshEmpty: function () {
            /// <signature>
            /// <summary>Refreshes the current collection.</summary>
            /// </signature>
            if (!this.items().length) {
                this.refresh(true);
            }
        },
        dispatch: function (member, args, onlyFirst) {
            /// <signature>
            /// <summary>Dispatches a command to the selected items(s) with the specified parameters.</summary>
            /// <param name="member" type="String">The name of the method on the selected object.</param>
            /// <param name="args" type="Array">An array of arguments passed to the function.</param>
            /// </signature>
            /// <signature>
            /// <summary>Dispatches a command to the selected items(s) with the specified parameters.</summary>
            /// <param name="member" type="String">The name of the method on the selected object.</param>
            /// <param name="args" type="Array">An array of arguments passed to the function.</param>
            /// <param name="onlyFirst" type="Boolean">A value indicating whether the first selected item will be notified.</param>
            /// </signature>
            var items = this.selection();
            for (var i = 0, len = items.length; i < len; ++i) {
                var item = items[i];
                var func = item && item[member];
                if (func) {
                    func.apply(item, args);
                }
                if (onlyFirst) {
                    break;
                }
            }
        },
        makeUrl: function (key, routeValues) {
            /// <signature>
            /// <summary>Gets a fully qualified URL path.</summary>
            /// <param name="key" type="String">The name of the key.</param>
            /// <param name="routeValues" type="Object" optional="true">A set of key/value pairs that can be used to configure URL parameters.</param>
            /// </signature>
            var path = this.urls && this.urls[key];
            if (path && routeValues) {
                path = _Utilities.actionLink(path, routeValues, null, null, true);
            }
            return path;
        },
        getFilter: function () {
            /// <signature>
            /// <summary>Deserializes the current knockout filter object to a JS object.</summary>
            /// <returns type="Object" />
            /// </signature>
            if (this.filter) {
                if (typeof this.filter.toObject === "function") {
                    return this.filter.toObject();
                }
                return _Knockout.deserialize(this.filter);
            }
            return null;
        },
        getEntityKey: function (item) {
            /// <signature>
            /// <param name="item" type="Object" />
            /// <returns type="Object" />
            /// </signature>
            return (item instanceof Entity ? item.entityKey() : ko.unwrap(item.id)) || null;
        },
        dispose: function () {
            /// <signature>
            /// <summary>Disposes the list.</summary>
            /// </signature>
            if (!this._disposed) {
                this.close();
            }
            _PagedList.prototype.dispose.apply(this, arguments);
            this._disposed = true;
        }
    }),

    DateClass = _Class.define(function (options) {
        /// <signature>
        /// <summary>Initializes a new instance of the Date class.</summary>
        /// <param name="options" type="Object">A set of key/value pairs that can be use to define date options.</param>
        /// <returns type="DateClass" />
        /// </signature>
        options = options || {};
        this.culture = window.Globalize.culture(options.culture || "hu");
        this.year = ko.observable();
        this.month = ko.observable();
        this.day = ko.observable();
        this.years = ko.observableArray();
        this.months = ko.observableArray();
        this.days = ko.observableArray();
        for (var year = new Date().getFullYear() - 13; year >= 1905; year--) {
            this.years.push(year);
        }
        this.months(this.culture.calendar.months.names || []);
        this.year.subscribe(this.update, this);
        this.month.subscribe(this.update, this);
        if (options.date) {
            this._parse(options.date);
        }
    }, {
        _parse: function (date) {
            /// <summary>Parses a date.</summary>
            this.year(date.getFullYear());
            this.month(this.months()[date.getMonth()]);
            this.day(date.getDate());
        },
        _month: function (zeroBased) {
            /// <summary>Gets the number representation of the selected month.</summary>
            var month = this.month();
            if (month) {
                if (zeroBased === true) {
                    return this.months.indexOf(month);
                } else {
                    return this.months.indexOf(month) + 1;
                }
            }
            return null;
        },
        update: function () {
            /// <summary>Updates the specified lists.</summary>
            this.days.removeAll();
            var year = this.year(), month = this.month();
            if (year && month) {
                var day = 1, days = Date.prototype.getDaysInMonth(year, this._month());
                while (day <= days) {
                    this.days.push(day++);
                }
            }
        },
        toDate: function (defaultYear, defaultMonth, defaultDay) {
            /// <summary>Converts the current object to a JS Date object.</summary>
            return new Date(
                this.year() || defaultYear || new Date().getFullYear(),
                this._month(true) || defaultMonth || 0,
                this.day() || defaultDay || 0);
        },
        toString: function () {
            return this.toDate().toISOString();
        }
    });

    //
    // Defines a class using the given constructor and the union of the set of instance members specified by all the mixin objects.
    //

    _Class.mix(Entity, _Utilities.createEventProperties("loaded", "created", "updated", "removed"));
    _Class.mix(Entity, EntityMixin);

    _Class.mix(EntityCollection, _Utilities.createEventProperties("opened"));
    _Class.mix(EntityCollection, EntityMixin);

    //
    // Public Namespace Exports
    //

    _WinJS.Namespace.defineWithParent(_PI, null, {
        options: options,
        EntityState: EntityState,
        EntityCollectionState: EntityCollectionState,
        Entity: Entity,
        EntityMixin: EntityMixin,
        EntityFilter: EntityFilter,
        EntityCollection: EntityCollection,
        Date: DateClass
    });

})(WinJS, PI);