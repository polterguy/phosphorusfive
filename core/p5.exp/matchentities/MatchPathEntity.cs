/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using p5.core;

namespace p5.exp.matchentities
{
    public class MatchPathEntity : MatchEntity
    {
        internal MatchPathEntity (Node node, Match match)
            : base (node, match)
        { }
        
        public override Match.MatchType TypeOfMatch {
            get { return Match.MatchType.path; }
        }

        public override object Value
        {
            get
            {
                object retVal = Node.Path;
                if (!string.IsNullOrEmpty (_match.Convert)) {
                    retVal = _match.Convert == "string" ?
                        Utilities.Convert<string> (retVal, _match.Context) :
                        _match.Context.Raise ("p5.hyperlisp.get-object-value." + _match.Convert, new Node (string.Empty, retVal)).Value;
                }
                return retVal;
            }
            set
            {
                throw new ApplicationException ("You cannot use 'path' as destinations in your expressions");
            }
        }
    }
}
