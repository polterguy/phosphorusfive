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

namespace p5.types.types {
    /// <summary>
    ///     Class helps converts from boolean to string, and vice versa
    /// </summary>
    public static class BoolType
    {
        /// <summary>
        ///     Creates a bool from its string representation
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.hyperlambda.get-object-value.bool")]
        private static void p5_hyperlisp_get_object_value_bool (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value is bool) {
                return;
            } else {
                if ((e.Args.Get<string> (context) ?? "").ToLower () == "false")
                    e.Args.Value = false;
                else
                    e.Args.Value = e.Args.Value != null;
            }
        }

        /// <summary>
        ///     Creates a string from a bool
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.hyperlambda.get-string-value.System.Boolean")]
        private static void p5_hyperlisp_get_string_value_System_Boolean (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = e.Args.Get<bool> (context).ToString ().ToLower ();
        }

        /// <summary>
        ///     Returns the Hyperlambda type-name for the bool type
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.hyperlambda.get-type-name.System.Boolean")]
        private static void p5_hyperlisp_get_type_name_System_Boolean (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "bool";
        }
    }
}
