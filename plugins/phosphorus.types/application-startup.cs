
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
    /// helper class for installing converters into core
    /// </summary>
    public static class applicationStartup
    {
        /*
         * called when Utilities.Convert needs to convert from one type to another
         */
        private static object ConvertCallback (object value, Type convertTo)
        {
            if (convertTo == typeof(string)) {
                if (value.GetType () == typeof(Node)) {
                    return Object2String.ToString ((Node)value);
                } else if (value.GetType () == typeof(Byte[])) {
                    return Object2String.ToString ((Byte[])value);
                } else if (value.GetType () == typeof(DateTime)) {
                    return Object2String.ToString ((DateTime)value);
                } else if (value.GetType () == typeof(TimeSpan)) {
                    return Object2String.ToString ((TimeSpan)value);
                } else if (value.GetType () == typeof(Boolean)) {
                    return Object2String.ToString ((Boolean)value);
                }
            }
            return null;
        }

        /// <summary>
        /// invoked during startup of application. install conversion object into phosphorus.core, for allowing
        /// Utilities.Convert to correctly understand and convert between types and their string representations
        /// </summary>
        /// <param name="context">application context</param>
        /// <param name="e">parameters</param>
        [ActiveEvent (Name = "pf.core.application-start")]
        private static void pf_core_application_start (ApplicationContext context, ActiveEventArgs e)
        {
            Utilities.Converters.Add (ConvertCallback);
        }
    }
}
