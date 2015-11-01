/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using p5.core;

namespace p5.exp.exceptions
{
    /// <summary>
    ///     Exception thrown when node hierarchy contains a logical error.
    /// 
    ///     Typically raised when there's a logical error in your p5.lambda code, such as having an [else-if] with no [if], and so on.
    /// </summary>
    public class LambdaException : ApplicationException
    {
        private readonly ApplicationContext _context;
        private readonly Node _node;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LambdaException" /> class.
        /// </summary>
        /// <param name="message">Message for exception, describing what went wrong.</param>
        /// <param name="node">Node where expression was found.</param>
        /// <param name="context">Application context. Necessary to perform conversion from p5.lambda to Hyperlisp to show Hyperlisp StackTrace.</param>
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
                _context.Raise ("p5.hyperlisp.lambda2hyperlisp", convert);
                return string.Format ("p5.lambda stack trace;\r\n{0}\r\n\r\nC# stack trace;\r\n{1}",
                    convert.Get<string> (_context),
                    base.StackTrace);
            }
        }
    }
}
