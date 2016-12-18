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

using System;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Collections.Generic;
using p5.ajax.core;

namespace p5.ajax.widgets
{
    /// <summary>
    ///     A widget that can contains children widgets.
    /// </summary>
    [ViewStateModeById]
    public class Container : Widget, INamingContainer
    {
        // Contains all the creator objects to create our controls when needed.
        private static readonly Dictionary<Type, ICreator> _creators = new Dictionary<Type, ICreator> ();
        private static readonly List<Tuple<string, Type>> _typeMappers = new List<Tuple<string, Type>> ();

        // Used to lock GetCreator to make sure we don't get a race condition when instantiating new creators.
        private static readonly object _lock = new object ();

        // Contains the original controls collection, before we started adding and removing controls for current request.
        private List<Control> _originalCollection;

        /*
         * Overridden to make sure the default element for this widget is "div".
         */
        public override string Element
        {
            get
            {
                if (string.IsNullOrEmpty (base.Element))
                    return "div";
                return base.Element;
            }
            set { base.Element = value; }
        }

        /*
         * Overridden to make sure we handle "value" attribute for "select" HTML widget, in addition to throwing an exception,
         * if user tries to set or get the "innerValue" property/attribute of widget.
         */
        public override string this [string name]
        {
            get {
                if (name == "innerValue")
                    throw new ArgumentException ("You cannot get the 'innerValue' property of a Container widget");

                // Special treatment for select HTML elements, to make it resemble what goes on on the client-side.
                if (name == "value" && Element == "select") {

                    // Returning each selected "option" element separated by comma, in case this is a multi select widget.
                    string retVal = "";
                    foreach (Widget idxWidget in Controls) {
                        if (idxWidget.HasAttribute ("selected"))
                            retVal += idxWidget ["value"] + ",";
                    }
                    return retVal.TrimEnd (','); // Removing last comma ",".
                } else {
                    return base [name];
                }
            }
            set {
                if (name == "innerValue")
                    throw new ArgumentException ("You cannot set the 'innerValue' property of a Container widget");

                // Special treatment for select HTML elements, to make it resemble what goes on on the client-side.
                if (name == "value" && Element == "select") {

                    // Splitting specified value by comma ",", and adding the "selected" attribute for each option element with a value
                    // matching anything in the split results.
                    foreach (Widget idxWidget in Controls) {
                        idxWidget.DeleteAttribute ("selected"); // DeleteAttribute will check if attribute exists before attempting to delete it.
                    }
                    foreach (string idxSplit in value.Split (',')) {
                        foreach (Widget idxWidget in Controls) {
                            if (idxWidget["value"] == idxSplit) {
                                idxWidget ["selected"] = null;
                            }
                        }
                    }
                } else {
                    base [name] = value;
                }
            }
        }

        /*
         * Overridden to make sure we can correctly handle "select" HTML widgets.
         */
        public override bool HasAttribute (string name)
        {
            if (name == "value" && Element == "select") {

                // Special treatment for select HTML elements, to make it resemble what goes on on the client-side.
                foreach (Widget idxWidget in Controls) {
                    if (idxWidget.HasAttribute ("selected"))
                        return true;
                }
                return false;
            } else {
                return base.HasAttribute (name);
            }
        }

        /*
         * Implementation of abstract base class method, to make sure we return true, only if widget has children widgets.
         */
        protected override bool HasContent
        {
            get { return Controls.Count > 0; }
        }

        /// <summary>
        ///     Returns all controls of the specified type T from the Controls collection.
        /// 
        ///     Useful to avoid returning automatically created LiteralControls, due to formatting applied in .aspx file.
        /// </summary>
        /// <returns>All controls of type T from the Controls property.</returns>
        /// <typeparam name="T">Type of controls to retrieve.</typeparam>
        public IEnumerable<T> ControlsOfType<T> () where T : Control
        {
            return from Control idx in Controls let tmp = idx as T where idx != null select tmp;
        }

