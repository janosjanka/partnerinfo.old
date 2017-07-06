// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="../base.js" />

PI.Portal.Modules.register("search", {
    createModuleOptions: function (options) {
        /// <signature>
        /// <param name="options" type="Object" optional="true" />
        /// <returns type="Object" />
        /// </signature>
        return $.extend(this._super(options), {
            facebookAppId: null,
            googleApiKey: null,
            editMode: null,
            autoHideMenu: false,
            logActionUrl: null,
            loginVisible: true,
            loginActionUrl: null,
            bannerEnabled: true,
            providerVisible: true,
            searchVisible: true,
            searchQuery: null,
            filterVisible: true,
            pagerVisible: true,
            playlistEnabled: true,
            playlistVisible: false,
            playlistUri: null,
            playlistContact: null,
            playlistMode: null,
            playlistLimit: null,
            playlistItemLimit: null,
            playlistDropImgUrl: null,
            hashPrefix: null
        }, options);
    },
    _activate: function () {
        /// <signature>
        /// <summary>Activates this module.</summary>
        /// </signature>
        var options = this.getModuleOptions();
        PI.ui({
            name: "search",
            element: this.element,
            model: {
                facebookAppId: options.facebookAppId,
                googleApiKey: options.googleApiKey,
                editMode: options.editMode,
                autoHideMenu: options.autoHideMenu,
                logActionUrl: options.logActionUrl,
                loginVisible: options.loginVisible,
                loginActionUrl: options.loginActionUrl || options.signUpActionUrl,
                bannerEnabled: options.bannerEnabled,
                providerVisible: options.providerVisible,
                searchVisible: options.searchVisible,
                searchQuery: options.searchQuery,
                filterVisible: options.filterVisible,
                pagerVisible: options.pagerVisible,
                playlistEnabled: options.playlistEnabled,
                playlistVisible: options.playlistVisible,
                playlistUri: options.playlistUri,
                playlistContact: options.playlistContact,
                playlistMode: options.playlistMode,
                playlistLimit: options.playlistLimit,
                playlistItemLimit: options.playlistItemLimit,
                playlistDropImgUrl: options.playlistDropImgUrl,
                hashPrefix: options.hashPrefix
            }
        });
    },
    _deactivate: function () {
        /// <signature>
        /// <summary>Deactivates this module.</summary>
        /// </signature>
        this.element.empty();
    }
});