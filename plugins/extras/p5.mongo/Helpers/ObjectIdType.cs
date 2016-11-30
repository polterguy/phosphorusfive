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
using MongoDB.Bson;

namespace p5.mongo.helpers
{
    /// <summary>
    ///     Class helps converts from BigInteger to string, and vice versa
    /// </summary>
    public static class ObjectIdType
    {
        /// <summary>
        ///     Creates an ObjectId from its string representation
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.hyperlambda.get-object-value.objectid")]
        private static void p5_hyperlisp_get_object_value_objectid (ApplicationContext context, ActiveEventArgs e)
        {
            var strValue = e.Args.Value as string;
            if (strValue != null) {
                e.Args.Value = ObjectId.Parse (strValue);
            } else {
                throw new LambdaException (
                    "Don't know how to convert that to a objectid",
                    e.Args, 
                    context);
            }
        }

        /// <summary>
        ///     Returns the Hyperlambda type-name for the ObjectId type
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.hyperlambda.get-type-name.MongoDB.Bson.ObjectId")]
        private static void p5_hyperlisp_get_type_name_MongoDB_Bson_ObjectId (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "objectid";
        }
    }
}
