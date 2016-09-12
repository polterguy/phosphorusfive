/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Web.UI;

namespace p5.ajax.widgets
{
    /// <summary>
    ///     A widget that has neither children controls, nor content
    /// </summary>
    [ViewStateModeById]
    public class Void : Widget
    {
        public Void ()
        {
            RenderType = RenderingType.open;
        }

        // Overridden to throw an exception if user tries to explicitly set the innerValue attribute of this control
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

        protected override void AddedControl (Control control, int index)
        {
            throw new ArgumentException ("Void widget cannot have children");
        }
    }
}