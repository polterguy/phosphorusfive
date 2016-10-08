/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using p5.exp;
using p5.core;

namespace p5.lambda.helpers
{
    /// <summary>
    ///     Class wrapping all comparison operators in p5 lambda.
    /// </summary>
    public static class ComparisonOperators
    {
        /// <summary>
        ///     Equal comparison operator, yields true if value equals value of parent
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "=")]
        public static void equals (ApplicationContext context, ActiveEventArgs e)
        {
            var sides = GetBothSides (e.Args, context);
            e.Args.Value = CompareValues (sides.Item1, sides.Item2) == 0;
        }

        /// <summary>
        ///     Non-equality comparison operator, yields true if value does not equals value of parent
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "!=")]
        public static void not_equals (ApplicationContext context, ActiveEventArgs e)
        {
            var sides = GetBothSides (e.Args, context);
            e.Args.Value = CompareValues (sides.Item1, sides.Item2) != 0;
        }

        /// <summary>
        ///     More than comparison operator, yields true if parent's value is more than value
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ">")]
        public static void more_than (ApplicationContext context, ActiveEventArgs e)
        {
            var sides = GetBothSides (e.Args, context);
            e.Args.Value = CompareValues (sides.Item1, sides.Item2) > 0;
        }

        /// <summary>
        ///     Less than comparison operator, yields true if parent's value is less than value
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "<")]
        public static void less_than (ApplicationContext context, ActiveEventArgs e)
        {
            var sides = GetBothSides (e.Args, context);
            e.Args.Value = CompareValues (sides.Item1, sides.Item2) < 0;
        }

        /// <summary>
        ///     More than equals comparison operator, yields true if parent's value is more than or equals value
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ">=")]
        public static void more_than_equals (ApplicationContext context, ActiveEventArgs e)
        {
            var sides = GetBothSides (e.Args, context);
            e.Args.Value = CompareValues (sides.Item1, sides.Item2) >= 0;
        }

        /// <summary>
        ///     Less than equals comparison operator, yields true if parent's value is less than or equals value
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "<=")]
        public static void less_than_equals (ApplicationContext context, ActiveEventArgs e)
        {
            var sides = GetBothSides (e.Args, context);
            e.Args.Value = CompareValues (sides.Item1, sides.Item2) <= 0;
        }

        /// <summary>
        ///     Contains comparison operator, yields true if parent's value contains the string/regex in contains value
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "~")]
        public static void contains (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving left hand side
            var lhs = e.Args.Parent.GetExValue (context, "");

            // Retrieving right hand side
            var rhs = e.Args.GetExValue<object> (context, null);

            // Checking if we were given a regex, or "anything else"
            if (rhs is Regex) {

                // Regular expression "like"
                e.Args.Value = (rhs as Regex).IsMatch (lhs);
            } else {

                // Simple string "like"
                e.Args.Value = lhs.Contains (Utilities.Convert<string> (context, rhs, ""));
            }
        }

        /// <summary>
        ///     Not-contains comparison operator, yields true if parent's value does not contain the string in not-contains value
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "!~")]
        public static void not_contains (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving left hand side
            var lhs = e.Args.Parent.GetExValue (context, "");

            // Retrieving right hand side
            var rhs = e.Args.GetExValue<object> (context, null);

            // Checking if we were given a regex, or "anything else"
            if (rhs is Regex) {

                // Regular expression "like"
                e.Args.Value = !(rhs as Regex).IsMatch (lhs);
            } else {

                // Simple string "like"
                e.Args.Value = !lhs.Contains (Utilities.Convert<string> (context, rhs, ""));
            }
        }

        /// <summary>
        ///     Returns all comparison operators in Phosphorus Five
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "operators")]
        private static void operators (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Add ("=");
            e.Args.Add ("!=");
            e.Args.Add (">");
            e.Args.Add ("<");
            e.Args.Add (">=");
            e.Args.Add ("<=");
            e.Args.Add ("~");
            e.Args.Add ("!~");
        }

        /*
         * Compares a list of values for equality, and returns -1, 0 or +1 depending upon which list
         * is more than, less than or equals to the other
         */
        private static int CompareValues (List<object> lhs, List<object> rhs)
        {
            // Checking for "null items" (which logically are equal)
            // This means that an expression yielding exclusively "null values", will be equal to anything else that somehow
            // yields only "null values"
            if (lhs.Count (ix => ix != null) == 0 && rhs.Count (ix => ix != null) == 0)
                return 0;
            if (rhs.Count (ix => ix != null) == 0 && lhs.Count (ix => ix != null) == 0)
                return 0;

            // Checking count of both sides
            if (lhs.Count < rhs.Count)
                return -1;
            if (lhs.Count > rhs.Count)
                return 1;

            // Looping through each item on both sides
            for (int idx = 0; idx < lhs.Count; idx++) {

                // Checking both sides for "null"
                if (lhs [idx] == null && rhs [idx] != null)
                    return -1;
                if (lhs [idx] != null && rhs [idx] == null)
                    return 1;
                if (lhs [idx] == null && rhs [idx] == null)
                    continue;

                // Checking to see if types are the same
                if (lhs [idx].GetType () != rhs [idx].GetType ())
                    return lhs [idx].GetType ().FullName.CompareTo (rhs [idx].GetType ().FullName);

                // Running through IComparable
                int retVal = ((IComparable)lhs [idx]).CompareTo (rhs [idx]);

                // Returning result of comparison, if they were not equal, otherwise iterating to next item in lists
                if (retVal != 0)
                    return retVal;
            }

            return 0; // Both sides are identical to each other
        }

        /*
         * Returns two lists containing the object values of both sides being compared to each other
         */
        private static Tuple<List<object>, List<object>> GetBothSides (Node args, ApplicationContext context)
        {
            // Left-hand side
            List<object> lhs = new List<object> (XUtil.Iterate<object> (context, args.Parent));

            // Right-hand side
            List<object> rhs = new List<object> (XUtil.Iterate<object> (context, args));

            // Returning both sides to caller
            return new Tuple<List<object>, List<object>> (lhs, rhs);
        }
    }
}
