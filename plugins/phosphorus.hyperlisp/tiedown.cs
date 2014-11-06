/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using phosphorus.core;

namespace phosphorus.hyperlisp
{
    public static class tiedown
    {
        [ActiveEvent (Name = "pf.application-start")]
        private static void pf_application_start (ApplicationContext sender, ActiveEventArgs e)
        {
            // execute startup hyperlisp file
        }

        [ActiveEvent (Name = "pf.load")]
        private static void pf_load (ApplicationContext sender, ActiveEventArgs e)
        {
            // execute load hyperlisp file
        }
    }
}

