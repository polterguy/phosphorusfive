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

using System;
using System.Linq;
using System.Web.UI;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.ajax.widgets;
using p5.exp.exceptions;

namespace p5.web.widgets
{
    /// <summary>
    ///     Class encapsulating Active Events that somehow retrieves Ajax widgets.
    /// </summary>
    public class WidgetRetriever : BaseWidget
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="p5.web.widgets.WidgetRetriever"/> class.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="manager">PageManager owning this instance</param>
        public WidgetRetriever (ApplicationContext context, PageManager manager)
            : base (context, manager)
        { }

        /// <summary>
        ///     Returns the ID and type of the given widget(s)' parent widget(s).
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.widgets.get-parent")]
        public void p5_web_widgets_get_parent (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Looping through all IDs given.
                foreach (var idxWidget in FindWidgets <Control> (context, e.Args)) {

                    // Returning parent of widget, and parent's typename.
                    e.Args.Add (idxWidget.ID).LastChild.Add (GetTypeName (idxWidget.Parent), idxWidget.Parent.ID);
                }
            }
        }

        /// <summary>
        ///     Returns the ID and type of the given widget(s)' children widgets.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.widgets.get-children")]
        public void p5_web_widgets_get_children (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Looping through all IDs given.
                foreach (var idxWidget in FindWidgets <Control> (context, e.Args)) {

                    // Adding currently iterated widget's ID.
                    var widgetResultNode = e.Args.Add (idxWidget.ID).LastChild;

                    // Then looping through currently iterated widget's children, adding them typename and ID.
                    foreach (Control idxCtrl in idxWidget.Controls) {

                        // Adding type of widget as name, and ID as value
                        widgetResultNode.Add (GetTypeName (idxCtrl), idxCtrl.ID);
                    }
                }
            }
        }

        /// <summary>
        ///     Find widget(s) according to criteria, and returns their ID(s).
        ///     Like version does not require exact match for criteria, only that value of widget property/attribute contains specified criteria.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.widgets.find")]
        [ActiveEvent (Name = "p5.web.widgets.find-like")]
        [ActiveEvent (Name = "p5.web.widgets.find-first")]
        [ActiveEvent (Name = "p5.web.widgets.find-first-like")]
        public void p5_web_widgets_find (ApplicationContext context, ActiveEventArgs e)
        {
            // Sanity check.
            if (e.Args.Children.Count (ix => ix.Name != "") == 0)
                throw new LambdaException (
                    string.Format ("[{0}] requires at least one child argument, being some property/attribute to look for", e.Name), 
                    e.Args, 
                    context);

            // Making sure we clean up and remove all arguments passed in after execution.
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Iterating through each argument supplied, making sure it has "cnt" as a default value, if no explicit start widget(s) are given.
                var list = new List<string> ();
                if (e.Args.Value != null)
                    list.AddRange (XUtil.Iterate<string> (context, e.Args));
                else
                    list.Add ("cnt"); // Defaulting to "cnt" main root widget of application pool.

                // Setting up a result node, to avoid modifying our args before entire iteration is finished.
                // Notice, if we hadn't done this, then each match would create a new (bogus) criteria, meaning there could never be more than one match.
                Node retVal = new Node ();

                // Iterating each starting widget.
                foreach (var idxStartWidgetId in list) {

                    // Retrieving start widget from where to start searching for widgets matching criteria.
                    var startControl = FindControl<Widget> (idxStartWidgetId, Manager.AjaxPage);

                    // Retrieving all widgets having properties matching whatever criteria are supplied.
                    foreach (var idxWidget in FindWidgetsMatchingCriteria (e.Args, startControl, context, e.Name.Contains ("-like"), false)) {

                        // Adding type of widget as name, and ID of widget as value.
                        retVal.FindOrInsert (idxStartWidgetId).Add (GetTypeName (idxWidget), idxWidget.ID);

                        // Checking if caller only wants one result for each starting widget.
                        if (e.Name.Contains ("-first"))
                            break;
                    }
                }
                e.Args.AddRange (retVal.Children);
            }
        }

        /// <summary>
        ///     Finds first ancestor widget matching given criteria.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.widgets.find-ancestor")]
        [ActiveEvent (Name = "p5.web.widgets.find-ancestor-like")]
        [ActiveEvent (Name = "p5.web.widgets.find-first-ancestor")]
        [ActiveEvent (Name = "p5.web.widgets.find-first-ancestor-like")]
        public void p5_web_widgets_find_ancestor (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Sanity check.
                if (e.Args.Children.Count (ix => ix.Name != "") == 0)
                    throw new LambdaException ("No criteria submitted to [" + e.Name + "]", e.Args, context);
                if (e.Args.Value == null)
                    throw new LambdaException ("No starting widget submitted to [" + e.Name + "]", e.Args, context);

                // Setting up a result node, to avoid modifying our args before all iteration is finished
                // Notice, if we hadn't done this, then each match would create a new (bogus) criteria, meaning there could never be more than one match.
                Node retVal = new Node ();

                // Iterating start widget(s) to start searching for criteria amongst ancestor widgets.
                foreach (var idxStartWidgetId in XUtil.Iterate<string> (context, e.Args)) {

                    // Retrieving start widget from where to start searching for widgets matching criteria.
                    var startControl = FindControl<Widget> (idxStartWidgetId, Manager.AjaxPage);

                    // Retrieving all widgets having properties matching whatever criteria are supplied.
                    foreach (var idxWidget in FindWidgetsMatchingCriteria (e.Args, startControl, context, e.Name.Contains ("-like"), true)) {

                        // Adding type of widget as name, and ID of widget as value.
                        retVal.FindOrInsert (idxStartWidgetId).Add (GetTypeName (idxWidget), idxWidget.ID);

                        // Checking if caller only wants one result for each starting widget.
                        if (e.Name.Contains ("-first"))
                            break;
                    }
                }
                e.Args.AddRange (retVal.Children);
            }
        }

        /// <summary>
        ///     Lists all widgets on page, optionally having or containing the specified string(s) as their ID(s).
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.widgets.list")]
        [ActiveEvent (Name = "p5.web.widgets.list-like")]
        public void p5_web_widgets_list (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Retrieving filter, if any.
                var filter = XUtil.Iterate<string>(context, e.Args).ToList ();
                if ((e.Args.Value != null || e.Args.Children.Count > 0) && filter.Count == 0)
                    return; // Possibly a filter expression, leading into oblivion.

                // Recursively retrieving all widgets on page, matching filter, or all widgets, if there are no filters.
                ListWidgets (filter, e.Args, Manager.AjaxPage, e.Name == "p5.web.widgets.list");
            }
        }

        /// <summary>
        ///     Returns true for the specified widget(s), if it exists.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.widgets.exists")]
        public void p5_web_widgets_exists (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Looping through all IDs given.
                foreach (var widgetId in XUtil.Iterate<string> (context, e.Args)) {

                    // Adding a boolean as result node, with specified ID as name, and boolean value, indicating if widget exists or not.
                    e.Args.Add (widgetId, FindControl<Widget> (widgetId, Manager.AjaxPage) != null);
                }
            }
        }

        /*
         * Recursively traverses entire Control hierarchy on page, and adds up into result node, optionally if a filter is given, widgets
         * must have ID matching one of the specified filter(s), unless exactMatch is false, at which point the ID must simply "contain"
         * the string from one of the specified filters.
         */
        private static void ListWidgets (
            List<string> filter, 
            Node args, 
            Control ctrl,
            bool exactMatch)
        {
            // Checking if current ctrl is mathing filters supplied, alternative there are no filters, at which point everything should match.
            if (filter.Count == 0 || filter.Count (ix => (exactMatch ? ix == ctrl.ID : (ctrl.ID != null && ctrl.ID.Contains (ix)))) > 0) {

                // Current control is matching one of our filters, or there are no filters.
                // Making sure control is a widget, before we return it to caller.
                if (ctrl is Widget)
                    args.Add (GetTypeName (ctrl), ctrl.ID);
            }

            // Iterating through each child control of current control, recursively invoking "self" to check for match amongst descendants.
            foreach (Control idxChildCtrl in ctrl.Controls) {

                // Recursively invoking "self".
                ListWidgets (filter, args, idxChildCtrl, exactMatch);
            }
        }

        /*
         * Helper to retrieve a list of widgets from a Node that serves as criteria.
         * TODO: Refactor, way too long ...!!
         */
        private IEnumerable<Widget> FindWidgetsMatchingCriteria (
            Node criteria, 
            Control startControl, 
            ApplicationContext context,
            bool likeMatch,
            bool upwardsInAncestors)
        {
            // Checking if current ctrl Widget, at which point we can check for match against criteria.
            var widget = startControl as Widget;
            if (widget != null) {

                // Looping through each criteria, to check for match.
                // Notice, ALL specified criteria must match for control to yield a match.
                // If "like" is true, then it will look for a "like" condition, meaning not exact match of criteria, but rather 
                // if attribute "contains" value of criteria.
                bool match = true;
                foreach (var idxCriteria in criteria.Children) {

                    // Checking if this is a "special property".
                    if (idxCriteria.Name == "element") {

                        // Retrieving element name, and doing some basic sanity checks.
                        var value = idxCriteria.GetExValue<string> (context, null);
                        if (value == null)
                            throw new LambdaException ("You cannot query for 'element exists', since all widgets have the 'element' property.", criteria, context);

                        // Checking if this is a "like" match, at which case element name only needs to "contain" specified criteria.
                        match = likeMatch ? widget.Element.Contains (value) : match = widget.Element == value;

                    } else if (idxCriteria.Name == "visible") {

                        // Basic sanity check.
                        if (idxCriteria.Value == null)
                            throw new LambdaException ("You cannot query for 'visible exists', since all widgets have the 'visible' property.", criteria, context);

                        // Matching widget, only if visibility property equals criteria.
                        var value = idxCriteria.GetExValue (context, true);
                        match = widget.Visible == value;

                    } else if (idxCriteria.Name == "parent") {

                        // Matching widget, only if parent's ID equals criteria.
                        var value = idxCriteria.GetExValue<string> (context, null);

                        // Basic sanity check.
                        if (value == null)
                            throw new LambdaException ("You cannot query for 'parent exists', since all widgets have a parent property.", criteria, context);

                        match = likeMatch ? (widget.Parent.ID ?? "").Contains (value) : widget.Parent.ID == value;

                    } else if (idxCriteria.Name == "position") {

                        // Matching widget, only if parent's ID equals criteria.
                        var value = idxCriteria.GetExValue (context, -1);

                        // Basic sanity check.
                        if (value == -1)
                            throw new LambdaException ("You cannot query for 'position exists', since all widgets have a position property.", criteria, context);

                        match = widget.Parent.Controls.IndexOf (widget) == value;

                    } else if (idxCriteria.Name == "before") {

                        // Matching widget, only if widget immediately after matches criteria with its ID.
                        var value = idxCriteria.GetExValue<string> (context, null);

                        // Basic sanity check.
                        if (value == null)
                            throw new LambdaException ("You cannot query for 'before exists', since (allmost) all widgets have another widget after them.", criteria, context);

                        var indexOfWidget = widget.Parent.Controls.IndexOf (widget);
                        if (indexOfWidget + 1 >= widget.Parent.Controls.Count)
                            match = false;
                        else
                            match = likeMatch ? 
                                (widget.Parent.Controls [indexOfWidget + 1].ID ?? "").Contains (value) : 
                                 widget.Parent.Controls [indexOfWidget + 1].ID == value;

                    } else if (idxCriteria.Name == "after") {

                        // Matching widget, only if widget immediately after matches criteria with its ID.
                        var value = idxCriteria.GetExValue<string> (context, null);

                        // Basic sanity check.
                        if (value == null)
                            throw new LambdaException ("You cannot query for 'after exists', since (allmost) all widgets have another widget before them.", criteria, context);

                        var indexOfWidget = widget.Parent.Controls.IndexOf (widget);
                        if (indexOfWidget == 0)
                            match = false;
                        else
                            match = likeMatch ?                                (widget.Parent.Controls [indexOfWidget - 1].ID ?? "").Contains (value) :                                 widget.Parent.Controls [indexOfWidget - 1].ID == value;

                    } else if (idxCriteria.Name == "render-type") {

                        // Matching widget, only if widget immediately after matches criteria with its ID.
                        var value = idxCriteria.GetExValue<string> (context, null);

                        // Basic sanity check.
                        if (value == null)
                            throw new LambdaException ("You cannot query for 'render-type exists', since all widgets have some sort of render-type.", criteria, context);

                        match = widget.RenderAs == (Widget.Rendering)Enum.Parse (typeof (Widget.Rendering), value);

                    } else if (!widget.HasAttribute (idxCriteria.Name)) {

                        // Since widget doesn't even have this attribute, there's no way this can possibly be a match.
                        match = false;

                    } else {

                        // Retrieving value of criteria node.
                        var value = idxCriteria.GetExValue<string> (context, null);
                        if (value != null && !likeMatch && widget [idxCriteria.Name] != value) {
                            match = false;
                        } else if (value != null && likeMatch && !widget [idxCriteria.Name].Contains (value)) {
                            match = false;
                        }
                    }

                    // Checking if currently iterated criteria did not yield a match, at which point checking the next criteria is meaningless.
                    if (!match)
                        break;
                }

                // Checking if criteria search above gave us a match.
                if (match)
                    yield return widget; // We have a match!
            }

            // Checking if we're supposed to iterate "upwards" amongst ancestors, or downwards into descendants.
            if (upwardsInAncestors) {

                // Moving upwards amongst ancestors.
                if (startControl.Parent != null) {
                    foreach (var idxParentCtrl in FindWidgetsMatchingCriteria (criteria, startControl.Parent, context, likeMatch, true)) {
                        yield return idxParentCtrl;
                    }
                }
            } else {

                // Iterating descendants widgets, looking for matches.
                foreach (Control idxChildCtrl in startControl.Controls) {

                    // Recursively invoking "self" with currently iterated child control.
                    foreach (var idxSubFind in FindWidgetsMatchingCriteria (criteria, idxChildCtrl, context, likeMatch, upwardsInAncestors)) {
                        yield return (idxSubFind as Widget);
                    }
                }
            }
        }

        /*
         * Returns typename of specified ctrl.
         */
        protected static string GetTypeName (Control ctrl)
        {
            // Adding type of widget as name, and ID as value
            string typeName = ctrl.GetType ().FullName;

            // Making sure we return "condensed typename" if widget type is from p5.ajax, meaning one of the standard types.
            if (ctrl is Widget)
                typeName = typeName.Substring (typeName.LastIndexOf (".") + 1).ToLower ();

            // Returning typename to caller.
            return typeName;
        }
    }
}