        /// <summary>
        ///     Creates a persistent child control, that will be automatically re-created during future server requests.
        /// </summary>
        /// <returns>The persistent control.</returns>
        /// <param name="id">ID of your control. If null, an automatic id will be created and assigned control.</param>
        /// <param name="index">Index of where to insert control. If -1, the control will be appended into Controls collection.</param>
        /// <typeparam name="T">The type of control you want to create.</typeparam>
        public T CreatePersistentControl<T> (string id = null, int index = -1) where T : Control, new ()
        {
            StoreOriginalControls ();
            // TODO: Figure out why this seems to work ...???
            ReRenderChildren ();

            // Creating a new control, and adding to the controls collection.
            var control = GetCreator<T> ().Create () as T;
            control.ID = string.IsNullOrEmpty (id) ? CreateUniqueId () : id;

            if (index == -1)
                Controls.Add (control);
            else
                Controls.AddAt (index, control);

            // Returning newly created control back to caller, such that he can set other properties and such for it.
            return control;
        }

        /// <summary>
        ///     Removes a control from the control collection, and persists the change.
        /// </summary>
        /// <param name="control">Control to remove</param>
        public void RemoveControlPersistent (Control control)
        {
            StoreOriginalControls ();
            Controls.Remove (control);
            ReRenderChildren ();
        }

        /// <summary>
        ///     Removes a control from the control collection, at the given index, and persists the change.
        /// </summary>
        /// <param name="index">Index of control to remove</param>
        public void RemoveControlPersistentAt (int index)
        {
            StoreOriginalControls ();
            Controls.RemoveAt (index);
            ReRenderChildren ();
        }

        /*
         * Overridden to make sure we can correctly reload the Controls collection of widget, as persisted during SaveViewState.
         */
        protected override void LoadViewState (object savedState)
        {
            // Reloading persisted controls, if there are any.
            var tmp = savedState as object[];
            if (tmp != null && tmp.Length > 0 && tmp [0] is string[][]) {

                // We're managing our own controls collection, and need to reload from ViewState all the 
                // control types and ids. First figuring out which controls actually exists in this control at the moment.
                var ctrlsViewstate = (from idx in (string[][]) tmp [0] select new Tuple<string, string> (idx [0], idx [1])).ToList ();

                // Then removing all controls that are not persisted, and all LiteralControls since they tend to mess up their IDs.
                var toRemove = Controls.Cast<Control> ().Where (
                    idxControl => string.IsNullOrEmpty (idxControl.ID) || !ctrlsViewstate.Exists (idxViewstate => idxViewstate.Item2 == idxControl.ID)).ToList ();
                foreach (var idxCtrl in toRemove) {
                    Controls.Remove ((Control)idxCtrl);
                }

                // Then adding all controls that are persisted but does not exist in the controls collection
                var controlPosition = 0;
                foreach (var idxTuple in ctrlsViewstate) {
                    var exist = Controls.Cast<Control> ().Any (idxCtrl => idxTuple.Item2 == idxCtrl.ID);
                    if (!exist) {
                        var control = _creators [GetTypeFromId (idxTuple.Item1)].Create ();
                        control.ID = idxTuple.Item2;
                        Controls.AddAt (controlPosition, control);
                    }
                    controlPosition += 1;
                }

                StoreOriginalControls ();

                base.LoadViewState (tmp [1]);

            } else {
                base.LoadViewState (savedState);
            }
        }

        /*
         * Making sure we can persist the Controls collection into ViewState.
         */
        protected override object SaveViewState ()
        {
            // Making sure all dynamically added controls are persistent to the control state, if there are any.
            if (_originalCollection != null) {

                // Yup, we're managing our own control collection, and need to save to viewstate all of the controls
                // types and ids that exists in our control collection
                var tmp = new object[2];
                tmp [0] = (from Control idx in Controls where !string.IsNullOrEmpty (idx.ID) select new[] { GetTypeId(idx.GetType ()), idx.ID}).ToArray();
                tmp [1] = base.SaveViewState ();
                return tmp;
            }

            // Not managing controls.
            return base.SaveViewState ();
        }

        /*
         * Overridden to make sure we remove all LiteralControls during Ajax requests.
         */
        protected override void OnInit (EventArgs e)
        {
            // Making sure all the automatically generated LiteralControls are removed, since they mess up their IDs,
            // but not in a normal postback, or initial loading of the page, since we need the formatting they provide.
            if (AjaxPage.IsAjaxRequest) {
                foreach (var idx in Controls.Cast<Control> ().Where (idx => string.IsNullOrEmpty (idx.ID)).ToList ()) {
                    Controls.Remove (idx);
                }
            }
            base.OnInit (e);
        }

        protected override void RenderChildrenWidgetsAsJson (HtmlTextWriter writer)
        {
            if (_originalCollection == null) {

                base.RenderChildrenWidgetsAsJson (writer);

            } else {

                RenderRemovedControls ();
                RenderAddedControls ();
                RenderOldControls (writer);
            }
        }

