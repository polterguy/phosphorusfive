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

using System;
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
            // Sanity check.
            var configConnectionStringInput = e.Args.GetExValue<string> (context);
            if (string.IsNullOrEmpty (configConnectionStringInput))
                throw new LambdaException ("No connection string given to [p5.mysql.connect]", e.Args, context);

            // Retrieving connection string from configuration file.
            var connectionStringConfigValue = ConfigurationManager.ConnectionStrings [configConnectionStringInput];
            if (connectionStringConfigValue == null)
                throw new LambdaException ("[p5.mysql.connect] couldn't find the specified connection string in your configuration file", e.Args, context);

            // Initializing and opening connection.
            var connectionString = connectionStringConfigValue.ConnectionString;
            using (var connection = new MySqlConnection (connectionString)) {

                // Storing connection in current context, making sure it's on top of "stack of connections".
                var connections = Connections (context);
                connections.Add (connection);

                // Opening connection.
                connection.Open ();

                // Evaluating lambda for current connection.
                context.RaiseEvent ("eval-mutable", e.Args);

                // Cleaning up ...
                connection.Close ();
                connections.RemoveAt (connections.Count - 1);
            }
        }

        /*
         * Returns the current (active) connection.
         */
        internal static MySqlConnection Active (ApplicationContext context)
        {
            var connections = Connections (context);
            return connections.Count > 0 ? connections [connections.Count - 1] : null;
        }

        /*
         * List of connections.
         */
        private static List<MySqlConnection> Connections (ApplicationContext context)
        {
            var node = context.RaiseEvent (".p5.web.context.get", new Node (".p5.web.context.get", ".p5.mysql.connections"));
            if (node.Count == 0) {
                var connections = new List<MySqlConnection> ();
                context.RaiseEvent (
                    ".p5.web.context.set",
                    new Node (".p5.web.context.set", ".p5.mysql.connections", new Node [] { new Node ("src", connections) }));
                return connections;
            }
            return node [0].Get<List<MySqlConnection>> (context);
        }
    }
}
