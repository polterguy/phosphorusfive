/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, thomas@gaiasoul.com
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

using System.Web.UI;
using System.Collections.Generic;
using p5.ajax.widgets;

namespace p5.ajax.core.internals
{
    /// <summary>
    ///     Class used to encapsulate all attributes for widgets.
    /// </summary>
    internal class AttributeStorage
    {
        // All of these will have values added and removed automatically during the request, depending upon when 
        // and how attributes are added and removed.
        // Notice, since we can have a mix of attributes declared in the .aspx markup, in addition to having these removed,
        // while other attributes are added dynamically during callbacks, etc - We need to keep track of which attributes
        // are originally in the markup, which are removed, and which are added.
        // Hence, we need several lists of attributes, to know which are removed, added, and so on, during requests, for each widget.

        // List of attributes that are dynamically added during the current request.
        private readonly List<Attribute> _dynamicallyAddedThisRequest = new List<Attribute> ();

        // List of attributes that are dynamically removed during the current request.
        private readonly List<Attribute> _dynamicallyRemovedThisRequest = new List<Attribute> ();

        // List of attributes (mostly form HTTP POST values) that are sent from the client as FORM data during the current request.
        private readonly List<Attribute> _formDataThisRequest = new List<Attribute> ();

        // Original value for attribute, before ViewState is de-serialized.
        private readonly List<Attribute> _preViewState = new List<Attribute> ();

        // List of attributes that are persisted in the ViewState during the current request.
        private readonly List<Attribute> _viewStatePersisted = new List<Attribute> ();

        // List of attributes that are persistently removed from the widget in ViewState.
        // Necessary to keep track of attributes that are declared in .aspx, but removed later.
        private readonly List<Attribute> _viewStatePersistedRemoved = new List<Attribute> ();

        /// <summary>
        ///     Determines whether this instance has the attribute with the specified name.
        /// </summary>
        /// <returns><c>true</c> if this instance has the attribute with the specified key; otherwise, <c>false</c></returns>
        /// <param name="name">The name of the attribute you wish to retrieve the value of</param>
        public bool HasAttribute (string name)
        {
            return GetAttributeInternal (name) != null;
        }

        /// <summary>
        ///     Returns the value of the attribute with the specified name.
        /// </summary>
        /// <returns>The value of the attribute</returns>
        /// <param name="name">The name of the attribute you wish to retrieve the value of</param>
        public string GetAttribute (string name)
        {
            var atr = GetAttributeInternal (name);
            if (atr != null)
                return atr.Value;
            return null;
        }

        /// <summary>
        ///     Changes the existing, or creates a new value, for the attribute with the specified name.
        /// </summary>
        /// <param name="name">The name of the attribute you wish to change the value of</param>
        /// <param name="value">The new value of attribute</param>
        public void ChangeAttribute (string name, string value)
        {
            SetAttributeInternal (_dynamicallyAddedThisRequest, name, value);

            // Removing attribute from all other lists.
            RemoveAttributeInternal (_dynamicallyRemovedThisRequest, name);
            RemoveAttributeInternal (_formDataThisRequest, name);
            RemoveAttributeInternal (_viewStatePersistedRemoved, name);
            RemoveAttributeInternal (_viewStatePersisted, name);
        }

        /// <summary>
        ///     Removes the attribute with the specified name.
        /// </summary>
        /// <param name="name">The name of the attribute you wish to remove</param>
        public void RemoveAttribute (string name, bool serializeToClient = true)
        {
            if (FindAttribute (_dynamicallyAddedThisRequest, name) != null) {

                // Attribute was added this request, simply removing the add.
                RemoveAttributeInternal (_dynamicallyAddedThisRequest, name);
            } else {

                // Changing attribute, and storing its old value, but only if caller says we should serialize the attribute change back to the client.
                if (serializeToClient) {
                    SetAttributeInternal (_dynamicallyRemovedThisRequest, name, null);
                }

                // Removing the attribute from all other lists.
                RemoveAttributeInternal (_dynamicallyAddedThisRequest, name);
                RemoveAttributeInternal (_formDataThisRequest, name);
                RemoveAttributeInternal (_viewStatePersistedRemoved, name);
                RemoveAttributeInternal (_viewStatePersisted, name);
                RemoveAttributeInternal (_preViewState, name);
            }
        }

