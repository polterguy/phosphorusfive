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
using p5.ajax.widgets;
using p5.exp.exceptions;

namespace p5.web.widgets
{
    /// <summary>
    ///     Class encapsulating ajax events of widgets
    /// </summary>
    public class WidgetAjaxEvents : BaseWidget
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="p5.web.widgets.WidgetAjaxEvents"/> class
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="manager">PageManager owning this instance</param>
        public WidgetAjaxEvents (ApplicationContext context, PageManager manager)
            : base (context, manager)
        { }

        #region [ -- Widget Ajax events -- ]

        /// <summary>
        ///     Returns the given ajax event(s) for the given widget(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-widget-ajax-event", Protection = EventProtection.LambdaClosed)]
        public void get_widget_ajax_event (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new p5.core.Utilities.ArgsRemover (e.Args, true)) {

                // Looping through all widgets
                foreach (var idxWidget in FindWidgets<Widget> (context, e.Args, "get-widget-ajax-event")) {

                    // Looping through events requested by caller
                    foreach (var idxEventNameNode in e.Args.Children.Where (ix => ix.Name != "").ToList ()) {

                        // Returning lambda object for Widget Ajax event
                        if (Manager.WidgetAjaxEventStorage[idxWidget.ID, idxEventNameNode.Name] != null)
                            e.Args.FindOrCreate(idxWidget.ID).Add(Manager.WidgetAjaxEventStorage[idxWidget.ID, idxEventNameNode.Name].Clone());
                    }
                }
            }
        }

        /// <summary>
        ///     Changes the given ajax event(s) for the given widget(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "set-widget-ajax-event", Protection = EventProtection.LambdaClosed)]
        public void set_widget_ajax_event (ApplicationContext context, ActiveEventArgs e)
        {
            // Looping through all widgets
            foreach (var idxWidget in FindWidgets<Widget> (context, e.Args, "set-widget-ajax-event")) {

                // Looping through events requested by caller
                foreach (var idxEventNameNode in e.Args.Children) {

                    // Checking if we should delete existing event
                    if (idxEventNameNode.Children.Count == 0) {

                        // Deleting existing ajax event
                        Manager.WidgetAjaxEventStorage.Remove (idxWidget.ID, idxEventNameNode.Name);
                    } else {

                        // Setting Widget's Ajax event to whatever we were given
                        Manager.WidgetAjaxEventStorage[idxWidget.ID, idxEventNameNode.Name] = idxEventNameNode.Clone();
                    }
                }
            }
        }

        /// <summary>
        ///     Raises the specified ajax event(s) for specified widget(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "raise-widget-ajax-event", Protection = EventProtection.LambdaClosed)]
        public void raise_widget_ajax_event (ApplicationContext context, ActiveEventArgs e)
        {
            // Looping through all widgets
            foreach (var idxWidget in FindWidgets<Widget> (context, e.Args, "raise-widget-ajax-event")) {

                // Looping through events requested by caller
                foreach (var idxEventNameNode in e.Args.Children) {

                    // Raising specified event
                    idxWidget.InvokeEventHandler (idxEventNameNode.Name);
                }
            }
        }

        /// <summary>
        ///     Lists all existing ajax events for given widget(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-widget-ajax-events", Protection = EventProtection.LambdaClosed)]
        public void list_widget_ajax_events (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new p5.core.Utilities.ArgsRemover(e.Args, true)) {

                // Looping through all widgets supplied
                foreach (var idxWidget in FindWidgets<Widget> (context, e.Args, "list-widget-ajax-events")) {

                    // Then looping through all attribute keys, filtering everything out that does not start with "on" of "_on"
                    Node curNode = new Node(idxWidget.ID);
                    foreach (var idxAtr in idxWidget.AttributeKeys.Where (ix => ix.StartsWith ("on") || ix.StartsWith ("_on"))) {

                        // Adding this attribute
                        curNode.Add(idxAtr);
                    }

                    // Special handling of [oninit], since it never leaves the server, and is hence not in widget's attribute collection
                    if (Manager.WidgetAjaxEventStorage[idxWidget.ID, "oninit"] != null)
                        curNode.Add("oninit");

                    // Checking if we've got more than zero events for given widget, and if so, adding event node, containing list of events
                    if (curNode.Children.Count > 0)
                        e.Args.Add(curNode);
                }
            }
        }

        #endregion
    }
}
