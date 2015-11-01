/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using p5.core;

namespace p5.exp.matchentities
{
    public abstract class MatchEntity
    {
        protected readonly Match _match;

        internal MatchEntity (Node node, Match match)
        {
            Node = node;
            _match = match;
        }

        public Node Node { get; private set; }

        public abstract Match.MatchType TypeOfMatch {
            get;
        }
        
        public abstract object Value {
            get;
            set;
        }
    }
}
