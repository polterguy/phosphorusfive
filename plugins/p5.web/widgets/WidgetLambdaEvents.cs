/*
 * Phosphorus Five, copyright 2014 - 2017, Thomas Hansen, thomas@gaiasoul.com
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
using p5.exp.exceptions;

namespace p5.web.widgets
{
    /// <summary>
    ///     Class encapsulating lambda events for widgets.
    /// </summary>
    public class WidgetLambdaEvents : BaseWidget
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="WidgetLambdaEvents"/> class.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="manager">PageManager owning this instance</param>
        public WidgetLambdaEvents (ApplicationContext context, PageManager manager)
            : base (context, manager)
        { }

        /// <summary>
        ///     Returns the requested lambda event(s) for the specified widget(s).
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.widgets.lambda-events.get")]
        public void p5_web_widgets_lambda_events_get (ApplicationContext context, ActiveEventArgs e)
        {
            // Sanity check.
            if (e.Args.Value == null || e.Args.Count == 0)
                throw new LambdaException (
                    string.Format ("[{0}] needs both a value being widget(s) to iterate, and children arguments being events to retrieve", e.Args.Name),
                    e.Args,
                    context);

            // Making sure we clean up and remove all arguments passed in after execution.
            using (new ArgsRemover (e.Args, true)) {

                // Since the process of returning widget's lambda events changes the e.Args children, we need to store the original children,
                // to make sure we only iterate the originally requested lambda events to caller.
                var eventList = e.Args.Children.Where (ix => ix.Name != "").ToList ();

                // Iterating through all widgets specified by caller.
                foreach (var idxWidget in FindWidgets<Widget> (context, e.Args)) {

                    // Iterating through all events requested by caller.
                    foreach (var idxEventNameNode in eventList) {

                        // Returning lambda object for widget lambda event, if it exists, making sure we clone it.
                        if (Manager.WidgetLambdaEventStorage [idxEventNameNode.Name, idxWidget.ID] != null) {

                            // We found a lambda event with the currently iterated name for the currently iterated widget.
                            var evtNode = Manager.WidgetLambdaEventStorage [idxEventNameNode.Name, idxWidget.ID].Clone ();
                            evtNode.Name = idxEventNameNode.Name;
                            e.Args.FindOrInsert (idxWidget.ID).Add (evtNode);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Changes or deletes the specified lambda event(s) for the specified widget(s).
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.widgets.lambda-events.set")]
        [ActiveEvent (Name = "p5.web.widgets.lambda-events.delete")]
        public void p5_web_widgets_lambda_events_set (ApplicationContext context, ActiveEventArgs e)
        {
            // Sanity check.
            if (e.Args.Value == null || e.Args.Count == 0)
                throw new LambdaException (
                    string.Format ("[{0}] needs both a value being widget(s) to iterate, and children arguments being events to retrieve", e.Args.Name),
                    e.Args,
                    context);

            // Making sure caller does not try to create "protected" lambda event(s).
            if (e.Args.Children.Any (ix => ix.Name.StartsWithEx (".") || ix.Name.StartsWithEx ("_")))
                throw new LambdaException ("Caller tried to create a protected lambda event starting with '_' or '.' for widget", e.Args, context);

            // Iterating through all widgets.
            foreach (var idxWidget in FindWidgets<Widget> (context, e.Args)) {

                // Looping through lambda events requested by caller, making sure we avoid formatting values.
                foreach (var idxEventNameNode in e.Args.Children.Where (ix => ix.Name != "")) {

                    // Checking if this is actually a deletion invocation.
                    if (idxEventNameNode.Count == 0) {

                        // Deleting currently iterated lambda event for currently iterated widget.
                        Manager.WidgetLambdaEventStorage.Remove (idxEventNameNode.Name, idxWidget.ID);

                    } else {

                        // Checking if this is "delete event" invocation, at which point it is a bug.
                        if (e.Name == "p5.web.widgets.lambda-events.delete")
                            throw new LambdaException ("Cannot pass in content to [p5.web.widgets.lambda-events.delete]", idxEventNameNode, context);

                        // Setting widget's lambda event to whatever we were given, making sure we clone the lambda supplied.
                        var clone = idxEventNameNode.Clone ();

                        // Making sure updated lambda event is parametrized with [_event] node.
                        clone.FindOrInsert ("_event", 0).Value = idxWidget.ID;
                        Manager.WidgetLambdaEventStorage [idxEventNameNode.Name, idxWidget.ID] = clone;
                    }
                }
            }
        }

        /// <summary>
        ///     Lists all existing lambda events for specified widget(s).
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.widgets.lambda-events.list")]
        public void p5_web_widgets_lambda_events_list (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new ArgsRemover (e.Args, true)) {

                // Iterating through all widgets caller specified.
                foreach (var idxWidget in FindWidgets<Widget> (context, e.Args)) {

                    // Setting up a return value node, and iterating through each lambda event, currently iterated widget contains.
                    var idxRetVal = new Node (idxWidget.ID);
                    foreach (var idxAtr in Manager.WidgetLambdaEventStorage.FindByKey2 (idxWidget.ID)) {

                        // Adding name of currently iterated lambda event into return node.
                        idxRetVal.Add (idxAtr);
                    }

                    // Checking if we've got more than zero events, and if so, adding event node, containing list of all lambda events for widget.
                    if (idxRetVal.Count > 0)
                        e.Args.Add (idxRetVal);
                }
            }
        }

        /// <summary>
        ///     Lists all dynamically created lambda event, for all widgets, matching an (optional) filter criteria.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.widgets.lambda-events.vocabulary")]
        public void p5_web_widgets_lambda_events_vocabulary (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up arguments after invocation.
            using (new ArgsRemover (e.Args, true)) {

                // Retrieving filter, if any.
                var filter = new List<string> (XUtil.Iterate<string> (context, e.Args));

                // Retrieving all lambda events, matching any optional filters supplied.
                ListActiveEvents (Manager.WidgetLambdaEventStorage.Keys, e.Args, filter, context);

                // Checking if there exists a whitelist, and if so, removing everything not in it.
                if (context.Ticket.Whitelist != null)
                    e.Args.RemoveAll (ix => context.Ticket.Whitelist [ix.Get<string> (context)] == null);
            }
        }

        /*
         * Handler for making sure we are able to evaluate widget lambda events.
         * Notice, this is one of few places where you can have multiple handlers for the same (dynamic) Active Event.
         * This creates a paradox, which is that only the lambda event that is created last can actually return values.
         * 
         * The same arguments are passed into each invocation, if you have multiple handlers, by cloning the original argument objects.
         * If you have multiple handlers, you can return nodes from each of them, which allows you to have multiple handlers returning
         * nodes, which will be returned after invocation of all handlers to caller. But if you have multiple handlers, and you return
         * values from them, only the lambda event that happens to be evaluated last, will be able to actually return anything as a value.
         */
        [ActiveEvent (Name = "")]
        void null_handler (ApplicationContext context, ActiveEventArgs e)
        {
            // Checking if we have a lambda event with the specified name.
            var enumerable = Manager.WidgetLambdaEventStorage [e.Name];
            if (enumerable != null) {

                // Used to store return values to return to caller after all invocations have been evaluated.
                var retVal = new Node ();

                // Used to hold original arguments.
                Node argsClone = null;

                // Iterating through each lambda event, invoking it with the specified arguments.
                // Notice the ToList invocation, which is necessary in case the lambda creates a new lambda event with the same name, possibly
                // on a different widget, which would kill our IEnumerable object unless we made sure it was made into a list first.
                var list = enumerable.ToList ();
                foreach (var idxLambda in list) {

                    // In case this is a second invocation, to the same lambda event, for a different widget, we reset the original args, to
                    // make sure each invocation gets the same set of arguments.
                    // If it is not a second invocation, we check to see if we have more than one invocation, and if so, we clone the original args,
                    // such that we can keep them around to our next invocation.
                    if (argsClone != null) {
                        e.Args.Value = argsClone.Value;
                        e.Args.AddRange (argsClone.Clone ().Children);
                    } else if (list.Count > 1) {
                        argsClone = e.Args.Clone ();
                    }

                    // Creating a clone of currently evaluated lambda object.
                    var clone = idxLambda.Clone ();

                    // Evaluating lambda event.
                    XUtil.EvaluateLambda (context, clone, e.Args);

                    // Moving returned nodes from invocation into retVal.
                    retVal.AddRange (e.Args.Children);

                    // Then making sure we return the value returned by invocation, if any.
                    // Notice, only the last lambda event handler, if there are multiple lambda events, will be able to return anything as value.
                    retVal.Value = e.Args.Value;
                }

                // Returning all return values, from all invocations to caller.
                e.Args.Clear ().AddRange (retVal.Children);
                e.Args.Value = retVal.Value;
            }
        }

        /*
         * Returns Active Events from source given, using name as type of Active Event.
         */
        static void ListActiveEvents (
            IEnumerable<string> source,
            Node args,
            List<string> filter,
            ApplicationContext context)
        {
            // Looping through each Active Event name from given IEnumerable.
            foreach (var idx in source) {

                // Notice, there should not exist any lambda events being "protected", meaning starting out with "." or "_".
                // There MIGHT however exist "hidden" lambda events for widgets.
                if (idx.Contains ("._"))
                    continue;

                // Checking to see if we have any filter, and if not, returning everything.
                if (filter.Count == 0) {

                    // No filter(s) given, returning everything.
                    args.Add (new Node ("dynamic", idx));

                } else {

                    // We have filter(s), checking to see if Active Event name matches at least one of our filters.
                    if (filter.Any (ix => ix.StartsWithEx ("~") ? idx.Contains (ix.Substring (1)) : idx == ix)) {
                        args.Add (new Node ("dynamic", idx));
                    }
                }
            }

            // Sorting events alphabetically.
            args.Sort (delegate (Node x, Node y) {
                return x.Get<string> (context).CompareTo (y.Value);
            });
        }
    }
}
