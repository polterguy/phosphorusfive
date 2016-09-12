/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System.Linq;
using System.Text;
using System.Collections.Generic;
using p5.core;
using p5.exp;

namespace p5.html
{
    /// <summary>
    ///     Class to help transform p5 lambda to HTML
    /// </summary>
    public static class Lambda2Html
    {
        /// <summary>
        ///     Creates an HTML document from the given p5 lambda object
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "lambda2html", Protection = EventProtection.LambdaClosed)]
        public static void lambda2html (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Used as buffer when converting from lambda to HTML
                StringBuilder builder = new StringBuilder ();

                // Doing actual conversion from lambda to HTML
                Convert (context, XUtil.Iterate<Node> (context, e.Args), builder);

                // Returning HTML to caller
                e.Args.Value = builder.ToString ().Trim ();
            }
        }

        /*
         * Helper for above
         */
        private static void Convert (
            ApplicationContext context, 
            IEnumerable<Node> nodes, 
            StringBuilder builder)
        {
            foreach (Node idxNode in nodes) {
                if (idxNode.Name == "#text") {
                    builder.Append (idxNode.Get<string>(context));
                    continue;
                }
                builder.Append (string.Format ("<{0}{1}>", idxNode.Name, GetAttributes (idxNode)));
                Convert (context, idxNode.Children.Where (ix => !ix.Name.StartsWith ("@")), builder);
                builder.Append (string.Format ("</{0}>", idxNode.Name));
            }
        }

        /*
         * Helper method, to parse and retrieve attributes
         */
        private static string GetAttributes (Node node)
        {
            string retVal = "";
            foreach (Node idxAtr in node.Children.Where (ix => ix.Name.StartsWith ("@"))) {
                retVal += " " + idxAtr.Name.Substring (1) + "=\"" + idxAtr.Value + "\"";
            }
            return retVal;
        }
    }
}
