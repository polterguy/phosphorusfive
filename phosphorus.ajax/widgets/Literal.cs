/*
 * phosphorus five, copyright 2014 - thomas@magixilluminate.com
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Web.UI;

namespace phosphorus.ajax.widgets
{
    /// <summary>
    /// a widget that does not contain children widgets, but instead it contains only text as its inner content. use the 
    /// innerHTML property to access the text content of the widget
    /// </summary>
    [ParseChildren(true, "innerHTML")]
    [PersistChildren(true)]
    public class Literal : Widget
    {
        /// <summary>
        /// gets or sets the innerHTML property of the widget. this is also the inner default property of the widget, 
        /// which means the stuff between the opening and end declaration of the widget in your .aspx markup
        /// </summary>
        /// <value>the inner html</value>
        [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public string innerHTML {
            get { return this ["innerHTML"]; }
            set { this ["innerHTML"] = value; }
        }

        protected override void RenderAttributes (HtmlTextWriter writer)
        {
            // unless we remove the innerHTML attribute before rendering, the contents of the control will 
            // be added as an attribute to the controls. to not mess with the viewstate however, we need to 
            // reinsert the attribute the same place afterwards that we remove it from
            int index = Attributes.FindIndex (
                delegate(Attribute obj) {
                    return obj.Name == "innerHTML";
                });

            // removing innerHTML attribute, if it exists
            Attribute atr = null;
            if (index != -1) {
                atr = Attributes [index];
                Attributes.RemoveAt (index);
            }

            // allowing base to render all attributes now, minus the innerHTML attribute
            base.RenderAttributes (writer);

            // reinserting the innerHTML attribute again, if it exists
            if (atr != null)
                Attributes.Insert (index, atr);
        }

        // notice how we do not call base here, and only render the innerHTML and none of its children
        protected override void RenderChildren (HtmlTextWriter writer)
        {
            writer.Write (innerHTML);
        }

        protected override void AddedControl (Control control, int index)
        {
            throw new ArgumentException ("Literal widget cannot have children controls");
        }
        
        protected override bool HasContent {
            get { return !string.IsNullOrEmpty (this ["innerHTML"]); }
        }
    }
}

