/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using p5.core;

namespace p5.types.types {
    /// <summary>
    ///     Class helps converts from char to string, and vice versa
    /// </summary>
    public static class CharType
    {
        /// <summary>
        ///     Creates a char from its string representation
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlambda.get-object-value.char")]
        private static void p5_hyperlisp_get_object_value_char (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value is char) {
                return;
            } else {
                e.Args.Value = char.Parse (e.Args.Get<string> (context));
            }
        }

        /// <summary>
        ///     Returns the Hyperlambda type-name for the char type
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlambda.get-type-name.System.Char")]
        private static void p5_hyperlisp_get_type_name_System_Char (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "char";
        }
    }
}
