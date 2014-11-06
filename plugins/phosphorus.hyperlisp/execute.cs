/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using phosphorus.core;

namespace phosphorus.hyperlisp
{
    public static class execute
    {
        /// <summary>
        /// hyperlisp main execution Active Event entry point
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.execute")]
        private static void execute_impl (ApplicationContext context, ActiveEventArgs e)
        {
            Node iterNode = e.Args.FirstChild;
            while (iterNode != null) {
                string activeEvent = iterNode.Name;
                Node next = iterNode.NextSibling;
                context.Raise (activeEvent, iterNode);
                iterNode = next;
            }
        }
    }
}

