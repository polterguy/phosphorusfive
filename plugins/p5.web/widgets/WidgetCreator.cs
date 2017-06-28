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
using p5.core;
using p5.exp;
using p5.ajax.widgets;
using p5.exp.exceptions;

namespace p5.web.widgets
{
    /// <summary>
    ///     Class encapsulating creation of Ajax widgets.
    /// </summary>
    public class WidgetCreator : BaseWidget
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="WidgetCreator"/> class.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="manager">PageManager owning this instance</param>
        internal WidgetCreator (ApplicationContext context, PageManager manager)
            : base (context, manager)
        { }

        /// <summary>
        ///     Creates an Ajax widget of specified type.
        /// </summary>
        /// <param name="context">Context for current request</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "create-widget")]
        [ActiveEvent (Name = "create-void-widget")]
        [ActiveEvent (Name = "create-literal-widget")]
        [ActiveEvent (Name = "create-container-widget")]
        [ActiveEvent (Name = "p5.web.widgets.create")]
        [ActiveEvent (Name = "p5.web.widgets.create-void")]
        [ActiveEvent (Name = "p5.web.widgets.create-literal")]
        [ActiveEvent (Name = "p5.web.widgets.create-container")]
        public void p5_web_widgets_create (ApplicationContext context, ActiveEventArgs e)
        {
            // Basic sanity check.
            if (e.Args ["_parent"] != null)
                throw new LambdaException ("[_parent] is a restricted property name", e.Args, context);

            // Figuring out which type of widget we're creating.
            string type = null;
            switch (e.Name) {
                case "create-void-widget":
                case "p5.web.widgets.create-void":
                    type = "void";
                    break;
                case "create-literal-widget":
                case "p5.web.widgets.create-literal":
                    type = "literal";
                    break;
                case "create-container-widget":
                case "p5.web.widgets.create-container":
                    type = "container";
                    break;
                case "create-widget":
                case "p5.web.widgets.create":

                    // Generic versions, figuring out type according to whether or not [innerValue] or [widgets] exists as arguments.
                    if (e.Args.Children.Any (ix => ix.Name == "innerValue"))
                        type = "literal";
                    else if (e.Args.Children.Any (ix => ix.Name == "widgets"))
                        type = "container";
                    else
                        type = "void";
                    break;
            }

            // Creating widget of specified type.
            CreateWidget (context, e.Args, type);
        }

        /// <summary>
        ///     Creates one or more Ajax widgets according to its children.
        /// </summary>
        /// <param name="context">Context for current request</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "create-widgets")]
        [ActiveEvent (Name = "p5.web.widgets.create-many")]
        public void p5_web_widgets_create_many (ApplicationContext context, ActiveEventArgs e)
        {
            CreateManyWidgets (context, e.Args);
        }

        /*
         * Helper for above.
         */
        private void CreateManyWidgets (ApplicationContext context, Node args)
        {
            // Looping through each argument, assuming each is a widget creation statement.
            foreach (var idxWidget in XUtil.Iterate<Node> (context, args)) {
                switch (idxWidget.Name) {
                case "literal":
                    CreateWidget (context, idxWidget, "literal");
                    break;
                case "container":
                    CreateWidget (context, idxWidget, "container");
                    break;
				case "void":
					CreateWidget (context, idxWidget, "void");
					break;
				case "text":
					CreateWidget (context, idxWidget, "text");
					break;
				default:

					// Making sure we store the ID for widget, since lambda event invocation will delete it after evaluation.
					var id = idxWidget.Value;

                    // Making sure we remove all special properties for widget, before we invoke custom Active Event, to avoid confusion.
                    var list = idxWidget.Children.Where (ix => ix.Name == "parent" || ix.Name == "position" || ix.Name == "before" || ix.Name == "after").Select (ix => ix.UnTie ());

                    // Raising extension widget event.
					context.RaiseEvent (idxWidget.Name, idxWidget);
					if (idxWidget.Count != 1)
						throw new LambdaException ("Custom widget event did not return exactly one child value", idxWidget, context);

					// Making sure we decorate the ID for widget automatically.
					idxWidget.FirstChild.Value = id;

                    // Passing in the list of properties extracted above, into the child widget, which should now be decorated according to the custom extension widget event.
                    idxWidget.FirstChild.AddRange (list); 

                    // Recursively invoking self for simplicity.
                    CreateManyWidgets (context, idxWidget);
					break;
				}
            }
        }

