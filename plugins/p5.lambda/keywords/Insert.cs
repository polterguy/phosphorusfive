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
    ///     Class wrapping execution engine keyword [insert-before] and [insert-after].
    /// </summary>
    public static class Insert
    {
        /// <summary>
        ///     The [insert-before] keyword allows you to insert nodes before another node.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "insert-before", Protection = EventProtection.LambdaClosed)]
        private static void lambda_insert_before (ApplicationContext context, ActiveEventArgs e)
        {
            InsertNodes (e.Args, context, false);
        }
        
        /// <summary>
        ///     The [insert-after] keyword allows you to insert nodes after another node.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "insert-after", Protection = EventProtection.LambdaClosed)]
        private static void lambda_insert_after (ApplicationContext context, ActiveEventArgs e)
        {
            InsertNodes (e.Args, context, true);
        }

        /*
         * commonalities for both Active Events
         */
        private static void InsertNodes (Node args, ApplicationContext context, bool after)
        {
            if (!(args.Value is Expression))
                throw new LambdaException ("Not a valid destination expression given, make sure you set [" + args.Name + "]'s value to an expression using :x:", args, context);

            if (args.Count > 0 && args.LastChild.Name == "rel-src") {

                // relative source
                InsertRelativeSource (args, context, after);
            } else {

                // static source
                InsertStaticSource (args, context, after);
            }
        }

        /*
         * source is static
         */
        private static void InsertStaticSource (Node args, ApplicationContext context, bool after)
        {
            // retrieving source before we start iterating destination,
            // in case destination and source overlaps
            var sourceNodes = XUtil.SourceNodes (context, args);

            // making sure there is a source
            if (sourceNodes == null)
                return;

            // looping through every destination node
            foreach (var idxDestination in args.Get<Expression> (context).Evaluate (context, args, args)) {

                // verifying destination actually is a node
                var curDest = idxDestination.Value as Node;
                if (curDest == null)
                    throw new LambdaException ("cannot [insert-before] or [insert-after] into something that's not a node", args, context);

                // looping through each node in source
                foreach (var idxSourceNode in sourceNodes) {

                    // inserting copy into currently iterated destination
                    curDest.Parent.Insert (curDest.Parent.IndexOf (curDest) + (after ? 1 : 0), idxSourceNode.Clone ());
                }
            }
        }

        /*
         * relative source
         */
        private static void InsertRelativeSource (Node args, ApplicationContext context, bool after)
        {
            // looping through each destination first, since source is relative to destination
            foreach (var idxDestination in args.Get<Expression> (context).Evaluate (context, args, args)) {

                // verifying destination actually is a node
                var curDest = idxDestination.Value as Node;
                if (curDest == null)
                    throw new LambdaException ("cannot [insert-before] or [insert-after] into something that's not a node", args, context);

                // evaluating source relative to destination, and iterating over each relative source
                foreach (var idxSource in XUtil.SourceNodes (context, args, idxDestination.Node)) {

                    // inserting copy of source into currently iterated destination
                    curDest.Parent.Insert (curDest.Parent.IndexOf (curDest) + (after ? 1 : 0), idxSource.Clone ());
                }
            }
        }
    }
}
