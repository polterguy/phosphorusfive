/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, thomas@gaiasoul.com
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
using p5.core;
using p5.ajax.widgets;
using p5.exp.exceptions;

namespace p5.web.widgets
{
    /// <summary>
    ///     Class encapsulating Ajax events for widgets.
    /// </summary>
    public class WidgetAjaxEvents : BaseWidget
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="p5.web.widgets.WidgetAjaxEvents"/> class.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="manager">PageManager owning this instance</param>
        public WidgetAjaxEvents (ApplicationContext context, PageManager manager)
            : base (context, manager)
        { }

        /// <summary>
        ///     Returns the given Ajax event(s) for the given widget(s).
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.widgets.ajax-events.get")]
        public void p5_web_widgets_ajax_events_get (ApplicationContext context, ActiveEventArgs e)
        {
            // Sanity check.
            if (e.Args.Value == null || e.Args.Children.Count == 0)
                throw new LambdaException (
                    string.Format ("[{0}] needs both a value being widget(s) to iterate, and children arguments being events to retrieve", e.Args.Name), 
                    e.Args, 
                    context);

            // Making sure we clean up and remove all arguments passed in after execution.
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Since the process of returning widget's Ajax events changes the e.Args children, we need to store the original children,
                // to make sure we only iterate the originally requested Ajax events to caller.
                var eventList = e.Args.Children.Where (ix => ix.Name != "").ToList ();

                // Finding all widgets caller requests to retrieve events for.
                foreach (var idxWidget in FindWidgets<Widget> (context, e.Args)) {

                    // Iterating through each Ajax event requested by caller to retrieve for widgets.
                    foreach (var idxEventNameNode in eventList) {

                        // Checking if the currently iterated widget has the currently iterated Ajax event, 
                        // and if so, making sure we return a cloned version of its lambda.
                        if (Manager.WidgetAjaxEventStorage[idxWidget.ID, idxEventNameNode.Name] != null)
                            e.Args.FindOrInsert(idxWidget.ID).Add(Manager.WidgetAjaxEventStorage[idxWidget.ID, idxEventNameNode.Name].Clone());
                    }
                }
            }
        }

        /// <summary>
        ///     Changes or deletes the given Ajax event(s) for the given widget(s).
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.widgets.ajax-events.set")]
        public void set_widget_ajax_event (ApplicationContext context, ActiveEventArgs e)
        {
            // Sanity check.
            if (e.Args.Value == null || e.Args.Children.Count == 0)
                throw new LambdaException (
                    string.Format ("[{0}] needs both a value being widget(s) to iterate, and children arguments being events to retrieve", e.Args.Name),                    e.Args,                    context);

            // Iterating through all widget(s) requested by caller.
            foreach (var idxWidget in FindWidgets<Widget> (context, e.Args)) {

                // Iterating through each event(s) requested by caller, making sure we avoid formatting value.
                foreach (var idxEventNameNode in e.Args.Children.Where (ix => ix.Name != "")) {

                    // Checking if we should delete existing Ajax event.
                    if (idxEventNameNode.Children.Count == 0) {

                        // Deleting existing Ajax event.
                        Manager.WidgetAjaxEventStorage.Remove (idxWidget.ID, idxEventNameNode.Name);
                        idxWidget.DeleteAttribute (idxEventNameNode.Name);

                    } else {

                        // Setting widget's Ajax event to whatever we were given, making sure we clone the lambda supplied.
                        var clone = idxEventNameNode.Clone();

                        // Making sure updated Ajax event is parametrized with [_event] node.
                        clone.FindOrInsert ("_event", 0).Value = idxWidget.ID;
                        Manager.WidgetAjaxEventStorage[idxWidget.ID, idxEventNameNode.Name] = clone;

                        // Notice, since [oninit] is a special server-side event, we do not map it up as an Ajax event.
                        if (idxEventNameNode.Name != "oninit")
                            idxWidget [idxEventNameNode.Name] = "common_event_handler";
                    }
                }
            }
        }

        /// <summary>
        ///     Lists all existing Ajax events for given widget(s).
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.widgets.ajax-events.list")]
        public void p5_web_widgets_ajax_events_list (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Iterating through all widgets supplied by caller.
                foreach (var idxWidget in FindWidgets<Widget> (context, e.Args)) {

                    // Then iterating through all attribute keys, filtering everything out that does not start with "on", ".on" or "_on".
                    Node curNode = new Node (idxWidget.ID);
                    foreach (var idxAtr in idxWidget.AttributeKeys.Where (ix => ix.StartsWith ("on") || ix.StartsWith ("_on") || ix.StartsWith (".on"))) {

                        // Adding currently iterated Ajax event.
                        curNode.Add (idxAtr);
                    }

                    // Special handling of [oninit], since it never leaves the server, and is hence not in widget's attribute collection.
                    if (Manager.WidgetAjaxEventStorage [idxWidget.ID, "oninit"] != null)
                        curNode.Add ("oninit");

                    // Checking if we've got more than zero events for given widget, and if so, adding event node, containing list of events.
                    if (curNode.Children.Count > 0)
                        e.Args.Add (curNode);
                }
            }
        }

        /// <summary>
        ///     Raises the specified Ajax event(s) for specified widget(s).
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.widgets.ajax-events.raise")]
        public void p5_web_widgets_ajax_events_raise (ApplicationContext context, ActiveEventArgs e)
        {
            // Sanity check.
            if (e.Args.Value == null || e.Args.Children.Count == 0)
                throw new LambdaException (
                    string.Format ("[{0}] needs both a value being widget(s) to iterate, and children arguments being events to retrieve", e.Args.Name),
                    e.Args,
                    context);

            // Iterating through all widget(s) supplied by caller.
            foreach (var idxWidget in FindWidgets<Widget> (context, e.Args)) {

                // Iterating through all Ajax event(s) requested by caller.
                foreach (var idxEventNameNode in e.Args.Children) {

                    // Raising currently iterated Ajax event for currently iterated widget, letting p5.ajax do the heavy lifting.
                    idxWidget.InvokeEventHandler (idxEventNameNode.Name);
                }
            }
        }
    }
}
