/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Runtime.InteropServices;
using p5.core;
using p5.exp;

namespace p5.types
{
    /// <summary>
    ///     Class helps figuring out if some value(s) can be converted to another type
    /// </summary>
    public static class CanConvert
    {
        /// <summary>
        ///     Returns true if value(s) can be converted to type of [type]
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "can-convert", Protection = EventProtection.Lambda)]
        private static void can_convert (ApplicationContext context, ActiveEventArgs e)
        {
            // making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Syntax checking invocation
                if (!(e.Args.Value is Expression) || e.Args ["type"] == null)
                    throw new ArgumentException ("[can-convert] needs both an expression as its value, and a [type] parameter");

                // Figuring out type to check for
                string type = e.Args.GetExChildValue<string> ("type", context);

                // Exploting the fact that conversions will throw exception if conversion is not possible
                try
                {
                    foreach (var idx in XUtil.Iterate<object> (context, e.Args)) {
                        var objValue = context.RaiseNative ("p5.hyperlisp.get-object-value." + type, new Node (string.Empty, idx)).Value;
                    }

                    // No exception occurred, conversion is possible
                    e.Args.Value = true;
                }
                catch {

                    // Oops, conversion is not possible!
                    e.Args.Value = false;
                }
            }
        }
    }
}
