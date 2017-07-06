// Copyright (c) Partnerinfo Ltd. All Rights Reserved.
/// <reference path="module.js" />

(function (_WinJS, _KO, _Resources) {
    "use strict";

    var _Class = _WinJS.Class;
    var _observable = ko.observable;
    var _observableArray = ko.observableArray;

    function generateGuid() {
        var d = new Date().getTime();
        var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = (d + Math.random() * 16) % 16 | 0;
            d = Math.floor(d / 16);
            return (c === 'x' ? r : (r & 0x3 | 0x8)).toString(16);
        });
        return uuid;
    }

    var EventData = _Class.define(function EventData_ctor(options) {
        /// <signature>
        /// <summary>Initializes a new instance of the EventData class.</summary>
        /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the EventData.</param>
        /// <returns type="EventData" />
        /// </signature>
        options = options || {};
        this.name = _observable(options.name || "ready");
        this.isDefault = _observable(options.isDefault !== false);
        this.isBubble = _observable(options.isBubble !== false);
        this.delay = _observable(options.delay | 0);
        this.reverse = _observable(options.reverse | 0);
        this.iterations = _observable(options.iterations || 1);
        this.active = _observable(options.active || null);
    }, {
        toObject: function () {
            /// <signature>
            /// <summary>Returns native representation of the model.</summary>
            /// <returns type="Object" />
            /// </signature>
            return {
                name: this.name(),
                isDefault: this.isDefault(),
                isBubble: this.isBubble(),
                delay: this.delay(),
                reverse: this.reverse(),
                iterations: this.iterations(),
                active: this.active()
            };
        }
    });

    var EventClassOperation = { add: 1, remove: 2, toggle: 3 };

    var EventClass = _Class.define(function EventClass_ctor(options) {
        /// <signature>
        /// <summary>Initializes a new instance of the EventClass class.</summary>
        /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the EventClass.</param>
        /// <returns type="EventClass" />
        /// </signature>
        options = options || {};
        this.name = _observable(options.name);
        this.operation = _observable(options.operation || EventClassOperation.add); // add | remove | toggle
    }, {
        toObject: function () {
            /// <signature>
            /// <summary>Returns native representation of the model.</summary>
            /// <returns type="Object" />
            /// </signature>
            return {
                name: this.name(),
                operation: this.operation()
            };
        }
    });

    var EventStyle = _Class.define(function EventStyle_ctor(options) {
        /// <signature>
        /// <summary>Initializes a new instance of the EventStyle class.</summary>
        /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the EventStyle.</param>
        /// <returns type="EventStyle" />
        /// </signature>
        options = options || {};
        this.property = _observable(options.property);
        this.value = _observable(options.value);
        this.duration = _observable(options.duration | 0);
        this.easing = _observable(options.easing);
        this.queue = _observable(!!options.queue);
    }, {
        toObject: function () {
            /// <signature>
            /// <summary>Returns native representation of the model.</summary>
            /// <returns type="Object" />
            /// </signature>
            return {
                property: this.property(),
                value: this.value(),
                duration: this.duration(),
                easing: this.easing(),
                queue: this.queue()
            };
        }
    });

    var EventAction = _Class.define(function EventAction_ctor(options) {
        /// <signature>
        /// <summary>Initializes a new instance of the EventAction class.</summary>
        /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the EventAction.</param>
        /// <returns type="EventAction" />
        /// </signature>
        options = options || {};
        this.link = _observable(options.link);
        this.item = options.item || { id: null, name: null };
        this.salt = options.salt;
        this.redirect = options.redirect !== false;
    }, {
        toObject: function () {
            /// <signature>
            /// <summary>Returns native representation of the model.</summary>
            /// <returns type="Object" />
            /// </signature>
            return {
                link: this.link(),
                item: this.item,
                salt: this.salt,
                redirect: this.redirect
            };
        }
    });

    var EventTrigger = _Class.define(function EventTrigger_ctor(options) {
        /// <signature>
        /// <summary>Initializes a new instance of the EventTrigger class.</summary>
        /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the EventTrigger.</param>
        /// <returns type="EventTrigger" />
        /// </signature>
        options = options || {};
        this.event = options.event;
        this.queue = !!options.queue;
    }, {
        toObject: function () {
            /// <signature>
            /// <summary>Returns native representation of the model.</summary>
            /// <returns type="Object" />
            /// </signature>
            return {
                event: this.event,
                queue: this.queue
            };
        }
    });

    var EventOptionsItem = _Class.define(function EventOptionsItem_ctor(options) {
        /// <signature>
        /// <summary>Initializes a new instance of the EventOptions class.</summary>
        /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the EventOptionsItem.</param>
        /// <returns type="EventOptionsItem" />
        /// </signature>
        options = options || {};
        this.id = options.id || generateGuid();                       // Unique identifier
        this.isEnabled = _observable(options.isEnabled !== false);  // True if the event is enabled
        this.name = _observable(options.name || "Esemény neve");    // Name
        this.source = _observable(options.source);                  // Source element (jQuery selector string)
        this.target = _observable(options.target);                  // Target element (jQuery selector string)
        this.event = new EventData(options.event);                    // Event descriptor
        this.classes = _observableArray(options.classes || []);     // CSS classes
        this.styles = _observableArray(options.styles || []);       // Styles
        this.script = _observable(options.script);                  // JavaScript
        this.action = new EventAction(options.action);                // Action link
        this.triggers = _observableArray(options.triggers || []);   // Fires other events

        this.classesView = this.classes.map(function (item) { return new EventClass(item); });
        this.stylesView = this.styles.map(function (item) { return new EventStyle(item); });
        this.triggersView = this.triggers.map(function (item) { return new EventTrigger(item); });
    }, {
        toObject: function () {
            /// <signature>
            /// <summary>Returns native representation of the model.</summary>
            /// <returns type="Object" />
            /// </signature>
            return {
                id: this.id,
                isEnabled: this.isEnabled(),
                name: this.name(),
                source: this.source(),
                target: this.target(),
                event: this.event.toObject(),
                classes: this.classesView().map(function (item) { return item.toObject(); }),
                styles: this.stylesView().map(function (item) { return item.toObject(); }),
                script: this.script(),
                action: this.action.toObject(),
                triggers: this.triggersView().map(function (item) { return item.toObject(); })
            };
        }
    });

    var EventOptionsList = _Class.derive(_KO.List, function EventOptionsList_ctor(options) {
        /// <signature>
        /// <summary>Initializes a new instance of the EventOptionsList class.</summary>
        /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the EventOptionsList.</param>
        /// <returns type="EventOptionsList" />
        /// </signature>
        options = options || {};
        _KO.List.apply(this, [options]);
    }, {
        mapItem: function (item) {
            /// <signature>
            /// <summary>Represents a function called before adding a new item to the list.</summary>
            /// <param name="item" type="Object" />
            /// <returns type="Object" />
            /// </signature>
            return new EventOptionsItem(item);
        },
        copyToClipboard: function () {
            /// <signature>
            /// <summary>Copies all the selected events to the clipboard.</summary>
            /// </signature>
            var items = this.selection();
            items.length || (items = this.items());
            PI.userCache(PI.Storage.local, "portal.events.clipboard", items.map(function (i) { return i.toObject(); }));
        },
        pasteFromClipboard: function () {
            /// <signature>
            /// <summary>Copies all the selected events to the clipboard.</summary>
            /// </signature>
            var events = PI.userCache(PI.Storage.local, "portal.events.clipboard");
            if (!Array.isArray(events)) {
                return;
            }
            this.splice.apply(this, [this.indexOf(this.current()) + 1, 0].concat(events.map(function (e) {
                e.id = generateGuid();
                e.name = e.name + " - copy";
                return e;
            })));
        }
    });

    // http://learn.jquery.com/jquery-ui/widget-factory/extending-widgets/#redefining-widgets

    $.widget("PI.PortalEventModule", $.PI.PortalEventModule, {
        options: {
            /// <field type="EventOptionsList" />
            events: null
        },
        _create: function () {
            /// <signature>
            /// <summary>Initializes a new instance of the page module widget.</summary>
            /// </signature>
            this.events = this.options.events || new EventOptionsList();
            this._super();
            this._setupEditorContainer(this.options.editorContainer);
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
            this.events.replaceAll.apply(this.events, options.events);
        },
        _setOption: function (key, value) {
            /// <signature>
            /// <summary>Occurs when a property changes (OnPropertyChanged).</summary>
            /// <param name="key" type="String">Name of the property.</param>
            /// <param name="value" type="Object">The value of the property.</param>
            /// </signature>
            this._super(key, value);
            if (key === "editorContainer" && value) {
                this._setupEditorContainer(value);
            }
            return this;
        },
        _setupEditorContainer: function (value) {
            /// <signature>
            /// <summary>Setups the editor container.</summary>
            /// <param name="value" type="Element" />
            /// </signature>
            PI.bind(value, this.events, "koPageEventList", ["portal/module/events.html"]);
        }
    });

})(WinJS, WinJS.Knockout, WinJS.Resources);
