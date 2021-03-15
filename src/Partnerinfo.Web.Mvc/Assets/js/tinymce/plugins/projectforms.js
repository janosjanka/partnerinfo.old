// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function () {
    "use strict";

    function createElement(tagName, innerHTML, options) {
        /// <signature>
        /// <param name="tagName" type="String" />
        /// <param name="innerHTML" type="String" />
        /// <param name="options" type="String" />
        /// </signature>
        var element = document.createElement(tagName);
        innerHTML && (element.innerHTML = innerHTML);
        if (arguments.length > 2) {
            for (var p in options) {
                if (options.hasOwnProperty(p)) {
                    if (p === "innerHTML") {
                        element.innerHTML = options[p];
                    } else {
                        element.setAttribute(p, options[p]);
                    }
                }
            }
        }
        return element;
    }
    function writeRow(table) {
        /// <signature>
        /// <param name="table" type="HTMLTableElement" />
        /// <returns type="HTMLTableRowElement" />
        /// </signature>
        return table.insertRow();
    }
    function writeCell(row, innerHTML) {
        /// <signature>
        /// <param name="row" type="HTMLTableRowElement" />
        /// <param name="innerHTML" type="String" optional="true" />
        /// <returns type="HTMLTableCellElement" />
        /// </signature>
        var cell = row.insertCell();
        innerHTML && (cell.innerHTML = innerHTML);
        return cell;
    }
    function writeInputCell(row, options) {
        /// <signature>
        /// <param name="row" type="HTMLTableRowElement" />
        /// <param name="options" type="Object" />
        /// <returns type="HTMLTableCellElement" />
        /// </signature>
        var cell = row.insertCell();
        var input = createElement("input", null, options);
        cell.appendChild(input);
        return cell;
    }
    function writeTextAreaCell(row, options) {
        /// <signature>
        /// <param name="row" type="HTMLTableRowElement" />
        /// <param name="options" type="Object" />
        /// <returns type="HTMLTableCellElement" />
        /// </signature>
        var cell = row.insertCell();
        var textarea = createElement("textarea", null, options);
        cell.appendChild(textarea);
        return cell;
    }
    function writeSelectCell(row, options, selectOptions) {
        /// <signature>
        /// <param name="row" type="HTMLTableRowElement" />
        /// <param name="options" type="Object" />
        /// <param name="selectOptions" type="Array" />
        /// <returns type="HTMLTableCellElement" />
        /// </signature>
        var cell = row.insertCell();
        var select = createElement("select", null, options);
        for (var i = 0, len = selectOptions.length; i < len; ++i) {
            var option = selectOptions[i];
            select.appendChild(createElement("option", option.innerHTML, option));
        }
        cell.appendChild(select);
        return cell;
    }
    function writeButtonCell(row, options) {
        /// <signature>
        /// <param name="row" type="HTMLTableRowElement" />
        /// <param name="options" type="Object" />
        /// <returns type="HTMLTableCellElement" />
        /// </signature>
        var cell = row.insertCell();
        cell.appendChild(createElement("button", null, options));
        return cell;
    }

    var ActFields = {
        name: 1 << 0,
        email: 1 << 1,
        other: 1 << 2
    };

    tinymce.PluginManager.add("projectforms", WinJS.Class.define(function (editor) {
        /// <signature>
        /// <param name="editor" type="tinymce.Editor" />
        /// </signature>
        this.res = _T("tinymce.projectforms") || {};
        this.editor = editor;
        this.editor.addButton("projectforms", {
            type: "menubutton",
            image: PI.Server.mapPath("/ss/tinymce/plugins/projectforms.png"),
            title: this.res.insert,
            menu: [{
                text: this.res.types.onlyName,
                onclick: this._insertActForm.bind(this, ActFields.name)
            }, {
                text: this.res.types.onlyEmail,
                onclick: this._insertActForm.bind(this, ActFields.email)
            }, {
                text: this.res.types.mailAddress,
                onclick: this._insertActForm.bind(this, ActFields.name | ActFields.email)
            }, {
                text: this.res.types.full,
                onclick: this._insertActForm.bind(this, ActFields.name | ActFields.email | ActFields.other)
            }, {
                text: "-"
            }, {
                text: this.res.types.invitation,
                onclick: this._insertInvForm.bind(this, 10)
            }]
        });
    }, {
        _insertActForm: function (fields) {
            /// <signature>
            /// <param name="fields" type="Number" />
            /// </signature>
            var table = this._createTable({ style: "width: 100px" }, {});
            var row, cell;

            if ((fields & ActFields.name) === ActFields.name) {
                row = writeRow(table);
                writeCell(row, "Név");
                writeInputCell(row, { type: "text", name: "contact.email.name", maxlength: "128", required: true });
            }
            if ((fields & ActFields.email) === ActFields.email) {
                row = writeRow(table);
                writeCell(row, "Email");
                writeInputCell(row, { type: "email", name: "contact.email.address", maxlength: "128", required: true });
            }
            if ((fields & ActFields.other) === ActFields.other) {
                row = writeRow(table);
                writeCell(row, "Vezetéknév");
                writeInputCell(row, { type: "text", name: "contact.lastName", maxlength: "64" });
                row = writeRow(table);
                writeCell(row, "Keresztnév");
                writeInputCell(row, { type: "text", name: "contact.firstName", maxlength: "64" });
                row = writeRow(table);
                writeCell(row, "Neme");
                writeSelectCell(row, { name: "contact.gender" }, [
                    { value: "unknown" },
                    { value: "male", innerHTML: "Férfi" },
                    { value: "female", innerHTML: "Nő" }
                ]);
                row = writeRow(table);
                writeCell(row, "Születésnap");
                writeInputCell(row, { type: "date", name: "contact.birthday", min: 1900 });
                row = writeRow(table);
                writeCell(row, "Magán telefon");
                writeInputCell(row, { type: "tel", name: "contact.phones.personal", maxlength: 16 });
                row = writeRow(table);
                writeCell(row, "Üzleti telefon");
                writeInputCell(row, { type: "tel", name: "contact.phones.business", maxlength: 16 });
                row = writeRow(table);
                writeCell(row, "Mobil telefon");
                writeInputCell(row, { type: "tel", name: "contact.phones.mobile", maxlength: 16 });
                row = writeRow(table);
                writeCell(row, "Egyéb telefon");
                writeInputCell(row, { type: "tel", name: "contact.phones.other", maxlength: 16 });
                row = writeRow(table);
                cell = writeTextAreaCell(row, { name: "contact.comment", rows: 3, maxlength: 512 });
                cell.setAttribute("colspan", 2);
            }
            row = writeRow(table);
            cell = writeButtonCell(row, { type: "submit", innerHTML: "Tovább &gt;&gt;" });
            cell.setAttribute("colspan", 2);
            cell.setAttribute("style", "text-align: center");

            // Creates a html chunk and inserts it at the current selection/caret location
            this.editor.insertContent(this.editor.dom.getOuterHTML(table));
        },
        _insertInvForm: function (type) {
            /// <signature>
            /// <summary>Writes an action table.</summary>
            /// </signature>
            var invitation = this.res.invitation;
            var container = document.createDocumentFragment();
            var table = container.appendChild(this._createTable({ style: "width: 150px" }, {}));
            var row, cell;

            row = writeRow(table);
            writeCell(row, invitation.sponsorName);
            writeInputCell(row, { type: "text", name: "contact.email.name", maxlength: "128" });
            row = writeRow(table);
            writeCell(row, invitation.sponsorEmail);
            writeInputCell(row, { type: "email", name: "contact.email.address", maxlength: "128" });
            row = writeRow(table);
            writeCell(row, invitation.message);
            writeTextAreaCell(row, { name: "invitation.message", rows: 3, maxlength: "512", innerHTML: invitation.messageContent });

            table = container.appendChild(this._createTable({ style: "width: 150px" }, {}, {}));
            row = writeRow(table);
            writeCell(row);
            writeCell(row, invitation.invitedName);
            writeCell(row, invitation.invitedEmail);
            for (var i = 1; i < 4; ++i) {
                row = writeRow(table);
                writeCell(row, String.format(invitation.invited, i));
                writeInputCell(row, { type: "text", name: "invitation.to[" + i + "].email.name", maxlength: "128" });
                writeInputCell(row, { type: "email", name: "invitation.to[" + i + "].email.address", maxlength: "128" });
            }

            table = container.appendChild(this._createTable({ style: "width: 150px" }, {}));
            row = writeRow(table);
            cell = writeButtonCell(row, { type: "submit", innerHTML: invitation.submit });
            cell.setAttribute("colspan", 2);
            cell.setAttribute("style", "text-align: center");

            // Creates a html chunk and inserts it at the current selection/caret location
            this.editor.insertContent(this.editor.dom.getOuterHTML(container));
        },
        _createTable: function () {
            /// <signature>
            /// <summary>Creates a new table element.</summary>
            /// <param name="columns" type="Array" />
            /// <returns type="HTMLTableElement" />
            /// </signature>
            var table = createElement("table", null, {
                style: "table-layout: fixed; width: 100%;",
                cellspacing: "0",
                cellpadding: "4"
            });
            var colgroup = table.appendChild(createElement("colgroup"));
            for (var i = 0, len = arguments.length; i < len; ++i) {
                colgroup.appendChild(createElement("col", null, arguments[i]));
            }
            table.createTBody();
            return table;
        }
    }));

})();
