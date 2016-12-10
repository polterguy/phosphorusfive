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
using System.Web.UI;
using System.Collections;
using System.Collections.Generic;
using p5.core;
using p5.ajax.widgets;
using p5.exp.exceptions;

namespace p5.web.widgets
{
    /// <summary>
    ///     Class encapsulating creation of web widgets
    /// </summary>
    public class WidgetCreator : BaseWidget
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="p5.web.widgets.WidgetCreator"/> class
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="manager">PageManager owning this instance</param>
        public WidgetCreator (ApplicationContext context, PageManager manager)
            : base (context, manager)
        { }

        #region [ -- Active events for creating and deleting widgets -- ]

        /// <summary>
        ///     Creates a web widget of specified type, defaults to [container]
        /// </summary>
        /// <param name="context">Context for current request</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.widgets.create")]
        [ActiveEvent (Name = "p5.web.widgets.create-container")]
        [ActiveEvent (Name = "p5.web.widgets.create-void")]
        [ActiveEvent (Name = "p5.web.widgets.create-literal")]
        public void p5_web_widgets_create (ApplicationContext context, ActiveEventArgs e)
        {
            // Figuring out which type of widget we're creating
            var splits = e.Name.Split('-');
            string type = splits.Length == 1 ? "container" : splits[1];

            // Creating widget of specified type
            e.Args.Value = CreateWidget (context, e.Args, type);
        }

        /// <summary>
        ///     Deletes the given widget(s) entirely
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.widgets.delete")]
        public void p5_web_widgets_delete (ApplicationContext context, ActiveEventArgs e)
        {
            // Looping through all IDs given
            foreach (var idxWidget in FindWidgets<Control> (context, e.Args, "p5.web.widgets.delete")) {

                // Removing widget
                RemoveWidget (context, e.Args, idxWidget);
            }
        }

        /// <summary>
        ///     Clears the given widget(s), removing all of its children widgets
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.widgets.clear")]
        public void p5_web_widgets_clear (ApplicationContext context, ActiveEventArgs e)
        {
            // Looping through all IDs given
            foreach (var idxWidget in FindWidgets<Container> (context, e.Args, "p5.web.widgets.clear")) {

                // Then looping through all of its children controls
                foreach (Control idxChildWidget in new ArrayList(idxWidget.Controls)) {

                    // Removing widget from page control collection
                    RemoveWidget (context, e.Args, idxChildWidget);
                }
            }
        }

        #endregion

        #region [ -- Private helper methods -- ]

        /*
         * Helper method for creating widgets
         */
        private string CreateWidget (
            ApplicationContext context, 
            Node args, 
            string type)
        {
            // Return value (becomes ID of control)
            string retVal = null;

            // Sanity check, only one of [parent], [before] or [after] can be supplied
            // And [position] can only be supplied if [parent] is supplied
            if (args ["parent"] != null) {
                if (args ["after"] != null || args ["before"] != null)
                    throw new LambdaException ("Supply only one of [parent], [after] or [before]", args, context);
            } else if (args ["after"] != null) {
                if (args ["parent"] != null || args ["before"] != null)
                    throw new LambdaException ("Supply only one of [parent], [after] or [before]", args, context);
                if (args ["position"] != null)
                    throw new LambdaException ("[position] can only be supplied with [parent]", args, context);
            } else if (args ["before"] != null) {
                if (args ["after"] != null || args ["parent"] != null)
                    throw new LambdaException ("Supply only one of [parent], [after] or [before]", args, context);
                if (args ["position"] != null)
                    throw new LambdaException ("[position] can only be supplied with [parent]", args, context);
            }

            // Finding parent widget first, which defaults to "main container" widget, if no parent is given
            var parent = FindControl<Widget>(args.GetChildValue ("parent", context, "cnt"), Manager.AjaxPage);
            var before = FindControl<Widget>(args.GetChildValue<string> ("before", context, null), Manager.AjaxPage);
            var after = FindControl<Widget>(args.GetChildValue<string>   ("after", context, null), Manager.AjaxPage);

            // Creating our widget by raising the active event responsible for creating it
            var createNode = args.Clone ();
            if (createNode.Value == null) {

                // Creating a unique ID for widget BEFORE we create Widget, since we need it to create our Widget Events 
                // before we create widget itself, since [oninit] might depend upon widget events, and oninit 
                // is raised during creation process, before it returns.
                retVal = Container.CreateUniqueId ();
                createNode.Value = retVal;
            } else {

                // Making sure we return retVal regardless of whether or not 'auto-ID' was chosen
                retVal = createNode.Get<string> (context);
            }

            // ORDER COUNTS!! Read above comments!

            // Initializing Active Events for widget, BEFORE widget is created, 
            // since [oninit] might depend upon Widget events for widget
            var eventNode = createNode.Children.FirstOrDefault (ix => ix.Name == "events");
            if (eventNode != null)
                CreateWidgetLambdaEvents (createNode.Get<string> (context), eventNode.UnTie (), context);

            createNode.Insert (0, new Node ("_parent", parent));
            if (before != null)
                createNode ["before"].Value = before;
            if (after != null)
                createNode ["after"].Value = after;
            context.Raise (".p5.web.widgets." + type, createNode);

            // Getting [oninit], if any, for entire hierarchy, and invoking each of them, in "breadth first" order
            // Notice that since this is done AFTER creation of entire hierarchy, then each [oninit] can invoke dynamically 
            // created widget lambda events, declared in completely different widgets, both up and down hierarchy. Which
            // completely eliminates any "cohesion" problems, besides remembering that all [oninit] events are raised in 
            // "breadth first" order though. FUCK THE PAGE LIFECYCLE!
            // Notice, we only run [oninit] if the widget is actually visible.
            // Otherwise, we defer [oninit] until the widget becomes visible.
            foreach (var idxOnInit in GetInitMethods (createNode)) {

                // Invoking currently iterated [oninit] lambda event
                var widget = idxOnInit.Parent.Get<Widget> (context);
                if (widget.Visible) {
                    idxOnInit.Insert (0, new Node ("_event", widget.ID));
                    context.Raise ("eval", idxOnInit.Clone ());
                }
            }

            // Making sure we return ID of control
            return retVal;
        }

