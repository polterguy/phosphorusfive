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

using p5.exp;
using p5.core;

namespace p5.lambda.keywords.extras
{
    /// <summary>
    ///     Class wrapping the [fetch] Active Event.
    /// </summary>
    public static class Fetch
    {
        /// <summary>
        ///     The [fetch] event, allows you to forward retrieve a single value, resulting of evaluation of a lambda object.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "fetch")]
        public static void lambda_fetch (ApplicationContext context, ActiveEventArgs e)
        {
            // Evaluating [fetch] lambda block.
            context.Raise ("eval-mutable", e.Args);

            // Now we can fetch expression value, and clear body, making sure we remove formatting parameters first.
            e.Args.Value = XUtil.Single<object> (context, e.Args, true);
            e.Args.Clear ();
        }
    }
}
