/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using p5.ajax.core;

namespace p5.samples
{
    using p5 = p5.ajax.widgets;

    public partial class Void : AjaxPage
    {
        protected p5.Literal lbl;
        protected p5.Void txtBox;

        [WebMethod]
        protected void btn_onclick (p5.Widget sender, EventArgs e)
        {
            lbl.innerValue = "Value of textbox was; '" + txtBox["value"] + "'";
        }
    }
}