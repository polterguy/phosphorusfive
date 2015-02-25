
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using phosphorus.core;

namespace phosphorus.hyperlisp
{
    /// <summary>
    /// helper class for converting between strings and objects for types supported by hyperlisp syntax
    /// </summary>
    public static class getTypeName
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
        [ActiveEvent (Name = "pf.hyperlisp.get-type-name.phosphorus.core.Node")]
        private static void pf_hyperlisp_get_type_name_phosphorus_core_Node (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "node";
        }

        /// <summary>
        /// returns "path" for using as type information for the phosphorus.core.Node+DNA type in hyperlisp
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlisp.get-type-name.phosphorus.core.Node+DNA")]
        private static void pf_hyperlisp_get_type_name_phosphorus_core_Node_DNA (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "path";
        }

        /// <summary>
        /// returns "guid" for using as type information for the given System type in hyperlisp
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlisp.get-type-name.System.Guid")]
        private static void pf_hyperlisp_get_type_name_System_Guid (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "guid";
        }

        /// <summary>
        /// returns "long" for using as type information for the given System type in hyperlisp
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlisp.get-type-name.System.Int64")]
        private static void pf_hyperlisp_get_type_name_System_Int64 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "long";
        }
        
        /// <summary>
        /// returns "ulong" for using as type information for the given System type in hyperlisp
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlisp.get-type-name.System.UInt64")]
        private static void pf_hyperlisp_get_type_name_System_UInt64 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "ulong";
        }

        /// <summary>
        /// returns "int" for using as type information for the given System type in hyperlisp
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlisp.get-type-name.System.Int32")]
        private static void pf_hyperlisp_get_type_name_System_Int32 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "int";
        }

        /// <summary>
        /// returns "uint" for using as type information for the given System type in hyperlisp
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlisp.get-type-name.System.UInt32")]
        private static void pf_hyperlisp_get_type_name_System_UInt32 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "uint";
        }

        /// <summary>
        /// returns "short" for using as type information for the given System type in hyperlisp
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlisp.get-type-name.System.Int16")]
        private static void pf_hyperlisp_get_type_name_System_Int16 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "short";
        }

        /// <summary>
        /// returns "ushort" for using as type information for the given System type in hyperlisp
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlisp.get-type-name.System.UInt16")]
        private static void pf_hyperlisp_get_type_name_System_UInt16 (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "ushort";
        }

        /// <summary>
        /// returns "single" for using as type information for the given System type in hyperlisp
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlisp.get-type-name.System.Single")]
        private static void pf_hyperlisp_get_type_name_System_Single (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "float";
        }

        /// <summary>
        /// returns "double" for using as type information for the given System type in hyperlisp
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlisp.get-type-name.System.Double")]
        private static void pf_hyperlisp_get_type_name_System_Double (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "double";
        }

        /// <summary>
        /// returns "decimal" for using as type information for the given System type in hyperlisp
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlisp.get-type-name.System.Decimal")]
        private static void pf_hyperlisp_get_type_name_System_Decimal (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "decimal";
        }
        
        /// <summary>
        /// returns "bool" for using as type information for the given System type in hyperlisp
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlisp.get-type-name.System.Boolean")]
        private static void pf_hyperlisp_get_type_name_System_Boolean (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "bool";
        }
        
        /// <summary>
        /// returns "byte" for using as type information for the given System type in hyperlisp
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlisp.get-type-name.System.Byte")]
        private static void pf_hyperlisp_get_type_name_System_Byte (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "byte";
        }
        
        /// <summary>
        /// returns "blob" for using as type information for the given System type in hyperlisp
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlisp.get-type-name.System.Byte[]")]
        private static void pf_hyperlisp_get_type_name_System_ByteBlob (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "blob";
        }

        /// <summary>
        /// returns "sbyte" for using as type information for the given System type in hyperlisp
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlisp.get-type-name.System.SByte")]
        private static void pf_hyperlisp_get_type_name_System_SByte (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "sbyte";
        }

        /// <summary>
        /// returns "char" for using as type information for the given System type in hyperlisp
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlisp.get-type-name.System.Char")]
        private static void pf_hyperlisp_get_type_name_System_Char (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "char";
        }
        
        /// <summary>
        /// returns "date" for using as type information for the given System type in hyperlisp
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlisp.get-type-name.System.DateTime")]
        private static void pf_hyperlisp_get_type_name_System_DateTime (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "date";
        }
        
        /// <summary>
        /// returns "time" for using as type information for the given System type in hyperlisp
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.hyperlisp.get-type-name.System.TimeSpan")]
        private static void pf_hyperlisp_get_type_name_System_TimeSpan (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "time";
        }
    }
}
