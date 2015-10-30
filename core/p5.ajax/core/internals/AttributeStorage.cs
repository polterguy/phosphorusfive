/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using System.Web.UI;
using p5.ajax.widgets;

namespace p5.ajax.core.internals
{
    /// <summary>
    ///     Class used to encapsulate all attributes for widgets. You rarely, if ever, have to fiddle with this class yourself.
    /// </summary>
    internal class AttributeStorage
    {
        private readonly List<Attribute> _dynamicallyAddedThisRequest = new List<Attribute> ();
        private readonly List<Attribute> _dynamicallyRemovedThisRequest = new List<Attribute> ();
        private readonly List<Attribute> _formDataThisRequest = new List<Attribute> ();
        // all of these will have values added and removed automatically during the request
        // depending upon when and how attributes are added and removed
        private readonly List<Attribute> _originalValue = new List<Attribute> ();
        // this is never touched after viewstate is loaded
        private readonly List<Attribute> _preViewState = new List<Attribute> ();
        private readonly List<Attribute> _viewStatePersisted = new List<Attribute> ();
        private readonly List<Attribute> _viewStatePersistedRemoved = new List<Attribute> ();

        /// <summary>
        ///     Determines whether this instance has the attribute with the specified name.
        /// </summary>
        /// <returns><c>true</c> if this instance has the attribute with the specified key; otherwise, <c>false</c>.</returns>
        /// <param name="name">The name of the attribute you wish to retrieve the value of.</param>
        public bool HasAttribute (string name)
        {
            return GetAttributeInternal (name) != null;
        }

        /// <summary>
        ///     Returns the value of the attribute with the specified name.
        /// </summary>
        /// <returns>The value of the attribute.</returns>
        /// <param name="name">The name of the attribute you wish to retrieve the value of.</param>
        public string GetAttribute (string name)
        {
            var atr = GetAttributeInternal (name);
            if (atr != null)
                return atr.Value;
            return null;
        }

        /// <summary>
        ///     Changes the value of the attribute with the specified name. If no attribute with the specified name exists, 
        ///     a new attribute will be created, with the specified name and value.
        /// </summary>
        /// <param name="name">The name of the attribute you wish to change the value of.</param>
        /// <param name="value">The new value of attribute.</param>
        public void ChangeAttribute (string name, string value)
        {
            // changing attribute, but first storing old value
            StoreOldValue (name);
            SetAttributeInternal (_dynamicallyAddedThisRequest, name, value);

            // removing from all other lists
            RemoveAttributeInternal (_dynamicallyRemovedThisRequest, name);
            RemoveAttributeInternal (_formDataThisRequest, name);
            RemoveAttributeInternal (_viewStatePersistedRemoved, name);
            RemoveAttributeInternal (_viewStatePersisted, name);
        }

        /// <summary>
        ///     Removes the attribute with the specified name.
        /// </summary>
        /// <param name="name">The name of the attribute you wish to remove.</param>
        public void RemoveAttribute (string name, bool serializeToClient = true)
        {
            if (FindAttribute (_dynamicallyAddedThisRequest, name) != null) {
                // attribute was added this request, simply removing the add
                RemoveAttributeInternal (_dynamicallyAddedThisRequest, name);
            } else {
                // changing attribute, but first storing old value, but only if caller says we should
                if (serializeToClient) {
                    StoreOldValue (name);
                    SetAttributeInternal (_dynamicallyRemovedThisRequest, name, null);
                }

                // removing from all other lists
                RemoveAttributeInternal (_dynamicallyAddedThisRequest, name);
                RemoveAttributeInternal (_formDataThisRequest, name);
                RemoveAttributeInternal (_viewStatePersistedRemoved, name);
                RemoveAttributeInternal (_viewStatePersisted, name);
            }
        }

