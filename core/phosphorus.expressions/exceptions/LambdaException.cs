/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using phosphorus.core;

namespace phosphorus.expressions.exceptions
{
    /// <summary>
    ///     exception thrown when node hierarchy contains a logical error
    /// </summary>
    public class LambdaException : ApplicationException
    {
        private readonly ApplicationContext _context;
        private readonly Node _node;

        /// <summary>
        ///     initializes a new instance of the <see cref="LambdaException" /> class
        /// </summary>
        /// <param name="message">message for exception, describing what went wrong</param>
        /// <param name="node">contextual information, where execution error was found</param>
        /// <param name="context">application context</param>
        public LambdaException (string message, Node node, ApplicationContext context)
            : base (message)
        {
            // making sure we go a couple of nodes "upwards" in hierarchy, to provide some context,
            // but not too much, to not overload user with information
            if (node.Parent != null) {
                _node = node.Parent;
                if (_node.Parent != null)
                    _node = _node.Parent;
                if (_node.Parent != null)
                    _node = _node.Parent;
                if (_node.Parent != null)
                    _node = _node.Parent;
                _node = _node.Clone ();
            } else {
                _node = node.Clone ();
            }

            // storing context since we need it to convert to Hyperlisp later
            _context = context;
        }

        /*
         * overiding StackTrace from Exception class to provide "Hyperlisp stack trace" as an additional piece of contextual information
         */

        public override string StackTrace
        {
            get
            {
                var convert = new Node ();
                convert.AddRange (_node.Clone ().Children);
                _context.Raise ("pf.hyperlisp.lambda2hyperlisp", convert);
                return string.Format ("pf.lambda stack trace;\r\n{0}\r\n\r\nC# stack trace;\r\n{1}",
                    convert.Get<string> (_context),
                    base.StackTrace);
            }
        }
    }
}