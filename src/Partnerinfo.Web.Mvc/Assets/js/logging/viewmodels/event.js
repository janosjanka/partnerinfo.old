// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="../services/category.js" />
/// <reference path="../services/event.js" />
/// <reference path="../services/notification.js" />

(function (_KO, _WinJS, PI) {
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
    var routes = PI.Routes;

    function getEmailLocalPart(email) {
        /// <param name="email" type="String" optional="true" />
        /// <returns type="String" />
        if (email) {
            var index = email.indexOf('@');
            if (index === -1) {
                return email;
            }
            return email.substr(0, index);
        }
    }

    function reverse(str) {
        /// <param name="str" type="String" optional="true" />
        /// <returns type="String" />
        if (str) {
            var ret = "";
            for (var i = str.length; --i >= 0;) {
                ret += str[i];
            }
            return ret;
        }
    }

    var logResources = {
        categoryDoesNotExist: function () { return _T("logging/logCategoryDoesNotExist"); },
        categoryReservedName: function () { return _T("logging/logCategoryReservedName"); },
        categoryName: function (itemCount) { return String.format(_T("logging/logCategoryTemplateName"), itemCount); },
        logObjectType: function () { return _T("logging/logObjectType"); }
    };

    var ns = _WinJS.Namespace.defineWithParent(PI, "Logging", {

        LogResources: logResources,

        TimeInterval: {
            custom: null,
            today: "today",
            thisWeek: "thisWeek",
            thisMonth: "thisMonth"
            //quarter1: "quarter1",
            //quarter2: "quarter2",
            //quarter3: "quarter3",
            //quarter4: "quarter4"
        },

        ObjectType: {
            message: "message",
            project: "project",
            action: "action",
            mailMessage: "mailMessage",
            portal: "portal",
            page: "page"
        },

        ObjectIcon: {
            message: "message",
            project: "project",
            action: "action",
            mailMessage: "mail",
            portal: "portal",
            page: "page"
        },

        ObjectState: {
            none: null,
            unchanged: "unchanged",
            added: "added",
            modified: "modified",
            deleted: "deleted"
        },

        ObjectLinks: _Class.define(function ObjectLinks_ctor(link, edit, rel) {
            /// <signature>
            /// <summary>Initializes a new instance of the ObjectLinks class.</summary>
            /// <param name="link" type="String" />
            /// <param name="edit" type="String" />
            /// <param name="rel" type="String" />
            /// <returns type="PI.Logging.ObjectLinks" />
            /// </signature>
            this.link = link;
            this.edit = edit;
            this.rel = rel;
        }),

        EventListItem: _Class.define(function EventListItem_ctor(options) {
            /// <signature>
            /// <summary>Initializes a new instance of the EventListItem class.</summary>
            /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the EventListItem.</param>
            /// <returns type="PI.Logging.EventListItem" />
            /// </signature>
            options = options || {};

            this.id = options.id;
            this.objectType = options.objectType || ns.ObjectType.message;
            this.object = options.object;
            this.category = _observable(options.category);
            this.project = options.project;
            this.projectLink = this.project && routes.Project.Logging.list(this.project.id);
            this.contact = options.contact;
            this.contactState = options.contactState || ns.ObjectState.none;
            this.contactLink = this.project && this.contact && routes.Project.Contacts.item(this.project.id, this.contact.id);
            this.correlation = options.correlation;
            this.correlation && (this.correlation.startDate = new Date(this.correlation.startDate));
            this.startDate = new Date(options.startDate);
            this.finishDate = options.finishDate && new Date(options.finishDate);
            this.browserBrand = options.browserBrand;
            this.browserVersion = options.browserVersion;
            this.mobileDevice = options.mobileDevice;
            this.clientId = options.clientId;
            this.clientTitle = this.clientId && reverse(this.clientId);
            this.customUri = options.customUri;
            this.referrerUrl = options.referrerUrl;
            this.message = options.message;

            this.objectIcon = "i " + ns.ObjectIcon[this.objectType];
            this.objectText = this.message || this.object && this.object.name;
            this.objectTitle = this.message || this.object && this.object.name;
            this.objectLinks = null;

            this.project && (
                this.objectTitle += "\r\n",
                this.objectTitle += this.project && this.project.name);
            this.contactText = this.contact && this.contact.email && (this.contact.email.name || getEmailLocalPart(this.contact.email.address));
            this.contactTitle = this.contact && this.contact.email && (this.contactText + "\r\n" + this.contact.email.address);
            this.startDateText = _Resources.format(this.startDate, "F");
            this.finishDateText = this.finishDate && _Resources.format(this.finishDate, "F");
            this.finishElapsedTimeText = this.finishDate && this.finishDate.elapsed(this.startDate).toElapsedShortTimeString();
            this.correlationStartDateText = this.correlation && _Resources.format(this.correlation.startDate, "F");
            this.correlationElapsedTimeText = this.correlation && this.startDate.elapsed(this.correlation.startDate).toElapsedShortTimeString();
        }, {
            isFromMobile: function () {
                /// <signature>
                /// <summary>Returns true if this event was obtained from a mobile device</summary>
                /// </signature>
                return this.mobileDevice && this.mobileDevice !== "unknown";
            },
            getObjectLinksAsync: function () {
                /// <signature>
                /// <summary>Returns a link for the current object.</summary>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                return _Promise(function (completeDispatch, errorDispatch) {
                    if (this.objectLinks) {
                        completeDispatch(this.objectLinks);
                        return;
                    }
                    if (this.objectType === ns.ObjectType.action) {
                        this.project
                            ? completeDispatch(this.objectLinks = new ns.ObjectLinks(routes.Project.Actions.item(this.project.id, this.object.id)))
                            : errorDispatch();
                        return;
                    }
                    if (this.objectType === ns.ObjectType.mailMessage) {
                        this.project
                            ? completeDispatch(this.objectLinks = new ns.ObjectLinks(routes.Project.MailMessages.item(this.project.id, this.object.id)))
                            : errorDispatch();
                        return;
                    }
                    if (this.objectType === ns.ObjectType.page) {
                        PI.api({
                            method: "GET",
                            path: "/portal/pages/" + this.object.id + "?fields=portal",
                            async: false
                        }, this).then(
                            function (page) {
                                completeDispatch(this.objectLinks = new ns.ObjectLinks(
                                    PI.Routes.Portal.Pages.preview(page.portal.uri, page.uri),
                                    PI.Routes.Portal.Pages.designer(page.portal.uri, page.uri),
                                    PI.Routes.Portal.Sites.pages(page.portal.uri)));
                            },
                            errorDispatch);
                        return;
                    }
                    this.project
                        ? completeDispatch(this.objectLinks = new ns.ObjectLinks(routes.Project.Logging.list(this.project.id)))
                        : errorDispatch();
                }, this);
            },
            viewInBrowser: function () {
                /// <signature>
                /// <summary>Opens a new browser tab navigating to the given object.</summary>
                /// </signature>
                var objectType = this.objectType;
                this.getObjectLinksAsync().done(function (objectLinks) {
                    if (objectType !== ns.ObjectType.page) {
                        window.open(objectLinks.link, "_blank");
                        return;
                    }
                    var optionsKey = "events.preview";
                    var options = PI.userCache(PI.Storage.local, optionsKey) || {
                        p: String.format("left+{0} top+{1}", window.innerWidth / 2 - 490, window.innerHeight / 2 - 300),
                        w: 980,
                        h: 600
                    };
                    var dialog = $.WinJS.dialog({
                        overflow: "hidden",
                        width: options.w,
                        height: options.h,
                        position: {
                            my: "left top",
                            at: options.p,
                            of: window
                        },
                        resizable: true,
                        content: String.format('<iframe src="{0}" width="100%" height="100%" frameborder="0" marginheight="0" marginwidth="0" seamless="seamless"></iframe>', objectLinks.link),
                        buttons: [{
                            "class": "ui-btn",
                            "text": "Portál weboldalak",
                            "click": function () { window.open(objectLinks.rel, "_blank"); }
                        }, {
                            "class": "ui-btn",
                            "text": "Weboldal tervezése",
                            "click": function () { window.open(objectLinks.edit, "_blank"); }
                        }, {
                            "class": "ui-btn",
                            "text": "Weboldal megtekintése",
                            "click": function () { window.open(objectLinks.link, "_blank"); }
                        }],
                        create: function () {
                            $(".ui-dialog-title", $(this).parent()).html('<a class="ui-type-link" href="' + objectLinks.link + '" target="_blank">' + objectLinks.link + '</a>');
                        },
                        beforeClose: function () {
                            var position = dialog.option("position");
                            PI.userCache(PI.Storage.local, optionsKey, {
                                p: position.at,
                                w: dialog.option("width"),
                                h: dialog.option("height")
                            });
                        }
                    });
                });
            }
        }),

        EventStringList: _Class.define(function EventStringList(items) {
            /// <signature>
            /// <summary>Initializes a new instance of the EventStringList class.</summary>
            /// <param name="items" type="Array" />
            /// <returns type="PI.Logging.EventStringList" />
            /// </signature>
            this._items = _observableArray(items || []);
            this.items = this._items.map(this.mapItem);
            this.push(EventStringList.placeholder);
        }, {
            mapItem: function (value) {
                /// <signature>
                /// <summary>Maps the value to an object</summary>
                /// <param name="value" type="String" />
                /// <returns type="Object" />
                /// </signature>
                return { value: value };
            },
            hasItems: function () {
                /// <signature>
                /// <summary>Returns true if there is at least one valid item</summary>
                /// <returns type="Boolean" />
                /// </signature>
                var items = this._items();
                var len = items.length;
                return len > 1 || len === 1 && items[0] !== ns.EventStringList.placeholder;
            },
            indexOf: function (value) {
                /// </signature>
                /// <summary>Returns the first index at which a given element can be found in the array, or -1 if it is not present.</summary>
                /// <param name="searchElement" type="Object">Element to locate in the array.</param>
                /// <param name="fromIndex" type="Number">The index at which to begin the search. Default: 0 (Entire array is searched).</param>
                /// <returns type="Number" />
                /// </signature>
                var items = this.items();
                for (var i = 0, len = items.length; i < len; ++i) {
                    if (items[i].value === value) {
                        return i;
                    }
                }
                return -1;
            },
            push: function (value) {
                /// <signature>
                /// <summary>Adds a new value to the array</summary>
                /// <param name="value" type="String" />
                /// </signature>
                value = value || ns.EventStringList.placeholder;
                if (this.indexOf(value) === -1) {
                    this._items.push(value);
                }
            },
            remove: function (value) {
                /// <signature>
                /// <summary>Removes the given value</summary>
                /// <param name="value" type="String" />
                /// </signature>
                if (value && value !== ns.EventStringList.placeholder) {
                    var i = this.indexOf(value);
                    if (i >= 0) {
                        this._items.splice(i, 1);
                        this.push();
                    }
                }
            },
            removeAll: function () {
                /// <signature>
                /// <summary>Removes all items</summary>
                /// </signature>
                this._items.removeAll();
                this.push();
            },
            toArray: function () {
                /// <signature>
                /// <summary>Returns a pure JS object without KO observable functions</summary>
                /// <returns type="Array" />
                /// </signature>
                return this.items()
                    .filter(function (item) {
                        return item.value !== ns.EventStringList.placeholder;
                    })
                    .map(function (item) {
                        return item.value;
                    });
            }
        }, {
            placeholder: null
        }),

        EventListField: {
            /// <field type="Number">
            /// Contains no extra fields
            /// </field>
            none: 0,

            /// <field type="Number">
            /// Contains categories
            /// </field>
            categories: 1 << 0
        },

        EventListFilter: _Class.define(function EventListFilter_ctor(options) {
            /// <signature>
            /// <summary>Initializes a new instance of the EventListFilter class.</summary>
            /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the EventListFilter.</param>
            /// <returns type="PI.Logging.EventListFilter" />
            /// </signature>
            this.options = options = options || {};

            this.category = _observable(options.category || ns.CategoryListItem.reserved);
            this.timeInterval = _observable(options.timeInterval || ns.TimeInterval.thisWeek);
            this.dateFrom = _observable(options.dateFrom);
            this.dateTo = _observable(options.dateTo);
            this.objectType = _observable(options.objectType);
            this.objectTypeOptions = [{ text: null, value: void 0 }].concat(_KO.utils.optionsMap(logResources.logObjectType()));
            this.object = _observable(options.object);
            this.contact = _observable(options.contact);
            this.contactState = _observable(options.contactState || ns.ObjectState.none);
            this.project = _observable(options.project);
            this.customUri = _observable(options.customUri);
            this.emails = new ns.EventStringList(options.emails);
            this.clients = new ns.EventStringList(options.clients);
            this.contactSID = _observable(options.contactSID);

            this._editSession = _editSession(this, {
                fields: options.editableFields || [
                 // "category",
                 // "timeInterval",
                    "dateFrom",
                    "dateTo",
                    "objectType",
                    "object",
                    "contactState",
                    "project",
                    "customUri",
                    "contactSID"]
            });
            this.hasChanged = _pureComputed(this._hasChanged, this);
        }, {
            _hasChanged: function () {
                /// <signature>
                /// <summary>Returns true if a filter changes</summary>
                /// <returns type="Boolean" />
                /// </signature>
                return this._editSession.hasChanged()
                    || this.emails.hasItems()
                    || this.clients.hasItems();
            },
            cancel: function () {
                /// <signature>
                /// <summary>Resets all the fields</summary>
                /// </signature>
                this.emails.removeAll();
                this.clients.removeAll();
                this._editSession.cancel();
            },
            getDateByInterval: function (timeInterval) {
                /// <signature>
                /// <summary>Gets a Date for the given time interval value</summary>
                /// <param name="timeInterval" type="PI.Identity.TimeInterval" />
                /// <returns type="Date" />
                /// </signature>
                var now = new Date();
                var thisMorning = new Date(Date.UTC(now.getFullYear(), now.getMonth(), now.getDate()));
                switch (timeInterval) {
                    case ns.TimeInterval.today:
                        return {
                            from: thisMorning,
                            to: null
                        };
                    case ns.TimeInterval.thisWeek:
                        return {
                            from: new Date(Date.UTC(thisMorning.getFullYear(), thisMorning.getMonth(), thisMorning.getDate() - (thisMorning.getDay() + 6) % 7)),
                            to: null
                        };
                    case ns.TimeInterval.thisMonth:
                        return {
                            from: new Date(Date.UTC(thisMorning.getFullYear(), thisMorning.getMonth())),
                            to: null
                        };
                }
            },
            toObject: function () {
                /// <signature>
                /// <summary>Returns a pure JS object without KO observable functions</summary>
                /// <returns type="Object" />
                /// </signature>
                var category = this.category();
                var interval = this.getDateByInterval(this.timeInterval());
                var dateFrom = interval ? interval.from : this.dateFrom();
                var dateTo = interval ? interval.to : this.dateTo();
                var object = this.object();
                var contact = this.contact();
                var project = this.project();
                var emails = this.emails.toArray();
                var clients = this.clients.toArray();

                // The contactSID contains an email or a client ID
                // that should be included in both of the lists
                var contactSID = this.contactSID();
                if (contactSID) {
                    if (emails.indexOf(contactSID) === -1) {
                        emails.push(contactSID);
                    }
                    if (clients.indexOf(contactSID) === -1) {
                        clients.push(contactSID);
                    }
                }

                return {
                    categoryId: category && category.id,
                    objectType: this.objectType(),
                    objectId: object && object.id,
                    projectId: project && project.id,
                    contactId: contact && contact.id,
                    contactState: this.contactState(),
                    dateFrom: dateFrom,
                    dateTo: dateTo,
                    customUri: this.customUri(),
                    emails: emails,
                    clients: clients
                };
            }
        }),

        EventCommandHandler: _Class.define(function EventCommandHandler_ctor(list) {
            /// <signature>
            /// <summary>Initializes a new instance of the EventCommandHandler class.</summary>
            /// <returns type="PI.Logging.EventCommandHandler" />
            /// </signature>
            this.list = list;
        }, {
            addToFilter: function (params) {
                /// <signature>
                /// <summary>Adds the given member to the filter</summary>
                /// <param name="params" type="Object" />
                /// </signature>
                if (!params || !params.item) {
                    return;
                }
                var what = params.what;
                var item = params.item;
                switch (what) {
                    case "object":
                        this.list.deferRefresh(function () {
                            this.filter.objectType(item.objectType);
                            item.object && this.filter.object(item.object);
                        }, this.list);
                        break;
                    case "category":
                    case "project":
                        this.list.deferRefresh(function () {
                            this.filter[what](_KO.unwrap(item[what]));
                        }, this.list);
                        break;
                    case "contact":
                        item.contact && item.contact.email && this.list.deferRefresh(function () {
                            this.filter.contactSID(item.contact.email.address);
                        }, this.list);
                        break;
                    case "contactState":
                        item.contactState && this.list.deferRefresh(function () {
                            this.filter.contactState(item.contactState);
                        }, this.list);
                        break;
                    case "clientId":
                        item.clientId && this.list.deferRefresh(function () {
                            this.filter.contactSID(item.clientId);
                        }, this.list);
                        break;
                    case "customUri":
                        item.customUri && this.list.deferRefresh(function () {
                            this.filter.customUri(item.customUri);
                        }, this.list);
                        break;
                    case "startDate":
                        item.startDate && this.list.deferRefresh(function () {
                            this.filter.dateFrom(item.startDate);
                            this.filter.dateTo(null);
                            this.filter.timeInterval(ns.TimeInterval.custom);
                        }, this.list);
                        break;
                }
            },
            setAllCategories: function (params) {
                /// <signature>
                /// <summary>Sets a category for a group of events.</summary>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                var category = params.item && params.item.toObject();
                var categoryId = category && category.id;
                if (this.list.filteredSelection()) {
                    var filter = this.list.filter.toObject();
                    return this.list.service.setAllCategoriesAsync({ categoryId: categoryId, filter: filter }, this).then(function () {
                        this.list.forEach(function (event) {
                            event.category(category);
                        });
                    });
                } else {
                    var events = this.list.selection();
                    var ids = events.map(function (event) { return event.id; });
                    return this.list.service.setAllCategoriesAsync({ categoryId: categoryId, ids: ids }, this).then(function () {
                        events.forEach(function (event) {
                            event.category(category);
                        });
                    });
                }
            },
            deleteAll: function () {
                /// <signature>
                /// <summary>Deletes all the selected items</summary>
                /// </signature>
                if (this.list.filteredSelection()) {
                    var filter = this.list.filter.toObject();
                    return this.list.service.deleteAllAsync({ filter: filter }, this).then(function () {
                        this.list.refresh();
                    });
                } else {
                    var events = this.list.selection();
                    var ids = events.map(function (event) { return event.id; });
                    return this.list.service.deleteAllAsync({ ids: ids }, this).then(function () {
                        this.list.refresh();
                    });
                }
            }
        }),

        EventCounter: _Class.define(function () {
            /// <signature>
            /// <summary>Initializes a new instance of the EventCounter class.</summary>
            /// <param name="options" type="Object" />
            /// <returns type="PI.Logging.EventCounter" />
            /// </signature>
            this._onTimeoutBound = this._onTimeout.bind(this);
            this.value = _observable(0);
            this.limit = 999;
            this.timeout = 2000;
            this.visible = _observable(false);
        }, {
            add: function (value) {
                /// <signature>
                /// <summary>Increases the value of the counter.</summary>
                /// <param name="value" type="Number" optional="true" />
                /// </signature>
                var value = this.value();
                if (value >= this.limit) {
                    this.clear();
                    return;
                }
                this.value(value + 1);
            },
            clear: function () {
                /// <signature>
                /// <summary>Clear the counter.</summary>
                /// </signature>
                this.value(0);
            },
            hide: function () {
                /// <signature>
                /// <summary>Hides the counter.</summary>
                /// </signature>
                this.stopTimer();
                this.visible(false);
                this.startTimer();
            },
            startTimer: function () {
                /// <signature>
                /// <summary>Starts the timer.</summary>
                /// </signature>
                this.stopTimer();
                this._handle = window.setTimeout(this._onTimeoutBound, this.timeout);
            },
            stopTimer: function () {
                /// <signature>
                /// <summary>Stops the timer.</summary>
                /// </signature>
                this._handle && window.clearTimeout(this._handle);
                this._handle = null;
            },
            _onTimeout: function () {
                /// <signature>
                /// <summary></summary>
                /// </signature>
                this.visible(true);
            }
        }),

        EventList: _Class.derive(_Knockout.PagedList, function EventList_ctor(options) {
            /// <signature>
            /// <summary>Initializes a new instance of the EventList class.</summary>
            /// <param name="options" type="Object" />
            /// <returns type="PI.Logging.EventList" />
            /// </signature>
            options = options || {};
            options.pageSize = options.pageSize || 50;
            _Knockout.PagedList.apply(this, arguments);
            this._disposed = false;
            this._onEventReceivedBound = this._onEventReceived.bind(this);
            this._onCategoryFilterChangedBound = this._onCategoryFilterChanged.bind(this);

            this.commandHandler = new ns.EventCommandHandler(this);
            this.categories = new ns.CategoryList(options.categories);
            this.categories.addEventListener("currentchanged", this._onCategoryFilterChangedBound, false);
            this.service = options.service || ns.EventService;
            this.filter = new ns.EventListFilter(options.filter);
            this.fields = ns.EventListField.categories;
            this.filteredSelection = _observable(false);
            this.hasSelection = _pureComputed(this._hasSelection, this);
            this.watch = _observable(!!options.watch);
            this.counter = new ns.EventCounter();
            this.hideCounter = _Utilities.debounce(this.hideCounter, 100);

            this._filterObjectTypeSn = this.filter.objectType.subscribe(this.refresh, this);
            this._filterObjectSn = this.filter.object.subscribe(this.refresh, this);
            this._filterTimeIntervalSn = this.filter.timeInterval.subscribe(this.refresh, this);
            this._watchSn = this.watch.subscribe(this._onWatchChanged, this);

            this.refresh();
        }, {
            _onWatchChanged: function (value) {
                /// <signature>
                /// <summary>Raised immediately after the watch property is changed.</summary>
                /// <param name="value" type="Boolean" />
                /// </signature>
                if (value) {
                    if (!this._watcher) {
                        this._watcher = new ns.EventWatcher();
                        this._watcher.addEventListener("eventreceived", this._onEventReceivedBound, false);
                    }
                } else {
                    if (this._watcher) {
                        this._watcher.removeEventListener("eventreceived", this._onEventReceivedBound, false);
                        this._watcher.dispose();
                        this._watcher = null;
                    }
                }
            },
            _onEventReceived: function (event) {
                /// <signature>
                /// <summary>Raised immediately after an event is received.</summary>
                /// <param name="event" type="Event" />
                /// </signature>
                if (event.detail) {
                    this.unshift(event.detail);
                    this.counter && this.counter.add();
                }
            },
            _onCategoryFilterChanged: function (event) {
                /// <signature>
                /// <summary>Raised when category selection is changed</summary>
                /// </signature>
                var category = event.detail.newItem;
                var reserved = false;
                if (category && (category.exists() || (reserved = category.isReserved()))) {
                    this.exec({
                        name: "addToFilter",
                        what: "category",
                        item: {
                            // Preserve the reference to the reserved item
                            // It makes change detection immensely easier
                            category: reserved
                                ? ns.CategoryListItem.reserved
                                : category.toObject()
                        }
                    });
                }
            },
            fireSelectionChanged: function (selection) {
                /// <signature>
                /// <summary>Raises an onselectionchanged event.</summary>
                /// <param name="selection" type="Array" />
                /// </signature>
                if (selection.length !== this.items().length) {
                    this.filteredSelection(false);
                }
                _Knockout.PagedList.prototype.fireSelectionChanged.apply(this, arguments);
            },
            _hasSelection: function () {
                /// <signature>
                /// <summary>Returns true if there is at least one selected event.</summary>
                /// <returns type="Boolean" />
                /// </signature>
                return this.filteredSelection() || this.selection().length > 0;
            },
            mapItem: function (item) {
                /// <signature>
                /// <summary>Represents a function called before adding a new item to the list.</summary>
                /// <param name="item" type="Object">The item to map.</param>
                /// <returns type="PI.Identity.EventListItem" />
                /// </signature>
                return new ns.EventListItem(item);
            },
            exec: function (command) {
                /// <signature>
                /// <summary>Execute the command on the given item.</summary>
                /// <param name="command" type="Object" />
                /// <returns type="Boolean" />
                /// </signature>
                return this.commandHandler[command.name](command);
            },
            download: function () {
                /// <signature>
                /// <summary>Downloads the filtered result set.</summary>
                /// </signature>
                this.service.downloadFile(this._createParams(), this);
            },
            hideCounter: function () {
                /// <signature>
                /// <summary>Hides the counter.</summary>
                /// </signature>
                this.counter && this.counter.hide();
            },
            resetFilter: function () {
                /// <signature>
                /// <summary>Resets the filter</summary>
                /// </signature>
                this.filter.cancel();
                this.refresh();
            },
            refresh: _Promise.tasks.watch(function () {
                /// <signature>
                /// <summary>Loads all events to the list.</summary>
                /// </signature>
                var params = this._createParams();
                return this.service.getAllAsync({
                    fields: this.fields,
                    events: params,
                    categories: this.categories.filter.toObject()
                }, this).then(
                    function (response) {
                        this.filteredSelection(false);
                        this.total = response.events.total;
                        this.replaceAll.apply(this, response.events.data);
                        if ((this.fields & ns.EventListField.categories) === ns.EventListField.categories) {
                            this.categories.replaceAll.apply(this.categories, response.categories.data);
                            this.fields = ns.EventListField.none;
                        }
                    },
                    function () {
                        this.filteredSelection(false);
                        this.total = 0;
                        this.removeAll();
                    });
            }),
            _createParams: function () {
                /// <signature>
                /// <returns type="Object" />
                /// </signature>
                var params = this.filter.toObject() || {};
                params.dateFrom = params.dateFrom && params.dateFrom.toISOString();
                params.dateTo = params.dateTo && params.dateTo.toISOString();
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
                this.watch(false);
                this._watchSn && this.watchSn.dispose();
                this._watchSn = null;
                this._filterObjectTypeSn = this.filterObjectTypeSn.dispose();
                this._filterObjectTypeSn = null;
                this._filterObjectSn && this.filterObjectSn.dispose();
                this._filterObjectSn = null;
                this._filterTimeIntervalSn && this.filterTimeIntervalSn.dispose();
                this._filterTimeIntervalSn = null;

                this.hasSelection && this.hasSelection.dispose();
                this.hasSelection = null;
                this.categories && this.categories.removeEventListener("currentchanged", this._onCategoryFilterChangedBound, false);
                this._disposed = true;
            }
        }),

        //
        // Categories
        //

        CategorySortOrder: {
            none: "none",
            recent: "recent",
            name: "name"
        },

        CategoryField: {
            none: "none"
        },

        CategoryListItem: _Class.define(function CategoryListItem_ctor(category, options) {
            /// <signature>
            /// <summary>Initializes a new instance of the EventCategory class.</summary>
            /// <param name="category" type="Object" />
            /// <param name="options" type="Object" />
            /// <returns type="PI.Logging.EventCategory" />
            /// </signature>
            category = category || {};
            _Utilities.setOptions(this, options = options || {});
            _Promise.tasks(this);

            this.autoSave = options.autoSave !== false;
            this.service = options.service || ns.CategoryService;

            this.id = _observable();
            this.name = _observable().extend({ required: true });
            this.color = _observable();
            this.modifiedDate = _observable();

            this.tooltip = _pureComputed(this._getTooltip, this);

            this.errors = _KO.validation.group(this);
            this.editing = _observable(false);
            this.exists = _pureComputed(this._exists, this);

            this.update(category);

            this._colorChangedSn = this.color.subscribe(this._colorChanged, this);
        }, {
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
            // Data Operations
            //

            update: function (category) {
                /// <signature>
                /// <param name="category" type="Object" />
                /// </signature>
                category = category || {};
                this._updating = true;
                this.id(category.id);
                this.name(category.name);
                this.color(category.color);
                this.modifiedDate(category.modifiedDate);
                this._updating = false;
            },
            toObject: function () {
                /// <signature>
                /// <returns type="Object" />
                /// </signature>
                return {
                    id: this.id(),
                    name: this.name(),
                    color: this.color(),
                    modifiedDate: this.modifiedDate()
                };
            },

            //
            // Storage Operations
            //

            saveAsync: _Promise.tasks.watch(function () {
                /// <signature>
                /// <summary>Saves this category</summary>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                if (!this.validate()) {
                    return _Promise.error();
                }
                var category = this.toObject();
                if (this.exists()) {
                    return this.service.updateAsync(category, this)
                        .then(function (category) {
                            this.update(category);
                            this.dispatchEvent("saved", { state: "modified" });
                        });
                }
                return this.service.createAsync(category, this)
                    .then(function (category) {
                        this.update(category);
                        this.dispatchEvent("saved", { state: "added" });
                    });
            }),
            deleteAsync: _Promise.tasks.watch(function () {
                /// <signature>
                /// <summary>Deletes this category</summary>
                /// <returns type="WinJS.Promise" />
                /// </signature>
                if (this.exists()) {
                    return this.service.deleteAsync(this.id(), this)
                        .then(function () {
                            this.update();
                            this.dispatchEvent("deleted");
                        });
                }
                return _Promise.error(logResources.categoryDoesNotExist());
            }),

            //
            // Event & Computed Members
            //

            _getTooltip: function () {
                /// <signature>
                /// <returns type="String" />
                /// </signature>
                return this.name() + " # " + this.id();
            },
            _colorChanged: function () {
                /// <signature>
                /// <summary>Occurs when the color changes.</summary>
                /// </signature>
                this.autoSave && !this._updating && this.saveAsync();
            },
            _exists: function () {
                /// <signature>
                /// <summary>Returns true if this event exists</summary>
                /// <returns type="Boolean" />
                /// </signature>
                return !this.isReserved() && !!this.id();
            },
            isReserved: function () {
                /// <signature>
                /// <summary>Returns true if this event is reserved</summary>
                /// <returns type="Boolean" />
                /// </signature>
                return this.id() === ns.CategoryListItem.reserved.id;
            },

            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                if (this._disposed) {
                    return;
                }
                this._colorChangedSn && this._colorChangedSn.dispose();
                this._colorChangedSn = null;
                this._disposed = true;
            }
        }, {
            reserved: {
                id: null,
                projectId: null,
                name: logResources.categoryReservedName(),
                color: "#fff"
            }
        }),

        CategoryListFilter: _Class.define(function CategoryListFilter_ctor(options) {
            /// <signature>
            /// <summary>Initializes a new instance of the CategoryListFilter class.</summary>
            /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the CategoryListFilter.</param>
            /// <returns type="PI.Logging.CategoryListFilter" />
            /// </signature>
            this.options = options = options || {};
            this.project = _observable(options.project);
        }, {
            toObject: function () {
                /// <signature>
                /// <summary>Returns a pure JS object without KO observable functions</summary>
                /// <returns type="Object" />
                /// </signature>
                var project = this.project();
                return {
                    projectId: project && project.id
                };
            }
        }),

        CategoryList: _Class.derive(_Knockout.List, function CategoryList_ctor(options) {
            /// <signature>
            /// <summary>Initializes a new instance of the CategoryList class.</summary>
            /// <param name="options" type="Object" />
            /// <returns type="PI.Logging.CategoryList" />
            /// </signature>
            options = options || {};
            _Knockout.List.call(this, options);
            this.service = options.service || ns.CategoryService;
            this.filter = new ns.CategoryListFilter(options.filter);
        }, {
            /// <field type="PI.Logging.CategoryService" />
            service: null,
            /// <field type="PI.Logging.CategoryListFilter" />
            filter: null,
            mapItem: function (item) {
                /// <signature>
                /// <summary>Represents a function called before adding a new item to the list.</summary>
                /// <param name="item" type="Object">The item to map.</param>
                /// <returns type="PI.Identity.CategoryListItem" />
                /// </signature>
                return new ns.CategoryListItem(item, { service: this.service });
            },
            addNew: function () {
                /// <signature>
                /// <summary>Adds a new item to the list</summary>
                /// </signature>
                var project = this.filter.project();
                _Knockout.List.prototype.addNew.call(this, {
                    projectId: project && project.id,
                    name: logResources.categoryName(this.items().length)
                });
            },
            removeAll: function () {
                /// <signature>
                /// <summary>Removes all items from the list without saving the pending added/edited item.</summary>
                /// <returns type="Array">The deleted elements.</returns>
                /// </signature>
                var removedItems = _Knockout.List.prototype.removeAll.apply(this, arguments);
                this.splice(0, 0, ns.CategoryListItem.reserved);
                return removedItems;
            },
            deleteAsync: function (category) {
                /// <signature>
                /// <summary>Deletes the given category</summary>
                /// <param name="category" type="PI.Identity.CategoryListItem" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                var that = this;
                return category.deleteAsync()
                    .then(function () {
                        that.remove(category);
                    });
            }
        })

    });

    _Class.mix(ns.CategoryListItem, _Utilities.createEventProperties("saved", "deleted"));
    _Class.mix(ns.CategoryListItem, _Utilities.eventMixin);

})(ko, WinJS, PI);