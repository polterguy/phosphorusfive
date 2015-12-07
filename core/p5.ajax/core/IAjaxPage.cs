/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.Collections.Generic;

namespace p5.ajax.core
{
    /// <summary>
    ///     Interface for all your pages that uses the p5.ajax library
    /// </summary>
    public interface IAjaxPage
    {
        /// <summary>
        ///     Returns the manager for your page
        /// </summary>
        /// <value>The manager</value>
        Manager Manager { get; }

        /// <summary>
        ///     Returns the list of JavaScript files/objects page contains
        /// </summary>
        /// <value>The JavaScript files to push back to client</value>
        List<Tuple<string, bool>> JavaScriptToPush { get; }

        /// <summary>
        ///     Returns the list of new JavaScript files/objects page added during this request
        /// </summary>
        /// <value>The JavaScript files to push back to client</value>
        List<Tuple<string, bool>> NewJavaScriptToPush { get; }

        /// <summary>
        ///     Returns the list of Stylesheet files that page contains
        /// </summary>
        /// <value>The CSS files to push back to client</value>
        List<string> StylesheetFilesToPush { get; }
        
        /// <summary>
        ///     Returns the list of Stylesheet files that was added during this request
        /// </summary>
        /// <value>The CSS files to push back to client</value>
        List<string> NewStylesheetFilesToPush { get; }

        /// <summary>
        ///     Registers a JavaScript file to be included on to the client-side
        /// </summary>
        /// <param name="url">URL to JavaScript file</param>
        void RegisterJavaScriptFile (string url);
        
        /// <summary>
        ///     Registers a stylesheet file to be included on to the client-side
        /// </summary>
        /// <param name="url">URL to JavaScript file</param>
        void RegisterStylesheetFile (string url);
    }
}