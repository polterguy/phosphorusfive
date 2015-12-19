/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Runtime.InteropServices;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.types.types
{
    /// <summary>
    ///     Class helps converts from expression to object, and vice versa
    /// </summary>
    public static class ExpressionConversion
    {
        /// <summary>
        ///     Creates a <see cref="p5.exp.Expression">Expression</see> from its string representation
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-object-value.x", Protection = EventProtection.NativeClosed)]
        private static void p5_hyperlisp_get_object_value_x (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = Expression.Create (e.Args.Get<string> (context), context);
        }

        /// <summary>
        ///     Creates a string from an Expression
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-string-value.p5.exp.Expression", Protection = EventProtection.NativeClosed)]
        private static void p5_hyperlisp_get_string_value_p5_exp_Expression (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = e.Args.Get<Expression> (context).Value;
        }

        /// <summary>
        ///     Returns the Hyperlisp type-name for the Expression type
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "p5.hyperlisp.get-type-name.p5.exp.Expression", Protection = EventProtection.NativeClosed)]
        private static void p5_hyperlisp_get_type_name_p5_exp_Expression (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "x";
        }
    }
}
