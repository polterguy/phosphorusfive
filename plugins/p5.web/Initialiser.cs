/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.Linq;
using System.Web.UI;
using System.Collections;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.ajax.core;
using p5.web.widgets;
using p5.ajax.widgets;
using p5.exp.exceptions;
using p5.web.widgets.helpers;

/// <summary>
///     Main namespace for web plugin, that allows for creation of ajax web widgets, and such
/// </summary>
namespace p5.web
{
    /// <summary>
    ///     Class initialising one HTTP request
    /// </summary>
    public class Initialiser
    {
        /*
         * Raised by page during initialization of page
         */
        [ActiveEvent (Name = "p5.web.initialize-page", Protection = EventProtection.NativeOpen)]
        private static void p5_web_initialize_page (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving Page and Manager for current HTTP context
            var page = e.Args ["page"].Get<AjaxPage> (context);

            // Creating an instance of this class, registering it as event listener in App Context
            var instance = new PageManager (
                context, 
                page);
            context.RegisterListeningObject (instance);
        }
    }
}
