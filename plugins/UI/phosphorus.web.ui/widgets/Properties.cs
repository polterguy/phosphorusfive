/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.Collections.Generic;
using phosphorus.ajax.widgets;
using phosphorus.core;
using phosphorus.expressions;
using phosphorus.expressions.exceptions;

// ReSharper disable UnusedMember.Local

/// <summary>
///     Main namespace for all Active Events related to Ajax Web Widgets.
/// 
///     Contains wrapper classes that creates or manipulates Ajax Web Widgets in Phosphorus.Five.
/// </summary>
namespace phosphorus.web.ui.widgets
{
    /// <summary>
    ///     Helper to retrieve and change properties of widgets.
    /// 
    ///     Class wrapping the Active Events necessary to manipulate properties and attributes of Web Widgets.
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public static class Properties
    {
        /// <summary>
        ///     Returns properties and/or attributes requested by caller as children nodes.
        /// 
        ///     Will return all properties and attributes on widget requested as the names of child nodes of the
        ///     main node when invoking the Active Event. The ID of the widgets you wish to retrieve properties from,
        ///     is given as the main value of the node.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.web.widgets.property.get")]
        private static void pf_web_widgets_property_get (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value == null || e.Args.Count == 0)
                return; // nothing to do here ...

            // need to store original children nodes, since method might create new children nodes, during enumeration
            var origNodeList = new List<Node> (e.Args.Children);

            // looping through all widget IDs given by caller
            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {

                // finding widget
                var widget = FindWidget (context, idx);
                if (widget == null)
                    continue;

                // looping through all properties requested by caller
                foreach (var nameNode in origNodeList) {

                    // checking if this is a generic attribute, or a specific property
                    switch (nameNode.Name) {
                        case "":
                            continue; // formatting parameter to expression in main node
                        case "visible":
                            CreatePropertyReturn (e.Args, nameNode, widget, widget.Visible);
                            break;
                        case "invisible-element":
                            CreatePropertyReturn (e.Args, nameNode, widget, widget.InvisibleElement);
                            break;
                        case "element":
                            CreatePropertyReturn (e.Args, nameNode, widget, widget.ElementType);
                            break;
                        case "has-id":
                            CreatePropertyReturn (e.Args, nameNode, widget, !widget.NoIdAttribute);
                            break;
                        case "render-type":
                            CreatePropertyReturn (e.Args, nameNode, widget, widget.RenderType.ToString ());
                            break;
                        default:
                            if (!string.IsNullOrEmpty (nameNode.Name))
                                CreatePropertyReturn (e.Args, nameNode, widget);
                            break;
                    }
                }
            }
        }

        /// <summary>
        ///     Sets properties and/or attributes of web widgets.
        /// 
        ///     Sets the properties and attributes of the widget(s) with the given ID(s) to the value of its children nodes.
        ///     The properties you wish to set, are given as the names of the children nodes. The widget(s) you
        ///     wish to set the properties of, are given as the value of node.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.web.widgets.property.set")]
        private static void pf_web_widgets_property_set (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value == null || e.Args.Count == 0)
                return; // nothing to do here ...

            // looping through all widget IDs given by caller
            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {
                
                // finding widget
                var widget = FindWidget (context, idx);
                if (widget == null)
                    continue;

                // looping through all properties requested by caller
                foreach (var valueNode in e.Args.Children) {
                    switch (valueNode.Name) {
                        case "":
                            continue; // formatting parameter to expression in main node
                        case "visible":
                            widget.Visible = valueNode.GetExValue<bool> (context);
                            break;
                        case "invisible-element":
                            widget.InvisibleElement = valueNode.GetExValue<string> (context);
                            break;
                        case "element":
                            widget.ElementType = valueNode.GetExValue<string> (context);
                            break;
                        case "has-id":
                            widget.NoIdAttribute = valueNode.GetExValue<bool> (context);
                            break;
                        case "render-type":
                            widget.RenderType = (Widget.RenderingType) Enum.Parse (typeof (Widget.RenderingType), valueNode.GetExValue<string> (context));
                            break;
                        default:
                            widget [valueNode.Name] = valueNode.GetExValue<string> (context);
                            break;
                    }
                }
            }
        }

        /// <summary>
        ///     Removes the properties and/or attributes of web widgets.
        /// 
        ///     The properties you wish to remove, are given as the names of the children nodes. The widget(s) you
        ///     wish to remove properties from, are given as the value of node.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.web.widgets.property.remove")]
        private static void pf_web_widgets_property_remove (ApplicationContext context, ActiveEventArgs e)
        {
            // looping through all widgets
            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {

                // finding widget
                var widget = FindWidget (context, idx);
                if (widget == null)
                    continue;

                // looping through each property to remove
                foreach (var nameNode in e.Args.Children) {

                    // verifying property can be removed
                    switch (nameNode.Name) {
                        case "":
                            continue; // formatting parameter to expression in main node
                        case "visible":
                        case "invisible-element":
                        case "element":
                        case "has-id":
                        case "has-name":
                        case "render-type":
                            throw new ArgumentException ("Cannot remove property; '" + nameNode.Name + "' of widget.");
                        default:
                            widget.RemoveAttribute (nameNode.Name);
                            break;
                    }
                }
            }
        }
        
        /// <summary>
        ///     Lists all existing properties for given web widget.
        /// 
        ///     Will return all properties and/or attributes of all widgets requested as the value of the main Active Event
        ///     node.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.web.widgets.property.list")]
        private static void pf_web_widgets_property_list (ApplicationContext context, ActiveEventArgs e)
        {
            // looping through all widgets
            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {

                // finding widget
                var widget = FindWidget (context, idx);
                if (widget == null)
                    continue;

                // creating our "return node" for currently handled widget
                Node curNode = e.Args.Add (widget.ID).LastChild;

                // first listing "static properties"
                curNode.Add ("visible");
                curNode.Add ("invisible-element");
                curNode.Add ("element");
                curNode.Add ("has-id");
                curNode.Add ("render-type");
                foreach (var idxAtr in widget.AttributeKeys) {
                    if (idxAtr == "Tag" || idxAtr.StartsWith ("on"))
                        continue;
                    curNode.Add (idxAtr);
                }
            }
        }

        /*
         * helper for [pf.web.widgets.property.get], creates a return value for one property
         */
        private static void CreatePropertyReturn (Node node, Node nameNode, Widget widget, object value = null)
        {
            // checking if widget has the attribute, if it doesn't, we don't even add any return nodes at all, to make it possible
            // to separate widgets which has the property, but no value, (such as the selected property on checkboxes for instance),
            // and widgets that does not have the property at all
            if (value == null && !widget.HasAttribute (nameNode.Name))
                return;

            node.FindOrCreate (widget.ID).Add (nameNode.Name).LastChild.Value = value == null ? widget [nameNode.Name] : value;
        }

        /*
         * returns the widget we're looking for
         */
        private static Widget FindWidget (ApplicationContext context, string widgetId)
        {
            var findCtrl = new Node (string.Empty, widgetId);
            context.Raise ("_pf.web.find-control", findCtrl);
            return findCtrl [0].Get<Widget> (context);
        }
    }
}