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

using System.Linq;
using p5.exp;
using p5.core;
using MySql.Data.MySqlClient;

namespace p5.mysql
{
    /// <summary>
    ///     Class helping to avoid SQL injection attacks.
    /// </summary>
    public static class Parameters
    {
        /*
         * Creates a parametrized SQL command object.
         */
        internal static MySqlCommand GetSqlCommand (this Node node, ApplicationContext context, MySqlConnection connection)
        {
            MySqlCommand retVal = new MySqlCommand (node.GetExValue<string> (context, ""), connection);

            // Parametrizing SQL Command with all parameters (children nodes, not having "empty names".
            foreach (var idx in node.Children.Where (ix => ix.Name != "")) {
                retVal.Parameters.AddWithValue (idx.Name, idx.GetExValue<object> (context, null));
            }
            return retVal;
        }
    }
}
