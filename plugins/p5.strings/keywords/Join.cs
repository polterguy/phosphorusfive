/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Text;
using p5.exp;
using p5.core;

namespace p5.strings.keywords
{
    /// <summary>
    ///     Class wrapping the [join] keyword in p5 lambda.
    /// </summary>
    public static class Join
    {
        /// <summary>
        ///     The [join] keyword, allows you to join multiple nodes/names/values into a single string
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "join")]
        public static void lambda_join (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // Retrieving separator character(s)
                var insertBetweenNodes = e.Args.GetExChildValue ("sep", context, "");

                // Retrieving wrapper character(s)
                var wrapValue = e.Args.GetExChildValue ("wrap", context, "");

                // Used as buffer
                StringBuilder result = new StringBuilder ();

                // Looping through each value
                foreach (var idx in XUtil.Iterate<string> (context, e.Args, true)) {

                    // Checking if this is first instance, and if not, we add separator value
                    if (result.Length != 0)
                        result.Append (insertBetweenNodes);

                    // Adding currently iterated result
                    result.Append (wrapValue + idx + wrapValue);
                }

                // Returning result
                e.Args.Value = result.ToString ();
            }
        }
    }
}
