/*
 * phosphorus five, copyright 2014 - thomas@magixilluminate.com
 * phosphorus five is licensed as mit, see the enclosed license.txt file for details
 */

using System;
using System.Web.UI;

namespace phosphorus.ajax.widgets
{
    /// <summary>
    /// a widget that does not contain children widgets, but instead it contains only text as its inner content
    /// </summary>
    [ParseChildren(true, "innerHTML")]
    [PersistChildren(true)]
    public class Literal : Widget
    {
        /// <summary>
        /// gets or sets the innerHTML property of the widget
        /// </summary>
        /// <value>the inner html</value>
        [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public string innerHTML {
            get { return this ["innerHTML"]; }
            set { this ["innerHTML"] = value; }
        }

        protected override void RenderAttributes (HtmlTextWriter writer)
        {
            // to not mess with the viewstate, we need to reinsert the attribute from the same place we remove it from
            // and to not render the innerHTML as an attribute for our html element, we need to remove it before we render the attributes
            int index = Attributes.FindIndex (
                delegate(Attribute obj) {
                    return obj.Name == "innerHTML";
                });
            Attribute atr = null;
            if (index != -1) {
                atr = Attributes [index];
                Attributes.RemoveAt (index);
            }
            base.RenderAttributes (writer);
            if (atr != null)
                Attributes.Insert (index, atr);
        }

        protected override void RenderChildren (HtmlTextWriter writer)
        {
            writer.Write (innerHTML);
        }
    }
}

