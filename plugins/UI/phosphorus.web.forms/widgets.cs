/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using System;
using phosphorus.core;
using phosphorus.ajax.widgets;

namespace phosphorus.web.forms
{
    /// <summary>
    /// class for creating web widgets
    /// </summary>
    public static class widgets
    {
        /// <summary>
        /// creates a container type of widget, that can have children widgets
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.widgets.panel")]
        private static void pf_web_controls_panel (ApplicationContext context, ActiveEventArgs e)
        {
            Container widget = CreateControl<Container> (e.Args, "div");
            e.Args.Value = DecorateWidget (context, widget, e.Args);
        }

        /// <summary>
        /// creates a label type of widget, that can have static text as content
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.widgets.label")]
        private static void pf_web_controls_label (ApplicationContext context, ActiveEventArgs e)
        {
            Literal widget = CreateControl<Literal> (e.Args, "p", Widget.RenderingType.NoClose);
            e.Args.Value = DecorateWidget (context, widget, e.Args);
        }

        /// <summary>
        /// creates a button type of widget, that is a natural clickable element. the button can have either [value] or children controls
        /// through [controls], but not both
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.widgets.button")]
        private static void pf_web_controls_button (ApplicationContext context, ActiveEventArgs e)
        {
            Widget widget;
            string value = GetNodeValue (e.Args, "innerHTML");
            if (!string.IsNullOrEmpty (value)) {

                // creating a Literal widget since [value] was passed in
                widget = CreateControl<Literal> (e.Args, "button", Widget.RenderingType.Default);
            } else {

                // no [value] was given, hence we create a Container widget, in case button contains children controls
                widget = CreateControl<Container> (e.Args, "button", Widget.RenderingType.Default);
            }
            e.Args.Value = DecorateWidget (context, widget, e.Args);
        }

        /// <summary>
        /// creates a check-box type of widget, that can be checked and unchecked
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.widgets.checkbox")]
        private static void pf_web_controls_checkbox (ApplicationContext context, ActiveEventArgs e)
        {
            phosphorus.ajax.widgets.Void widget = CreateControl<phosphorus.ajax.widgets.Void> (e.Args, "input");
            widget ["type"] = "checkbox";
            widget ["name"] = e.Args.Get<string> ();
            e.Args.Value = DecorateWidget (context, widget, e.Args);
        }

        /// <summary>
        /// creates a radio button type of widget, that can be checked and unchecked, in groups of other similar radio buttons
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.widgets.radio")]
        private static void pf_web_controls_radio (ApplicationContext context, ActiveEventArgs e)
        {
            phosphorus.ajax.widgets.Void widget = CreateControl<phosphorus.ajax.widgets.Void> (e.Args, "input");
            widget ["type"] = "radio";
            widget ["name"] = e.Args.Get<string> ();
            e.Args.Value = DecorateWidget (context, widget, e.Args);
        }

        /// <summary>
        /// creates a select type of widget, that can have the user select one of many items from a drop down list of items
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.widgets.select")]
        private static void pf_web_controls_select (ApplicationContext context, ActiveEventArgs e)
        {
            Container widget = CreateControl<Container> (e.Args, "select");
            e.Args.Value = DecorateWidget (context, widget, e.Args);
        }

        /// <summary>
        /// creates an option value for a select list widget. parent must be [select]
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.widgets.option")]
        private static void pf_web_controls_option (ApplicationContext context, ActiveEventArgs e)
        {
            if (!(e.Args [0].Value is Container) && e.Args [0].Get<Container> ().ElementType == "select")
                throw new ArgumentException ("you cannot have an [option] widget outside a [select] widget");
            Literal widget = CreateControl<Literal> (e.Args, "option");
            widget.NoIDAttribute = true;
            e.Args.Value = DecorateWidget (context, widget, e.Args);
        }

        /// <summary>
        /// creates a hidden input type of widget, that can store values in your forms, with no visual interface
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.widgets.hidden")]
        private static void pf_web_controls_hidden (ApplicationContext context, ActiveEventArgs e)
        {
            phosphorus.ajax.widgets.Void widget = CreateControl<phosphorus.ajax.widgets.Void> (e.Args, "input");
            widget ["type"] = "hidden";
            widget ["name"] = widget.ID;
            e.Args.Value = DecorateWidget (context, widget, e.Args);
        }

        /// <summary>
        /// creates a text input type of widget, that can have the user type single line text values into it
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.widgets.text")]
        private static void pf_web_controls_text (ApplicationContext context, ActiveEventArgs e)
        {
            phosphorus.ajax.widgets.Void widget = CreateControl<phosphorus.ajax.widgets.Void> (e.Args, "input");
            widget ["type"] = "text";
            widget ["name"] = widget.ID;
            e.Args.Value = DecorateWidget (context, widget, e.Args);
        }

        /// <summary>
        /// creates a text area input type of widget, that can have the user type multiple lines of text values into it
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.widgets.textarea")]
        private static void pf_web_controls_textarea (ApplicationContext context, ActiveEventArgs e)
        {
            Literal widget = CreateControl<Literal> (e.Args, "textarea");
            widget ["name"] = widget.ID;
            e.Args.Value = DecorateWidget (context, widget, e.Args);
        }

        /// <summary>
        /// creates an image type of control, that will display an image on the form
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.widgets.img")]
        private static void pf_web_controls_img (ApplicationContext context, ActiveEventArgs e)
        {
            phosphorus.ajax.widgets.Void widget = CreateControl<phosphorus.ajax.widgets.Void> (e.Args, "img");
            e.Args.Value = DecorateWidget (context, widget, e.Args);
        }

