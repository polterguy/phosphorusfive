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

using System.Linq;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.lambda.keywords.core
{
    /// <summary>
    ///     Class wrapping the [switch] Active Event.
    /// </summary>
    public static class Switch
    {
        /// <summary>
        ///     The [switch] event, allows you to create matches for values within [case] lambda children.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "switch")]
        public static void lambda_switch (ApplicationContext context, ActiveEventArgs e)
        {
            // Sanity check syntax of invocation.
            ForwardEvaluateValuesAndSanityCheck (context, e.Args);

            // Retrieving value of switch,and sanity checking it.
            var value = e.Args.GetExValue<object> (context, null);
            if (value is Node)
                throw new LambdaException ("[switch] cannot act upon nodes, only other types of values", e.Args, context);

            // Our lambda evaluation object.
            Node lambdaObj = null;

            // Retrieving correct lambda, if there are any.
            // Notice, due to that [default] are now syntax checked, and we have confirmed it has no value, then default will be returned if no [case] with
            // a null value exists, if [default] exists.
            // Notice also that all [case] values should be forward evaluated at this point, hence we can simply compare against its value, and there is no need
            // to evaluate expressions or snything like that in them.
            if (value == null)
                lambdaObj = e.Args.Children.FirstOrDefault (ix => ix.Value == null);
            else
                lambdaObj = e.Args.Children.FirstOrDefault (ix => ix.Name == "case" && value.Equals (ix.Value)) ?? e.Args["default"];

            // Checking if we have a match, and if so, evaluate lambda belonging to match.
            if (lambdaObj != null) {

                // Finding first non-empty [case] lambda, in case of fallthrough.
                // Notice, formatting parameters are removed at this point.
                while (lambdaObj != null && lambdaObj.Count == 0) {
                    lambdaObj = lambdaObj.NextSibling;
                }

                // Evaluating [case] lambda.
                if (lambdaObj != null)
                    context.RaiseEvent (lambdaObj.Name, lambdaObj);
            }
        }

        /*
         * Notice, these next two events might look like they're not necessary.
         * However, we want to as much as possible, make sure one "keyword" equals one Active Event, for among other things due to [voocabulary] 
         * and [eval-whitelist] concepts, hence we simply wrap [eval-mutable], and create an "empty" Active Event for each "keyword" below.
         */

        /// <summary>
        ///     Supporting Active Event for [switch].
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "case")]
        public static void lambda_case (ApplicationContext context, ActiveEventArgs e)
        {
            context.RaiseEvent ("eval-mutable", e.Args);
        }

        /// <summary>
        ///     Supporting Active Event for [switch].
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "default")]
        public static void lambda_default (ApplicationContext context, ActiveEventArgs e)
        {
            context.RaiseEvent ("eval-mutable", e.Args);
        }

        /*
         * Forward evaluates values of [case] lambdas, and sanity checks syntax of [switch].
         */
        static void ForwardEvaluateValuesAndSanityCheck (ApplicationContext context, Node args)
        {
            // Sanity check, only [case], [default], and formatting parameters are legal as children nodes.
            if (args.Children.Count (ix => ix.Name != "" && ix.Name != "case" && ix.Name != "default") > 0)
                throw new LambdaException ("Only [case] and [default] lambdas are legal beneath [switch]", args, context);

            // Sanity check, only one [default] lambda is allowed.
            if (args.Children.Count (ix => ix.Name == "default") > 1)
                throw new LambdaException ("[switch] can only have one [default] lambda", args, context);

            // Sanity check, at least one [case] or [default] exists.
            if (args.Children.Count (ix => ix.Name == "default" || ix.Name == "case") == 0)
                throw new LambdaException ("[switch] must have at least one [default] or [case] lambda", args, context);

            // Sanity check, [default], if given, has null value.
            if ((args["default"]?.Value ?? null) != null)
                throw new LambdaException ("[default] cannot have a value", args, context);

            // Sanity checking that [default], if provided, is our last lambda.
            if (args["default"] != null && args.IndexOf (args["default"]) != args.Count - 1)
                throw new LambdaException ("[default] must be the last lambda in a [switch]", args, context);

            // Unrolling case values, and sanity checking them, that each [case] holds a unique value, after having evaluated their values.
            foreach (var idx in args.Children.Where (ix => ix.Name == "case")) {

                // Forward evaluating value of currently iterated [case], and making sure we remove any formatting parameters.
                idx.Value = idx.GetExValue<object> (context, null);
                idx.RemoveAll (ix => ix.Name == "");

                // Making sure there's only one [case] in the children collection with the given value.
                if (args.Children.Count (ix => (ix.Value == null && idx.Value == null) || (ix.Value != null && ix.Value.Equals (idx.Value) )) > 1)
                    throw new LambdaException ("All your [case] lambdas must have unique values within your [switch]", idx, context);
            }

            // More sanity check, to make sure the last [case] or [default] lambda is not empty, which would signify fallthrough into "nothing".
            if (args.LastChild.Count == 0)
                throw new LambdaException ("Your last lambda in a [switch] cannot be a fallthrough lambda block", args.LastChild, context);
        }
    }
}
