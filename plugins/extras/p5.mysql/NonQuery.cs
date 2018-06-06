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

using p5.exp;
using p5.core;
using p5.exp.exceptions;
using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using CsvHelper;
using MySql.Data.MySqlClient;

namespace p5.mysql
{
    /// <summary>
    ///     Class wrapping [p5.mysql.insert] and [p5.mysql.update].
    /// </summary>
    public static class NonQuery
    {
        /*
         * Internal helper to map our CSV file to a generic type.
         */
        private class CsvRecord {
            public string [] Fields { get; set; }
        }

        /// <summary>
        ///     Inserts or updates data in the given database [p5.mysql.connect] has previously connected to.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.mysql.insert")]
        [ActiveEvent (Name = "p5.mysql.update")]
        [ActiveEvent (Name = "p5.mysql.delete")]
        [ActiveEvent (Name = "p5.mysql.execute")]
        public static void p5_mysql_execute (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves.
            using (new ArgsRemover (e.Args, false)) {

                // Getting connection, and doing some basic sanity check.
                var connection = Connection.Active (context, e.Args);
                if (connection == null)
                    throw new LambdaException ("No connection has been opened, use [p5.mysql.connect] before trying to invoke this event", e.Args, context);

                // Creating command object.
                using (var cmd = e.Args.GetSqlCommand (context, connection)) {

                    // Executing non-query, returning affected records to caller, and insert id if relevant.
                    e.Args.Value = cmd.ExecuteNonQuery ();

                    if (e.Name == "p5.mysql.insert")
                        e.Args.Add ("id", cmd.LastInsertedId);
                }
            }
        }

        /// <summary>
        ///     Loads a CSV file into specified tableß.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.mysql.load-from")]
        public static void p5_mysql_load_from (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up after ourselves.
            using (new ArgsRemover (e.Args, false)) {

                // Getting connection, and doing some basic sanity check.
                var connection = Connection.Active (context, e.Args);
                if (connection == null)
                    throw new LambdaException ("No connection has been opened, use [p5.mysql.connect] before trying to invoke this event", e.Args, context);

                // Figuring out file to import from.
                var infile = e.Args.GetExChildValue<string> ("infile", context, null);
                if (string.IsNullOrEmpty (infile))
                    throw new LambdaException ("No [infile] supplied to [p5.mysql.load-from],", e.Args, context);
                
                // Figuring out table to import into.
                var table = e.Args.GetExChildValue<string> ("table", context, null);
                if (string.IsNullOrEmpty (table))
                    throw new LambdaException ("No [table] supplied to [p5.mysql.load-from],", e.Args, context);

                // Unrolling [infile].
                infile = context.RaiseEvent ("p5.io.unroll-path", new Node ("", infile).Add ("args", e.Args)).Get<string> (context);

                // Verifying user is authorized to reading from [infile].
                context.RaiseEvent (".p5.io.authorize.read-file", new Node ("", infile).Add ("args", e.Args));
                
                // Getting server's root filepath.
                string rootFolder = context.RaiseEvent (".p5.core.application-folder").Get<string> (context);
                
                // Figuring our DB types of columns.
                var types = GetDbTypes (connection, table);

                // Opening up [infile] in read-only mode.
                using (var stream = File.OpenText (rootFolder + infile)) {

                    // Using the generic "CsvParser" since we've got no idea of the file's structure.
                    using (var csv = new CsvParser (stream)) {

                        // Reading the headers.
                        var headers = csv.Read ();

                        // Creating our insert SQL, which is reused, only with different parameters.
                        string insertSql = CreateInsertSQL (table, types, headers);

                        // Iterating through each record in file.
                        var record = csv.Read ();
                        long no = 0;
                        while (record != null) {

                            // Creating our SQL command.
                            var cmd = new MySqlCommand (insertSql, connection);

                            // Adding SQL parameters to our command.
                            for (var idx = 0; idx < headers.Length; idx++) {
                                if (types [headers [idx]] != MySqlDbType.Timestamp) {
                                    var val = Convert (record [idx], types, headers [idx]);
                                    cmd.Parameters.AddWithValue ("@val" + idx, val);
                                }
                            }

                            // Executing our SQL command.
                            cmd.ExecuteNonQuery ();

                            // Reading our next record.
                            record = csv.Read ();

                            // Incrementing counter.
                            no += 1;
                        }
                        e.Args.Value = no;
                    }
                }
            }
        }

