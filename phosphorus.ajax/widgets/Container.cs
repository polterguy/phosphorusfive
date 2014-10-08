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
    /// in your .aspx markup will be treated as controls
    /// </summary>
    public class Container : Widget, INamingContainer
    {
        private List<Tuple<string, string>> _dynamicControls = new List<Tuple<string, string>> ();

        // overridden to throw an exception if user tries to explicitly set the innerHTML attribute of this control
        public override void SetAttribute (string key, string value)
        {
            if (key == "innerHTML")
                throw new ArgumentException ("you cannot set the innerHTML property of a Container widget", key);
            base.SetAttribute (key, value);
        }
        
        protected override bool HasContent {
            get { return Controls.Count > 0; }
        }

        protected override void LoadControlState (object savedState)
        {
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
            foreach (var idx in _dynamicControls) {
                Type type = Type.GetType (idx.Item1);
                ConstructorInfo ctor = type.GetConstructor (new Type[] { });
                Control ctr = ctor.Invoke (new object[] { }) as Control;
                ctr.ID = idx.Item2;
                Controls.Add (ctr);
            }
        }

        public T CreatePersistentControl<T> (string id)  where T : Control, new()
        {
            _renderMode = RenderMode.RenderVisible;
            T control = new T ();
            if (id == null) {
                control.ID = "x" + Controls.Count;
            } else {
                control.ID = id;
            }
            Controls.Add (control);
            _dynamicControls.Add (new Tuple<string, string> (control.GetType ().FullName, control.ID));
            return control;
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
            if (!isRemoved) {
                throw new ApplicationException ("you cannot remove controls that was not dynamically added to through the CreatePersistentControl method");
            }
            base.RemovedControl (control);
        }
    }
}

