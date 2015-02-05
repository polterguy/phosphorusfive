
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.Globalization;
using phosphorus.core;

namespace phosphorus.hyperlisp
{
    /// <summary>
    /// helper class for converting from string value representation to actual object
    /// </summary>
    public static class getObjectValue
    {
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
            string code = e.Args.Get<string> (context);
            Node tmp = new Node (string.Empty, code);
            context.Raise ("pf.hyperlisp.hyperlisp2lambda", tmp);
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
            e.Args.Value = new Node.DNA (e.Args.Get<string> (context));
        }

        /// <summary>
        /// returns a guid created from its string representation
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-object-value.guid")]
        private static void pf_hyperlist_get_object_value_guid (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = Guid.Parse (e.Args.Get<string> (context));
        }
        
        /// <summary>
        /// returns a long created from its string representation
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-object-value.long")]
        private static void pf_hyperlist_get_object_value_long (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = long.Parse (e.Args.Get<string> (context), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// returns a ulong created from its string representation
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-object-value.ulong")]
        private static void pf_hyperlist_get_object_value_ulong (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = ulong.Parse (e.Args.Get<string> (context), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// returns an integer created from its string representation
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-object-value.int")]
        private static void pf_hyperlist_get_object_value_int (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = int.Parse (e.Args.Get<string> (context), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// returns an integer created from its string representation
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-object-value.uint")]
        private static void pf_hyperlist_get_object_value_uint (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = uint.Parse (e.Args.Get<string> (context), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// returns a 16 bit integer created from its string representation
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-object-value.short")]
        private static void pf_hyperlist_get_object_value_short (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = short.Parse (e.Args.Get<string> (context), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// returns a single or 32 bit floating point number created from its string representation
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-object-value.float")]
        private static void pf_hyperlist_get_object_value_float (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = float.Parse (e.Args.Get<string> (context), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// returns a double or 64 bit floating point number created from its string representation
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-object-value.double")]
        private static void pf_hyperlist_get_object_value_double (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = Double.Parse (e.Args.Get<string> (context), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// returns a decimal number created from its string representation
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-object-value.decimal")]
        private static void pf_hyperlist_get_object_value_decimal (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = decimal.Parse (e.Args.Get<string> (context), CultureInfo.InvariantCulture);
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
            e.Args.Value = e.Args.Get<string> (context).ToLower () == "true";
        }

        /// <summary>
        /// returns a byte value from its string representation
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-object-value.byte")]
        private static void pf_hyperlist_get_object_value_byte (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = byte.Parse (e.Args.Get<string> (context), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// returns a byte[] value from its string representation
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-object-value.blob")]
        private static void pf_hyperlist_get_object_value_blob (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = Convert.FromBase64String (e.Args.Get<string> (context));
        }

        /// <summary>
        /// returns a sbyte value from its string representation
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-object-value.sbyte")]
        private static void pf_hyperlist_get_object_value_sbyte (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = sbyte.Parse (e.Args.Get<string> (context), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// returns a char value from its string representation
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-object-value.char")]
        private static void pf_hyperlist_get_object_value_char (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = char.Parse (e.Args.Get<string> (context));
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
            string strDate = e.Args.Get<string> (context);
            if (strDate.Length == 10)
                e.Args.Value = DateTime.ParseExact (strDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            else if (strDate.Length == 19)
                e.Args.Value = DateTime.ParseExact (strDate, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
            else if (strDate.Length == 23)
                e.Args.Value = DateTime.ParseExact (strDate, "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture);
            else
                throw new ArgumentException ("date; '" + strDate + "' is not recognized as a valid date");
        }
        
        /// <summary>
        /// returns a TimeSpan parsed from the given node's value
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlist.get-object-value.time")]
        private static void pf_hyperlist_get_object_value_time (ApplicationContext context, ActiveEventArgs e)
        {
            string str = e.Args.Get<string> (context);
            e.Args.Value = TimeSpan.ParseExact (str, "c", CultureInfo.InvariantCulture);
        }
    }
}
