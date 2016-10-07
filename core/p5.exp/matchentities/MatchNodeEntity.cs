/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
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
 * along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the Affero GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System;
using p5.core;

namespace p5.exp.matchentities
{
    /// <summary>
    ///     Represents a match entity wrapping the Node itself
    /// </summary>
    public class MatchNodeEntity : MatchEntity
    {
        internal MatchNodeEntity (Node node, Match match)
            : base (node, match)
        { }
        
        public override Match.MatchType TypeOfMatch {
            get { return Match.MatchType.node; }
        }
        
        public override object Value
        {
            get
            {
                object retVal = Node;
                if (!string.IsNullOrEmpty (_match.Convert)) {

                    // Need to convert value before returning
                    retVal = _match.Convert == "string" ?
                        Utilities.Convert<string> (_match.Context, retVal) :
                            _match.Context.Raise ("p5.hyperlambda.get-object-value." + _match.Convert, new Node ("", retVal)).Value;
                }
                return retVal;
            }
            set
            {
                if (value == null) {

                    // Simply removing node
                    Node.UnTie ();
                } else {
                    var tmp = Utilities.Convert<Node> (_match.Context, value);
                    if (value is string) {

                        // Node was created from a conversion from string, making sure that we discard
                        // the automatically created "root node" in object
                        if (tmp.Children.Count != 1)
                            throw new ApplicationException ("Tried to convert a string that would create multiple nodes to one node");
                        tmp = tmp [0];
                        Node.Replace (tmp); // ps, NOT cloned!
                    } else {
                        Node.Replace (tmp.Clone ()); // ps, cloned!
                    }
                }
            }
        }
    }
}
