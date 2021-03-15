// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

tinymce.PluginManager.add("placeholders", WinJS.Class.define(function (editor) {
    /// <signature>
    /// <param name="editor" type="tinymce.Editor" />
    /// </signature>
    var res = _T("tinymce.placeholders");
    var exp = PI.ExpressionType;
    editor.addButton("placeholders", {
        type: "menubutton",
        image: PI.Server.mapPath("/ss/tinymce/plugins/placeholders.png"),
        title: res.insert,
        menu: [{
            text: exp.from,
            menu: [
                this._createItem(editor, exp.from, res.contact.name),
                this._createItem(editor, exp.from, res.contact.email)
            ]
        }, {
            text: exp.to,
            menu: [
                this._createItem(editor, exp.to, res.contact.name),
                this._createItem(editor, exp.to, res.contact.email),
                this._createItem(editor, exp.to, res.contact.lastName),
                this._createItem(editor, exp.to, res.contact.firstName),
                this._createItem(editor, exp.to, res.contact.gender),
                this._createItem(editor, exp.to, res.contact.birthday),
                this._createItem(editor, exp.to, res.contact.phones.personal),
                this._createItem(editor, exp.to, res.contact.phones.business),
                this._createItem(editor, exp.to, res.contact.phones.mobile),
                this._createItem(editor, exp.to, res.contact.phones.other),
                this._createItem(editor, exp.to, res.contact.comment)
            ]
        }, {
            icon: "email",
            text: exp.invitation,
            menu: [
                this._createItem(editor, exp.invitation, res.invitation.message)
            ]
        }]
    });
}, {
    _createItem: function (editor, type, property) {
        /// <signature>
        /// <param name="editor" type="tinymce.Editor" />
        /// <param name="type" type="String" />
        /// <param name="property" type="String" />
        /// <returns type="Object" />
        /// </signature>
        return {
            text: property,
            onclick: function () {
                // TinyMCE crashes the expression, so we need to wrap it in a <span> tag
                var expr = PI.Expression.make(type, property);
                // Creates a html chunk and inserts it at the current selection/caret location
                editor.insertContent(editor.dom.createHTML("span", { "class": "pi-tmpl-field" }, expr) + "&nbsp;");
            }
        };
    }
}));
