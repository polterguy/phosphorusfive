/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, isa.lightbringer@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 * MongoDB is licensed as Affero GPL, see the enclosed README.md file for details, 
 * since this file is linking towards the MongoDB .net driver, it hence needs to
 * be redistrivuted according to the Affero GPL license
 */

using MongoDB.Bson;
using p5.core;

namespace p5.mongodb.helpers
{
    /// <summary>
    ///     contains Hyperlisp type converters for MongoDB types, such as ObjectId, etc
    /// </summary>
    public static class TypeConverters
    {
        /// <summary>
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-object-value.objectid")]
        private static void p5_hyperlisp_get_object_value_objectid (ApplicationContext context, ActiveEventArgs e)
        {
            var strValue = e.Args.Get<string> (context);
            e.Args.Value = ObjectId.Parse (strValue);
        }

        /// <summary>
        ///     returns "objectid" for using as type information for the MongoDB.Bson.ObjectId type in hyperlisp
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-type-name.MongoDB.Bson.ObjectId")]
        [ActiveEvent (Name = "p5.hyperlisp.get-type-name.MongoDB.Bson.BsonObjectId")]
        private static void p5_hyperlisp_get_type_name_MongoDB_Bson_ObjectId (ApplicationContext context, ActiveEventArgs e) { e.Args.Value = "objectid"; }
    }
}