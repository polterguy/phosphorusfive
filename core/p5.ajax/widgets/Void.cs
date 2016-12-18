/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, thomas@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System;
using System.Web.UI;

namespace p5.ajax.widgets
{
    /// <summary>
    ///     A widget that has neither children controls, nor content.
    /// 
    ///     Useful for "input", "hr" and "br" elements, etc.
    /// </summary>
    [ViewStateModeById]
    public class Void : Widget
    {
        public Void ()
        {
            // Setting the default rendering type of widget, which is "open", since it most likely is an input element, br element or hr element.
            RenderType = RenderingType.open;
        }

        /*
         * Overridden to make sure the default element type for this widget is "input".
         */
        public override string Element {
            get {
                if (string.IsNullOrEmpty (base.Element))
                    return "input";
                return base.Element;
            }
            set { base.Element = value == "input" ? null : value; }
        }

        /*
         * Overridden to throw an exception if user tries to explicitly set the innerValue attribute of this control.
         */
        public override string this [string name]
        {
            get { return base [name]; }
            set {
                if (name == "innerValue")
                    throw new ArgumentException ("you cannot set the 'innerValue' property of a Void widget");
                base [name] = value;
            }
        }

        /*
         * Implementation of abstract base class property, which always returns false, sine void widget never has any content.
         */
        protected override bool HasContent
        {
            get { return false; }
        }

        /*
         * Overridden to make sure user doesn't blow off his feet ...
         */
        protected override void AddedControl (Control control, int index)
        {
            throw new ArgumentException ("Void widget cannot have children");
        }
    }
}