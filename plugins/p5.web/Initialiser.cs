/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
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

using p5.core;
using p5.ajax.core;

namespace p5.web
{
    /// <summary>
    ///     Class initialising one HTTP request
    /// </summary>
    public class Initialiser
    {
        /*
         * Raised by page during initialization of page
         */
        [ActiveEvent (Name = "p5.web.initialize-page")]
        private static void p5_web_initialize_page (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving Page and Manager for current HTTP context
            var page = e.Args ["page"].Get<AjaxPage> (context);

            // Creating an instance of this class, registering it as event listener in App Context
            var instance = new PageManager (
                context, 
                page);
            context.RegisterListeningObject (instance);
        }
    }
}
