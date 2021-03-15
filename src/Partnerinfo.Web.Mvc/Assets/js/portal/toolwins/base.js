// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="../designer.js" />

(function (_Global, _WinJS, _PI, _Portal, $) {
    "use strict";

    var toolWinRes = _T("portal.toolWins.base") || {};

    $.widget("PI.PortalToolWin", {
        options: {
            /// <field type="$.PI.PortalDesigner" />
            designer: null,
            /// <field type="PI.Portal.ToolWinOptions" />
            state: null,
            /// <field type="String" />
            css: null,
            /// <field type="String" />
            icon: null,
            /// <field type="Object" />
            resources: null
        },
        _onAlign: $.noop,
        _onRender: $.noop,
        _onRefresh: $.noop,
        _onSelectionChanged: $.noop,
        _create: function () {
            /// <signature>
            /// <summary>Initializes a new instance of the tool window.</summary>
            /// </signature>
            if (!this.options.designer) {
                throw new TypeError("designer is required");
            }

            this._applyStateBound = this.applyState.bind(this);
            this._onRefreshBound = this._onRefresh.bind(this);
            this._onSelectionChangedBound = this._onSelectionChanged.bind(this);
            this._promise = $.Deferred();

            this.toolWinHead = $("<div>")
                .addClass("ui-portal-toolwin-head")
                .on("mousedown.PortalToolWin", this._headMouseDown.bind(this))
                .appendTo(this.element);
            this.toolWinHeadIcon = $("<i>").appendTo(this.toolWinHead);
            this.toolWinHeadTitle = $("<h2>").appendTo(this.toolWinHead);
            this.toolWinHeadMenu = $("<div>")
                .attr("role", "group")
                .addClass("ui-btn-group ui-btn-group-xs")
                .appendTo(this.toolWinHead);
            this.toolWinHeadMenu.append(
                $("<button>")
                    .addClass("ui-btn ui-btn-flat")
                    .attr("title", toolWinRes.switchDM)
                    .append($("<i>").addClass("i portal toolwin-btn-mode"))
                    .on("click.PortalToolWin", this.toggleDisplay.bind(this)),
                $("<button>")
                    .addClass("ui-btn ui-btn-flat")
                    .attr("title", toolWinRes.collapse)
                    .append($("<i>").addClass("i portal toolwin-btn-close"))
                    .on("click.PortalToolWin", this.close.bind(this)));

            this.toolWinMenu = $("<div>")
                .addClass("ui-portal-toolwin-menu")
                .appendTo(this.element);
            this.toolWinBody = $("<div>")
                .addClass("ui-portal-toolwin-body")
                .appendTo(this.element);
            this.element
				.addClass("ui-portal-toolwin")
                .addClass(this.options.css)
				.draggable({
				    stack: ".ui-portal-toolwin",
				    snap: true,
				    handle: this.toolWinHead,
				    stop: this._draggableStop.bind(this)
				})
				.resizable({
				    handles: "n,e,s,w",
				    stop: this._resizableStop.bind(this)
				});

            this._setupIcon(this.options.icon);
            this._setupTitle();
            this.applyState();
            this.initialize();
        },
        _initAsync: function () {
            /// <signature>
            /// <summary>Initializes the tool window.</summary>
            /// <returns type="WinJS.Promise" />
            /// </signature>
            return _WinJS.Promise.complete();
        },
        initialize: function () {
            /// <signature>
            /// <summary>Fires an onitialize event.</summary>
            /// </signature>
            if (this.initialized) {
                return;
            }
            var that = this;
            this._initAsync().then(function () {
                that.initialized = true;
                that._promise.resolve();
                that.options.state.addEventListener("changed", that._applyStateBound, false);
                that.options.designer.options.selection.addEventListener("changed", that._onSelectionChangedBound, false);
                that.options.designer.addEventListener("rendered", that._onRefreshBound, false);
                that._onRender();
                that.options.designer.rendered && that._onRefresh();
            });
        },
        applyState: function () {
            /// <signature>
            /// <summary>Applies the specified state to the tool window.</summary>
            /// </signature>
            var state = this.options.state;
            if (state.display === _Portal.WinDisplay.transparent) {
                this.element.addClass("transparent");
            } else {
                this.element.removeClass("transparent");
            }
            this.element.css({
                position: "fixed", // jQuery.draggable makes the element relative
                left: state.offsetX,
                top: state.offsetY,
                width: state.width,
                height: state.height
            });
        },
        toggleDisplay: function () {
            /// <signature>
            /// <summary>Toggles display mode.</summary>
            /// </signature>
            this.options.state.setOption("display",
                this.options.state.display = this.options.state.display === _Portal.WinDisplay.opaque
                    ? _Portal.WinDisplay.transparent
                    : _Portal.WinDisplay.opaque)
                .save();
        },
        close: function () {
            /// <signature>
            /// <summary>Closes this window.</summary>
            /// </signature>
            this.options.state.setOption("display", _Portal.WinDisplay.none).save();
        },
        refresh: function () {
            /// <signature>
            /// <summary>Refreshes the control.</summary>
            /// </signature>
            this._promise.done(this._onRefreshBound);
        },
        _setOption: function (key, value) {
            /// <signature>
            /// <summary>Sets the specified value for the property of the current tool window.</summary>
            /// </signature>
            if (key === "designer") {
                throw new TypeError("designer is immutable");
            }
            this._super(key, value);
            if (key === "icon") {
                this._setupIcon(value);
            } else if (key === "title") {
                this._setupTitle(value);
            }
        },
        _setupIcon: function (icon) {
            /// <signature>
            /// <summary>Sets the specified icon for the current tool window.</summary>
            /// <param name="icon" type="String" />
            /// </signature>
            this.toolWinHeadIcon.attr("class", icon);
        },
        _setupTitle: function (title) {
            /// <signature>
            /// <summary>Sets the specified title for the current tool window.</summary>
            /// <param name="title" type="String" />
            /// </signature>
            this.toolWinHeadTitle.text(title || this.options.resources && this.options.resources.name);
        },
        _headMouseDown: function (event) {
            /// <signature>
            /// <summary>Brings the control to the front of the z-order.</summary>
            /// </signature>
            var widget = this.element.data("ui-draggable");
            widget._mouseStart(event);
            widget._mouseDrag(event);
            widget._mouseStop(event);
        },
        _draggableStop: function () {
            /// <signature>
            /// <summary>Triggered when resizing stops.</summary>
            /// </signature>
            this._savePosition();
        },
        _resizableStop: function () {
            /// <signature>
            /// <summary>Triggered when dragging stops.</summary>
            /// </signature>
            this._savePosition();
            _Global.dispatchEvent(new Event("resize"));
        },
        _savePosition: function () {
            /// <signature>
            /// <summary>Saves the window position.</summary>
            /// </signature>
            this.options.state.setOptions({
                offsetX: this.element.css("left"),
                offsetY: this.element.css("top"),
                width: this.element.css("width"),
                height: this.element.css("height")
            }).save();
        },
        _destroy: function () {
            /// <signature>
            /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            /// </signature>
            this.options.designer.removeEventListener("rendered", this._onRefreshBound, false);
            this.options.designer.options.selection.removeEventListener("changed", this._onSelectionChangedBound, false);
            this.options.state.removeEventListener("changed", this._applyStateBound, false);

            this.element
                .draggable("destroy")
                .resizable("destroy")
                .empty()
                .removeClass("ui-portal-toolwin");
        }
    });

})(window, WinJS, PI, PI.Portal, jQuery);