// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (_WinJS) {
    "use strict";

    _WinJS.Resources.define("en", {
        "pi/project/actionNameRequired": "Action name is required.",
        "pi/project/actionRootName": "Name of the root action",
        "pi/project/actionChildName": "Name of the child action",
        "pi/project/actionEnabled": "Turned On",

        "pi/project/actionTypeUnknown": "",
        "pi/project/actionTypeRedirect": "Open website",
        "pi/project/actionTypeRedirectDescription": "Átirányítja a felhasználót a megadott oldalra. Ez megszakítja a további akciók futását!",
        "pi/project/actionTypeRedirectUrlRequired": "A webcímet kötelező megadni.",
        "pi/project/actionTypeSequence": "Akciók rendezett csoportja",
        "pi/project/actionTypeSequenceDescription": "Lehetőséget ad több akció hozzáadására folytonos sorrendben, egyetlen közös szülő alá csoportosítva azokat.",
        "pi/project/actionTypeSchedule": "Akciók ütemezése",
        "pi/project/actionTypeScheduleDescription": "Ütemezi a gyermek akciók végrehajtását a megadott időpontra, amely abszolút és relatív módon is megadható.",
        "pi/project/actionTypeCondition": "Akciók feltételes futtatása",
        "pi/project/actionTypeConditionDescription": "A gyermek akciók csak abban az esetben kerülnek végrehajtásra, ha a megadott feltételek teljesülnek.",
        "pi/project/actionTypeConditionDateGreaterThanOrEqualTo": "Az aktuális idő >=",
        "pi/project/actionTypeConditionDateLessThanOrEqualTo": "Az aktuális idő <=",
        "pi/project/actionTypeConditionAuthenticated": "Bejelentkezett felhasználó?",
        "pi/project/actionTypeConditionContactExists": "Regisztrált felhasználó?",
        "pi/project/actionTypeConditionContactWithTag": "Felhasználó címkével",
        "pi/project/actionTypeConditionContactWithoutTag": "Felhasználó címke nélkül",
        "pi/project/actionTypeAuthenticate": "Felhasználó bejelentkeztetése",
        "pi/project/actionTypeAuthenticateDescription": "Bejelentkezve küldi tovább a felhasználót a projekthez tartozó portálok valamelyikére.",
        "pi/project/actionTypeRegister": "Felhasználó regisztrálása",
        "pi/project/actionTypeRegisterDescription": "Regisztrálja, azaz hozzáadja a felhasználót a megadott adataival a projekthez.",
        "pi/project/actionTypeUnregister": "Felhasználó leiratkoztatása",
        "pi/project/actionTypeUnregisterDescription": "Véglegesen törli a felhasználót minden adatával a projektből.",
        "pi/project/actionTypeSetTags": "Felhasználó címkézése",
        "pi/project/actionTypeSetTagsDescription": "Hozzáad-, illetve eltávolít címkéket a felhasználótól.",
        "pi/project/actionTypeSendMail": "Levél küldése a felhasználónak",
        "pi/project/actionTypeSendMailDescription": "Elküld egy emailt kicserélve az abban található helyőrzőket a felhasználó adataival.",
        "pi/project/actionTypeLog": "Esemény naplózása",
        "pi/project/actionTypeLogDescription": "Naplózza az akció és felhasználói adatokat. Akár ki is kapcsolható az alapértelmezett naplózás.",

        "pi/project/actionLink": "Action Link",

        "pi/project/mailMessageSubjectRequired": "The subject is required.",
        "pi/project/mailMessageDoesNotExist": "This mail message already exists.",
        "pi/project/mailMessageHeaderSummaryById": "<strong>{0}<\/strong> selected"
    });

})(WinJS);