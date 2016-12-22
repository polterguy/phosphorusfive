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
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

namespace p5.core
{
    /// <summary>
    ///     Helper to remove all arguments passed into active events after invocation.
    /// 
    ///     Wrap instance of class inside a "using" statement, to have automatic and deterministic removal of arguments of specified 
    ///     Node instance.
    /// </summary>
    public class ArgsRemover : IDisposable
    {
        private List<Node> _nodes;
        private Node _args = null;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Utilities+ArgsRemover"/> class.
        /// </summary>
        /// <param name="args">Arguments.</param>
        /// <param name="removeValue">If set to <c>true</c> removes value</param>
        public ArgsRemover (Node args, bool removeValue = false)
        {
            // Storing original children, such that we can remove them when instance is disposed.
            _nodes = new List<Node> (args.Children);

            // If we should also remove value of node, we keep a reference to node such that Dispose understands it should also remove value.
            if (removeValue)
                _args = args;
        }

        /*
         * Private implementation.
         */
        void IDisposable.Dispose ()
        {
            foreach (var idx in _nodes) {
                idx.UnTie ();
            }
            if (_args != null)
                _args.Value = null;
        }
    }
}
