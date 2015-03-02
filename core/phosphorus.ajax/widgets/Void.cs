/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Web.UI;

namespace phosphorus.ajax.widgets
{
    /// <summary>
    ///     a widget that has no children and no content and is self closed
    /// </summary>
    [ViewStateModeById]
    public class Void : Widget
    {
        public Void () { RenderType = RenderingType.NoClose; }
        // overridden to throw an exception if user tries to explicitly set the innerValue attribute of this control
        public override string this [string name]
        {
            get { return base [name]; }
            set {
                if (name == "innerValue")
                    throw new ArgumentException ("you cannot set the 'innerValue' property of a Void widget");
                base [name] = value;
            }
        }

        protected override bool HasContent
        {
            get { return false; }
        }

        protected override void AddedControl (Control control, int index) { throw new ArgumentException ("Void widget cannot have children"); }
    }
}