        /*
         * Creates local widget events for web widgets created through [p5.web.widgets.create]
         */
        private void CreateWidgetLambdaEvents (
            string widgetId, 
            Node eventNode, 
            ApplicationContext context)
        {
            // Looping through each event in args
            foreach (var idxEvt in eventNode.Children.ToList ()) {

                // Adding lambda event to lambda event storage
                Manager.WidgetLambdaEventStorage [idxEvt.Name, widgetId] = idxEvt;
            }
        }

        /*
         * Retrieves all [oninit] lambda events for widget hierarchy, in "breadth first" order
         */
        private IEnumerable<Node> GetInitMethods (Node args)
        {
            // This is a literal/container/void/create-x-widget node, looping through all children to
            // find any potential [oninit] lambda events
            foreach (var idxNode in args.Children.Where (ix => ix.Name == "oninit")) {

                // And we have a MATCH! (this is an [oninit] lambda event
                yield return idxNode;
            }

            // Checking if currently iterated widget has children widgets
            if (args ["widgets"] != null) {

                // Looping through all child widgets of currently handled widget
                foreach (var idxNode in args["widgets"].Children) {

                    // Checking if this is a "custom control", at which point we should iterate its children again.
                    if (idxNode.Name.Contains (".")) {

                        // Recursively invoking "self" to retrieve [oninit] of children's children widgets of currently handled widget
                        // Notice "ToList", which makes sure enumeration doesn't halt when finding our first candidate
                        foreach (var idxInnerFirst in idxNode.Children.ToList ()) {
                            foreach (var idxInnerSecond in GetInitMethods (idxInnerFirst).ToList ()) {

                                // And we have a MATCH! (this is an [oninit] lambda event, found in recursive invocation of "self"
                                yield return idxInnerSecond;
                            }
                        }
                    } else {

                        // Recursively invoking "self" to retrieve [oninit] of children widgets of currently handled widget
                        // Notice "ToList", which makes sure enumeration doesn't halt when finding our first candidate
                        foreach (var idxInner in GetInitMethods (idxNode).ToList ()) {

                            // And we have a MATCH! (this is an [oninit] lambda event, found in recursive invocation of "self"
                            yield return idxInner;
                        }
                    }
                }
            }
        }

        /*
         * Helper to remove a widget from Page control collection
         */
        private void RemoveWidget (
            ApplicationContext context, 
            Node args, 
            Control widget)
        {
            // Basic logical error checking
            var parent = widget.Parent as Container;
            if (parent == null)
                throw new LambdaException (
                    "You cannot delete a widget who's parent is not a Phosphorus Five Ajax Container widget",
                    args,
                    context);

            // Removing all events, both "lambda" and "ajax"
            RemoveAllEventsRecursive (widget);

            // Removing widget itself from page control collection, making sure we persist the change to parent Container
            parent.RemoveControlPersistent (widget);
        }

        /*
         * Removing all events for widget recursively
         */
        private void RemoveAllEventsRecursive (Control widget)
        {
            // Checking if currently iterated Control is Widget, since only Widgets have
            // ajax and lambda events
            if (widget is Widget) {

                // Removing all Ajax Events for widget
                Manager.WidgetAjaxEventStorage.RemoveFromKey1(widget.ID);

                // Removing all lambda events for widget
                Manager.WidgetLambdaEventStorage.RemoveFromKey2(widget.ID);
            }

            // Recursively invoking "self" for all children widgets
            foreach (Widget idxChildWidget in widget.Controls) {

                // Removing all events for currently iterated child
                RemoveAllEventsRecursive(idxChildWidget);
            }
        }

        #endregion
    }
}
