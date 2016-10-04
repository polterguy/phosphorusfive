/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Text;
using p5.exp;
using p5.core;
using p5.hyperlambda.helpers;

/// <summary>
///     Main namespace for parsing and creating Hyperlambda
/// </summary>
namespace p5.hyperlambda
{
    /// <summary>
    ///     Class to help transform between Hyperlambda and <see cref="phosphorus.core.Node">Nodes</see>
    /// </summary>
    public static class Hyperlambda
    {
        /// <summary>
        ///     Tranforms the given Hyperlambda to a p5 lambda node structure
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "hyper2lambda")]
        public static void hyper2lambda (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Concatenating all Hyperlambda submitted, injecting CR/LF between each component
                StringBuilder builder = new StringBuilder();
                foreach (var idxHyperlisp in XUtil.Iterate<string> (context, e.Args, false, false, true)) {

                    // Making sure we put in a carriage return between each Hyperlambda entity
                    if (builder.Length > 0)
                        builder.Append ("\r\n");

                    // Appending currently iterated Hyperlambda into StringBuilder
                    builder.Append (idxHyperlisp);
                }

                // Utilizing NodeBuilder to create our p5 lambda return value
                e.Args.AddRange (new NodeBuilder (context, builder.ToString ()).Nodes);
            }
        }

        /// <summary>
        ///     Transforms the given p5 lambda node structure to Hyperlambda
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "lambda2hyper")]
        public static void lambda2hyper (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Using HyperlispBuilder to create Hyperlambda from p5 lambda
                e.Args.Value = new HyperlambdaBuilder (
                    context, 
                    XUtil.Iterate<Node> (context, e.Args, false, false, true))
                    .Hyperlambda;
            }
        }
    }
}
