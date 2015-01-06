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
    public static class widget_events
    {
        /// <summary>
        /// returns properties requested by caller as children nodes of [pf.web.widgets.get-property]
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.widgets.get-property")]
        private static void pf_web_widgets_get_property (ApplicationContext context, ActiveEventArgs e)
        {
            Node findCtrl = new Node (string.Empty, e.Args.Value);
            context.Raise ("pf.find-control", findCtrl);
            Widget widget = findCtrl [0].Get<Widget> ();

            foreach (Node nameNode in e.Args.Children) {

                string propertyName = nameNode.Name;
                if (propertyName == "parent")
                    continue; // referring to where to start looking for widget, and not what property to retrieve

                if (widget.ElementType == "select" && propertyName == "value") {

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
                    nameNode.Value = widget [propertyName];
                }
            }
        }

        /// <summary>
        /// set a property of the widget with the ID of the value of [pf.web.widgets.set-property] to the value of the [value] child node
        /// of [pf.web.widgets.set-property]. the property to set, is given through the [name] child of [pf.web.widgets.set-property]. the [value]
        /// node can also contain formatting parameters if you wish
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.widgets.set-property")]
        private static void pf_web_widgets_set_property (ApplicationContext context, ActiveEventArgs e)
        {
            Node findCtrl = new Node (string.Empty, e.Args.Value);
            context.Raise ("pf.find-control", findCtrl);
            Widget widget = findCtrl [0].Get<Widget> ();

            foreach (Node valueNode in e.Args.Children) {

                string propertyValue = valueNode.Get<string> ();
                string propertyName = valueNode.Name;
                if (propertyName == "parent")
                    continue; // referring to where to start looking for widget, and not what property to retrieve

                if (Expression.IsExpression (propertyValue)) {

                    // value is an expression, hence returning value of expression and not value directly
                    var match = Expression.Create (propertyValue).Evaluate (valueNode);
                    if (match.TypeOfMatch == Match.MatchType.Count) {
                        propertyValue = match.Count.ToString ();
                    } else if (!match.IsSingleLiteral) {
                        throw new ArgumentException ("[pf.set-widget-property] can only take a single literal expression");
                    } else {
                        propertyValue = match.GetValue (0).ToString ();
                    }
                } else if (valueNode.Count > 0) {

                    // making sure we support formatting nodes
                    propertyValue = Expression.FormatNode (valueNode);
                }
                widget [propertyName] = propertyValue == null ? null : propertyValue.TrimStart ('\\');
            }
        }
    }
}