        /// <summary>
        /// creates a generic void type of Widget, that will not allow for any content, but only attributes
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.widgets.void")]
        private static void pf_web_controls_void (ApplicationContext context, ActiveEventArgs e)
        {
            phosphorus.ajax.widgets.Void widget = CreateControl<phosphorus.ajax.widgets.Void> (e.Args, "void");
            e.Args.Value = DecorateWidget (context, widget, e.Args);
        }

        /// <summary>
        /// creates a generic literal type of Widget, that will allow for only innerHTML and no children controls
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.widgets.literal")]
        private static void pf_web_controls_literal (ApplicationContext context, ActiveEventArgs e)
        {
            Literal widget = CreateControl<Literal> (e.Args, "literal");
            e.Args.Value = DecorateWidget (context, widget, e.Args);
        }

        /// <summary>
        /// creates a generic containerr type of Widget, that will only allow for child controls, and not content besides that
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.widgets.container")]
        private static void pf_web_controls_container (ApplicationContext context, ActiveEventArgs e)
        {
            Container widget = CreateControl<Container> (e.Args, "container");
            e.Args.Value = DecorateWidget (context, widget, e.Args);
        } // TODO: create UserControl type of widget, that allows for linking in pf.lambda files, or something .......

        /*
         * returns the "name" children value from the node given
         */
        private static string GetNodeValue (Node node, string name)
        {
            foreach (Node idx in node.Children) {
                if (idx.Name == name)
                    return idx.Get<string> ();
            }
            return null;
        }

        /*
         * creates a widget from the given node
         */
        private static T CreateControl<T> (Node node, string elementType, Widget.RenderingType type = Widget.RenderingType.Default) where T : Widget, new()
        {
            // creating widget as persistent control
            Container parent = node [0].Get<Container> ();
            T widget = parent.CreatePersistentControl<T> (node.Get<string> ());

            // in case no ID was given, we "return" it to creator as value of current node
            if (node.Value == null)
                node.Value = widget.ID;

            // setting ElementType (html element) of Widget
            widget.ElementType = elementType;

            // setting rendering type (no closing element, void or default)
            if (type != Widget.RenderingType.Default)
                widget.RenderType = type;

            // returning widget to caller
            return widget;
        }

        /*
         * decorates widget with common properties
         */
        private static Widget DecorateWidget (ApplicationContext context, Widget widget, Node args)
        {
            // looping through all children nodes of Widget's node to decorate Widget
            foreach (Node idxArg in args.Children) {
                switch (idxArg.Name) {
                case "checked":
                    SetChecked (widget, idxArg.Get<string> ());
                    break;
                case "noid":
                    if (idxArg.Get<bool> ())
                        widget.NoIDAttribute = true;
                    break;
                case "render-type":
                    SetRenderType (widget, idxArg.Get<string> ());
                    break;
                case "element":
                    SetElementType (widget, idxArg.Get<string> ());
                    break;
                case "controls":
                    CreateChildWidgets (context, widget, idxArg);
                    break;
                default:

                    // this might be an event, it might be a node we should ignore (starting with "_") or it might be any arbitrary attribute
                    // we should render. HandleDefaultProperty will figure out
                    HandleDefaultProperty (context, widget, idxArg);
                    break;
                }
            }
            return widget;
        }

        /*
         * sets the checked property of widget
         */
        private static void SetChecked (Widget widget, string checkedValue)
        {
            if (checkedValue == "checked")
                widget ["checked"] = null;
        }

        /*
         * sets the rendering type of Widget
         */
        private static void SetRenderType (Widget widget, string renderType)
        {
            widget.RenderType = (Widget.RenderingType)Enum.Parse (typeof(Widget.RenderingType), renderType);
        }

        /*
         * changes the ElementType of the Widget
         */
        private static void SetElementType (Widget widget, string elementType)
        {
            widget.ElementType = elementType;
        }
        
        /*
         * creates children widgets of widget
         */
        private static void CreateChildWidgets (ApplicationContext context, Widget widget, Node children)
        {
            foreach (Node idxChild in children.Children) {
                idxChild.Insert (0, new Node ("__parent", widget));
                context.Raise ("pf.web.widgets." + idxChild.Name, idxChild);
            }
        }
        
        /*
         * handles all default properties of Widget
         */
        private static void HandleDefaultProperty (ApplicationContext context, Widget widget, Node node)
        {
            if (node.Name.StartsWith ("on")) {
                CreateEventHandler (context, widget, node);
            } else if (!node.Name.StartsWith ("_")) {
                widget [node.Name] = node.Get<string> ();
            }
        }

        /*
         * creates an event handler on the given widget for the given node
         */
        private static void CreateEventHandler (ApplicationContext context, Widget widget, Node node)
        {
            if (node.Value != null) {

                // javascript code to be executed
                widget [node.Name] = node.Get<string> ();
            } else {

                // creating our pf.lambda object, and invoking our [pf.web.add-widget-event] Active Event to map the
                // ajax event to our pf.lambda object, passing in the widget that contains the Ajax Event
                Node eventNode = new Node (string.Empty, widget);
                eventNode.Add (node.Clone ());

                // making sure [_form-id] and [_widget-id] is passed into pf.lambda object as parameters
                eventNode [0].Insert (0, new Node ("_form-id", node.Root.Value));
                eventNode [0].Insert (1, new Node ("_widget-id", node.Parent.Value));

                // raising the Active Event that actually creates our ajax event handler for our pf.lambda object
                context.Raise ("pf.web.add-widget-event", eventNode);
            }
        }
    }
}
