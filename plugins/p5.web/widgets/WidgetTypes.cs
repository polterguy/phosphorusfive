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
using p5.exp;
using p5.core;
using p5.ajax.widgets;
using p5.exp.exceptions;
using Void = p5.ajax.widgets.Void;

namespace p5.web.widgets
{
    /// <summary>
    ///     Class encapsulating creation of Ajax widgets.
    /// </summary>
    public class WidgetTypes : BaseWidget
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="p5.web.widgets.WidgetRetriever"/> class.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="manager">PageManager owning this instance</param>
        public WidgetTypes (ApplicationContext context, PageManager manager)
            : base (context, manager)
        { }

        /// <summary>
        ///     Creates an Ajax container widget.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.web.widgets.container")]
        public void _p5_web_controls_container (ApplicationContext context, ActiveEventArgs e)
        {
            CreateWidget<Container> (context, e.Args, "div");
        }

        /// <summary>
        ///     Creates an Ajax literal widget.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.web.widgets.literal")]
        public void _p5_web_controls_literal (ApplicationContext context, ActiveEventArgs e)
        {
            CreateWidget<Literal> (context, e.Args, "p");
        }

        /// <summary>
        ///     Creates an Ajax void widget.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.web.widgets.void")]
        public void _p5_web_controls_void (ApplicationContext context, ActiveEventArgs e)
        {
            CreateWidget<Void> (context, e.Args, "input");
        }

        /// <summary>
        ///     Creates a simple text widget, non ajax, non-control, simply inline HTML content.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.web.widgets.text")]
        public void _p5_web_controls_text (ApplicationContext context, ActiveEventArgs e)
        {
            // Creating widget as persistent control
            var parent = e.Args.GetChildValue<Container> ("_parent", context);
            var position = e.Args.GetChildValue ("position", context, -1);

            var ctrl = new LiteralControl ();
            ctrl.Text = e.Args.Get<string> (context);
            parent.Controls.AddAt (position, ctrl);
        }

        /*
         * Creates a widget from the given node, and inserts it into the specified [_parent] widget in args.
         */
        private void CreateWidget<T> (
            ApplicationContext context, 
            Node args, 
            string elementType) where T : Widget, new ()
        {
            switch (args.Name) {
                case "container":
                case "literal":
                case "void":
                case "create-widget":
                case "create-void-widget":
                case "create-literal-widget":
                case "create-container-widget":
                case "p5.web.widgets.create":
                case "p5.web.widgets.create-void":
                case "p5.web.widgets.create-literal":
                case "p5.web.widgets.create-container":
                    break;
                default:
                    elementType = args.Name;
                    break;
            }
            // Figuring out which container widget is the created widget's parent, and which position we should inject the widget at.
            var parent = args.GetChildValue<Container> ("_parent", context);
            var position = args.GetExChildValue ("position", context, -1);

            // retrieving ID if any, and making sure another widget doesn't exists from before on page with the same ID.
            var id = args.GetExValue<string> (context, null) ?? Container.CreateUniqueId ();
            if (!string.IsNullOrEmpty (id) && FindControl<Control> (id, Manager.AjaxPage) != null)
                throw new LambdaException ("A widget with the same ID already exists on page.", args, context);

            // Creating widget as persistent control, making sure we atomically create widget, in case an exception occurs inwards in hierarchy.
            var widget = parent.CreatePersistentControl<T> (id, position);

            // Making sure we return ID after creation to caller.
            args.Value = id;
            try {

                // Setting element type, and decorating properties/children-widgets/attributes/events/etc ...
                widget.Element = elementType;
                DecorateWidget (context, widget, args);

            } catch {

                // Removing widget from parent, to make operation "atomic", before we rethrow exception.
                parent.Controls.Remove (widget);
                throw;
            }
        }

        /*
         * Decorates widget with common properties/arguments.
         */
        private void DecorateWidget (ApplicationContext context, Widget widget, Node args)
        {
            // Looping through all properties/arguments of widget, figuring out what to do with them.
            foreach (var idxArg in args.Children) {

                switch (idxArg.Name) {
                    case "visible":
                        widget.Visible = idxArg.GetExValue<bool> (context);
                        break;
                    case "element":
                        widget.Element = idxArg.GetExValue<string> (context);
                        break;
                    case "widgets":
                        CreateChildWidgets (context, widget, idxArg);
                        break;
                    case "events":
                        CreateWidgetLambdaEvents (context, widget, idxArg);
                        break;
                    case "_parent":
                    case "position":
                    case "parent":
                    case "before":
                    case "after":
                        // Skipping these buggers, since they're handled elsewhere, being "special properties".
                        break;
                    case "oninit":

                        // Adding lambda event to lambda event storage.
                        Manager.WidgetAjaxEventStorage [widget.ID, "oninit"] = idxArg.Clone ();
                        break;
                    default:

                        // This might be an event, or an arbitrary attribute.
                        HandleDefaultProperty (context, widget, idxArg);
                        break;
                }
            }
        }

