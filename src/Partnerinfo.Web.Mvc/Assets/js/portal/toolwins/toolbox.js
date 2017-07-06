// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

/// <reference path="base.js" />

(function (_WinJS, _PI, $) {
    "use strict";

    var _Class = _WinJS.Class;

    var ns = _WinJS.Namespace.define("PI.Portal", {

        ToolBoxCommand: _Class.define(function ToolBoxCommand_ctor(options) {
            /// <signature>
            /// <summary>Initializes a new instance of the ToolBoxCommand class.</summary>
            /// <param name="options" type="Object" />
            /// <returns type="ToolBoxCommand" />
            /// </signature>
            this._disposed = false;
            this.css = options.css || "ui-btn ui-btn-flat";
            this.icon = options.icon;
            this.text = options.text;
            this.tooltip = options.tooltip;
            this.checked = options.checked ? ko.pureComputed(options.checked, this).extend({ throttle: 0 }) : undefined;
            this.click = options.click;
        }, {
            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                if (this._disposed) {
                    return;
                }
                ko.isSubscribable(this.checked) && this.checked.dispose();
                this.checked = null;
                this._disposed = true;
            }
        }),

        ToolBoxGroup: _Class.define(function ToolBoxGroup_ctor(toolBox) {
            /// <signature>
            /// <summary>Initializes a new instance of the ToolBoxGroup class.</summary>
            /// <param name="toolBox" type="PI.Portal.ToolBox" />
            /// <returns type="ToolBoxGroup" />
            /// </signature>
            this._disposed = false;
            this.toolBox = toolBox;
            this.commands = ko.observableArray();
            this.commandsView = this.commands.map(function (command) { return new ns.ToolBoxCommand(command); });
        }, {
            /// <field type="$" canBeNull="true">
            /// The current element to edit.
            /// </field>
            element: {
                get: function () {
                    return this.toolBox.element();
                }
            },
            notifyChanged: function () {
                /// <signature>
                /// <summary>Raises an update event.</summary>
                /// </signature>
                this.toolBox.notifyChanged();
            },
            removeClasses: function (classNamePrefix) {
                /// <signature>
                /// <summary>Removes all CSS style classes.</summary>
                /// </signature>
                /// <signature>
                /// <summary>Removes all CSS style classes with the specified prefix.</summary>
                /// <param name="classNamePrefix" type="String" />
                /// </signature>
                $.each(this.element, function (index, element) {
                    var classNames = element.classList || element.className.match(/\S+/g);
                    if (!classNames) {
                        return;
                    }
                    var elementClassNames = [];
                    for (var i = 0, len = classNames.length; i < len; ++i) {
                        var className = classNames[i];
                        if (className === "ui-resizable" ||
                            className === "ui-draggable" ||
                            className.indexOf(_PI.Portal.ModuleClasses.module) >= 0 ||
                            classNamePrefix && className.indexOf(classNamePrefix) < 0) {
                            elementClassNames.push(className);
                        }
                    }
                    element.className = elementClassNames.join(" ");
                });
            },

            createModule: function (moduleClass, themeClasses, moduleOptions) {
                /// <signature>
                /// <summary>Creates a module using the specified CSS class.</summary>
                /// <param name="moduleClass" type="String" />
                /// <param name="themeClasses" type="String" optional="true" />
                /// <param name="moduleOptions" type="Object" optional="true" />
                /// <returns type="jQuery" />
                /// </signature>
                var $module = this.toolBox.engine.context.createElement(moduleClass);
                themeClasses && $module.addClass(themeClasses);
                moduleOptions && $module.attr("data-module-options", JSON.stringify(moduleOptions));
                return $module;
            },
            appendAsync: function (elements) {
                /// <signature>
                /// <summary>Appends the specified element(s) to the element parsing its content.</summary>
                /// <param name="elements" type="Array" parameterArray="true" />
                /// <returns type="WinJS.Promise" />
                /// </signature>
                this.element.append.apply(this.element, arguments);
                return this.designer.options.module.parseAllAsync(null, this.element);
            },
            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                if (this._disposed) {
                    return;
                }
                this.commandsView().forEach(function (command) { command.dispose(); });
                this._disposed = true;
            }
        }),

        ToolBox: _Class.define(function ToolBox_ctor(designer, toolboxGroups) {
            /// <signature>
            /// <summary>Initializes a new instance of the ToolBox class.</summary>
            /// <param name="designer" type="$.PI.PortalDesigner" />
            /// <param name="toolboxGroups" type="Array" />
            /// <returns type="ToolBox" />
            /// </signature>
            var that = this;
            this._disposed = false;
            this.designer = designer;
            this.element = ko.observable();
            this.defaultGroup = new ns.ToolBoxGroup(that);
            this.groups = toolboxGroups.map(function (ToolBoxGroupCtor) { return new ToolBoxGroupCtor(that); });
        }, {
            engine: {
                get: function () {
                    return this.designer.options.engine;
                }
            },
            notifyChanged: function () {
                /// <signature>
                /// <summary>Raises an update.</summary>
                /// </signature>
                this.element.valueHasMutated();
            },
            dispose: function () {
                /// <signature>
                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                /// </signature>
                if (this._disposed) {
                    return;
                }
                this.groups.forEach(function (group) { group.dispose(); });
                this.groups = [];
                this._disposed = true;
            }
        })

    });

    _WinJS.Namespace.define("PI.Portal", {

        ColorToolBoxGroup: _Class.derive(ns.ToolBoxGroup, function ColorToolBoxGroup_ctor(designer) {
            /// <signature>
            /// <summary>Initializes a new instance of the ColorToolBoxGroup class.</summary>
            /// <param name="designer" type="$.PI.PortalDesigner" />
            /// <returns type="ColorToolBoxGroup" />
            /// </signature>
            ns.ToolBoxGroup.call(this, designer);

            this.commands.push({
                text: "c-x",
                tooltip: "Nincs szín",
                click: this.setColor.bind(this, null)
            }, {
                text: "c-1",
                tooltip: "Első szín",
                click: this.setColor.bind(this, 1)
            }, {
                text: "c-2",
                tooltip: "Második szín",
                click: this.setColor.bind(this, 2)
            }, {
                text: "c-3",
                tooltip: "Harmadik szín",
                click: this.setColor.bind(this, 3)
            }, {
                text: "c-4",
                tooltip: "Negyedik szín",
                click: this.setColor.bind(this, 4)
            });
        }, {
            setColor: function (schema) {
                /// <signature>
                /// <param name="schema" type="Number" />
                /// </signature>
                this.removeClasses("ui-color");
                schema && this.element.addClass("ui-color-" + schema);
                this.notifyChanged();
            }
        }),

        RowToolBoxGroup: _Class.derive(ns.ToolBoxGroup, function RowToolBoxGroup_ctor(designer) {
            /// <signature>
            /// <summary>Initializes a new instance of the RowToolBoxGroup class.</summary>
            /// <param name="designer" type="$.PI.PortalDesigner" />
            /// <returns type="RowToolBoxGroup" />
            /// </signature>
            ns.ToolBoxGroup.call(this, designer);

            this.commands.push({
                text: "| 12",
                tooltip: "12 oszlop",
                click: this.addAsync.bind(this, [1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1])
            }, {
                text: "| 6",
                click: this.addAsync.bind(this, [2, 2, 2, 2, 2, 2])
            }, {
                text: "| 4",
                click: this.addAsync.bind(this, [3, 3, 3, 3])
            }, {
                text: "| 3",
                click: this.addAsync.bind(this, [4, 4, 4])
            }, {
                text: "| 2",
                click: this.addAsync.bind(this, [6, 6])
            });
        }, {
            addAsync: function (cols) {
                /// <signature>
                /// <summary>Inserts a new row with the specified columns.</summary>
                /// </signature>
                var row = this.createModule("ui-module-panel", "ui-row");
                for (var i = 0, len = cols.length; i < len; ++i) {
                    row.append(this.createModule("ui-module-panel", "ui-col-md-" + cols[i]));
                }
                return this.appendAsync(row);
            }
        }),

        ContainerToolBoxGroup: _Class.derive(ns.ToolBoxGroup, function ContainerToolBoxGroup_ctor(designer) {
            /// <signature>
            /// <summary>Initializes a new instance of the ContainerToolBoxGroup class.</summary>
            /// <param name="designer" type="$.PI.PortalDesigner" />
            /// <returns type="ContainerToolBoxGroup" />
            /// </signature>
            ns.ToolBoxGroup.call(this, designer);

            this.commands.push({
                text: "[*]",
                tooltip: "Hozzáad egy tároló panelt, ami független a kijelző méretétől",
                click: this.addAsync.bind(this, true)
            }, {
                text: "[+]",
                tooltip: "Hozzáad egy tároló panelt, ami függ a kijelző méretétől",
                click: this.addAsync.bind(this, false)
            });
        }, {
            addAsync: function (fluid) {
                /// <signature>
                /// <summary>Inserts a new row with the specified columns.</summary>
                /// </signature>
                return this.appendAsync(this.createModule("ui-module-panel", fluid ? "ui-container-fluid" : "ui-container"));
            }
        }),

        BoxToolBoxGroup: _Class.derive(ns.ToolBoxGroup, function BoxToolBoxGroup_ctor(designer) {
            /// <signature>
            /// <summary>Initializes a new instance of the ContainerToolBoxGroup class.</summary>
            /// <param name="designer" type="$.PI.PortalDesigner" />
            /// <returns type="ContainerToolBoxGroup" />
            /// </signature>
            ns.ToolBoxGroup.call(this, designer);

            this.commands.push({
                text: "[]",
                tooltip: "Hozzáad egy dobozt ami beigazítja a tartalmat",
                click: this.addAsync.bind(this, false)
            }, {
                text: "[]",
                tooltip: "Hozzáad egy dobozt kerettel",
                click: this.addAsync.bind(this, true)
            });
        }, {
            addAsync: function (win) {
                /// <signature>
                /// <summary>Inserts a new row with the specified columns.</summary>
                /// <param name="win" type="Boolean" />
                /// </signature>
                var box = this.createModule("ui-module-panel", win ? "ui-box ui-box-bordered" : "ui-box");
                box.append(this.createModule("ui-module-panel", "ui-box-header").append(this.createModule("ui-module-html").html("<h2>Title<\/h2>")));
                box.append(this.createModule("ui-module-panel", "ui-box-group").append(this.createModule("ui-module-html").html("Content")));
                box.append(this.createModule("ui-module-panel", "ui-box-footer").append(this.createModule("ui-module-html").html("Footer")));
                return this.appendAsync(box);
            }
        }),

        EmbedToolBoxGroup: _Class.derive(ns.ToolBoxGroup, function EmbedToolBoxGroup_ctor(designer) {
            /// <signature>
            /// <summary>Initializes a new instance of the EmbedToolBoxGroup class.</summary>
            /// <param name="designer" type="$.PI.PortalDesigner" />
            /// <returns type="EmbedToolBoxGroup" />
            /// </signature>
            ns.ToolBoxGroup.call(this, designer);

            this.commands.push({
                icon: "i portal size-4by3",
                tooltip: "4x3-as képarány",
                checked: this.hasEmbedClass.bind(this, "4by3"),
                click: this.addEmbedClass.bind(this, "4by3")
            }, {
                icon: "i portal size-16by9",
                tooltip: "16x9-as képarány",
                checked: this.hasEmbedClass.bind(this, "16by9"),
                click: this.addEmbedClass.bind(this, "16by9")
            });
        }, {
            hasEmbedClass: function (size) {
                /// <signature>
                /// <summary>Returns true if the specified embed class is defined.</summary>
                /// <param name="size" type="String" />
                /// <returns type="Boolean" />
                /// </signature>
                if (!this.element) {
                    return false;
                }
                return this.element.hasClass("ui-embed-" + size);
            },
            addEmbedClass: function (size) {
                /// <signature>
                /// <param name="size" type="String" />
                /// </signature>
                if (!this.element) {
                    return;
                }
                this.removeClasses("ui-embed");
                size && this.element.addClass("ui-embed ui-embed-" + size);
                this.notifyChanged();
            }
        }),

        ButtonToolBoxGroup: _Class.derive(ns.ToolBoxGroup, function ButtonToolBoxGroupGroup_ctor(designer) {
            /// <signature>
            /// <summary>Initializes a new instance of the ButtonToolBoxGroup class.</summary>
            /// <param name="designer" type="$.PI.PortalDesigner" />
            /// <returns type="ButtonToolBoxGroup" />
            /// </signature>
            ns.ToolBoxGroup.call(this, designer);

            this.commands.push({
                text: "txt",
                tooltip: "Gomb stílus eltávolítása",
                checked: this.hasBtnClass.bind(this, undefined),
                click: this.setBtnClass.bind(this, undefined)
            }, {
                css: "ui-btn",
                text: "def",
                tooltip: "Gomb stílus alkalmazása",
                checked: this.hasBtnClass.bind(this, null),
                click: this.setBtnClass.bind(this, null)
            }, {
                css: "ui-btn ui-btn-primary",
                text: "pry",
                tooltip: "Elsődleges gomb stílus alkalmazása",
                checked: this.hasBtnClass.bind(this, "primary"),
                click: this.setBtnClass.bind(this, "primary")
            }, {
                css: "ui-btn ui-btn-success",
                text: "suc",
                tooltip: "Siker gomb stílus alkalmazása",
                checked: this.hasBtnClass.bind(this, "success"),
                click: this.setBtnClass.bind(this, "success")
            }, {
                css: "ui-btn ui-btn-info",
                text: "inf",
                tooltip: "Info gomb stílus alkalmazása",
                checked: this.hasBtnClass.bind(this, "info"),
                click: this.setBtnClass.bind(this, "info")
            }, {
                css: "ui-btn ui-btn-warning",
                text: "war",
                tooltip: "Figyelmeztető gomb stílus alkalmazása",
                checked: this.hasBtnClass.bind(this, "warning"),
                click: this.setBtnClass.bind(this, "warning")
            }, {
                css: "ui-btn ui-btn-danger",
                text: "dan",
                tooltip: "Veszély gomb stílus alkalmazása",
                checked: this.hasBtnClass.bind(this, "danger"),
                click: this.setBtnClass.bind(this, "danger")
            }, {
                css: "ui-btn ui-btn-flat",
                text: "fla",
                tooltip: "Lapos gomb stílus alkalmazása",
                checked: this.hasBtnClass.bind(this, "flat"),
                click: this.setBtnClass.bind(this, "flat")
            });
        }, {
            hasBtnClass: function (btnClass) {
                /// <signature>
                /// <summary>Returns true if the element has a className with the specified name.</summary>
                /// <returns type="Boolean" />
                /// </signature>
                if (!this.element) {
                    return false;
                }
                return this.element.hasClass("ui-btn-" + btnClass);
            },
            setBtnClass: function (btnClass) {
                /// <signature>
                /// <summary>Adds the specified button class to the element.</summary>
                /// <param name="btnClass" type="String" />
                /// </signature>
                if (!this.element) {
                    return;
                }
                this.removeClasses("ui-btn");
                if (btnClass !== undefined) {
                    this.element.addClass("ui-btn");
                    if (btnClass) {
                        this.element.addClass("ui-btn-" + btnClass);
                    }
                }
                this.notifyChanged();
            }
        }),

        VisibilityToolBoxGroup: _Class.derive(ns.ToolBoxGroup, function VisibilityToolBoxGroup_ctor(designer) {
            /// <signature>
            /// <summary>Initializes a new instance of the VisibilityToolBoxGroup class.</summary>
            /// <param name="designer" type="$.PI.PortalDesigner" />
            /// <returns type="ColorToolBoxGroup" />
            /// </signature>
            ns.ToolBoxGroup.call(this, designer);

            this.commands.push({
                text: "*",
                tooltip: "Minden eszközön látszik",
                click: this.setVisibleClass.bind(this, null, null)
            }, {
                icon: "i portal device-mobile",
                tooltip: "Mobilon látszik",
                checked: this.hasVisibleClass.bind(this, true, "xs"),
                click: this.setVisibleClass.bind(this, true, "xs")
            }, {
                icon: "i portal device-tablet",
                tooltip: "Tábla gépen látszik",
                checked: this.hasVisibleClass.bind(this, true, "sm"),
                click: this.setVisibleClass.bind(this, true, "sm")
            }, {
                icon: "i portal device-desktop-md",
                tooltip: "Asztali közepes kijelzőn látszik",
                checked: this.hasVisibleClass.bind(this, true, "md"),
                click: this.setVisibleClass.bind(this, true, "md")
            }, {
                icon: "i portal device-desktop-lg",
                tooltip: "Asztali nagyméretű kijelzőn látszik",
                checked: this.hasVisibleClass.bind(this, true, "lg"),
                click: this.setVisibleClass.bind(this, true, "lg")
            }, {
                css: "ui-btn ui-btn-flat pi-device-hidden",
                icon: "i portal device-mobile",
                tooltip: "Mobilon nem látszik",
                checked: this.hasVisibleClass.bind(this, false, "xs"),
                click: this.setVisibleClass.bind(this, false, "xs")
            }, {
                css: "ui-btn ui-btn-flat pi-device-hidden",
                icon: "i portal device-tablet",
                tooltip: "Tábla gépen nem látszik",
                checked: this.hasVisibleClass.bind(this, false, "sm"),
                click: this.setVisibleClass.bind(this, false, "sm")
            }, {
                css: "ui-btn ui-btn-flat pi-device-hidden",
                icon: "i portal device-desktop-md",
                tooltip: "Asztali közepes kijelzőn nem látszik",
                checked: this.hasVisibleClass.bind(this, false, "md"),
                click: this.setVisibleClass.bind(this, false, "md")
            }, {
                css: "ui-btn ui-btn-flat pi-device-hidden",
                icon: "i portal device-desktop-lg",
                tooltip: "Asztali nagyméretű kijelzőn nem látszik",
                checked: this.hasVisibleClass.bind(this, false, "lg"),
                click: this.setVisibleClass.bind(this, false, "lg")
            });
        }, {
            hasVisibleClass: function (visible, sizeClass) {
                /// <signature>
                /// <returns type="Boolean" />
                /// </signature>
                if (!this.element) {
                    return false;
                }
                return this.element.hasClass("ui-" + (visible ? "visible" : "hidden") + "-" + sizeClass);
            },
            setVisibleClass: function (visible, sizeClass) {
                /// <signature>
                /// <param name="btnClass" type="String" />
                /// </signature>
                if (!this.element) {
                    return;
                }
                if (!visible) {
                    this.removeClasses("ui-visible");
                    this.removeClasses("ui-hidden");
                }
                if (sizeClass) {
                    this.element.addClass("ui-" + (visible ? "visible" : "hidden") + "-" + sizeClass);
                }
                this.notifyChanged();
            }
        })

    });

    $.widget("PI.PortalToolboxToolWin", $.PI.PortalToolWin, {
        options: {
            css: "ui-portal-toolwin-toolbox",
            toolBox: ns.ToolBox,
            toolBoxGroups: [
                ns.ContainerToolBoxGroup,
                ns.BoxToolBoxGroup,
                ns.RowToolBoxGroup,
                ns.ColorToolBoxGroup,
                ns.VisibilityToolBoxGroup,
                ns.ButtonToolBoxGroup,
                ns.EmbedToolBoxGroup
            ]
        },
        _onRender: function () {
            /// <signature>
            /// <summary>Renders content.</summary>
            /// </signature>
            this.toolbox = new this.options.toolBox(this.options.designer, this.options.toolBoxGroups);
            this.toolWinMenu.html(
'<div class="ui-btn-group ui-btn-group-sm">' +
    '<button class="ui-btn" data-bind="click: function (t) { t.defaultGroup.removeClasses(); }, clickBubble: false">Nincs stílus</button>' +
'</div>');
            this.toolWinBody.html(
'<div role="toolbar" class="ui-toolbar" data-bind="foreach: groups">' +
    '<div role="group" class="ui-btn-group-vl ui-btn-group-sm" data-bind="foreach: commandsView">' +
        '<button data-bind="attr: { \'title\': tooltip, \'aria-checked\': checked }, css: css, click: click, clickBubble: false">' +
            '<!-- ko if: icon --><i data-bind="attr: { class: icon }"></i><!-- /ko -->' +
            '<!-- ko if: text --><span data-bind="text: text"></span><!-- /ko -->' +
        '</button>' +
    '</div>' +
'</div>');
            ko.applyBindings(this.toolbox, this.toolWinMenu[0]);
            ko.applyBindings(this.toolbox, this.toolWinBody[0]);
        },
        _onSelectionChanged: function (event) {
            /// <signature>
            /// <summary>Raised after modules are selected or deselected.</summary>
            /// </signature>
            this._super(event);
            if (this.toolbox) {
                this.toolbox.element(event.detail.element);
            }
        },
        _destroy: function () {
            /// <signature>
            /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            /// </signature>
            ko.cleanNode(this.toolWinMenu[0]);
            ko.cleanNode(this.toolWinBody[0]);
            this.toolbox && this.toolbox.dispose();
            this.toolbox = null;
            this._super();
        }
    });

})(WinJS, PI, jQuery);