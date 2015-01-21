
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.tiedown
{
    /// <summary>
    /// utility class for common operations
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// executes a pf.lambda file
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="filePath">absolute path to file to execute</param>
        /// <param name="args">list of nodes that will be passsed in as arguments to file</param>
        public static void ExecuteLambdaFile (ApplicationContext context, string filePath, IEnumerable<Node> args = null)
        {
            // loading file
            Node loadFileNode = new Node (string.Empty, filePath);
            context.Raise ("pf.file.load", loadFileNode);

            // converting file to lambda tree
            Node fileToNodes = new Node (string.Empty, loadFileNode [0].Get<string> ());
            context.Raise ("code2lambda", fileToNodes);

            // appending args into lambda tree
            if (args != null) {
                foreach (var idxArg in args) {
                    fileToNodes.Add (idxArg.Clone ());
                }
            }

            // raising file as pf.lambda object
            context.Raise ("lambda", fileToNodes);
        }
    }
}
