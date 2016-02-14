/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Linq;
using p5.core;
using p5.exp;
using p5.exp.exceptions;

namespace p5.lambda.keywords
{
    /// <summary>
    ///     Class wrapping the [switch] keyword in p5 lambda
    /// </summary>
    public static class Switch
    {
        /// <summary>
        ///     The [switch] keyword, allows you to create matches for value with [case] children
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "switch", Protection = EventProtection.LambdaClosed)]
        public static void lambda_switch (ApplicationContext context, ActiveEventArgs e)
        {
            // Sanity check
            if (e.Args.Children.Count (ix => ix.Name != "" && ix.Name != "case" && ix.Name != "default") > 0)
                throw new LambdaException ("Only [case] and [default] children are legal beneath [switch]", e.Args, context);

            // Sanity check
            if (e.Args.Children.Count (ix => ix.Name == "default") > 1)
                throw new LambdaException ("[switch] can only have one [default] block", e.Args, context);

            // Retrieving value
            var value = e.Args.GetExValue<object> (context, null);

            // Finding out what to evaluate, defaulting to [default] block
            var eval = e.Args.Children.FirstOrDefault (ix => ix.Name == "default");

            // Special case for "null value"
            if (value == null) {

                // Finding first [case] with null value, defaulting to existing [default]
                eval = e.Args.Children.FirstOrDefault (ix => ix.Name == "case" && ix.GetExValue<object> (context) == null) ?? eval;
            } else {

                // Special case for node comparison
                if (value is Node) {

                    // Doing CompareTo
                    eval = e.Args.Children.FirstOrDefault (ix => 
                        ix.Name == "case" && (value as Node).CompareTo (ix.GetExValue<object> (context)) == 0) ?? eval;
                    
                } else {

                    // Finding first [case] that matches value, defaulting to existing [default]
                    eval = e.Args.Children.FirstOrDefault (ix => 
                        ix.Name == "case" && value.Equals (ix.GetExValue<object> (context))) ?? eval;
                }
            }

            // Evaluating eval, unless it is null, but before we do, we set all [case] and [default] to boolean false
            foreach (var idxChild in e.Args.Children) {
                idxChild.Value = false;
            }
            if (eval != null) {

                // Supporting "fallthrough"
                while (eval != null && eval.Children.Count == 0)
                    eval = eval.NextSibling;
                if (eval != null) {
                    eval.Value = true;
                    context.RaiseLambda ("eval-mutable", eval);
                }
            }
        }
    }
}
