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
    /// class wrapping execution engine keyword "get", which allows for retrieving values of nodes
    /// </summary>
    public static class get
    {
        /// <summary>
        /// get keyword for execution engine
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.get")]
        private static void pf_get (ApplicationContext context, ActiveEventArgs e)
        {
            if (string.IsNullOrEmpty (e.Args.Get<string> ()) || !e.Args.Get<string> ().StartsWith ("@"))
                throw new ApplicationException ("[pf.get] needs at the very least an expression as its value");

            string expression = Expression.FormatNode (e.Args);
            Match sourceMatch = new Expression (expression).Evaluate (e.Args);
            if (sourceMatch == null)
                return; // destination node not found

            if (sourceMatch.TypeOfMatch == Match.MatchType.Count) {
                e.Args.Add (new Node (string.Empty, sourceMatch.Count));
            } else {
                foreach (Node idxMatch in sourceMatch.Matches) {
                    switch (sourceMatch.TypeOfMatch) {
                    case Match.MatchType.Name:
                        e.Args.Add (new Node (string.Empty, idxMatch.Name));
                        break;
                    case Match.MatchType.Value:
                        e.Args.Add (new Node (string.Empty, idxMatch.Value));
                        break;
                    case Match.MatchType.Path:
                        e.Args.Add (new Node (string.Empty, idxMatch.Path));
                        break;
                    case Match.MatchType.Node:
                        e.Args.Add (idxMatch.Clone ());
                        break;
                    }
                }
            }
        }
    }
}

