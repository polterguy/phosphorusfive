/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Configuration;
using phosphorus.core;

namespace phosphorus.execute
{
    /// <summary>
    /// class to help transform between code and <see cref="phosphorus.core.Node"/> 
    /// </summary>
    public static class code
    {
        /// <summary>
        /// helper to transform from code code syntax to <see cref="phosphorus.core.Node"/> tree structure and vice versa
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.code-2-nodes")]
        [ActiveEvent (Name = "pf.nodes-2-code")]
        [ActiveEvent (Name = "pf.node-2-code")]
        private static void code_transformer (ApplicationContext context, ActiveEventArgs e)
        {
            string language = ConfigurationManager.AppSettings ["default-execution-language"];
            context.Raise (e.Name.Replace ("code", language), e.Args);
        }
    }
}

