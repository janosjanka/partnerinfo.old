// Copyright (c) Partnerinfo Ltd. All Rights Reserved.

(function (_WinJS, _PI) {
    "use strict";

    var Sounds = {};

    _WinJS.Namespace.defineWithParent(_PI, null, {
        Sound: _WinJS.Class.define(null, null, {
            register: function (name, url) {
                /// <signature>
                /// <summary>Registers an audio resource with the given name.</summary>
                /// <param name="name" type="String" />
                /// <param name="url" type="String" />
                /// </signature>
                Sounds[name] = { url: url, audio: null };
            },
            audio: function (name) {
                /// <signature>
                /// <summary>Gets the audio object for the given name.</summary>
                /// <param name="name" type="String" />
                /// <returns type="Audio" />
                /// </signature>
                var sound;
                if (!window.Audio || !(sound = Sounds[name])) {
                    return;
                }
                return (sound.audio = sound.audio || new window.Audio(sound.url));
            },
            play: function (name) {
                /// <signature>
                /// <summary>Starts playing the audio with the given name.</summary>
                /// <param name="name" type="String" />
                /// </signature>
                var audio = this.audio(name);
                audio && audio.play();
            }
        })
    });

    _PI.Sound.register("notify", _PI.Server.mapPath("/sn/notify.mp3"));

})(WinJS, PI);