        /*
         * Helper for above.
         */
        private static string CreateInsertSQL (string table, Dictionary<string, MySqlDbType> types, string [] headers)
        {
            // Creating our reused insert SQL command text.
            var insertSql = string.Format ("insert into `{0}` (", table);
            var first = true;
            foreach (var idxHeader in headers) {
                
                // Sanity checking column name
                foreach (var idxChar in idxHeader) {
                    if ("0123456789_-abcdefghijklmnopqrstuvwxyz".IndexOf (idxChar.ToString ().ToLower (CultureInfo.InvariantCulture), StringComparison.InvariantCulture) == -1)
                        throw new System.Security.SecurityException ("Unsupported and insecure column name found in your CSV file.");
                }

                // Skipping TIMESTAMP columns.
                if (types [idxHeader] == MySqlDbType.Timestamp)
                    continue;
                if (first)
                    first = false;
                else
                    insertSql += ",";
                insertSql += "`" + idxHeader + "`";
            }
            insertSql += ") values (";
            first = true;
            for (var idx = 0; idx < headers.Length; idx++) {

                // Skipping TIMESTAMP columns.
                if (types [headers [idx]] == MySqlDbType.Timestamp)
                    continue;
                if (first)
                    first = false;
                else
                    insertSql += ",";
                insertSql += "@val" + idx;
            }
            insertSql += ")";
            return insertSql;
        }

