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
using System;

namespace p5.lambda.keywords.extras
{
    /// <summary>
    ///     Class wrapping the [apply] Active Event.
    /// </summary>
    public static class Apply
    {
        /// <summary>
        ///     The [apply] event, allows you to braid together the results of a destination expression, with the results of one or more sources,
        ///     applying a [template] for each iteration into the destination.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "apply")]
        public static void lambda_apply (ApplicationContext context, ActiveEventArgs e)
        {
            // Retrieving [template], and performing basic sanity check.
            var template = e.Args.Children.Where (ix => ix.Name == "template").ToList ();
            if (template.Count != 1 || template[0].NextSibling != null)
                throw new LambdaException ("You must supply exactly one [template], and it must be the last argument", e.Args, context);

            // Retrieving source, and making sure it's not null, before we start iterating destinations.
            // Also making sure we ignore [template], to make sure it's not used as a source.
            var source = SourceHelper.GetSourceNodes (context, e.Args, "template");
            if (source != null) {

                // Looping through each destination, and applying all sources according to [template] definition, making sure destination is node result.
                foreach (var idxDest in SourceHelper.GetDestinationMatch (context, e.Args, true)) {

                    // We have one or more sources, iterating through each, applying to template, appending results into destination.
                    foreach (var idxSrc in source) {

                        // Applying the results of currently iterated source to the destination, according to [template] definition.
                        idxDest.Node.AddRange (ApplySourceToTemplate (context, idxSrc, template[0]));
                    }
                }
            }
        }

        /*
         * Parses the supplied template, according to specified source, and returns a list of nodes to insert.
         */
        private static IEnumerable<Node> ApplySourceToTemplate (
            ApplicationContext context,
            Node source, 
            Node template)
        {
            // Looping through each child node of template, returning parsed node object instance to caller.
            foreach (var idxTemplate in template.Children) {

                // Processing template child
                foreach (var idxResult in ProcessTemplate (context, idxTemplate, source)) {

                    // Returning currently processed template instance.
                    yield return idxResult;
                }
            }
        }

        /*
         * Processes a single template child node, by applying entire node, and children of node, recursively.
         */
        private static IEnumerable<Node> ProcessTemplate (
            ApplicationContext context, 
            Node template, 
            Node source)
        {
            // Checking if node is an event invocation.
            if (template.Name.StartsWith ("{@") && template.Name.EndsWith ("}")) {

                // Event apply invocation.
                foreach (var idx in ProcessEventInvocation (context, template, source)) {
                    yield return idx;
                }

            } else if (template.Name.StartsWith ("{") && template.Name.EndsWith ("}")) {

                // Normal databound value, retrieving match and checking if template is an inner template, at which case we don't recursively process children.
                Match match = null;
                var ex = template.Value as Expression;
                if (ex != null)
                    match = (template.Value as Expression).Evaluate (context, source, template);
                foreach (var idx in ProcessDataboundExpression (context, match, template, source)) {

                    // Checking if we should apply children.
                    if (match != null && match.TypeOfMatch != Match.MatchType.node && match.Convert != "node")
                        ProcessTemplateChildren (context, template, source, idx);
                    yield return idx;
                }

            } else {

                // This is not a databound node at all, but a static node.
                // Creating our default return value, making sure we support "escaped names", which among other things is necessary to
                // support nested [apply] invocations, where one [apply] creates another [apply].
                var retVal = new Node (template.Name.StartsWith ("\\") ? template.Name.Substring (1) : template.Name, template.Value);

                // Looping through all children of template, processing recursively.
                ProcessTemplateChildren (context, template, source, retVal);
                yield return retVal;
            }
        }

        /*
         * Process children of template recusrsively.
         */
        private static void ProcessTemplateChildren (ApplicationContext context, Node template, Node source, Node idx)
        {
            foreach (var idxTemplate in template.Children) {

                // Recursively databinding children of template node.
                idx.AddRange (ProcessTemplate (context, idxTemplate, source));
            }
        }

        /*
         * Processing a single event invocation template.
         */
        private static IEnumerable<Node> ProcessEventInvocation (
            ApplicationContext context, 
            Node template, 
            Node source)
        {
            // Figuring out Active Event to raise, making sure we remove initial "{@" and trailing "}".
            var eventName = template.Name.Substring (2, template.Name.Length - 3);

            // Keeping original node around, such that we can reset template, after invocation of Active Event.
            // This is done since the same template node might be reused for another source.
            var originalLambda = template.Clone ();

            // Setting accurate name for node, adding datasource as [_dn], and invoking Active Event.
            template.Name = eventName;
            template.Insert (0, new Node ("_dn", source));
            context.Raise (template.Name, template);

            // Returning result, prioritizing value.
            if (template.Value != null) {

                // Returning value of invocation as result, making sure we convert result to a node.
                if (template.Value is Node) {
                    yield return (template.Value as Node).Clone ();
                } else {
                    foreach (var idxChild in template.Get<Node> (context).Children) {
                        yield return idxChild.Clone ();
                    }
                }

            } else {

                // Looping through result of Active Event invocation, and returning result to caller.
                foreach (var idx in template.Children) {
                    yield return idx.Clone ();
                }
            }

            // House cleaning after invocation, since template might be reused for another source.
            template.Name = originalLambda.Name;
            template.Value = originalLambda.Value;
            template.Clear ().AddRange (originalLambda.Children);
        }

        /*
         * Invoked for a normal databound expression template node.
         */
        private static IEnumerable<Node> ProcessDataboundExpression (ApplicationContext context, Match match, Node template, Node source)
        {
            // Checking if we were given a match, at which case we iterate over match.
            if (match != null) {

                // Checking if this is an "inner apply operation", meaning a "sub template".
                if (match.TypeOfMatch == Match.MatchType.node || match.Convert == "node") {

                    // Inner apply expression type, meaning an "inner template", using result of expression as new source
                    // Notice we do NOT return "retVal" here, but rather one node for each result
                    // in inner datasource expression, treating the children nodes as "inner templates"
                    foreach (var idx in XUtil.Iterate<Node> (context, template, source)) {

                        // Creating return value databound to inner databound expression
                        // Notice we ABORT the yielding of return values after this loop, to avoid returning the "retVal" itself,
                        // but rather only return the results of this loop
                        var idxRet = new Node (template.Name.Substring (1, template.Name.Length - 2));
                        idxRet.AddRange (ApplySourceToTemplate (context, idx, template));
                        yield return idxRet;
                    }
                } else {

                    // Node is databound as simple value, using Single implementation on value, and returning applied node.
                    yield return new Node (template.Name.Substring (1, template.Name.Length - 2), XUtil.Single<object> (context, template, source));
                }
            } else {

                // Node was possibly databound, but not as an expression, but rather a formatting expression, or it was a constant.
                yield return new Node (template.Name.Substring (1, template.Name.Length - 2), XUtil.FormatNode (context, template));
            }
        }
    }
}
