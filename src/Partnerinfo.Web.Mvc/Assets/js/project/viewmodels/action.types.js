// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="action.js" />

(function (_KO, _WinJS, _PI, _Project) {
    "use strict";

    var _Class = _WinJS.Class;
    var _Promise = _WinJS.Promise;
    var _Resources = _WinJS.Resources;

    var _observable = _KO.observable;
    var _observableArray = _KO.observableArray;
    var _validation = _KO.validation;

    function asArray(value) {
        /// <signature>
        /// <summary>Converts the given value to an array.</summary>
        /// <param name="value" type="Object" />
        /// <returns type="Array" />
        /// </signature>
        return Array.isArray(value) ? value : value ? [value] : [];
    }

    function asNumber(value) {
        /// <signature>
        /// <summary>Converts the given value to a number.</summary>
        /// <param name="value" type="Object" />
        /// <returns type="Number" />
        /// </signature>
        return value === undefined ? undefined : +value;
    }

    var ns = _WinJS.Namespace.defineWithParent(_PI, "Project.Actions", {

        ActionResources: {
            actionTypeRedirectUrlRequired: function () { return _T("pi/project/actionTypeRedirectUrlRequired"); }
        },

        RedirectAction: _Class.derive(_Project.ActionOptionsBase, function (action) {
            /// <signature>
            /// <param name="action" type="PI.Project.Action" />
            /// <returns type="Object" />
            /// </signature>
            _Project.ActionOptionsBase.call(this, action);
            this.url = _observable(this.options.url).extend({ required: { message: ns.ActionResources.actionTypeRedirectUrlRequired() }, maxLength: 512 });
        }, {
            toObject: function () {
                /// <signature>
                /// <returns type="Object" />
                /// </signature>
                return {
                    url: this.url()
                };
            }
        }, {
            getInfo: function (action, typeInfo) {
                /// <signature>
                /// <returns type="String" />
                /// </signature>
                var options = action.options() || {};
                return {
                    name: options.url,
                    link: options.url
                };
            }
        }),

        ScheduleAction: _Class.derive(_Project.ActionOptionsBase, function (action) {
            /// <signature>
            /// <param name="action" type="PI.Project.Action" />
            /// <returns type="Object" />
            /// </signature>
            _Project.ActionOptionsBase.call(this, action);
            this.startDate = _observable(this.options.startDate);
            this.offsetTime = _observable(this.options.offsetTime);
        }, {
            toObject: function () {
                /// <signature>
                /// <returns type="Object" />
                /// </signature>
                return {
                    startDate: this.startDate(),
                    offsetTime: this.offsetTime()
                };
            }
        }, {
            getInfo: function (action, typeInfo) {
                /// <signature>
                /// <returns type="String" />
                /// </signature>
                var options = action.options() || {};
                var help = "Kezdés időpontja: ";
                help += options.startDate ? _Resources.format(new Date(options.startDate), "F") : "Most";
                if (options.offsetTime) {
                    help += " + Relatív időeltolás: ";
                    help += options.offsetTime;
                }
                return {
                    help: help
                };
            }
        }),

        ConditionAction: _Class.derive(_Project.ActionOptionsBase, function (action) {
            /// <signature>
            /// <param name="action" type="PI.Project.Action" />
            /// <returns type="Object" />
            /// </signature>
            _Project.ActionOptionsBase.call(this, action);
            this.types = [{
                name: "dateGreaterThanOrEqualTo",
                normalizedName: "DateGreaterThanOrEqualTo",
                displayName: _T("pi/project/actionTypeConditionDateGreaterThanOrEqualTo"),
                viewModel: ns.ConditionAction.ConditionActionDateItem
            }, {
                name: "dateLessThanOrEqualTo",
                normalizedName: "DateLessThanOrEqualTo",
                displayName: _T("pi/project/actionTypeConditionDateLessThanOrEqualTo"),
                viewModel: ns.ConditionAction.ConditionActionDateItem
            }, {
                name: "authenticated",
                normalizedName: "Authenticated",
                displayName: _T("pi/project/actionTypeConditionAuthenticated"),
                viewModel: ns.ConditionAction.ConditionActionCheckedItem
            }, {
                name: "contactExists",
                normalizedName: "ContactExists",
                displayName: _T("pi/project/actionTypeConditionContactExists"),
                viewModel: ns.ConditionAction.ConditionActionCheckedItem
            }, {
                name: "contactWithTag",
                normalizedName: "ContactWithTag",
                displayName: _T("pi/project/actionTypeConditionContactWithTag"),
                viewModel: ns.ConditionAction.ConditionActionTagItem
            }, {
                name: "contactWithoutTag",
                normalizedName: "ContactWithoutTag",
                displayName: _T("pi/project/actionTypeConditionContactWithoutTag"),
                viewModel: ns.ConditionAction.ConditionActionTagItem
            }];
            this.conditions = _observableArray().extend({ required: { message: "Legalább egy feltételre szükség van." } });
            asArray(this.options.conditions).forEach(function (c) { this.addCondition(c.type, c.value); }, this);
        }, {
            findType: function (name) {
                /// <signature>
                /// <param name="name" type="String" />
                /// <returns type="Object" />
                /// </signature>
                return this.types.find(function (t) {
                    return t.name === name;
                });
            },
            addCondition: function (typeName, value) {
                /// <signature>
                /// <param name="typeName" type="String" />
                /// <param name="value" type="String" />
                /// </signature>
                var type = this.findType(typeName);
                type && type.viewModel && this.conditions.push({
                    type: type,
                    value: new type.viewModel(this, value)
                });
            },
            moveCondition: function (oldIndex, newIndex) {
                /// <signature>
                /// <summary>Moves the given condition.</summary>
                /// <param name="oldIndex" type="Number" />
                /// <param name="newIndex" type="Number" />
                /// </signature>
                var conditions = this.conditions();
                if (oldIndex < 0 || newIndex < 0 ||
                    oldIndex >= conditions.length ||
                    newIndex >= conditions.length) {
                    return;
                }
                this.conditions.valueWillMutate();
                conditions.splice(newIndex, 0, conditions.splice(oldIndex, 1)[0]);
                this.conditions.valueHasMutated();
            },
            toObject: function () {
                /// <signature>
                /// <returns type="Object" />
                /// </signature>
                return {
                    conditions: this.conditions().map(function (c) {
                        return {
                            type: c.type.name,
                            value: c.value.toObject()
                        };
                    })
                };
            }
        }, {
            ConditionActionDateItem: _Class.define(function (condition, value) {
                /// <signature>
                /// <returns type="Object" />
                /// </signature>
                this.datetime = _observable(value ? new Date(value) : new Date());
            }, {
                toObject: function () {
                    return this.datetime();
                }
            }),
            ConditionActionCheckedItem: _Class.define(function (condition, value) {
                /// <signature>
                /// <returns type="Object" />
                /// </signature>
                this.checked = _observable(!!value);
            }, {
                toObject: function () {
                    return this.checked();
                }
            }),
            ConditionActionTagItem: _Class.define(function (condition, value) {
                /// <signature>
                /// <returns type="Object" />
                /// </signature>
                var that = this;
                var valueAsNumber = asNumber(value);
                this.item = _observable();
                this.list = ns.ConditionAction.ConditionActionTagItem._createList(condition.action.project);
                this.list.then(function () {
                    that.item(that.list.find(function (t) {
                        return _KO.unwrap(t.id) === valueAsNumber;
                    }) || { id: null, color: null, name: "Kiválasztás..." });
                });
            }, {
                toObject: function () {
                    var item = this.item();
                    return item && _KO.unwrap(item.id);
                }
            }, {
                _lists: {},
                _createList: function (project) {
                    /// <signature>
                    /// <summary>This function creates a cached version from the tag list per project.</summary>
                    /// <param name="project" type="Object" />
                    /// <returns type="PI.Project.BusinessTagList" />
                    /// </signature>
                    return ns.ConditionAction.ConditionActionTagItem._lists[project.id] ||
                        (ns.ConditionAction.ConditionActionTagItem._lists[project.id] =
                            new _Project.BusinessTagList({ filter: { project: project } }));
                }
            })
        }),

        AuthenticateAction: _Class.derive(_Project.ActionOptionsBase, function (action) {
            /// <signature>
            /// <param name="action" type="PI.Project.Action" />
            /// <returns type="Object" />
            /// </signature>
            _Project.ActionOptionsBase.call(this, action);
            this.checkPassword = _observable(!!this.options.checkPassword);
        }, {
            toObject: function () {
                /// <signature>
                /// <returns type="Object" />
                /// </signature>
                return {
                    checkPassword: this.checkPassword()
                };
            }
        }),

        SetTagsAction: _Class.derive(_Project.ActionOptionsBase, function (action) {
            /// <signature>
            /// <param name="action" type="PI.Project.Action" />
            /// <returns type="Object" />
            /// </signature>
            var that = this;
            _Project.ActionOptionsBase.call(this, action);
            this.includeWithTags = asArray(this.options.include).map(function (id) { return { id: asNumber(id) }; });
            this.excludeWithTags = asArray(this.options.exclude).map(function (id) { return { id: asNumber(id) }; });
            this._validatedTags = _observable().extend({
                required: {
                    onlyIf: function () {
                        return (that.includeWithTags.length | that.excludeWithTags.length) === 0;
                    },
                    message: "Legalább egy címkézési szabályra szükség van."
                }
            });
        }, {
            update: function (includeWithTags, excludeWithTags) {
                /// <signature>
                /// <param name="includeWithTags" type="Array" />
                /// <param name="excludeWithTags" type="Array" />
                /// </signature>
                this.includeWithTags = includeWithTags || [];
                this.excludeWithTags = excludeWithTags || [];
                this._validatedTags.valueHasMutated();
            },
            toObject: function () {
                /// <signature>
                /// <returns type="Object" />
                /// </signature>
                return {
                    include: this.includeWithTags.map(function (t) { return t.id; }),
                    exclude: this.excludeWithTags.map(function (t) { return t.id; })
                };
            }
        }),

        SendMailAction: _Class.derive(_Project.ActionOptionsBase, function (action) {
            /// <signature>
            /// <param name="action" type="PI.Project.Action" />
            /// <returns type="Object" />
            /// </signature>
            _Project.ActionOptionsBase.call(this, action);
            this.mailMessage = _observable(this.options.id ? { id: this.options.id, subject: null } : null).extend({ required: { message: "Egy levelet meg kell adni." } });
        }, {
            toObject: function () {
                /// <signature>
                /// <returns type="Object" />
                /// </signature>
                var mailMessage = this.mailMessage();
                return {
                    id: mailMessage && mailMessage.id
                };
            }
        }),

        LogAction: _Class.derive(_Project.ActionOptionsBase, function (action) {
            /// <signature>
            /// <param name="action" type="PI.Project.Action" />
            /// <returns type="Object" />
            /// </signature>
            _Project.ActionOptionsBase.call(this, action);
            this.anonymous = _observable(!!this.options.anonymous);
        }, {
            toObject: function () {
                /// <signature>
                /// <returns type="Object" />
                /// </signature>
                return {
                    anonymous: this.anonymous()
                };
            }
        })

    });

    _Project.globalActionTypes.registerAll({
        name: "redirect",
        color: "#10c",
        children: false,
        type: ns.RedirectAction
    }, {
        name: "sequence",
        color: "#fff",
        children: true
    }, {
        name: "schedule",
        color: "#33cccc",
        children: true,
        type: ns.ScheduleAction
    }, {
        name: "condition",
        color: "#33cc33",
        children: true,
        type: ns.ConditionAction
    }, {
        name: "authenticate",
        color: "#2aff00",
        children: false,
        type: ns.AuthenticateAction
    }, {
        name: "register",
        color: "#ff0000",
        children: false
    }, {
        name: "unregister",
        color: "#009aff",
        children: false
    }, {
        name: "setTags",
        color: "#ffd800",
        children: false,
        type: ns.SetTagsAction
    }, {
        name: "sendMail",
        color: "#ba75ff",
        children: false,
        type: ns.SendMailAction
    }, {
        name: "log",
        color: "#ccc",
        children: false,
        type: ns.LogAction
    });

})(ko, WinJS, PI, PI.Project);
