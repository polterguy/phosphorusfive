/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using System;
using phosphorus.ajax.widgets;
using phosphorus.core;
using Void = phosphorus.ajax.widgets.Void;

// ReSharper disable UnusedMember.Local

namespace phosphorus.web.ui.widgets
{
    /// <summary>
    ///     class for creating web widgets
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public static class Widgets
    {
        /// <summary>
        ///     creates a generic container type of Widget, that will only allow for child controls, and not content besides that
        /// </summary>
        /// <param name="context"><see cref="phosphorus.core.ApplicationContext" /> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.widgets.container")]
        private static void pf_web_controls_container (ApplicationContext context, ActiveEventArgs e)
        {
            var widget = CreateControl<Container> (context, e.Args, "div");
            var formId = e.Args.Find (idx => idx.Name == "_form-id");
            if (formId != null && formId.Value == null)
                formId.Value = widget.ClientID;
            e.Args.Value = DecorateWidget (context, widget, e.Args);
            if (widget.ElementType == "select" && !widget.HasAttribute ("name"))
                widget ["name"] = widget.ClientID;
        }

        /// <summary>
        ///     creates a generic literal type of Widget, that will allow for only innerValue and no children controls
        /// </summary>
        /// <param name="context"><see cref="phosphorus.core.ApplicationContext" /> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.widgets.literal")]
        private static void pf_web_controls_literal (ApplicationContext context, ActiveEventArgs e)
        {
            var widget = CreateControl<Literal> (context, e.Args, "p");
            e.Args.Value = DecorateWidget (context, widget, e.Args);
            if (widget.ElementType == "textarea" && !widget.HasAttribute ("name"))
                widget ["name"] = widget.ClientID;
        }

        /// <summary>
        ///     creates a generic void type of Widget, that will not allow for any content, but only attributes
        /// </summary>
        /// <param name="context"><see cref="phosphorus.core.ApplicationContext" /> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.widgets.void")]
        private static void pf_web_controls_void (ApplicationContext context, ActiveEventArgs e)
        {
            var widget = CreateControl<Void> (context, e.Args, "input");
            e.Args.Value = DecorateWidget (context, widget, e.Args);
            if (widget.ElementType == "input" && !widget.HasAttribute ("name"))
                widget ["name"] = widget.ClientID;
            if (widget.ElementType == "input" && widget ["type"] == "radio" && !widget.HasAttribute ("value"))
                widget ["value"] = widget.ClientID;
        }

        /*
         * creates a widget from the given node
         */

        private static T CreateControl<T> (ApplicationContext context, Node node, string elementType, Widget.RenderingType type = Widget.RenderingType.Default) where T : Widget, new ()
        {
            // creating widget as persistent control
            var parent = node.Find (idx => idx.Name == "__parent").Get<Container> (context);

            var widget = parent.CreatePersistentControl<T> (
                node.Get<string> (context),
                -1,
                delegate (object sender, EventArgs e) {
                    // hooks up "oninitialload" event, if we're supposed to
                    CreateLoadingEvents (context, node, sender as Widget);
                });

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
         * creates the [oninitialload] event for widget, if we should
         */

        private static void CreateLoadingEvents (ApplicationContext context, Node node, Widget widget)
        {
            // checking to see if we've got an "initialload" Active Event for widget, and if so, handle it
            var onInitialLoad = node.Find (idx => idx.Name == "oninitialload");
            if (onInitialLoad != null) {
                onInitialLoad = onInitialLoad.Clone ();
                onInitialLoad.Insert (0, new Node ("_form-id", node.Root.Find (idx => idx.Name == "_form-id").Value));
                onInitialLoad.Insert (1, new Node ("_widget-id", widget.ID));
                context.Raise ("lambda", onInitialLoad);
            }
        }

        /*
         * decorates widget with common properties
         */

        private static Widget DecorateWidget (ApplicationContext context, Widget widget, Node args)
        {
            // looping through all children nodes of Widget's node to decorate Widget
            foreach (var idxArg in args.Children) {
                switch (idxArg.Name) {
                    case "has-id":
                        if (!idxArg.Get<bool> (context))
                            widget.NoIdAttribute = true;
                        break;
                    case "render-type":
                        SetRenderType (widget, idxArg.Get<string> (context));
                        break;
                    case "element":
                        SetElementType (widget, idxArg.Get<string> (context));
                        break;
                    case "controls":
                        CreateChildWidgets (context, widget, idxArg);
                        break;
                    case "checked":
                        if (idxArg.Value == null || idxArg.Get<bool> (context))
                            widget ["checked"] = null;
                        break;
                    case "parent":
                    case "events":
                        // skipping these buggers, since they're not supposed to be handled here
                        break;
                    default:

                        // this might be an event, it might be a node we should ignore (starting with "_") or it might be any arbitrary attribute
                        // we should render. HandleDefaultProperty will figure out
                        HandleDefaultProperty (context, widget, idxArg);
                        break;
                }
            }

            // ensures "name" property is created, if necessary
            EnsureNameProperty (widget);

            return widget;
        }

        /*
         * ensuring the "name" property is the same as the "ID" of the widget, unless a name property is explicitly given,
         * or element type doesn't necessarily require a "name" to function correctly
         */

        private static void EnsureNameProperty (Widget widget)
        {
            // making sure "input", "select" and "textarea" widgets have a name corresponding to 
            // their ID unless name is explicitly given
            var addName = false;
            switch (widget.ElementType) {
                case "input":
                    if (widget.ElementType != "button")
                        addName = true;
                    break;
                case "textarea":
                case "select":
                    addName = true;
                    break;
            }
            if (addName) {
                widget ["name"] = widget.ID;
            }
        }

        /*
         * sets the rendering type of Widget
         */
        private static void SetRenderType (Widget widget, string renderType) { widget.RenderType = (Widget.RenderingType) Enum.Parse (typeof (Widget.RenderingType), renderType); }
        /*
         * changes the ElementType of the Widget
         */
        private static void SetElementType (Widget widget, string elementType) { widget.ElementType = elementType; }
        /*
         * creates children widgets of widget
         */

        private static void CreateChildWidgets (ApplicationContext context, Widget widget, Node children)
        {
            foreach (var idxChild in children.Children) {
                idxChild.Insert (0, new Node ("__parent", widget));
                idxChild.Insert (1, new Node ("_form-id", children.Parent.Find (idx => idx.Name == "_form-id").Value));
                context.Raise ("pf.web.widgets." + idxChild.Name, idxChild);
            }
        }

        /*
         * handles all default properties of Widget
         */

        private static void HandleDefaultProperty (ApplicationContext context, Widget widget, Node node)
        {
            if (node.Name.StartsWith ("on")) {
                if (node.Name != "oninitialload")
                    CreateEventHandler (context, widget, node);
            } else if (!node.Name.StartsWith ("_")) {
                widget [node.Name] = node.Get<string> (context);
            }
        }

        /*
         * creates an event handler on the given widget for the given node
         */

        private static void CreateEventHandler (ApplicationContext context, Widget widget, Node node)
        {
            if (node.Value != null) {
                // javascript code to be executed
                widget [node.Name] = node.Get<string> (context);
            } else {
                // creating our pf.lambda object, and invoking our [_pf.web.add-widget-event] Active Event to map the
                // ajax event to our pf.lambda object, passing in the widget that contains the Ajax Event
                var eventNode = new Node (string.Empty, widget);
                eventNode.Add (node.Clone ());

                // making sure [_form-id] and [_widget-id] is passed into pf.lambda object as parameters
                eventNode [0].Insert (0, new Node ("_form-id", node.Root.Find (idx => idx.Name == "_form-id").Value));
                eventNode [0].Insert (1, new Node ("_widget-id", node.Parent.Value));

                // raising the Active Event that actually creates our ajax event handler for our pf.lambda object
                context.Raise ("_pf.web.add-widget-event", eventNode);
            }
        }
    }
}