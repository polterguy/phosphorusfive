/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Configuration;
using phosphorus.core;

namespace phosphorus.lambda
{
    /// <summary>
    /// class to help transform between code and <see cref="phosphorus.core.Node"/> 
    /// </summary>
    public static class code
    {
        private static string _language;

        /// <summary>
        /// helper to transform from code code syntax to <see cref="phosphorus.core.Node"/> tree structure and vice versa
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "code2lambda")]
        [ActiveEvent (Name = "lambda2code")]
        private static void code_transformer (ApplicationContext context, ActiveEventArgs e)
        {
            if (!string.IsNullOrEmpty (_language)) {
                context.Raise ("pf." + _language + "." + e.Name.Replace ("code", _language), e.Args);
            } else {
                _language = ConfigurationManager.AppSettings ["default-execution-language"] ?? "hyperlisp";
                context.Raise (e.Name, e.Args);
            }
        }

        /// <summary>
        /// changes or retrieves the default execution language of the lambda engine. if the value of the
        /// execution node is empty, it will return the default execution language, otherwise it will
        /// change the default execution language to the value given
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "execution-language")]
        private static void execution_language (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value == null) {
                e.Args.Value = _language;
            } else {
                _language = e.Args.Get<string> ();
            }
        }
    }
}

