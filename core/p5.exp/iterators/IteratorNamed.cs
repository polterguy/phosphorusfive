/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.Linq;
using System.Collections.Generic;
using p5.core;

namespace p5.exp.iterators
{
    /// <summary>
    ///     Returns all nodes with the specified name
    /// </summary>
    [Serializable]
    public class IteratorNamed : Iterator
    {
        internal readonly string Name;
        private bool _like;

        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.expressions.iterators.IteratorNamed" /> class
        /// </summary>
        /// <param name="name">name to match</param>
        public IteratorNamed (string name)
        {
            if (name.StartsWith ("~")) {

                // "Like" equality
                Name = name.Substring (1);
                _like = true;
            } else if (name.StartsWith ("\\")) {

                // Escaped "like operator"
                Name = name.Substring (1);
            } else {

                // Plain and simple
                Name = name;
            }
        }

        public override IEnumerable<Node> Evaluate (ApplicationContext context)
        {
            if (_like)
                return Left.Evaluate (context).Where (idxCurrent => idxCurrent.Name.Contains (Name));
            else
                return Left.Evaluate (context).Where (idxCurrent => idxCurrent.Name == Name);
        }
    }
}
