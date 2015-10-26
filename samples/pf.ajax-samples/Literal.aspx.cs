/*
 * Phosphorus.Five, copyright 2014 - 2015, Thomas Hansen, isa.lightbringer@gmail.com
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using pf.ajax.core;
// ReSharper disable UnusedParameter.Global

// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable UnusedMember.Global

namespace phosphorus.five.samples
{
    using pf = pf.ajax.widgets;

    public partial class Literal : AjaxPage
    {
        [WebMethod]
        protected void element_onclick (pf.Literal literal, EventArgs e) { literal.innerValue = "this is the updated text"; }
    }
}