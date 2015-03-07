/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using phosphorus.ajax.core;
// ReSharper disable UnusedParameter.Global

// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable UnusedMember.Global

namespace phosphorus.five.samples
{
    using pf = ajax.widgets;

    public partial class Literal : AjaxPage
    {
        [WebMethod]
        protected void element_onclick (pf.Literal literal, EventArgs e) { literal.innerValue = "this is the updated text"; }
    }
}