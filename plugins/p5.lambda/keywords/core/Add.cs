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

using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.lambda.keywords.core
{
    /// <summary>
    ///     Class wrapping execution engine keyword [add]
    /// </summary>
    public static class Add
    {
        /// <summary>
        ///     The [add] keyword allows you to append a source lambda object into a destination lambda object
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "add")]
        public static void lambda_add (ApplicationContext context, ActiveEventArgs e)
        {
            // Asserting destination is expression
            var destEx = e.Args.Value as Expression;
            if (destEx == null)
                throw new LambdaException (
                    string.Format ("Not a valid destination expression given to [add], value was '{0}', expected expression", e.Args.Value),
                    e.Args,
                    context);

            // Updating destination(s) with value of source
            foreach (var idxDestination in destEx.Evaluate (context, e.Args, e.Args)) {

                // Raising source Active Event
                var src = XUtil.Source (context, e.Args, idxDestination.Node);

                // Adding source nodes to destination
                var tmpNodeIdxDest = idxDestination.Value as Node;
                if (tmpNodeIdxDest == null)
                    throw new LambdaException ("Destination for [add] was not a node", e.Args, context);

                // Iterating each source, appending into currently iterated destination
                foreach (var idxSource in src) {

                    // Checking if currently iterated source is a node, and if not, we convert it into a node, which will
                    // result in a "root node", which we remove, and only add its children, into currently iterated destination
                    if (idxSource is Node)
                        tmpNodeIdxDest.Add ((idxSource as Node).Clone ());
                    else
                        tmpNodeIdxDest.AddRange (Utilities.Convert<Node> (context, idxSource).Children);
                }
            }
        }
    }
}
