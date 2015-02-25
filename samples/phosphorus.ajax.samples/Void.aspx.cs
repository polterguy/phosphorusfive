/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using System;
using phosphorus.ajax.core;

// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable UnassignedField.Global
// ReSharper disable UnusedMember.Global

namespace phosphorus.five.samples
{
    using pf = ajax.widgets;

    public partial class Void : AjaxPage
    {
        protected pf.Literal Lbl;
        protected pf.Void Txt;

        [WebMethod]
        protected void btn_onclick (pf.Widget sender, EventArgs e) { Lbl.innerValue = "value of textbox was; '" + Txt ["value"] + "'"; }
    }
}