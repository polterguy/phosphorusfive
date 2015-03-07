/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System.Linq;
using System.Text;
using phosphorus.core;
using phosphorus.expressions;
using phosphorus.hyperlisp.helpers;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace phosphorus.hyperlisp
{
    /// <summary>
    ///     class to help transform between hyperlisp and <see cref="phosphorus.core.Node" />
    /// </summary>
    public static class Hyperlisp
    {
        /// <summary>
        ///     helper to transform from hyperlisp code syntax to <see cref="phosphorus.core.Node" /> tree structure.
        ///     will transform the given hyperlisp into a list of nodes and append them into the root node given
        ///     through the active event args
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.hyperlisp.hyperlisp2lambda")]
        private static void pf_hyperlisp_hyperlisp2lambda (ApplicationContext context, ActiveEventArgs e)
        {
            var builder = new StringBuilder ();
            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {
                builder.Append (idx + "\r\n");
            }
            e.Args.AddRange (new NodeBuilder (context, builder.ToString ().TrimEnd ('\r', '\n', ' ')).Nodes);
        }

        /// <summary>
        ///     helper to transform from <see cref="phosphorus.core.Node" /> tree structure to hyperlisp code syntax.
        ///     will transform the children nodes of the given active event args into hyperlisp and return as string
        ///     value of the value of the given active event args
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.hyperlisp.lambda2hyperlisp")]
        private static void pf_code_lambda2hyperlisp (ApplicationContext context, ActiveEventArgs e)
        {
            if (XUtil.IsExpression (e.Args.Value)) {
                var nodeList = XUtil.Iterate<Node> (e.Args, context).ToList ();
                e.Args.Value = new HyperlispBuilder (context, nodeList).Hyperlisp;
            } else {
                var node = e.Args.Value as Node;
                e.Args.Value = node != null ? new HyperlispBuilder (context, new[] {node}).Hyperlisp : new HyperlispBuilder (context, e.Args.Children).Hyperlisp;
            }
        }
    }
}