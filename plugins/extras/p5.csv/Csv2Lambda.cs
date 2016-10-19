/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
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

using System;
using System.Linq;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

/// <summary>
///     Main namespace for handling CSV
/// </summary>
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
        [ActiveEvent (Name = "csv2lambda")]
        public static void csv2lambda (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution.
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Figuring out separator character, defaulting to ",".
                var sep = e.Args.GetExChildValue ("sep", context, ",");

                // Loops through all documents we're supposed to transform.
                foreach (var idxCsvDoc in XUtil.Iterate<string> (context, e.Args, true)) {

                    // Converting currently iterated document, making sure we have our result node.
                    var curFile = e.Args.Add ("result").LastChild;

                    // Splitting file into lines, and looping through each line in file.
                    int noEntities = -1;
                    int previousNoEntities = -1;
                    Node curLine = null;
                    var lines = idxCsvDoc.Split (new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var idxLine in lines) {

                        // Splitting line into entity.
                        var entities = idxLine.Trim ('\r', '\n').Split (new string[] { sep }, StringSplitOptions.None);

                        // Checking if we should set noEntities.
                        // Notice, this assumes the first record in file does NOT span multiple lines.
                        // Which usually is the case, since most CSV files have header rows.
                        // Wee need to do this, to allow for CSV files having records spanning multiple lines.
                        if (noEntities == -1) {

                            // We store the "previous line's number of entities" such that we can  support records spanning multiple lines.
                            noEntities = entities.Length;
                            previousNoEntities = noEntities;
                        }

                        // Making sure we create our record node, such that we can reuse it across multiple lines, where records
                        // are spanning multiple lines.
                        if (curLine == null || previousNoEntities == noEntities)
                            curLine = curFile.Add ("").LastChild;

                        // Sanity check.
                        // Notice, a record in our CSV file can span more than 2 lines, hence we only check for "more than".
                        if (previousNoEntities != noEntities && previousNoEntities + entities.Length > noEntities)
                            throw new LambdaException ("Malformed CSV file close to; '" + idxLine + "'", e.Args, context);

                        // Storing number of entities for next iteration.
                        previousNoEntities = entities.Length;

                        // Adding entities to curLine, making sure we accommodate for wrapping characters.
                        curLine.AddRange (entities.Select (ix => new Node (ix.Trim ('"').Replace ("\"\"", "\""))));
                    }
                }
            }
        }
    }
}
