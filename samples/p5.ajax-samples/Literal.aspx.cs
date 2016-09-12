/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using p5.ajax.core;

namespace p5.samples
{
    using p5 = p5.ajax.widgets;

    public partial class Literal : AjaxPage
    {
        [WebMethod]
        protected void element_onclick (p5.Literal literal, EventArgs e)
        {
            literal.innerValue = "this is the updated text";
        }
    }
}