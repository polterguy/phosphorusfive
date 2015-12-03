/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Linq;
using System.Text;
using p5.core;
using p5.exp;
using p5.hyperlisp.helpers;

/// <summary>
///     Main namespace for parsing and creating Hyperlisp
/// </summary>
namespace p5.hyperlisp
{
    /// <summary>
    ///     Class to help transform between Hyperlisp and <see cref="phosphorus.core.Node">Nodes</see>
    /// </summary>
    public static class Hyperlisp
    {
        /// <summary>
        ///     Tranforms the given Hyperlisp to a p5.lambda node structure
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "lisp2lambda", Protection = EntranceProtection.Lambda)]
        private static void lisp2lambda (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Concatenating all Hyperlisp submitted, injecting CR/LF between each component
                StringBuilder builder = new StringBuilder();
                foreach (var idxHyperlisp in XUtil.Iterate<string> (context, e.Args, false, false, true)) {

                    // Making sure we put in a carriage return between each Hyperlisp entity
                    if (builder.Length > 0)
                        builder.Append ("\r\n");

                    // Appending currently iterated Hyperlisp into StringBuilder
                    builder.Append (idxHyperlisp);
                }

                // Utilizing NodeBuilder to create our p5.lambda return value
                e.Args.AddRange (new NodeBuilder (context, builder.ToString ()).Nodes);
            }
        }

        /// <summary>
        ///     Transforms the given p5.lambda node structure to Hyperlisp
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "lambda2lisp", Protection = EntranceProtection.Lambda)]
        private static void lambda2lisp (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                e.Args.Value = new HyperlispBuilder (context, XUtil.Iterate<Node> (context, e.Args, false, false, true)).Hyperlisp;
            }
        }
    }
}
