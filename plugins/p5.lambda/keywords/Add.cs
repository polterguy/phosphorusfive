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
    ///     Class wrapping execution engine keyword [add].
    /// </summary>
    public static class Add
    {
        /// <summary>
        ///     The [add] keyword allows you to append a node-set into another node-set.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "add")]
        private static void lambda_add (ApplicationContext context, ActiveEventArgs e)
        {
            // figuring out source type, for then to execute the corresponding logic, but first asserting destination is expression
            if (!(e.Args.Value is Expression))
                throw new LambdaException ("Not a valid destination expression given, make sure you set [add]'s value to an expression using :x:", e.Args, context);

            if (e.Args.Count > 0 && e.Args.LastChild.Name == "rel-src") {

                // relative source
                AppendRelativeSource (e.Args, context);
            } else {

                // static source
                AppendStaticSource (e.Args, context);
            }
        }
        
        /*
         * source is static
         */
        private static void AppendStaticSource (Node args, ApplicationContext context)
        {
            // retrieving source before we start iterating destination,
            // in case destination and source overlaps
            var sourceNodes = XUtil.SourceNodes (args, context);

            // making sure there is a source
            if (sourceNodes == null)
                return;

            // looping through every destination node
            foreach (var idxDestination in args.Get<Expression> (context).Evaluate (args, context, args)) {

                // verifying destination actually is a node
                var curDest = idxDestination.Value as Node;
                if (curDest == null)
                    throw new LambdaException ("cannot [add] into something that's not a node", args, context);

                // cloning source and appending into destination
                foreach (var idxSource in sourceNodes) {

                    curDest.Add (idxSource.Clone ());
                }
            }
        }

        /*
         * relative source
         */
        private static void AppendRelativeSource (Node args, ApplicationContext context)
        {
            foreach (var idxDestination in args.Get<Expression> (context).Evaluate (args, context, args)) {

                // verifying destination actually is a node
                var curDest = idxDestination.Value as Node;
                if (curDest == null)
                    throw new LambdaException ("cannot [add] into something that's not a node", args, context);

                // evaluating source for each destination since it is relative to destination
                foreach (var idxSource in XUtil.SourceNodes (args, idxDestination.Node, context)) {

                    curDest.Add (idxSource.Clone ());
                }
            }
        }
    }
}
