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
            if (e.Args.Name != "pf.set")
                throw new ApplicationException ("reached [pf.set] without execution node being [pf.set]");
            if (string.IsNullOrEmpty (e.Args.Get<string> ()) || !e.Args.Get<string> ().StartsWith ("@"))
                throw new ApplicationException ("[pf.set] needs at the very least an expression as its value");

            Match destinationMatch = new Expression (e.Args.Get<string> ()).Evaluate (e.Args);
            if (destinationMatch == null)
                return; // destination node not found

            if (e.Args.Count > 0) {
                if (!e.Args [0].Get<string> ().StartsWith ("@") || e.Args [0].Get<string> ().StartsWith (@"@"""))
                    destinationMatch.Assign (e.Args [0].Get<string> ());
                else
                    destinationMatch.Assign (new Expression (e.Args [0].Get<string> ()).Evaluate (e.Args [0]));
            } else {
                destinationMatch.Assign ();
            }
        }
    }
}

