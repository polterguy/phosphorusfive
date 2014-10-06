/*
 * phosphorus five, copyright 2014 - thomas@magixilluminate.com
 * phosphorus five is licensed as mit, see the enclosed license.txt file for details
 */

using System;
using System.Web.UI;

namespace phosphorus.ajax.widgets
{
    /// <summary>
    /// a widget that contains children widgets
    /// </summary>
    public class Container : Widget
    {
        public override void SetAttribute (string name, string value)
        {
            if (name == "innerHTML")
                throw new ArgumentException ("you cannot set the innerHTML property of a Container widget");
            base.SetAttribute (name, value);
        }
    }
}

