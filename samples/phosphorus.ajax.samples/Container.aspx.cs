/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using phosphorus.ajax.core;
// ReSharper disable UnusedMember.Global
// ReSharper disable PartialTypeWithSinglePart

namespace phosphorus.five.samples
{
    using pf = ajax.widgets;

    public partial class Container : AjaxPage
    {
        // using the same event handler for both of our literal widgets
        [WebMethod]
        protected void element_onclick (pf.Literal literal, EventArgs e) { literal.innerValue = "widget was clicked"; }
    }
}