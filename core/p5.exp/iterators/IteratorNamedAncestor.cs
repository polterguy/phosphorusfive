/*
 * Phosphorus Five, copyright 2014 - 2017, Thomas Hansen, thomas@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System;
using System.Linq;
using System.Collections.Generic;
using p5.core;

namespace p5.exp.iterators
{
    /// <summary>
    ///     Returns the first ancestor node matching the specified name
    /// </summary>
    [Serializable]
    public class IteratorNamedAncestor : Iterator
    {
        readonly string _name;

        /// <summary>
        ///     Initializes a new instance of the <see cref="IteratorNamedAncestor" /> class
        /// </summary>
        /// <param name="name">name to look for</param>
        public IteratorNamedAncestor (string name)
        {
            _name = name;
        }

        public override IEnumerable<Node> Evaluate (ApplicationContext context)
        {
            var retVal = new List<Node> ();
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
