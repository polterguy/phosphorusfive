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
            Container parent = node [0].Get<Container> ();
            T widget = parent.CreatePersistentControl<T> (node.Get<string> ());
            widget.ElementType = elementType;
            if (type != Widget.RenderingType.Default)
                widget.RenderType = type;
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
                        widget.Controls.Add (context.Raise ("pf.web.widgets." + idxChild.Name, idxChild).Get<Widget> ());
                    }
                    break;
                case "class":
                    widget ["class"] = idxArg.Get<string> ();
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

                // pf.lambda event handler
                widget [node.Name] = "common_event_handler";
                Node eventNode = new Node (string.Empty, widget.ID);
                eventNode.Add (node.Clone ());
                context.Raise ("pf.add-widget-event", eventNode);
            }
        }
    }
}
