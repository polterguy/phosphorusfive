
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using System;
using System.Collections.Generic;
using phosphorus.core;
using phosphorus.expressions;
using phosphorus.ajax.widgets;

namespace phosphorus.web.ui
{
    /// <summary>
    /// helper to retrieve and change properties of widgets
    /// </summary>
    public static class properties
    {
        /// <summary>
        /// returns properties requested by caller as children nodes of [pf.web.widgets.get-property]. the properties you
        /// wish to retrieve, are given as the names of the children nodes of [pf.web.widgets.get-property]. the widget you
        /// wish to retrieve properties from, is given as the value of [pf.web.widgets.get-property]. the value of
        /// [pf.web.widgets.get-property] can also be an expression
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.widgets.property.get")]
        private static void pf_web_widgets_property_get (ApplicationContext context, ActiveEventArgs e)
        {
            var origNodeList = new List<Node> (e.Args.Children);
            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {
                var widget = FindWidget (context, idx);
                foreach (var nameNode in origNodeList) {
                    if (widget.ElementType == "select" && nameNode.Name == "value") {
                        foreach (var idxCtrl in widget.Controls) {
                            var idxWidget = idxCtrl as Widget;
                            if (idxWidget != null) {
                                if (idxWidget.HasAttribute ("selected")) {
                                    if (XUtil.IsExpression (e.Args.Value)) {
                                        e.Args.FindOrCreate (nameNode.Name).Value = idxWidget ["value"];
                                    } else {
                                        nameNode.Value = idxWidget ["value"];
                                    }
                                    break;
                                }
                            }
                        }
                    } else {
                        switch (nameNode.Name) {
                        case "element":
                            if (XUtil.IsExpression (e.Args.Value)) {
                                e.Args.FindOrCreate (nameNode.Name).Value = widget.ElementType;
                            } else {
                                nameNode.Value = widget.ElementType;
                            }
                            break;
                        default:
                            if (!string.IsNullOrEmpty (nameNode.Name)) {
                                if (XUtil.IsExpression (e.Args.Value)) {
                                    e.Args.FindOrCreate (widget.ID).Add (nameNode.Name).LastChild.Value = widget [nameNode.Name];
                                } else {
                                    nameNode.Value = widget [nameNode.Name];
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// set properties of the widget with the ID of the value of [pf.web.widgets.set-property] to the value of the children
        /// nodes of [pf.web.widgets.set-property]. the properties you wish to set, is given through the names of the children
        /// nodes of [pf.web.widgets.set-property]. the values you wish to set can also be expressions, or formatting expressions.
        /// the value of [pf.web.widgets.set-property] can also be an expression
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.widgets.property.set")]
        private static void pf_web_widgets_property_set (ApplicationContext context, ActiveEventArgs e)
        {
            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {
                var widget = FindWidget (context, idx);
                foreach (var valueNode in e.Args.Children) {
                    string propertyValue;
                    switch (valueNode.Name) {
                    case "element":
                        propertyValue = valueNode.Get<string> (context);
                        widget.ElementType = propertyValue;
                        break;
                    default:
                        propertyValue = valueNode.Get<string> (context);
                        widget [valueNode.Name] = propertyValue;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// removes properties requested by caller as children nodes of [pf.web.widgets.remove-property]. the properties you
        /// wish to remove, are given as the names of the children nodes of [pf.web.widgets.get-property]. the widget ID you
        /// wish to retrieve properties from, is given as the value of [pf.web.widgets.get-property].
        /// the value of [pf.web.widgets.remove-property] can also be an expression
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.widgets.property.remove")]
        private static void pf_web_widgets_property_remove (ApplicationContext context, ActiveEventArgs e)
        {
            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {
                var widget = FindWidget (context, idx);
                foreach (var nameNode in e.Args.Children) {
                    widget.RemoveAttribute (nameNode.Name);
                }
            }
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
