/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Collections.Generic;
using System.Linq;
using p5.core;
using p5.exp;
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
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "add", Protection = EventProtection.LambdaClosed)]
        private static void lambda_add (ApplicationContext context, ActiveEventArgs e)
        {
            // Basic syntax checking
            if (!(e.Args.Value is Expression))
                throw new LambdaException ("[add] was not given a destination expression", e.Args, context);

            // Figuring out source type
            if (e.Args.Count > 0 && e.Args.LastChild.Name == "rel-src") {

                // Relative source
                AppendRelativeSource (e.Args, context);
            } else {

                // Static source
                AppendStaticSource (e.Args, context);
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
                    throw new LambdaException ("Cannot [add] into something that's not a node", args, context);

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
        private static void AppendRelativeSource (Node args, ApplicationContext context)
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