        /// <summary>
        ///     Returns all attribute keys.
        /// </summary>
        /// <value>The names of all attributes stored in this instance</value>
        public IEnumerable<string> Keys {
            get {

                // In order to not yield the same attribute several times, we need to keep track of which attributes we have already seen.
                Dictionary<string, bool> _alreadySen = new Dictionary<string, bool> ();

                // Making sure we first iterate through all attributes that were removed, such that we do NOT yield those to caller.
                foreach (var idx in _dynamicallyRemovedThisRequest) {
                    _alreadySen [idx.Name] = true;
                }

                // Then yielding all attributes for widget back to caller.
                foreach (var idx in _dynamicallyAddedThisRequest) {
                    if (!_alreadySen.ContainsKey (idx.Name)) {
                        yield return idx.Name;
                        _alreadySen [idx.Name] = true;
                    }
                }
                foreach (var idx in _formDataThisRequest) {
                    if (!_alreadySen.ContainsKey (idx.Name)) {
                        yield return idx.Name;
                        _alreadySen [idx.Name] = true;
                    }
                }
                foreach (var idx in _preViewState) {
                    if (!_alreadySen.ContainsKey (idx.Name)) {
                        yield return idx.Name;
                        _alreadySen [idx.Name] = true;
                    }
                }
                foreach (var idx in _viewStatePersisted) {
                    if (!_alreadySen.ContainsKey (idx.Name)) {
                        yield return idx.Name;
                        _alreadySen [idx.Name] = true;
                    }
                }
            }
        }

        /// <summary>
        ///     Sets the state the attribute has, before ViewState is loaded.
        /// </summary>
        /// <param name="name">Name of attribute</param>
        /// <param name="value">Value of attribute</param>
        internal void SetAttributePreViewState (string name, string value)
        {
            SetAttributeInternal (_preViewState, name, value);
        }

        /// <summary>
        ///     Sets the attribute value as read from your HTTP POST form data.
        /// </summary>
        /// <param name="name">Name of attribute</param>
        /// <param name="value">Value of attribute</param>
        internal void SetAttributeFormData (string name, string value)
        {
            // Making sure we normalize Carriage Return, such that they're always in "canonical form".
            if (value != null && value.Contains ("\n") && !value.Contains ("\r\n"))
                value = value.Replace ("\n", "\r\n");

            // Adding attribute to form data list.
            SetAttributeInternal (_formDataThisRequest, name, value);

            // Removing from all other lists.
            RemoveAttributeInternal (_dynamicallyRemovedThisRequest, name);
            RemoveAttributeInternal (_dynamicallyAddedThisRequest, name);
            RemoveAttributeInternal (_viewStatePersistedRemoved, name);
            RemoveAttributeInternal (_viewStatePersisted, name);
        }

        /// <summary>
        ///     Loads the attribute values from the ViewState object given.
        /// </summary>
        /// <param name="viewStateObject">The ViewState object</param>
        internal void LoadFromViewState (object viewStateObject)
        {
            if (viewStateObject == null)
                return;

            var vals = viewStateObject as string[][];
            foreach (var idx in vals) {

                // Skipping "id" attribute, which might end up in our list, whenever a widget has its ID changed.
                if (idx [0] != "id")
                    _viewStatePersisted.Add (new Attribute (idx [0], idx [1]));
            }
        }

        /// <summary>
        ///     Loads the attributes that are persistently removed from ViewState.
        ///     Necessary to make it possible to remove attributes that are declared in .aspx markup.
        /// </summary>
        /// <param name="viewstateObject">Viewstate object</param>
        internal void LoadRemovedFromViewState (object viewstateObject)
        {
            if (viewstateObject == null)
                return;

            var vals = viewstateObject as string[];
            foreach (var idx in vals) {
                _viewStatePersistedRemoved.Add (new Attribute (idx));
            }
        }

        /// <summary>
        ///     Returns an object intended to be put into the ViewState back to caller.
        ///     Invoked during serialization of ViewState for control/widget.
        /// </summary>
        /// <returns>The attribute changes in ViewState format</returns>
        internal object SaveToViewState (Widget widget)
        {
            var atrs = new List<Attribute> ();

            // First add all that are dynamically added.
            atrs.AddRange (_dynamicallyAddedThisRequest);

            // Then add all that are already in the ViewState.
            atrs.AddRange (_viewStatePersisted);

            // Then if widget is not visible, we add the form data attributes, to make sure they don't disappear during next callback.
            // This is necessary in order to have a widget keep its FORM data value(s), if it is later made invisible.
            if (!widget.Visible)
                atrs.AddRange (_formDataThisRequest);

            // Then removing all attributes that has the same value as when they were created before ViewState was being tracked.
            atrs.RemoveAll (ix => _preViewState.Exists (ixPre => ixPre.Name == ix.Name && ixPre.Value == ix.Value));

            // Checking if we have anything to serialize into our ViewState, and if not, returning null, early.
            if (atrs.Count == 0)
                return null;

            // Returning attributes, as a two dimensional array back to caller.
            var retVal = new string[atrs.Count][];
            for (var idx = 0; idx < atrs.Count; idx++) {

                retVal [idx] = new string[2];
                retVal [idx] [0] = atrs [idx].Name;
                retVal [idx] [1] = atrs [idx].Value;
            }
            return retVal;
        }

