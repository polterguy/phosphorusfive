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
            widget.innerHTML = GetNodeValue (e.Args, "value");
            e.Args.Value = DecorateWidget (context, widget, e.Args);
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
         * decorates widget with commons properties
         */
        private static Widget DecorateWidget (ApplicationContext context, Widget widget, Node args)
        {
            foreach (Node idxArg in args.Children) {
                switch (idxArg.Name) {
                case "controls":
                    foreach (Node idxChild in idxArg.Children) {
                        idxChild.Insert (0, new Node ("__parent", widget));
                        context.Raise ("pf.web.widgets." + idxChild.Name, idxChild);
                    }
                    break;
                case "class":
                    widget ["class"] = idxArg.Get<string> ();
                    break;
                case "style":
                    widget ["style"] = idxArg.Get<string> ();
                    break;
                case "element":
                    widget.ElementType = idxArg.Get<string> ();
                    break;
                case "render-type":
                    widget.RenderType = (Widget.RenderingType)Enum.Parse (typeof(Widget.RenderingType), idxArg.Get<string> ());
                    break;
                default:
                    if (idxArg.Name.StartsWith ("on")) {
                        CreateEventHandler (context, widget, idxArg);
                    }
                    break;
                }
            }
            return widget;
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
