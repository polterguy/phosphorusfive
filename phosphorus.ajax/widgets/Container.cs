/*
 * phosphorus five, copyright 2014 - thomas@magixilluminate.com
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Reflection;
using System.Web.UI;
using System.Collections.Generic;
using phosphorus.types;
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
        private List<Tuple<string, string>> _dynamicControls = new List<Tuple<string, string>> ();
        private bool _isManaging;
        private List<Control> _toRender = new List<Control> ();
        private List<Control> _toRemove = new List<Control> ();
        private bool _loadDone;

        /// <summary>
        /// creates a persistent control that will be automatically re-created during future postbacks
        /// </summary>
        /// <returns>the persistent control</returns>
        /// <param name="id">id of control, if null, and automatic id will be created</param>
        /// <param name="index">index of where to insert control</param>
        /// <typeparam name="T">the type of control you wish to create</typeparam>
        public T CreatePersistentControl<T> (string id, int index = -1)  where T : Control, new()
        {
            _isManaging = true;

            if (RenderingMode != RenderMode.RenderVisible)
                RenderingMode = RenderMode.RenderChildren;

            // creating new control, and adding to the controls collection
            T control = new T ();
            if (string.IsNullOrEmpty (id)) {

                // creating a unique id for widget
                int highest = 0;
                foreach (Control idx in Controls) {
                    int x;
                    if (!string.IsNullOrEmpty(idx.ID) && idx.ID.Length > 1 && int.TryParse (idx.ID.Substring (1), out x) && x > highest)
                        highest = x;
                }
                control.ID = "x" + (highest + 1);
            } else {

                // using the supplied id
                control.ID = id;
            }

            if (index == -1)
                Controls.Add (control);
            else
                Controls.AddAt (index, control);

            _toRender.Add (control);

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
                // control types and ids, to later merge into control collection
                _isManaging = true;
                string[][] ctrls = tmp [0] as string[][];
                foreach (string[] idx in ctrls) {
                    _dynamicControls.Add (new Tuple<string, string> (idx [0], idx [1]));
                }
                base.LoadControlState (tmp [1]);
            } else {
                base.LoadControlState (savedState);
            }
        }

        protected override object SaveControlState ()
        {
            // making sure all persistent controls are persistent to the control state, if there are any
            if (_isManaging) {

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

                // "screw this, I'm going home" ... ;)
                return base.SaveControlState ();
            }
        }
        
        protected override void OnLoad (EventArgs e)
        {
            base.OnLoad (e);

            if (_isManaging) {

                // removing controls that are not persisted in viewstate
                // these are normally controls that exists in markup, or was created 
                // before controls was rooted to page, but later removed by user
                List<Control> toRemove = new List<Control> ();
                foreach (Control idx in Controls) {
                    if (!_dynamicControls.Exists (
                        delegate(Tuple<string, string> obj) {
                        return obj.Item2 == idx.ID;
                    }))
                        toRemove.Add (idx);
                }
                foreach (Control idx in toRemove) {
                    Controls.Remove (idx);
                }

                // re-creating all of our persistent controls that does not exist from before
                // ps, viewstate will reload all properties as long as we get the type and id right
                // order will be automatically taken care of since we're persisting them into viewstate
                // in their existing when viewstate is saved
                int idxNo = 0;
                foreach (var idx in _dynamicControls) {
                    bool exist = false;
                    foreach (Control idxC in Controls) {
                        if (idxC.ID == idx.Item2) {
                            exist = true;
                            break;
                        }
                    }
                    if (!exist) {
                        Type type = Type.GetType (idx.Item1);
                        ConstructorInfo ctor = type.GetConstructor (new Type[] { });
                        Control ctr = ctor.Invoke (new object[] { }) as Control;
                        ctr.ID = idx.Item2;
                        Controls.AddAt (idxNo, ctr);
                    }
                    idxNo += 1;
                }
            }
            _loadDone = true;
        }

        protected override void RemovedControl (Control control)
        {
            // automatically changing the rendering mode of the widget if we should
            if (IsTrackingViewState) {
                _isManaging = true;
                if (_loadDone)
                    _toRemove.Add (control);
                if (RenderingMode == RenderMode.Default) {
                    RenderingMode = RenderMode.RenderChildren;
                }
            }
            base.RemovedControl (control);
        }

        protected override void AddedControl (Control control, int index)
        {
            // automatically changing the rendering mode of the widget if we should
            if (IsTrackingViewState && RenderingMode == RenderMode.Default)
                RenderingMode = RenderMode.RenderChildren;
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

        protected override void RenderChildrenWidgetsAsJson ()
        {
            // rendering all widgets that was added this request, and returning back as html, and their position in dom
            var widgets = new List<Tuple<string, int>> ();
            foreach (Control idx in _toRender) {

                // checking if control is still around
                if (!Controls.Contains (idx))
                    continue;

                // getting child html
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

                // finding position in dom
                int position = Controls.IndexOf (idx);
                widgets.Add (new Tuple<string, int> (html, position));
            }

            // sorting by first at the top
            widgets.Sort (
                delegate(Tuple<string, int> lhs, Tuple<string, int> rhs) {
                return lhs.Item2.CompareTo (rhs.Item2);
            });

            if (widgets.Count > 0) {
                // registering json changes
                Node tmp = new Node (ClientID);
                foreach (var idx in widgets) {
                    tmp ["__pf_add_" + idx.Item2].Value = new Node (null, idx.Item1);
                }
                (Page as core.IAjaxPage).Manager.RegisterWidgetChanges (tmp);
            }

            // then rendering all widgets that should be removed
            var toRemove = new List<string> ();
            foreach (Control idx in _toRemove) {
                // finding position in dom
                toRemove.Add (idx.ClientID);
            }

            if (toRemove.Count > 0) {
                // registering json changes
                Node tmp = new Node (ClientID);
                foreach (var idx in toRemove) {
                    tmp ["__pf_remove"].Value = new Node (null, idx);
                }
                (Page as core.IAjaxPage).Manager.RegisterWidgetChanges (tmp);
            }
        }
    }
}