        /// <summary>
        ///     Returns all attribute keys.
        /// </summary>
        /// <value>The names of all attributes stored in this instance.</value>
        public IEnumerable<string> Keys {
            get {
                /// \todo refactor, too much repetition
                Dictionary<string, bool> _alreadySen = new Dictionary<string, bool> ();
                foreach (var idx in this._dynamicallyAddedThisRequest) {
                    yield return idx.Name;
                    _alreadySen [idx.Name] = true;
                }
                foreach (var idx in this._formDataThisRequest) {
                    if (!_alreadySen.ContainsKey (idx.Name)) {
                        yield return idx.Name;
                        _alreadySen [idx.Name] = true;
                    }
                }
                foreach (var idx in this._originalValue) {
                    if (!_alreadySen.ContainsKey (idx.Name)) {
                        yield return idx.Name;
                        _alreadySen [idx.Name] = true;
                    }
                }
                foreach (var idx in this._preViewState) {
                    if (!_alreadySen.ContainsKey (idx.Name)) {
                        yield return idx.Name;
                        _alreadySen [idx.Name] = true;
                    }
                }
                foreach (var idx in this._viewStatePersisted) {
                    if (!_alreadySen.ContainsKey (idx.Name)) {
                        yield return idx.Name;
                        _alreadySen [idx.Name] = true;
                    }
                }
            }
        }

        /// <summary>
        ///     Sets the state the attribute has, before ViewState is loaded. Typically this is the value found in
        ///     for instance your .ASPX markup page.
        /// </summary>
        /// <param name="name">Name of attribute.</param>
        /// <param name="value">Value of attribute.</param>
        internal void SetAttributePreViewState (string name, string value)
        {
            SetAttributeInternal (_preViewState, name, value);
        }

        // invoked when form data is being retrieved from http request parameters
        /// <summary>
        ///     Sets the attribute value as read from your HTTP POST parameters, typically passed in from form element
        ///     on the client-side.
        /// </summary>
        /// <param name="name">Name of attribute.</param>
        /// <param name="value">Value of attribute.</param>
        internal void SetAttributeFormData (string name, string value)
        {
            // adding attribute to form data list
            SetAttributeInternal (_formDataThisRequest, name, value);

            // removing from all other lists
            RemoveAttributeInternal (_dynamicallyRemovedThisRequest, name);
            RemoveAttributeInternal (_dynamicallyAddedThisRequest, name);
            RemoveAttributeInternal (_viewStatePersistedRemoved, name);
            RemoveAttributeInternal (_viewStatePersisted, name);
        }

        /// <summary>
        ///     Loads the attribute values from the ViewState object given.
        /// </summary>
        /// <param name="viewStateObject">The ViewState object.</param>
        internal void LoadFromViewState (object viewStateObject)
        {
            if (viewStateObject == null)
                return;

            var vals = viewStateObject as string[][];
            foreach (var idx in vals) {
                _viewStatePersisted.Add (new Attribute (idx [0], idx [1]));
            }
        }

        /// <summary>
        ///     Loads the attributes that are removed from ViewState. Sometimes you remove an attribute
        ///     that exists for instance in your .ASPX markup. For such cases, we actually need to track which 
        ///     attributes have been removed, in addition to which have been changed. This method takes care of just that.
        /// </summary>
        /// <param name="viewstateObject">Viewstate object.</param>
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
        ///     Returns an object intended to be put into the ViewState of your page back to caller, containing
        ///     all attribute changes on your page.
        /// </summary>
        /// <returns>The attribute changes in ViewState format.</returns>
        internal object SaveToViewState ()
        {
            var atrs = new List<Attribute> ();

            // first add all that are dynamically added
            atrs.AddRange (_dynamicallyAddedThisRequest);

            // then add all that are already in the viewstate
            atrs.AddRange (_viewStatePersisted);

            // then removing all that has the same value as when they were created before viewstate was being tracked
            atrs.RemoveAll (
                delegate (Attribute idx) { return _preViewState.Exists (idxPre => idxPre.Name == idx.Name && idxPre.Value == idx.Value); });

            // nothing to return
            if (atrs.Count == 0)
                return null;

            // returning attributes
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
        /// </summary>
        /// <returns>The removed attributes.</returns>
        internal object SaveRemovedToViewState ()
        {
            var atrs = new List<Attribute> ();
            atrs.AddRange (_dynamicallyRemovedThisRequest);
            atrs.AddRange (_viewStatePersistedRemoved);

