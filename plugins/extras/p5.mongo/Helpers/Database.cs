/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Linq;
using System.Configuration;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

/// <summary>
///     Main namespace for all helpers in p5.mongo project
/// </summary>
namespace p5.mongo.helpers
{
    /// <summary>
    ///     Class wrapping the MongoDB database connection instance
    /// </summary>
    public class Database
    {
        private static Database _instance;
        private IMongoClient _client;
        private IMongoDatabase _database;

        /*
         * Private CTOR, class is a Singleton
         */
        private Database ()
        {
            // Connecting to MongoDB client
            var connectionString = ConfigurationManager.AppSettings ["mongo-connection-string"];
            if (!string.IsNullOrEmpty (connectionString)) {

                // Using connection string to specific instance/port/etc
                _client = new MongoClient (connectionString);
            } else {

                // Using default localhost instance
                _client = new MongoClient ();
            }

            // Opening up database
            var databaseName = ConfigurationManager.AppSettings ["mongo-database"];
            _database = _client.GetDatabase (databaseName);
        }

        /// <summary>
        ///     Gets the Instance for the Singleton object
        /// </summary>
        /// <value>The instance</value>
        public static Database Instance {
            get {
                if (_instance == null) {
                    lock (typeof (Database)) {
                        if (_instance == null)
                            _instance = new Database ();
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        ///     Gets the MongoDB client
        /// </summary>
        /// <value>The client</value>
        public IMongoClient Client {
            get { return _client; }
        }

        /// <summary>
        ///     Gets the MongoDB database used by system
        /// </summary>
        /// <value>The database</value>
        public IMongoDatabase MongoDatabase {
            get { return _database; }
        }
    }
}

