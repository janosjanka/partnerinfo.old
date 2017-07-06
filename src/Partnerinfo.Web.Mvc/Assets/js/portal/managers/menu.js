// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (_Base) {
    "use strict";

    var _Class = _Base.Class;
    var _Utilities = _Base.Utilities;

    var _observable = ko.observable;

    var ns = _Base.Namespace.defineWithParent(PI, "Portal", {
        MenuManager: _Class.define(function MenuManager_ctor(designer, options) {
            /// <signature>
            /// <summary>Initializes a new instance of the MenuManager class.</summary>
            /// <param name="designer" type="$.PI.PortalDesigner" optional="true" />
            /// <param name="options" type="Object" optional="true">The set of options to be applied initially to the MenuManager.</param>
            /// <returns type="MenuManager" />
            /// </signature>
            options = options || {};

            this._disposed = false;
            this._designer = designer;
            this._editSessions = 0;

            this.editing = _observable(false);
            this.mode = _observable(options.mode);
            this.renderMaster = _observable(!!options.renderMaster);
            this.hiddenVisible = _observable(!!options.hiddenVisible);

            _Utilities.setOptions(this, options);

            this._renderMasterSn = this.renderMaster.subscribe(this._onChanged.bind(this, "renderMaster"));
            this._hiddenVisibleSn = this.hiddenVisible.subscribe(this._onChanged.bind(this, "hiddenVisible"));
        }, {
            _onChanged: function (property, value) {
                /// <signature>
                /// <param name="property" type="String" />
                /// <param name="value" type="Object" />
                /// </signature>
                this.dispatchEvent("changed", { property: property, value: value });
            },
            setEditing: function (editing) {
                /// <signature>
                /// <param name="editing" type="Boolean" />
                /// </signature>
                this._editSessions += editing ? +1 : -1;
                this.editing(this._editSessions > 0);
            },
            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                if (this._disposed) {
                    return;
                }
                this._renderMasterSn && this._renderMasterSn.dispose();
                this._hiddenVisibleSn && this._hiddenVisibleSn.dispose();
                this._renderMasterSn = null;
                this._hiddenVisibleSn = null;
                this._disposed = true;
            }
        })
    });

    _Class.mix(ns.MenuManager, _Utilities.createEventProperties("changed"));
    _Class.mix(ns.MenuManager, _Utilities.eventMixin);

})(WinJS);