            if (atrs.Count == 0)
                return null;

            var retVal = new string[atrs.Count];
            for (var idx = 0; idx < atrs.Count; idx++) {
                retVal [idx] = atrs [0].Name;
            }
            return retVal;
        }

        /// <summary>
        ///     Renders the attributes to the given HtmlTextWriter. Used to render the attributes as HTML.
        /// </summary>
        /// <param name="writer">Where to write the attributes.</param>
        /// <param name="widget">Widget we are rendering for.</param>
        internal void Render (HtmlTextWriter writer, Widget widget)
        {
            // adding all changes
            var lst = new List<Attribute> ();
            lst.AddRange (_dynamicallyAddedThisRequest);
            lst.AddRange (_formDataThisRequest);
            lst.AddRange (_viewStatePersisted);

            // adding all that existed before viewstate was being tracked, but ONLY if they do not exist in other lists
            foreach (var idx in _preViewState) {
                if (FindAttribute (lst, idx.Name) == null)
                    lst.Add (idx);
            }

            // removing stuff that's not really attributes
            /// \todo create generic version, that allows for supplying a list of removals to do
            lst.RemoveAll (idx => idx.Name == "outerHTML" || idx.Name == "innerValue" || idx.Name == "Tag");

            // rendering to html writer
            foreach (var idx in lst) {
                var name = idx.Name;
                string value;
                if (idx.Name.StartsWith ("on") && Utilities.IsLegalMethodName (idx.Value)) {
                    if (widget.NoIdAttribute)
                        throw new ArgumentException ("cannot have events on a Widget that doesn't render its ID attribute");
                    if (name.EndsWith ("_"))
                        continue; // "invisible" event
                    value = "p5.e(event)";
                } else {
                    value = idx.Value;
                }
                writer.Write (" ");
                if (value == null) {
                    writer.Write (@"{0}", name);
                } else {
                    writer.Write (@"{0}=""{1}""", name, value.Replace ("\"", "\\\""));
                }
            }
        }

        /// <summary>
        ///     Registers the changed attributes during this request into the given Manager object, such that
        ///     changes can be rendered back to client.
        /// </summary>
        /// <param name="manager">Manager to render changes into.</param>
        /// <param name="id">ID of widget that owns storage object.</param>
        internal void RegisterChanges (Manager manager, string id)
        {
            // adding up the ones that were deleted during this request
            foreach (var idx in _dynamicallyRemovedThisRequest) {
                manager.RegisterDeletedAttribute (id, idx.Name);
            }

            // adding up our changes
            foreach (var idx in _dynamicallyAddedThisRequest) {
                // finding old value, if any
                var oldAtr = FindAttribute (_originalValue, idx.Name);
                if (oldAtr != null) {
                    if (oldAtr.Value != idx.Value)
                        manager.RegisterWidgetChanges (id, idx.Name, idx.Value, oldAtr.Value);
                } else {
                    manager.RegisterWidgetChanges (id, idx.Name, idx.Value);
                }
            }
        }

        /*
         * helper method
         */
        private void StoreOldValue (string name)
        {
            // we only store old value the first time attribute is touched
            if (FindAttribute (_originalValue, name) == null) {
                // storing old value
                var old = FindAttribute (_formDataThisRequest, name) ?? (FindAttribute (_viewStatePersisted, name) ?? FindAttribute (_preViewState, name));
                if (old != null) {
                    // "deep copy"
                    _originalValue.Add (new Attribute (old.Name, old.Value));
                }
            }
        }

        /*
         * helper method
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
         * helper method
         */
        private static void SetAttributeInternal (List<Attribute> attributes, string name, string value)
        {
            attributes.RemoveAll (idx => idx.Name == name);
            attributes.Add (new Attribute (name, value));
        }

        /*
         * helper method
         */
        private static void RemoveAttributeInternal (List<Attribute> attributes, string name)
        {
            attributes.RemoveAll (idx => idx.Name == name);
        }

        /*
         * helper method
         */
        private static Attribute FindAttribute (List<Attribute> attributes, string name)
        {
            return attributes.Find (idx => idx.Name == name);
        }
    }
}