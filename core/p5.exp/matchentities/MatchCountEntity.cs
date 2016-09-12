/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using p5.core;

namespace p5.exp.matchentities
{
    /// <summary>
    ///     Class encapsulating a '?count' match entity from expression
    /// </summary>
    public class MatchCountEntity : MatchEntity
    {
        internal MatchCountEntity (Node node, Match match)
            : base (node, match)
        { }
        
        public override Match.MatchType TypeOfMatch {
            get { return Match.MatchType.count; }
        }

        public override object Value
        {
            get
            {
                throw new ApplicationException ("Retrieving the value of a 'count' entity is not possible.");
            }
            set
            {
                throw new ApplicationException ("Changing the value of a 'count' entity is not possible.");
            }
        }
    }
}
