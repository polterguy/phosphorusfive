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

using p5.core;
using p5.exp.exceptions;
using MySql.Data.MySqlClient;

namespace p5.mysql
{
    /// <summary>
    ///     Class wrapping [p5.mysql.scalar].
    /// </summary>
public static class Scalar
    {
        /// <summary>
        ///     Returns the resulting scalar value from specified SQL.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.mysql.scalar")]
        public static void p5_mysql_scalar (ApplicationContext context, ActiveEventArgs e)
        {
            // Getting connection, and doing some basic sanity check.
            var connection = Connection.Active (context, e.Args);
            if (connection == null)
                throw new LambdaException ("No connection has been opened, use [p5.mysql.connect] before trying to invoke this event", e.Args, context);

            // Making sure we clean up after ourselves.
            using (new ArgsRemover (e.Args, false)) {

	            // Creating command object.
	            using (var cmd = e.Args.GetSqlCommand (context, connection)) {

	                // Executing scalar, returning results to caller.
	                e.Args.Value = cmd.ExecuteScalar ();
	            }
            }
        }
    }
}
