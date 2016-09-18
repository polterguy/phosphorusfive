/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Linq;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.lambda.keywords
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
        [ActiveEvent (Name = "add", Protection = EventProtection.LambdaClosed)]
        public static void lambda_add (ApplicationContext context, ActiveEventArgs e)
        {
            // Basic syntax checking
            if (!(e.Args.Value is Expression))
                throw new LambdaException ("[add] was not given a destination expression", e.Args, context);

            // Retrieving source nodes
            var srcNodes = e.Args.Children.Where (ix => ix.Name != "").ToList ();

            // Sanity check
            if (srcNodes.Count != 1)
                throw new LambdaException (
                    "[add] must be given exactly one source child node",
                    e.Args,
                    context);

            // OK, we're sane, so far ...
            var srcNode = srcNodes[0];

            // Figuring out source type
            if (srcNode.Name == "src") {

                // Static source
                AppendStaticSource (e.Args, context);
            } else {

                // Relative source
                AppendRelativeOrEventSource (e.Args, context);
            }
        }
        
        /*
         * Appends a static source into destination
         */
        private static void AppendStaticSource (Node args, ApplicationContext context)
        {
            // Retrieving source before we start iterating destination, in case destination and source overlaps
            var sourceNodes = XUtil.SourceNodes (context, args);

            // Making sure there is a source
            if (sourceNodes == null)
                return;

            // Looping through each destination node
            foreach (var idxDestination in args.Get<Expression> (context).Evaluate (context, args, args)) {

                // Verifying destination actually is a node
                var curDest = idxDestination.Value as Node;
                if (curDest == null)
                    throw new LambdaException (
                        "Cannot [add] into something that's not a node", 
                        args, 
                        context);

                // Looping through each source
                foreach (var idxSource in sourceNodes) {

                    // Appending currently iterated cloned source into destination
                    curDest.Add (idxSource.Clone ());
                }
            }
        }

        /*
         * Appends a relative source into destination
         */
        private static void AppendRelativeOrEventSource (Node args, ApplicationContext context)
        {
            // Looping through each destination
            foreach (var idxDestination in args.Get<Expression> (context).Evaluate (context, args, args)) {

                // Verifying destination actually is a node
                var curDest = idxDestination.Value as Node;
                if (curDest == null)
                    throw new LambdaException ("Cannot [add] into something that's not a node", args, context);

                // Evaluating source for each destination since it is relative to destination
                foreach (var idxSource in XUtil.SourceNodes (context, args, idxDestination.Node)) {

                    // Appending currently iterated cloned source into destination
                    curDest.Add (idxSource.Clone ());
                }
            }
        }
    }
}
