/*
 * phosphorus five, copyright 2014 - thomas@magixilluminate.com
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Web.UI;

namespace phosphorus.ajax.widgets
{
    /// <summary>
    /// a widget that has no children and no content and is self closed
    /// </summary>
    public class Void : Widget
    {
        public Void ()
        {
            SelfClosed = true;
            HasEndTag = false;
        }

        // overridden to throw an exception if user tries to explicitly set the innerHTML attribute of this control
        public override void SetAttribute (string key, string value)
        {
            if (key == "innerHTML")
                throw new ArgumentException ("you cannot set the innerHTML property of a Void widget", key);
            base.SetAttribute (key, value);
        }

        protected override void AddedControl (Control control, int index)
        {
            throw new ArgumentException ("Void widget cannot have children controls");
        }
        
        protected override bool HasContent {
            get { return false; }
        }
    }
}

