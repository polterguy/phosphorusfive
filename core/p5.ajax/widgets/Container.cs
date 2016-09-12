/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Collections.Generic;
using p5.ajax.core;

/// <summary>
///     Contains all the main widgets in p5 ajax
/// </summary>
namespace p5.ajax.widgets
{
    /// <summary>
    ///     A widget that can contains children widgets of its own
    /// </summary>
    [ViewStateModeById]
    public class Container : Widget, INamingContainer
    {
        // Contains all the creator objects to create our controls when needed
        private static readonly Dictionary<Type, ICreator> Creators = new Dictionary<Type, ICreator> ();
        private static readonly List<Tuple<string, Type>> TypeMapper = new List<Tuple<string, Type>> ();

        // Used to lock GetCreator to make sure we don't get a race condition when instantiating new creators
        private static readonly object Lock = new object ();

        // Contains the original controls collection, before we started adding and removing controls for current request
        private List<Control> _originalCollection;

        // Overridden to supply default element
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

        public override string this [string name]
        {
            get {
                if (name == "value" && Element == "select" && AllChildrenHasIds ()) {

                    // Special treatment for select HTML elements, to make it resemble what goes on on the client-side
                    string retVal = "";
                    foreach (Control idxCtrl in Controls) {
                        var idxWidget = idxCtrl as Widget;
                        if (idxWidget != null) {
                            if (idxWidget.HasAttribute ("selected"))
                                retVal += idxWidget ["value"] + ",";
                        }
                    }
                    return retVal.TrimEnd (',');
                }
                return base [name];
            }
            set {
                if (name == "innerValue")
                    throw new ArgumentException ("you cannot set the 'innerValue' property of the '" + ID + "' Container widget");
                if (name == "value" && Element == "select" && AllChildrenHasIds ()) {

                    // Special treatment for select HTML elements, to make it resemble what goes on on the client-side
                    var splits = value.Split (',');
                    foreach (Control idxCtrl in Controls) {
                        var idxWidget = idxCtrl as Widget;
                        if (idxWidget != null) {
                            idxWidget.RemoveAttribute ("selected");
                        }
                    }
                    foreach (string idxSplit in splits) {
                        foreach (Control idxCtrl in Controls) {
                            var idxWidget = idxCtrl as Widget;
                            if (idxWidget != null) {
                                if (idxWidget ["value"] == idxSplit) {
                                    idxWidget ["selected"] = null;
                                }
                            }
                        }
                    }
                    return;
                }
                base [name] = value;
            }
        }

        public override bool HasAttribute (string name)
        {
            if (name == "value" && Element == "select") {

                // Special treatment for select HTML elements, to make it resemble what goes on on the client-side
                foreach (Control idxCtrl in Controls) {
                    var idxWidget = idxCtrl as Widget;
                    if (idxWidget != null) {
                        if (idxWidget.HasAttribute ("selected"))
                            return true;
                    }
                }
            }
            return base.HasAttribute (name);
        }

        protected override bool HasContent
        {
            get { return Controls.Count > 0; }
        }

        /// <summary>
        ///     Returns all controls of the given type T from the Controls collection
        /// </summary>
        /// <returns>All controls of type T from the Controls property</returns>
        /// <typeparam name="T">Type of controls to retrieve</typeparam>
        public IEnumerable<T> GetChildControls<T> () where T : Control
        {
            return from Control idx in Controls let tmp = idx as T where idx != null select tmp;
        }

        /// <summary>
        ///     Creates a persistent child control, that will be automatically re-created during future server requests
        /// </summary>
        /// <returns>The persistent control</returns>
        /// <param name="id">ID of your control. If null, an automatic id will be created and assigned</param>
        /// <param name="index">Index of where to insert control. If -1, the control will be appended into Controls collection</param>
        /// <param name="onLoad">Event handler callback for what to do during OnLoad. If you supply an event handler here, then your 
        /// method will be invoked during LoadComplete of your Page, allowing you to have initialization functionality for your control</param>
        /// <typeparam name="T">The type of control you wish to create</typeparam>
        public T CreatePersistentControl<T> (
            string id = null, 
            int index = -1) where T : Control, new ()
        {
            StoreOriginalControls ();
            ReRenderChildren ();

            // Creating new control, and adding to the controls collection
            var control = GetCreator<T> ().Create () as T;
            control.ID = string.IsNullOrEmpty (id) ? CreateUniqueId () : id;

            if (index == -1)
                Controls.Add (control);
            else
                Controls.AddAt (index, control);

            // Returning newly created control back to caller, such that he can set his properties and such for it
            return control;
        }

        /// <summary>
        ///     Removes a control from the control collection, and persist the change
        /// </summary>
        /// <param name="control">Control to remove</param>
        public void RemoveControlPersistent (Control control)
        {
            StoreOriginalControls ();
            Controls.Remove (control);
            ReRenderChildren ();
        }

        /// <summary>
        ///     Removes a control from the control collection, at the given index, and persists the change
        /// </summary>
        /// <param name="index">Index of control to remove</param>
        public void RemoveControlPersistentAt (int index)
        {
            StoreOriginalControls ();
            Controls.RemoveAt (index);
            ReRenderChildren ();
        }

