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

using System.Configuration;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.exp.exceptions;
using MySql.Data.MySqlClient;

namespace p5.mysql
{
    /// <summary>
    ///     Class wrapping [p5.mysql.connect].
    /// </summary>
    public static class Connection
    {
        /// <summary>
        ///     Connects to a MySQL database, and executes the given lambda with the opened connection as the "current" connection.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.mysql.connect")]
        public static void p5_mysql_connect (ApplicationContext context, ActiveEventArgs e)
        {
            // Creating connection, opening it, and evaluating lambda for [p5.mysql.connect].
            using (var connection = new MySqlConnection (ConnectionString (context, e.Args))) {

                // Opening connection.
                connection.Open ();

                // Storing connection in current context, making sure it's on top of "stack of connections".
                var connections = Connections (context);
                connections.Add (connection);

                // Evaluating lambda for current connection, making sure we are able to remove connection, even if an exception occurs.
                try {

                    // Evaluating lambda for [p5.mysql.connect].
                    context.RaiseEvent ("eval-mutable", e.Args);

                } finally {

                    // Cleaning up ...
                    connections.Remove (connection);
                    connection.Close ();
                }
            }
        }

        /// <summary>
        ///     Returns the currently active database for MySQL, if any.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.mysql.database.get")]
        public static void p5_mysql_database_get (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves.
            using (new ArgsRemover (e.Args, false)) {

                // Retrieving active (top most) database, and returning to caller, if any.
                e.Args.Value = Active (context, e.Args).Database;
            }
        }

        /// <summary>
        ///     Sets the currently active database for MySQL, if any.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.mysql.database.set")]
        public static void p5_mysql_database_set (ApplicationContext context, ActiveEventArgs e)
        {
            // Changing active (top most) database.
            Active (context, e.Args).ChangeDatabase (e.Args.GetExValue<string> (context));
        }

        /*
         * Returns the current (active) connection.
         */
        internal static MySqlConnection Active (ApplicationContext context, Node args)
        {
            var connections = Connections (context);
            if (connections.Count == 0)
                throw new LambdaException ("No active database, make sure you invoke [p5.mysql.connect]", args, context);
            return connections [connections.Count - 1];
        }

        /*
         * List of connections.
         */
        private static List<MySqlConnection> Connections (ApplicationContext context)
        {
            // Checking if our connection pool is already registered in context, and if not, making sure we create it.
            if (!context.HasActiveEvent (".p5.mysql.connections.get")) {

                // Creating a new connection pool for our context, to handle all connections created for context.
                ConnectionPool pool = new ConnectionPool ();
                context.RegisterListeningInstance (pool);
            }
            return context.RaiseEvent (".p5.mysql.connections.get").Get<List<MySqlConnection>> (context);
        }

        /*
         * Returns connection string from arguments, helper for above.
         */
        private static string ConnectionString (ApplicationContext context, Node args)
        {
            // Retrieving input connection string, or reference to connection string, or first connection string from configuration file.
            var argsValue = args.GetExValue<string> (context);
            if (argsValue == null) {

                // Sanity check, in case expression leads into oblivion, we deny connection.
                if (args.Value != null)
                    throw new LambdaException ("That connection string wasn't found, make sure your expressions leads to an actual connection string when invoking [p5.mysql.connect], if you're using expressions to reference them", args, context);

                // Checking if we have a "default connection string", which is the first connection string from configuration file.
                var connectionStrings = ConfigurationManager.ConnectionStrings;
                if (connectionStrings.Count == 0)
                    throw new LambdaException ("No connection string given to [p5.mysql.connect], and no default connection string found in configuration file", args, context);

                // Returning first connection string from configuration file.
                return connectionStrings [0].ConnectionString;
            }

            // Checking if this is a reference to a connection string in our configuration file.
            if (argsValue.StartsWithEx ("[") && argsValue.EndsWithEx ("]")) {

                // Retrieving connection string from configuration file, trimming away square brackets.
                argsValue = argsValue.Substring (1, argsValue.Length - 2);
                var configValue = ConfigurationManager.ConnectionStrings [argsValue];
                if (configValue == null)
                    throw new LambdaException ("[p5.mysql.connect] couldn't find the specified connection string in your configuration file", args, context);

                // Initializing and opening connection.
                return configValue.ConnectionString;

            } else {

                // Assuming input was a complete connection string in itself.
                return argsValue;
            }
        }
    }
}
