// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (_WinJS, ko, undefined) {
    "use strict";

    var _Class = _WinJS.Class;
    var _Utilities = _WinJS.Utilities;
    var _Promise = _WinJS.Promise;

    var _observable = ko.observable;
    var _observableArray = ko.observableArray;
    var _pureComputed = ko.pureComputed;

    var ns = _WinJS.Namespace.defineWithParent(_WinJS, "Knockout", {
        Editable: _Class.define(function Editable_ctor(options) {
            /// <signature>
            /// <summary>Provides functionality to commit or rollback changes to an object that is used as a data source.</summary>
            /// <param name="options" type="Object" optional="true">A set of key/value pairs that can be used to configure object options.
            ///     <para>item: Object - The editable object that can be mapped to a KO object.</para>
            ///     <para>mapping?: Object - An object that defines KO mapping rules. If set to null, and is not "undefined", the item and its properties are not going to be editable by data binding.</para>
            /// </param>
            /// <returns type="WinJS.Knockout.Editable" />
            /// </signature>
            this.options = options = options || {};
            _Utilities.setOptions(this, options);
            _Promise.tasks(this);

            this._disposed = false;
            this._editSession = null;
            this._errors = _observableArray();

            this.item = null;
            this.mapping = options.mapping; // KO mapping, e.g. { ignore: ["children"] }
            this.mapItem(options.item, this.mapping); // Maps the item to a new KO object.
            this.isEditing = _observable(false); // true if this item is being edited
            this.hasErrors = _pureComputed(this._hasErrors, this); // true if the entity currently has validation errors
            this.errors = _pureComputed(this._getErrors, this); // active error objects on the bound element
            this.validationErrors = ko.validation ? ko.validation.group(this.item, options.validationOptions || { deep: true }) : _WinJS.noop;
        }, {
            setOption: function (key, value) {
                /// <signature>
                /// <summary>Updates the value of the specified property.</summary>
                /// <param name="key" type="String">The name of the property.</param>
                /// <param name="value" type="Object" optional="true">The value to update.</param>
                /// </signature>
                if (key === "validation") {
                    throw new Error("validation cannot be changed after initialization");
                }
                this[key] = value;
                var updateModel = false;
                if (key === "mapping") {
                    this.mapping = value;
                    updateModel = true;
                } else if (key === "item") {
                    updateModel = true;
                }
                if (updateModel) {
                    this.mapItem(this.options.item, this.mapping);
                }
                return this;
            },

            //
            // Mapping
            //

            mapItem: function (item, mapping) {
                /// <signature>
                /// <summary>Maps the current item to a new object.</summary>
                /// <param name="item" type="Object" />
                /// <param name="mapping" type="Object" optional="true" />
                /// </signature>
                if (mapping === null) {
                    this.item = item; // mapping = null means nothing
                    return;
                }
                var params = mapping === undefined ? [item] : [item, mapping];
                if (this.item === null) {
                    this.item = ko.mapping.fromJS.apply(ko.mapping, params);
                } else {
                    params.push(this.item);
                    ko.mapping.fromJS.apply(ko.mapping, params);
                }
            },
            setItem: function (item) {
                /// <signature>
                /// <summary>Replaces the item.</summary>
                /// <param name="item" type="Object">The item.</param>
                /// <returns type="Object" />
                /// </signature>
                this.mapItem(item, this.mapping);
            },

            //
            // Edit Session
            //

            beginEdit: function () {
                /// <signature>
                /// <summary>Begins an edit on an object.</summary>
                /// </signature>
                if (!this.isEditing() && this.item) {
                    this._editSession = ko.editSession(this.item);
                    this.isEditing(true);
                }
            },
            cancelEdit: function () {
                /// <signature>
                /// <summary>Discards changes since the last BeginEdit call.</summary>
                /// </signature>
                if (this.isEditing()) {
                    if (this._editSession) {
                        this._editSession.cancel();
                        this._editSession = null;
                    }
                    this.isEditing(false);
                }
            },
            endEdit: function () {
                /// <signature>
                /// <summary>Pushes changes since the last beginEdit.</summary>
                /// </signature>
                if (this.isEditing()) {
                    this._editSession = null;
                    this.isEditing(false);
                }
            },

            //
            // Validation
            //

            _hasErrors: function () {
                /// <signature>
                /// <summary>Gets a value indicating whether the entity has validation errors. </summary>
                /// <returns>true if the entity currently has validation errors; otherwise, false.</returns>
                /// <returns type="Boolean" />
                /// </signature>
                return this.validationErrors && this.validationErrors().length > 0
                    || this._errors().length > 0;
            },
            _getErrors: function () {
                /// <signature>
                /// <summary>Gets all error messages merging both validation mechanism</summary>
                /// <returns type="Array" />
                /// </signature>
                return this.validationErrors
                    ? this._errors().concat(this.validationErrors())
                    : this._errors();
            },
            validate: function () {
                /// <signature>
                /// <summary>Validates the current object.</summary>
                /// <returns type="Boolean" />
                /// </signature>
                if (this._disposed) {
                    return false;
                }
                this.clearErrors();
                return this.errors().length === 0;
            },
            addError: function (message, isWarning) {
                /// <signature>
                /// <summary>Adds the specified error to the errors collection if it is not already present,
                /// inserting it in the first position if isWarning is false.</summary>
                /// <param name="message" type="String">The error message.</param>
                /// <param name="isWarning" type="Boolean" optional="true" value="false">A value indicating whether the message is just a warning message.</param>
                /// </signature>
                if (this._errors.indexOf(message) < 0) {
                    if (isWarning === true) {
                        this._errors.push(message);
                    } else {
                        this._errors.splice(0, 0, message);
                    }
                }
            },
            clearErrors: function () {
                /// <signature>
                /// <summary>Removes all validation error messages from the errors collection.</summary>
                /// </signature>
                this._errors.removeAll();
            },

            //
            // Serialization
            //

            toObject: function (options) {
                /// <signature>
                /// <summary>Serializes the current KO object to native object.</summary>
                /// <param name="options" type="Object" optional="true">A set of key/value pairs that can be used to configure knockout.js mapping plugin.</param>
                /// <returns type="Object" />
                /// </signature>
                return ko.mapping.toJS(this.item, options || this.mapping);
            },

            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                if (this._disposed) {
                    return;
                }
                this.hasErrors && this.hasErrors.dispose();
                this.hasErrors = null;
                this.errors && this.errors.dispose();
                this.errors = null;
                this.validationErrors && this.validationErrors.dispose();
                this.validationErrors = null;
                this._editSession = null;
                this._disposed = true;
            }
        }),

        Filter: _Class.define(function Filter_ctor(options) {
            /// <signature>
            /// <summary>An object that can be used to filter data.</summary>
            /// <returns type="WinJS.Knockout.Filter" />
            /// </signature>
            this.options = options = options || {};
            options.ignoreNull = options.ignoreNull === undefined || options.ignoreNull;
            _Utilities.setOptions(this, options);
            this.changed = this.options.changed;
        }, {
            _deferRefresh: 0,
            isProperty: function (property) {
                /// <summary>Gets a value indicating whether the specified property is serializable.</summary>
                /// <param name="property" type="String">The name of the property.</param>
                /// <returns type="Boolean" />
                if (!property || property.charAt(0) === '_' ||
                    property === "options" || property === "changed") {
                    return false;
                }
                return this.hasOwnProperty(property);
            },
            isRequired: function (property) {
                /// <summary>Gets a value indicating whether the property is required.</summary>
                /// <param name="property" type="String">The property.</param>
                /// <returns type="Boolean" />
                var required = this.options.required;
                if (required) {
                    return required.indexOf(property) >= 0;
                }
                return false;
            },
            getValue: function (property, unwrapValue) {
                /// <signature>
                /// <summary>Gets a filter proerty.</summary>
                /// <param name="property" type="String">The property name.</param>
                /// <param name="unwrapValue" type="Boolean" />
                /// <returns type="Boolean" />
                /// </signature>
                return unwrapValue !== false ? ko.unwrap(this[property]) : this[property];
            },
            setValue: function (propName, newValue) {
                /// <signature>
                /// <summary>Resets the specified property.</summary>
                /// <param name="property" type="String">The name of the property.</param>
                /// </signature>
                /// <signature>
                /// <summary>Sets the specified property.</summary>
                /// <param name="property" type="String">The name of the property.</param>
                /// <param name="value" type="Object">The value of the property.</param>
                /// </signature>
                if (!newValue && this.isRequired(propName)) {
                    return this;
                }
                var property = this[propName];
                if (ko.isWriteableObservable(property)) {
                    property(newValue || (typeof property.push === "function" ? [] : null));
                } else if (Array.isArray(property)) {
                    this[propName] = newValue || [];
                } else {
                    this[propName] = newValue || null;
                }
                return this;
            },
            refresh: function () {
                /// <signature>
                /// <summary>Refreshes the dataset.</summary>
                /// </signature>
                if (!this._deferRefresh &&
                    typeof this.changed === "function") {
                    this.changed();
                }
            },
            deferRefresh: function (callback, suppressOnChange) {
                /// <signature>
                /// <summary>Enters a defer cycle that you can use to merge changes to the view and delay automatic refresh.</summary>
                /// <param name="callback" type="Function">A callback function that can be called when enters a defer cycle. 
                /// The callback function will be executed in the context of the filter object.</param>
                /// </signature>
                ++this._deferRefresh;
                try {
                    callback.apply(this, [this]);
                } finally {
                    if (!--this._deferRefresh) {
                        if (suppressOnChange === true) {
                            return;
                        }
                        this.refresh();
                    }
                }
            },
            clear: function (suppressOnChange) {
                /// <signature>
                /// <summary>Removes all filter conditions.</summary>
                /// </signature>
                this.deferRefresh(function () {
                    for (var p in this) {
                        if (this.isProperty(p)) {
                            this.setValue(p);
                        }
                    }
                }, suppressOnChange === true);
            },
            monitorEnter: function () {
                /// <signature>
                /// <summary>Attaches new monitored properties to the changed event handler.</summary>
                /// <param name="excludedProperties" type="params" optional="true">An array of excluded properties.</param>
                /// </signature>
                this.monitorExit();
                this._subscr = [];
                var args = Array.prototype.slice.apply(arguments, []);
                for (var p in this) {
                    if (args.indexOf(p) < 0 && this.isProperty(p)) {
                        var v = this[p];
                        if (ko.isObservable(v)) {
                            this._subscr.push(v.subscribe(this.refresh, this));
                        }
                    }
                }
            },
            monitorExit: function () {
                /// <signature>
                /// <summary>Detaches all monitored properties.</summary>
                /// </signature>
                if (this._subscr) {
                    for (var i = this._subscr.length; --i >= 0;) {
                        this._subscr[i].dispose();
                    }
                    this._subscr = null;
                }
            },
            toObject: function (mapper) {
                /// <signature>
                /// <summary>Serializes the current KO object to native object.</summary>
                /// <param name="mapper" type="Function" optional="true">A callback function that can be used to map property values.</param>
                /// <returns type="Object" />
                /// </signature>
                mapper = mapper || this.options.mapper;
                var f = {};
                var ism = typeof mapper === "function";
                for (var p in this) {
                    if (this.isProperty(p)) {
                        var v = ko.unwrap(this[p]);
                        if (ism) {
                            v = mapper(p, v);
                        }
                        if (v === undefined || v === null && this.options.ignoreNull) {
                            continue;
                        }
                        f[p] = v;
                    }
                }
                return f;
            }
        })
    });

    _Class.mix(ns.Editable, _Utilities.eventMixin);

})(WinJS, ko);
