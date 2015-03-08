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

/// <summary>
///     Main namespace for parsing and creating Hyperlisp.
/// 
///     Encapsulates all classes and Active Event necessary to create and parse Hyperlisp.
/// </summary>
namespace phosphorus.hyperlisp
{
    /// \todo Create more unit tests for all different permutations of Iterate.
    /// <summary>
    ///     Class to help transform between Hyperlisp and <see cref="phosphorus.core.Node">Nodes</see>.
    /// 
    ///     Contains the [pf.hyperlisp.hyperlisp2lambda] and the [pf.hyperlisp.lambda2hyperlisp] Active Events,
    ///     necessary to be able to parse Hyperlisp from pf.lambda nodes, and vice versa.
    /// </summary>
    public static class Hyperlisp
    {
        /// <summary>
        ///     Tranforms the given Hyperlisp to a pf.lambda node structure.
        /// 
        ///     Active Event will transform the given Hyperlisp to a pf.lambda node structure.
        /// 
        ///     Example;
        /// 
        ///     <pre>pf.hyperlisp.hyperlisp2lambda:@"foo:bar"</pre>
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.hyperlisp.hyperlisp2lambda")]
        private static void pf_hyperlisp_hyperlisp2lambda (ApplicationContext context, ActiveEventArgs e)
        {
            var builder = new StringBuilder ();
            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {
                builder.Append (idx + "\r\n");
            }
            e.Args.AddRange (new NodeBuilder (context, builder.ToString ().TrimEnd ('\r', '\n', ' ')).Nodes);
        }

        /// \todo Create more unit tests for all different permutations of Iterate
        /// <summary>
        ///     Transforms the given pf.lambda node structure to Hyperlisp.
        /// 
        ///     Active Event will transform the given pf.lambda node structure to Hyperlisp
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
