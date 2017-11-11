/*
 * Phosphorus Five, copyright 2014 - 2017, Thomas Hansen, thomas@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System;
using p5.ajax.core;

namespace p5.samples
{
    public partial class Postback : AjaxPage
    {
        protected p5.ajax.widgets.Literal btnAdd;
        protected p5.ajax.widgets.Container mainCtr;

        [WebMethod]
        protected void add_ctrl (p5.ajax.widgets.Literal sender, EventArgs e)
        {
            sender.innerValue = "Hello World!";
            sender ["style"] = "background-color:LightBlue;";

            var lit = mainCtr.CreatePersistentControl<p5.ajax.widgets.Literal> ();
            lit.Element = "li";
            lit ["class"] = "some-class-value";
            lit.innerValue = "Item no; " + mainCtr.Controls.Count;
            lit ["onclick"] = "ctrl_clicked";
        }

        [WebMethod]
        protected void ctrl_clicked (p5.ajax.widgets.Literal sender, EventArgs e)
        {
            sender.innerValue = "I was clicked";
        }

        protected void btnPostback_Click (object sender, EventArgs e)
        {
            btnAdd.innerValue = "Postback";
        }
    }
}