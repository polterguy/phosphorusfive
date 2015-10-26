/*
 * Phosphorus.Five, copyright 2014 - 2015, Thomas Hansen, isa.lightbringer@gmail.com
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using pf.core;
using pf.lambda.keywords.helpers;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace pf.lambda.keywords
{
    /// <summary>
    ///     Class wrapping the [while] keyword in pf.lambda.
    /// 
    ///     The [while] keyword allows you to loop for as long as some condition is true.
    /// </summary>
    public static class While
    {
        /// <summary>
        ///     The [while] keyword allows you to loop for as long as some condition is true.
        /// 
        ///     The [while] keyword is an alternative to [for-each] to create loops. However, where the [for-each] loops for each item in a 
        ///     result-set, [while] loops as long as some <see cref="pf.lambda.keywords.helpers.Conditions">Condition</see> is true.
        /// 
        ///     Example;
        /// 
        ///     <pre>_data
        ///   foo1:bar1
        ///   foo2:bar2
        /// while:@/-/"*"?node
        ///   set:@/./-/0?node</pre>
        /// 
        ///     Since [while] will loop for as long as a condition is true, it is easy to create a never-ending loop, that will lock the current
        ///     execution thread, creating a "dead-lock" in your pf.lambda code. To avoid such dead-locks, you must makke sure that your condition
        ///     at some point yields false. Just like the [if] keyword, the [while] keyword must have explicit [lambda.xxx] objects beneath itself,
        ///     unless your [while] condition is a "simple exists" condition. For instance;
        /// 
        ///     <pre>_data
        ///   foo1:bar1
        ///   foo2:bar2
        /// while:@/-/0?name
        ///   =:foo1
        ///   lambda
        ///     set:@/././&lt;?node</pre>
        /// 
        ///     To see a more extensive example of how to create conditions for your [while] loops, please see the documentation for 
        ///     <see cref="pf.lambda.keywords.Branching.lambda_if">[if]</see> or 
        ///     <see cref="pf.lambda.keywords.helpers.Conditions">Condition</see>.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "while")]
        private static void lambda_while (ApplicationContext context, ActiveEventArgs e)
        {
            var condition = new Conditions (e.Args, context);
            while (condition.Evaluate ()) {
                // if you are only checking for a value's existence, you don't need to supply a [lambda] object
                // beneath [while]. if you do not, [while] will execute as [lambda.immutable]
                if (condition.IsSimpleExist) {
                    // code tree does not contain any [lambda] objects beneath [while]
                    context.Raise ("lambda.immutable", e.Args);
                } else {
                    // code tree contains [lambda.xxx] objects beneath [while]
                    foreach (var idxExe in condition.ExecutionLambdas) {
                        context.Raise (idxExe.Name, idxExe);
                    }
                }
            }
        }
    }
}
