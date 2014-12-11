/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Reflection;
using System.Web.UI;
using System.Collections.Generic;
using core = phosphorus.ajax.core;

namespace phosphorus.ajax.widgets
{
    /// <summary>
    /// a widget that contains children widgets. everything between the opening and end declaration of this widget 
    /// in your .aspx markup will be treated as controls. you can also dynamically add child controls to this widget
    /// by using the <see cref="phosphorus.ajax.widgets.Container.CreatePersistentControl"/> method
    /// </summary>
    public class Container : Widget, INamingContainer
    {
        // interface to create controls to avoid reflection as much as possible
        private interface ICreator
        {
            Control Create ();
        }

        // some "anti-reflection magic"
        private class Creator<T> : ICreator where T : Control, new()
        {
            public Control Create() {
                return new T ();
            }
        }

        // contains all the creator objects to create our controls when needed
        // the whole purpose of this bugger is to avoid the use of reflection as much as possible, since it is slow and 
        // requires lowering security settings on server to be used
        // by storing "factory objects" like this in a dictionary with the type being the key, we avoid 
        // having to use anymore reflection than absolutely necessary
        // please notice that this dictionary is static, and hence will be reused across multiple requests and sessions
        private static Dictionary<Type, ICreator> _creators = new Dictionary<Type, ICreator>();

        private static List<Tuple<string, Type>> _typeMapper = new List<Tuple<string, Type>> ();

        // contains the original controls collection, before we started adding and removing controls for current request
        private List<Control> _originalCollection;

        // used to lock GetCreator to make sure we don't get a race condition when instantiating new creators
        private static object _lock = new object ();

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.ajax.widgets.Container"/> class
        /// </summary>
        public Container ()
        { }

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.ajax.widgets.Container"/> class
        /// </summary>
        /// <param name="elementType">html element to render widget with</param>
        public Container (string elementType)
            : base (elementType)
        { }

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.ajax.widgets.Container"/> class
        /// </summary>
        /// <param name="elementType">html element to render widget with</param>
        /// <param name="renderType">how to render the widget</param>
        public Container (string elementType, Widget.RenderingType renderType)
            : base (elementType, renderType)
        { }

        // overridden to supply default element
        public override string ElementType {
            get {
                if (string.IsNullOrEmpty (base.ElementType))
                    return "div";
                return base.ElementType;
            }
            set {
                base.ElementType = value;
            }
        }

        /// <summary>
        /// returns all controls of the given type T from the Controls collection
        /// </summary>
        /// <returns>the controls</returns>
        /// <typeparam name="T">type of controls to return</typeparam>
        public IEnumerable<T> GetChildControls<T> () where T : Control
        {
            foreach (Control idx in Controls) {
                T tmp = idx as T;
                if (idx != null)
                    yield return tmp;
            }
        }

        /// <summary>
        /// returns all descendant controls of type T
        /// </summary>
        /// <returns>all controls of type T that are children, or children of childre, etc of this control</returns>
        /// <typeparam name="T">the type of controls to return</typeparam>
        public IEnumerable<T> GetDescendantControls<T> () where T : Control
        {
            return GetDescendantControlsImplementation<T> (this);
        }

        /// <summary>
        /// creates a persistent control that will be automatically re-created during future postbacks. you can create any Control 
        /// here you wish, but your control must have a public constructor taking no arguments. only controls created through this 
        /// method will be persisted and automatically re-created in future http requests
        /// </summary>
        /// <returns>the persistent control</returns>
        /// <param name="id">id of control, if null, and automatic id will be created</param>
        /// <param name="index">index of where to insert control</param>
        /// <typeparam name="T">the type of control you wish to create</typeparam>
        public T CreatePersistentControl<T> (string id = null, int index = -1)  where T : Control, new()
        {
            StoreOriginalControls ();
            ReRenderChildren ();

            // creating new control, and adding to the controls collection
            T control = GetCreator<T>().Create () as T;
            if (string.IsNullOrEmpty (id)) {

                // creating an automatic unique id for widget
                control.ID = "x" + Guid.NewGuid ().ToString ().Replace ("-", "");
            } else {

                // using the supplied id
                control.ID = id;
            }

            if (index == -1)
                Controls.Add (control);
            else
                Controls.AddAt (index, control);

            // returning newly created control back to caller, such that he can set his properties and such for it
            return control;
        }

