/*
 * phosphorus five, copyright 2014 - thomas@magixilluminate.com
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

        // contains the original controls collection, before we started adding and removing controls
        private List<Control> _originalCollection;

        // used to lock GetCreator to make sure we don't get a race condition when instantiating new creators
        private static object _lock = new object ();

        // next available id for controls automatically created
        private int _nextId;

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

        /// <summary>
        /// creates a persistent control that will be automatically re-created during future postbacks. you can create any Control 
        /// here you wish, but your control must have a public constructor taking no arguments. only controls created through this 
        /// method will be persisted and automatically re-created in future http requests
        /// </summary>
        /// <returns>the persistent control</returns>
        /// <param name="id">id of control, if null, and automatic id will be created</param>
        /// <param name="index">index of where to insert control</param>
        /// <typeparam name="T">the type of control you wish to create</typeparam>
        public T CreatePersistentControl<T> (string id, int index = -1)  where T : Control, new()
        {
            if (_originalCollection == null) {
                // storing original collection such that we can do a "diff" during rendering
                _originalCollection = new List<Control> ();
                foreach (Control idxCtrl in Controls) {
                    _originalCollection.Add (idxCtrl);
                }
            }

            // creating new control, and adding to the controls collection
            T control = GetCreator<T>().Create () as T;
            if (string.IsNullOrEmpty (id)) {

                // creating an automatic unique id for widget
                control.ID = "x" + (_nextId++);
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

                // then removing all controls that is not persisted
                var toRemove = new List<Control> ();
                foreach (Control idxControl in Controls) {
                    if (!ctrlsViewstate.Exists (
                        delegate (Tuple<string, string> idxViewstate) {
                        return idxViewstate.Item2 == idxControl.ID;
                    }))
                        toRemove.Add (idxControl);
                }
                foreach (Control idxCtrl in toRemove) {
                    Controls.Remove (idxCtrl);
                }

                // then adding all controls that are persisted but does not exist in controls collection
                int controlPosition = 0;
                foreach (var idxTuple in ctrlsViewstate) {
                    bool exist = false;
                    foreach (Control idxCtrl in Controls) {
                        if (idxCtrl.ID == idxTuple.Item2) {
                            exist = true;
                            break;
                        }
                    }
                    if (!exist) {
                        Control control = _creators [Type.GetType (idxTuple.Item1)].Create ();
                        control.ID = idxTuple.Item2;
                        Controls.AddAt (controlPosition, control);
                    }
                    controlPosition += 1;
                }

                // making sure future controls gets unique ids
                _nextId = Controls.Count;

                // then storing the original controls that was there before user starts adding and removing controls
                _originalCollection = new List<Control> ();
                foreach (Control idxCtrl in Controls) {
                    _originalCollection.Add (idxCtrl);
                }

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
                    lst.Add (new string[] { idx.GetType ().AssemblyQualifiedName, idx.ID });
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
        
        protected override void RemovedControl (Control control)
        {
            // automatically changing the rendering mode of the widget if we should and tracking which controls are removed
            if (IsTrackingViewState) {
                if (_originalCollection == null) {

                    // storing original controls that were there before we started adding and removing controls
                    _originalCollection = new List<Control> ();
                    foreach (Control idxCtrl in Controls) {
                        _originalCollection.Add (idxCtrl);
                    }

                    // we have to add the removed control too, since that bugger is already out of the control collection
                    _originalCollection.Add (control);
                }
                if (RenderingMode == RenderMode.Default) {
                    RenderingMode = RenderMode.RenderChildren;
                }
            }
            base.RemovedControl (control);
        }

        protected override void AddedControl (Control control, int index)
        {
            // automatically changing the rendering mode of the widget if we should
            if (IsTrackingViewState) {
                if (RenderingMode == RenderMode.Default) {
                    RenderingMode = RenderMode.RenderChildren;
                }
            }
            base.AddedControl (control, index);
        }

        protected override void AddParsedSubObject (object obj)
        {
            // simply skipping these buggers, makes ugly markup, and they're noisy also ...
            // besides, every now and then, some asshole comes around with no id, fucking up the viewstate ...!! :P
            if (obj is System.Web.UI.LiteralControl)
                return;
            base.AddParsedSubObject (obj);
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
                if (_originalCollection.Contains (idx))
                    continue; // control has already been rendered

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
                widgets.Add (new Tuple<string, int> (html, position));
            }

            // we have to insert such that the first controls becomes added before controls behind it, such that the dom position
            // don't gets messy
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
                if (!exist)
                    (Page as core.IAjaxPage).Manager.RegisterDeletedWidget (idxOriginal.ClientID);
            }
        }
        
        private void RenderOldControls (HtmlTextWriter writer)
        {
            RenderMode old = RenderingMode;
            RenderingMode = RenderMode.Default;
            foreach (Control idx in Controls) {
                if (_originalCollection.Contains (idx)) {
                    idx.RenderControl (writer);
                }
            }
            RenderingMode = old;
        }
    }
}

