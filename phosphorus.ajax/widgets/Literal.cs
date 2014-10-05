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

        /// <summary>
        /// renders the content
        /// </summary>
        /// <param name="writer">html writer</param>
        protected override void RenderChildren (HtmlTextWriter writer)
        {
            writer.Write (innerHTML);
        }
    }
}

