
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using System;
using phosphorus.ajax.core;

namespace phosphorus.five.samples
{
    using pf = ajax.widgets;

    public partial class Container : AjaxPage
    {
        // using the same event handler for both of our literal widgets
        [WebMethod]
        protected void element_onclick (pf.Literal literal, EventArgs e)
        {
            literal.innerValue = "widget was clicked";
        }
    }
}
