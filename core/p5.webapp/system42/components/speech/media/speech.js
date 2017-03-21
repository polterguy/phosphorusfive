/*
 * Phosphorus Five, copyright 2014 - 2017, Thomas Hansen, thomas@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

(function() {


    /*
     * Main wrapper namespace object for speech parts of Phosphorus Five.
     */
    p5.speech = {};


    /*
     * Function that speaks a given "txt" in a given "voice", resorting to default voice if no voice is given.
     *
     * Notice, "voice" can be for instance "en-US", or it can be "Samantha,en-US" or it can be "Fred".
     * If a name is given, this function will search for the specified name, and use that if it can, otherwise resort to the
     * first voice matching the "locale" parts after the ",".
     */
    p5.speech.speak = function (txt, voice) {

        // Checking if voices has already been initialized.
        if (p5.speech._sys42_speech_voices) {

            // Voices are already initialized, invoking "_speak" function immediately.
            p5.speech._speak (txt, voice);

        } else {

            // Voices have NOT been initialized yet, provding our callback function, for retrieving all available voices on client.
            window.speechSynthesis.onvoiceschanged = function() {

                // To avoid callback from being evaluated twice, we return "early" if it has already been invoked previously.
                if (p5.speech._sys42_speech_voices) {
                  return;
                }

                // Retrieving voices, and storing them, such that we don't need another roundtrip through here later.
                p5.speech._sys42_speech_voices = window.speechSynthesis.getVoices ();
                p5.speech._speak (txt, voice);
            }
        }
    }


    /*
     * Function that queries the speechSynthesis object, and sends the supported voices back to the server.
     *
     * "widget" must be the ID of an Ajax widget that contains an [.ondone] Ajax event.
     * This [.ondone] Ajax event, will be invoked when all voices have been retrieved, and one "p5-speech-voice.XXX" HTTP
     * POST parameter will be passed in, for each voice the client supports.
     * The locale/language of the voice, will be the value of the HTTP POST parameter(s).
     */
    p5.speech.query = function (widget) {

        // Checking if voices has already been initialized.
        if (p5.speech._sys42_speech_voices) {

            // Voices are already initialized, invoking "_query" function immediately.
            p5.speech._query (widget);

        } else {

            // Voices have NOT been initialized yet, provding our callback function, for retrieving all available voices on client.
            window.speechSynthesis.onvoiceschanged = function() {

                // To avoid callback from being evaluated twice, we return "early" if it has already been invoked previously.
                if (p5.speech._sys42_speech_voices) {
                  return;
                }

                // Retrieving voices, and storing them, such that we don't need another roundtrip through here later.
                p5.speech._sys42_speech_voices = window.speechSynthesis.getVoices ();
                p5.speech._query (widget);
            }
        }
    }


    /*
     * Private implementation for the above.
     * Expects "p5.speech._sys42_speech_voices" to have already beein initialized.
     */
    p5.speech._speak = function (txt, voice) {

        // Creating our utterance object, wrapping specified text that should be spoken.
        var utter = new SpeechSynthesisUtterance (txt);

        // Retrieving the specified voice, such that we support all possible permutations.
        var voices = p5.speech._sys42_speech_voices.filter (function (ix) {

            // Checking if voice contains both name and locale.
            if (voice.indexOf (',') != -1) {
                return ix.name == voice.split(',')[0];
            }

            // "voice" is either a name or a locale.
            return ix.name == voice || ix.lang == voice;
        });

        /*
         * If voice was in "name,locale" format, and the "name" voice was not found, we must look for the first voice matching
         * the "locale" (meaning the parts after the ",").
         */
        if (voices.length == 0 && voice.indexOf(',') != -1) {
          voices = p5.speech._sys42_speech_voices.filter (function (ix) {return ix.lang == voice.split (',')[1]});
        }

        // Checking if we have a voice matching the one specified from caller.
        if (voices.length > 0) {

            // Speaking utterance object, with the first voice from our list of matches.
            utter.voice = voices [0];
            window.speechSynthesis.speak(utter);

        } else {

            // Unsupported voice was supplied.
            window.speechSynthesis.speak(new SpeechSynthesisUtterance('That voice is not supported!'));
        }
    }


    /*
     * Private implementation for the above.
     * Expects "p5.speech._sys42_speech_voices" to have already beein initialized.
     */
    p5.speech._query = function (widget) {
        p5.$(widget).raise('.ondone', {
            onbefore: function (pars, evt) {
                for (var ix = 0; ix < p5.speech._sys42_speech_voices.length;ix++) {
                    var cur = p5.speech._sys42_speech_voices[ix];
                    pars.push (['p5-speech-voice.' + cur.name, cur.lang + ',' + cur.localService]);
                }
            }
        });
    }
})();
