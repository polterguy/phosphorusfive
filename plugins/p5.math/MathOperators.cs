/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Linq;
using p5.core;
using p5.exp;

/// <summary>
///     Main snamespace for all math Active Events
/// </summary>
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
        private static void math_plus (ApplicationContext context, ActiveEventArgs e)
        {
            Calculate (
                context,
                e.Args,
                delegate (dynamic sum, dynamic input) {
                    return sum + input;
                });
        }

        /// <summary>
        ///     Subtracts zero or more objects to another object
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "-", Protection = EventProtection.LambdaClosed)]
        private static void math_minus (ApplicationContext context, ActiveEventArgs e)
        {
            Calculate (
                context,
                e.Args,
                delegate (dynamic sum, dynamic input) {
                    return sum - input;
                });
        }

        /// <summary>
        ///     Multiplies zero or more objects with another object
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "*", Protection = EventProtection.LambdaClosed)]
        private static void math_multiply (ApplicationContext context, ActiveEventArgs e)
        {
            Calculate (
                context,
                e.Args,
                delegate (dynamic sum, dynamic input) {
                    return sum * input;
                });
        }
        
        /// <summary>
        ///     Divides zero or more objects with another object
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "/", Protection = EventProtection.LambdaClosed)]
        private static void math_divide (ApplicationContext context, ActiveEventArgs e)
        {
            Calculate (
                context,
                e.Args,
                delegate (dynamic sum, dynamic input) {
                    return sum / input;
                });
        }
        
        /// <summary>
        ///     Returns the modulo of zero or more objects
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "%", Protection = EventProtection.LambdaClosed)]
        private static void math_modulo (ApplicationContext context, ActiveEventArgs e)
        {
            Calculate (
                context,
                e.Args,
                delegate (dynamic sum, dynamic input) {
                    return sum % input;
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

                dynamic result = args.GetExValue<object> (context, "");
                Type resultType = null;
                if (args ["_pf_type"] == null) {

                    // Using left most type as guiding type for result
                    resultType = result.GetType ();
                } else {

                    // Type was explicitly passed in due to recursive invocations
                    resultType = args ["_pf_type"].Get<Type> (context);
                    result = Convert.ChangeType (result, resultType);
                }

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
                        idxChild.FindOrCreate ("_pf_type").Value = resultType;
                        nextValue = context.RaiseLambda (idxChild.Name, idxChild).Get<object> (context, 0);
                    }

                    // Now evaluating result of above node's value with existing result, evaluating the "functor" passed
                    // in as evaluation functor
                    result = functor (result, Convert.ChangeType (nextValue, resultType));
                }

                // Returning result as value of node
                args.Value = result;
            }
        }
    }
}
