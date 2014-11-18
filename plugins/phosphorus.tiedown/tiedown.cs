/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using phosphorus.core;

namespace phosphorus.tiedown
{
    /// <summary>
    /// class to help tie together application-start Active Event and page-load Active Event.  loads and executes the startup hyperlisp file
    /// and the page-load hyperlisp file
    /// </summary>
    public static class tiedown
    {
        /// <summary>
        /// executes the startup hyperlisp files
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.application-start")]
        private static void pf_application_start (ApplicationContext context, ActiveEventArgs e)
        {
            // execute startup hyperlisp file
        }

        /// <summary>
        /// executes the page-load hyperlisp file
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.page-load")]
        private static void pf_page_load (ApplicationContext context, ActiveEventArgs e)
        {
            // execute load hyperlisp file
            string code = @"
_data:liv
  flercellede:dyr
    reptiler:slanger, osv
    pattedyr:hunder, mennesker, osv
      kattedyr:pusekatter, tigre, løver og sånt no :)
        tigre:bengalsk
        løver:afrika
      primater:apekatter, osv
        mennesker:per, ole og jens
          johan:kult!!
      hundedyr:coyote, ulv, hund
        ulv:grå
        coyote:ørken
_val
pf.add:@/-/?node
  :@/\/**/_data/**/[2,]/%2/?path
pf.set:@/-2/*/?refnode
  :@/\/**/mennesker/?node
";
            /////////////////////////////////////////////
            /// "adressible nodes" through DNA code, which is usable in for instance 
            /// "for-each" and "storing execution state" of execution engine
            /////////////////////////////////////////////
            Node node = new Node ("root", code);
            context.Raise ("pf.hyperlisp-2-node", node);
            node.Value = null;
            context.Raise ("pf.execute", node);
            context.Raise ("pf.node-2-hyperlisp", node);
        }
    }
}

