/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using p5.core;

namespace p5.lambda.keywords.core
{
    /// <summary>
    ///     Class wrapping the [break] keyword in p5 lambda.
    /// </summary>
    public static class Break
    {
        /// <summary>
        ///     The [break] keyword, allows you to break out of a loop, such as [while] or [for-each]
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "break", Protection = EventProtection.LambdaClosed)]
        public static void lambda_break (ApplicationContext context, ActiveEventArgs e)
        {
            // Inserting "return signaling node", such that [eval] and similar constructs will break out
            // of their current execution
            e.Args.Root.Insert (0, new Node ("_break"));
        }
    }
}