        private static object Convert (string val, Dictionary<string, MySqlDbType> types, string colName)
        {
            switch (types [colName]) {
                case MySqlDbType.Binary:
                    return val;
                case MySqlDbType.Bit:
                    return System.Convert.ChangeType (val, typeof (Int16), CultureInfo.InvariantCulture);
                case MySqlDbType.Blob:
                    return Encoding.UTF8.GetBytes (val);
                case MySqlDbType.Byte:
                    return System.Convert.ChangeType (val, typeof (byte), CultureInfo.InvariantCulture);
                case MySqlDbType.Date:
                    return DateTime.ParseExact (val, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                case MySqlDbType.DateTime:
                    return DateTime.ParseExact (val, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                case MySqlDbType.Decimal:
                    return System.Convert.ChangeType (val, typeof (decimal), CultureInfo.InvariantCulture);
                case MySqlDbType.Double:
                    return System.Convert.ChangeType (val, typeof (double), CultureInfo.InvariantCulture);
                case MySqlDbType.Enum:
                    return val;
                case MySqlDbType.Float:
                    return System.Convert.ChangeType (val, typeof (float), CultureInfo.InvariantCulture);
                case MySqlDbType.Guid:
                    return System.Convert.ChangeType (val, typeof (Guid), CultureInfo.InvariantCulture);
                case MySqlDbType.Int16:
                    return System.Convert.ChangeType (val, typeof (Int16), CultureInfo.InvariantCulture);
                case MySqlDbType.Int24:
                    return System.Convert.ChangeType (val, typeof (Int32), CultureInfo.InvariantCulture);
                case MySqlDbType.Int32:
                    return System.Convert.ChangeType (val, typeof (Int32), CultureInfo.InvariantCulture);
                case MySqlDbType.Int64:
                    return System.Convert.ChangeType (val, typeof (long), CultureInfo.InvariantCulture);
                case MySqlDbType.JSON:
                    return val;
                case MySqlDbType.LongBlob:
                    return Encoding.UTF8.GetBytes (val);
                case MySqlDbType.LongText:
                    return val;
                case MySqlDbType.MediumBlob:
                    return Encoding.UTF8.GetBytes (val);
                case MySqlDbType.MediumText:
                    return val;
                case MySqlDbType.NewDecimal:
                    return System.Convert.ChangeType (val, typeof (decimal), CultureInfo.InvariantCulture);
                case MySqlDbType.Set:
                    return val;
                case MySqlDbType.String:
                    return val;
                case MySqlDbType.Text:
                    return val;
                case MySqlDbType.Time:
                    return TimeSpan.ParseExact (val, "c", CultureInfo.InvariantCulture);
                case MySqlDbType.TinyBlob:
                    return Encoding.UTF8.GetBytes (val);
                case MySqlDbType.TinyText:
                    return val;
                case MySqlDbType.UByte:
                    return System.Convert.ChangeType (val, typeof (byte), CultureInfo.InvariantCulture);
                case MySqlDbType.UInt16:
                    return System.Convert.ChangeType (val, typeof (UInt16), CultureInfo.InvariantCulture);
                case MySqlDbType.UInt24:
                    return System.Convert.ChangeType (val, typeof (UInt32), CultureInfo.InvariantCulture);
                case MySqlDbType.UInt32:
                    return System.Convert.ChangeType (val, typeof (UInt32), CultureInfo.InvariantCulture);
                case MySqlDbType.UInt64:
                    return System.Convert.ChangeType (val, typeof (ulong), CultureInfo.InvariantCulture);
                case MySqlDbType.VarBinary:
                    return val;
                case MySqlDbType.VarChar:
                    return val;
                case MySqlDbType.VarString:
                    return val;
                case MySqlDbType.Year:
                    return System.Convert.ChangeType (val, typeof (int), CultureInfo.InvariantCulture);
                default:
                    throw new ArgumentException ("You shouldn't have ended up here!");
            }
        }
        
        /*
         * Helper for above.
         */
        private static Dictionary<string, MySqlDbType> GetDbTypes (MySqlConnection connection, string table)
        {
            // Our return value.
            var retVal = new Dictionary<string, MySqlDbType> ();
            
            // Creating our SQL command.
            var cmd = new MySqlCommand (
                string.Format ("select column_name, data_type from information_schema.columns where table_name = '{0}' and table_schema='{1}';",
                               table, connection.Database),
                connection);

            // Creating SQL data reader, and iterating as long as we have resulting rows.
            using (var reader = cmd.ExecuteReader ()) {
                while (reader.Read ()) {
                    retVal [(string)reader [0]] = Convert ((string)reader [1]);
                }
            }

            // Returning mappings to caller.
            return retVal;
        }
        
        private static MySqlDbType Convert (string name)
        {
            switch (name.ToUpper (CultureInfo.InvariantCulture)) {
                case "CHAR": return MySqlDbType.String;
                case "VARCHAR": return MySqlDbType.VarChar;
                case "DATE": return MySqlDbType.Date;
                case "DATETIME": return MySqlDbType.DateTime;
                case "NUMERIC":
                case "DECIMAL":
                case "DEC":
                case "FIXED":
                    return MySqlDbType.NewDecimal;
                case "YEAR":
                    return MySqlDbType.Year;
                case "TIME":
                    return MySqlDbType.Time;
                case "TIMESTAMP":
                    return MySqlDbType.Timestamp;
                case "SET": return MySqlDbType.Set;
                case "ENUM": return MySqlDbType.Enum;
                case "BIT": return MySqlDbType.Bit;
                case "TINYINT":
                    return MySqlDbType.Byte;
                case "BOOL":
                case "BOOLEAN":
                    return MySqlDbType.Byte;
                case "SMALLINT":
                    return MySqlDbType.Int16;
                case "MEDIUMINT":
                    return MySqlDbType.Int24;
                case "INT":
                case "INTEGER":
                    return MySqlDbType.Int32;
                case "SERIAL":
                    return MySqlDbType.UInt64;
                case "BIGINT":
                    return MySqlDbType.Int64;
                case "FLOAT": return MySqlDbType.Float;
                case "DOUBLE": return MySqlDbType.Double;
                case "REAL":
                    return MySqlDbType.Double;
                case "TEXT":
                    return MySqlDbType.Text;
                case "BLOB":
                    return MySqlDbType.Blob;
                case "LONGBLOB":
                    return MySqlDbType.LongBlob;
                case "LONGTEXT":
                    return MySqlDbType.LongText;
                case "MEDIUMBLOB":
                    return MySqlDbType.MediumBlob;
                case "MEDIUMTEXT":
                    return MySqlDbType.MediumText;
                case "TINYBLOB":
                    return MySqlDbType.TinyBlob;
                case "TINYTEXT":
                    return MySqlDbType.TinyText;
                case "BINARY":
                    return MySqlDbType.Binary;
                case "VARBINARY":
                    return MySqlDbType.VarBinary;
            }
            throw new ArgumentException ("Never encountered, hopefully!");
        }
    }
}
