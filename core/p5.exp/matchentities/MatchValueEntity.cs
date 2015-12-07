/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using p5.core;

namespace p5.exp.matchentities
{
    /// <summary>
    ///     Represents a match entity wrapping the Value of a node
    /// </summary>
    public class MatchValueEntity : MatchEntity
    {
        internal MatchValueEntity (Node node, Match match)
            : base (node, match)
        { }
        
        public override Match.MatchType TypeOfMatch {
            get { return Match.MatchType.value; }
        }

        public override object Value
        {
            get
            {
                object retVal = Node.Value;
                if (!string.IsNullOrEmpty (_match.Convert)) {
                    retVal = _match.Convert == "string" ?
                        Utilities.Convert<string> (_match.Context, retVal) :
                        _match.Context.RaiseNative ("p5.hyperlisp.get-object-value." + _match.Convert, new Node ("", retVal)).Value;
                }
                return retVal;
            }
            set
            {
                Node.Value = value; // ps, not cloned!
            }
        }
    }
}
