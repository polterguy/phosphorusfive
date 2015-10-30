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
    ///     Returns all nodes with the specified name.
    /// 
    ///     Will filter away all nodes from previous Iterator that does not match the name given through this iterator. If 
    ///     name given starts with a back-slash, then the first back-slash will be removed. This allows you to use named iterators,
    ///     where the Expression engine would normally choose another type of iterator, due to that your name fulfills the criteria for
    ///     being a different type of iterator.
    /// 
    ///     For instance, to return a node who's name is "555", instead of the 555th child node of previous results, you can use; /\555
    ///     as the value of your iterator.
    /// 
    ///     Example;
    ///     <pre>/some-name</pre>
    /// 
    ///     This iterator is the default iterator being used by the Expression engine, unless it can find a better match for another type
    ///     of iterator. Meaning, if your iterator doesn't match any of the other specialized iterators, then the engine defaults to
    ///     treating your iterator as an iterator of this type.
    /// </summary>
    [Serializable]
    public class IteratorNamed : Iterator
    {
        private readonly string _name;

        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.expressions.iterators.IteratorNamed" /> class.
        /// </summary>
        /// <param name="name">name to match</param>
        public IteratorNamed (string name)
        {
            if (name.StartsWith ("\\"))
                name = name.Substring (1);
            _name = name;
        }

        public override IEnumerable<Node> Evaluate (ApplicationContext context)
        {
            return Left.Evaluate (context).Where (idxCurrent => idxCurrent.Name == _name);
        }
    }
}