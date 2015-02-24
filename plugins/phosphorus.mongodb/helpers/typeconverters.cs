
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 * MongoDB is licensed as Affero GPL, see the enclosed README.md file for details, 
 * since this file is linking towards the MongoDB .net driver, it hence needs to
 * be redistrivuted according to the Affero GPL license
 */

using System;
using System.Configuration;
using MongoDB.Bson;
using phosphorus.core;

namespace phosphorus.mongodb
{
    /// <summary>
    /// contains Hyperlisp type converters for MongoDB types, such as ObjectId, etc
    /// </summary>
    public static class typeconverters
    {
        /// <summary>
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlisp.get-object-value.objectid")]
        private static void pf_hyperlisp_get_object_value_objectid (ApplicationContext context, ActiveEventArgs e)
        {
            string strValue = e.Args.Get<string> (context);
            e.Args.Value = ObjectId.Parse (strValue);
        }

        /// <summary>
        /// returns "objectid" for using as type information for the MongoDB.Bson.ObjectId type in hyperlisp
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlisp.get-type-name.MongoDB.Bson.ObjectId")]
        [ActiveEvent (Name = "pf.hyperlisp.get-type-name.MongoDB.Bson.BsonObjectId")]
        private static void pf_hyperlisp_get_type_name_MongoDB_Bson_ObjectId (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "objectid";
        }
    }
}
