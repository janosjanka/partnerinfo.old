// Knockout EditSession plugin v1.0
// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (ko) {
    "use strict";

    function EditSession(target, options) {
        /// <signature>
        /// <summary>Provides functionality to commit or rollback changes to an object that is used as a data source.</summary>
        /// <param name="target" type="Object" />
        /// <param name="options" type="Object" optional="true">
        ///     <para>fields: Array - An array of properties to monitor.</para>
        /// </param>
        /// <returns type="EditSession" />
        /// </signature>
        options = options || {};
        this._disposed = false;
        this._target = target;
        this._snapshot = null;
        this._fields = options.fields || this._getFieldNames();
        this.update();
    }
    EditSession.prototype.hasChanged = function () {
        /// <signature>
        /// <summary>Returns true if this item is changed.</summary>
        /// <returns type="Boolean" />
        /// </signature>
        if (!this._snapshot) {
            return false;
        }
        for (var i = 0, len = this._fields.length; i < len; ++i) {
            var field = this._fields[i];
            if (this._snapshot[field] !== ko.unwrap(this._target[field])) {
                return true;
            }
        }
        return false;
    };
    EditSession.prototype.update = function () {
        /// <signature>
        /// <summary>Saves the current state of the target object.</summary>
        /// </signature>
        this._snapshot = this._createSnapshot();
    };
    EditSession.prototype.cancel = function () {
        /// <signature>
        /// <summary>Reverts all changes made to the target object.</summary>
        /// </signature>
        if (!this._snapshot) {
            return;
        }
        for (var i = 0, len = this._fields.length; i < len; ++i) {
            var field = this._fields[i];
            var value = this._target[field];
            if (ko.isWritableObservable(value)) {
                value(this._snapshot[field]);
            } else {
                this._target[field] = this._snapshot[field];
            }
        }
    };
    EditSession.prototype._createSnapshot = function () {
        /// <signature>
        /// <returns type="Object" />
        /// </signature>
        var snapshot = {};
        for (var i = 0, len = this._fields.length; i < len; ++i) {
            var field = this._fields[i];
            snapshot[field] = ko.unwrap(this._target[field]);
        }
        return snapshot;
    };
    EditSession.prototype._getFieldNames = function () {
        /// <signature>
        /// <returns type="Array" />
        /// </signature>
        var fields = [];
        for (var field in this._target) {
            if (this._target.hasOwnProperty(field) &&
                ko.isWritableObservable(this._target[field])) {
                fields.push(field);
            }
        }
        return fields;
    };

    ko.editSession = function (target, options) {
        /// <signature>
        /// <summary>Provides functionality to commit or rollback changes to an object that is used as a data source.</summary>
        /// <param name="target" type="Object" />
        /// <param name="options" type="Object" optional="true">
        ///     <para>fields: Array - An array of properties to monitor.</para>
        /// </param>
        /// <returns type="EditSession" />
        /// </signature>
        return new EditSession(target, options);
    };

})(ko);