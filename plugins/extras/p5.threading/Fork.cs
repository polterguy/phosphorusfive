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

using p5.core;
using p5t = p5.threading.helpers;

/// <summary>
///     Main namespace for all things related to threading in Phosphorus Five
/// </summary>
namespace p5.threading
{
    /// <summary>
    ///     Class wrapping the [fork] keyword
    /// </summary>
    public static class Fork
    {
        /// <summary>
        ///     Forks a new thread with the given lambda object(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "fork")]
        public static void fork (ApplicationContext context, ActiveEventArgs e)
        {
            // Looping through each lambda object in fork statement
            foreach (var idxThreadLambda in p5t.Thread.GetForkObjects (context, e.Args)) {

                // Starts a new thread, notice CLONE, to avoid thread having access to nodes outside of itself,
                // or lambda object being manipulated after evaluation of thread has started
                new p5t.Thread (context, idxThreadLambda.Clone ()).Start ();
            }
        }
    }
}
