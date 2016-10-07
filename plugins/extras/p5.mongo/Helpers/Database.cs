/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the Affero GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System.Configuration;
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

