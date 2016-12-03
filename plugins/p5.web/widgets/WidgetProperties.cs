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

namespace p5.web.widgets
{
    /// <summary>
    ///     Class encapsulating properties of widgets
    /// </summary>
    public class WidgetProperties : BaseWidget
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="p5.web.widgets.WidgetProperties"/> class
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="manager">PageManager owning this instance</param>
        public WidgetProperties (ApplicationContext context, PageManager manager)
            : base (context, manager)
        { }

        #region [ -- Widget property getters and setters -- ]

        /// <summary>
        ///     Returns properties and/or attributes requested by caller as children nodes
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-widget-property")]
        public void get_widget_property (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Looping through all widget IDs given by caller
                foreach (var idxWidget in FindWidgets<Widget> (context, e.Args, "get-widget-property")) {

                    // Looping through all properties requested by caller
                    foreach (var nameNode in e.Args.Children.Where (ix => ix.Name != "").ToList ()) {

                        // Checking if this is a generic attribute, or a specific property
                        switch (nameNode.Name) {
                        case "visible":
                            CreatePropertyReturn (e.Args, nameNode.Name, idxWidget, idxWidget.Visible);
                            break;
                        case "element":
                            CreatePropertyReturn (e.Args, nameNode.Name, idxWidget, idxWidget.Element);
                            break;
                        case "render-type":
                            CreatePropertyReturn (e.Args, nameNode.Name, idxWidget, idxWidget.RenderType.ToString ());
                            break;
                        default:
                            if (!string.IsNullOrEmpty (nameNode.Name))
                                CreatePropertyReturn (e.Args, nameNode.Name, idxWidget);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Sets properties and/or attributes of web widgets
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "set-widget-property")]
        public void set_widget_property (ApplicationContext context, ActiveEventArgs e)
        {
            // Looping through all widget IDs given by caller
            foreach (var idxWidget in FindWidgets<Widget> (context, e.Args, "set-widget-property")) {

                // Looping through all properties requested by caller
                foreach (var valueNode in e.Args.Children.Where (ix => ix.Name != "")) {
                    switch (valueNode.Name) {
                    case "visible":
                            var newValue = valueNode.GetExValue<bool> (context);
                            var oldValue = idxWidget.Visible;
                            idxWidget.Visible = newValue;
                            if (newValue && !oldValue) {

                                // Evaluating [oninit] recursively for widget, and all children widgets.
                                EnsureOnInit (context, idxWidget);
                            }
                            break;
                    case "element":
                        idxWidget.Element = valueNode.GetExValue<string> (context);
                        idxWidget.ReRender ();
                        break;
                    case "render-type":
                        idxWidget.RenderType = (Widget.RenderingType) Enum.Parse (typeof (Widget.RenderingType), valueNode.GetExValue<string> (context));
                        idxWidget.ReRender ();
                        break;
                    case "id":
                        var oldID = idxWidget.ID;
                        var newID = valueNode.GetExValue<string> (context);
                        if (string.IsNullOrEmpty (newID)) {

                            // Widget's new id was null or empty, creating a new random unique ID for widget
                            newID = Container.CreateUniqueId ();
                        }
                        idxWidget.ID = newID;
                        if (oldID != newID) {
                            Manager.WidgetAjaxEventStorage.ChangeKey1 (context, oldID, newID);
                            Manager.WidgetLambdaEventStorage.ChangeKey2 (oldID, newID);
                        }
                        break;
                    default:
                        idxWidget [valueNode.Name.StartsWith ("\\") ? valueNode.Name.Substring(1) : valueNode.Name] = valueNode.GetExValue<string> (context);
                        break;
                    }
                }
            }
        }

        /*
         * Ensures [oninit] is evaluated for widget, and all children widgets.
         */
        private void EnsureOnInit (ApplicationContext context, Widget widget)
        {
            // Making sure this is a widget.
            if (widget != null) {

                // Checking if widget has an [oninit].
                var onInitNode = Manager.WidgetAjaxEventStorage[widget.ID, "oninit"];
                if (onInitNode != null) {

                    // [oninit] should be evaluated now!
                    var clone = onInitNode.Clone ();
                    clone.Insert (0, new Node ("_event", widget.ID));
                    context.Raise ("eval", clone.Clone ());
                }

                // Recursively ensuring [oninit] for children widgets.
                foreach (var idxCtrl in widget.Controls) {
                    EnsureOnInit (context, idxCtrl as Widget);
                }
            }
        }

        /// <summary>
        ///     Removes the properties and/or attributes of web widgets
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "delete-widget-property")]
        public void delete_widget_property (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value == null || e.Args.Children.Count == 0)
                return; // Nothing to do here ...

