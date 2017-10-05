/*
 * Phosphorus Five, copyright 2014 - 2017, Thomas Hansen, thomas@gaiasoul.com
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
using p5.exp;
using p5.core;

namespace p5.csv
{
    /// <summary>
    ///     Class to help transform CSV to a p5.lambda structure
    /// </summary>
    public static class Lambda2Csv
    {
        /// <summary>
        ///     Creates a CSV document from the given lambda.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.csv.lambda2csv")]
        public static void p5_csv_lambda2csv (ApplicationContext context, ActiveEventArgs e) {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new ArgsRemover (e.Args)) {

                // Loops through all documents we're supposed to transform, and stuffing into temporary StringBuilder.
                var builder = new StringBuilder ();
                bool first = true;
                foreach (var idxLambda in XUtil.Iterate<Node> (context, e.Args)) {
                    if (first) {

                        // Adding headers.
                        foreach (var idxInnerHeader in idxLambda.Children) {
                            if (builder.Length > 0)
                                builder.Append (",");
                            builder.Append (idxInnerHeader.Name);
                        }
                        builder.Append ("\r\n");
                        first = false;
                    }

                    // Adding values.
                    var content = "";
                    foreach (var idxInner in idxLambda.Children) {
                        var val = idxInner.Get<string> (context);
                        if (val.Contains (",") || val.Contains ("\n")) {
                            content += "\"" + val.Replace ("\"", "\"\"") + "\",";
                        } else {
                            content += val + ",";
                        }
                    }
                    builder.Append (content.Substring (0, content.Length - 1) + "\r\n");
                }
                e.Args.Value = builder.ToString ();
            }
        }
    }
}
