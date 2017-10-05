/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, thomas@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System.Text;
using System.Linq;
using p5.exp;
using p5.core;

namespace p5.xml
{
    /// <summary>
    ///     Class to help transform XML to a lambda structure
    /// </summary>
    public static class Lambda2Xml
    {
        /// <summary>
        ///     Parses an XML document, and creates a p5.lambda node structure from the results
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "lambda2xml")]
        public static void lambda2xml (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new ArgsRemover (e.Args)) {

                // Loops through all nodes we're supposed to transform.
                var builder = new StringBuilder ();
                builder.Append ("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n");
                foreach (var idx in XUtil.Iterate<Node> (context, e.Args)) {
                    BuildNode (context, idx, builder, 0);
                }
                e.Args.Value = builder.ToString ();
            }
        }

        static void BuildNode (ApplicationContext context, Node node, StringBuilder builder, int sp)
        {
            for (var idxNo = 0; idxNo < sp; idxNo++)
                builder.Append (" ");

            builder.Append ("<" + node.Name);
            foreach (var idx in node.Children.Where (ix => ix.Name.StartsWithEx ("@"))) {
                builder.Append (" " + idx.Name.Substring (1) + "=\"" + idx.Value + "\"");
            }
            if (!node.Children.Any (ix => !ix.Name.StartsWithEx ("@"))) {
                builder.Append (" />\r\n");
            } else {
                builder.Append (">\r\n");
                foreach (var idx in node.Children.Where (ix => !ix.Name.StartsWithEx ("@") && !ix.Name.StartsWithEx ("#"))) {
                    BuildNode (context, idx, builder, sp + 1);
                }
                if (node.Children.Any (ix => ix.Name == "#text"))
                    builder.Append (node.Children.First (ix => ix.Name == "#text").Value);
                for (var idxNo = 0; idxNo < sp; idxNo++)
                    builder.Append (" ");
                builder.Append ("</" + node.Name + ">\r\n");
            }
        }
    }
}
