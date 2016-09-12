/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using p5.exp;
using p5.core;

namespace p5.lambda.keywords {
    /// <summary>
    ///     Class wrapping the [to-upper] keyword in p5 lambda.
    /// </summary>
    public static class ToUpper
    {
        /// <summary>
        ///     The [to-upper] keyword, allows you to transform all lowercase characters in a string to UPPERCASE
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "to-upper", Protection = EventProtection.LambdaClosed)]
        public static void lambda_to_upper (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Returning to lowers of expression or constant
                e.Args.Value = XUtil.Single<string> (context, e.Args, true, "").ToUpperInvariant ();
            }
        }
    }
}
