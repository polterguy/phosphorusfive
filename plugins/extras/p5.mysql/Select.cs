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

using p5.exp;
using p5.core;
using p5.exp.exceptions;
using MySql.Data.MySqlClient;

namespace p5.mysql
{
    /// <summary>
    ///     Class wrapping [p5.mysql.select].
    /// </summary>
    public static class Select
    {
        /// <summary>
        ///     Selects data from the given database [p5.mysql.connect] has previously connected to.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.mysql.select")]
        public static void p5_mysql_select (ApplicationContext context, ActiveEventArgs e) {
            // Making sure we clean up after ourselves.
            using (new ArgsRemover (e.Args, true)) {

                // Getting connection, and doing some basic sanity check.
                var connection = Connection.Active (context, e.Args);
                if (connection == null)
                    throw new LambdaException ("No connection has been opened, use [p5.mysql.connect] before trying to invoke this event", e.Args, context);

                // Creating command object.
                using (var cmd = e.Args.GetSqlCommand (context, connection)) {

                    // Creating reader, and iterating as long as we have resulting rows.
                    using (var reader = cmd.ExecuteReader ()) {
                        while (reader.Read ()) {

                            // Adding row.
                            var current = e.Args.Add ("row").LastChild;

                            // Looping through all columns in current result row, returning to caller.
                            for (int ix = 0; ix < reader.FieldCount; ix++) {

                                // Adding currently iterated cell for row.
                                object val = reader [ix];
                                if (val is System.DBNull)
                                    val = null;
                                current.Add (reader.GetName (ix), val);
                            }
                        }
                    }                }
            }
        }
    }
}
