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

using System;
using System.Linq;
using System.Collections.Generic;
using p5.core;

namespace p5.web.widgets.helpers
{
    /*
     * Used as storage for Ajax and lambda events for Widgets
     */
    [Serializable]
    public class WidgetEventStorage
    {
        readonly Dictionary<string, List<Node>> _events = new Dictionary<string, List<Node>> ();

        /*
         * Returns all items that matches "key1".
         */
        internal IEnumerable<Node> this [string key1] {
            get {
                if (_events.ContainsKey (key1))
                    return _events [key1];
                return null;
            }
        }

        /*
         * Returns all Keys from dictionary
         */
        internal IEnumerable<string> Keys {
            get {
                return _events.Keys;
            }
        }

        /*
         * Returns item that matches both "key1" and "key2"
         */
        internal Node this [string key1, string key2] {
            get {
                if (!_events.ContainsKey (key1))
                    return null;
                return _events [key1].Find (delegate (Node idx) { return idx.Name == key2; });
            }
            set {
                // Checking if we've got any events for given widget from before, and if not, create our specific Widget storage
                if (value != null && !_events.ContainsKey (key1)) {

                    // First Ajax event for Widget
                    _events [key1] = new List<Node> ();
                } else if (_events.ContainsKey (key1)) {

                    // Making sure we remove any previously added Ajax events with same name for Widget
                    _events [key1].RemoveAll (delegate (Node idx) { return idx.Name == key2; });
                }

                if (value != null) {

                    // Adding a new lambda object for widget's event
                    value.Name = key2; // just to be sure!
                    _events [key1].Add (value);
                } else if (_events.ContainsKey (key1) && _events [key1].Count == 0) {

                    // Removing Widget storage entirely
                    _events.Remove (key1);
                }
            }
        }

        /*
         * Changes the key1 parts of dictionary
         */
        internal void ChangeKey1 (ApplicationContext context, string oldKey, string newKey)
        {
            // First copying the old dictionary item into new dictionary item, before we delete the old dictionary item
            _events [newKey] = _events [oldKey];
            _events.Remove (oldKey);

            // Then looping through all [_event] references inside of what are probably our Ajax Events, and
            // updating the [_event] node's value to the new ID of element.
            // This might not ALWAYS succeed, though it is good enough for all practical concerns!
            // If it does not succeed, due to for instance the ID of your widget being referenced by some other
            // node in your events, you'll either have to manually change your Ajax Events, or refresh the page ...
            // Sorry ...!!
            // If it does not work, you've probably written what's defined as "bad Hyperlambda" anyway ...!

            // Looping through all Ajax Events on widget
            foreach (var idxChildNode in _events [newKey]) {

                // Looping through all children nodes of Ajax Events that matches the "oldKey" and the name of [_event]
                // updating the values of these nodes to the new ID
                foreach (var idxChilcChildNode in idxChildNode.Children.Where (ix => ix.Name == "_event" && ix.Get<string> (context) == oldKey)) {
                    idxChilcChildNode.Value = newKey;
                }
            }
        }

        /*
         * Changes the key2 parts of dictionary
         */
        internal void ChangeKey2 (string oldKey, string newKey)
        {
            foreach (var idxKey1 in _events.Keys) {
                foreach (var idxKey2 in _events [idxKey1]) {
                    if (idxKey2.Name == oldKey) {
                        idxKey2.Name = newKey;
                    }
                }
            }
        }

        /*
         * Removes Dictionary items from dictionary matching both key1 and key2
         */
        internal void Remove (string key1, string key2)
        {
            if (_events.ContainsKey (key1)) {
                _events [key1].RemoveAll (ix => ix.Name == key2);
            }
        }

        /*
         * Removes Dictionary item entirely
         */
        internal void RemoveFromKey1 (string key1)
        {
            if (_events.ContainsKey (key1))
                _events.Remove (key1);
        }

        /*
         * Removes from key2
         */
        internal void RemoveFromKey2 (string key2)
        {
            foreach (var key1 in _events.Keys.ToList ()) {
                _events [key1].RemoveAll (ix => ix.Name == key2);
                if (_events [key1].Count == 0)
                    _events.Remove (key1);
            }
        }

        /*
         * Returns all lambda objects that matches given key2
         */
        internal IEnumerable<string> FindByKey2 (string key2)
        {
            foreach (var idxKey1 in _events.Keys) {
                foreach (var idxNode in _events [idxKey1].FindAll (delegate (Node idx) { return idx.Name == key2; })) {
                    yield return idxKey1;
                }
            }
        }
    }
}
