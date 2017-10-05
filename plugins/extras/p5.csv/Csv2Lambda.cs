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

using System.IO;
using p5.exp;
using p5.core;
using CsvHelper;

namespace p5.csv
{
    /// <summary>
    ///     Class to help transform CSV to a p5.lambda structure
    /// </summary>
    public static class Csv2Lambda
    {
        /// <summary>
        ///     Parses an CSV document, and creates a p5.lambda node structure from the results
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.csv.csv2lambda")]
        public static void p5_csv_csv2lambda (ApplicationContext context, ActiveEventArgs e) {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new ArgsRemover (e.Args, true)) {

                // Loops through all documents we're supposed to transform.
                foreach (var idxCsvDoc in XUtil.Iterate<string> (context, e.Args)) {

                    // Converting currently iterated document, making sure we have our result node.
                    var curFile = e.Args.Add ("result").LastChild;

                    // Reading CSV content, by creating a string reader, which we supply to the CsvParser.
                    using (TextReader reader = new StringReader (idxCsvDoc)) {

                        // Creating our CSV parser.
                        var parser = new CsvParser (reader);

                        // Looping through each row in file.
                        while (true) {
                            var row = parser.Read ();
                            if (row == null)
                                break; // Finished!

                            // Adding current line into return value.
                            var curLine = curFile.Add ("").LastChild;
                            for (int idxCell = 0; idxCell < row.Length; idxCell++) {
                                curLine.Add (row [idxCell]);
                            }
                        }
                    }
                }
            }
        }
    }
}
