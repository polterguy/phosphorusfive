/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Collections.Generic;
using System.Linq;
using p5.core;
using p5.exp;
using p5.exp.exceptions;

namespace p5.lambda.keywords
{
    /// <summary>
    ///     Class wrapping execution engine keyword [databind]
    /// </summary>
    public static class Databind
    {
        /// <summary>
        ///     The [databind] keyword allows you to databind a p5.lambda object to another p5.lambda object
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "databind", Protection = EventProtection.LambdaClosed)]
        public static void lambda_databind (ApplicationContext context, ActiveEventArgs e)
        {
            // Basic syntax checking
            if (!(e.Args.Value is Expression))
                throw new LambdaException ("[databind] was not given a valid destination expression", e.Args, context);

            // Retrieving template and performing sanity check
            var template = e.Args ["template"];
            if (template == null)
                throw new LambdaException ("No [template] supplied to [databind]", e.Args, context);

            // Retrieving source
            var source = XUtil.SourceNodes (context, e.Args);

            // Databinding destination(s) by looping through each destinations given
            var ex = e.Args.Get<Expression> (context);
            foreach (var idxDest in ex.Evaluate (context, e.Args, e.Args)) {

                // Sanity check
                if (idxDest.TypeOfMatch != p5.exp.Match.MatchType.node)
                    throw new LambdaException ("The destination of a [databind] operation must be a node", e.Args, context);

                // Looping through each source
                foreach (var idxSrc in source) {

                    // Creating insertion node(s) from template, and appending to destination
                    (idxDest.Value as Node).AddRange (CreateInsertionFromTemplate (context, template, idxSrc));
                }
            }
        }

        /*
         * Parses the supplied template, and returns a list of nodes to insert
         */
        private static IEnumerable<Node> CreateInsertionFromTemplate (
            ApplicationContext context, 
            Node template, 
            Node dataSource)
        {
            // Looping through each child node of template, returning parsed node object instance to caller
            foreach (var idxTemplateChild in template.Children) {

                // Processing template child
                foreach (var idxNode in ProcessTemplateChild (context, idxTemplateChild, dataSource)) {

                    // Returning currently processed template instance
                    yield return idxNode;
                }
            }
        }

        /*
         * Processes a single template child node, by databinding entire node and children of node
         */
        private static IEnumerable<Node> ProcessTemplateChild (
            ApplicationContext context, 
            Node current, 
            Node dataSource)
        {
            // Creating our default return value
            var retVal = new Node ();

            // Checking if node has databound expression
            if (current.Name.StartsWith ("{") &&
                current.Name.EndsWith ("}")) {

                // Retrieving expression, and evaluating it
                var ex = current.Value as Expression;
                if (ex != null) {

                    // Databound expression
                    var match = ex.Evaluate (context, dataSource, current);

                    // Checking if this is an "inner databind operation", meaning a "sub template"
                    if ((match.TypeOfMatch == p5.exp.Match.MatchType.node && 
                        (string.IsNullOrEmpty (match.Convert) || match.Convert == "node")) || match.Convert == "node") {

                        // Inner databind expression type, meaning an "inner template", using result of expression as new source
                        // Notice we do NOT return "retVal" here, but rather one node for each result
                        // in inner datasource expression, treating the children nodes as "inner templates"
                        foreach (var idxSourceNode in XUtil.Iterate<Node> (context, current, dataSource)) {

                            // Creating return value databound to inner databound expression
                            // Notice we ABORT the yielding of return values after this loop, to avoid returning the "retVal" itself,
                            // but rather only return the results of this loop
                            var tmpRetVal = new Node (current.Name.Substring (1, current.Name.Length - 2));
                            tmpRetVal.AddRange (CreateInsertionFromTemplate (context, current, idxSourceNode));
                            yield return tmpRetVal;
                        }

                        // Aborting the rest of our yield operation!
                        yield break;
                    } else {

                        // Node is databound as simple value, using Single implementation on value
                        retVal.Name = current.Name.Substring (1, current.Name.Length - 2);
                        retVal.Value = XUtil.Single<object> (context, current, dataSource);
                    }
                } else {

                    // Node was possibly databound, but not as an expression, but rather a formatting expression
                    retVal.Name = current.Name.Substring (1, current.Name.Length - 2);
                    retVal.Value = XUtil.Single<object> (context, current, dataSource);
                }
            } else {

                // Node was not databound, returning "as is", making sure we support escaped curly braces
                if (current.Name.StartsWith ("\\"))
                    retVal.Name = current.Name.Substring (1);
                else
                    retVal.Name = current.Name;
                retVal.Value = current.Value;
            }

            // Looping through all children of template, processing recursively
            foreach (var idxTemplateChild in current.Children) {

                // Recursively databinding children of template node
                retVal.AddRange (ProcessTemplateChild (context, idxTemplateChild, dataSource));
            }
            yield return retVal;
        }
    }
}
