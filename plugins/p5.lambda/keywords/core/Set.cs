/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Linq;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.lambda.keywords.core
{
    /// <summary>
    ///     Class wrapping the [set] keyword in p5 lambda
    /// </summary>
    public static class Set
    {
        /// <summary>
        ///     The [set] keyword, allows you to change nodes through p5 lambda
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "set")]
        public static void lambda_set (ApplicationContext context, ActiveEventArgs e)
        {
            // Asserting destination is expression
            var destEx = e.Args.Value as Expression;
            if (destEx == null)
                throw new LambdaException (
                    string.Format ("Not a valid destination expression given to [set], value was '{0}', expected expression", e.Args.Value),
                    e.Args,
                    context);

            // Updating destination(s) with value of source
            foreach (var idxDestination in destEx.Evaluate (context, e.Args, e.Args)) {

                // Raising "src" Active Event. Notice, this must be done once, for each destination, in case the source event is expecting
                // the relative destination to evaluate
                var src = XUtil.Source (context, e.Args, idxDestination.Node);

                // Checking how many values source returned, and throwing if there's more than one
                if (src.Count > 1)
                    throw new LambdaException ("Multiple sources for [set]", e.Args, context);

                // Single source, or null source
                idxDestination.Value = src.Count == 1 ? src [0] : null;
            }
        }
    }
}
