/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, thomas@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System.Linq;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.exp.exceptions;
using p5.lambda.helpers;

namespace p5.lambda.keywords.extras
{
    /// <summary>
    ///     Class wrapping the [apply] Active Event.
    /// </summary>
    public static class Apply
    {
        /// <summary>
        ///     The [apply] event, allows you to braid together the results of one or more sources, with one template, 
        ///     and append the results into one or more destinations.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "apply")]
        [ActiveEvent (Name = "braid")]
        public static void lambda_apply_braid (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving [template], and performing basic sanity check.
            var templates = e.Args.Children.Where (ix => ix.Name == "template");
            if (templates.Count () != 1)
                throw new LambdaException ("You must provide exactly one [template] to [apply]", e.Args, context);
            var template = templates.First ();

            // Retrieving source, ignoring [template], and making sure source is not null, before we start braiding source(s) and template into destination.
            var source = XUtil.GetSourceNodes (context, e.Args, "template");
            if (source != null) {

                // Looping through each destination, and braiding source(s) and template, before appending into destination node, 
                // assuming destination is node type of expression.
                foreach (var idxDest in XUtil.GetDestinationMatch (context, e.Args, true)) {

                    // Iterating through each source, braiding with template, and appending to destination node.
                    foreach (var idxSource in source) {

                        // Braiding template and source, appending into destination.
                        idxDest.Node.AddRange (BraidTemplateWithSource (context, template, idxSource));
                    }
                }
            }
        }

        /*
         * Braids the specified template with the specified source, and returns a bunch of nodes.
         */
        private static IEnumerable<Node> BraidTemplateWithSource (
            ApplicationContext context,
            Node template,
            Node source)
        {
            // Looping through each child node of template, returning parsed node object instance to caller.
            foreach (var idxTemplate in template.Children) {

                // Processing template child
                foreach (var idxResult in ProcessTemplateItem (context, idxTemplate, source)) {

                    // Returning currently processed template instance.
                    yield return idxResult;
                }
            }
        }

        /*
         * Processes a single template child node, by applying entire node, and children of node, recursively.
         */
        private static IEnumerable<Node> ProcessTemplateItem (
            ApplicationContext context, 
            Node child, 
            Node source)
        {
            // Checking type of template node.
            if (child.Name.StartsWith ("{@") && child.Name.EndsWith ("}")) {

                // Event invocation.
                foreach (var idx in ProcessEventItem (context, child, source)) {
                    yield return idx;
                }

            } else if (child.Name.StartsWith ("{") && child.Name.EndsWith ("}")) {

                // A databound expression, which might be either a single databound value, or a sub-template definition.
                foreach (var idx in ProcessDataItem (context, child, source)) {
                    yield return idx;
                }

            } else {

                // This is not a databound node at all, but a static node.
                // Creating our default return value, making sure we support escaping node names, which among other things is necessary to
                // support creating one [apply] from within another [apply].
                yield return new Node (
                    child.Name.StartsWith ("\\") ? child.Name.Substring (1) : child.Name, 
                    child.Value, 
                    ProcessItemChildren (context, child, source));
            }
        }

        /*
         * Processing a single event invocation template.
         */
        private static IEnumerable<Node> ProcessEventItem (
            ApplicationContext context,
            Node template,
            Node source)
        {
            // Figuring out Active Event to raise, making sure we remove initial "{@" and trailing "}".
            var name = template.Name.Substring (2, template.Name.Length - 3);

            // Sanity check.
            if (name.StartsWith (".") || name.StartsWith ("_") || name == "")
                throw new LambdaException ("[apply] tried to invoke protected event", template, context);

            // Keeping original node around, such that we can reset template, after invocation of Active Event.
            // This is done since the same template node might be reused for another source.
            var originalLambda = template.Clone ();

            // Setting accurate name for node, adding datasource as [_dn], and invoking event.
            template.Name = name;
            template.Insert (0, new Node ("_dn", source));
            context.Raise (template.Name, template);

            // Returning results of event invocation, prioritizing value.
            if (template.Value != null) {

                // Returning value of invocation as result, making sure we convert result to a node.
                if (template.Value is Node) {

                    // Returning a node that's already a node.
                    // Notice the Clone invocation, which is necessary, to support events returning "static nodes" by value.
                    yield return (template.Value as Node).Clone ();
                } else {

                    // Converting possibly a string, or something else to a node, which creates a "wrapper node", and returning that wrapper node's children.
                    // Notice the ToList invocation, which is necessary, since we're attaching the Children nodes, to another node within the caller of this method.
                    // If we hadn't Cloned, then IEnumerable would change during iteration, which is a severe .Net logical error.
                    foreach (var idxChild in template.Get<Node> (context).Children.ToList ()) {
                        yield return idxChild;
                    }
                }

            } else {

                // Looping through result of Active Event invocation, and returning result to caller.
                // Notice the ToList invocation, since we're attaching the nodes from Children to another node in the caller of this method.
                foreach (var idx in template.Children.ToList ()) {
                    yield return idx;
                }
            }

            // House cleaning after invocation, since template might be reused for another source.
            template.Name = originalLambda.Name;
            template.Value = originalLambda.Value;
            template.Clear ().AddRange (originalLambda.Children);
        }

        /*
         * Processing a single data item, which might be a single data value, constant, null, or a sub-template.
         */
        private static IEnumerable<Node> ProcessDataItem (ApplicationContext context, Node template, Node source)
        {
            // Retrieving node's name
            var name = template.Name.Substring (1, template.Name.Length - 2);

            // Checking type of template.
            if (XUtil.IsExpression (template.Value)) {

                // Expression type, checking if this is a single value template definition, or a sub-template definition.
                var match = template.Get<Expression> (context).Evaluate (context, source, template);
                if (match.TypeOfMatch == Match.MatchType.node) {

                    // Sanity check, before applying sub-template, ignoring children, since they're arguments to sub-template, and not current template.
                    if (name != "template")
                        throw new LambdaException ("A node expression can only be supplied to a sub [{template}] definition", template, context);

                    // Processing sub-template, returning one node for each item braided with source.
                    foreach (var idxResult in ProcessSubTemplate (context, template, source)) {
                        yield return idxResult;
                    }

                    // Preventing execution of default logic below.
                    yield break;
                } // Notice the fallthrough to final yield return in method, instead of else logic.
            }

            // Databound simple value, making sure we process children.
            yield return new Node (name, XUtil.Single<object> (context, template, source), ProcessItemChildren (context, template, source));
        }

        /*
         * Processing a sub-template.
         */
        private static IEnumerable<Node> ProcessSubTemplate (ApplicationContext context, Node template, Node source)
        {
            // This is a sub-template, which we treat as a new template definition, evaluating its expression relatively to the current source,
            // and braids together, before returning results to caller.
            foreach (var idxSource in XUtil.Iterate<Node> (context, template, source)) {

                // Braiding source with template, and returning to caller.
                foreach (var idx in BraidTemplateWithSource (context, template, idxSource)) {
                    yield return idx;
                }
            }
        }

        /*
         * Process children of template items recusrsively.
         */
        private static IEnumerable<Node> ProcessItemChildren (ApplicationContext context, Node template, Node source)
        {
            // Looping through children of template, avoiding formatted expressions and values.
            foreach (var idxTemplate in template.Children.Where (ix => ix.Name != "")) {

                // Recursively databinding children of template node.
                foreach (var idx in ProcessTemplateItem (context, idxTemplate, source)) {
                    yield return idx;
                }
            }
        }
    }
}