        /*
         * Helper method for creating widgets.
         */
        void CreateWidget (
            ApplicationContext context, 
            Node args, 
            string type)
        {
            // Retrieving [position] and [parent]/[before]/[after] widget, and doing some basic sanity check inside of GetParent.
            int position;
            Widget parent = GetParentWidget (context, args, out position);

            // Creating a unique ID for our widget, unless an explicitly ID was specified during invocation.
            if (args.Value == null)
                args.Value = Container.CreateUniqueId ();

            // Making sure widget creation Active Event has access to parent widget, in such a way that we can also clean up after ourselves afterwards.
            args.Insert (0, new Node ("_parent", parent));
            Node insertedPos = null;
            try {
                if (position != -1 && args ["position"] == null)
                    insertedPos = args.Add ("position", position).LastChild;
                context.RaiseEvent (".p5.web.widgets." + type, args);

            } finally {

                // House cleaning.
                args ["_parent"].UnTie ();
                insertedPos?.UnTie ();
            }

            // Making sure we invoke our [oninit] lambdas, but only if widget's parent widget is visible.
            if (parent.Visible) {
                foreach (var idxInit in GetInitLambdas (args, context)) {

                    // Invoking currently iterated [oninit] lambda.
                    var clone = idxInit.Clone ();
                    clone.Insert (0, new Node ("_event", idxInit.Parent.Value));
                    context.RaiseEvent ("eval", clone);
                }
            }
        }

        /*
         * Returns the parent widget where we should create our widget, according to arguments specified.
         */
        Widget GetParentWidget (ApplicationContext context, Node args, out int index)
        {
            if (args ["before"] != null) {

                // Sanity check, before returning parent widget.
                if (args ["parent"] != null || args ["after"] != null || args ["position"] != null)
                    throw new LambdaException ("Specify exactly one of [parent], [before] or [after].", args, context);

                // Returning index of [before] as index and parent of [before] as widget.
                var widget = FindControl<Widget> (args.GetExChildValue ("before", context, "cnt"), Manager.AjaxPage);
                index = widget.Parent.Controls.IndexOf (widget);
                return widget.Parent as Widget;


            }

            if (args ["after"] != null) {

                // Sanity check, before returning parent widget.
                if (args ["parent"] != null || args ["position"] != null)
                    throw new LambdaException ("Specify exactly one of [parent], [before] or [after].", args, context);

                // Returning index of [after] + 1 as index and parent of [after] as widget.
                var widget = FindControl<Widget> (args.GetExChildValue ("after", context, "cnt"), Manager.AjaxPage);
                index = widget.Parent.Controls.IndexOf (widget) + 1;
                return widget.Parent as Widget;

            }

            // Returning [position] and [parent] widget.
            index = args.GetExChildValue ("position", context, -1);
            return FindControl<Widget> (args.GetExChildValue ("parent", context, "cnt"), Manager.AjaxPage);
        }

        /*
         * Retrieves all [oninit] lambda events for visible widgets in hierarchy, breadth first, and returns them to caller.
         */
        IEnumerable<Node> GetInitLambdas (Node args, ApplicationContext context)
        {
            // Checking if widget is invisible, at which point we're supposed to return early.
            if (!args.GetExChildValue ("visible", context, true))
                yield break;

            // Checking if widget has an [oninit] lambda.
            if (args ["oninit"] != null)
                yield return args ["oninit"];

            // Checking if currently iterated widget has children widgets, since its children might also have [oninit] lambdas.
            if (args ["widgets"] != null) {
                foreach (var idxNode in args["widgets"].Children) {

                    // Notice, we need to recursively find the first widget which is not a "custom widget", hence the while loop.
                    var cur = idxNode;
                    while (cur != null && cur.Name.Contains ("."))
                        cur = cur.FirstChild;
                    foreach (var idxInnerSecond in GetInitLambdas (cur, context)) {
                        yield return idxInnerSecond;
                    }
                }
            }
        }
    }
}
