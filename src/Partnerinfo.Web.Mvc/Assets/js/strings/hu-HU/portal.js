// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

WinJS.Resources.define("hu", {
    //"portal/toolboxSectionAddX": "",

    "portal": {
        "actions": {
            "ok": "OK",
            "cancel": "Mégse"
        },
        "errors": {
            "nonEditableModule": "Ez a modul nem szerkeszthető vagy már szerkesztési állapotban van.",
            "containerRequired": "Csak panel modulokon belül helyezhetőek el további modulok."
        },
        "permOptions": [
            { "id": 0, "name": "Senki nem látja" },
            { "id": 1, "name": "Mindenki látja (alapértelmezett)" },
            { "id": 2, "name": "Csak felhasználók látják" },
            { "id": 3, "name": "Csak bejelentkezett felhasználók látják" },
            { "id": 4, "name": "Csak nem bejelentkezett felhasználók látják" }
        ],
        "events": {
            "none": null,
            "login": "Felhasználó bejelentkezett",
            "logout": "Felhasználó kijelentkezett",
            "ready": "Betöltődött",
            "change": "Módosult",
            "click": "Kattintott",
            "dblclick": "Duplán kattintott",
            "mousedown": "Egérgombot lenyomta",
            "mouseup": "Egérgombot felengedte",
            "mouseenter": "Egér belépett",
            "mouseleave": "Egér kilépett",
            "mouseover": "Egér felette áll",
            "mouseout": "Egér kikerült felőle",
            "focus": "Kurzor rákerült",
            "blur": "Kurzor elhagyta",
            "contextmenu": "Popup menü megjelent"
        },
        "eventOptions": [
            { "text": null, "value": null },
            { "text": "Betöltődött", "value": "ready" },
            { "text": "Módosult", "value": "change" },
            { "text": "Kattintott", "value": "click" },
            { "text": "Duplán kattintott", "value": "dblclick" },
            { "text": "Egérgombot lenyomta", "value": "mousedown" },
            { "text": "Egérgombot felengedte", "value": "mouseup" },
            { "text": "Egér belépett", "value": "mouseenter" },
            { "text": "Egér kilépett", "value": "mouseleave" },
            { "text": "Egér felette áll", "value": "mouseover" },
            { "text": "Egér kikerült felőle", "value": "mouseout" },
            { "text": "Kurzor rákerült", "value": "focus" },
            { "text": "Kurzor elhagyta", "value": "blur" },
            { "text": "Popup menü megjelent", "value": "contextmenu" }
        ],
        "toolWins": {
            "base": {
                "switchDM": "Megjelenítési mód váltása",
                "collapse": "Bezár/kinyit"
            },
            "toolbox": {
                "name": "Eszközök"
            },
            "event": {
                "name": "Események"
            },
            "info": {
                "name": "Info",
                "inline": "Inline stílusok",
                "className": "Téma osztály"
            },
            "module": {
                "name": "Modul"
            },
            "preview": {
                "name": "Előnézet"
            },
            "style": {
                "name": "Stílusok"
            },
            "reference": {
                "name": "Referenciák"
            },
            "tree": {
                "name": "Hiearachia"
            }
        },
        "modules": {
            "animatedpanel": {
                "name": "Animált panel"
            },
            "content": {
                "name": "Mesteroldal tartalom"
            },
            "chat": {
                "name": "Chat"
            },
            "event": {
                "name": "Eseményszerkesztő"
            },
            "events": {
                "name": "Események", "fakeName": "Esemény {0}"
            },
            "form": {
                "name": "Űrlap"
            },
            "frame": {
                "name": "Beágyazott keret"
            },
            "image": {
                "name": "Kép"
            },
            "link": {
                "name": "Link"
            },
            "panel": {
                "name": "Panel"
            },
            "scroller": {
                "name": "Görgető"
            },
            "search": {
                "name": "Kereső"
            },
            "store": {
                "name": "Áruház"
            },
            "html": {
                "name": "Szövegdoboz"
            },
            "timer": {
                "name": "Időzítő "
            },
            "video": {
                "name": "Video"
            }
        },
        "properties": {
            "background-color": "Háttérszín",
            "background-image": "Háttérkép",
            "background-repeat": "Háttér ismétlés",
            "background-attachment": "Háttér rögzítés",
            "background-position": "Háttér pozíció",
            "background-size": "Háttér méret",
            "border-style": "Keret stílus",
            "border-width": "Keret szélesség",
            "border-color": "Keretszín",
            "border-radius": "Keret sugár",
            "bottom": "Pozíció lentről",
            "box-shadow": "Keret árnyék",
            "clear": "Oldaltörés",
            "color": "Betűszín",
            "cursor": "Egér kurzor",
            "display": "Megjelenés",
            "height": "Magasság",
            "float": "Lebegés",
            "font-family": "Betűtípus",
            "font-size": "Betűméret",
            "font-style": "Betű stílus",
            "font-variant": "Betű változat",
            "font-weight": "Betű vastagság",
            "left": "Pozíció balról",
            "letter-spacing": "Betű távolság",
            "line-height": "Szöveg sormagassága",
            "margin-left": "Margó balról",
            "margin-top": "Margó fentről",
            "margin-right": "Margó jobbról",
            "margin-bottom": "Margó lentről",
            "min-width": "Min. magasság",
            "max-height": "Max. magasság",
            "opacity": "Áttetszőség",
            "overflow": "Kicsordulás",
            "overflow-x": "Kicsordulás (bal, jobb)",
            "overflow-y": "Kicsordulás (fent, lent)",
            "padding-left": "Behúzás balról",
            "padding-top": "Behúzás fentről",
            "padding-right": "Behúzás jobbról",
            "padding-bottom": "Behúzás lentről",
            "position": "Pozíció",
            "right": "Pozíció jobbról",
            "text-align": "Szöveg igazítás",
            "text-decoration": "Szöveg díszítés",
            "text-indent": "Szöveg bekezdés",
            "text-transform": "Szöveg átalakítás",
            "text-shadow": "Szöveg árnyék",
            "top": "Pozíció fentről",
            "vertical-align": "Függőleges igazítás",
            "visibility": "Láthatóság",
            "width": "Szélesség",
            "white-space": "Szóköz",
            "word-spacing": "Szó távolság",
            "z-index": "Réteg sorrend"
        }
    }
});