        /// <summary>
        /// removes a control from the control collection, and persist the change
        /// </summary>
        /// <param name="control">control to remove</param>
        public void RemoveControlPersistent (Control control)
        {
            StoreOriginalControls ();
            Controls.Remove (control);
            ReRenderChildren ();
        }

        /// <summary>
        /// removes a control from the control collection, and persist the change
        /// </summary>
        /// <param name="index">index of control to remove</param>
        public void RemoveControlPersistentAt (int index)
        {
            StoreOriginalControls ();
            Controls.RemoveAt (index);
            ReRenderChildren ();
        }

        // overridden to throw an exception if user tries to explicitly set the innerHTML attribute of this control
        public override string this [string name] {
            get { return base [name]; }
            set {
                if (name == "innerHTML")
                    throw new ArgumentException ("you cannot set the 'innerHTML' property of the '" + ID + "' Container widget");
                base [name] = value;
            }
        }

        protected override bool HasContent {
            get { return Controls.Count > 0; }
        }

        protected override void LoadControlState (object savedState)
        {
            // reloading persisted controls, if there are any
            object[] tmp = savedState as object[];
            if (tmp != null && tmp.Length > 0 && tmp [0] is string[][]) {

                // we're managing our own controls collection, and need to reload from viewstate all the 
                // control types and ids. first figuring out which controls actually exists in this control at the moment
                var ctrlsViewstate = new List<Tuple<string, string>> ();
                foreach (string[] idx in tmp [0] as string[][]) {
                    ctrlsViewstate.Add (new Tuple<string, string> (idx [0], idx [1]));
                }

                // then removing all controls that is not persisted, and all LiteralControls since they tend to mess up their IDs
                var toRemove = new List<Control> ();
                foreach (Control idxControl in Controls) {
                    if (string.IsNullOrEmpty (idxControl.ID) || !ctrlsViewstate.Exists (
                        delegate (Tuple<string, string> idxViewstate) {
                        return idxViewstate.Item2 == idxControl.ID;
                    }))
                        toRemove.Add (idxControl);
                }
                foreach (Control idxCtrl in toRemove) {
                    Controls.Remove (idxCtrl);
                }

                // then adding all controls that are persisted but does not exist in the controls collection
                int controlPosition = 0;
                foreach (var idxTuple in ctrlsViewstate) {
                    bool exist = false;
                    foreach (Control idxCtrl in Controls) {
                        if (idxTuple.Item2 == idxCtrl.ID) {
                            exist = true;
                            break;
                        }
                    }
                    if (!exist) {
                        Control control = _creators [GetTypeFromID (idxTuple.Item1)].Create ();
                        control.ID = idxTuple.Item2;
                        Controls.AddAt (controlPosition, control);
                    }
                    controlPosition += 1;
                }

                StoreOriginalControls ();

                base.LoadControlState (tmp [1]);
            } else {
                base.LoadControlState (savedState);
            }
        }

        protected override object SaveControlState ()
        {
            // making sure all dynamically added controls are persistent to the control state, if there are any
            if (_originalCollection != null) {

                // yup, we're managing our own control collection, and need to save to viewstate all of the controls
                // types and ids that exists in our control collection
                var lst = new List<string []> ();
                foreach (Control idx in Controls) {
                    if (!string.IsNullOrEmpty (idx.ID)) // skipping auto-generated literal controls
                        lst.Add (new string[] { GetTypeID ( idx.GetType ()), idx.ID });
                }
                object[] tmp = new object [2];
                tmp [0] = lst.ToArray ();
                tmp [1] = base.SaveControlState ();
                return tmp;
            } else {

                // not managing controls
                return base.SaveControlState ();
            }
        }

