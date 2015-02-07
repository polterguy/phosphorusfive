
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Configuration;
using System.Collections.Generic;
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
        [ActiveEvent (Name = "pf.core.application-start")]
        private static void pf_core_application_start (ApplicationContext context, ActiveEventArgs e)
        {
            // execute startup hyperlisp file, if we should
            if (!string.IsNullOrEmpty (ConfigurationManager.AppSettings ["application-startup-file"])) {

                // there is an application-startup-file declared in app.config file, executing it as pf.lambda file
                string appStartFilePath = ConfigurationManager.AppSettings ["application-startup-file"];
                ExecuteLambdaFile (context, appStartFilePath);
            }
        }

        /*
         * executes a lambda file
         */
        private static void ExecuteLambdaFile (ApplicationContext context, string filePath)
        {
            // loading file
            Node loadFileNode = new Node (string.Empty, filePath);
            context.Raise ("pf.file.load", loadFileNode);

            // TODO: use Utilities.Convert later when code is stable
            // converting file to lambda tree
            Node fileToNodes = new Node (string.Empty, loadFileNode [0].Get<string> (context));
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", fileToNodes);

            // raising file as pf.lambda object
            context.Raise ("lambda", fileToNodes);
        }
    }
}
