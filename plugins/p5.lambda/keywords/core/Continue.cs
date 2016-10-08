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

using p5.core;

namespace p5.lambda.keywords.core
{
    /// <summary>
    ///     Class wrapping the [continue] keyword in p5 lambda.
    /// </summary>
    public static class Continue
    {
        /// <summary>
        ///     The [break] keyword, allows you to break out of a loop, such as [while] or [for-each]
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "continue")]
        public static void lambda_continue (ApplicationContext context, ActiveEventArgs e)
        {
            // Inserting "return signaling node", such that [eval] and similar constructs will break out
            // of their current execution
            e.Args.Root.Insert (0, new Node ("_continue"));
        }
    }
}
