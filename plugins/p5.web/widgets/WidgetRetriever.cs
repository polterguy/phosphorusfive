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
    ///     Class encapsulating retrieving web widgets
    /// </summary>
    public class WidgetRetriever
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="p5.web.widgets.WidgetRetriever"/> class
        /// </summary>
        /// <param name="page">Page.</param>
        public WidgetRetriever (ApplicationContext context, PageManager manager)
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

        #region [ -- Active Events for retrieving widgets -- ]

        /// <summary>
        ///     Returns the ID and type of the given widget's parent(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-parent-widget", Protection = EventProtection.LambdaClosed)]
        private void get_parent_widget (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new p5.core.Utilities.ArgsRemover (e.Args)) {

                // Looping through all IDs given
                foreach (var idxWidget in Manager.FindWidgets <Control> (context, e.Args, "get-parent-widget")) {

                    // Finding parent and returning type as Name and ID as Value
                    var parent = idxWidget.Parent;
                    string type = parent.GetType().FullName;

                    // Checking if type is from p5 Ajax, and if so, returning "condensed" typename
                    if (parent is Widget)
                        type = type.Substring(type.LastIndexOf(".") + 1).ToLower();
                    e.Args.Add(type, parent.ID);
                }
            }

            // Making sure we set value of main event node to widget's ID if only one widget was requested
            if (e.Args.Count == 1)
                e.Args.Value = e.Args.FirstChild.Value;
            else
                e.Args.Value = null; // Making sure we remove arguments if there are multiple return values
        }

        /// <summary>
        ///     Returns the ID and type of the given widget's children
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-children-widgets", Protection = EventProtection.LambdaClosed)]
        private void get_children_widgets (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new p5.core.Utilities.ArgsRemover (e.Args, true)) {

                // Looping through all IDs given
                foreach (var idxWidget in Manager.FindWidgets <Control> (context, e.Args, "get-children-widgets")) {

                    // Adding currently iterated widget's ID
                    e.Args.Add(idxWidget.ID);

                    // Then looping through currently iterated widget's children, adding them
                    foreach (Control idxCtrl in idxWidget.Controls) {

                        // Adding type of widget as name, and ID as value
                        string type = idxCtrl.GetType().FullName;

                        // Making sure we return "condensed typename" if widget type is from p5 Ajax
                        if (idxCtrl is Widget)
                            type = type.Substring(type.LastIndexOf(".") + 1).ToLower();
                        e.Args.LastChild.Add(type, idxCtrl.ID);
                    }
                }
            }
        }

        /// <summary>
        ///     Find widget(s) according to criteria underneath an (optionally) declared widget, and returns its ID
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "find-widget", Protection = EventProtection.LambdaClosed)]
        private void find_widget (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new p5.core.Utilities.ArgsRemover (e.Args)) {

                // Retrieving where to start looking
                var root = Manager.FindControl<Widget> (e.Args.GetExValue (context, "cnt"), Manager.AjaxPage);

                // Retrieving all controls having properties matching whatever arguments supplied
                foreach (var idxWidget in FindWidgetsBy <Widget> (e.Args, root, context)) {

                    // Adding type of widget as name, and ID as value
                    string type = idxWidget.GetType().FullName;

                    // Making sure we return "condensed typename" if widget type is from p5 Ajax
                    if (idxWidget is Widget)
                        type = type.Substring(type.LastIndexOf(".") + 1).ToLower();
                    e.Args.Add (type, idxWidget.ID);
                }
            }

            // Making sure we set value of main event node to widget's ID if only one widget was found
            if (e.Args.Count == 1)
                e.Args.Value = e.Args.FirstChild.Value;
            else
                e.Args.Value = null; // Making sure we remove arguments if there are multiple return values
        }

        /// <summary>
        ///     Lists all widgets on page
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-widgets", Protection = EventProtection.LambdaClosed)]
        private void list_widgets (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new p5.core.Utilities.ArgsRemover (e.Args, true)) {

                // Retrieving filter, if any
                var filter = XUtil.Iterate<string>(context, e.Args).ToList ();
                if (e.Args.Value != null && filter.Count == 0)
                    return; // Possibly a filter expression, leading into oblivion

                // Recursively retrieving all widgets on page
                ListWidgets(filter, e.Args, Manager.AjaxPage);
            }
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

        #endregion

        #region [ -- Private helper methods -- ]

        /*
         * Recursively traverses entire Control hierarchy on page, and adds up into result node
         */
        private static void ListWidgets (List<string> filter, Node args, Control current)
        {
            if (filter.Count == 0 || filter.Count(ix => ix == current.ID) > 0) {

                // Current Control is matching one of our filters, or there are no filters
                string typeString = current.GetType().FullName;
                if (current is Widget)
                    typeString = typeString.Substring(typeString.LastIndexOf(".") + 1).ToLower();

                args.Add(typeString, current.ID);
            }
            foreach (Control idxChild in current.Controls) {

                // Recursively invoking "self"
                ListWidgets (filter, args, idxChild);
            }
        }

        /*
         * Helper to retrieve a list of widgets from a Node that serves as criteria
         */
        private IEnumerable<T> FindWidgetsBy<T> (Node args, Widget idx, ApplicationContext context) where T : Widget
        {
            if (idx == null)
                yield break;

            bool match = true;
            foreach (var idxNode in args.Children) {
                if (!idx.HasAttribute(idxNode.Name) || idx[idxNode.Name] != idxNode.GetExValue<string>(context, null)) {
                    match = false;
                    break;
                }
            }
            if (match)
                yield return idx as T;
            foreach (var idxChild in idx.Controls) {
                foreach (var idxSubFind in FindWidgetsBy<T> (args, idxChild as T, context)) {
                    yield return idxSubFind;
                }
            }
        }

        #endregion
    }
}
