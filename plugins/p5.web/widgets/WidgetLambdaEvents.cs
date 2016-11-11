/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System.Linq;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.ajax.widgets;

namespace p5.web.widgets
{
    /// <summary>
    ///     Class encapsulating lambda events of widgets
    /// </summary>
    public class WidgetLambdaEvents : BaseWidget
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="p5.web.widgets.WidgetLambdaEvents"/> class
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="manager">PageManager owning this instance</param>
        public WidgetLambdaEvents (ApplicationContext context, PageManager manager)
            : base (context, manager)
        { }

        #region [ -- Widget lambda events -- ]

        /// <summary>
        ///     Returns the given lambda event(s) for the given widget(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-widget-lambda-event")]
        public void get_widget_lambda_event (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Looping through all widgets
                foreach (var idxWidget in FindWidgets<Widget> (context, e.Args, "get-widget-lambda-event")) {

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
        [ActiveEvent (Name = "set-widget-lambda-event")]
        public void set_widget_lambda_event (ApplicationContext context, ActiveEventArgs e)
        {
            // Looping through all widget IDs
            foreach (var idxWidget in FindWidgets<Widget> (context, e.Args, "set-widget-lambda-event")) {

                // Looping through events requested by caller
                foreach (var idxEventNameNode in e.Args.Children) {

                    // Checking if this is actually a deletion invocation
                    if (idxEventNameNode.Children.Count == 0) {

                        // Deleting existing event
                        Manager.WidgetLambdaEventStorage.Remove (idxEventNameNode.Name, idxWidget.ID);
                    } else {

                        // Setting Widget's Ajax event to whatever we were given
                        var clone = idxEventNameNode.Clone();
                        Manager.WidgetLambdaEventStorage[idxEventNameNode.Name, idxWidget.ID] = clone;
                    }
                }
            }
        }

        /// <summary>
        ///     Lists all existing lambda events for given widget(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-widget-lambda-events")]
        public void list_widget_lambda_events (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new p5.core.Utilities.ArgsRemover(e.Args, true)) {

                // Looping through all widgets
                foreach (var idxWidget in FindWidgets<Widget> (context, e.Args, "list-widget-lambda-events")) {

                    // Then looping through all attribute keys, filtering everything out that does not start with "on"
                    Node curNode = new Node(idxWidget.ID);
                    foreach (var idxAtr in Manager.WidgetLambdaEventStorage.FindByKey2 (idxWidget.ID)) {

                        // Checking if this attribute is an event or not
                        curNode.Add(idxAtr);
                    }

                    // Checking if we've got more than zero events for given widget, and if so, 
                    // adding event node, containing list of events
                    if (curNode.Children.Count > 0)
                        e.Args.Add(curNode);
                }
            }
        }

        /// <summary>
        ///     Lists all dynamically created Active Events.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "vocabulary")]
        [ActiveEvent (Name = ".vocabulary")]
        public void vocabulary (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving filter, if any
            var filter = e.Args.Value == null ? new List<string>() : new List<string> (XUtil.Iterate<string> (context, e.Args));

            // Getting all dynamic Active Events
            ListActiveEvents (Manager.WidgetLambdaEventStorage.Keys, e.Args, filter, "dynamic", context, e.Name.StartsWith ("."));
        }

        /*
         * Raises Widget specific lambda events
         */
        [ActiveEvent (Name = "")]
        private void null_handler (ApplicationContext context, ActiveEventArgs e)
        {
            // Notice, since lambda events might end up creating new lambda events, the "ToList" operation below
            // is necessary!
            // But first, we check to see if there are any lambda objects for given Active Event
            var list = Manager.WidgetLambdaEventStorage [e.Name].ToList ();
            if (list.Count > 0) {

                // Used to store return values to return to caller after all invocations have been evaluated
                Node retVal = new Node ();

                // Looping through each lambda event handler for given event
                foreach (var idxLambda in list) {

                    // Creating a clone of currently iterated lambda object for Active Event, and inserting [_event] to allow
                    // event to know the ID the widget the lambda event exists within
                    var idxLambdaClone = idxLambda.Clone ();
                    idxLambdaClone.Insert (0, new Node ("_event", idxLambdaClone.Name));

                    // Creating a clone of args, such that we can make sure each invocation have the same set of parameters
                    var argsClone = e.Args.Clone ();
                    XUtil.RaiseEvent (context, e.Args, idxLambdaClone, e.Name);

                    // Moving stuff returned from invocation into retVal, which holds return values from all invocations
                    retVal.AddRange (e.Args.Children);

                    // Then making sure we return the (last) value returned by invocation, if any
                    if (e.Args.Value != null)
                        retVal.Value = e.Args.Value;

                    // Then resetting e.Args back to what it was, to make sure next invocation, if any, gets the same set of arguments
                    e.Args.Value = argsClone.Value;
                    e.Args.AddRange (argsClone.Children);
                }

                // Returning all return values, from all invocations to caller
                e.Args.Clear ().AddRange (retVal.Children);
                e.Args.Value = retVal.Value;
            }
        }

        #endregion

        #region [ -- Private helper methods -- ]

        /*
         * Returns Active Events from source given, using name as type of Active Event
         */
        private static void ListActiveEvents (
            IEnumerable<string> source,
            Node node,
            List<string> filter,
            string eventTypeName,
            ApplicationContext context,
            bool isNative)
        {
            // Looping through each Active Event from IEnumerable
            foreach (var idx in source) {

                if (!isNative && (idx.StartsWith (".") || idx.StartsWith ("_") || idx.Contains ("._")))
                    continue;

                // Checking to see if we have any filter
                if (filter.Count == 0) {

                    // No filter(s) given, slurping up everything
                    node.Add (new Node (eventTypeName, idx));
                } else {

                    // We have filter(s), checking to see if Active Event name matches at least one of our filters
                    if (filter.Any (ix => idx.Contains (ix))) {
                        node.Add (new Node (eventTypeName, idx));
                    }
                }
            }

            // Sorting such that keywords comes first
            node.Sort (delegate (Node x, Node y) {
                if (x.Get<string> (context).Contains (".") && !y.Get<string> (context).Contains ("."))
                    return 1;
                else if (!x.Get<string> (context).Contains (".") && y.Get<string> (context).Contains ("."))
                    return -1;
                else
                    return x.Get<string> (context).CompareTo (y.Value);
            });
        }

        #endregion
    }
}
