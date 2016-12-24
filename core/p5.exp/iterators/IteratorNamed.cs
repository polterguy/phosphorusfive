/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, thomas@gaiasoul.com
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
    ///     Returns all nodes with the specified name
    /// </summary>
    [Serializable]
    public class IteratorNamed : Iterator
    {
        internal readonly string Name;
        readonly bool _like;
        readonly bool _regex;

        /// <summary>
        ///     Initializes a new instance of the <see cref="IteratorNamed" /> class
        /// </summary>
        /// <param name="name">name to match</param>
        public IteratorNamed (string name)
        {
            if (name.StartsWithEx ("~")) {

                // "Like" equality
                Name = name.Substring (1);
                _like = true;
            } else if (name.StartsWithEx (":regex:")) {
                _regex = true;
                Name = name.Substring (7);
            } else if (name.StartsWithEx ("\\")) {

                // Escaped "named operator"
                Name = name.Substring (1);
            } else {

                // Plain and simple
                Name = name;
            }
        }

        public override IEnumerable<Node> Evaluate (ApplicationContext context)
        {
            if (_regex) {
                var ex = Utilities.Convert<Regex> (context, Name);
                return Left.Evaluate (context).Where (idxCurrent => ex.IsMatch(idxCurrent.Name));
            }

            if (_like)
                return Left.Evaluate (context).Where (idxCurrent => idxCurrent.Name.Contains (Name));
            return Left.Evaluate (context).Where (idxCurrent => idxCurrent.Name == Name);
        }
    }
}
