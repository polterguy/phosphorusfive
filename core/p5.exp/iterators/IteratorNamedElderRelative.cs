/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
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
using System.Text.RegularExpressions;
using p5.core;

namespace p5.exp.iterators
{
    /// <summary>
    ///     Returns first matching elder relative with given name
    /// </summary>
    [Serializable]
    public class IteratorNamedElderRelative : Iterator
    {
        internal readonly string Name;

        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.expressions.iterators.IteratorNamedElderRelative" /> class
        /// </summary>
        /// <param name="name">name to match</param>
        public IteratorNamedElderRelative (string name)
        {
            // Plain and simple
            Name = name;
        }

        public override IEnumerable<Node> Evaluate (ApplicationContext context)
        {
            foreach (var idxCur in Left.Evaluate (context)) {
                var idxCurInner = idxCur.PreviousSibling ?? idxCur.Parent;
                while (idxCurInner != null) {
                    if (idxCurInner.Name == Name) {
                        yield return idxCurInner;
                        break;
                    }
                    idxCurInner = idxCurInner.PreviousSibling ?? idxCurInner.Parent;
                }
            }
        }
    }
}
