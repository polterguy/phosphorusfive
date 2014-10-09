/*
 * phosphorus five, copyright 2014 - thomas@magixilluminate.com
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Reflection;
using System.Web.UI;
using System.Collections.Generic;

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

        /// <summary>
        /// creates a persistent control that will be automatically re-created during future postbacks
        /// </summary>
        /// <returns>the persistent control</returns>
        /// <param name="id">id of control, if null, and automatic id will be created</param>
        /// <typeparam name="T">the type of control you wish to create</typeparam>
        public T CreatePersistentControl<T> (string id)  where T : Control, new()
        {
            if (RenderingMode != RenderMode.RenderVisible)
                RenderingMode = RenderMode.RenderChildren;
            T control = new T ();

            if (string.IsNullOrEmpty (id)) {

                // creating a unique id for widget
                int highest = 0;
                foreach (Control idx in Controls) {
                    int x;
                    if (int.TryParse (idx.ID.Substring (1), out x) && x > highest)
                        highest = x;
                }
                control.ID = "x" + (highest + 1);
            } else {

                // using the supplied id
                control.ID = id;
            }

            // adding control to controls collection
            Controls.Add (control);

            // storing control such that we can reload it automatically next postback
            _dynamicControls.Add (new Tuple<string, string> (control.GetType ().FullName, control.ID));

            // returning newly created control back to caller, such that he can set his properties and such for it
            return control;
        }

        // overridden to throw an exception if user tries to explicitly set the innerHTML attribute of this control
        public override string this [string name] {
            get { return base [name]; }
            set {
                if (name == "innerHTML")
                    throw new ArgumentException ("you cannot set the 'innerHTML' property of a Void widget");
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
            if (_dynamicControls.Count > 0) {
                var lst = new List<string []> ();
                foreach (var idxTuple in _dynamicControls) {
                    lst.Add (new string[] { idxTuple.Item1, idxTuple.Item2 });
                }
                object[] tmp = new object [2];
                tmp [0] = lst.ToArray ();
                tmp [1] = base.SaveControlState ();
                return tmp;
            } else {
                return base.SaveControlState ();
            }
        }
        
        protected override void OnLoad (EventArgs e)
        {
            base.OnLoad (e);

            // re-creating all of our persistent controls
            // ps, viewstate will reload all properties as long as we get the type and id right
            foreach (var idx in _dynamicControls) {
                Type type = Type.GetType (idx.Item1);
                ConstructorInfo ctor = type.GetConstructor (new Type[] { });
                Control ctr = ctor.Invoke (new object[] { }) as Control;
                ctr.ID = idx.Item2;
                Controls.Add (ctr);
            }
        }

        protected override void RemovedControl (Control control)
        {
            bool isRemoved = false;
            foreach (var idx in _dynamicControls) {
                if (idx.Item2 == control.ID) {
                    _dynamicControls.Remove (idx);
                    isRemoved = true;
                    break;
                }
            }
            RenderingMode = RenderMode.RenderChildren;
            if (!isRemoved) {
                throw new ApplicationException ("you cannot remove controls that was not dynamically added to through the CreatePersistentControl method");
            }
            base.RemovedControl (control);
        }
    }
}

