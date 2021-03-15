// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

PI.Portal.Modules.extend("search", {
    _onCreateEditDialog: function (complete) {
        this._createEditDialog({
            model: {
                contacts: ko.observableArray(),
                create: function (module) {
                    if (module.playlistContact) {
                        this.contacts([module.playlistContact]);
                    }
                },
                submit: function (module) {
                    var contact = this.contacts()[0];
                    module.playlistContact = contact ? {
                        id: contact.id,
                        email: contact.email
                    } : null;
                }
            },
            complete: complete
        }, {
            width: 700
        });
    }
});