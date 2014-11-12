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
foo:howdy
  :joppla
  hel/\lo"":""  world      ""
  long-string:@""jo dude, this is
a test of """"how cool"""" it is possible to be :)
since it tests the nice stuff :D""
    h1:@"" x""
    h2:@""y ""
    zz:THOMAS
      qq:@""tjolla """"hopp/sann
sa\""
  xx:yy
    zz:ff
d:qwerty
pf.get:@/*/0|/*/1|/*/2&/*/long-string|/*/0/path";
            Node node = new Node (null, code);
            context.Raise ("pf.hyperlisp-2-node", node);
            context.Raise ("pf.execute", node);
            node.Value = null;
            context.Raise ("pf.node-2-hyperlisp", node);
        }
    }
}

