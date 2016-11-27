/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
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
using p5.core;
using p5.exp.exceptions;

namespace p5.lambda.keywords.extras
{
    /// <summary>
    ///     Class wrapping the [whitelist] keyword in p5 lambda.
    /// </summary>
    public static class Whitelist
    {
        /// <summary>
        ///     The [whitelist] keyword, allows you to create a lambda context of pre-defined legal Active Events
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "whitelist")]
        public static void lambda_whitelist (ApplicationContext context, ActiveEventArgs e)
        {
            // House cleaning.
            using (new Utilities.ArgsRemover (e.Args)) {

                // Retrieves the legal keywords, and the lambda object to evaluate within this context.
                var whitelist = e.Args["events"].Clone ();
                var lambda = e.Args[".lambda"].Clone ();

                // Sanity check.
                if (whitelist == null || lambda == null)
                    throw new LambdaException ("[whitelist] requires you to supply both an [events] definition and a [.lambda] callback.", e.Args, context);

                // Making sure we evaluate our [.lambda] such that we set back the previous whitelast afterwards, if any.
                var oldWhitelist = context.Whitelist;
                try {

                    // Making sure we merge old whitelist with new whitelist, such that restrictions are not in any ways looser after merging.
                    context.Whitelist = MergeWhitelist (context, e.Args, oldWhitelist, whitelist);

                    // Evaluating [.lambda] now with our [whitelist] definition.
                    context.Raise ("eval", lambda);
                    e.Args.AddRange (lambda.Children);
                    e.Args.Value = lambda.Value;

                } finally {

                    // Making sure we set back the Whitelist definition to its old value, if any.
                    context.Whitelist = oldWhitelist;
                }
            }
        }

        /*
         * Merges whitelist with the oldWhitelist, making sure restrictions are not looser afterwards.
         */
        private static Node MergeWhitelist (
            ApplicationContext context, 
            Node args, 
            Node oldWhitelist, 
            Node whitelist)
        {
            // Checking for simple case first, which is no previous whitelist definition.
            if (oldWhitelist == null)
                return whitelist;

            // Looping through each node in whitelist, making sure it also exists in oldWhitelist.
            foreach (var idxNew in whitelist.Children) {

                // Making sure node exists in old definition.
                if (oldWhitelist[idxNew.Name] == null)
                    throw new LambdaSecurityException ("Tried to create a less restricted [whitelist]", args, context);
            }
            return whitelist;
        }
    }
}