            // Looping through all widgets supplied by caller
            foreach (var widget in FindWidgets<Widget> (context, e.Args, "delete-widget-property")) {

                // Looping through each property to remove
                foreach (var nameNode in e.Args.Children.Where (ix => ix.Name != "")) {

                    // Verifying property can be removed
                    switch (nameNode.Name) {
                    case "visible":
                    case "invisible-element":
                    case "element":
                    case "render-type":
                        throw new LambdaException ("Cannot remove property '" + nameNode.Name + "' of widget", e.Args, context);
                    default:
                        widget.RemoveAttribute (nameNode.Name.StartsWith ("\\") ? nameNode.Name.Substring(1) : nameNode.Name);
                        break;
                    }
                }
            }
        }

        /// <summary>
        ///     Lists all existing properties for given web widget
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-widget-properties")]
        public void list_widget_properties (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new p5.core.Utilities.ArgsRemover (e.Args, true)) {

                // Looping through all widgets
                foreach (var widget in FindWidgets<Widget> (context, e.Args, "list-widget-properties")) {

                    // Creating our "return node" for currently handled widget
                    Node curNode = e.Args.Add (widget.ID).LastChild;

                    // First listing "static properties"
                    if (!widget.Visible)
                        curNode.Add ("visible", false);
                    curNode.Add ("element", widget.Element);

                    // Then the generic attributes
                    foreach (var idxAtr in widget.AttributeKeys) {

                        // Dropping the Tag property and all events, except events that are "JavaScript declarations"
                        if (idxAtr == "Element" || 
                            idxAtr == "id" ||
                            ((idxAtr.StartsWith ("on") || idxAtr.StartsWith ("_on") || idxAtr.StartsWith(".on")) && widget [idxAtr] == "common_event_handler"))
                            continue;
                        curNode.Add (idxAtr, widget [idxAtr]);
                    }
                }
            }
        }

        /// <summary>
        ///     Recursively retrieves properties from widgets specified by caller.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-widget-properties")]
        public void get_widget_properties (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Looping through all widget IDs given by caller
                foreach (var idxWidget in FindWidgets<Widget> (context, e.Args, "get-widget-properties")) {

                    // Serialize currently iterated widgets, and all descendants of given widget, yielding all properties requested by caller.
                    SerializeWidgetPropertiesRecursively (context, e.Args, idxWidget);
                }
            }
        }

        #endregion

        #region [ -- Private helper methods -- ]

        /*
         * Recursively retrieves all values from form element widget descendants from given widget
         */
        private static void SerializeWidgetPropertiesRecursively (ApplicationContext context, Node args, Widget widget)
        {
            // Checking if we even have a widget
            if (widget == null)
                return;

            // Then looping through all additional properties requested by caller, making sure we don't invalidate our IEnumerable.
            foreach (var idxNode in args.Children.Where (ix => ix.Name != "").ToList ()) {
                CreatePropertyReturn (args, idxNode.Name, widget);
            }

            // Looping through all children widgets, recursively
            foreach (Control idxCtrl in widget.Controls) {
                SerializeWidgetPropertiesRecursively (context, args, idxCtrl as Widget);
            }
        }

        /*
         * Helper for [get-widget-property], creates a return value for one property
         */
        private static void CreatePropertyReturn (Node node, string name, Widget widget, object value = null)
        {
            var propertyName = name.StartsWith ("\\") ? name.Substring (1) : name;
            // Checking if widget has the attribute, if it doesn't, we don't even add any return nodes at all, to make it possible
            // to separate widgets which has the property, but no value, (such as the selected property on checkboxes for instance),
            // and widgets that does not have the property at all
            if (value == null && !widget.HasAttribute (propertyName))
                return;

            if ((propertyName.StartsWith ("on") || propertyName.StartsWith ("_on") || propertyName.StartsWith(".on")) && widget [propertyName] == "common_event_handler")
                return; // Skipping these guys

            node.FindOrInsert (widget.ID).Add (name).LastChild.Value = value == null ? 
                widget [propertyName] : 
                value;
        }
        #endregion
    }
}
