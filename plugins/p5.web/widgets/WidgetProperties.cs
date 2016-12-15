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
    ///     Class encapsulating retrieving, setting, deleting and listing properties of widgets.
    /// </summary>
    public class WidgetProperties : BaseWidget
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="p5.web.widgets.WidgetProperties"/> class.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="manager">PageManager owning this instance</param>
        public WidgetProperties (ApplicationContext context, PageManager manager)
            : base (context, manager)
        { }

        /// <summary>
        ///     Returns properties and/or attributes requested by caller as children nodes.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.widgets.property.get")]
        public void p5_web_widgets_property_get (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Making sure we fetch property list before we start modifying arguments.
                var propertyList = e.Args.Children.Where (ix => ix.Name != "").ToList ();

                // Looping through all widget caller requests to retrieve properties for.
                foreach (var idxWidget in FindWidgets<Widget> (context, e.Args)) {

                    // Looping through all properties requested by caller.
                    foreach (var idxProp in propertyList) {

                        // Invoking method which retrieves the currently iterated property/attribute value of currently iterated widget.
                        RetrieveWidgetProperty (e.Args, idxProp.Name, idxWidget);
                    }
                }
            }
        }

        /// <summary>
        ///     Sets properties and/or attributes of web widgets.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.widgets.property.set")]
        public void p5_web_widgets_property_set (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we fetch property list before we start modifying arguments.
            var propertyList = e.Args.Children.Where (ix => ix.Name != "").ToList ();

            // Looping through all widget IDs given by caller.
            foreach (var idxWidget in FindWidgets<Widget> (context, e.Args)) {

                // Looping through all properties requested by caller.
                foreach (var valueNode in propertyList) {
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
                        case "parent":
                        case "position":
                        case "before":
                        case "after":
                            throw new LambdaException ("You cannot change [parent], [position], [before] or [after], since these are read only after creation of widget", e.Args, context);
                        default:
                            idxWidget [valueNode.Name.StartsWith ("\\") ? valueNode.Name.Substring(1) : valueNode.Name] = valueNode.GetExValue<string> (context);
                            break;
                    }
                }
            }
        }

        /// <summary>
        ///     Deletes/removes the properties and/or attributes of one or more Ajax widgets.
        ///     Notice, since an attibute can be existing, with a null value, only relying upon "set", simply won't cut it for us.
        ///     Hence, we need an explicit delete attribute Active Event.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.widgets.property.delete")]
        public void p5_web_widgets_property_delete (ApplicationContext context, ActiveEventArgs e)
        {
            // Checking if caller even supplied any argument.
            if (e.Args.Value == null || e.Args.Children.Count == 0)
                return;

            // Looping through all widgets supplied by caller.
            foreach (var widget in FindWidgets<Widget> (context, e.Args)) {

                // Looping through each property to delete.
                foreach (var nameNode in e.Args.Children.Where (ix => ix.Name != "")) {

                    // Verifying property can legally be removed.
                    switch (nameNode.Name) {
                        case "parent":
                        case "position":
                        case "before":
                        case "after":
                        case "visible":
                        case "element":
                        case "render-type":
                        case "id":
                            throw new LambdaException ("Cannot remove property '" + nameNode.Name + "' of widget", e.Args, context);
                        default:
                            widget.RemoveAttribute (nameNode.Name.StartsWith ("\\") ? nameNode.Name.Substring(1) : nameNode.Name);
                            break;
                    }
                }
            }
        }

        /// <summary>
        ///     Lists all properties for given Ajax widget(s).
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.widgets.property.list")]
        public void p5_web_widgets_property_list (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new p5.core.Utilities.ArgsRemover (e.Args, true)) {

                // Looping through all widgets.
                foreach (var widget in FindWidgets<Widget> (context, e.Args)) {

                    // Creating our "return node" for currently handled widget.
                    Node curNode = e.Args.Add (widget.ID).LastChild;

                    // First listing special properties.
                    if (!widget.Visible)
                        curNode.Add ("visible", false);
                    curNode.Add ("element", widget.Element);

                    // Then the generic attributes.
                    foreach (var idxAtr in widget.AttributeKeys) {

                        // Dropping the Element property and all events, except events that are JavaScript client side attributes.
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
        ///     Recursively retrieves properties from widgets, and their descendant widgets, specified by caller.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.web.widgets.properties.get")]
        public void p5_web_widgets_properties_get (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Retrieving list of properties to retrieve, before we start modifying arguments.
                var list = e.Args.Children.Where (ix => ix.Name != "").ToList ();

                // Looping through all widget IDs given by caller.
                foreach (var idxWidget in FindWidgets<Widget> (context, e.Args)) {

                    // Serialize currently iterated widgets, and all descendants of given widget, yielding all properties requested by caller.
                    SerializeWidgetPropertiesRecursively (context, e.Args, list, idxWidget);
                }
            }
        }

        /*
         * Returns as a child of args the specified property's value for specified widget.
         */
        private void RetrieveWidgetProperty (Node args, string propertyName, Widget widget)
        {
            // Checking if this is a generic attribute, or a special property.
            switch (propertyName) {
                case "visible":
                    CreateAttributeReturn (args, propertyName, widget, widget.Visible);
                    break;
                case "element":
                    CreateAttributeReturn (args, propertyName, widget, widget.Element);
                    break;
                case "parent":
                    CreateAttributeReturn (args, propertyName, widget, widget.Parent.ID);
                    break;
                case "position":
                    CreateAttributeReturn (args, propertyName, widget, widget.Parent.Controls.IndexOf (widget));
                    break;
                case "before":
                    var indexBefore = widget.Parent.Controls.IndexOf (widget) + 1;
                    CreateAttributeReturn (args,
                                          propertyName,
                                          widget,
                                          indexBefore >= widget.Parent.Controls.Count ? null : widget.Parent.Controls [indexBefore].ID);
                    break;
                case "after":
                    var indexAfter = widget.Parent.Controls.IndexOf (widget) - 1;
                    CreateAttributeReturn (args,
                                          propertyName,
                                          widget,
                                          indexAfter < 0 ? null : widget.Parent.Controls [indexAfter].ID);
                    break;
                case "render-type":
                    CreateAttributeReturn (args, propertyName, widget, widget.RenderType.ToString ());
                    break;
                default:

                    // This is a generic attribute.
                    CreateAttributeReturn (args, propertyName, widget);
                    break;
            }
        }

        /*
         * Ensures [oninit] is evaluated for widget, and all of widgets' descendant widgets.
         */
        private void EnsureOnInit (ApplicationContext context, Widget widget)
        {
            // Making sure this is a widget.
            if (widget != null) {

                // Checking if widget has an [oninit].
                var onInitNode = Manager.WidgetAjaxEventStorage[widget.ID, "oninit"];
                if (onInitNode != null) {

                    // [oninit] should be evaluated now.
                    var clone = onInitNode.Clone ();
                    clone.FindOrInsert ("_event", 0).Value = widget.ID;
                    context.Raise ("eval", clone);
                }

                // Recursively ensuring [oninit] for children widgets.
                foreach (var idxCtrl in widget.Controls) {
                    EnsureOnInit (context, idxCtrl as Widget);
                }
            }
        }

        /*
         * Recursively retrieves all specified values from widget, and widget's descendants.
         */
        private void SerializeWidgetPropertiesRecursively (
            ApplicationContext context, 
            Node args, 
            List<Node> list,
            Widget widget)
        {
            // Checking if we even have a widget.
            if (widget == null)
                return;

            // Then looping through all properties requested by caller.
            foreach (var idxProp in list) {

                // Invoking method which retrieves the currently iterated property/attribute value of currently iterated widget.
                RetrieveWidgetProperty (args, idxProp.Name, widget);
            }

            // Looping through all children widgets, recursively, invoking "self" for each child control.
            foreach (Control idxCtrl in widget.Controls) {
                SerializeWidgetPropertiesRecursively (context, args, list, idxCtrl as Widget);
            }
        }

        /*
         * Helper for RetrieveWidgetProperty, creates a return value for one normal attribute.
         */
        private static void CreateAttributeReturn (Node node, string name, Widget widget, object value = null)
        {
            // Fetching property name.
            var propertyName = name.StartsWith ("\\") ? name.Substring (1) : name;

            // Checking if widget has the attribute, if it doesn't, we don't even add any return nodes at all, to make it possible
            // to separate widgets which has the property, but no value, (such as the selected property on checkboxes for instance),
            // and widgets that does not have the property at all.
            if (value == null && !widget.HasAttribute (propertyName))
                return;

            // Making sure we skip server-side Ajax events.
            if ((propertyName.StartsWith ("on") || propertyName.StartsWith ("_on") || propertyName.StartsWith(".on")) && widget [propertyName] == "common_event_handler")
                return;

            // Returning specified value, unless no value is given, at which case we return the attribute value from widget.
            node.FindOrInsert (widget.ID).Add (name).LastChild.Value = value == null ? 
                widget [propertyName] : 
                value;
        }
    }
}
