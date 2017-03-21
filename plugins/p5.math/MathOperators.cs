/*
 * Phosphorus Five, copyright 2014 - 2017, Thomas Hansen, thomas@gaiasoul.com
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

using System;
using System.Linq;
using p5.exp;
using p5.core;

namespace p5.math
{
    /// <summary>
    ///     Class wrapping all core math operators
    /// </summary>
    public static class MathOperators
    {
        /*
         * Functor for calculating single instance
         */
        delegate dynamic CalculateFunctor (dynamic sum, dynamic input);

        /// <summary>
        ///     Adds zero or more objects to another object
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "+")]
        public static void plus (ApplicationContext context, ActiveEventArgs e)
        {
            Calculate (
                context,
                e.Args,
                delegate (dynamic sum, dynamic input) {
                    return sum + ChangeType (input, sum.GetType ());
                });
        }

        /// <summary>
        ///     Subtracts zero or more objects to another object
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "-")]
        public static void minus (ApplicationContext context, ActiveEventArgs e)
        {
            Calculate (
                context,
                e.Args,
                delegate (dynamic sum, dynamic input) {
                    return sum - ChangeType (input, sum.GetType ());
                });
        }

        /// <summary>
        ///     Multiplies zero or more objects with another object
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "*")]
        public static void multiply (ApplicationContext context, ActiveEventArgs e)
        {
            Calculate (
                context,
                e.Args,
                delegate (dynamic sum, dynamic input) {
                    return sum * ChangeType (input, sum.GetType ());
                });
        }
        
        /// <summary>
        ///     Divides zero or more objects with another object
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "/")]
        public static void divide (ApplicationContext context, ActiveEventArgs e)
        {
            Calculate (
                context,
                e.Args,
                delegate (dynamic sum, dynamic input) {
                    return sum / ChangeType (input, sum.GetType ());
                });
        }
        
        /// <summary>
        ///     Returns the exponent of zero or more objects
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "^")]
        public static void exponent (ApplicationContext context, ActiveEventArgs e)
        {
            Calculate (
                context,
                e.Args,
                delegate (dynamic sum, dynamic input) {
                    return Math.Pow (ChangeType (sum, typeof(double)), ChangeType (input, typeof(double)));
                });
        }

        /// <summary>
        ///     Returns the modulo of zero or more objects
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "%")]
        public static void modulo (ApplicationContext context, ActiveEventArgs e)
        {
            Calculate (
                context,
                e.Args,
                delegate (dynamic sum, dynamic input) {
                    return sum % ChangeType (input, sum.GetType ());
                });
        }

        /*
         * Helper method for calculations, pass in specialized delegate for math function you wish to apply
         */
        static void Calculate (
            ApplicationContext context,
            Node args,
            CalculateFunctor functor)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new ArgsRemover (args)) {

                // Getting lhs, or existing result
                dynamic result = args.GetExValue<object> (context, "");

                // Looping through each child node, and evaluating value of node with existing result
                // Avoiding explicit type declarations and formatting parameters
                foreach (var idxChild in args.Children.Where (ix => ix.Name != "" && ix.Name != "_pf_type")) {

                    // Finding value of object, which might be an Active Event invocation, a constant, or an expression
                    dynamic nextValue;
                    if (idxChild.Name == "_") {

                        // Simple value, or expression yielding value to use
                        nextValue = idxChild.GetExValue<object> (context, 0);
                    } else if (idxChild.Name.StartsWithEx ("_")) {

                        // Possible [_dn] or other ignored node
                        continue;
                    } else {

                        // Active Event invocation to retrieve value to use
                        nextValue = context.RaiseEvent (idxChild.Name, idxChild).Get<object> (context, 0);
                    }

                    // Now evaluating result of above node's value with existing result, evaluating the "functor" passed
                    // in as evaluation functor
                    result = functor (result, nextValue);
                }

                // Returning result as value of node
                args.Value = result;
            }
        }

        /*
         * Changes type of value to given resultType
         */
        internal static object ChangeType (object value, Type resultType)
        {
            if (value.GetType () == resultType)
                return value;
            return Convert.ChangeType (value, resultType);
        }
    }
}
