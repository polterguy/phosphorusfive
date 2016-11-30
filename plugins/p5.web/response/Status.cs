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

using System.Web;
using p5.exp;
using p5.core;

namespace p5.web.ui.response
{
    /// <summary>
    ///     Helper class to manipulate the HTTP status
    /// </summary>
    public static class Status
    {
        /// <summary>
        ///     Changes the HTTP status code for the current response
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "set-http-status-code")]
        public static void set_http_status_code (ApplicationContext context, ActiveEventArgs e)
        {
            HttpContext.Current.Response.StatusCode = e.Args.GetExValue<int> (context);
        }

        /// <summary>
        ///     Changes the HTTP status description for the current response
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "set-http-status")]
        public static void set_http_status (ApplicationContext context, ActiveEventArgs e)
        {
            HttpContext.Current.Response.Status = e.Args.GetExValue<string> (context);
        }
    }
}
