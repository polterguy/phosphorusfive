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
    public static class typeconverters
    {
        /*
         * retrieving type name active events
         */

        [ActiveEvent (Name = "pf.hyperlist.get-type-name.System.Int32")]
        [ActiveEvent (Name = "pf.hyperlist.get-type-name.System.Int16")]
        private static void pf_hyperlist_get_type_name_System_int (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "int";
        }

        [ActiveEvent (Name = "pf.hyperlist.get-type-name.System.Single")]
        [ActiveEvent (Name = "pf.hyperlist.get-type-name.System.Double")]
        private static void pf_hyperlist_get_type_name_System_single_double (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "float";
        }

        [ActiveEvent (Name = "pf.hyperlist.get-type-name.System.Decimal")]
        private static void pf_hyperlist_get_type_name_System_Decimal (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "decimal";
        }
        
        [ActiveEvent (Name = "pf.hyperlist.get-type-name.System.Boolean")]
        private static void pf_hyperlist_get_type_name_System_Boolean (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "bool";
        }
        
        [ActiveEvent (Name = "pf.hyperlist.get-type-name.System.Byte")]
        private static void pf_hyperlist_get_type_name_System_Byte (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "byte";
        }
        
        [ActiveEvent (Name = "pf.hyperlist.get-type-name.System.Char")]
        private static void pf_hyperlist_get_type_name_System_Char (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "char";
        }
        
        [ActiveEvent (Name = "pf.hyperlist.get-type-name.System.DateTime")]
        private static void pf_hyperlist_get_type_name_System_DateTime (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "date";
        }
        
        [ActiveEvent (Name = "pf.hyperlist.get-type-name.System.TimeSpan")]
        private static void pf_hyperlist_get_type_name_System_TimeSpan (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "timespan";
        }
        
        [ActiveEvent (Name = "pf.hyperlist.get-type-name.phosphorus.core.Node")]
        private static void pf_hyperlist_get_type_name_Node (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "node";
        }

        /*
         * retrieving value in string format active events
         */
        
        [ActiveEvent (Name = "pf.hyperlist.get-string-value.System.DateTime")]
        private static void pf_hyperlist_get_string_value_System_DateTime (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = e.Args.Get<DateTime> () .ToString ("yyyy.MM.ddTHH:mm:ss", CultureInfo.InvariantCulture);
        }
        
        [ActiveEvent (Name = "pf.hyperlist.get-string-value.System.Boolean")]
        private static void pf_hyperlist_get_string_value_System_Boolean (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = e.Args.Get<bool> () .ToString ().ToLower ();
        }
    }
}

