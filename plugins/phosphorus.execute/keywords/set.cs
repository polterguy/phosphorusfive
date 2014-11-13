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
    /// class wrapping execution engine keyword "set", which allows for changing values of nodes
    /// </summary>
    public static class set
    {
        /// <summary>
        /// set keyword for execution engine
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.set")]
        private static void pf_set (ApplicationContext context, ActiveEventArgs e)
        {
            if (!Expression.IsExpression (e.Args.Get<string> ()))
                throw new ApplicationException ("[pf.set] needs at the very least an expression as its value");

            // finding Match object for destination
            Match destinationMatch = new Expression (e.Args.Get<string> ()).Evaluate (e.Args);
            if (destinationMatch == null)
                return; // destination node not found

            if (e.Args.Count > 0) {
                string source = Expression.FormatNode (e.Args.FirstChild);
                if (!Expression.IsExpression (source)) {

                    // source is a string literal value
                    if (source.StartsWith (@"\"))
                        source = source.Substring (1); // escaped value, possible escaped expression
                    destinationMatch.AssignValue (source);
                } else {

                    // source is an expression
                    destinationMatch.AssignMatch (new Expression (source).Evaluate (e.Args.FirstChild));
                }
            } else {

                // there is no source, hence we assign null to destination
                destinationMatch.AssignNull ();
            }
        }
    }
}