        /*
         * Creating children widgets of widget.
         */
        private void CreateChildWidgets (ApplicationContext context, Widget widget, Node children)
        {
            // Looping through all children widgets.
            foreach (var idxChild in children.Children) {

                // Figuring out type of widget to create.
                switch (idxChild.Name) {
                    case "literal":
                    case "container":
                    case "void":
                    case "text":

                        // Sanity check, before invoking "create native widget" event.                        if (idxChild.Children.Count (ix => ix.Name == "parent" || ix.Name == "_parent" || ix.Name == "before" || ix.Name == "after" || ix.Name == "position") > 0)
                            throw new LambdaException ("You cannot specify [parent], [_parent], [before], [after] or [position] for a child widget", idxChild, context);
                        idxChild.Insert (0, new Node ("_parent", widget));
                        try {
                            context.RaiseEvent (".p5.web.widgets." + idxChild.Name, idxChild);
                        } finally {
                            idxChild ["_parent"].UnTie ();
                        }
                        break;
                    default:

                        // Checking if this is a "custom widget", which it is, if name contains a ".".
                        if (idxChild.Name.IndexOf (".") > 0) {

                            // Making sure we store the ID for widget, since lambda event invocation will delete it after evaluation.
                            var id = idxChild.Value;
                            context.RaiseEvent (idxChild.Name, idxChild);
                            if (idxChild.Children.Count != 1)
                                throw new LambdaException ("Custom widget event did not return exactly one child value", idxChild, context);

                            // Making sure we decorate the ID for widget automatically.
                            idxChild.FirstChild.Value = id;

                            // Recursively invoking self, with the children widget, that should now be declared as the first child node of idxChild.
                            CreateChildWidgets (context, widget, idxChild);

                        } else {

                            // This is the "HTML element" helper syntax, declaring the element of the widget as the node's name.
                            // Checking type of widget, before we invoke creation event.
                            idxChild.Insert (0, new Node ("_parent", widget));
                            try {
                                if (idxChild ["innerValue"] != null) {
                                    context.RaiseEvent (".p5.web.widgets.literal", idxChild);
                                } else if (idxChild ["widgets"] != null) {
                                    context.RaiseEvent (".p5.web.widgets.container", idxChild);
                                } else {
                                    context.RaiseEvent (".p5.web.widgets.void", idxChild);
                                }
                            } finally {
                                idxChild ["_parent"].UnTie ();
                            }
                        }
                        break;
                }
            }
        }

        /*
         * Creates lambda events for Widget.
         */
        private void CreateWidgetLambdaEvents (ApplicationContext context, Widget widget, Node events)
        {
            // Looping through all events for widget, and registering them as widget lambda events.
            foreach (var idxEvt in events.Children) {

                // Making sure there actually is a lambda for currently iterated event.
                if (idxEvt.Children.Count (ix => ix.Name != "") > 0) {

                    // Cloning lambda and inserting [_event] node, before storing its lambda in lambda event storage for page.
                    var clone = idxEvt.Clone ();
                    clone.Insert (0, new Node ("_event", widget.ID));
                    Manager.WidgetLambdaEventStorage [idxEvt.Name, widget.ID] = clone;
                }
            }
        }

        /*
         * Handles all default properties of widget.
         */
        private void HandleDefaultProperty (ApplicationContext context, Widget widget, Node node)
        {
            // Checking if this is a declaration of an event handler, either "visible" or "invisible server-side".
            if (node.Name.StartsWith ("on") || node.Name.StartsWith ("_on") || node.Name.StartsWith(".on")) {

                // This is an event, creating it.
                CreateEventHandler (context, widget, node);

            } else {

                // This is a normal attribute, making sure we escape attribute if necessary.
                widget [node.Name.StartsWith ("\\") ? node.Name.Substring(1) : node.Name] = node.GetExValue<string> (context);
            }
        }

        /*
         * Creates an event handler on the given widget for the given node.
         * If a value is given in node, the event will be assumed to be a JavaScript event, and simply sent back to client as JavaScript.
         * If node does not contain a value, the event will be handled as a server-side lambda event, assuming children nodes is lambda to evaluate.
         */
        private void CreateEventHandler (ApplicationContext context, Widget widget, Node lambda)
        {
            // Checking what type of event this is, JavaScript or server-side lambda.
            if (lambda.Value != null) {

                // Javascript code to be executed.
                widget [lambda.Name] = lambda.GetExValue<string> (context);

            } else {

                // Mapping the widget's ajax event to our common event handler on page.
                // But intentionally dropping [oninit], since it's a purely server-side lambda event, and not supposed to be pushed to client.
                // Making sure we clone lambda given.
                var clone = lambda.Clone ();
                if (clone.Name != "oninit")
                    widget [clone.Name] = "common_event_handler"; // Name of common WebMethod from page.

                // Making sure event has a reference to ID of widget in its body as the first child node's value.
                clone.Insert (0, new Node ("_event", widget.ID));

                // Storing our Widget Ajax event in storage.
                Manager.WidgetAjaxEventStorage [widget.ID, lambda.Name] = clone;
            }
        }
    }
}
