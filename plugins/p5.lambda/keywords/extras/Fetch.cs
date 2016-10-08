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
 * along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the Affero GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using p5.core;
using p5.exp;

namespace p5.lambda.keywords.extras
{
    /// <summary>
    ///     Class wrapping the [fetch] keyword in p5 lambda
    /// </summary>
    public static class Fetch
    {
        /// <summary>
        ///     The [fetch] keyword, allows you to forward retrieve value results of evaluation of its body
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "fetch")]
        public static void lambda_fetch (ApplicationContext context, ActiveEventArgs e)
        {
            // Evaluating [fetch] lambda block
            context.Raise ("eval-mutable", e.Args);

            // Now we can fetch expression value, and clear body
            e.Args.Value = XUtil.Single<object> (context, e.Args, true);
            e.Args.Clear ();
        }
    }
}
