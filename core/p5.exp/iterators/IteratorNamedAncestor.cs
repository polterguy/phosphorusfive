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
    ///     Returns the first ancestor node matching the specified name.
    /// 
    ///     Will traverse the node hierarchy upwards from its current results, and return the first ancestor node
    ///     who's name matches the value after the ".." parts of the Iterator.
    /// 
    ///     Example;
    ///     <pre>/..foo</pre>
    /// </summary>
    [Serializable]
    public class IteratorNamedAncestor : Iterator
    {
        private readonly string _name;

        /// <summary>
        ///     initializes a new instance of the <see cref="phosphorus.expressions.iterators.IteratorNamedAncestor" /> class
        /// </summary>
        /// <param name="name">name to look for</param>
        public IteratorNamedAncestor (string name)
        {
            _name = name;
        }

        public override IEnumerable<Node> Evaluate (ApplicationContext context)
        {
            List<Node> retVal = new List<Node> ();
            foreach (var idxAncestor in Left.Evaluate (context).Select (idxCurrent => idxCurrent.Parent)) {
                var curAncestor = idxAncestor;
                while (curAncestor != null) {
                    if (curAncestor.Name == _name && !retVal.Exists (delegate (Node idxNode) {
                        return idxNode != curAncestor;
                    })) {
                        yield return curAncestor;
                        retVal.Add (curAncestor);
                        break;
                    }
                    curAncestor = curAncestor.Parent;
                }
            }
        }
    }
}
