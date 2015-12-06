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
    ///     Class encapsulating lambda events of widgets
    /// </summary>
    public class WidgetLambdaEvents
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="p5.web.widgets.WidgetLambdaEvents"/> class
        /// </summary>
        /// <param name="page">Page.</param>
        public WidgetLambdaEvents (ApplicationContext context, PageManager manager)
        {
            // Setting WidgetManager for this instance
            Manager = manager;
        }

        /*
         * PageManager for this instance
         */
        private PageManager Manager {
            get;
            set;
        }

        #region [ -- Widget lambda events -- ]

        /// <summary>
        ///     Returns the given lambda event(s) for the given widget(s).
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-widget-lambda-event", Protection = EventProtection.LambdaClosed)]
        private void get_widget_lambda_event (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new p5.core.Utilities.ArgsRemover (e.Args, true)) {

                // Looping through all widgets
                foreach (var idxWidget in Manager.FindWidgets<Widget> (context, e.Args, "get-widget-lambda-event")) {

                    // Looping through events requested by caller
                    foreach (var idxEventNameNode in e.Args.Children.ToList ()) {

                        // Returning lambda object for Widget Ajax event
                        if (Manager.WidgetLambdaEventStorage[idxEventNameNode.Name, idxWidget.ID] != null) {

                            // We found a Lambda event with that name for that widget
                            var evtNode = Manager.WidgetLambdaEventStorage[idxEventNameNode.Name, idxWidget.ID].Clone();
                            evtNode.Name = idxEventNameNode.Name;
                            e.Args.FindOrCreate(idxWidget.ID).Add(evtNode);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Changes the given lambda event(s) for the given widget(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "set-widget-lambda-event", Protection = EventProtection.LambdaClosed)]
        private void set_widget_lambda_event (ApplicationContext context, ActiveEventArgs e)
        {
            // Looping through all widget IDs
            foreach (var idxWidget in Manager.FindWidgets<Widget> (context, e.Args, "set-widget-lambda-event")) {

                // Looping through events requested by caller
                foreach (var idxEventNameNode in e.Args.Children) {

                    // Verifying Active Event is not protected
                    if (!Manager.CanOverrideEventInLambda (context, idxEventNameNode.Name))
                        throw new LambdaSecurityException(
                            string.Format ("You cannot override Active Event '{0}' since it is protected", e.Args.Name),
                            e.Args,
                            context);

                    // Checking if this is actually a deletion invocation
                    if (idxEventNameNode.Count == 0) {

                        // Deleting existing event
                        Manager.WidgetLambdaEventStorage.Remove (idxEventNameNode.Name, idxWidget.ID);
                    } else {

                        // Setting Widget's Ajax event to whatever we were given
                        Manager.WidgetLambdaEventStorage[idxEventNameNode.Name, idxWidget.ID] = idxEventNameNode.Clone();
                    }
                }
            }
        }

        /// <summary>
        ///     Lists all existing lambda events for given widget(s)
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-widget-lambda-events", Protection = EventProtection.LambdaClosed)]
        private void list_widget_lambda_events (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new p5.core.Utilities.ArgsRemover(e.Args, true)) {

                // Looping through all widgets
                foreach (var idxWidget in Manager.FindWidgets<Widget> (context, e.Args, "list-widget-lambda-events")) {

                    // Then looping through all attribute keys, filtering everything out that does not start with "on"
                    Node curNode = new Node(idxWidget.ID);
                    foreach (var idxAtr in Manager.WidgetLambdaEventStorage.FindByKey2 (idxWidget.ID)) {

                        // Checking if this attribute is an event or not
                        curNode.Add(idxAtr);
                    }

                    // Checking if we've got more than zero events for given widget, and if so, 
                    // adding event node, containing list of events
                    if (curNode.Count > 0)
                        e.Args.Add(curNode);
                }
            }
        }

        /*
         * Raises Widget specific lambda events
         */
        [ActiveEvent (Name = "", Protection = EventProtection.NativeOpen)]
        private void null_handler (ApplicationContext context, ActiveEventArgs e)
        {
            // Looping through each lambda event handler for given event
            foreach (var idxLambda in Manager.WidgetLambdaEventStorage [e.Name].ToList ()) {

                // Raising Active Event
                XUtil.RaiseEvent (context, e.Args, idxLambda, e.Name);
            }
        }

        #endregion
    }
}
