/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using p5.exp;
using p5.core;

namespace p5.strings.keywords
{
    /// <summary>
    ///     Class wrapping the [length] keyword in p5 lambda.
    /// </summary>
    public static class Length
    {
        /// <summary>
        ///     The [length] keyword, retrieves the length of the specified string, performing conversion if necessary
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "length")]
        public static void lambda_length (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Returning length of constant or expression, converted to string if necessary
                e.Args.Value = XUtil.Single<string> (context, e.Args, true, "").Length;
            }
        }
    }
}
