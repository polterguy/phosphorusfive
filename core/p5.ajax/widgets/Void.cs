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
using System.Linq;
using System.Web.UI;

namespace p5.ajax.widgets
{
    /// <summary>
    ///     A widget that has neither children controls, nor content.
    /// 
    ///     Useful for "input", "hr" and "br" elements, etc.
    /// </summary>
    public class Void : Widget
    {
        public Void ()
        {
            Element = "input";
        }

        /*
         * Overridden to throw an exception if user tries to explicitly set the innerValue attribute of this control.
         */
        public override void SetAttribute (string name, string value)
        {
            if (name == "innerValue")
                throw new ArgumentException ("you cannot set the 'innerValue' property of a Void widget");
            base.SetAttribute (name, value);
        }

        /*
         * Overridden to make sure user doesn't blow off his feet ...
         */
        protected override void AddedControl (Control control, int index)
        {
            throw new ArgumentException ("Void widget cannot have children");
        }

        /// <summary>
        ///     Renders widget's content as pure HTML into specified HtmlTextWriter.
        /// 
        ///     Override this one to provide custom rendering.
        ///     Notice, you can also override one of the other rendering methods, if you only wish to slightly modify the widget's rendering, such as
        ///     its opening or closing tag rendering.
        /// </summary>
        /// <param name="writer">The HtmlTextWriter to render the widget into.</param>
        protected override void RenderHtmlResponse (HtmlTextWriter writer)
        {
            // Rendering opening tag for element, then its children, before we render the closing tag.
            // Making sure we nicely indent element, unless this is an Ajax request.
            IndentWidgetRendering (writer);

            // Render start of opening tag, before we render all attributes.
            writer.Write (@"<{0} id=""{1}""", Element, ClientID);
            Attributes.Render (writer);
            writer.Write (" />");
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

        /// <summary>
        ///     Loads the form data from the HTTP request object for the current widget, if there is any data.
        /// </summary>
        protected override void LoadFormData ()
        {
            // Checking if this widget is a "input", and if so, loading its HTTP POST form data, if we should.
            if (Visible && Element == "input" && !string.IsNullOrEmpty (this ["name"]) && !HasAttribute ("disabled")) {

                // Figuring out what to do, according to what "type" of input element this is.
                switch (this ["type"]) {
                    case "radio":
                    case "checkbox":

                        // Notice, both checkboxes and radio buttons can be "grouped", by making them have the same "name" attribute value.
                        // If they do, they will be serialized as one HTTP POST parameter, with each checked element's value, separated by comma.
                        var splits = Page.Request.Params [this ["name"]].Split (',');
                        if (splits.Length == 1 && splits [0] == "on")
                            Attributes.SetAttributeFormData ("checked", null);
                        else if (splits.Any (ix => ix == this ["value"]))
                            Attributes.SetAttributeFormData ("checked", null);
                        else
                            Attributes.DeleteAttribute ("checked", false);
                        break;
                    default:
                        Attributes.SetAttributeFormData ("value", Page.Request.Params [this ["name"]]);
                        break;
                }
            }
        }

        /// <summary>
        ///     Overridden to ensure all "input" elements have a "name" attribute, correspnding to their ID, unless a name has already 
        ///     been explicitly created, in addition to ensuring all radio input elements have a "value", unless explicitly specified from before.
        /// </summary>
        /// <param name="e">EventArgs</param>
        protected override void OnPreRender (EventArgs e)
        {
            if (Element == "input") {
                if (string.IsNullOrEmpty (this ["name"]))
                    this ["name"] = ID;
                if (this ["type"] == "radio" && string.IsNullOrEmpty (this ["value"]))
                    this ["value"] = ID;
            }
            base.OnPreRender (e);
        }
    }
}