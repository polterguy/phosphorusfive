/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Text;
using System.Linq;
using p5.core;
using p5.exp;
using p5.exp.exceptions;

namespace p5.lambda.keywords
{
    /// <summary>
    ///     Class wrapping the [return] keyword in p5 lambda.
    /// </summary>
    public static class Return
    {
        /// <summary>
        ///     The [return] keyword, allows you to return immediately from the evaluation of your code
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "return", Protection = EventProtection.LambdaClosed)]
        public static void lambda_return (ApplicationContext context, ActiveEventArgs e)
        {
            // Inserting "return signaling node", such that [eval] and similar constructs will break out
            // of their current execution
            e.Args.Root.Insert (0, new Node ("_return"));
            e.Args.Root.Value = e.Args.GetExValue<object> (context);

            // Adding all children of [return] as result value to evaluation
            int idxNo = 1;
            foreach (var idxRetNode in e.Args.Children) {
                e.Args.Root.Insert (idxNo++, idxRetNode.Clone ());
            }
        }
    }
}
