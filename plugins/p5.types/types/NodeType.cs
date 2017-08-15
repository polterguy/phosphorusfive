/*
 * Phosphorus Five, copyright 2014 - 2017, Thomas Hansen, thomas@gaiasoul.com
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

using System.Text;
using p5.core;
using p5.exp.exceptions;

namespace p5.types.types
{
    /// <summary>
    ///     Class helps converts from node to string, and vice versa
    /// </summary>
    public static class NodeType
    {
        /// <summary>
        ///     Creates a <see cref="Node">Node</see> list from its string representation
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.hyperlambda.get-object-value.node")]
        static void p5_hyperlisp_get_object_value_node (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value is Node)
                return;
            var code = e.Args.Get<string> (context);
            var tmp = new Node ("", code);
            context.RaiseEvent ("hyper2lambda", tmp);
            e.Args.Value = tmp.Count > 0 ? new Node ("", null, tmp.Children) : null;
        }

        /// <summary>
        ///     Creates a single <see cref="Node">Node</see> from its string representation
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.hyperlambda.get-object-value.abs.node")]
        static void p5_hyperlisp_get_object_value_abs_node (ApplicationContext context, ActiveEventArgs e)
        {
            var code = e.Args.Get<string> (context);
            var tmp = new Node ("", code);
            context.RaiseEvent ("hyper2lambda", tmp);

            // Different logic if there's one node or multiple nodes!
            if (tmp.Count == 1) {

                // If there's only one node, we return that as result
                e.Args.Value = tmp [0].UnTie ();
            } else if (tmp.Count > 1) {

                // Oops, error!
                throw new LambdaException (
                    "Cannot convert string to 'abs' Node, since it would create more than one resulting root node",
                    e.Args, 
                    context);
            } else {

                // No result!
                e.Args.Value = null;
            }
        }

        /// <summary>
        ///     Creates a string from a <see cref="Node">Node</see>
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.hyperlambda.get-string-value.p5.core.Node")]
        static void p5_hyperlisp_get_string_value_p5_core_Node (ApplicationContext context, ActiveEventArgs e)
        {
            var tmp = new Node ("", e.Args.Value);
            context.RaiseEvent ("lambda2hyper", tmp);
            e.Args.Value = tmp.Value;
        }

        /// <summary>
        ///     Creates a string from a <see cref="Node">Node</see> array.
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.hyperlambda.get-string-value.p5.core.Node[]")]
        static void p5_hyperlisp_get_string_value_p5_core_Node_array (ApplicationContext context, ActiveEventArgs e)
        {
            var retVal = new StringBuilder ();
            foreach (var idx in e.Args.Value as Node []) {
                retVal.Append (context.RaiseEvent ("lambda2hyper", new Node ("", idx)).Value as string);
                retVal.Append ("\r\n");
            }
            e.Args.Value = retVal.ToString ().TrimEnd ();
        }

        /// <summary>
        ///     Returns the Hyperlambda type-name for the <see cref="Node">Node</see> type
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = ".p5.hyperlambda.get-type-name.p5.core.Node")]
        static void p5_hyperlisp_get_type_name_p5_core_Node (ApplicationContext context, ActiveEventArgs e)
        {
            e.Args.Value = "node";
        }
    }
}
