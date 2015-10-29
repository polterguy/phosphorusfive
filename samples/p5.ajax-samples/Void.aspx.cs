/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, isa.lightbringer@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using p5.ajax.core;

namespace p5.samples
{
    using p5 = p5.ajax.widgets;

    public partial class Void : AjaxPage
    {
        protected p5.Literal Lbl;
        protected p5.Void Txt;

        [WebMethod]
        protected void btn_onclick (p5.Widget sender, EventArgs e)
        {
            Lbl.innerValue = "value of textbox was; '" + Txt ["value"] + "'";
        }
    }
}