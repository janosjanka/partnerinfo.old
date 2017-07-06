// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="../services/action.js" />

(function (_Global, _KO, _WinJS, _PI) {
    "use strict";

    var _Class = _WinJS.Class;
    var _Utilities = _WinJS.Utilities;
    var _Promise = _WinJS.Promise;
    var _Knockout = _WinJS.Knockout;

    var _observable = _KO.observable;
    var _observableArray = _KO.observableArray;
    var _pureComputed = _KO.pureComputed;

    function normalize(str) {
        return str.substr(0, 1).toUpperCase() + str.substr(1);
    }

    var ns = _WinJS.Namespace.defineWithParent(_PI, "Project", {

        ActionResources: {
            createLink: function (projectId) { return _Utilities.actionLink("/admin/projects/{projectId}/#/actions/create", { projectId: projectId }); },
            updateLink: function (projectId, id) { return _Utilities.actionLink("/admin/projects/{projectId}/#/actions/{id}", { projectId: projectId, id: id }); },
            actionEnabled: function () { return _T("pi/project/actionEnabled"); },
            actionName: function (child) { return _T(child ? "pi/project/actionChildName" : "pi/project/actionRootName"); },
            actionNameRequired: function () { return _T("pi/project/actionNameRequired"); },
            actionTypeName: function (type) { return _T("pi/project/actionType" + normalize(type)); },
            actionTypeDescription: function (type) { return _T("pi/project/actionType" + normalize(type) + "Description"); }
        },

        ActionField: {
            /// <field>
            /// Includes no extra fields
            /// </field>
            none: 0,

            /// <field>
            /// Includes project info
            /// </field>
            project: 1 << 0
        },

        ActionSortOrder: {
            /// <field>
            /// This is the default value for actions
            /// </field>
            none: "none",

            /// <field>
            /// Actions are returned in reverse chronological order
            /// </field>
            recent: "recent",

            /// <field>
            /// Actions are returned in name order
            /// </field>
            name: "name"
        },

        ActionOptionsBase: _Class.define(function ActionOptionsBase_ctor(action) {
            /// <signature>
            /// <summary>Initializes a new instance of the ActionOptionsBase class.</summary>
            /// <param name="action" type="PI.Project.Action" />
            /// <returns type="PI.Project.ActionOptionsBase" />
            /// </signature>
            this._disposed = false;
            this.action = action;
            this.options = action.options() || {};
        }, {
            initialize: function () {
                /// <signature>
                /// <summary>Initializes validation logic.</summary>
                /// </signature>
                this.errors = _KO.validation.group(this);
            },
            validate: function () {
                /// <signature>
                /// <summary>Returns true if this object is valid.</summary>
                /// <returns type="Boolean" />
                /// </signature>
                return this.errors().length === 0;
            },
            toObject: function () {
                /// <signature>
                /// <returns type="Object" />
                /// </signature>
                return this.action.options();
            },
            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                if (this._disposed) {
                    return;
                }
                this.errors && this.errors.dispose();
                this.errors = null;
                this._disposed = true;
            }
        }),

        ActionTypeList: _Class.define(function ActionTypeList_ctor(types) {
            /// <signature>
            /// <summary>Initializes a new instance of the ActionTypeList class.</summary>
            /// <param name="types" type="Array" optional="true" />
            /// <returns type="ActionTypeList" />
            /// </signature>
            this.types = _observableArray();
            this.register({ name: "unknown", color: "transparent" });
            types && types.forEach(this.register, this);
        }, {
            find: function (name) {
                /// <signature>
                /// <summary>Finds a type by name.</summary>
                /// <returns type="Object" />
                /// </signature>
                return this.types().find(function (t) { return t.name === name; }) || this.types()[0];
            },
            register: function (actionType) {
                /// <signature>
                /// <summary>Registers an action type.</summary>
                /// <param name="actionType" type="Object" />
                /// </signature>
                this.types.push({
                    name: actionType.name,
                    normalizedName: normalize(actionType.name),
                    displayName: ns.ActionResources.actionTypeName(actionType.name),
                    description: ns.ActionResources.actionTypeDescription(actionType.name),
                    color: actionType.color || "#fff",
                    children: !!actionType.children,
                    type: actionType.type
                });
            },
            registerAll: function () {
                /// <signature>
                /// <summary>Registers an array of action types.</summary>
                /// <param name="actionTypes" type="Array" parameterArray="true" />
                /// </signature>
                for (var i = 0, len = arguments.length; i < len; ++i) {
                    this.register(arguments[i]);
                }
            },
            create: function (action) {
                /// <signature>
                /// <summary>Creates a new instance of the given action type.</summary>
                /// <param name="action" type="PI.Project.Action" />
                /// <returns type="Object" />
                /// </signature>
                var typeInfo = this.find(action.type());
                if (!typeInfo || !typeInfo.type) {
                    return;
                }
                var options = new typeInfo.type(action);
                if (options.initialize) {
                    options.initialize();
                }
                return options;
            }
        }),

        Action: _Class.define(function Action_ctor(project, action, options) {
            /// <signature>
            /// <summary>Initializes a new instance of the Action class.</summary>
            /// <param name="project" type="Object" />
            /// <param name="action" type="Object" optional="true" />
            /// <param name="options" type="Object" optional="true" />
            /// <returns type="Action" />
            /// </signature>
            var that = this;
            action = action || {};
            _Utilities.setOptions(this, options = options || {});
            _Promise.tasks(this);

            this._disposed = false;

            this.autoSave = options.autoSave !== false;
            this.service = options.service || ns.ActionService;
            this.types = options.types || ns.globalActionTypes;
            this.project = project;
            this.parent = options.parent;
            this.index = 1;

            this.id = _observable();
            this.type = _observable().extend({ required: true });
            this.typeInfo = _observable();
            this.enabled = _observable();
            this.modifiedDate = _observable();
            this.name = _observable().extend({ required: { onlyIf: function () { return !options.parent; }, message: ns.ActionResources.actionNameRequired() } });
            this.options = _observable();
            this.optionsView = _observable();
            this.link = _observable();
            this.children = _observableArray();
            this.childrenView = this.children.map(function (child) { return new ns.Action(project, child, { service: options.service, types: options.types, parent: that }); });

            this.editing = _observable(false);
            this.errors = _KO.validation.group(this);
            this.exists = _pureComputed(this._exists, this);
            this.info = _pureComputed(this._getInfo, this);
            this.canSave = _pureComputed(this._canSave, this);
            this.canCancel = _pureComputed(this._canCancel, this);
            this.canAddNew = _pureComputed(this._canAddNew, this);
            this.canRemove = _pureComputed(this._canRemove, this);

            this._typeChangedSn = this.type.subscribe(this._typeChanged, this);
            this._update(action, true);
        }, {
            /// <field name="parent" type="PI.Project.Action" />
            parent: {
                value: null,
                writable: true
            },

            //
            // Public Serialization Functions
            //

            toObject: function (includeChildActions) {
                /// <signature>
                /// <param name="includeChildActions" type="Boolean" />
                /// <returns type="Object" />
                /// </signature>
                var optionsView = this.optionsView();
                var action = {
                    id: this.id(),
                    name: this.name(),
                    type: this.type(),
                    options: optionsView ? optionsView.toObject() : this.options(),
                    enabled: this.enabled(),
                    modifiedDate: this.modifiedDate()
                };
                if (includeChildActions) {
                    action.children = this.childrenView().map(function (child) { return child.toObject(); });
                }
                return action;
            },

            //
            // Public In-Memory Data Functions
            //

            addNew: function (action) {
                /// <signature>
                /// <summary>Adds a new child action to the children collection.</summary>
                /// <param name="action" type="Object" optional="true" />
                /// <returns type="PI.Project.Action" />
                /// </signature>
                action = action || {};
                action.type = action.type || "sequence"; // TODO: it is hard-coded now
                var actionView = this.childrenView()[this.children.push(action) - 1];
                actionView.beginEdit();
                return actionView;
            },
            beginEdit: function () {
                /// <signature>
                /// <summary>Begins an edit on an object.</summary>
                /// </signature>
                if (this._editSession) {
                    return;
                }
                this._editSession = _KO.editSession(this, { fields: ["enabled", "name", "options", "type"] });
                this._createOptionsView();
                this.editing(true);
            },
            cancelEdit: function () {
                /// <signature>
                /// <summary>Discards changes since the last beginEdit call.</summary>
                /// </signature>
                if (!this._editSession) {
                    return;
                }
                this._editSession.cancel();
                this._editSession = null;
                this._createOptionsView();
                this.editing(false);
                !this.exists() && this._remove();
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
            validate: function () {
                /// <signature>
                /// <summary>Returns true if the object is valid. This method also validates the options viewModel object.</summary>
                /// <returns type="Boolean" />
                /// </signature>
                var optionsView = this.optionsView();
                if (optionsView && optionsView.validate && !optionsView.validate()) {
                    return false;
                }
                return this.errors().length === 0;
            },

            //
            // Public Service Data Functions
            //

            loadAsync: _Promise.tasks.watch(function () {
                /// <signature>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                if (this.exists()) {
                    return this.service.getByIdAsync(this.id(), this)
                        .then(function (action) {
                            this._update(action, true);
                            this.dispatchEvent("loaded");
                        });
                }
                return _Promise.error();
            }),
            saveAsync: _Promise.tasks.watch(function () {
                /// <signature>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                if (!this.validate()) {
                    return _Promise.error();
                }
                var action = this.toObject();
                if (this.exists()) {
                    return this.service.updateAsync(action.id, action, this)
                        .then(function (action) {
                            this._update(action, false);
                            this.dispatchEvent("saved", { state: "modified", data: action });
                        });
                }
                return this.service.createAsync(this.project.id, this.parent && this.parent.id(), action, this)
                    .then(function (action) {
                        this._update(action, false);
                        this.dispatchEvent("saved", { state: "added", data: action });
                    });
            }),
            copyBeforeAsync: _Promise.tasks.watch(function (referenceAction) {
                /// <signature>
                /// <summary>Copies the specified action before a reference action as a child of the current action.</summary>
                /// <param name="referenceAction" type="PI.Project.Action" optional="true" mayBeNull="true">
                /// The reference action. If null, action is inserted at the end of the list of child nodes.
                /// </param>
                /// <returns type="WinJS.Promise" /> 
                /// </signature>
                if (!this.exists()) {
                    return _Promise.error();
                }
                return this.service.copyBeforeAsync(this.id(), referenceAction && referenceAction.id(), this)
                    .then(function (newAction) {
                        this._copyBefore(newAction, referenceAction);
                        this.dispatchEvent("copied");
                    });
            }),
            moveBeforeAsync: _Promise.tasks.watch(function (referenceAction) {
                /// <signature>
                /// <summary>Moves the specified action before a reference action as a child of the current action.</summary>
                /// <param name="referenceAction" type="PI.Project.Action" optional="true" mayBeNull="true">
                /// The reference action. If null, action is inserted at the end of the list of child nodes.
                /// </param>
                /// <returns type="WinJS.Promise" /> 
                /// </signature>
                if (!this.exists()) {
                    return _Promise.error();
                }
                return this.service.moveBeforeAsync(this.id(), referenceAction && referenceAction.id(), this)
                    .then(function () {
                        this._moveBefore(referenceAction);
                        this.dispatchEvent("moved", { referenceAction: referenceAction });
                    });
            }),
            deleteAsync: _Promise.tasks.watch(function () {
                /// <signature>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                if (!this.exists()) {
                    this._remove();
                    return _Promise.complete();
                }
                return this.service.deleteAsync(this.id(), this)
                    .then(function () {
                        this._remove();
                        this.dispatchEvent("deleted");
                    });
            }),
            toggleAsync: function () {
                /// <signature>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                this.enabled(!this.enabled());
                return this.saveAsync();
            },

            //
            // Public Tree Traversal Functions
            //

            enumerateParents: function (callback) {
                /// <signature>
                /// <param name="callback" type="Function" />
                /// </signature>
                var parent = this.parent;
                while (parent) {
                    if (callback(parent)) {
                        break;
                    }
                    parent = parent.parent;
                }
            },
            enumerateChildren: function (callback) {
                /// <signature>
                /// <param name="callback" type="Function" />
                /// </signature>
                var stack = this.childrenView().splice(0);
                while (stack.length) {
                    var action = stack.pop();
                    if (callback(action)) {
                        break;
                    }
                    var children = action.childrenView();
                    for (var i = 0, len = children.length; i < len; ++i) {
                        stack.push(children[i]);
                    }
                }
            },

            //
            // Private In-Memory Data Functions
            //

            _update: function (action, includeChildActions) {
                /// <signature>
                /// <summary>Updates this action with the specified values.</summary>
                /// <param name="action" type="Object" />
                /// <param name="includeChildActions" type="Boolean" />
                /// </signature>
                this.project = action.project || this.project;

                this.id(action.id);
                this.name(action.name);
                this.enabled(action.enabled !== false);
                this.modifiedDate(action.modifiedDate);
                includeChildActions && this.children(action.children || []);
                this.options(action.options);
                this.link(action.link);

                // This must be the last operation to be available all field data when firing event handlers
                this.type(action.type || "unknown");
            },
            _copyBefore: function (newAction, referenceAction) {
                /// <signature>
                /// <summary>Copies the specified action before a reference action as a child of the current action.</summary>
                /// <param name="newAction" type="Object" />
                /// <param name="referenceAction" type="PI.Project.Action" optional="true" mayBeNull="true">
                /// The reference action. If null, action is inserted at the end of the list of child nodes.
                /// </param>
                /// </signature>
                if (!newAction || !this.parent) {
                    return;
                }
                var newActionView = this.parent.childrenView()[this.parent.children.push(newAction) - 1];
                newActionView._moveBefore(referenceAction);
            },
            _moveBefore: function (referenceAction) {
                /// <signature>
                /// <summary>Moves the specified action before a reference action as a child of the current action.</summary>
                /// <param name="referenceAction" type="PI.Project.Action" optional="true" mayBeNull="true">
                /// The reference action. If null, action is inserted at the end of the list of child nodes.
                /// </param>
                /// </signature>
                if (!this.parent) {
                    return;
                }
                var actionIndex = this.parent.childrenView().indexOf(this);
                if (actionIndex < 0) {
                    return;
                }
                if (!referenceAction) {
                    this.parent.children.splice(this.parent.children().length - 1, 0, this.parent.children.splice(actionIndex, 1)[0]);
                } else {
                    if (!referenceAction.parent) {
                        return;
                    }
                    var referenceActionIndex = referenceAction.parent.childrenView().indexOf(referenceAction);
                    if (referenceActionIndex < 0) {
                        return;
                    }
                    referenceAction.parent.children.splice(referenceActionIndex, 0, this.parent.children.splice(actionIndex, 1)[0]);
                }
            },
            _remove: function () {
                /// <signature>
                /// <summary>Removes this action from the parent collection.</summary>
                /// </signature>
                if (!this.parent) {
                    return;
                }
                var childIndex = this.parent.childrenView().indexOf(this);
                if (childIndex >= 0) {
                    this.parent.children.splice(childIndex, 1);
                    this.enumerateChildren(function (action) { action.dispose(); });
                    this.dispose();
                }
            },
            _createOptionsView: function () {
                /// <signature>
                /// <summary>Creates the given view for this action type.</summary>
                /// </signature>
                this._destroyOptionsView();
                this.optionsView(this.types.create(this));
            },
            _destroyOptionsView: function () {
                /// <signature>
                /// <summary>Destroys the current view for this action type.</summary>
                /// </signature>
                var optionsView = this.optionsView();
                if (optionsView && optionsView.dispose) {
                    optionsView.dispose();
                }
                this.optionsView(null);
            },

            //
            // Event & Computed Members
            //

            _canSave: function () {
                /// <signature>
                /// <returns type="Boolean" />
                /// </signature>
                return this.editing() && this.validate();
            },
            _canAddNew: function () {
                /// <signature>
                /// <returns type="Boolean" />
                /// </signature>
                return this.exists()
                    && this.enabled()
                    && this.typeInfo().children;
            },
            _canRemove: function () {
                /// <signature>
                /// <returns type="Boolean" />
                /// </signature>
                return this.parent && this.exists();
            },
            _canCancel: function () {
                /// <signature>
                /// <returns type="Boolean" />
                /// </signature>
                if (!this.parent && !this.exists()) {
                    // This is the root item and still has not been saved.
                    return false;
                }
                return this.editing();
            },
            _exists: function () {
                /// <signature>
                /// <returns type="Boolean" />
                /// </signature>
                return !!this.id();
            },
            _getInfo: function () {
                /// <signature>
                /// <returns type="Object" />
                /// </signature>
                var typeInfo = this.typeInfo();
                var info = typeInfo.type && typeInfo.type.getInfo && typeInfo.type.getInfo(this, typeInfo) || {};
                info.name = info.name || this.name() || typeInfo.displayName;
                info.help = info.help;
                info.link = info.link;
                return info;
            },
            _typeChanged: function (value) {
                /// <signature>
                /// <param name="value" type="String" />
                /// </signature>
                this._destroyOptionsView();

                // Find an action type editor for the specified type.
                this.typeInfo(this.types.find(value));

                // Create a new instance of the 
                this.editing() && this._createOptionsView();
            },
            getPath: function (index) {
                /// <signature>
                /// <summary>Gets level information at the specified UI index. This operation can be optimized out later.</summary>
                /// <param name="index" type="Number" />
                /// <returns type="String" />
                /// </signature>
                this.index = index;
                var values = [index];
                var parent = this.parent;
                while (parent) {
                    values.unshift(parent.index);
                    parent = parent.parent;
                }
                return values.join(".");
            },
            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                if (this._disposed) {
                    return;
                }
                this._typeChangedSn && this._typeChangedSn.dispose();
                this._typeChangedSn = null;
                this.canRemove && this.canRemove.dispose();
                this.canRemove = null;
                this.canAddNew && this.canAddNew.dispose();
                this.canAddNew = null;
                this.canCancel && this.canCancel.dispose();
                this.canCancel = null;
                this.canSave && this.canSave.dispose();
                this.canSave = null;
                this.info && this.info.dispose();
                this.info = null;
                this.exists && this.exists.dispose();
                this.exists = null;
                this.errors && this.errors.dispose();
                this.errors = null;
                this._disposed = true;
            }
        }),

        ActionListFilter: _Class.define(function ActionListFilter_ctor(list, options) {
            /// <signature>
            /// <summary>Initializes a new instance of the ActionListFilter class.</summary>
            /// <param name="list" type="PI.Project.ActionList" />
            /// <param name="options" type="Object" optional="true" />
            /// <returns type="ActionFilter" />
            /// </signature>
            options = options || {};
            this._disposed = false;
            this._list = list;
            this._cacheKey = options._cacheKey || "project.action-filter";

            this.project = options.project;
            this.name = _observable(options.name);
            this.orderBy = _observable(options.orderBy || ns.ActionSortOrder.recent);
            this.fields = _observable(options.fields || ns.ActionField.none);
            this.load();

            this._session = _KO.editSession(this, { fields: ["name"] });
            this._orderBySn = this.orderBy.subscribe(this.submit, this);
        }, {
            load: function () {
                /// <signature>
                /// <summary>Loads this filter.</summary>
                /// </signature>
                var filter = _PI.userCache(_PI.Storage.local, this._cacheKey);
                if (filter) {
                    this.orderBy(filter.orderBy || ns.ActionSortOrder.recent);
                    this.fields(filter.fields || ns.ActionField.none);
                }
            },
            save: function () {
                /// <signature>
                /// <summary>Save this filter.</summary>
                /// </signature>
                _PI.userCache(_PI.Storage.local, this._cacheKey, {
                    orderBy: this.orderBy(),
                    fields: this.fields()
                });
            },
            submit: function () {
                /// <signature>
                /// <summary>Commits the edit session.</summary>
                /// </signature>
                this._list.refresh();
                this.save();
            },
            cancel: function () {
                /// <signature>
                /// <summary>Cancels the edit session.</summary>
                /// </signature>
                this._session.cancel();
                this._list.refresh();
                this.save();
            },
            toObject: function () {
                /// <signature>
                /// <summary>Returns a pure JS object without knockout observable functions</summary>
                /// <returns type="Object" />
                /// </signature>
                return {
                    projectId: this.project && this.project.id,
                    name: this.name(),
                    orderBy: this.orderBy(),
                    fields: this.fields()
                };
            },
            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                if (this._disposed) {
                    return;
                }
                this._orderBySn && this._orderBySn.dispose();
                this._session && this._session.dispose();
                this._orderBySn = null;
                this._session = null;
                this._disposed = true;
            }
        }),

        ActionListItem: _Class.define(function ActionListItem_ctor(list, project, action) {
            /// <signature>
            /// <summary>Initializes a new instance of the ActionListItem class.</summary>
            /// <param name="list" type="PI.Project.ActionList" />
            /// <param name="project" type="Object" />
            /// <param name="action" type="Object" />
            /// <returns type="ActionListItem" />
            /// </signature>
            this._list = list;
            this.project = project;
            this.id = action.id;
            this.type = action.type || "unknown";
            this.enabled = !!action.enabled;
            this.modifiedDate = action.modifiedDate;
            this.name = action.name;
            this.component = ns.globalActionTypes.find(this.type);
        }, {
            showLinkDialog: function () {
                /// <signature>
                /// <summary>Displays the actionlink dialog.</summary>
                /// </signature>
                _PI.dialog({
                    name: "project.actionlink",
                    actionlink: { action: this }
                });
            }
        }),

        ActionListCommands: _Class.define(function ActionListCommands_ctor(list) {
            /// <signature>
            /// <summary>Initializes a new instance of the ActionListCommands class.</summary>
            /// <param name="list" type="Object" optional="true">The set of options to be applied initially to the ActionListCommands.</param>
            /// <returns type="PI.Project.ActionListCommands" />
            /// </signature>
            this._list = list;
        }, {
            "list.refresh": function () {
                /// <signature>
                /// <summary>Performs a list refresh operations.</summary>
                /// </signature>
                this._list.refresh();
            },
            "list.create": function () {
                /// <signature>
                /// <summary></summary>
                /// </signature>
                var project = this._list.filter.project;
                project && _Global.open(ns.ActionResources.createLink(project.id), "_self");
            },
            "selection.update": function () {
                /// <signature>
                /// <summary>Performs an update operations.</summary>
                /// </signature>
                var project = this._list.filter.project;
                var action = this._list.selection()[0];
                action && _Global.open(ns.ActionResources.updateLink(project.id, action.id), "_self");
            },
            "selection.copy": function () {
                /// <signature>
                /// <summary>Copies the current item.</summary>
                /// </signature>
                var action = this._list.selection()[0];
                action && this._list.service.copyBeforeAsync(action.id, null, this)
                    .then(function (d) {
                        var index = this._list.indexOf(action);
                        this._list.splice(index, 0, d);
                        this._list.selection([this._list.getAt(index)]);
                    });
            },
            "selection.delete": function () {
                /// <signature>
                /// <summary>Deletes the current item.</summary>
                /// </signature>
                var action = this._list.selection()[0];
                action && this._list.service.deleteAsync(action.id, this)
                    .then(function () {
                        this._list.remove(action);
                    });
            },
            "selection.link": function () {
                /// <signature>
                /// <summary>Shares the current item.</summary>
                /// </signature>
                var action = this._list.selection()[0];
                if (action) {
                    action.showLinkDialog();
                } else {
                    _PI.dialog({
                        name: "project.actionlink",
                        actionlink: { project: this._list.filter.project }
                    });
                }
            }
        }),

        ActionList: _Class.derive(_Knockout.PagedList, function ActionList_ctor(options) {
            /// <signature>
            /// <summary>Initializes a new instance of the ActionList class.</summary>
            /// <param name="options" type="Object" />
            /// <returns type="ActionList" />
            /// </signature>
            options = options || {};
            options.pageSize = options.pageSize || 50;
            _Knockout.PagedList.call(this, options);
            this._disposed = false;
            this.service = options.service || ns.ActionService;
            this.commands = new ns.ActionListCommands(this);
            this.viewMode = _observable(options.viewMode || "list"); // list | edit
            this.filter = new ns.ActionListFilter(this, options.filter);
            this.refresh();
        }, {
            mapItem: function (item) {
                /// <signature>
                /// <summary>Represents a function called before adding a new item to the list.</summary>
                /// <param name="item" type="Object">The item to map.</param>
                /// <returns type="PI.Project.ActionListItem" />
                /// </signature>
                return new ns.ActionListItem(this, this.filter.project, item);
            },
            refresh: _Promise.tasks.watch(function () {
                /// <signature>
                /// <summary>Refreshes the list.</summary>
                /// </signature>
                var params = this._createParams();
                return this.service.getAllAsync(params, this).then(
                    function (response) {
                        this.replaceAll.apply(this, response.data);
                        this.total = response.total;
                    },
                    function () {
                        this.removeAll();
                        this.total = 0;
                    });
            }),
            _createParams: function () {
                /// <signature>
                /// <returns type="Object" />
                /// </signature>
                var params = this.filter.toObject() || {};
                params.page = this.pageIndex;
                params.limit = this.pageSize;
                return params;
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
                _Knockout.PagedList.prototype.dispose.call(this);
                this._disposed = true;
            }
        }),

        ActionLink: _Class.define(function ActionLink_ctor(options) {
            /// <signature>
            /// <summary>Initializes a new instance of the ActionLink class.</summary>
            /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the ActionLink.</param>
            /// <returns type="ActionLink" />
            /// </signature>
            var that = this;
            this._disposed = false;
            this._listen = true;
            this._outputLinkHandled = false;

            this.options = options = options || {};
            this.displayMode = options.displayMode || "link"; // link | name
            this.service = options.service || ns.ActionService;
            this.project = options.project || options.action && options.action.project && { id: options.action.project.id };
            this.action = _observable(options.action);
            this.actionName = _pureComputed(this.getActionName, this);
            this.contacts = _observableArray(options.contact ? [options.contact] : []);
            this.customUri = _observable(options.customUri);
            this.outputLink = _KO.isWritableObservable(options.outputLink) ? options.outputLink : _observable(options.outputLink);
            this.outputMessage = _observable();

            this._actionSn = this.action.subscribe(this.generateAsync, this);
            this._contactsSn = this.contacts.subscribe(this.generateAsync, this);
            this._customUriSn = this.customUri.subscribe(this.generateAsync, this);
            this._outputLinkSn = this.outputLink.subscribe(this._outputLinkChanged, this);

            if (this.outputLink()) {
                this.loadByLink(this.outputLink());
            } else if (this.action()) {
                this.loadByIdAsync(this.action().id);
            }
        }, {
            loadByLink: function (link) {
                /// <signature>
                /// <returns type="_WinJS.Promise" />
                /// </signature>
                this.service.getByLinkAsync(link, this).then(
                    function (data) {
                        this._listen = false;
                        var contacts = [];
                        data.contact && contacts.push(data.contact);
                        this.project = data.action.project;
                        this.action(data.action);
                        this.contacts(contacts);
                        this.customUri(data.customUri);
                        this.setOutputLink(data.link);
                        this._listen = true;
                    },
                    function (error) {
                        this.outputMessage(error.message);
                    });
            },
            loadByIdAsync: function (id) {
                /// <signature>
                /// <returns type="_WinJS.Promise" />
                /// </signature>
                return this.service.getByIdAsync(id, this).then(
                    function (data) {
                        this._listen = false;
                        this.project = data.project;
                        this.action(data);
                        this.setOutputLink(data.link);
                        this._listen = true;
                    },
                    function (error) {
                        this.outputMessage(error.message);
                    });
            },
            generateAsync: function () {
                /// <signature>
                /// <summary>Generates an action link.</summary>
                /// <returns type="_WinJS.Promise" />
                /// </signature>
                if (!this._listen) {
                    return _Promise.error();
                }
                var actionlink = this.toObject();
                if (!actionlink.action) {
                    this.setOutputLink(null);
                    return _Promise.complete();
                }
                this.service.getLinkAsync(actionlink.action.id, {
                    contactId: actionlink.contact && actionlink.contact.id,
                    customUri: actionlink.customUri
                }, this).then(
                    function (link) {
                        this.setOutputLink(link);
                    },
                    function (error) {
                        this.setOutputLink(null);
                        this.outputMessage(error.message);
                    });
            },
            getActionName: function () {
                /// <signature>
                /// <returns type="String" />
                /// </signature>
                var action = this.action();
                return action && action.name || "";
            },
            setOutputLink: function (link) {
                /// <signature>
                /// <summary>Sets the output link without making an extra HTTP request.</summary>
                /// <param name="link" type="String" />
                /// </signature>
                this._outputLinkHandled = true;
                this.outputLink(link);
                this.outputMessage(null);
                this._outputLinkHandled = false;
            },
            toObject: function () {
                /// <signature>
                /// <returns type="Object" />
                /// </signature>
                return {
                    action: this.action(),
                    contact: this.contacts()[0],
                    customUri: this.customUri(),
                    outputLink: this.outputLink()
                };
            },
            _outputLinkChanged: function (value) {
                /// <signature>
                /// <param name="value" type="String" />
                /// </signature>
                if (this._outputLinkHandled) {
                    return;
                }
                if (value) {
                    this.loadByLink(value);
                } else {
                    this.action(null);
                }
            },
            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                if (this._disposed) {
                    return;
                }
                this.actionName && this.actionName.dispose();
                this.actionName = null;
                this._outputLinkSn && this._outputLinkSn.dispose();
                this._outputLinkSn = null;
                this._actionSn && this._actionSn.dispose();
                this._actionSn = null;
                this._contactsSn && this._contactsSn.dispose();
                this._contactsSn = null;
                this._customUriSn && this._customUriSn.dispose();
                this._customUriSn = null;
                this._disposed = true;
            }
        }),

        ActionLinkNode: _Class.define(function ActionLinkNode_ctor(options) {
            /// <signature>
            /// <summary>Initializes a new instance of the ActionLinkNode class.</summary>
            /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the ActionLinkNode.</param>
            /// <returns type="ActionLinkNode" />
            /// </signature>
            options = options || {};
            this.className = _observable(options.className);
            this.href = options.href;
            this.title = options.title;
            this.text = options.text;
            this.actionlink = new ns.ActionLink(options.actionlink);
        }, {
            toObject: function () {
                /// <signature>
                /// <returns type="Object" />
                /// </signature>
                return {
                    className: this.className(),
                    href: this.href,
                    title: this.title,
                    text: this.text,
                    actionlink: this.actionlink.toObject()
                };
            },
            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                this.actionlink && this.actionlink.dispose();
                this.actionlink = null;
            }
        })

    });

    _Class.mix(ns.Action, _Utilities.createEventProperties("loaded", "saved", "copied", "moved", "deleted"));
    _Class.mix(ns.Action, _Utilities.eventMixin);

    //
    // Exposes a _Global container that can be used to register action types
    //

    ns.globalActionTypes = new ns.ActionTypeList();

})(window, ko, WinJS, PI);