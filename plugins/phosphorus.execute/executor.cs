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
    /// class to help execute nodes
    /// </summary>
    public static class executor
    {
        /// <summary>
        /// main execution Active Event entry point for executing nodes as execution trees
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.execute")]
        private static void pf_execute (ApplicationContext context, ActiveEventArgs e)
        {
            List<Node> nodes = new List<Node> (e.Args.Children);
            foreach (Node idx in nodes) {
                context.Raise (idx.Name, idx);
            }
        }
    }
}

