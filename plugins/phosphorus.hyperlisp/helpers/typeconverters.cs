/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.hyperlisp
{
    /// <summary>
    /// helper class for converting between strings and objects for types supported by hyperlisp syntax
    /// </summary>
    public static class typeconverters
    {
        /*
         * hyperlisp type name Active Events for converting fully qualified typenames into hyperlisp typenames. if you
         * create your own types that you wish to store in hyperlisp using some sort of string representation, representing
         * your objects, then you need to implement your own hyperlisp type name conversion Active Events. these Active Events
         * must be named "pf.hyperlisp.get-type-name.*", where "*" is the fully qualified name of your type, meaning
         * typeof(MyType).FullName
         * 
         * if you wish for hyperlisp to support your own custom types, you will have to implement three Active Events;
         * 
         *   1. "pf.hyperlisp.get-type-name.*" - (*) being the fully qualified name of your type, or typeof (YourType).FullName,
         *      that returns your hyperlisp typename
         * 
         *   2. "pf.hyperlisp.get-string-value.*" - (*) being the fully qualified name of your type, or typeof (YourType).FullName,
         *      that returns a string representing your object
         * 
         *   3. "pf.hyperlisp.get-object-value.*" - (*) being the hyperlisp typename of your type, 
         *      returned from your "pf.hyperlisp.get-type-name.*",
         *      that returns an object created from the string representing your object
         */

        /*
         * retrieving type name active events
         */

        /// <summary>
        /// returns "node" for using as type information for the phosphorus.core.Node type in hyperlisp
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-type-name.phosphorus.core.Node")]
        private static void pf_hyperlist_get_type_name_phosphorus_core_Node (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "node";
        }

        /// <summary>
        /// returns "path" for using as type information for the phosphorus.core.Node+DNA type in hyperlisp
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-type-name.phosphorus.core.Node+DNA")]
        private static void pf_hyperlist_get_type_name_phosphorus_core_Node_DNA (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "path";
        }

        /// <summary>
        /// returns "int" for using as type information for the given System type in hyperlisp
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-type-name.System.Int32")]
        private static void pf_hyperlist_get_type_name_System_Int32 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "int";
        }

        /// <summary>
        /// returns "int16" for using as type information for the given System type in hyperlisp
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-type-name.System.Int16")]
        private static void pf_hyperlist_get_type_name_System_Int16 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "int16";
        }

        /// <summary>
        /// returns "single" for using as type information for the given System type in hyperlisp
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-type-name.System.Single")]
        private static void pf_hyperlist_get_type_name_System_Single (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "single";
        }

        /// <summary>
        /// returns "double" for using as type information for the given System type in hyperlisp
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-type-name.System.Double")]
        private static void pf_hyperlist_get_type_name_System_Double (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "double";
        }

        /// <summary>
        /// returns "decimal" for using as type information for the given System type in hyperlisp
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-type-name.System.Decimal")]
        private static void pf_hyperlist_get_type_name_System_Decimal (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "decimal";
        }
        
        /// <summary>
        /// returns "bool" for using as type information for the given System type in hyperlisp
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-type-name.System.Boolean")]
        private static void pf_hyperlist_get_type_name_System_Boolean (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "bool";
        }
        
        /// <summary>
        /// returns "byte" for using as type information for the given System type in hyperlisp
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-type-name.System.Byte")]
        private static void pf_hyperlist_get_type_name_System_Byte (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "byte";
        }
        
        /// <summary>
        /// returns "char" for using as type information for the given System type in hyperlisp
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-type-name.System.Char")]
        private static void pf_hyperlist_get_type_name_System_Char (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "char";
        }
        
        /// <summary>
        /// returns "date" for using as type information for the given System type in hyperlisp
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-type-name.System.DateTime")]
        private static void pf_hyperlist_get_type_name_System_DateTime (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "date";
        }
        
        /// <summary>
        /// returns "time" for using as type information for the given System type in hyperlisp
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-type-name.System.TimeSpan")]
        private static void pf_hyperlist_get_type_name_System_TimeSpan (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "time";
        }

        /*
         * retrieving value in string format Active Events. all types that support automatic conversion
         * from their object representation to a string, do not need their own event handlers, since the
         * default logic is to use "Convert.ChangeType". hence you only need to implement Active Event converters
         * for types that do not implement IConvertible, or whos default implementation of IConvertible is
         * not sufficient for creating a [*sane*] string representation of your object. examples are DateTime and 
         * bool, since it creates a non-ISO date string representation by default, and Boolean, since it creates "True" and
         * "False", instead of "true" and "false" - [capital letters are avoided in hyperlisp, if we can]
         * 
         * the name of all of these Active Events is "pf.hyperlist.get-string-value." + the fully qualified name of your type,
         * or the return value of "typeof(YourType).FullName"
         */

        /// <summary>
        /// returns Node as string value
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-string-value.phosphorus.core.Node")]
        private static void pf_hyperlist_get_string_value_phosphorus_core_Node (ApplicationContext context, ActiveEventArgs e)
        {
            Node node = e.Args.Get<Node> ();
            Node tmp = new Node ();
            tmp.Add (node.Clone ());
            context.Raise ("pf.nodes-2-code", tmp);
            if (tmp.Get<string> () == @"""""")
                e.Args.Value = string.Empty; // node contained one node, with no value, and its name was empty
            else
                e.Args.Value = tmp.Value;
        }

        /// <summary>
        /// returns date's value in ISO string representation format, meaning date in "yyyy.MM.ddTHH:mm:ss" format
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-string-value.System.DateTime")]
        private static void pf_hyperlist_get_string_value_System_DateTime (ApplicationContext context, ActiveEventArgs e)
        {
            DateTime date = e.Args.Get<DateTime> ();
            if (date.Hour == 0 && date.Minute == 0 && date.Second == 0)
                e.Args.Value = date.ToString ("yyyy-MM-dd", CultureInfo.InvariantCulture);
            else if (date.Millisecond == 0)
                e.Args.Value = date.ToString ("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
            else
                e.Args.Value = date.ToString ("yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture);
        }
        
        /// <summary>
        /// returns "true" or "false" depending upon whether or not the given bool value is true or not
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-string-value.System.Boolean")]
        private static void pf_hyperlist_get_string_value_System_Boolean (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = e.Args.Get<bool> () .ToString ().ToLower ();
        }

        /*
         * the rest of the native System types are automatically returning a "sane" string value through their 
         * IConvertible implementation ...
         */

        /*
         * retrieving object value from string representation Active Events
         * 
         * the name of all of these Active Events is "pf.hyperlist.get-object-value." + the hyperlisp typename returned
         * in your "pf.hyperlist.get-type-name.*" Active Events. if you create support for your own types in hyperlisp, then
         * make sure you return an application wide unique hyperlisp typename in your "pf.hyperlist.get-type-name.*" Active 
         * Event. this can be done by using a "your-company.your-type" format for your hyperlisp type and "get-type-name"
         * Active Event
         */
        
        /// <summary>
        /// returns a Node created from its string representation
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-object-value.node")]
        private static void pf_hyperlist_get_object_value_node (ApplicationContext context, ActiveEventArgs e)
        {
            string code = e.Args.Get<string> ();
            Node tmp = new Node (string.Empty, code);
            context.Raise ("pf.code-2-nodes", tmp);
            if (tmp.Count == 1)
                e.Args.Value = tmp [0].Clone ();
            else if (tmp.Count > 1)
                throw new ArgumentException ("cannot use a node with multiple root nodes as a value of another node");
            else
                e.Args.Value = null;
        }
        
        /// <summary>
        /// returns a DNA created from its string representation
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-object-value.path")]
        private static void pf_hyperlist_get_object_value_path (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = new Node.DNA (e.Args.Get<string> ());
        }

        /// <summary>
        /// returns an integer created from its string representation
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-object-value.int")]
        private static void pf_hyperlist_get_object_value_int (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = int.Parse (e.Args.Get<string> ());
        }

        /// <summary>
        /// returns a 16 bit integer created from its string representation
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-object-value.int16")]
        private static void pf_hyperlist_get_object_value_int16 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = Int16.Parse (e.Args.Get<string> ());
        }

        /// <summary>
        /// returns a single or 32 bit floating point number created from its string representation
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-object-value.single")]
        private static void pf_hyperlist_get_object_value_single (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = Single.Parse (e.Args.Get<string> ());
        }

        /// <summary>
        /// returns a double or 64 bit floating point number created from its string representation
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-object-value.double")]
        private static void pf_hyperlist_get_object_value_double (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = Double.Parse (e.Args.Get<string> ());
        }

        /// <summary>
        /// returns a decimal number created from its string representation
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-object-value.decimal")]
        private static void pf_hyperlist_get_object_value_decimal (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = decimal.Parse (e.Args.Get<string> ());
        }
        
        /// <summary>
        /// returns a Boolean value being either true or false depending upon whether or not the the string given it
        /// is "true" or not. meaning "true", "True" and "trUE" becomes true, all other values becomes false
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-object-value.bool")]
        private static void pf_hyperlist_get_object_value_bool (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = e.Args.Get<string> ().ToLower () == "true";
        }

        /// <summary>
        /// returns a byte value from its string representation
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-object-value.byte")]
        private static void pf_hyperlist_get_object_value_byte (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = byte.Parse (e.Args.Get<string> ());
        }

        /// <summary>
        /// returns a char value from its string representation
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-object-value.char")]
        private static void pf_hyperlist_get_object_value_char (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = char.Parse (e.Args.Get<string> ());
        }

        /// <summary>
        /// returns a DateTime from the given string representation, expected to be in format "yyyy.MM.ddTHH:mm:ss", which
        /// is an ISO valid date represented using its string representation
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-object-value.date")]
        private static void pf_hyperlist_get_object_value_date (ApplicationContext context, ActiveEventArgs e)
        {
            string strDate = e.Args.Get<string> ();
            if (strDate.Length == 10)
                e.Args.Value = DateTime.ParseExact (strDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            else if (strDate.Length == 19)
                e.Args.Value = DateTime.ParseExact (strDate, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
            else if (strDate.Length == 23)
                e.Args.Value = DateTime.ParseExact (strDate, "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture);
            else
                throw new ArgumentException ("date; '" + strDate + "' is not recognized as a valid date");
        }
    }
}

