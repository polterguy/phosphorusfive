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
    public class Literal : Widget
    {
        public Literal ()
        {
            Element = "p";
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
            get { return base.GetAttribute ("innerValue"); }
            set { base.SetAttribute ("innerValue", value); }
        }

        /*
         * Overridden to provide some help for retrieving value of textarea and option elements.
         */
        public override string GetAttribute (string key)
        {
            if (Element == "textarea" && key == "value") {

                // Special treatment for textarea, to make it resemble what goes on on the client-side.
                return innerValue;

            }

            if (Element == "option" && key == "value" && !HasAttribute ("value")) {

                // By default, "option" HTML elements returns their "innerValue" if they have no value attribute.
                return innerValue;

            }

            // No need for special treatment, letting base do the heavy lifting.
            return base.GetAttribute (key);
        }

        /*
         * Overridden to provide some help for setting value of textarea and option elements.
         */
        public override void SetAttribute (string key, string value)
        {
            if (Element == "textarea" && key == "value") {

                // Special treatment for textarea, to make it resemble what goes on on the client-side.
                base.SetAttribute ("innerValue", value);

            } else if (Element == "option" && key == "selected") {

                // Sanity check.
                if (value != null && value != "selected")
                    throw new ArgumentException ("You cannot set the selected attribute of an option element to anything but null or 'selected'.");

                // Returning early if widget already has the attribute, to avoid re-rendering, when selected property is set to whatever it was before.
                if (HasAttribute ("selected") && GetAttribute ("selected") == value)
                    return;

                // Since there can only be one "selected" widget inside a "select" HTML element, we remove the "selected" attribute on all widgets
                // in parent Controls collection, before we add the "selected" attribute for this "option" HTML element.
                foreach (Widget idxWidget in Parent.Controls) {
                    idxWidget.DeleteAttribute ("selected");
                }
                base.SetAttribute (key, value);

                // Due to a "bug" in the way browsers handles the "selected" property on "option" elements, we need to re-render all
                // select widgets, every time one of its "option" elements' "selected" attribute is changed.
                // Read more here; https://bugs.chromium.org/p/chromium/issues/detail?id=662669
                (Parent as Widget).RenderMode = RenderingMode.ReRender;

            } else {

                // No need for special treatment.
                base.SetAttribute (key, value);
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
            // Rendering opening tag for element, then its "innerValue", before we render the closing tag.
            // Making sure we nicely format output, if we should.
            IndentWidgetRendering (writer);

            // Render start of opening tag, before we render all attributes.
            writer.Write (@"<{0} id=""{1}""", Element, ClientID);
            Attributes.Render (writer);
            writer.Write (">");
            writer.Write (innerValue);
            writer.Write ("</{0}>", Element);
        }

        /*
         * Overridden to make sure user doesn't blow off his feet ...
         */
        protected override void AddedControl (Control control, int index)
        {
            throw new ApplicationException ("Literal widget cannot have children controls");
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

                // Although technically the Literal widget could be used for a "select" element, we prevent it, to avoid messing up FORM data loading, 
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

        /// <summary>
        ///     Loads the form data from the HTTP request object for the current widget, if there is any data.
        /// </summary>
        protected override void LoadFormData ()
        {
            // Checking if this widget is a "textarea", and if so, loading its HTTP POST form data, if we should.
            if (Visible && Element == "textarea" && !string.IsNullOrEmpty (this ["name"]) && !HasAttribute ("disabled")) {
                Attributes.SetAttributeFormData ("innerValue", Page.Request.Form [this ["name"]]);
            }
        }

        /// <summary>
        ///     Overridden to ensure all "textarea" elements have a "name" attribute, correspnding to their ID, unless a name has already 
        ///     been explicitly created.
        /// </summary>
        /// <param name="e">EventArgs</param>
        protected override void OnPreRender (EventArgs e)
        {
            if (Element == "textarea" && !HasAttribute ("name"))
                this ["name"] = ID;
            base.OnPreRender (e);
        }
    }
}
