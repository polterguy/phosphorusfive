/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Linq;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.lambda.keywords.extras
{
    /// <summary>
    ///     Class wrapping execution engine keyword [apply]
    /// </summary>
    public static class Apply
    {
        /// <summary>
        ///     The [apply] keyword allows you to apply a p5.lambda object to another p5.lambda object
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "apply", Protection = EventProtection.LambdaClosed)]
        public static void lambda_apply (ApplicationContext context, ActiveEventArgs e)
        {
            // Basic syntax checking
            var destEx = e.Args.Value as Expression;
            if (destEx == null)
                throw new LambdaException ("[apply] was not given a valid destination expression", e.Args, context);

            // Retrieving template and performing sanity check
            var template = e.Args ["template"];
            if (template == null)
                throw new LambdaException ("No [template] supplied to [apply]", e.Args, context);

            // Databinding destination(s) by looping through each destinations given
            foreach (var idxDest in destEx.Evaluate (context, e.Args, e.Args)) {

                // Sanity check
                if (idxDest.TypeOfMatch != Match.MatchType.node)
                    throw new LambdaException ("The destination of a [apply] operation must be a node", e.Args, context);

                // Retrieving source, possibly relative to destination
                var source = XUtil.Source (context, e.Args, idxDest.Node, "src", new List<string> (new string[] { "template" }));

                // Looping through each source
                foreach (var idxSrc in source) {

                    // Creating insertion node(s) from template, and appending to destination
                    if (idxSrc is Node) {

                        // Source is already a node, simply inserting a cloned copy
                        (idxDest.Value as Node).AddRange (CreateInsertionFromTemplate (context, template, (idxSrc as Node).Clone ()));
                    } else {

                        // Source was not a node, converting source into a node, which will create a "wrapper node", which we discard,
                        // and only insert its children
                        var list = Utilities.Convert<Node> (context, idxSrc).Children.ToList ();
                        foreach (var idxSrcInner in list) {
                            (idxDest.Value as Node).AddRange (CreateInsertionFromTemplate (context, template, idxSrcInner));
                        }
                    }
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
            if (current.Name.StartsWith ("{@") &&
                current.Name.EndsWith ("}")) {

                // Node is databound to an Active Event source
                var activeEventName = current.Name.Substring (2, current.Name.Length - 3);

                // Keeping original node around, such that we can reset template, after invocation of Active Event
                var originalNode = current.Clone ();
                try {

                    // Setting accurate name for node, adding datasource as [_arg], and invoking Active Event source
                    current.Name = activeEventName;
                    current.Insert (0, new Node ("_dn", dataSource));
                    context.RaiseLambda (current.Name, current);

                    // Returning result, prioritising value
                    if (current.Value != null && current.Value != originalNode.Value) {
    
                        // Returning value of invocation as result
                        yield return current.Get<Node> (context).Clone ();
                    } else {

                        // Looping through result of Active Event invocation, and returning result to caller
                        foreach (var idxResult in current.Children) {
                            yield return idxResult.Clone ();
                        }
                    }

                    // Aborting the rest of our yield operation!
                    yield break;
                } finally {

                    // House cleaning after invocation
                    current.Name = originalNode.Name;
                    current.Value = originalNode.Value;
                    current.Clear ();
                    current.AddRange (originalNode.Children);
                }
            } else if (current.Name.StartsWith ("{") &&
                current.Name.EndsWith ("}")) {

                // Retrieving expression, and evaluating it
                var ex = current.Value as Expression;
                if (ex != null) {

                    // Databound expression
                    var match = ex.Evaluate (context, dataSource, current);

                    // Checking if this is an "inner apply operation", meaning a "sub template"
                    if ((match.TypeOfMatch == p5.exp.Match.MatchType.node && 
                        (string.IsNullOrEmpty (match.Convert) || match.Convert == "node")) || match.Convert == "node") {

                        // Inner apply expression type, meaning an "inner template", using result of expression as new source
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
