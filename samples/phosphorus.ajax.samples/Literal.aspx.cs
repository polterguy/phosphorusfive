
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using System;
using phosphorus.ajax.core;

namespace phosphorus.five.samples
{
    using pf = ajax.widgets;

    public partial class Literal : AjaxPage
    {
        [WebMethod]
        protected void element_onclick (pf.Literal literal, EventArgs e)
        {
            literal.innerValue = "this is the updated text";
        }
    }
}
