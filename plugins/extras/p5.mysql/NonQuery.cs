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

using p5.core;
using p5.exp.exceptions;
using MySql.Data.MySqlClient;

namespace p5.mysql
{
    /// <summary>
    ///     Class wrapping [p5.mysql.insert] and [p5.mysql.update].
    /// </summary>
public static class NonQuery
    {
        /// <summary>
        ///     Inserts or updates data in the given database [p5.mysql.connect] has previously connected to.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.mysql.insert")]
        [ActiveEvent (Name = "p5.mysql.update")]
        public static void p5_mysql_insert_update (ApplicationContext context, ActiveEventArgs e)
        {
            // Getting connection, and doing some basic sanity check.
            var connection = Connection.Active (context);
            if (connection == null)
                throw new LambdaException ("No connection has been opened, use [p5.mysql.connect] before trying to invoke this event", e.Args, context);

            // Retrieving SQL and running query, returning result to caller. Making sure we run basic sanity check.
            var sql = e.Args.Get<string> (context);
            if (string.IsNullOrEmpty (sql))
                throw new LambdaException ("No SQL or query given to [p5.mysql.insert/update]", e.Args, context);

            // Creating command object.
            using (var cmd = new MySqlCommand (sql, connection)) {

                // Executing non-query, returning affected records to caller.
                e.Args.Value = cmd.ExecuteNonQuery ();
            }
        }
    }
}
