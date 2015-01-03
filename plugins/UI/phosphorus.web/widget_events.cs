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
        /// returns the [name] property of widget with ID of the value of [pf.web.widgets.get-property] as first child of
        /// [pf.web.widgets.get-property], named [value]
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.widgets.get-property")]
        private static void pf_web_widgets_get_property (ApplicationContext context, ActiveEventArgs e)
        {
            Node findCtrl = new Node (string.Empty, e.Args.Value);
            context.Raise ("pf.find-control", findCtrl);
            Widget widget = findCtrl [0].Get<Widget> ();
            Node nameNode = e.Args.Find (
                delegate (Node idx) {
                    return idx.Name == "name";
            });
            if (widget.ElementType == "select" && nameNode.Get<string> () == "value") {

                // special treatment for select html elements
                foreach (var idxCtrl in widget.Controls) {
                    Widget idxWidget = idxCtrl as Widget;
                    if (idxWidget != null) {
                        if (idxWidget.HasAttribute ("selected")) {
                            e.Args.Insert (0, new Node ("value", idxWidget ["value"]));
                            break;
                        }
                    }
                }
            } else {
                e.Args.Insert (0, new Node ("value", widget [nameNode.Get<string> ()]));
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
            Node nameNode = e.Args.Find (
                delegate (Node idx) {
                    return idx.Name == "name";
            });
            Node valueNode = e.Args.Find (
                delegate (Node idx) {
                    return idx.Name == "value";
            });
            string value = valueNode.Get<string> ();
            if (Expression.IsExpression (value)) {
                var match = Expression.Create (value).Evaluate (valueNode);
                if (match.TypeOfMatch == Match.MatchType.Count) {
                    value = match.Count.ToString ();
                } else if (!match.IsSingleLiteral) {
                    throw new ArgumentException ("[pf.set-widget-property] can only take a single literal expression");
                } else {
                    value = match.GetValue (0).ToString ();
                }
            } else if (valueNode.Count > 0) {
                value = Expression.FormatNode (valueNode);
            }
            widget [nameNode.Get<string> ()] = value;
        }
    }
}
