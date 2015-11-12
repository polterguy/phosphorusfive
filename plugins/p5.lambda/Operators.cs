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
    /// </summary>
    public static class Operators
    {
        /// <summary>
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "equals")]
        [ActiveEvent (Name = "=")]
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
        [ActiveEvent (Name = "!=")]
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
        [ActiveEvent (Name = ">")]
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
        [ActiveEvent (Name = "<")]
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
        [ActiveEvent (Name = ">=")]
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
        [ActiveEvent (Name = "<=")]
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
            // checking for "null items" (which logically are equal)
            // This means that an expression yielding exclusively "null values" will be equal to anything else that somehow
            // yields only "null values"
            if (lhs.Count (ix => ix != null) == 0 && rhs.Count (ix => ix != null) == 0)
                return 0;
            if (rhs.Count (ix => ix != null) == 0 && lhs.Count (ix => ix != null) == 0)
                return 0;

            // checking count of both sides
            if (lhs.Count < rhs.Count)
                return -1;
            if (lhs.Count > rhs.Count)
                return 1;

            // looping through each item on both sides
            for (int idx = 0; idx < lhs.Count; idx++) {

                // checking both sides for "null"
                if (lhs [idx] == null && rhs [idx] != null)
                    return -1;
                if (lhs [idx] != null && rhs [idx] == null)
                    return 1;
                if (lhs [idx] == null && rhs [idx] == null)
                    continue;

                // checking to see if types match
                if (lhs [idx].GetType () != rhs [idx].GetType ())
                    return lhs [idx].GetType ().FullName.CompareTo (rhs [idx].GetType ().FullName);

                // running through IComparable
                int retVal = ((IComparable)lhs [idx]).CompareTo (rhs [idx]);

                // returning value if they were not the same, otherwise iterating to next item in lists
                if (retVal != 0)
                    return retVal;
            }

            return 0; // both sides are identical to each other
        }

        /*
         * returns two lists containing the object values of both sides being compared to each other
         */
        private static Tuple<List<object>, List<object>> GetBothSides (Node args, ApplicationContext context)
        {
            // left-hand side
            List<object> lhs = new List<object> (XUtil.Iterate<object> (args.Parent, context));

            // right-hand side
            List<object> rhs = new List<object> (XUtil.Iterate<object> (args, context));

            // returning both sides to caller
            return new Tuple<List<object>, List<object>> (lhs, rhs);
        }
    }
}
