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

using System.Collections.Generic;
using p5.core;
using MySql.Data.MySqlClient;

namespace p5.mysql
{
    /// <summary>
    ///     Connection pool instance Active Event handler, for storing connections to MySQL instances.
    /// </summary>
    public class ConnectionPool
    {
        // All active connections.
        List<MySqlConnection> _connections = new List<MySqlConnection> ();

        /// <summary>
        ///     Returns all active connections.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.mysql.connections.get")]
        public void _p5_mysql_connections_get (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = _connections;
        }
    }
}
