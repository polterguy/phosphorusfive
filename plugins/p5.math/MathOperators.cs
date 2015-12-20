/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Linq;
using System.Numerics;
using p5.core;
using p5.exp;

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
        private delegate dynamic CalculateFunctor (dynamic sum, dynamic input);

        /// <summary>
        ///     Adds zero or more objects to another object
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "+", Protection = EventProtection.LambdaClosed)]
        private static void plus (ApplicationContext context, ActiveEventArgs e)
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
        [ActiveEvent (Name = "-", Protection = EventProtection.LambdaClosed)]
        private static void minus (ApplicationContext context, ActiveEventArgs e)
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
        [ActiveEvent (Name = "*", Protection = EventProtection.LambdaClosed)]
        private static void multiply (ApplicationContext context, ActiveEventArgs e)
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
        [ActiveEvent (Name = "/", Protection = EventProtection.LambdaClosed)]
        private static void divide (ApplicationContext context, ActiveEventArgs e)
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
        [ActiveEvent (Name = "^", Protection = EventProtection.LambdaClosed)]
        private static void exponent (ApplicationContext context, ActiveEventArgs e)
        {
            Calculate (
                context,
                e.Args,
                delegate (dynamic sum, dynamic input) {
                    if (sum is BigInteger) {
                        return BigInteger.Pow (sum, ChangeType (input, typeof (int)));
                    } else {
                        return Math.Pow (ChangeType (sum, typeof(double)), ChangeType (input, typeof(double)));
                    }
                });
        }

        /// <summary>
        ///     Returns the modulo of zero or more objects
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "%", Protection = EventProtection.LambdaClosed)]
        private static void modulo (ApplicationContext context, ActiveEventArgs e)
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
        private static void Calculate (
            ApplicationContext context,
            Node args,
            CalculateFunctor functor)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (args)) {

                // Getting lhs, or existing result
                dynamic result = args.GetExValue<object> (context, "");

                // Looping through each child node, and evaluating value of node with existing result
                // Avoiding explicit type declarations and formatting parameters
                foreach (var idxChild in args.Children.Where (ix => ix.Name != "" && ix.Name != "_pf_type")) {

                    // Finding value of object, which might be an Active Event invocation, a constant, or an expression
                    dynamic nextValue;
                    if (idxChild.Name.StartsWith ("_")) {

                        // Simple value, or expression yielding value to use
                        nextValue = idxChild.GetExValue<object> (context, (object)0);
                    } else {

                        // Active Event invocation to retrieve value to use
                        nextValue = context.RaiseLambda (idxChild.Name, idxChild).Get<object> (context, 0);
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
