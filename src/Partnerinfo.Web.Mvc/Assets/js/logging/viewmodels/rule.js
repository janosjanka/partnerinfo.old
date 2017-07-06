// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="../services/rule.js" />

(function (_KO, _WinJS, _PI) {
    "use strict";

    var _Class = _WinJS.Class;
    var _Utilities = _WinJS.Utilities;
    var _Promise = _WinJS.Promise;
    var _Resources = _WinJS.Resources;
    var _Knockout = _WinJS.Knockout;

    var _observable = _KO.observable;
    var _observableArray = _KO.observableArray;
    var _pureComputed = _KO.pureComputed;
    var _validation = _KO.validation;
    var _editSession = _KO.editSession;

    var ns = _WinJS.Namespace.defineWithParent(_PI, "Logging", {

        RuleConditionCode: {
            uknown: "unknown",
            startDateGreaterThan: "startDateGreaterThan",
            startDateLessThan: "startDateLessThan",
            clientIdContains: "clientIdContains",
            customUriContains: "customUriContains",
            referrerUrlContains: "referrerUrlContains",
            projectIdEquals: "projectIdEquals",
            contactStateEquals: "contactStateEquals",
            contactMailContains: "contactMailContains"
        },

        RuleConditionItem: _Class.define(function (ruleCondition) {
            /// <signature>
            /// <param name="ruleCondition" type="Object" optional="true" />
            /// <returns type="PI.Logging.RuleConditionItem" />
            /// </signature>
            this.code = ruleCondition.code;
            this.value = ruleCondition.value;
        }, {
            toObject: function () {
                /// <signature>
                /// <summary>Returns a native JS object without observable members.</summary>
                /// <returns type="Object" />
                /// </signature>
                return {
                    code: this.code,
                    value: this.value
                };
            }
        }),

        RuleActionCode: {
            uknown: "unknown",
            remove: "remove",
            categorize: "categorize"
        },

        RuleActionItem: _Class.define(function (ruleAction) {
            /// <signature>
            /// <param name="ruleAction" type="Object" optional="true" />
            /// <returns type="PI.Logging.RuleActionItem" />
            /// </signature>
            this.code = ruleAction.code;
            this.value = ruleAction.value;
        }, {
            toObject: function () {
                /// <signature>
                /// <summary>Returns a native JS object without observable members.</summary>
                /// <returns type="Object" />
                /// </signature>
                return {
                    code: this.code,
                    value: this.value
                };
            }
        }),

        RuleItem: _Class.define(function RuleItem_ctor(rule, options) {
            /// <signature>
            /// <param name="rule" type="Object" optional="true" />
            /// <param name="options" type="Object" optional="true" />
            /// <returns type="PI.Logging.RuleItem" />
            /// </signature>
            _Utilities.setOptions(this, options = options || {});
            _Promise.tasks(this);

            this.autoSave = options.autoSave !== false;
            this.service = options.service || ns.RuleService;

            this.id = _observable();
            this.conditions = _observableArray();
            this.conditionsView = this.conditions.map(function (condition) { return new ns.RuleConditionItem(condition); });
            this.actions = _observableArray();
            this.actionsView = this.actions.map(function (action) { return new ns.RuleActionItem(action); });

            this.errors = _KO.validation.group(this);
            this.editing = _observable(false);
            this.exists = _pureComputed(this._exists, this);

            this.update(rule);
        }, {
            validate: function () {
                /// <signature>
                /// <summary>Returns true if this rule is valid.</summary>
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
                this._editSession = new _KO.editSession(this, { fields: ["conditions", "actions"] });
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
                    this.saveAsync().then(function () {
                        this._editSession = null;
                        this.editing(false);
                    });
                } else {
                    this._editSession = null;
                    this.editing(false);
                }
            },

            //
            // Data Operations
            //

            update: function (rule) {
                /// <signature>
                /// <param name="rule" type="Object" />
                /// </signature>
                rule = rule || {};
                this.id(rule.id);
                this.conditions(rule.conditions || []);
                this.actions(rule.actions || []);
            },
            toObject: function () {
                /// <signature>
                /// <returns type="Object" />
                /// </signature>
                return {
                    id: this.id(),
                    conditions: this.conditionsView().map(function (condition) { return condition.toObject(); }),
                    actions: this.actionsView().map(function (action) { return action.toObject(); })
                };
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
                        .then(function (rule) {
                            this.update(rule);
                            this.dispatchEvent("loaded", rule);
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
                var rule = this.toObject();
                if (this.exists()) {
                    return this.service.updateAsync(rule.id, rule, this)
                        .then(function (rule) {
                            this.update(rule);
                            this.dispatchEvent("saved", rule);
                        });
                }
                return this.service.createAsync(rule, this)
                    .then(function (rule) {
                        this.update(rule);
                        this.dispatchEvent("saved", rule);
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
            discard: function () {
                /// <signature>
                /// <summary>Discards data changes and raises an ondiscarded event that can be handled by views.</summary>
                /// </signature>
                this.cancelEdit();
                this.dispatchEvent("discarded");
            },

            //
            // Event & Computed Members
            //

            _exists: function () {
                /// <signature>
                /// <returns type="Boolean" />
                /// </signature>
                return !!this.id();
            },
            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                this.errors && this.errors.dispose();
                this.errors = null;
                this.exists && this.exists.dispose();
                this.exists = null;
            }
        }),

        RuleList: _Class.derive(_Knockout.List, function RuleList_ctor(options) {
            /// <signature>
            /// <summary>Initializes a new instance of the RuleList class.</summary>
            /// <param name="options" type="Object" />
            /// <returns type="PI.Logging.RuleList" />
            /// </signature>
            options = options || {};
            options.autoLoad = options.autoLoad !== false;
            _Knockout.List.call(this, options);

            this._disposed = false;
            this._itemDeletedBound = this._itemDeleted.bind(this);
            this.service = options.service || ns.RuleService;
            options.autoLoad && this.refresh();
        }, {
            mapItem: function (item) {
                /// <signature>
                /// <param name="item" type="Object" />
                /// <returns type="PI.Logging.RuleItem" />
                /// </signature>
                var rule = new ns.RuleItem(item);
                rule.addEventListener("deleted", this._itemDeletedBound, false);
                return rule;
            },
            refresh: function () {
                /// <signature>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return this.service.getAllAsync({}, this).then(
                    function (response) {
                        this.replaceAll.apply(this, response.data);
                        this.total = response.total;
                    },
                    function () {
                        this.removeAll();
                        this.total = 0;
                    });
            },
            _itemDeleted: function (event) {
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
                _Knockout.List.prototype.dispose.call(this);
                this._disposed = true;
            }
        })

    });

    _Class.mix(ns.RuleItem, _Utilities.createEventProperties("loaded", "saved", "deleted", "discarded"));
    _Class.mix(ns.RuleItem, _Utilities.eventMixin);

})(ko, WinJS, PI);