        protected override void LoadViewState (object savedState)
        {
            // Reloading persisted controls, if there are any
            var tmp = savedState as object[];
            if (tmp != null && tmp.Length > 0 && tmp [0] is string[][]) {

                // We're managing our own controls collection, and need to reload from viewstate all the 
                // control types and ids. First figuring out which controls actually exists in this control at the moment
                var ctrlsViewstate = (from idx in (string[][]) tmp [0] select new Tuple<string, string> (idx [0], idx [1])).ToList ();

                // Then removing all controls that is not persisted, and all LiteralControls since they tend to mess up their IDs
                var toRemove = Controls.Cast<Control> ().Where (
                    idxControl => string.IsNullOrEmpty (idxControl.ID) || !ctrlsViewstate.Exists (idxViewstate => idxViewstate.Item2 == idxControl.ID)).ToList ();
                foreach (var idxCtrl in toRemove) {
                    Controls.Remove (idxCtrl);
                }

                // Then adding all controls that are persisted but does not exist in the controls collection
                var controlPosition = 0;
                foreach (var idxTuple in ctrlsViewstate) {
                    var exist = Controls.Cast<Control> ().Any (idxCtrl => idxTuple.Item2 == idxCtrl.ID);
                    if (!exist) {
                        var control = Creators [GetTypeFromId (idxTuple.Item1)].Create ();
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

        protected override object SaveViewState ()
        {
            // Making sure all dynamically added controls are persistent to the control state, if there are any
            if (_originalCollection != null) {

                // Yup, we're managing our own control collection, and need to save to viewstate all of the controls
                // types and ids that exists in our control collection
                var tmp = new object[2];
                tmp [0] = (from Control idx in Controls where !string.IsNullOrEmpty (idx.ID) select new[] {GetTypeId (idx.GetType ()), idx.ID}).ToArray ();
                tmp [1] = base.SaveViewState ();
                return tmp;
            }

            // Not managing controls
            return base.SaveViewState ();
        }

        protected override void OnInit (EventArgs e)
        {
            // Making sure all the automatically generated LiteralControls are removed, since they mess up their IDs,
            // but not in a normal postback, or initial loading of the page, since we need the formatting they provide
            if ((Page as IAjaxPage).Manager.IsPhosphorusAjaxRequest) {
                var ctrls = Controls.Cast<Control> ().Where (idx => string.IsNullOrEmpty (idx.ID)).ToList ();
                foreach (var idx in ctrls) {
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
                using (var stream = new MemoryStream ()) {
                    using (var txt = new HtmlTextWriter (new StreamWriter (stream))) {
                        idx.RenderControl (txt);
                        txt.Flush ();
                    }
                    stream.Seek (0, SeekOrigin.Begin);
                    using (TextReader reader = new StreamReader (stream)) {
                        html = reader.ReadToEnd ();
                    }
                }
                var position = Controls.IndexOf (idx);
                widgets.Add (new Tuple<string, int> (html, position));
            }

            // We have to insert such that the first controls becomes added before controls behind it, such that the dom position
            // don't become messed up
            widgets.Sort (
                (lhs, rhs) => lhs.Item2.CompareTo (rhs.Item2));

            // Informing our manager that the current widget has changes, if we should
            if (widgets.Count > 0) {
                foreach (var idx in widgets) {
                    (Page as IAjaxPage).Manager.RegisterWidgetChanges (ClientID, "__p5_add_" + idx.Item2, idx.Item1);
                }
            }
        }

        // Renders all controls that was removed, and returns list back to caller
        private void RenderRemovedControls ()
        {
            foreach (var idxOriginal in _originalCollection) {
                var exist = Controls.Cast<Control> ().Any (idxActual => idxActual == idxOriginal);
                if (!exist && !string.IsNullOrEmpty (idxOriginal.ID))
                    (Page as IAjaxPage).Manager.RegisterDeletedWidget (idxOriginal.ClientID);
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
            if (!Creators.ContainsKey (typeof (T))) {
                lock (Lock) {
                    if (!Creators.ContainsKey (typeof (T)))
                        Creators [typeof (T)] = new Creator<T> ();
                }
            }
            return Creators [typeof (T)];
        }

        // Used to "pack" the types stored in the ViewState to make viewstate as small as possible
        private static string GetTypeId (Type type)
        {
            foreach (var idx in TypeMapper) {
                if (idx.Item2 == type)
                    return idx.Item1;
            }

            // Didn't exist, need to create it
            lock (Lock) {
                if (!TypeMapper.Exists (
                    idx => idx.Item2 == type))
                    TypeMapper.Add (new Tuple<string, Type> (TypeMapper.Count.ToString (), type));
            }

            // Recursively calling self for simplicity
            return GetTypeId (type);
        }

        // Used to retrieve the type from the type mapper collection
        private static Type GetTypeFromId (string id)
        {
            return TypeMapper.Find (idx => idx.Item1 == id).Item2;
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
