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
    /// 
    ///     The [split] keyword allows you to split any single string value into multiple nodes.
    /// </summary>
    public static class Split
    {
        /// <summary>
        ///     The [split] keyword, allows you to split a single string into multiple values.
        /// 
        ///     Use [=] to declare what string you wish to split upon. The separated values will be returned as values of
        ///     children nodes. Optionally declare [==] to separate the separated values into name and value of result nodes.
        /// 
        ///     Add [trim] with a value of 'true' if you wish to trim your entities before adding them as result.
        /// 
        ///     Both [=] and [==] if given, will be removed from the children collection after evaluation.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "split")]
        private static void lambda_split (ApplicationContext context, ActiveEventArgs e)
        {
            Node sepNode = e.Args ["="];
            if (sepNode == null)
                throw new LambdaException ("No [=] given to [split]", e.Args, context);

            Node valueSepNode = e.Args ["=="];
            Node trimNode = e.Args ["trim"];

            string whatToSplit = XUtil.Single<string> (e.Args, context);
            string separator = XUtil.Single<string> (sepNode, context);
            string valueSep = valueSepNode == null ? null : XUtil.Single<string> (valueSepNode, context);
            bool trim = trimNode == null ? false : trimNode.GetExValue (context, false);

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

            // cleaning up
            sepNode.UnTie ();
            if (trimNode != null)
                trimNode.UnTie ();
            if (valueSepNode != null)
                valueSepNode.UnTie ();
        }
    }
}
