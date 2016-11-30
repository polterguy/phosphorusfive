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
    ///     Allows you to create a widget that contains simple text or HTML
    /// </summary>
    [ParseChildren (true, "innerValue")]
    [PersistChildren (false)]
    [ViewStateModeById]
    public class Literal : Widget
    {
        // Overridden to supply default html element
        public override string Element
        {
            get
            {
                if (string.IsNullOrEmpty (base.Element))
                    return "p";
                return base.Element;
            }
            set { base.Element = value; }
        }

        /// <summary>
        ///     Gets or sets the innerValue property of the widget
        /// </summary>
        /// <value>The innerHTML equivalent from JavaScript</value>
        [PersistenceMode (PersistenceMode.InnerDefaultProperty)]
        public string innerValue
        {
            get { return this ["innerValue"]; }
            set { this ["innerValue"] = value; }
        }
        
        public override string this [string name]
        {
            get {
                if (name == "value" && Element == "textarea") {

                    // Special treatment for textarea, to make it resemble what goes on on the client-side
                    return base ["innerValue"];
                }
                if (name == "value" && Element == "option" && !base.HasAttribute ("value"))

                    // By default, option HTML elements returns their "innerValue" if they have no value attribute
                    return this ["innerValue"];
                return base [name];
            }
            set {
                if (name == "value" && Element == "textarea") {

                    // Special treatment for textarea, to make it resemble what goes on on the client-side
                    base ["innerValue"] = value;
                } else {
                    base [name] = value;
                }
            }
        }

        public override bool HasAttribute (string name)
        {
            if (name == "value" && Element == "textarea") {

                // Special treatment for textarea, to make it resemble what goes on on the client-side
                return HasAttribute ("innerValue");
            }
            return base.HasAttribute (name);
        }

        // Notice how we do not call base here, and only render the innerValue and none of its children
        protected override void RenderChildren (HtmlTextWriter writer)
        {
            writer.Write (innerValue);
        }

        protected override void AddedControl (Control control, int index)
        {
            throw new ApplicationException ("Literal widget cannot have children controls");
        }

        protected override bool HasContent
        {
            get { return !string.IsNullOrEmpty (innerValue); }
        }
    }
}
