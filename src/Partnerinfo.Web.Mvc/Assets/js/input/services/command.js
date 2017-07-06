// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

WinJS.Namespace.defineWithParent(PI, "Input.CommandService", {

    invokeAsync: function (uri, rollback, thisArg) {
        /// <signature>
        /// <summary>Invokes the given command</summary>
        /// <param name="uri" type="String" />
        /// <param name="rollback" type="Boolean" optional="true" value="false" />
        /// <param name="thisArg" type="Object" optional="true" />
        /// <returns type="WinJS.Promise" />
        /// </signature>
        return PI.api({
            method: "GET",
            path: "/c/{uri}",
            data: {
                uri: uri,
                rollback: rollback
            }
        }, thisArg);
    },
    createAsync: function (request, thisArg) {
        /// <signature>
        /// <summary>Creates a new command object</summary>
        /// <param name="request" type="Object">
        ///     <para>mail: Object</para>
        ///     <para>command: Object</para>
        ///     <para>returnUrl: String</para>
        /// </param>
        /// <param name="thisArg" type="Object" optional="true" />
        /// <returns type="WinJS.Promise" />
        /// </signature>
        return PI.api({
            method: "POST",
            path: "input/commands",
            data: request
        }, thisArg);
    },
    generateLine: function (operation, objects) {
        /// <signature>
        /// <summary>Generates a valid command line text</summary>
        /// <param name="operation" type="String" />
        /// <param name="objects" type="Array" optional="true" />
        /// <returns type="String" />
        /// </signature>
        if (!operation) {
            throw new TypeError("invalid operation");
        }
        if (!objects) {
            return operation;
        }
        var str = operation;
        str += " ! ";
        for (var i = 0, len = objects.length; i < len; ++i) {
            var obj = objects[i];
            str += obj.type;
            str += ": ";
            str += obj.id;
            if (i < len - 1) {
                str += " >> ";
            }
        }
        return str;
    },

});
