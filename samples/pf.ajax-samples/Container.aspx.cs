/*
 * Phosphorus.Five, copyright 2014 - 2015, Thomas Hansen, isa.lightbringer@gmail.com
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using pf.ajax.core;
// ReSharper disable UnusedMember.Global
// ReSharper disable PartialTypeWithSinglePart

namespace phosphorus.five.samples
{
    using pf = pf.ajax.widgets;

    public partial class Container : AjaxPage
    {
        // using the same event handler for both of our literal widgets
        [WebMethod]
        protected void element_onclick (pf.Literal literal, EventArgs e) { literal.innerValue = "widget was clicked"; }
    }
}