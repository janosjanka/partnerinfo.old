// Copyright (c) Partnerinfo TV. All Rights Reserved.

(function (_Global, _KO, _WinJS, _PI) {
    "use strict";

    var _Class = _WinJS.Class;
    var _Knockout = _WinJS.Knockout;

    var ns = _WinJS.Namespace.defineWithParent(_PI, "Portal", {

        ReferenceItem: _Class.define(function ReferenceItem_ctor(reference) {
            /// <signature>
            /// <param name="reference" type="Object" />
            /// <returns type="PI.Portal.ReferenceItem" />
            /// </signature>
            this.type = reference.type;
            this.uri = reference.uri;
        }),

        ReferenceList: _Class.derive(_Knockout.List, function ReferenceList_ctor(options) {
            /// <signature>
            /// <param name="options" type="Object" />
            /// <returns type="PI.Portal.ReferenceList" />
            /// </signature>
            _Knockout.List.call(this, options);
        }, {
            mapItem: function (item) {
                /// <signature>
                /// <param name="item" type="Object" />
                /// <returns type="PI.Portal.ReferenceItem" />
                /// </signature>
                return new ns.ReferenceItem(item);
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

})(window, ko, WinJS, PI);