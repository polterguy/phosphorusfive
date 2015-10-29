/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, isa.lightbringer@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using p5.core;

/// <summary>
///     Main namespace for exceptions within the phosphorus.expressions project.
/// 
///     Contains the exceptions that "phosphorus.expressions" might throw.
/// </summary>
namespace p5.exp.exceptions
{
    /// <summary>
    ///     Exception thrown when expressions contains syntax errors.
    /// 
    ///     Contains some helper logic to see Hyperlisp StackTrace, among other things, in addition to which expression
    ///     that fissled in your code, and why.
    /// </summary>
    public class ExpressionException : ArgumentException
    {
        private readonly ApplicationContext _context;
        private readonly string _expression;
        private readonly string _message;
        private readonly Node _node;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExpressionException" /> class.
        /// </summary>
        /// <param name="expression">Expression that caused exception.</param>
        /// <param name="node">Node where expression was found.</param>
        /// <param name="context">Application context. Necessary to perform conversion from p5.lambda to Hyperlisp to show Hyperlisp StackTrace.</param>
        public ExpressionException (string expression, Node node, ApplicationContext context)
            : this (expression, null, node, context) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExpressionException" /> class.
        /// </summary>
        /// <param name="expression">Expression that caused exception.</param>
        /// <param name="message">Message providing additional information.</param>
        /// <param name="node">Node where expression was found.</param>
        /// <param name="context">Application context. Necessary to perform conversion from p5.lambda to Hyperlisp to show Hyperlisp StackTrace.</param>
        public ExpressionException (string expression, string message, Node node, ApplicationContext context)
        {
            _message = message;
            _expression = expression;
            if (node != null && context != null) {
                // making sure we go a "couple of nodes upwards" in hierarchy, to provide some context,
                // but not too much, to not overload user with information
                _node = node;
                if (_node.Parent != null) {
                    _node = node.Parent;
                    if (_node.Parent != null)
                        _node = _node.Parent;
                }
                _node = _node.Clone (); // cloning, to make sure later changes don't mess with our "stack trace"
            }
            _context = context;
        }

        /*
         * overiding StackTrace from Exception class to provide "Hyperlisp stack trace" as an additional piece of contextual information
         */
        public override string StackTrace
        {
            get
            {
                if (_node == null)
                    return base.StackTrace;

                var convert = new Node ();
                convert.AddRange (_node.Clone ().Children);
                _context.Raise ("p5.hyperlisp.lambda2hyperlisp", convert);
                return string.Format ("p5.lambda stack trace;\r\n{0}\r\n\r\nC# stack trace;\r\n{1}", convert.Get<string> (_context), base.StackTrace);
            }
        }

        /*
         * overriding Message to provide expression that malfunctioned as an additional piece of contextual information
         */
        public override string Message
        {
            get
            {
                var retVal = string.Format ("Expression '{0}' is not a valid expression.", _expression);
                if (_message != null)
                    retVal = _message + "\r\n" + retVal;
                return retVal;
            }
        }
    }
}