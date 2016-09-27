/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using p5.exp;
using p5.core;

/// <summary>
///     Main namespace for types and type-conversions in Phosphorus Five
/// </summary>
namespace p5.types {
    /// <summary>
    ///     Class helps figuring out if some value(s) can be converted to another type
    /// </summary>
    public static class CanConvert
    {
        /// <summary>
        ///     Returns true if value(s) can be converted to type of [type]
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "can-convert")]
        public static void can_convert (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Syntax checking invocation
                if (!(e.Args.Value is Expression) || e.Args ["type"] == null)
                    throw new ArgumentException ("[can-convert] needs both an expression as its value, and a [type] parameter");

                // Figuring out type to check for
                string type = e.Args.GetExChildValue<string> ("type", context);

                // Exploiting the fact that conversions will throw exception if conversion is not possible
                try {

                    // Looping through all arguments supplied
                    foreach (var idx in XUtil.Iterate<object> (context, e.Args, true)) {
                        var objValue = context.Raise ("p5.hyperlisp.get-object-value." + type, new Node ("", idx)).Value;
                    }

                    // No exception occurred, conversion is possible
                    e.Args.Value = true;
                } catch {

                    // Oops, conversion is not possible!
                    e.Args.Value = false;
                }
            }
        }
    }
}
