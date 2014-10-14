/*
 * phosphorus five, copyright 2014 - thomas@magixilluminate.com
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Web.UI;
using System.Collections.Generic;
using phosphorus.ajax.core;

namespace phosphorus.ajax.core.internals
{
    internal class AttributeStorage
    {
        // this is never touched after viewstate is loaded
        private List<Attribute> _preViewState = new List<Attribute>();

        // all of these will have values added and removed automatically during the request
        // depending upon when and how attributes are added and removed
        private List<Attribute> _originalValue = new List<Attribute>();
        private List<Attribute> _dynamicallyRemovedThisRequest = new List<Attribute>();
        private List<Attribute> _dynamicallyAddedThisRequest = new List<Attribute>();
        private List<Attribute> _formDataThisRequest = new List<Attribute>();
        private List<Attribute> _viewStatePersistedRemoved = new List<Attribute>();
        private List<Attribute> _viewStatePersisted = new List<Attribute>();

        public AttributeStorage ()
        { }

        /// <summary>
        /// determines whether this instance has the attribute with the specified name
        /// </summary>
        /// <returns><c>true</c> if this instance has the attribute with the specified key; otherwise, <c>false</c></returns>
        /// <param name="name">name of attribute to retrieve value of</param>
        public bool HasAttribute (string name)
        {
            return GetAttributeInternal (name) != null;
        }

        /// <summary>
        /// returns the value of the attribute with the specified name
        /// </summary>
        /// <returns>the value of the attribute</returns>
        /// <param name="name">name of attribute to retrieve value of</param>
        public string GetAttribute (string name)
        {
            var atr = GetAttributeInternal (name);
            if (atr != null)
                return atr.Value;
            return null;
        }

        /// <summary>
        /// changes the value of the attribute with the specified name. if no attribute exists, a new attribute
        /// will be created with the specified name and value
        /// </summary>
        /// <param name="name">name of attribute to change</param>
        /// <param name="value">new value of attribute</param>
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
        /// removes the attribute with the specified name
        /// </summary>
        /// <param name="name">name of attribute to remove</param>
        public void RemoveAttribute (string name)
        {
            if (FindAttribute (_dynamicallyAddedThisRequest, name) != null) {

                // attribute was added this request, simply removing the add
                RemoveAttributeInternal (_dynamicallyAddedThisRequest, name);
            } else {

                // changing attribute, but first storing old value
                StoreOldValue (name);
                SetAttributeInternal (_dynamicallyRemovedThisRequest, name, null);
            
                // removing from all other lists
                RemoveAttributeInternal (_dynamicallyAddedThisRequest, name);
                RemoveAttributeInternal (_formDataThisRequest, name);
                RemoveAttributeInternal (_viewStatePersistedRemoved, name);
                RemoveAttributeInternal (_viewStatePersisted, name);
            }
        }

        // invoked before viewstate is being tracked
        internal void SetAttributePreViewState (string name, string value)
        {
            SetAttributeInternal (_preViewState, name, value);
        }

        // invoked when form data is being retrieved from http request parameters
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

        internal void LoadFromViewState (object viewstateObject)
        {
            if (viewstateObject == null)
                return;

            string[][] vals = viewstateObject as string[][];
            foreach (string[] idx in vals) {
                _viewStatePersisted.Add (new Attribute (idx [0], idx [1]));
            }
        }

        internal void LoadRemovedFromViewState (object viewstateObject)
        {
            if (viewstateObject == null)
                return;

            string[] vals = viewstateObject as string[];
            foreach (string idx in vals) {
                _viewStatePersistedRemoved.Add (new Attribute (idx));
            }
        }
        
        internal object SaveToViewState()
        {
            List<Attribute> atrs = new List<Attribute> ();

            // first add all that are dynamically added
            atrs.AddRange (_dynamicallyAddedThisRequest);

            // then add all that are already in the viewstate
            atrs.AddRange (_viewStatePersisted);

            // then removing all that has the same value as when they were created before viewstate was being tracked
            atrs.RemoveAll (
                delegate(Attribute idx) {
                return _preViewState.Exists (
                    delegate(Attribute idxPre) {
                    return idxPre.Name == idx.Name && idxPre.Value == idx.Value;
                });
            });

            // nothing to return
            if (atrs.Count == 0)
                return null;

            // returning attributes
            string[][] retVal = new string [atrs.Count][];
            for (int idx = 0; idx < atrs.Count; idx++) {
                retVal [idx] = new string [2];
                retVal [idx] [0] = atrs [idx].Name;
                retVal [idx] [1] = atrs [idx].Value;
            }
            return retVal;
        }

        internal object SaveRemovedToViewState()
        {
            List<Attribute> atrs = new List<Attribute> ();
            atrs.AddRange (_dynamicallyRemovedThisRequest);
            atrs.AddRange (_viewStatePersistedRemoved);

            if (atrs.Count == 0)
                return null;

            string[] retVal = new string [atrs.Count];
            for (int idx = 0; idx < atrs.Count; idx++) {
                retVal [idx] = atrs [0].Name;
            }
            return retVal;
        }

        // invoked when rendering of attributes to html is required
        internal void Render (HtmlTextWriter writer)
        {
            // adding all changes
            List<Attribute> lst = new List<Attribute> ();
            lst.AddRange (_dynamicallyAddedThisRequest);
            lst.AddRange (_formDataThisRequest);
            lst.AddRange (_viewStatePersisted);

            // adding all that existed before viewstate was being tracked, but ONLY if they do not exist in other lists
            foreach (Attribute idx in _preViewState) {
                if (FindAttribute (lst, idx.Name) == null)
                    lst.Add (idx);
            }

            // removing stuff that's not really attributes
            // TODO: create generic version, that allows for supplying a list of removals to do
            lst.RemoveAll (
                delegate(Attribute idx) {
                    return idx.Name == "outerHTML" || idx.Name == "innerHTML" || idx.Name == "Tag";
                });

            // rendering to html writer
            foreach (Attribute idx in lst) {
                string name = idx.Name;
                string value;
                if (idx.Name.StartsWith ("on") && Utilities.IsLegalMethodName (idx.Value)) {
                    value = "pf.e(event)";
                } else {
                    value = idx.Value;
                }
                writer.Write (" ");
                if (value == null) {
                    writer.Write (string.Format (@"{0}", name));
                } else {
                    writer.Write (string.Format (@"{0}=""{1}""", name, value.Replace ("\"", "\\\"")));
                }
            }
        }

        internal void RegisterChanges (Manager manager, string id)
        {
            // adding up the ones that were deleted during this request
            foreach (Attribute idx in _dynamicallyRemovedThisRequest) {
                manager.RegisterDeletedAttribute (id, idx.Name);
            }

            // adding up our changes
            foreach (Attribute idx in _dynamicallyAddedThisRequest) {

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

        private void StoreOldValue (string name)
        {
            // we only store old value the first time attribute is touched
            if (FindAttribute (_originalValue, name) == null) {

                // storing old value
                var old = FindAttribute (_formDataThisRequest, name);
                if (old == null) {
                    old = FindAttribute (_viewStatePersisted, name);
                    if (old == null) {
                        old = FindAttribute (_preViewState, name);
                    }
                }
                if (old != null) {

                    // "deep copy"
                    _originalValue.Add (new Attribute (old.Name, old.Value));
                }
            }
        }

        private Attribute GetAttributeInternal(string name)
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
            if (preViewState != null)
                return preViewState;

            return null;
        }

        private void SetAttributeInternal (List<Attribute> attributes, string name, string value)
        {
            attributes.RemoveAll (
                delegate(Attribute idx) {
                return idx.Name == name;
            });
            attributes.Add (new Attribute (name, value));
        }

        private void RemoveAttributeInternal (List<Attribute> attributes, string name)
        {
            attributes.RemoveAll (
                delegate(Attribute idx) {
                return idx.Name == name;
            });
        }

        private Attribute FindAttribute (List<Attribute> attributes, string name)
        {
            return attributes.Find (delegate(Attribute idx) {
                return idx.Name == name;
            });
        }
    }
}

