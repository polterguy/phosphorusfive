/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Web.UI;

// ReSharper disable InconsistentNaming

namespace phosphorus.ajax.widgets
{
    /// <summary>
    ///     a widget that does not contain children widgets, but instead it contains only text as its inner content. use the
    ///     innerValue property to access the text content of the widget
    /// </summary>
    [ParseChildren (true, "innerValue")]
    [PersistChildren (false)]
    [ViewStateModeById]
    public class Literal : Widget
    {
        // overridden to supply default html element
        public override string ElementType
        {
            get
            {
                if (string.IsNullOrEmpty (base.ElementType))
                    return "p";
                return base.ElementType;
            }
            set { base.ElementType = value; }
        }

        /// <summary>
        ///     gets or sets the innerValue property of the widget. this is also the inner default property of the widget,
        ///     which means the stuff between the opening and end declaration of the widget in your .aspx markup
        /// </summary>
        /// <value>the inner html</value>
        [PersistenceMode (PersistenceMode.InnerDefaultProperty)]
        public string innerValue
        {
            get { return this ["innerValue"]; }
            set { this ["innerValue"] = value; }
        }
        
        public override string this [string name]
        {
            get {
                if (name == "value" && ElementType == "textarea") {
                    // special treatment for textarea HTML elements "value" property, to create uniform access of all
                    // form element values possible
                    return base ["innerValue"];
                }
                return base [name];
            }
            set {
                // special treatment for textarea to make it resemble what goes on on the client-side
                if (name == "value" && ElementType == "textarea") {
                    base ["innerValue"] = value;
                } else {
                    base [name] = value;
                }
            }
        }

        public override bool HasAttribute (string name)
        {
            if (name == "value" && ElementType == "textarea") {
                // special treatment for textarea HTML elements "value" property, to create uniform access of all
                // form element values possible
                return HasAttribute ("innerValue");
            }
            return base.HasAttribute (name);
        }

        // notice how we do not call base here, and only render the innerValue and none of its children
        protected override void RenderChildren (HtmlTextWriter writer) { writer.Write (innerValue); }

        protected override void AddedControl (Control control, int index) { throw new ApplicationException ("Literal widget cannot have children controls"); }

        protected override bool HasContent
        {
            get { return !string.IsNullOrEmpty (innerValue); }
        }
    }
}