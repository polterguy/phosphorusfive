/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
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
    using p5 = p5.ajax.widgets;

    public partial class Literal : AjaxPage
    {
        protected p5.Literal txt;

        [WebMethod]
        protected void element_onclick (p5.Literal literal, EventArgs e)
        {
            literal.innerValue = "This is the updated text";
        }

        [WebMethod]
        protected void btn_onclick (p5.Literal literal, EventArgs e)
        {
            txt.innerValue += @"
Howdy world!";
        }

        [WebMethod]
        protected void btn2_onclick (p5.Literal literal, EventArgs e)
        {
            txt.innerValue = @"Howdy world
foo bar!";
        }

        [WebMethod]
        protected void btn3_onclick (p5.Literal literal, EventArgs e)
        {
            txt.innerValue = @"Howdy World
<strong>foo
<em>THOMAS</em>
sdf</strong> sdfoih
sdfpih <i>TJOBING</i>";
        }
    }
}