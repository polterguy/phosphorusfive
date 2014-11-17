/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.execute
{
    /// <summary>
    /// class wrapping execution engine keyword "add", which allows for changing values of nodes
    /// </summary>
    public static class add
    {
        /// <summary>
        /// put keyword for execution engine
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.add")]
        private static void pf_add (ApplicationContext context, ActiveEventArgs e)
        {
            /*Match destinationMatch = GetDestinationMatch (e.Args);
            if (e.Args.Count == 1 && Expression.IsExpression (e.Args.FirstChild.Get<string> ()) && e.Args.FirstChild.Name == string.Empty) {

                // we're assigning an expression here
                Match sourceMatch = new Expression (e.Args.FirstChild.Get<string> ()).Evaluate (e.Args.FirstChild);
                if (sourceMatch.Count == 0)
                    throw new ArgumentException ("source expression yielded no matches, [pf.put] must have an existing source");
                destinationMatch.AssignMatch (sourceMatch, context, false);
            } else {

                // source is not an expression, either it's a "null assignment" or we're putting a bunch of nodes into another node
                if (e.Args.Count == 0) {

                    // "null assignment", and they're not legal here
                    throw new ArgumentException ("you cannot [pf.put] a null value, [pf.put] needs an existing source");
                } else {

                    // assigning a bunch of nodes to destination
                    destinationMatch.AssignNodes (e.Args.Children, context, false);
                }
            }*/
        }

        /*
         * will return a Match object for the destination of the "pf.put"
         */
        private static Match GetDestinationMatch (Node node)
        {
            string destinationExpression = node.Get<string> ();
            if (!Expression.IsExpression (destinationExpression))
                throw new ApplicationException ("[pf.put] needs a valid expression yielding an actual result as its value");

            // finding Match object for destination
            Match destinationMatch = new Expression (destinationExpression).Evaluate (node);
            if (destinationMatch.Count == 0)
                throw new ArgumentException ("destination expression for [pf.put] yielded no result, expression was; '" + destinationExpression + "'");

            if (!destinationMatch.IsAssignable)
                throw new ArgumentException ("destination expression for [pf.put] is not assignable, expression was; '" + destinationExpression + "'");

            return destinationMatch;
        }
    }
}

