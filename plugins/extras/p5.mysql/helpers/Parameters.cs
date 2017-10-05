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
using System.Collections.Generic;
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

                // Checking if this is an "array parameter".
                if (idx.Children.Any (ix => ix.Name != "")) {

                    // Array parameter.
                    AddArrayParameters (retVal, idx.Name, idx.Children.Select (ix => ix.Value != null ? ix.Value : ix.Name));

                } else {

                    // Simple parameter.
                    retVal.Parameters.AddWithValue (idx.Name, idx.GetExValue<object> (context, null));
                }
            }
            return retVal;
        }

        /*
         * Helper for above, to add arrays of parameters to SQL command.
         */
        private static void AddArrayParameters (
            MySqlCommand cmd,
            string originalParamName,
            IEnumerable<object> parameterValues)
        {
            var names = new List<string> ();
            var idxNo = 1;
            foreach (var idxVal in parameterValues) {
                var name = string.Format ("{0}{1}", originalParamName, idxNo++);
                names.Add (name);
                cmd.Parameters.AddWithValue (name, idxVal);
            }

            cmd.CommandText = cmd.CommandText.Replace (originalParamName, string.Join (",", names));
        }
    }
}
