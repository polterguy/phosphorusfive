/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.Linq;
using System.Collections.Generic;
using p5.core;

namespace p5.webapp.code
{
    /*
     * Used as storage for Ajax events for Widgets and lambda events for widgets
     */
    [Serializable]
    internal class WidgetEventStorage
    {
        private Dictionary<string, List<Node>> _events = new Dictionary<string, List<Node>>();

        internal IEnumerable<Node> this [string key1]
        {
            get {
                if (_events.ContainsKey(key1))
                    return _events[key1];
                return new Node[] { };
            }
        }

        internal Node this [string key1, string key2]
        {
            get {
                if (!_events.ContainsKey(key1))
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
         * Removes Dictionary item entirely
         */
        internal void Remove (string key1, string key2)
        {
            if (_events.ContainsKey(key1)) {
                _events[key1].RemoveAll(ix => ix.Name == key2);
            }
        }

        /*
         * Removes Dictionary item entirely
         */
        internal void RemoveFromKey1 (string key1)
        {
            if (_events.ContainsKey(key1))
                _events.Remove(key1);
        }
                
        /*
         * Removes from key2
         */
        internal void RemoveFromKey2 (string key2)
        {
            foreach (var key1 in _events.Keys.ToList ()) {
                _events[key1].RemoveAll(ix => ix.Name == key2);
                if (_events[key1].Count == 0)
                    _events.Remove(key1);
            }
        }

        /*
         * returns all lambda objects that matches given key2
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
