/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Globalization;
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
        [ActiveEvent (Name = "p5.hyperlisp.get-object-value.objectid", Protection = EventProtection.NativeClosed)]
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
        ///     Returns the Hyperlisp type-name for the ObjectId type
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-type-name.MongoDB.Bson.ObjectId", Protection = EventProtection.NativeClosed)]
        private static void p5_hyperlisp_get_type_name_MongoDB_Bson_ObjectId (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "objectid";
        }
    }
}
