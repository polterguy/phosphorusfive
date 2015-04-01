/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using phosphorus.core;
using phosphorus.expressions;

/// <summary>
///     Main snamespace for all math Active Events.
/// 
///     Contains math Active Events and operators.
/// </summary>
namespace phosphorus.math
{
    /// <summary>
    ///     Class wrapping all core math operators.
    /// 
    ///     Operators and math Active Events contained in this class are '+', '-', '/' and '*'.
    /// </summary>
    public static class MathOperators
    {
        /// <summary>
        ///     Adds zero or more objects to another object.
        /// 
        ///     Will traverse all children of the given node, change the type of its underlaying values to
        ///     the type of the object in the value of main node, and add these two objects together.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "+")]
        private static void math_plus (ApplicationContext context, ActiveEventArgs e)
        {
            dynamic result = e.Args.GetExValue<object> (context);
            Type resultType = result.GetType ();
            foreach (var idxChild in e.Args.Children) {
                if (idxChild.Name == string.Empty)
                    continue; // formatting parameter ...
                dynamic nextValue = context.Raise (idxChild.Name, idxChild).GetExValue<object> (context, (object)0);
                result += Convert.ChangeType (nextValue, resultType);
            }
            e.Args.Value = result;
        }

        /// <summary>
        ///     Subtracts zero or more objects to another object.
        /// 
        ///     Will traverse all children of the given node, change the type of its underlaying values to
        ///     the type of the object in the value of main node, and subtract these two objects from each other.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "-")]
        private static void math_minus (ApplicationContext context, ActiveEventArgs e)
        {
            dynamic result = e.Args.GetExValue<object> (context);
            Type resultType = result.GetType ();
            foreach (var idxChild in e.Args.Children) {
                if (idxChild.Name == string.Empty)
                    continue; // formatting parameter ...
                dynamic nextValue = context.Raise (idxChild.Name, idxChild).GetExValue<object> (context, (object)0);
                result -= Convert.ChangeType (nextValue, resultType);
            }
            e.Args.Value = result;
        }

        /// <summary>
        ///     Multiplies zero or more objects with another object.
        /// 
        ///     Will traverse all children of the given node, change the type of its underlaying values to
        ///     the type of the object in the value of main node, and multiply these objects together.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "*")]
        private static void math_multiply (ApplicationContext context, ActiveEventArgs e)
        {
            dynamic result = e.Args.GetExValue<object> (context);
            Type resultType = result.GetType ();
            foreach (var idxChild in e.Args.Children) {
                if (idxChild.Name == string.Empty)
                    continue; // formatting parameter ...
                dynamic nextValue = context.Raise (idxChild.Name, idxChild).GetExValue<object> (context, (object)0);
                result *= Convert.ChangeType (nextValue, resultType);
            }
            e.Args.Value = result;
        }
        
        /// <summary>
        ///     Divides zero or more objects with another object.
        /// 
        ///     Will traverse all children of the given node, change the type of its underlaying values to
        ///     the type of the object in the value of main node, and divide this object with the value from the main node.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "/")]
        private static void math_divide (ApplicationContext context, ActiveEventArgs e)
        {
            dynamic result = e.Args.GetExValue<object> (context);
            Type resultType = result.GetType ();
            foreach (var idxChild in e.Args.Children) {
                if (idxChild.Name == string.Empty)
                    continue; // formatting parameter ...
                dynamic nextValue = context.Raise (idxChild.Name, idxChild).GetExValue<object> (context, (object)0);
                result /= Convert.ChangeType (nextValue, resultType);
            }
            e.Args.Value = result;
        }
        
        /// <summary>
        ///     Returns the modulo of zero or more objects.
        /// 
        ///     Will traverse all children of the given node, change the type of its underlaying values to
        ///     the type of the object in the value of main node, and return the modulo of the two objects.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "%")]
        private static void math_modulo (ApplicationContext context, ActiveEventArgs e)
        {
            dynamic result = e.Args.GetExValue<object> (context);
            Type resultType = result.GetType ();
            foreach (var idxChild in e.Args.Children) {
                if (idxChild.Name == string.Empty)
                    continue; // formatting parameter ...
                dynamic nextValue = context.Raise (idxChild.Name, idxChild).GetExValue<object> (context, (object)0);
                result %= Convert.ChangeType (nextValue, resultType);
            }
            e.Args.Value = result;
        }
    }
}
