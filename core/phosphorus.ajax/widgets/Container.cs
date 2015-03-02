/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI;
using phosphorus.ajax.core;

// ReSharper disable PossibleNullReferenceException

namespace phosphorus.ajax.widgets
{
    /// <summary>
    ///     a widget that contains children widgets. everything between the opening and end declaration of this widget
    ///     in your .aspx markup will be treated as controls. you can also dynamically add child controls to this widget
    ///     by using the CreatePersistentControl method
    /// </summary>
    [ViewStateModeById]
    public class Container : Widget, INamingContainer
    {
        // contains all the creator objects to create our controls when needed
        // the whole purpose of this bugger is to avoid the use of reflection as much as possible, since it is slow and 
        // requires lowering security settings on server to be used
        // by storing "factory objects" like this in a dictionary with the type being the key, we avoid 
        // having to use anymore reflection than absolutely necessary
        // please notice that this dictionary is static, and hence will be reused across multiple requests and sessions
        private static readonly Dictionary<Type, ICreator> Creators = new Dictionary<Type, ICreator> ();
        private static readonly List<Tuple<string, Type>> TypeMapper = new List<Tuple<string, Type>> ();
        // used to lock GetCreator to make sure we don't get a race condition when instantiating new creators
        private static readonly object Lock = new object ();
        // contains the original controls collection, before we started adding and removing controls for current request
        private List<Control> _originalCollection;
        // overridden to supply default element
        public override string ElementType
        {
            get
            {
                if (string.IsNullOrEmpty (base.ElementType))
                    return "div";
                return base.ElementType;
            }
            set { base.ElementType = value; }
        }

