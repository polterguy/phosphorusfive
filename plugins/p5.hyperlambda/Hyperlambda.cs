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

using System.Text;
using p5.exp;
using p5.core;
using p5.hyperlambda.helpers;

namespace p5.hyperlambda
{
    /// <summary>
    ///     Class to help transform between Hyperlambda and <see cref="Node">Nodes</see>.
    /// </summary>
    public static class Hyperlambda
    {
        /// <summary>
        ///     Tranforms the given Hyperlambda to a lambda graph object.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "hyper2lambda")]
        public static void hyper2lambda (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new ArgsRemover (e.Args, true)) {

                // Concatenating all Hyperlambda submitted, injecting CR/LF between each component
                var builder = new StringBuilder();
                foreach (var idxHyperlisp in XUtil.Iterate<string> (context, e.Args)) {

                    // Making sure we put in a carriage return between each Hyperlambda entity
                    if (builder.Length > 0)
                        builder.Append ("\r\n");

                    // Appending currently iterated Hyperlambda into StringBuilder
                    builder.Append (idxHyperlisp);
                }

                // Utilizing NodeBuilder to create our p5 lambda return value
                e.Args.AddRange (new NodeBuilder (context, builder.ToString ()).Nodes);
            }
        }

        /// <summary>
        ///     Transforms the given lambda graph object to Hyperlambda.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "lambda2hyper")]
        public static void lambda2hyper (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new ArgsRemover (e.Args)) {

                // Using HyperlispBuilder to create Hyperlambda from p5 lambda
                e.Args.Value = new HyperlambdaBuilder (
                    context, 
                    XUtil.Iterate<Node> (context, e.Args))
                    .Hyperlambda;
            }
        }
    }
}
