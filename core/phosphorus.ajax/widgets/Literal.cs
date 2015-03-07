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
    ///     Allows you to create a Widget that does not have children widgets, but instead has a simple text, HTML or string value.
    /// 
    ///     This Widget can only contain text or HTML. Use the innerValue property to access or change the text of your widget.
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
        ///     and end declaration of the widget in your .aspx markup, will automatically end up as its content here.
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
