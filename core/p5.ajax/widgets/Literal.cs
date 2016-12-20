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
    ///     Allows you to create a widget that contains simple text or HTML.
    /// 
    ///     Useful for most widgets, where you don't care about de-referencing its content as widgets.
    /// </summary>
    [ParseChildren (true, "innerValue")]
    [PersistChildren (false)]
    [ViewStateModeById]
    public class Literal : Widget
    {
        /*
         * Overridden to make sure the default element type for this widget is "p".
         */
        public override string Element {
            get { return string.IsNullOrEmpty (base.Element) ? "p" : base.Element; }
            set { base.Element = value == "p" ? null : value; }
        }

        /// <summary>
        ///     Gets or sets the innerValue property of the widget.
        /// 
        ///     This is the HTML or text content of the widget.
        /// </summary>
        /// <value>The innerHTML equivalent from JavaScript</value>
        [PersistenceMode (PersistenceMode.InnerDefaultProperty)]
        public string innerValue
        {
            get { return base.GetAttributeValue ("innerValue"); }
            set { base.SetAttributeValue ("innerValue", value); }
        }

        /*
         * Overridden to provide some help for retrieving value of textarea and option elements.
         */
        public override string GetAttributeValue (string name)
        {
            if (Element == "textarea" && name == "value") {

                // Special treatment for textarea, to make it resemble what goes on on the client-side.
                return base.GetAttributeValue ("innerValue");

            } else if (Element == "option" && name == "value" && !HasAttribute ("value")) {

                // By default, "option" HTML elements returns their "innerValue" if they have no value attribute.
                return innerValue;

            } else {

                // No need for special treatment, letting base do the heavy lifting.
                return base.GetAttributeValue (name);
            }
        }

        /*
         * Overridden to provide some help for setting value of textarea and option elements.
         */
        public override void SetAttributeValue (string name, string value)
        {
            if (Element == "textarea" && name == "value") {

                // Special treatment for textarea, to make it resemble what goes on on the client-side.
                base.SetAttributeValue ("innerValue", value);

            } else if (Element == "option" && name == "selected") {

                // Sanity check
                if (value != null && value != "selected")
                    throw new ArgumentException ("You cannot set the selected attribute of an option element to anything but null or 'selected'.");

                // Returning early if widget already has the attribute, to avoid re-rendering, when selected property is set to what it was before.
                if (HasAttribute ("selected") && GetAttributeValue ("selected") == value)
                    return;

                // Since there can only be one "selected" widget inside a "select" HTML element, we remove the "selected" attribute on all widgets
                // in parent Controls collection, before we add the "selected" attribute for this "option" HTML element.
                foreach (Widget idxWidget in Parent.Controls) {
                    idxWidget.DeleteAttribute ("selected");
                }
                base.SetAttributeValue (name, value);

                // Due to a "bug" in the way browsers handles the "selected" property on "option" elements, we need to re-render all
                // select widgets, every time one of its "option" elements' "selected" attribute is changed.
                // Read more here; https://bugs.chromium.org/p/chromium/issues/detail?id=662669
                (Parent as Widget).ReRender ();

            } else {

                // No need for special treatment.
                base.SetAttributeValue (name, value);
            }
        }

        /*
         * Overridden to make sure "textarea" logic resembles what happens on client side.
         */
        public override bool HasAttribute (string name)
        {
            if (name == "value" && Element == "textarea") {

                // Special treatment for textarea, to make it resemble what goes on on the client-side.
                return HasAttribute ("innerValue");
            }
            return base.HasAttribute (name);
        }

        /*
         * Notice how we do not call base here, and only render its innerValue, and none of its children.
         */
        protected override void RenderChildren (HtmlTextWriter writer)
        {
            writer.Write (innerValue);
        }

        /*
         * Overridden to make sure user doesn't blow off his feet ...
         */
        protected override void AddedControl (Control control, int index)
        {
            throw new ApplicationException ("Literal widget cannot have children controls");
        }

        /*
         * Implementation of abstract base class property.
         */
        protected override bool HasContent
        {
            get { return !string.IsNullOrEmpty (innerValue); }
        }

        /// <summary>
        ///     Verifies element is legal to use for this widget type.
        /// </summary>
        /// <param name="elementName">Element name.</param>
        protected override void SanitizeElementName (string elementName)
        {
            // Letting base do its magic.
            base.SanitizeElementName (elementName);

            // Making sure element name is legal for this widget.
            switch (elementName) {
                // Although technically the select element could be used for a Literal, we prevent it, to avoid messing up FORM data loading, 
                // and setting/getting of its value property, which is done by iterating its children widgets, looking for a "selected" attribute.
                case "select":
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
                    throw new ArgumentException ("You cannot use this Element for the Literal widget", nameof (Element));
            }
        }
    }
}
