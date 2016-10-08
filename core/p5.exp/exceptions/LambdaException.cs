/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, mr.gaia@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
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
using p5.core;

namespace p5.exp.exceptions
{
    /// <summary>
    ///     Exception thrown when node hierarchy contains a logical error
    /// </summary>
    public class LambdaException : ApplicationException
    {
        private readonly ApplicationContext _context;
        private readonly Node _node;
        private readonly int _lineNo;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LambdaException" /> class
        /// </summary>
        /// <param name="message">Message for exception, describing what went wrong</param>
        /// <param name="node">Node where expression was found</param>
        /// <param name="context">Application context Necessary to perform conversion from p5 lambda to Hyperlambda to show Hyperlambda StackTrace</param>
        public LambdaException (string message, Node node, ApplicationContext context, Exception innerException = null)
            : base (message, innerException)
        {
            // Need to find root to append current evaluation scope to stack trace!
            _node = node.Root.Clone ();

            // Figuring out which line number error node has in hierarchy
            var idxCurrent = node;
            int idxNo = 0;
            while (idxCurrent != node.Root) {
                idxCurrent = idxCurrent.PreviousNode;
                idxNo += 1;
            }
            _lineNo = idxNo + 1;

            // Storing context since we need it to convert to Hyperlambda later
            _context = context;
        }

        /*
         * Overiding StackTrace from Exception class to provide "Hyperlambda stack trace" instead of default stacktrace
         */
        public override string StackTrace
        {
            get
            {
                var convert = new Node ();
                convert.Add (_node);
                _context.Raise ("lambda2hyper", convert);
                string lisp = convert.Get<string> (_context);
                int curPosCrLf = 0, noCrLf = 0;
                while (noCrLf != _lineNo) {
                    var newLineIndex = lisp.IndexOf ("\r\n", curPosCrLf + 2);
                    if (newLineIndex == -1) {
                        curPosCrLf = lisp.Length;
                        break;
                    }
                    curPosCrLf = newLineIndex;
                    noCrLf += 1;
                }
                lisp = lisp.Substring (0, curPosCrLf) + "<<======================" + lisp.Substring (curPosCrLf);
                return lisp;
            }
        }
    }
}