        protected override void OnInit (EventArgs e)
        {
            // making sure all the automatically generated LiteralControls are removed, since they mess up their IDs,
            // but not in a normal postback, or initial loading of the page, since we need the formatting they provide
            if ((Page as core.IAjaxPage).Manager.IsPhosphorusRequest) {
                List<Control> ctrls = new List<Control> ();
                foreach (Control idx in Controls) {
                    if (string.IsNullOrEmpty (idx.ID))
                        ctrls.Add (idx);
                }
                foreach (Control idx in ctrls) {
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

        // renders all controls that was added this request, and return list back to caller
        private void RenderAddedControls ()
        {
            var widgets = new List<Tuple<string, int>> ();
            foreach (Control idx in Controls) {
                if (_originalCollection.Contains (idx) || string.IsNullOrEmpty (idx.ID))
                    continue; // control has already been rendered, or is a literal control without an ID

                // getting control's html
                string html;
                using (MemoryStream stream = new MemoryStream ()) {
                    using (HtmlTextWriter txt = new HtmlTextWriter (new StreamWriter (stream))) {
                        idx.RenderControl (txt);
                        txt.Flush ();
                    }
                    stream.Seek (0, SeekOrigin.Begin);
                    using (TextReader reader = new StreamReader (stream)) {
                        html = reader.ReadToEnd ();
                    }
                }
                int position = Controls.IndexOf (idx);
                widgets.Add (new Tuple<string, int> (html, position + 1));
            }

            // we have to insert such that the first controls becomes added before controls behind it, such that the dom position
            // don't become messed up
            widgets.Sort (
                delegate(Tuple<string, int> lhs, Tuple<string, int> rhs) {
                return lhs.Item2.CompareTo (rhs.Item2);
            });

            // informing our manager that the current widget has changes, if we should
            if (widgets.Count > 0) {
                foreach (var idx in widgets) {
                    (Page as core.IAjaxPage).Manager.RegisterWidgetChanges (ClientID, "__pf_add_" + idx.Item2, idx.Item1);
                }
            }
        }

        // renders all controls that was removed, and returns list back to caller
        private void RenderRemovedControls ()
        {
            foreach (Control idxOriginal in _originalCollection) {
                bool exist = false;
                foreach (Control idxActual in Controls) {
                    if (idxActual.ID == idxOriginal.ID) {
                        exist = true;
                        break;
                    }
                }
                if (!exist && !string.IsNullOrEmpty (idxOriginal.ID))
                    (Page as core.IAjaxPage).Manager.RegisterDeletedWidget (idxOriginal.ClientID);
            }
        }
        
        private void RenderOldControls (HtmlTextWriter writer)
        {
            RenderingMode old = _renderMode;
            _renderMode = RenderingMode.Default;
            foreach (Control idx in Controls) {
                if (_originalCollection.Contains (idx)) {
                    idx.RenderControl (writer);
                }
            }
            _renderMode = old;
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
        private static ICreator GetCreator<T>() where T : Control, new()
        {
            if (!_creators.ContainsKey (typeof (T))) {
                lock (_lock) {
                    if (!_creators.ContainsKey (typeof(T)))
                        _creators [typeof(T)] = new Creator<T> ();
                }
            }
            return _creators [typeof(T)];
        }

        // used to "pack" the types stored in the ControlState to make viewstate as small as possible
        private static string GetTypeID (Type type)
        {
            foreach (var idx in _typeMapper) {
                if (idx.Item2 == type)
                    return idx.Item1;
            }

            // didn't exist, need to create it
            lock (_lock) {
                if (!_typeMapper.Exists (
                    delegate(Tuple<string, Type> idx) {
                    return idx.Item2 == type;
                }))
                    _typeMapper.Add (new Tuple<string, Type> (_typeMapper.Count.ToString (), type));
            }
            return GetTypeID (type);
        }

        // used to retrieve the type from the type mapper collection
        private static Type GetTypeFromID (string id)
        {
            return _typeMapper.Find (
                delegate(Tuple<string, Type> idx) {
                return idx.Item1 == id;
            }).Item2;
        }

        // used to retrieve all descendant controls of type T
        private static IEnumerable<T> GetDescendantControlsImplementation<T> (Control from) where T : Control
        {
            foreach (Control idx in from.Controls) {
                T tmp = idx as T;
                if (tmp != null) {
                    yield return tmp;
                }
                var tmpCollection = GetDescendantControlsImplementation<T> (idx);
                foreach (var idxChild in tmpCollection) {
                    yield return idxChild;
                }
            }
        }
    }
}

