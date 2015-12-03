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
    ///     Class wrapping the [split] keyword in p5.lambda.
    /// </summary>
    public static class Split
    {
        /// <summary>
        ///     The [split] keyword, allows you to split a single string into multiple values.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "split", Protection = EventProtection.Lambda)]
        private static void lambda_split (ApplicationContext context, ActiveEventArgs e)
        {
            // making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args, true)) {

                string whatToSplit = XUtil.Single<string> (context, e.Args);
                if (whatToSplit == null)
                    return; // nothing to split

                Node sepNode = e.Args ["="];
                string separator = sepNode == null ? null : XUtil.Single<string> (context, sepNode);

                Node valueSepNode = e.Args ["=="];
                string valueSep = valueSepNode == null ? null : XUtil.Single<string> (context, valueSepNode);

                Node trimNode = e.Args ["trim"];
                bool trim = trimNode == null ? false : trimNode.GetExValue (context, false);
                if (string.IsNullOrEmpty (separator) && !trim && valueSep == null) {

                    // special case, splitting into each character in string
                    foreach (var idxCh in whatToSplit) {
                        e.Args.Add (idxCh.ToString ());
                    }
                } else {

                    string[] entities = whatToSplit.Split (new string[] { separator }, System.StringSplitOptions.RemoveEmptyEntries);
                    foreach (var idx in entities) {

                        if (valueSep == null) {

                            // no name/value separator given or found
                            e.Args.Add (trim ? idx.Trim () : idx);
                        } else {

                            // caller requests to split further into name/value
                            string[] valueNameEntities = idx.Split (new string[] { valueSep }, System.StringSplitOptions.RemoveEmptyEntries);
                            if (valueNameEntities.Length > 2)
                                throw new LambdaException ("Value/Name separator found more than 2 instances in; " + idx, e.Args, context);

                            if (valueNameEntities.Length == 2) {

                                // both name and value where found
                                e.Args.Add (trim ? valueNameEntities [0].Trim () : valueNameEntities [0], trim ? valueNameEntities [1].Trim () : valueNameEntities [1]);
                            } else {

                                // only value was found
                                e.Args.Add (trim ? idx.Trim () : idx); // couldn't split string into name/value, no value separator found
                            }
                        }
                    }
                }
            }
        }
    }
}
