/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, thomas@gaiasoul.com
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

    public partial class Default : AjaxPage
    {
        [WebMethod]
        protected void hello_onclick (p5.Literal sender, EventArgs e)
        {
            if (sender.innerValue == "Click me for hello world") {
                sender.innerValue = "Click me again, while inspecting the HTTP request using your browser";
                sender ["class"] = "change-is-the-only-constant";
            } else {
                sender.innerValue = "Click me for hello world";
                sender.RemoveAttribute ("class");
            }
        }
    }
}