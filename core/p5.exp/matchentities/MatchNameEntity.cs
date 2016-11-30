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

using p5.core;

namespace p5.exp.matchentities
{
    /// <summary>
    ///     Represents a match entity wrapping the Name of a node
    /// </summary>
    public class MatchNameEntity : MatchEntity
    {
        internal MatchNameEntity (Node node, Match match)
            : base (node, match)
        { }
        
        public override Match.MatchType TypeOfMatch {
            get { return Match.MatchType.name; }
        }

        public override object Value
        {
            get
            {
                object retVal = Node.Name;
                if (!string.IsNullOrEmpty (_match.Convert) && _match.Convert != "string") {

                    // We need to convert value before returning to caller
                    retVal = _match.Context.Raise (
                        ".p5.hyperlambda.get-object-value." + _match.Convert, 
                        new Node ("", retVal)).Value;
                }
                return retVal;
            }
            set
            {
                Node.Name = Utilities.Convert (_match.Context, value, "");
            }
        }
    }
}
