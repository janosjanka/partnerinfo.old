// Copyright (c) Partnerinfo Ltd. All Rights Reserved.
/// <reference path="../base.js" />
/// <reference path="../edit.js" />

(function (_WinJS, _Project, _Portal) {
    "use strict";

    var _Promise = _WinJS.Promise;

    function asNumber(value) {
        /// <signature>
        /// <summary>Converts a value to valid number.</summary>
        /// <param name="value" type="Object" />
        /// <returns type="Number" />
        /// </signature>
        return value === undefined ? undefined : +value;
    }

    $.widget("PI.PortalEventModule", $.PI.PortalModule, {
        options: {
            typeClass: "ui-module-event",
            typeName: "event"
        },
        _activate: function () {
            /// <signature>
            /// <returns type="WinJS.Promise" />
            /// </signature>
            this.executeAll();
        },
        _deactivate: function () {
            /// <signature>
            /// <returns type="WinJS.Promise" />
            /// </signature>
            $("*").unbind(".ui-module-event-" + this.uuid);
        },
        createModuleOptions: function (/* options */) {
            /// <signature>
            /// <summary>Extends default module options with the specified module options.</summary>
            /// <returns type="Object" />
            /// </signature>
            /// <signature>
            /// <summary>Extends default module options with the specified module options.</summary>
            /// <param name="options" type="Object">A set of key/value pairs that can be used to configure module options.</param>
            /// <returns type="Object" />
            /// </signature>
            var o = arguments[0] || {};
            if ($.isArray(o)) {
                return this._super({
                    events: o // Backward compatibility.
                });
            }
            return this._super(o);
        },
        authorize: function () {
            /// <signature>
            /// <summary>Sets permissions</summary>
            /// </signature>
            this._super();
            if (this._isInDesignTime) {
                return;
            }
            this.executeByEvent(this.isAuthenticated() ? "login" : "logout");
        },
        findEvents: function () {
            /// <signature>
            /// <summary>Gets a collection of events from the module element..</summary>
            /// <returns type="Array" />
            /// </signature>
            return this.getModuleOptions().events || [];
        },
        findEvent: function (name) {
            /// <signature>
            /// <summary>Gets the information from the module data for the event associated with the specified event name.</summary>
            /// <param name="name" type="String">The name of the event.</param>
            /// <returns type="Object" mayBeNull="true" />
            /// </signature>        
            if (!name) {
                return null;
            }
            var events = this.findEvents(), event;
            for (var i = events.length; i >= 0; --i) {
                if ((event = events[i]) && (event.name === name)) {
                    return event;
                }
            }
            return null;
        },
        execute: function (name) {
            /// <signature>
            /// <summary>Executes an event associated with the specified name.</summary>
            /// <param name="name" type="String">The event name.</param>
            /// <returns type="WinJS.Promise" />
            /// </signature>
            if (this._isInDesignTime) {
                return;
            }
            var event = this.findEvent(name);
            if (event) {
                return this._execute($(event.source), event);
            }
            return _Promise.error();
        },
        executeAll: function () {
            /// <signature>
            /// <summary>Executes all events.</summary>
            /// </signature>
            if (this._isInDesignTime) {
                return;
            }
            var events = this.findEvents();
            for (var i = 0, len = events.length; i < len; ++i) {
                this._scheduleEvent(events[i]);
            }
        },
        executeByEvent: function (eventName) {
            /// <signature>
            /// <summary>Executes all the events by name (click, mouseover, etc.)</summary>
            /// <param name="eventName" type="String" />
            /// </signature>
            if (!eventName) {
                return;
            }
            var events = this.findEvents(), event;
            for (var i = 0, len = events.length; i < len; ++i) {
                if ((event = events[i]) &&
                    (event.isEnabled) &&
                    (event.name === eventName)) {
                    this._execute($(event.source), event);
                }
            }
        },
        _scheduleEvent: function (options) {
            /// <signature>
            /// <summary>Creates a new event.</summary>
            /// <param name="event" type="Object" />
            /// </signature>
            if (options && options.isEnabled &&
                options.event && options.event.name) {
                if (options.event.name === "ready") {
                    this._execute($(document), options);
                } else {
                    $(options.source).bind(
                        options.event.name + ".ui-module-event-" + this.uuid,
                        this._eventHandler.bind(this, options));
                }
            }
        },
        _eventHandler: function (options, event) {
            /// <signature>
            /// <param name="options" type="Object" />
            /// <param name="event" type="Event" />
            /// </signature>
            !options.event.isDefault && event.preventDefault();
            !options.event.isBubble && event.stopPropagation();
            this._execute($(event.currentTarget), options);
        },
        _createStateEvent: function (context, options) {
            /// <signature>
            /// <summary>Create an inverse event object.</summary>
            /// </signature>
            var ops = $.extend(true, {}, options);
            ops.event || (ops.event = {});
            if (options._state && ops.styles) {
                for (var i = 0, len = ops.styles.length; i < len; ++i) {
                    var style = ops.styles[i];
                    for (var j = 0, k = options._state.length; j < k; ++j) {
                        var stateDescriptor = options._state[j];
                        style.value = stateDescriptor[style.property];
                    }
                }
            }
            return ops;
        },
        _execute: function ($element, options) {
            /// <signature>
            /// <summary>Executes the specified event.</summary>
            /// <param name="$element" type="jQuery" />
            /// <param name="options" type="Object" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            if (!options || !options.isEnabled) {
                return _Promise.error();
            }
            if (options.target !== "this") {
                $element = $(options.target, this.options.engine.context.container);
            }
            return this._executeAsync($element, options);
        },
        _executeAsync: function ($element, options) {
            /// <signature>
            /// <summary>Executes the current animation.</summary>
            /// <param name="$element" type="jQuery" />
            /// <param name="options" type="Object" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            return _Promise.timeout(asNumber(options.event.delay) || 1, this).then(function () {
                options.styles && this._executeAnimationAsync($element, options.styles);
                options.event && this._activateModuleAsync($element, options.event);
                options.action && this._executeActionAsync($element, options.action);
                options.script && this._executeScriptAsync($element, options.script, options);
                options.triggers && this._executeTriggersAsync($element, options.triggers);
            });
        },
        _activateModuleAsync: function ($element, event) {
            /// <signature>
            /// <param name="$element" type="jQuery" />
            /// <param name="action" type="Object" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            return _Promise(function (completeDispatch, errorDispatch) {
                if (event.active === null ||
                    event.active === undefined) {
                    errorDispatch();
                    return;
                }
                this.options.engine.context.forEach(
                    function (e, t, m) {
                        m && m.option("active", event.active);
                    }, $element);
                completeDispatch();
            }, this);
        },
        _executeAnimationAsync: function ($element, styles) {
            /// <signature>
            /// <summary>Applies the specified style.</summary>
            /// <param name="$element" type="jQuery" />
            /// <param name="styles" type="Array" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            return _Promise(function (completeDispatch, errorDispatch) {
                var len = styles.length;
                if (len === 0) {
                    completeDispatch();
                    return;
                }
                var ix = len;
                function complete() {
                    if (--ix === 0) {
                        completeDispatch();
                    }
                }
                for (var i = 0; i < len; ++i) {
                    var style = styles[i];
                    if (style.duration) {
                        var prop = {};
                        prop[style.property] = style.value;
                        $element.animate(prop, {
                            duration: asNumber(style.duration),
                            queue: style.queue,
                            //easing: style.easing,
                            complete: complete
                        });
                    } else {
                        $element.css(style.property, style.value);
                        if (--ix === 0) {
                            completeDispatch();
                        }
                    }
                }
            });
        },
        _executeActionAsync: function ($element, action) {
            /// <signature>
            /// <param name="$element" type="jQuery" />
            /// <param name="action" type="Object" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            if (!action || !action.item || !action.item.id) {
                return _Promise.error();
            }
            return this.options.engine.workflow.invokeByIdAsync(
                action.item.id,
                action.salt,
                null,
                action.redirect,
                this);
        },
        _executeScriptAsync: function ($element, script, options) {
            /// <signature>
            /// <param name="$element" type="jQuery" />
            /// <param name="script" type="String" />
            /// <param name="options" type="Object" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            if (!script) {
                return _Promise.error();
            }
            var that = this;
            return _Promise(function (completeDispatch, errorDispatch) {
                try {
                    // TODO: Cache the compiled function
                    (new Function(script)).apply({
                        $element: $element,
                        element: $element[0],
                        engine: that.options.engine,
                        context: that.options.engine.context,
                        security: _Project.Security,
                        workflow: _Project.Workflow
                    }, []);
                    completeDispatch();
                } catch (ex) {
                    _WinJS.log({ type: "error", message: "Event: %s - script error: %s" }, options.name, ex.message);
                    errorDispatch(ex.message);
                }
            });
        },
        _executeTriggersAsync: function ($element, triggers) {
            /// <signature>
            /// <summary>Applies the specified style.</summary>
            /// <param name="$element" type="jQuery" />
            /// <param name="triggers" type="Array" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            return _Promise(function (completeDispatch, errorDispatch) {
                for (var i = 0, len = triggers.length; i < len; ++i) {
                    this.execute(triggers[i].event);
                }
                completeDispatch();
            }, this);
        }
    });

})(WinJS, PI.Project, PI.Portal);
