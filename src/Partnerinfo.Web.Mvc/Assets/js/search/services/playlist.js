// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (_WinJS, _PI) {
    "use strict";

    var _Class = _WinJS.Class;

    _WinJS.Namespace.defineWithParent(_PI, "Project.Search", {

        PlaylistService: _Class.define(function PlaylistService_ctor(token) {
            /// <signature>
            /// <summary>Initializes a new instance of the PlaylistService class.</summary>
            /// <param name="token" type="String" optional="true">Authorization token.</param>
            /// <returns type="PlaylistService" />
            /// </signature>
            this.token = token;
        }, {
            getByIdAsync: function (id, thisArg) {
                /// <signature>
                /// <summary>Gets a playlist by ID</summary>
                /// <param name="id" type="Number" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="$.Deferred" />
                /// </signature>
                return _PI.api({
                    auth: this._getToken(),
                    method: "GET",
                    path: "project/playlists/" + id
                }, thisArg);
            },
            getDefaultByContactAsync: function (contactId, thisArg) {
                /// <signature>
                /// <param name="contactId" type="Number" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="$.Deferred" />
                /// </signature>
                return _PI.api({
                    auth: this._getToken(),
                    method: "GET",
                    path: "project/playlists/default/" + contactId
                }, thisArg);
            },
            getByUriAsync: function (uri, thisArg) {
                /// <signature>
                /// <summary>Gets a playlist by URI</summary>
                /// <param name="uri" type="String" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="$.Deferred" />
                /// </signature>
                return _PI.api({
                    auth: this._getToken(),
                    method: "GET",
                    path: "project/playlists/" + uri
                }, thisArg);
            },
            getAllAsync: function (filter, thisArg) {
                /// <signature>
                /// <summary>Gets all playlists</summary>
                /// <param name="filter" type="Object" optional="true" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="$.Deferred" />
                /// </signature>
                return _PI.api({
                    auth: this._getToken(),
                    method: "GET",
                    path: "project/playlists",
                    data: filter
                }, thisArg);
            },
            createAsync: function (playlist, thisArg) {
                /// <signature>
                /// <param name="playlist" type="Object" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="$.Deferred" />
                /// </signature>
                return _PI.api({
                    auth: this._getToken(),
                    method: "POST",
                    path: "project/playlists",
                    data: playlist
                }, thisArg);
            },
            updateAsync: function (id, playlist, thisArg) {
                /// <signature>
                /// <param name="id" type="Number" />
                /// <param name="playlist" type="Object" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="$.Deferred" />
                /// </signature>
                return _PI.api({
                    auth: this._getToken(),
                    method: "PUT",
                    path: "project/playlists/" + id,
                    data: playlist
                }, thisArg);
            },
            setAsDefaultAsync: function (id, thisArg) {
                /// <signature>
                /// <param name="contactId" type="Number" />
                /// <param name="id" type="Number" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="$.Deferred" />
                /// </signature>
                return _PI.api({
                    auth: this._getToken(),
                    method: "POST",
                    path: "project/playlists/" + id + "/default"
                }, thisArg);
            },
            deleteAsync: function (id, thisArg) {
                /// <signature>
                /// <param name="id" type="Number" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="$.Deferred" />
                /// </signature>
                return _PI.api({
                    auth: this._getToken(),
                    method: "DELETE",
                    path: "project/playlists/" + id
                }, thisArg);
            },

            //
            // Playlist Items
            //

            getAllItemsAsync: function (playlistId, filter, thisArg) {
                /// <signature>
                /// <summary>Gets all playlist items</summary>
                /// <param name="playlistId" type="Number" />
                /// <param name="filter" type="Object" optional="true" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="$.Deferred" />
                /// </signature>
                return _PI.api({
                    auth: this._getToken(),
                    method: "GET",
                    path: "project/playlists/" + playlistId + "/items",
                    data: filter
                }, thisArg);
            },
            getAllItemsByContact: function (contactId, filter, thisArg) {
                /// <signature>
                /// <summary>Gets all playlist items</summary>
                /// <param name="contactId" type="Number" />
                /// <param name="filter" type="Object" optional="true" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="$.Deferred" />
                /// </signature>
                return _PI.api({
                    auth: this._getToken(),
                    method: "GET",
                    path: "project/contacts/" + contactId + "/playlists/items",
                    data: filter
                }, thisArg);
            },
            saveAllItemsAsync: function (playlistId, items, thisArg) {
                /// <signature>
                /// <summary>Gets all playlist items</summary>
                /// <param name="playlistId" type="Number" />
                /// <param name="filter" type="Object" optional="true" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="$.Deferred" />
                /// </signature>
                return _PI.api({
                    auth: this._getToken(),
                    method: "POST",
                    path: "project/playlists/" + playlistId + "/items",
                    data: items
                }, thisArg);
            },
            moveItemAsync: function (id, sortOrderId, thisArg) {
                /// <signature>
                /// <summary>Moves a playlist item to the given position.</summary>
                /// <param name="id" type="Number" />
                /// <param name="sortOrderId" type="Number" />
                /// <param name="thisArg" type="Object" optional="true" />
                /// <returns type="$.Deferred" />
                /// </signature>
                return _PI.api({
                    auth: this._getToken(),
                    method: "MOVE",
                    path: "project/playlists-items/" + id + "/" + sortOrderId
                }, thisArg);
            },

            //
            // Private Members
            //

            _getToken: function () {
                /// <signature>
                /// <summary>Gets the current authorization token.</summary>
                /// <returns type="String" />
                /// </signature>
                return this.token || _PI.Project.Security.getToken();
            }
        })

    });

})(WinJS, PI);