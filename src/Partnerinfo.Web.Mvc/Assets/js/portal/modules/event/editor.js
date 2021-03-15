// Copyright (c) Partnerinfo Ltd. All Rights Reserved.
/// <reference path="module.js" />

(function (_WinJS, _KO) {
    "use strict";

    var _Editable = _KO.Editable;
    var _pureComputed = ko.pureComputed;

    var EventModel = _WinJS.Class.derive(_Editable, function (module, options) {
        /// <signature>
        /// <summary>Allows a user to edit an event.</summary>
        /// <returns type="EventModule" />
        /// </signature>
        options = options || {};
        options.mapping = {
            copy: ["action.item"]
        };

        _Editable.apply(this, [options]);

        this._disposed = false;

        this.designer = options.designer;
        this.action = ko.observable(options.item && options.item.action && options.item.action.item);
        this.userEvents = module.options.engine.context.filterEvents(module.element);

        this.module = module;
        this.eventNames = options.eventNames;
        this.events = ko.utils.optionsMap(options.eventNames);
        this.cssMap = this._createStyleMap();
        this.easings = [null, "linear", "swing", "easeInQuad", "easeOutQuad", "easeInOutQuad", "easeInCubic", "easeOutCubic", "easeInOutCubic", "easeInQuart", "easeOutQuart", "easeInOutQuart", "easeInQuint", "easeOutQuint", "easeInOutQuint", "easeInSine", "easeOutSine", "easeInOutSine", "easeInExpo", "easeOutExpo", "easeInOutExpo", "easeInCirc", "easeOutCirc", "easeInOutCirc", "easeInElastic", "easeOutElastic", "easeInOutElastic", "easeInBack", "easeOutBack", "easeInOutBack", "easeInBounce", "easeOutBounce", "easeInOutBounce"];

        this.currentEventName = _pureComputed(this._getCurrentEventName, this);
        this.sourceCount = _pureComputed(this._getSourceCount, this).extend({ rateLimit: 100 });
        this.targetCount = _pureComputed(this._getTargetCount, this).extend({ rateLimit: 100 });

        this._actionSn = this.action.subscribe(this._actionChanged, this);
    }, {
        _createStyleMap: function () {
            /// <signature>
            /// <summary>Creates a new ordered style map</summary>
            /// <returns type="Array" />
            /// </signature>
            return [{ name: "", property: "" }].concat(
                $.ui.cssEditor.prototype._createCssMap()
                    .filter(function (item) {
                        return item.type !== "group";
                    })
                .sort(function (a, b) {
                    var nameA = a.name.toUpperCase().normalize();
                    var nameB = b.name.toUpperCase().normalize();
                    if (nameA < nameB) { return -1; }
                    if (nameA > nameB) { return 1; }
                    return 0;
                }));
        },
        _getCurrentEventName: function () {
            /// <signature>
            /// <summary>Gets the current event's localized name</summary>
            /// <returns type="String" />
            /// </signature>
            return this.eventNames[this.item.event.name()] || "";
        },
        _getSourceCount: function () {
            /// <signature>
            /// <summary>Returns the number of elements based on the given query selector</summary>
            /// <returns type="Number" />
            /// </signature>
            return this._getElementCount(this.item.source());
        },
        _getTargetCount: function () {
            /// <signature>
            /// <summary>Returns the number of elements based on the given query selector</summary>
            /// <returns type="Number" />
            /// </signature>
            return this._getElementCount(this.item.target());
        },
        _getElementCount: function (selector) {
            /// <signature>
            /// <summary>Gets the number of elements selected by the given expression</summary>
            /// <returns type="Number" />
            /// </signature>
            if (!selector) {
                return 0;
            }
            if (selector === "this" ||
                selector === "parent") {
                return 1;
            }
            return $(selector, this.designer.pageModules).length;
        },
        _actionChanged: function (action) {
            /// <signature>
            /// <summary>Fires when the current action changes</summary>
            /// <param name="action" type="Object" />
            /// </signature>
            var actionItem = this.item.action.item;
            if (action) {
                actionItem.id = action.id;
                actionItem.name = action.name;
            } else {
                actionItem.id = null;
                actionItem.name = null;
            }
        },
        createStyle: function () {
            /// <signature>
            /// <summary>Creates a new style and adds it to the styles.</summary>
            /// </signature>
            var style = this.module.createStyle();
            this.item.styles.push(ko.mapping.fromJS(style));
        },
        removeStyle: function (style) {
            /// <signature>
            /// <summary>Deletes the specified style.</summary>
            /// <param name="style" type="Object" />
            /// </signature>
            this.item.styles.remove(style);
        },
        moveUpStyle: function (style) {
            /// <signature>
            /// <summary>Moves up an style in the list.</summary>
            /// <param name="style" type="Object" />
            /// </signature>
            var index = this.item.styles.indexOf(style);
            if (index > 0) {
                this.item.styles.remove(style);
                this.item.styles.splice(index - 1, 0, style);
            }
        },
        addTrigger: function () {
            /// <signature>
            /// <summary>Adds a new trigger</summary>
            /// </signature>
            var tigger = this.module.createTrigger();
            this.item.triggers.push(ko.mapping.fromJS(tigger));
        },
        removeTrigger: function (tigger) {
            /// <signature>
            /// <summary>Deletes a trigger</summary>
            /// <param name="tigger" type="Object" />
            /// </signature>
            this.item.triggers.remove(tigger);
        },
        dispose: function () {
            /// <signature>
            /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            /// </signature>
            if (this._disposed) {
                return;
            }
            this.currentEventName && this.currentEventName.dispose();
            this.currentEventName = null;
            this.sourceCount && this.sourceCount.dispose();
            this.sourceCount = null;
            this.targetCount && this.targetCount.dispose();
            this.targetCount = null;

            _Editable.prototype.dispose.apply(this, arguments);

            this._disposed = true;
        }
    });

    // http://learn.jquery.com/jquery-ui/widget-factory/extending-widgets/#redefining-widgets

    $.widget("PI.PortalEventModule", $.PI.PortalEventModule, {
        _create: function () {
            /// <signature>
            /// <summary>Initializes a new instance of the page module widget.</summary>
            /// </signature>
            this.events = ko.observableArray();
            this.selection = ko.observableArray();
            this.eventNames = _T("portal").events;

            this._super();

            this._eventsSn = this.events.subscribe(this._eventsChanged, this);
            this._setupEditor(this.options.editorMenu, this.options.editorContent);
        },
        _eventsChanged: function (value) {
            /// <signature>
            /// <summary>Occurs when the observable event array changes.</summary>
            /// <param name="value" type="Array" />
            /// </signature>
            this._locked = true;
            try {
                this._moduleOptions.events = value.map(function (item) {
                    return {
                        isEnabled: item.isEnabled,
                        name: item.name,           // User event name
                        source: item.source,       // Source element (jQuery selector string)
                        target: item.target,       // Target element (jQuery selector string)
                        event: item.event,         // Event descriptor
                        classes: item.classes,     // CSS classes
                        styles: item.styles,       // Animated styles
                        script: item.script,       // JavaScript
                        action: item.action,       // Action link
                        triggers: item.triggers    // Other events ( chaining )
                    };
                });
                this.saveModuleOptions(this._moduleOptions);
            } finally {
                this._locked = false;
            }
        },
        setModuleOptions: function (options) {
            /// <signature>
            /// <param name="options" type="Object" />
            /// </signature>
            if (this._locked) {
                return;
            }
            options = options || {};
            options.events = options.events || [];
            this._super(options);
            this.events(options.events);
        },
        createEvent: function (options) {
            /// <signature>
            /// <summary>Creates a native event model.</summary>
            /// <param name="options" type="Object" optional="true" />
            /// <returns type="Object" />
            /// </signature>
            return options = options || {}, {
                isEnabled: options.isEnabled != false,
                name: options.name || String.format(_T("portal.modules.events.fakeName"), Date.now()),
                source: options.source,
                target: options.target,
                event: this.createNativeEvent(options.event),
                styles: Array.isArray(options.styles) ? options.styles.map(function (style) { return this.createStyle(style); }, this) : [],
                script: options.script,
                action: this.createAction(options.action),
                triggers: Array.isArray(options.triggers) ? options.triggers.map(function (trigger) { return this.createTrigger(trigger); }, this) : []
            };
        },
        createNativeEvent: function (event) {
            /// <signature>
            /// <summary>Creates a native event model.</summary>
            /// <returns type="Object" />
            /// </signature>
            /// <signature>
            /// <summary>Creates a native event model.</summary>
            /// <param name="event" type="Object">A native JS object to extend.</param>
            /// <returns type="Object" />
            /// </signature>
            return event = event || {}, {
                name: event.name || "ready",
                isDefault: event.isDefault !== false,
                isBubble: event.isBubble !== false,
                delay: event.delay | 0,
                reverse: event.reverse | 0,
                iterations: event.iterations || 1,
                active: event.active || null
            };
        },
        createStyle: function (style) {
            /// <signature>
            /// <summary>Creates a style model.</summary>
            /// <returns type="Object" />
            /// </signature>
            /// <signature>
            /// <summary>Creates a style model.</summary>
            /// <param name="style" type="Object">A native JS object to extend.</param>
            /// <returns type="Object" />
            /// </signature>
            return style = style || {}, {
                property: style.property,
                value: style.value,
                duration: style.duration,
                easing: style.easing,
                queue: style.queue !== false
            };
        },
        createAction: function (action) {
            /// <signature>
            /// <summary>Creates a style model.</summary>
            /// <returns type="Object" />
            /// </signature>
            /// <signature>
            /// <summary>Creates a style model.</summary>
            /// <param name="action" type="Object">A native JS object to extend.</param>
            /// <returns type="Object" />
            /// </signature>
            return action = action || {},
                (action.item = action.item || {}),
                (action.link = action.link || null), {
                    link: null,
                    item: {
                        id: action.item.id,
                        name: action.item.name
                    },
                    salt: action.salt,
                    redirect: action.redirect !== false
                };
        },
        createTrigger: function (trigger) {
            /// <signature>
            /// <summary>Creates a new trigger</summary>
            /// <returns type="Object" />
            /// </signature>
            return trigger = trigger || {}, {
                event: trigger.event,
                queue: !!trigger.queue
            };
        },
        insertEvent: function (item) {
            /// <signature>
            /// <summary>Saves the event.</summary>
            /// <param name="item" type="Object">The event object to save.</param>
            /// <returns type="Boolean" />
            /// </signature>
            if (this.isValidEvent(item)) {
                var oldEvent = this.findEvent(name);
                if (oldEvent) {
                    return false;
                }
                this.events.push(this.createEvent(item));
                return true;
            }
            return false;
        },
        updateEvent: function (item) {
            /// <signature>
            /// <summary>Updates an existing event object.</summary>
            /// <param name="item" type="Object">The event object to update.</param>
            /// <returns type="Boolean" />
            /// </signature>
            if (!item) {
                return;
            }
            if (this.isValidEvent(item)) {
                var oldEvent = this.findEvent(item.name);
                if (oldEvent) {
                    this.events.replace(oldEvent, this.createEvent(this.createEvent(oldEvent), item));
                    return true;
                }
            }
            return false;
        },
        isValidEvent: function (item) {
            /// <signature>
            /// <summary>Validates the specified event object.</summary>
            /// <returns type="Boolean" />
            /// </signature>
            return !!(item && item.name);
        },
        addNewEvent: function () {
            /// <signature>
            /// <summary>Adds a new event to the list and begins a new edit session.</summary>
            /// <returns type="Object" />
            /// </signature>
            var that = this;
            var selected = this.options.designer.options.selection.current;
            var selectedId = selected && selected.attr("id");
            var event = this.createEvent(selected && {
                name: selectedId,
                source: selectedId && ("#" + selectedId),
                event: { name: "click" }
            });
            this._editEvent(event).done(
                function (oldItem, newItem) {
                    that.events.push(newItem);
                });
        },
        editEvent: function (item) {
            /// <signature>
            /// <summary>Edits the specified event of the queue.</summary>
            /// <param name="item" type="Object">The event to edit.</param>
            /// </signature>
            var that = this;
            this._editEvent(item).done(function (oldItem, newItem) {
                that.events.replace(oldItem, newItem);
            });
        },
        moveUpEvent: function (item) {
            /// <summary>Moves up an event in the list.</summary>
            /// <param name="event" type="Object" />
            if (!item) {
                throw new TypeError("Failed to move the event. The item cannot be null or undefined.");
            }
            var i = this.events.indexOf(item);
            if (i > 0) {
                this.events.valueWillMutate();
                this.events().swap(i, i - 1);
                this.events.valueHasMutated();
            }
        },
        copyToClipboard: function () {
            /// <signature>
            /// <summary>Copies all the selected events to the clipboard.</summary>
            /// </signature>
            PI.userCache(PI.Storage.local, "portal.events.clipboard", this.selection());
        },
        pasteFromClipboard: function () {
            /// <signature>
            /// <summary>Copies all the selected events to the clipboard.</summary>
            /// </signature>
            var events = PI.userCache(PI.Storage.local, "portal.events.clipboard");
            if (Array.isArray(events)) {
                events.forEach(function (item) {
                    this.events.push(this.createEvent(item));
                }, this);
            }
        },
        removeEvent: function (item) {
            /// <signature>
            /// <summary>Deletes the event.</summary>
            /// <param name="event" type="Object" />
            /// </signature>
            this.events.remove(item);
        },
        _editEvent: function (item) {
            /// <signature>
            /// <summary>Edits the event.</summary>
            /// <param name="item" type="Object" />
            /// <returns type="WinJS.Promise" />
            /// </signature>
            var that = this;
            return _WinJS.Promise(function (completeDispatch, errorDispatch) {
                var model = new EventModel(that, {
                    designer: that.options.designer,
                    item: that.createEvent(item),
                    eventNames: that.eventNames
                });
                that._createDialog({
                    title: _T("portal.modules.event.name"),
                    width: 800,
                    height: 600,
                    overflow: "auto",
                    resizable: true,
                    open: function () {
                        var element = $(this);
                        element.dialog("progress", model.designer.options.engine.context.loadEditorAsync(that.options.typeClass).then(function () {
                            var task = PI.bind(element, model, "koPortalEventModule");
                            model.beginEdit();
                            model.designer.options.menu.setEditing(true);
                            return task;
                        }));
                    },
                    buttons: [{
                        "class": "ui-btn ui-btn-primary",
                        "text": _T("portal.actions.ok"),
                        "click": function () {
                            var newItem = model.toObject();
                            if (that.isValidEvent(newItem)) {
                                model.endEdit();
                                completeDispatch(item, newItem);
                                $(this).dialog("close");
                            }
                        }
                    }, {
                        "class": "ui-btn",
                        "text": _T("portal.actions.cancel"),
                        "click": function () { $(this).dialog("close"); }
                    }],
                    close: function () {
                        model.cancelEdit();
                        model.designer.options.menu.setEditing(false);
                        model.dispose();
                        errorDispatch();
                    }
                });
            });
        },
        _setOption: function (key, value) {
            /// <signature>
            /// <summary>Occurs when a property changes (OnPropertyChanged).</summary>
            /// <param name="key" type="String">Name of the property.</param>
            /// <param name="value" type="Object">The value of the property.</param>
            /// </signature>
            this._super(key, value);

            if (key === "editor") {
                this._setupEditor(value);
            }

            return this;
        },
        _setupEditor: function (value) {
            /// <signature>
            /// <summary>Setups the editor container.</summary>
            /// <param name="value" type="Element" />
            /// </signature>
            if (!value) {
                return;
            }
            var that = this;
            require(["portal/modules/event/list.html"]).then(function () {
                ko.renderTemplate("koPageEventMenu", that, null, value.menu && value.menu[0]);
                ko.renderTemplate("koPageEventList", that, null, value.content && value.content[0]);
            });
        },
        _destroy: function () {
            /// <signature>
            /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            /// </signature>
            this._eventsSn && this._eventsSn.dispose();
            this._eventsSn = null;
            this._super();
        }
    });

})(WinJS, WinJS.Knockout);