        /// <summary>
        ///     Returns an object intended to be put into your ViewState containing all the removed attributes back to caller.
        ///     Invoked during serialization of ViewState for control/widget.
        /// </summary>
        /// <returns>The removed attributes</returns>
        internal object SaveRemovedToViewState ()
        {
            var atrs = new List<Attribute> ();
            atrs.AddRange (_dynamicallyRemovedThisRequest);
            atrs.AddRange (_viewStatePersistedRemoved);

            // Making sure we have something to serialize.
            if (atrs.Count == 0)
                return null;

            // Returing attribute names as an array, to serialize into ViewState.
            // Notice, no need to serialize value (obviously!)
            var retVal = new string[atrs.Count];
            for (var idx = 0; idx < atrs.Count; idx++) {
                retVal [idx] = atrs [0].Name;
            }
            return retVal;
        }

        /// <summary>
        ///     Renders the attributes to the given HtmlTextWriter.
        /// </summary>
        /// <param name="writer">Where to render the attribute names/values.</param>
        internal void Render (HtmlTextWriter writer)
        {
            // Adding all changes we need to render.
            var lst = new List<Attribute> ();
            lst.AddRange (_dynamicallyAddedThisRequest);
            lst.AddRange (_formDataThisRequest);
            lst.AddRange (_viewStatePersisted);

            // Adding all that existed before viewstate was being tracked, but ONLY if they do not exist in other lists.
            foreach (var idx in _preViewState) {
                if (FindAttribute (lst, idx.Name) == null)
                    lst.Add (idx);
            }

            // Removing stuff that's not actually attributes, but still persisted here for convenience,
            // in addition to all "private attributes", meaning server-side only attributes.
            lst.RemoveAll (ix => ix.Name == "outerHTML" || ix.Name == "innerValue" || ix.Name == "Element" || ix.Name.StartsWith ("_") || ix.Name.StartsWith ("."));

            // Rendering attributes, both names and values, to HtmlTextWriter.
            foreach (var idx in lst) {
                var name = idx.Name;
                string value;
                if (idx.Name.StartsWith ("on") && Utilities.IsLegalMethodName (idx.Value)) {

                    // This is an Ajax event.
                    value = "p5.e(event)";

                } else {

                    // This is either a normal attribute, or a JavaScript DOM event handler, with inline JavaScript.
                    value = idx.Value;
                }

                // Rendering attribute, making sure we support "empty" attributes, and that we escape double quotes.
                writer.Write (value == null ? string.Format (@" {0}", name) : string.Format (@" {0}=""{1}""", name, value.Replace ("\"", "&quot;")));
            }
        }

        /// <summary>
        ///     Registers the changed attributes during this request.
        /// </summary>
        /// <param name="manager">Manager to render changes into</param>
        /// <param name="id">ID of widget that owns storage object</param>
        internal void RegisterChanges (AjaxPage page, string id)
        {
            // Adding up the ones that were deleted during this request.
            foreach (var idx in _dynamicallyRemovedThisRequest) {
                page.RegisterDeletedAttribute (id, idx.Name);
            }

            // Adding up our changes.
            foreach (var idx in _dynamicallyAddedThisRequest) {

                // Checking if this is an invisible attribute, at which case it is never rendered back to client.
                if (idx.Name.StartsWith ("_") || idx.Name.StartsWith ("."))
                    continue;

                // Registering change to be returned to client.
                page.RegisterWidgetChanges (id, idx.Name, idx.Value);
            }
        }

        /*
         * Helper method for retrieving attribute
         */
        private Attribute GetAttributeInternal (string name)
        {
            var added = FindAttribute (_dynamicallyAddedThisRequest, name);
            if (added != null)
                return added;

            var form = FindAttribute (_formDataThisRequest, name);
            if (form != null)
                return form;

            var viewStatePersisted = FindAttribute (_viewStatePersisted, name);
            if (viewStatePersisted != null)
                return viewStatePersisted;

            var viewStateRemoved = FindAttribute (_viewStatePersistedRemoved, name);
            if (viewStateRemoved != null)
                return null; // if attribute was removed during viewstate load, we DO NOT check values that were saved before viewstate was being tracked

            var removed = FindAttribute (_dynamicallyRemovedThisRequest, name);
            if (removed != null)
                return null; // if attribute was removed this request, we DO NOT check values that were saved before viewstate was being tracked

            // last resort ...
            var preViewState = FindAttribute (_preViewState, name);
            return preViewState;
        }

        /*
         * Helper method for changing or setting attribute
         */
        private static void SetAttributeInternal (List<Attribute> attributes, string name, string value)
        {
            attributes.RemoveAll (idx => idx.Name == name);
            attributes.Add (new Attribute (name, value));
        }

        /*
         * Helper method for removing attribute
         */
        private static void RemoveAttributeInternal (List<Attribute> attributes, string name)
        {
            attributes.RemoveAll (idx => idx.Name == name);
        }

        /*
         * Helper method for retrieving attribute
         */
        private static Attribute FindAttribute (List<Attribute> attributes, string name)
        {
            return attributes.Find (idx => idx.Name == name);
        }
    }
}