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
    ///     Class encapsulating creation of web widgets
    /// </summary>
    public class WidgetCreator
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="p5.web.widgets.WidgetCreator"/> class
        /// </summary>
        /// <param name="page">Page.</param>
        public WidgetCreator (ApplicationContext context, PageManager manager)
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

        #region [ -- Active events for creating and deleting widgets -- ]

        /// <summary>
        ///     Creates a web widget of specified type, defaults to [container]
        /// </summary>
        /// <param name="context">Context for current request</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "create-widget", Protection = EventProtection.LambdaClosed)]
        [ActiveEvent (Name = "create-void-widget", Protection = EventProtection.LambdaClosed)]
        [ActiveEvent (Name = "create-literal-widget", Protection = EventProtection.LambdaClosed)]
        private void create_widget (ApplicationContext context, ActiveEventArgs e)
        {
            var splits = e.Name.Split('-');
            string type = splits.Length == 2 ? "container" : splits[1];
            CreateWidget (context, e.Args, type);
        }

        /// <summary>
        ///     Checks if the given widget(s) exist
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "widget-exist", Protection = EventProtection.LambdaClosed)]
        private void widget_exist (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new p5.core.Utilities.ArgsRemover(e.Args)) {

                // Looping through all IDs given
                foreach (var widgetId in XUtil.Iterate<string> (context, e.Args, true)) {

                    // Adding a boolean as result node, with widget's ID as name, indicating existence of widget
                    e.Args.Add(widgetId, Manager.FindControl<Control>(widgetId, Manager.AjaxPage) != null);
                }

                // Return true as main Active Event node if ALL widgets existed, otherwise returning false
                e.Args.Value = !e.Args.Children.Where(ix => !ix.Get<bool> (context)).GetEnumerator().MoveNext();
            }
        }

        /// <summary>
        ///     Deletes the given widget(s) entirely
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "delete-widget", Protection = EventProtection.LambdaClosed)]
        private void delete_widget (ApplicationContext context, ActiveEventArgs e)
        {
            // Looping through all IDs given
            foreach (var idxWidget in Manager.FindWidgets<Control> (context, e.Args, "delete-widget")) {

                // Removing widget
                RemoveWidget (context, e.Args, idxWidget);
            }
        }

        /// <summary>
        ///     Clears the given widget(s), removing all of its children widgets
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "clear-widget", Protection = EventProtection.LambdaClosed)]
        private void clear_widget (ApplicationContext context, ActiveEventArgs e)
        {
            // Looping through all IDs given
            foreach (var idxWidget in Manager.FindWidgets<Container> (context, e.Args, "clear-widget")) {

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
        private void CreateWidget (ApplicationContext context, Node args, string type)
        {
            // Finding parent widget first, which defaults to "main container" widget, if no parent is given
            var parent = Manager.FindControl<Widget>(args.GetChildValue ("parent", context, "cnt"), Manager.AjaxPage);

            // Creating our widget by raising the active event responsible for creating it
            var createNode = args.Clone ();
            if (createNode.Value == null) {

                // Creating a unique ID for widget BEFORE we create Widget, since we need it to create our Widget Events 
                // before we create widget itself, since [oninit] might depend upon widget events, and oninit 
                // is raised during creation process, before it returns.
                createNode.Value = Container.CreateUniqueId ();
            }

            // ORDER COUNTS!! Read above comments!

            // Initializing Active Events for widget, BEFORE widget is created, 
            // since [oninit] might depend upon Widget events for widget
            var eventNode = createNode.Find (idx => idx.Name == "events");
            if (eventNode != null)
                CreateWidgetLambdaEvents (createNode.Get<string> (context), eventNode.UnTie (), context);

            createNode.Insert (0, new Node ("__parent", parent));
            context.RaiseNative ("p5.web.widgets." + type, createNode);
        }

        /*
         * Creates local widget events for web widgets created through [create-widget]
         */
        private void CreateWidgetLambdaEvents (string widgetId, Node eventNode, ApplicationContext context)
        {
            // Looping through each event in args
            foreach (var idxEvt in eventNode.Children.ToList ()) {

                // Verifying Active Event is not protected
                if (!Manager.CanOverrideEventInLambda (context, idxEvt.Name))
                    throw new LambdaException(
                        string.Format ("You cannot override Active Event '{0}' since it is protected", idxEvt.Name),
                        eventNode,
                        context);

                // Adding lambda event to lambda event storage
                Manager.WidgetLambdaEventStorage [idxEvt.Name, widgetId] = idxEvt;
            }
        }

        /*
         * Helper to remove a widget from Page control collection
         */
        private void RemoveWidget (ApplicationContext context, Node args, Control widget)
        {
            // Basic logical error checking
            var parent = widget.Parent as Container;
            if (parent == null)
                throw new LambdaException (
                    "You cannot delete a widget who's parent is not a Phosphorus Five Ajax Container widget. Tried to delete; " + widget.ID + " which is of type " + widget.GetType().FullName,
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
