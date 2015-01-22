
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using System;
using System.Web;
using System.Text;
using System.Web.UI;
using System.Globalization;
using System.Security.Cryptography;
using phosphorus.core;
using phosphorus.lambda;
using phosphorus.ajax.widgets;

namespace phosphorus.web
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
        [ActiveEvent (Name = "pf.web.widgets.get-property")]
        private static void pf_web_widgets_get_property (ApplicationContext context, ActiveEventArgs e)
        {
            Widget widget = FindWidget (context, e.Args);
            foreach (Node nameNode in e.Args.Children) {

                if (widget.ElementType == "select" && nameNode.Name == "value") {

                    // special treatment for select html elements
                    foreach (var idxCtrl in widget.Controls) {
                        Widget idxWidget = idxCtrl as Widget;
                        if (idxWidget != null) {
                            if (idxWidget.HasAttribute ("selected")) {
                                nameNode.Value = idxWidget ["value"];
                                break;
                            }
                        }
                    }
                } else {
                    switch (nameNode.Name) {
                    case "element":
                        nameNode.Value = widget.ElementType;
                        break;
                    default:
                        nameNode.Value = widget [nameNode.Name];
                        break;
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
        [ActiveEvent (Name = "pf.web.widgets.set-property")]
        private static void pf_web_widgets_set_property (ApplicationContext context, ActiveEventArgs e)
        {
            Widget widget = FindWidget (context, e.Args);
            foreach (Node valueNode in e.Args.Children) {

                string propertyValue = valueNode.Get<string> ();
                if (Expression.IsExpression (propertyValue)) {

                    // value is an expression, hence returning value of expression and not value directly
                    var match = Expression.Create (propertyValue).Evaluate (valueNode);
                    if (match.TypeOfMatch == Match.MatchType.Count) {
                        propertyValue = match.Count.ToString ();
                    } else if (!match.IsSingleLiteral) {
                        throw new ArgumentException ("[pf.web.widgets.set-property] can only take a single literal expression");
                    } else {
                        propertyValue = match.GetValue (0).ToString ();
                    }
                } else if (valueNode.Count > 0) {

                    // making sure we support formatting nodes
                    propertyValue = Expression.FormatNode (valueNode);
                } else if (propertyValue.StartsWith ("\\")) {

                    // to support values who's value are expressions, where we do not want to 
                    // evaluate the expression, but pass it onwards to the value of a widget's property
                    propertyValue = propertyValue.Substring (1);
                }
                switch (valueNode.Name) {
                case "element":
                    widget.ElementType = valueNode.Get<string> ();
                    break;
                default:
                    widget [valueNode.Name] = propertyValue == null ? null : propertyValue;
                    break;
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
        [ActiveEvent (Name = "pf.web.widgets.remove-property")]
        private static void pf_web_widgets_remove_property (ApplicationContext context, ActiveEventArgs e)
        {
            Widget widget = FindWidget (context, e.Args);
            foreach (Node nameNode in e.Args.Children) {

                widget.RemoveAttribute (nameNode.Name);
            }
        }

        /*
         * returns the widget we're looking for
         */
        private static Widget FindWidget (ApplicationContext context, Node node)
        {
            string widgetId = node.Get<string> ();
            if (Expression.IsExpression (widgetId)) {
                var match = Expression.Create (widgetId).Evaluate (node);
                if (!match.IsSingleLiteral || !match.IsAssignable || match.TypeOfMatch == Match.MatchType.Node)
                    throw new ArgumentException ("find widget requires an expression being a single literal of type 'value' or 'name'");
                widgetId = match.GetValue (0) as string;
            }
            Node findCtrl = new Node (string.Empty, widgetId);
            context.Raise ("_pf.web.find-control", findCtrl);
            return findCtrl [0].Get<Widget> ();
        }
    }
}
