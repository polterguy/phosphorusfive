/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Linq;
using System.Collections.Generic;
using p5.core;
using p5.exp;
using p5.exp.exceptions;

namespace p5.lambda
{
    /// <summary>
    ///     Class wrapping all operators in p5.lambda.
    /// 
    ///     Operators are used in conditional statements, such as [if], [else-if] and [while] to evaluate conditions
    /// </summary>
    public static class Operators
    {
        /// <summary>
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "equals")]
        private static void equals (ApplicationContext context, ActiveEventArgs e)
        {
            var sides = GetBothSides (e.Args, context);
            e.Args.Parent.Value = CompareValues (sides.Item1, sides.Item2) == 0;
        }

        /// <summary>
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "not-equals")]
        private static void not_equals (ApplicationContext context, ActiveEventArgs e)
        {
            var sides = GetBothSides (e.Args, context);
            e.Args.Parent.Value = CompareValues (sides.Item1, sides.Item2) != 0;
        }

        /// <summary>
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "more-than")]
        private static void more_than (ApplicationContext context, ActiveEventArgs e)
        {
            var sides = GetBothSides (e.Args, context);
            e.Args.Parent.Value = CompareValues (sides.Item1, sides.Item2) > 0;
        }

        /// <summary>
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "less-than")]
        private static void less_than (ApplicationContext context, ActiveEventArgs e)
        {
            var sides = GetBothSides (e.Args, context);
            e.Args.Parent.Value = CompareValues (sides.Item1, sides.Item2) < 0;
        }

        /// <summary>
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "more-than-equals")]
        private static void more_than_equals (ApplicationContext context, ActiveEventArgs e)
        {
            var sides = GetBothSides (e.Args, context);
            e.Args.Parent.Value = CompareValues (sides.Item1, sides.Item2) >= 0;
        }

        /// <summary>
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "less-than-equals")]
        private static void less_than_equals (ApplicationContext context, ActiveEventArgs e)
        {
            var sides = GetBothSides (e.Args, context);
            e.Args.Parent.Value = CompareValues (sides.Item1, sides.Item2) <= 0;
        }

        /*
         * compares a list of values for equality, and returns -1, 0 or +1 depending upon which list
         * is more than, less than or equals the other
         */
        private static int CompareValues (List<object> lhs, List<object> rhs)
        {
            if (lhs.Count < rhs.Count)
                return -1;
            if (lhs.Count > rhs.Count)
                return 1;
            for (int idx = 0; idx < lhs.Count; idx++) {
                if (lhs [idx] == null && rhs [idx] != null)
                    return -1;
                if (lhs [idx] != null && rhs [idx] == null)
                    return 1;
                if (lhs [idx] == null && rhs [idx] == null)
                    continue;
                if (lhs [idx].GetType () != rhs [idx].GetType ())
                    return lhs [idx].GetType ().FullName.CompareTo (rhs [idx].GetType ().FullName);
                int retVal = ((IComparable)lhs [idx]).CompareTo (rhs [idx]);
                if (retVal != 0)
                    return retVal;
            }
            return 0; // equals
        }
        
        private static Tuple<List<object>, List<object>> GetBothSides (Node args, ApplicationContext context)
        {
            List<object> lhs = new List<object> (XUtil.Iterate<object> (args.Parent, context));
            if (args.Parent.Value == null)
                lhs.Add (null);
            List<object> rhs = new List<object> (XUtil.Iterate<object> (args, context));
            if (args.Value == null)
                rhs.Add (null);
            return new Tuple<List<object>, List<object>> (lhs, rhs);
        }
    }
}