        public override string this [string name]
        {
            get {
                if (name == "value" && ElementType == "select") {
                    // special treatment for select HTML elements "value" property, since they might still have a value, 
                    // even though their "value" property returns null, since one of their children, "option" elements, 
                    // still might contain a "selected" property
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
                base [name] = value;
            }
        }

        public override bool HasAttribute (string name)
        {
            if (name == "value" && ElementType == "select") {
                // special treatment for select HTML elements "value" property, since they might still have a value, even though
                // their "value" property returns null, since one of their children, "option" elements, still might contain a "selected" property
                foreach (Control idxCtrl in Controls) {
                    var idxWidget = idxCtrl as Widget;
                    if (idxWidget != null) {
                        if (idxWidget.HasAttribute ("selected") && idxWidget.HasAttribute ("value"))
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
        ///     returns all controls of the given type T from the Controls collection
        /// </summary>
        /// <returns>the controls</returns>
        /// <typeparam name="T">type of controls to return</typeparam>
        public IEnumerable<T> GetChildControls<T> () where T : Control { return from Control idx in Controls let tmp = idx as T where idx != null select tmp; }

        /// <summary>
        ///     creates a persistent control that will be automatically re-created during future postbacks. you can create any
        ///     Control
        ///     here you wish, but your control must have a public constructor taking no arguments. only controls created through
        ///     this
        ///     method will be persisted and automatically re-created in future http requests
        /// </summary>
        /// <returns>the persistent control</returns>
        /// <param name="id">id of control, if null, and automatic id will be created</param>
        /// <param name="index">index of where to insert control</param>
        /// <param name="onLoad">event handler callback for what to do during OnLoad</param>
        /// <typeparam name="T">the type of control you wish to create</typeparam>
        public T CreatePersistentControl<T> (string id = null, int index = -1, EventHandler onLoad = null) where T : Control, new ()
        {
            StoreOriginalControls ();
            ReRenderChildren ();

            // creating new control, and adding to the controls collection
            var control = GetCreator<T> ().Create () as T;
            control.ID = string.IsNullOrEmpty (id) ? CreateId () : id;

            if (index == -1)
                Controls.Add (control);
            else
                Controls.AddAt (index, control);

            if (onLoad != null) {
                control.Page.LoadComplete += delegate {
                    onLoad (control, new EventArgs ());
                };
            }

            // returning newly created control back to caller, such that he can set his properties and such for it
            return control;
        }

        /// <summary>
        ///     removes a control from the control collection, and persist the change
        /// </summary>
        /// <param name="control">control to remove</param>
        public void RemoveControlPersistent (Control control)
        {
            StoreOriginalControls ();
            Controls.Remove (control);
            ReRenderChildren ();
        }

        /// <summary>
        ///     removes a control from the control collection, and persist the change
        /// </summary>
        /// <param name="index">index of control to remove</param>
        public void RemoveControlPersistentAt (int index)
        {
            StoreOriginalControls ();
            Controls.RemoveAt (index);
            ReRenderChildren ();
        }

        protected override void LoadViewState (object savedState)
        {
            // reloading persisted controls, if there are any
            var tmp = savedState as object[];
            if (tmp != null && tmp.Length > 0 && tmp [0] is string[][]) {
                // we're managing our own controls collection, and need to reload from viewstate all the 
                // control types and ids. first figuring out which controls actually exists in this control at the moment
                var ctrlsViewstate = (from idx in (string[][]) tmp [0] select new Tuple<string, string> (idx [0], idx [1])).ToList ();

                // then removing all controls that is not persisted, and all LiteralControls since they tend to mess up their IDs
                var toRemove = Controls.Cast<Control> ().Where (
                    idxControl => string.IsNullOrEmpty (idxControl.ID) || !ctrlsViewstate.Exists (idxViewstate => idxViewstate.Item2 == idxControl.ID)).ToList ();
                foreach (var idxCtrl in toRemove) {
                    Controls.Remove (idxCtrl);
                }

                // then adding all controls that are persisted but does not exist in the controls collection
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
            // making sure all dynamically added controls are persistent to the control state, if there are any
            if (_originalCollection != null) {
                // yup, we're managing our own control collection, and need to save to viewstate all of the controls
                // types and ids that exists in our control collection
                var tmp = new object[2];
                tmp [0] = (from Control idx in Controls where !string.IsNullOrEmpty (idx.ID) select new[] {GetTypeId (idx.GetType ()), idx.ID}).ToArray ();
                tmp [1] = base.SaveViewState ();
                return tmp;
            }
            // not managing controls
            return base.SaveViewState ();
        }

        protected override void OnInit (EventArgs e)
        {
            // making sure all the automatically generated LiteralControls are removed, since they mess up their IDs,
            // but not in a normal postback, or initial loading of the page, since we need the formatting they provide
            if ((Page as IAjaxPage).Manager.IsPhosphorusRequest) {
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
                RenderAddedControls ();
                RenderRemovedControls ();
                RenderOldControls (writer);
            }
        }

        /*
         * creates a new unique ID
         */

        private string CreateId ()
        {
            // TODO: statistically this is supposed to become a unique 7 digits hexadecimal number, but we should improve this logic later!
            var retVal = Guid.NewGuid ().ToString ().Replace ("-", "");
            retVal = "x" + retVal [0] + retVal [5] + retVal [10] + retVal [15] + retVal [20] + retVal [25] + retVal [30];
            return retVal;
        }

        // renders all controls that was added this request, and return list back to caller
        private void RenderAddedControls ()
        {
            var widgets = new List<Tuple<string, int>> ();
            foreach (Control idx in Controls) {
                if (_originalCollection.Contains (idx) || string.IsNullOrEmpty (idx.ID))
                    continue; // control has already been rendered, or is a literal control without an ID

                // getting control's html
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

            // we have to insert such that the first controls becomes added before controls behind it, such that the dom position
            // don't become messed up
            widgets.Sort (
                (lhs, rhs) => lhs.Item2.CompareTo (rhs.Item2));

            // informing our manager that the current widget has changes, if we should
            if (widgets.Count > 0) {
                foreach (var idx in widgets) {
                    (Page as IAjaxPage).Manager.RegisterWidgetChanges (ClientID, "__pf_add_" + idx.Item2, idx.Item1);
                }
            }
        }

        // renders all controls that was removed, and returns list back to caller
        private void RenderRemovedControls ()
        {
            foreach (var idxOriginal in _originalCollection) {
                var exist = Controls.Cast<Control> ().Any (idxActual => idxActual.ID == idxOriginal.ID);
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

        // storing original controls that were there before we started adding and removing controls
        private void StoreOriginalControls ()
        {
            if (_originalCollection == null) {
                _originalCollection = new List<Control> ();
                foreach (Control idxCtrl in Controls) {
                    _originalCollection.Add (idxCtrl);
                }
            }
        }

        // use to make sure we store a reference to our creator instance for later requests
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

        // used to "pack" the types stored in the ViewState to make viewstate as small as possible
        private static string GetTypeId (Type type)
        {
            foreach (var idx in TypeMapper) {
                if (idx.Item2 == type)
                    return idx.Item1;
            }

            // didn't exist, need to create it
            lock (Lock) {
                if (!TypeMapper.Exists (
                    idx => idx.Item2 == type))
                    TypeMapper.Add (new Tuple<string, Type> (TypeMapper.Count.ToString (), type));
            }
            return GetTypeId (type);
        }

        // used to retrieve the type from the type mapper collection
        private static Type GetTypeFromId (string id) { return TypeMapper.Find (idx => idx.Item1 == id).Item2; }
        // interface to create controls to avoid reflection as much as possible
        private interface ICreator
        {
            Control Create ();
        }

        // some "anti-reflection magic"
        private class Creator<T> : ICreator where T : Control, new ()
        {
            public Control Create () { return new T (); }
        }
    }
}