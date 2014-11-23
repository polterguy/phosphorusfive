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
         * will be named "pf.hyperlisp.get-type-name.*", where "*" is the fully qualified name of your type, meaning
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
         * 
         * pst, Node is automatically handled in the hyperlisp engine, since it is native to hyperlisp, *obviously*...!!
         */

        /*
         * retrieving type name active events
         */

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
         * for types who does not implement IConvertible, or whos default implementation of IConvertible is
         * not sufficient for creating a [*sane*] string representation of your object, such as DateTime, since it
         * creates a non-ISO date string representation by default, and Boolean, since it creates "True" and
         * "False", instead of "true" and "false" - [capital letters are avoided in hyperlisp, if we can]
         * 
         * the name of all of these Active Events is "pf.hyperlist.get-string-value." + the hyperlisp typename returned
         * in your "pf.hyperlist.get-type-name.*" Active Events. if you create your own type conversion Active Events,
         * then please make sure that your Active Events have an application wide unique name. this can be done by prefixing
         * your type with the name of your company, such as "pf.hyperlist.get-object-value.my-company.my-type", or something
         * similar
         * 
         * pst, nodes are handled automatically by hyperlisp engine, since they're native to the engine, *obviously* ...!
         */

        /// <summary>
        /// returns date in ISO string representation as "yyyy.MM.ddTHH:mm:ss"
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-string-value.System.DateTime")]
        private static void pf_hyperlist_get_string_value_System_DateTime (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = e.Args.Get<DateTime> () .ToString ("yyyy.MM.ddTHH:mm:ss", CultureInfo.InvariantCulture);
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
         * retrieving object value from string representation Active Events
         * 
         * the name of all of these Active Events is "pf.hyperlist.get-object-value." + the hyperlisp typename returned
         * in your "pf.hyperlist.get-type-name.*" Active Events
         * 
         * pst, nodes are handled automatically by hyperlisp engine, since they're native to the engine, *obviously* ...!
         */
        
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
            e.Args.Value = DateTime.ParseExact (e.Args.Get<string> (), "yyyy.MM.ddTHH:mm:ss", CultureInfo.InvariantCulture);
        }
    }
}

