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

using p5.exp;
using p5.core;

namespace p5.lambda.keywords.core
{
    /// <summary>
    ///     Class wrapping the [return] keyword in p5 lambda.
    /// </summary>
    public static class Return
    {
        /// <summary>
        ///     The [return] keyword, allows you to return immediately from the evaluation of your code
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "return")]
        public static void lambda_return (ApplicationContext context, ActiveEventArgs e)
        {
            // Inserting "return signaling node", such that [eval] and similar constructs will break out
            // of their current execution
            e.Args.Root.Insert (0, new Node ("_return"));
            e.Args.Root.Value = e.Args.GetExValue<object> (context);

            // Adding all children of [return] as result value to evaluation
            int idxNo = 1;
            foreach (var idxRetNode in e.Args.Children) {
                e.Args.Root.Insert (idxNo++, idxRetNode.Clone ());
            }
        }
    }
}
