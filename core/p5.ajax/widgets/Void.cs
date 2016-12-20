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
        /*
         * Overridden to make sure the default element type for this widget is "input".
         */
        public override string Element
        {
            get { return string.IsNullOrEmpty (base.Element) ? "input": base.Element; }
            set { base.Element = value == "input" ? null : value; }
        }

        /*
         * Overridden to throw an exception if user tries to explicitly set the innerValue attribute of this control.
         */
        public override void SetAttributeValue (string name, string value)
        {
            if (name == "innerValue")
                throw new ArgumentException ("you cannot set the 'innerValue' property of a Void widget");
            base.SetAttributeValue (name, value);
        }

        /*
         * Implementation of abstract base class property, which always returns false, sine void widget never has any content.
         */
        protected override bool HasContent
        {
            get { return false; }
        }

        /// <summary>
        ///     Verifies element name is legal to use for the this widget type.
        /// </summary>
        /// <param name="elementName">Element name.</param>
        protected override void SanitizeElementName (string elementName)
        {
            // Letting base do its magic.
            base.SanitizeElementName (elementName);

            // Making sure element name is legal for this widget.
            switch (elementName) {
                case "input":
                case "br":
                case "col":
                case "hr":
                case "link":
                case "meta":
                case "area":
                case "base":
                case "command":
                case "embed":
                case "img":
                case "keygen":
                case "param":
                case "source":
                case "track":
                case "wbr":
                    break; // Legal "void" element.
                default:
                    if (elementName == "textarea")
                        throw new ArgumentException ("You cannot use the Void widget here, use the Literal widget instead", nameof (Element));
                    else
                        throw new ArgumentException ("You cannot use Void widget here, use the Container or the Literal widget instead", nameof (Element));
            }
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