// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="../services/businesstag.js" />

(function (_Global, _KO, _WinJS, _PI) {
    "use strict";

    var _Class = _WinJS.Class;
    var _Utilities = _WinJS.Utilities;
    var _Promise = _WinJS.Promise;
    var _Knockout = _WinJS.Knockout;

    var _observable = _KO.observable;
    var _observableArray = _KO.observableArray;
    var _pureComputed = _KO.pureComputed;

    function getTagInfo(tags, strike) {
        return tags.map(function (tag) {
            return '<strong>' + (strike ? ('<strike>' + tag.name + '<\/strike>') : tag.name) + '<\/strong>';
        }).join(", ");
    }

    var ns = _WinJS.Namespace.defineWithParent(_PI, "Project", {

        BusinessTag: _Class.define(function BusinessTag_ctor(businessTag, options) {
            /// <signature>
            /// <summary>Initializes a new instance of the BusinessTag class.</summary>
            /// <param name="businessTag" type="Object" optional="true" />
            /// <param name="options" type="Object" optional="true" />
            /// <returns type="PI.Project.BusinessTag" />
            /// </signature>
            businessTag = businessTag || {};
            _Utilities.setOptions(this, options = options || {});
            _Promise.tasks(this);

            this.autoSave = options.autoSave !== false;
            this.service = options.service || ns.BusinessTagService;
            this.project = options.project || businessTag.project;

            this.id = _observable();
            this.name = _observable().extend({ required: { message: "Címke névre szükség van." } });
            this.color = _observable();
            this.itemCount = businessTag.itemCount | 0;
            this.checked = _observable(options.checked);

            this.errors = _KO.validation.group(this);
            this.editing = _observable(false);
            this.exists = _pureComputed(this._exists, this);
            this.displayName = _pureComputed(this._getDisplayName, this);
            this.description = _pureComputed(this._getDescription, this);

            this.update(businessTag);

            this._colorChangedSn = this.color.subscribe(this.onColorChanged, this);
            this._checkedChangedSn = this.checked.subscribe(this.onCheckedChanged, this);
        }, {
            update: function (businessTag) {
                /// <signature>
                /// <param name="businessTag" type="Object" />
                /// </signature>
                businessTag = businessTag || {};
                this._updating = true;
                this.id(businessTag.id);
                this.name(businessTag.name);
                this.color(businessTag.color);
                this._updating = false;
            },
            toObject: function () {
                /// <signature>
                /// <returns type="Object" />
                /// </signature>
                return {
                    id: this.id(),
                    name: this.name(),
                    color: this.color()
                };
            },

            //
            // Validation
            //

            validate: function () {
                /// <signature>
                /// <summary>Returns true if this project is valid.</summary>
                /// <returns type="Boolean" />
                /// </signature>
                return this.errors().length === 0;
            },

            //
            // Edit Session
            //

            beginEdit: function () {
                /// <signature>
                /// <summary>Begins an edit on an object.</summary>
                /// </signature>
                if (this._editSession) {
                    return;
                }
                this._editSession = new _KO.editSession(this, { fields: ["name", "color"] });
                this.editing(true);
            },
            cancelEdit: function () {
                /// <signature>
                /// <summary>Discards changes since the last BeginEdit call.</summary>
                /// </signature>
                if (!this._editSession) {
                    return;
                }
                this._editSession.cancel();
                this._editSession = null;
                this.editing(false);
            },
            endEdit: function () {
                /// <signature>
                /// <summary>Pushes changes since the last beginEdit.</summary>
                /// </signature>
                if (!this.validate()) {
                    return;
                }
                if (this.autoSave) {
                    var that = this;
                    this.saveAsync().then(function () {
                        that._editSession = null;
                        that.editing(false);
                    });
                } else {
                    this._editSession = null;
                    this.editing(false);
                }
            },

            //
            // Storage Operations
            //

            loadAsync: _Promise.tasks.watch(function () {
                /// <signature>
                /// <returns type="WinJS._Promise" />
                /// </signature>
                if (this.exists()) {
                    return this.service.getByIdAsync(this.id(), this)
                        .then(function (businessTag) {
                            this.update(businessTag);
                            this.dispatchEvent("loaded", businessTag);
                        });
                }
                return _Promise.error();
            }),
            saveAsync: _Promise.tasks.watch(function () {
                /// <signature>
                /// <returns type="WinJS._Promise" />
                /// </signature>
                if (!this.validate()) {
                    return _Promise.error();
                }
                var businessTag = this.toObject();
                if (this.exists()) {
                    return this.service.updateAsync(businessTag.id, businessTag, this)
                        .then(function (businessTag) {
                            this.update(businessTag);
                            this.dispatchEvent("saved", businessTag);
                        });
                }
                return this.service.createAsync(this.project.id, businessTag, this)
                    .then(function (businessTag) {
                        this.update(businessTag);
                        this.dispatchEvent("saved", businessTag);
                    });
            }),
            deleteAsync: _Promise.tasks.watch(function () {
                /// <signature>
                /// <returns type="WinJS._Promise" />
                /// </signature>
                if (!this.exists()) {
                    return _Promise.complete();
                }
                return this.service.deleteAsync(this.id(), this)
                    .then(function () {
                        this.dispatchEvent("deleted");
                    });
            }),

            //
            // Event & Computed Members
            //

            onColorChanged: function (value) {
                /// <signature>
                /// <param name="value" type="String" />
                /// </signature>
                this.autoSave && !this._updating && this.saveAsync();
            },
            onCheckedChanged: function (value) {
                /// <signature>
                /// <param name="value" type="Boolean" />
                /// </signature>
                this.dispatchEvent("checked", value);
            },
            _exists: function () {
                /// <signature>
                /// <returns type="Boolean" />
                /// </signature>
                return !!this.id();
            },
            _getDisplayName: function () {
                /// <signature>
                /// <returns type="String" />
                /// </signature>
                return this.name() + " (" + this.itemCount + ")";
            },
            _getDescription: function () {
                /// <signature>
                /// <returns type="String" />
                /// </signature>
                return this._getDisplayName() + " # " + this.id();
            },

            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                this._colorChangedSn && this._colorChangedSn.dispose();
                this._colorChangedSn = null;
                this._checkedChangedSn && this._checkedChangedSn.dispose();
                this._checkedChangedSn = null;
                this.exists && this.exists.dispose();
                this.exists = null;
                this.errors && this.errors.dispose();
                this.errors = null;
            }
        }),

        BusinessTagFilter: _Class.define(function BusinessTagFilter_ctor(list, options) {
            /// <signature>
            /// <summary>Initializes a new instance of the BusinessTagFilter class.</summary>
            /// <param name="list" type="PI.Project.BusinessTagList" />
            /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the BusinessTagFilter.</param>
            /// <returns type="PI.Project.BusinessTagFilter" />
            /// </signature>
            options = options || {};
            this._disposed = false;
            this._list = list;
            this.project = _observable(options.project);
            this._session = _KO.editSession(this);
        }, {
            submit: function () {
                /// <signature>
                /// <summary>Commits the edit session.</summary>
                /// </signature>
                this._list.refresh();
            },
            cancel: function () {
                /// <signature>
                /// <summary>Cancels the edit session.</summary>
                /// </signature>
                this._session.cancel();
                this._list.refresh();
            },
            toObject: function () {
                /// <signature>
                /// <summary>Returns a pure JS object without knockout observable functions</summary>
                /// <returns type="Object" />
                /// </signature>
                var project = this.project();
                return {
                    projectId: project && project.id
                };
            },
            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                if (this._disposed) {
                    return;
                }
                this._session && this._session.dispose();
                this._session = null;
                this._disposed = true;
            }
        }),
        /*
        BusinessTagListItem: _Class.define(function BusinessTagListItem_ctor(businessTag) {
            /// <signature>
            /// <summary>Initializes a new instance of the BusinessTagListItem class.</summary>
            /// <param name="options" type="businessTag" />
            /// <returns type="PI.Project.BusinessTagListItem" />
            /// </signature>
            this.id = businessTag.id;
            this.project = businessTag.project;
            this.name = businessTag.name;
            this.color = businessTag.color;
            this.itemCount = businessTag.itemCount | 0;
            this.displayName = this.name + " (" + this.itemCount + ")";
            this.checked = _observable(businessTag.checked);
            this.editing = _observable(false);
        }, {
            toObject: function () {
                /// <signature>
                /// <summary>Returns a pure object.</summary>
                /// <returns type="Object" />
                /// </signature>
                return {
                    id: this.id,
                    project: this.project,
                    name: this.name,
                    color: this.color,
                    checked: this.checked()
                };
            }
        }),
        */
        BusinessTagListCommands: _Class.define(function BusinessTagListCommands_ctor(list) {
            /// <signature>
            /// <summary>Initializes a new instance of the BusinessTagListCommands class.</summary>
            /// <param name="list" type="Object" optional="true" />
            /// <returns type="PI.Project.BusinessTagListCommands" />
            /// </signature>
            this._list = list;
        }, {
            "selection.delete": function () {
                /// <signature>
                /// <summary>Deletes the current item.</summary>
                /// </signature>
                var item = this._list.selection()[0];
                item && this._list.service.deleteAsync(item.id, this)
                    .then(function () {
                        this._list.remove(item);
                    });
            }
        }),

        BusinessTagList: _Class.derive(_Knockout.List, function BusinessTagList_ctor(options) {
            /// <signature>
            /// <summary>Initializes a new instance of the BusinessTagList class.</summary>
            /// <param name="options" type="Object" />
            /// <returns type="PI.Project.BusinessTagList" />
            /// </signature>
            options = options || {};
            options.autoLoad = options.autoLoad !== false;
            options.pageIndex = options.pageIndex || 1;
            options.pageSize = options.pageSize || 50;
            _Knockout.List.call(this, options);

            this._disposed = false;
            this._onItemCheckedBound = this._onItemChecked.bind(this);
            this._onItemSavedBound = this._onItemSaved.bind(this);
            this._onItemDeletedBound = this._onItemDeleted.bind(this);

            this.service = options.service || ns.BusinessTagService;
            this.commands = new ns.BusinessTagListCommands(this);
            this.filter = new ns.BusinessTagFilter(this, options.filter);
            this.readonly = _observable(!!options.readonly);
            this.options.autoLoad && this.refresh();
        }, {
            mapItem: function (item) {
                /// <signature>
                /// <param name="item" type="Object" />
                /// <returns type="PI.Project.BusinessTag" />
                /// </signature>
                var params = { item: item, options: { project: this.filter.project() } };
                this.dispatchEvent("iteminit", params);
                params.options.onchecked = this._onItemCheckedBound;
                params.options.onsaved = this._onItemSavedBound;
                params.options.ondeleted = this._onItemDeletedBound;
                return new ns.BusinessTag(params.item, params.options);
            },
            refresh: _Promise.tasks.watch(function () {
                /// <signature>
                /// <summary>Refreshes the list.</summary>
                /// </signature>
                var params = this._createParams();
                return this.service.getAllAsync(params, this).then(
                    function (response) {
                        this.dispatchEvent("loaded", response);
                        this.replaceAll.apply(this, response.data);
                    },
                    function () {
                        this.removeAll();
                    });
            }),
            sortByName: function () {
                /// <signature>
                /// <summary>Sorts the items by name.</summary>
                /// </signature>
                this.sort(function (t1, t2) {
                    return t1.name > t2.name ? 1 : t1.name < t2.name ? -1 : 0;
                });
            },
            _createParams: function () {
                /// <signature>
                /// <returns type="Object" />
                /// </signature>
                return this.filter.toObject() || {};
            },

            //
            // Item State Operations
            //

            getCheckedItems: function () {
                /// <signature>
                /// <summary>Gets a state object.</summary>
                /// </signature>
                var checked = [];
                var unchecked = [];
                this.forEach(function (item) {
                    var checkedState = item.checked();
                    if (checkedState === undefined || checkedState === null) {
                        return;
                    }
                    var businessTag = item.toObject();
                    if (checkedState) {
                        checked.push(businessTag);
                    } else {
                        unchecked.push(businessTag);
                    }
                });
                var checkedInfo = getTagInfo(checked, false);
                var uncheckedInfo = getTagInfo(unchecked, true);
                return {
                    checked: checked,
                    unchecked: unchecked,
                    expression: (checkedInfo || (uncheckedInfo ? "Mind" : "")) + (uncheckedInfo ? (", de nem " + uncheckedInfo) : "")
                };
            },
            clearCheckedItems: function () {
                /// <signature>
                /// <summary>Sets the checked flag to undefined on each of the items.</summary>
                /// </signature>
                this.forEach(function (businessTag) {
                    businessTag.checked(undefined);
                });
            },

            //
            // Event Handlers
            //

            _onItemChecked: function (event) {
                /// <signature>
                /// <param name="event" type="Event" />
                /// </signature>
                this.dispatchEvent("itemchecked", this.getCheckedItems());
            },
            _onItemSaved: function (event) {
                /// <signature>
                /// <param name="event" type="Event" />
                /// </signature>
                this.sortByName();
            },
            _onItemDeleted: function (event) {
                /// <signature>
                /// <param name="event" type="Event" />
                /// </signature>
                this.remove(event.target);
            },

            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                if (this._disposed) {
                    return;
                }
                this.filter && this.filter.dispose();
                this.filter = null;
                _Knockout.List.prototype.dispose.apply(this, arguments);
                this._disposed = true;
            }
        })

    });

    _Class.mix(ns.BusinessTag, _Utilities.createEventProperties("loaded", "saved", "deleted", "checked"));
    _Class.mix(ns.BusinessTag, _Utilities.eventMixin);
    _Class.mix(ns.BusinessTagList, _Utilities.createEventProperties("loaded", "iteminit", "itemchecked"));

})(window, ko, WinJS, PI);