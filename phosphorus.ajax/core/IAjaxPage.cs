/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;

namespace phosphorus.ajax.core
{
    /// <summary>
    /// interface for all your pages that uses the phosphorus.ajax library. instead of implementing this interface 
    /// yourself, you can inherit from the <see cref="phosphorus.ajax.AjaxPage"/>, which takes care of everything 
    /// automatically for you
    /// </summary>
    public interface IAjaxPage
    {
        /// <summary>
        /// returns the manager for your page
        /// </summary>
        /// <value>the manager</value>
        Manager Manager {
            get;
        }

        /// <summary>
        /// registers a JavaScript file to be transmitted to the client
        /// </summary>
        /// <param name="url">URL to JavaScript file</param>
        void RegisterJavaScriptFile (string url);

        /// <summary>
        /// returns the list of JavaScript files that was added during this request, and must be pushed back to client somehow
        /// </summary>
        /// <value>The java script files to push.</value>
        List<string> JavaScriptFilesToPush {
            get;
        }
        
        /// <summary>
        /// registers a css stylesheet file to be transmitted to the client
        /// </summary>
        /// <param name="url">URL to stylesheet file</param>
        void RegisterStylesheetFile (string url);

        /// <summary>
        /// returns the list of JavaScript files that was added during this request, and must be pushed back to client somehow
        /// </summary>
        /// <value>The java script files to push.</value>
        List<string> StylesheetFilesToPush {
            get;
        }
    }
}

