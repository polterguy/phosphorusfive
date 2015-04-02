/*
 * Phosphorus.Five, Copyright 2014 - 2015, Thomas Hansen - thomas@magixilluminate.com
 * Phosphorus.Five is licensed under the terms of the MIT license.
 * See the enclosed LICENSE file for details.
 */

using System.Collections;
using System.Collections.Generic;
using phosphorus.core;

namespace phosphorus.expressions
{
    public class Match : IEnumerable<MatchEntity>
    {
        public enum MatchType
        {
            name,
            value,
            count,
            node
        }

        private readonly List<MatchEntity> _matchEntities;
        private string _tail;

        internal Match (IEnumerable<Node> nodes, MatchType type, string tail)
        {
            TypeOfMatch = type;
            _matchEntities = new List<MatchEntity> ();
            _tail = tail;
            foreach (var idx in nodes) {
                _matchEntities.Add (new MatchEntity (idx, this, null));
            }
        }

        internal Match (MatchType type, string tail)
        {
            _matchEntities = new List<MatchEntity> ();
            TypeOfMatch = type;
            _tail = tail;
        }

        public int Count
        {
            get { return _matchEntities.Count; }
        }

        public MatchType TypeOfMatch
        {
            get; 
            private set;
        }

        public MatchEntity this [int index]
        {
            get { return _matchEntities [index]; }
        }

        internal List<MatchEntity> Entities
        {
            get { return _matchEntities; }
        }

        public IEnumerator<MatchEntity> GetEnumerator ()
        {
            return _matchEntities.GetEnumerator ();
        }

        IEnumerator IEnumerable.GetEnumerator ()
        {
            return _matchEntities.GetEnumerator ();
        }
    }
}