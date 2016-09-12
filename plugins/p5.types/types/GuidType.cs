/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using p5.core;

namespace p5.types.types {
    /// <summary>
    ///     Class helps converts from Guid to string, and vice versa
    /// </summary>
    public static class GuidType
    {
        /// <summary>
        ///     Creates a Guid from its string representation
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-object-value.guid", Protection = EventProtection.NativeClosed)]
        private static void p5_hyperlisp_get_object_value_guid (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value is Guid) {
                return;
            } else {
                e.Args.Value = Guid.Parse (e.Args.Get<string> (context));
            }
        }

        /// <summary>
        ///     Returns the Hyperlisp type-name for the Guid type
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-type-name.System.Guid", Protection = EventProtection.NativeClosed)]
        private static void p5_hyperlisp_get_type_name_System_Guid (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "guid";
        }

        /// <summary>
        ///     Returns a new randomly created Guid
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "new-guid", Protection = EventProtection.LambdaClosed)]
        private static void new_guid (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = Guid.NewGuid ();
        }
    }
}
