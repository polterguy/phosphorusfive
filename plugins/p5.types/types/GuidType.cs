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
using p5.core;

namespace p5.types.types
{
    /// <summary>
    ///     Class helps converts from Guid to string, and vice versa
    /// </summary>
    public static class GuidType
    {
        /// <summary>
        ///     Returns a new randomly created Guid
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.types.guid.new")]
        static void p5_types_guid_new (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = Guid.NewGuid ();
        }

        /// <summary>
        ///     Creates a Guid from its string representation
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.hyperlambda.get-object-value.guid")]
        static void p5_hyperlisp_get_object_value_guid (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value is Guid) {
                return;
            }
            e.Args.Value = Guid.Parse (e.Args.Get<string> (context));
        }

        /// <summary>
        ///     Returns the Hyperlambda type-name for the Guid type
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.hyperlambda.get-type-name.System.Guid")]
        static void p5_hyperlisp_get_type_name_System_Guid (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "guid";
        }
    }
}
