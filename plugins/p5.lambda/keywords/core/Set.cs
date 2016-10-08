/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
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

using p5.exp;
using p5.core;
using p5.exp.exceptions;

namespace p5.lambda.keywords.core
{
    /// <summary>
    ///     Class wrapping the [set] keyword in p5 lambda
    /// </summary>
    public static class Set
    {
        /// <summary>
        ///     The [set] keyword, allows you to change nodes through p5 lambda
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "set")]
        public static void lambda_set (ApplicationContext context, ActiveEventArgs e)
        {
            // Asserting destination is expression
            var destEx = e.Args.Value as Expression;
            if (destEx == null)
                throw new LambdaException (
                    string.Format ("Not a valid destination expression given to [set], value was '{0}', expected expression", e.Args.Value),
                    e.Args,
                    context);

            // Updating destination(s) with value of source
            foreach (var idxDestination in destEx.Evaluate (context, e.Args, e.Args)) {

                // Raising "src" Active Event. Notice, this must be done once, for each destination, in case the source event is expecting
                // the relative destination to evaluate
                var src = XUtil.Source (context, e.Args, idxDestination.Node);

                // Checking how many values source returned, and throwing if there's more than one
                if (src.Count > 1)
                    throw new LambdaException ("Multiple sources for [set]", e.Args, context);

                // Single source, or null source
                idxDestination.Value = src.Count == 1 ? src [0] : null;
            }
        }
    }
}
