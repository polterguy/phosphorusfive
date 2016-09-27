/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Linq;
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
