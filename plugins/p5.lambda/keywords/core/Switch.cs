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

using System.Linq;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.lambda.keywords.core
{
    /// <summary>
    ///     Class wrapping the [switch] keyword in p5 lambda
    /// </summary>
    public static class Switch
    {
        /// <summary>
        ///     The [switch] keyword, allows you to create matches for value with [case] children
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "switch")]
        public static void lambda_switch (ApplicationContext context, ActiveEventArgs e)
        {
            // Sanity check
            if (e.Args.Children.Count (ix => ix.Name != "" && ix.Name != "case" && ix.Name != "default") > 0)
                throw new LambdaException ("Only [case] and [default] children are legal beneath [switch]", e.Args, context);

            // Sanity check
            if (e.Args.Children.Count (ix => ix.Name == "default") > 1)
                throw new LambdaException ("[switch] can only have one [default] block", e.Args, context);

            // Retrieving value
            var value = e.Args.GetExValue<object> (context, null);

            // Finding out what to evaluate, defaulting to [default] block
            var eval = e.Args.Children.FirstOrDefault (ix => ix.Name == "default");

            // Special case for "null value"
            if (value == null) {

                // Finding first [case] with null value, defaulting to existing [default]
                eval = e.Args.Children.FirstOrDefault (ix => ix.Name == "case" && ix.GetExValue<object> (context, null) == null) ?? eval;
            } else {

                // Special case for node comparison
                if (value is Node) {

                    // Doing CompareTo
                    eval = e.Args.Children.FirstOrDefault (ix => 
                        ix.Name == "case" && (value as Node).CompareTo (ix.GetExValue<object> (context)) == 0) ?? eval;
                    
                } else {

                    // Finding first [case] that matches value, defaulting to existing [default]
                    eval = e.Args.Children.FirstOrDefault (ix => 
                        ix.Name == "case" && value.Equals (ix.GetExValue<object> (context))) ?? eval;
                }
            }

            // Evaluating eval, unless it is null, but before we do, we set all [case] and [default] to boolean false
            foreach (var idxChild in e.Args.Children) {
                idxChild.Value = false;
            }
            if (eval != null) {

                // Supporting "fallthrough"
                while (eval != null && eval.Children.Count == 0)
                    eval = eval.NextSibling;
                if (eval != null) {
                    eval.Value = true;
                    context.Raise ("eval-mutable", eval);
                }
            }
        }
    }
}
