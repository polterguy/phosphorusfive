/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using p5.core;
using p5.exp;
using p5.exp.exceptions;

namespace p5.lambda.keywords
{
    /// <summary>
    ///     Class wrapping the [split] keyword in p5 lambda
    /// </summary>
    public static class Split
    {
        /// <summary>
        ///     The [split] keyword, allows you to split a single string into multiple values
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "split", Protection = EventProtection.LambdaClosed)]
        public static void lambda_split (ApplicationContext context, ActiveEventArgs e)
        {
            // Making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args, true)) {

                // Figuring out source value of [split]
                string source = XUtil.Single<string> (context, e.Args, true);
                if (source == null)
                    return; // Nothing to split

                // Retrieving separators (which strings to split upon)
                Node sepNode = e.Args ["="];
                string separator = sepNode == null ? null : XUtil.Single<string> (context, sepNode);
                Node valueSepNode = e.Args ["=="];
                string valueSep = valueSepNode == null ? null : XUtil.Single<string> (context, valueSepNode);

                // Retrieving whether or not we should trim each split result
                Node trimNode = e.Args ["trim"];
                bool trim = trimNode == null ? false : trimNode.GetExValue (context, false);
                if (string.IsNullOrEmpty (separator) && !trim && valueSep == null) {

                    // Special case, splitting into each character in string
                    foreach (var idxCh in source) {
                        e.Args.Add (idxCh.ToString ());
                    }
                } else {

                    // Splitting source
                    string[] entities = source.Split (
                        new string[] { separator }, 
                        System.StringSplitOptions.RemoveEmptyEntries);

                    // Looping through each split result
                    foreach (var idxSplitResult in entities) {

                        if (valueSep == null) {

                            // No name/value separator given or found
                            e.Args.Add (trim ? idxSplitResult.Trim () : idxSplitResult);
                        } else {

                            // Caller requests to split further into name/value
                            string[] valueNameEntities = idxSplitResult.Split (
                                new string[] { valueSep }, 
                                System.StringSplitOptions.RemoveEmptyEntries);

                            // Basic syntax checking, or "data checking"
                            if (valueNameEntities.Length > 2)
                                throw new LambdaException ("Value/Name separator found more than 2 instances in; " + idxSplitResult, e.Args, context);

                            // Checking if we found both name and value
                            if (valueNameEntities.Length == 2) {

                                // Both name and value where found
                                e.Args.Add (trim ? valueNameEntities [0].Trim () : valueNameEntities [0], trim ? valueNameEntities [1].Trim () : valueNameEntities [1]);
                            } else {

                                // Only value was found
                                e.Args.Add (trim ? idxSplitResult.Trim () : idxSplitResult); // couldn't split string into name/value, no value separator found
                            }
                        }
                    }
                }
            }
        }
    }
}
