/*
 * phosphorus five, copyright 2014 - thomas@magixilluminate.com
 * phosphorus five is licensed as mit, see the enclosed license.txt file for details
 */

using System;
using System.Web.UI;

namespace phosphorus.ajax.widgets
{
    /// <summary>
    /// a widget that contains children widgets. everything between the opening and end declaration of this widget 
    /// in your .aspx markup will be treated as controls
    /// </summary>
    public class Container : Widget
    {
        // overridden to throw an exception if user tries to explicitly set the innerHTML attribute of this control
        public override void SetAttribute (string key, string value)
        {
            if (key == "innerHTML")
                throw new ArgumentException ("you cannot set the innerHTML property of a Container widget", key);
            base.SetAttribute (key, value);
        }
    }
}

