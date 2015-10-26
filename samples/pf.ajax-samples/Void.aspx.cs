/*
 * Phosphorus.Five, copyright 2014 - 2015, Thomas Hansen, isa.lightbringer@gmail.com
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using pf.ajax.core;

// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable UnassignedField.Global
// ReSharper disable UnusedMember.Global

namespace phosphorus.five.samples
{
    using pf = pf.ajax.widgets;

    public partial class Void : AjaxPage
    {
        protected pf.Literal Lbl;
        protected pf.Void Txt;

        [WebMethod]
        protected void btn_onclick (pf.Widget sender, EventArgs e) { Lbl.innerValue = "value of textbox was; '" + Txt ["value"] + "'"; }
    }
}