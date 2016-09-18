/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using p5.core;
using p5.exp;

namespace p5.lambda.keywords.extras
{
    /// <summary>
    ///     Class wrapping the [fetch] keyword in p5 lambda
    /// </summary>
    public static class Fetch
    {
        /// <summary>
        ///     The [fetch] keyword, allows you to forward retrieve value results of evaluation of its body
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "fetch", Protection = EventProtection.LambdaClosed)]
        public static void lambda_fetch (ApplicationContext context, ActiveEventArgs e)
        {
            // Evaluating [fetch] lambda block
            context.RaiseLambda ("eval-mutable", e.Args);

            // Now we can fetch expression value, and clear body
            e.Args.Value = XUtil.Single<object> (context, e.Args, true);
            e.Args.Clear ();
        }
    }
}
