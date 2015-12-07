/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
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
                    retVal = _match.Context.RaiseNative (
                        "p5.hyperlisp.get-object-value." + _match.Convert, 
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
