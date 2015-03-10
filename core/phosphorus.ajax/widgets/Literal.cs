/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Web.UI;

// ReSharper disable InconsistentNaming

namespace phosphorus.ajax.widgets
{
    /// <summary>
    ///     Allows you to create a widget that contains simple text or HTML.
    /// 
    ///     This widget can only contain text or HTML. Use the <em>"innerValue"</em> property to access or change the text of your widget.
    /// 
    ///     Contrary to the Container widget, this widget cannot have children controls, and trying to add controls to its Controls collection,
    ///     will throw an exception. This widget is for HTML elements where you don't care about having Ajax functionality, or DOM event handlers
    ///     for its children, but are satisified with being able to set its <em>"text"</em> or <em>"innerHTML"</em> property.
    /// 
    ///     Use the <em>"innerValue"</em> property of the widget to change or retrieve its inner text/HTML content.
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
        ///     Gets or sets the innerValue property of the widget.
        /// 
        ///     This is also the inner default property of the widget, which means the stuff between the opening
        ///     and end declaration of the widget in your .aspx markup, will automatically end up as its content, or innerHTML property here.
        /// </summary>
        /// <value>The innerHTML equivalent from JavaScript.</value>
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
                    // special treatment for textarea, to make it resemble what goes on on the client-side
                    return base ["innerValue"];
                }
                if (name == "value" && ElementType == "option" && !base.HasAttribute ("value"))
                    // by default, option HTML elements returns their "innerValue" if they have no value attribute
                    return this ["innerValue"];
                return base [name];
            }
            set {
                if (name == "value" && ElementType == "textarea") {
                    // special treatment for textarea, to make it resemble what goes on on the client-side
                    base ["innerValue"] = value;
                } else {
                    base [name] = value;
                }
            }
        }

        public override bool HasAttribute (string name)
        {
            if (name == "value" && ElementType == "textarea") {
                // special treatment for textarea, to make it resemble what goes on on the client-side
                return HasAttribute ("innerValue");
            }
            return base.HasAttribute (name);
        }

        // notice how we do not call base here, and only render the innerValue and none of its children
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
