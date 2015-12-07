/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Linq;
using p5.core;
using p5.exp;

/// <summary>
///     Main snamespace for all math Active Events.
/// 
///     Contains math Active Events and operators.
/// </summary>
namespace p5.math
{
    /// <summary>
    ///     Class wrapping all core math operators.
    /// 
    ///     Operators and math Active Events contained in this class are '+', '-', '/' and '*'.
    /// </summary>
    public static class MathOperators
    {
        private delegate dynamic CalculateFunctor (dynamic sum, dynamic input);

        private static void Calculate (
            CalculateFunctor functor, 
            Node args, 
            ApplicationContext context)
        {
            // making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (args)) {

                dynamic result = args.GetExValue<object> (context, "");
                Type resultType = null;
                if (args ["__pf_type"] == null) {

                    // Using left most type as guiding type for result
                    resultType = result.GetType ();
                } else {

                    // type was explicitly passed in due to recursive invocations
                    resultType = args ["__pf_type"].Get<Type> (context);
                    result = Convert.ChangeType (result, resultType);
                }

                foreach (var idxChild in args.Children.Where (ix => ix.Name != "" && ix.Name != "__pf_type")) {

                    dynamic nextValue;
                    if (idxChild.Name.StartsWith ("_")) {

                        // Simple value, or expression yielding value to use
                        nextValue = idxChild.GetExValue<object> (context, (object)0);
                    } else {

                        // Active Event invocation to retrieve value to use
                        idxChild.FindOrCreate ("__pf_type").Value = resultType;
                        nextValue = context.RaiseLambda (idxChild.Name, idxChild).Get<object> (context, 0);
                    }
                    result = functor (result, Convert.ChangeType (nextValue, resultType));
                }
                args.Value = result;
            }
        }

        /// <summary>
        ///     Adds zero or more objects to another object.
        /// 
        ///     Will traverse all children of the given node, change the type of its underlaying values to
        ///     the type of the object in the value of main node, and add these two objects together.
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "+", Protection = EventProtection.LambdaClosed)]
        private static void math_plus (ApplicationContext context, ActiveEventArgs e)
        {
            Calculate (delegate (dynamic sum, dynamic input) {
                return sum + input;
            }, e.Args, context);
        }

        /// <summary>
        ///     Subtracts zero or more objects to another object.
        /// 
        ///     Will traverse all children of the given node, change the type of its underlaying values to
        ///     the type of the object in the value of main node, and subtract these two objects from each other.
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "-", Protection = EventProtection.LambdaClosed)]
        private static void math_minus (ApplicationContext context, ActiveEventArgs e)
        {
            Calculate (delegate (dynamic sum, dynamic input) {
                return sum - input;
            }, e.Args, context);
        }

        /// <summary>
        ///     Multiplies zero or more objects with another object.
        /// 
        ///     Will traverse all children of the given node, change the type of its underlaying values to
        ///     the type of the object in the value of main node, and multiply these objects together.
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "*", Protection = EventProtection.LambdaClosed)]
        private static void math_multiply (ApplicationContext context, ActiveEventArgs e)
        {
            Calculate (delegate (dynamic sum, dynamic input) {
                return sum * input;
            }, e.Args, context);
        }
        
        /// <summary>
        ///     Divides zero or more objects with another object.
        /// 
        ///     Will traverse all children of the given node, change the type of its underlaying values to
        ///     the type of the object in the value of main node, and divide this object with the value from the main node.
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "/", Protection = EventProtection.LambdaClosed)]
        private static void math_divide (ApplicationContext context, ActiveEventArgs e)
        {
            Calculate (delegate (dynamic sum, dynamic input) {
                return sum / input;
            }, e.Args, context);
        }
        
        /// <summary>
        ///     Returns the modulo of zero or more objects.
        /// 
        ///     Will traverse all children of the given node, change the type of its underlaying values to
        ///     the type of the object in the value of main node, and return the modulo of the two objects.
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "%", Protection = EventProtection.LambdaClosed)]
        private static void math_modulo (ApplicationContext context, ActiveEventArgs e)
        {
            Calculate (delegate (dynamic sum, dynamic input) {
                return sum % input;
            }, e.Args, context);
        }
    }
}