        /// <summary>
        ///     Creates a new Unique ID for a Control
        /// </summary>
        /// <returns>The identifier</returns>
        public static string CreateUniqueId ()
        {
            var retVal = Guid.NewGuid ().ToString ().Replace ("-", "");
            retVal = "x" + retVal [0] + retVal [5] + retVal [10] + retVal [15] + retVal [20] + retVal [25] + retVal [30];
            return retVal;
        }

        // Renders all controls that was added this request, and return list back to caller
        private void RenderAddedControls ()
        {
            var widgets = new List<Tuple<string, int>> ();
            foreach (Control idx in Controls) {
                if (_originalCollection.Contains (idx) || string.IsNullOrEmpty (idx.ID))
                    continue; // Control has already been rendered, or is a literal control without an ID

                // Getting control's html
                string html;
                using (var stream = new MemoryStream()) {
                    using (var txt = new HtmlTextWriter(new StreamWriter(stream))) {
                        idx.RenderControl (txt);
                        txt.Flush ();
                    }
                    stream.Seek (0, SeekOrigin.Begin);
                    using (TextReader reader = new StreamReader(stream)) {
                        html = reader.ReadToEnd ();
                    }
                }
                var position = Controls.IndexOf (idx);
                widgets.Add (new Tuple<string, int>(html, position));
            }

            // We have to insert such that the first controls becomes added before controls behind it, such that the dom position
            // don't become messed up
            widgets.Sort (
                (lhs, rhs) => lhs.Item2.CompareTo (rhs.Item2));

            // Informing our manager that the current widget has changes, if we should
            if (widgets.Count > 0) {
                foreach (var idx in widgets) {
                    AjaxPage.RegisterWidgetChanges (ClientID, "__p5_add_" + idx.Item2, idx.Item1);
                }
            }
        }

        // Renders all controls that was removed, and returns list back to caller
        private void RenderRemovedControls ()
        {
            foreach (var idxOriginal in _originalCollection) {
                var exist = Controls.Cast<Control> ().Any (idxActual => idxActual == idxOriginal);
                if (!exist && !string.IsNullOrEmpty (idxOriginal.ID))
                    AjaxPage.RegisterDeletedWidget (idxOriginal.ClientID);
            }
        }

        private void RenderOldControls (HtmlTextWriter writer)
        {
            var old = RenderMode;
            RenderMode = RenderingMode.Default;
            foreach (Control idx in Controls) {
                if (_originalCollection.Contains (idx)) {
                    idx.RenderControl (writer);
                }
            }
            RenderMode = old;
        }

        // Storing original controls that were there before we started adding and removing controls
        private void StoreOriginalControls ()
        {
            if (_originalCollection == null) {
                _originalCollection = new List<Control> ();
                foreach (Control idxCtrl in Controls) {
                    _originalCollection.Add (idxCtrl);
                }
            }
        }

        // Use to make sure we store a reference to our creator instance for later requests
        private static ICreator GetCreator<T> () where T : Control, new ()
        {
            if (!_creators.ContainsKey (typeof (T))) {
                lock (_lock) {
                    if (!_creators.ContainsKey (typeof (T)))
                        _creators [typeof (T)] = new Creator<T> ();
                }
            }
            return _creators [typeof (T)];
        }

        // Used to "pack" the types stored in the ViewState to make viewstate as small as possible
        private static string GetTypeId (Type type)
        {
            foreach (var idx in _typeMappers) {
                if (idx.Item2 == type)
                    return idx.Item1;
            }

            // Didn't exist, need to create it
            lock (_lock) {
                if (!_typeMappers.Exists (
                    idx => idx.Item2 == type))
                    _typeMappers.Add (new Tuple<string, Type> (_typeMappers.Count.ToString (), type));
            }

            // Recursively calling self for simplicity
            return GetTypeId (type);
        }

        // Used to retrieve the type from the type mapper collection
        private static Type GetTypeFromId (string id)
        {
            return _typeMappers.Find (idx => idx.Item1 == id).Item2;
        }

        // Interface to create controls to avoid reflection as much as possible
        private interface ICreator
        {
            Control Create ();
        }

        // Some "anti-reflection magic"
        private class Creator<T> : ICreator where T : Control, new ()
        {
            public Control Create () { return new T (); }
        }
    }
}
