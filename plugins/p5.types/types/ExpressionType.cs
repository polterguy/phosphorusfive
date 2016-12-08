/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, thomas@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using p5.exp;
using p5.core;

namespace p5.types.types
{
    /// <summary>
    ///     Class helps converts from expression to string, and vice versa
    /// </summary>
    public static class ExpressionType
    {
        /// <summary>
        ///     Creates a <see cref="p5.exp.Expression">Expression</see> from its string representation
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.hyperlambda.get-object-value.x")]
        private static void p5_hyperlisp_get_object_value_x (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value is Expression) {
                return;
            } else {
                e.Args.Value = Expression.Create (context, e.Args.Get<string> (context));
            }
        }

        /// <summary>
        ///     Creates a string from an <see cref="p5.exp.Expression">Expression</see>
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.hyperlambda.get-string-value.p5.exp.Expression")]
        private static void p5_hyperlisp_get_string_value_p5_exp_Expression (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = e.Args.Get<Expression> (context).Value;
        }

        /// <summary>
        ///     Returns the Hyperlambda type-name for the <see cref="p5.exp.Expression">Expression</see> type
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.hyperlambda.get-type-name.p5.exp.Expression")]
        private static void p5_hyperlisp_get_type_name_p5_exp_Expression (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "x";
        }
    }
}
