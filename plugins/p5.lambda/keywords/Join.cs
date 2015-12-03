/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Text;
using p5.core;
using p5.exp;
using p5.exp.exceptions;

namespace p5.lambda.keywords
{
    /// <summary>
    ///     Class wrapping the [join] keyword in p5.lambda.
    /// </summary>
    public static class Join
    {
        /// <summary>
        ///     The [join] keyword, allows you to join multiple nodes/names/values into a single string
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "join", Protection = EventProtection.LambdaClosed)]
        private static void lambda_join (ApplicationContext context, ActiveEventArgs e)
        {
            // making sure we clean up and remove all arguments passed in after execution
            using (new Utilities.ArgsRemover (e.Args)) {

                // verifying syntax of statement
                var ex = e.Args.Value as Expression;
                if (ex == null)
                    throw new LambdaException ("No expression supplied to [join], join needs an expression to evaluate", e.Args, context);

                Node sepNode = e.Args ["="];
                Node valueSepNode = e.Args ["=="];
                Node trimNode = e.Args ["trim"];

                string insertBetweenNodes = sepNode == null ? "" : XUtil.Single<string> (context, sepNode);
                string insertBetweenNameValue = valueSepNode == null ? "" : XUtil.Single<string> (context, valueSepNode);
                bool trim = trimNode == null ? false : trimNode.GetExValue (context, false);

                StringBuilder result = new StringBuilder ();
                foreach (var idx in ex.Evaluate (e.Args, context)) {

                    if (result.Length != 0)
                        result.Append (insertBetweenNodes);

                    if (idx.TypeOfMatch == Match.MatchType.node) {

                        // adding both name and value
                        result.Append (trim ? idx.Node.Name.Trim () : idx.Node.Name);
                        if (idx.Node.Value != null)
                            result.Append (insertBetweenNameValue)
                            .Append (trim ? idx.Node.Get<string> (context, "").Trim () : idx.Node.Value);
                    } else {

                        // adding only result of MatchEntity
                        result.Append (idx.Value);
                    }
                }

                // returning result
                e.Args.Value = result.ToString ();
            }
        }
    }
}
