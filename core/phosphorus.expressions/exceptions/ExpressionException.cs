
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using phosphorus.core;

namespace phosphorus.expressions
{
    /// <summary>
    /// exception thrown when expressions contains syntax errors
    /// </summary>
    public class ExpressionException : ArgumentException
    {
        private string _expression;
        private Node _node;
        private ApplicationContext _context;
        private string _message;

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.expressions.ExpressionException"/> class
        /// </summary>
        /// <param name="expression">expression that caused exception</param>
        public ExpressionException (string expression)
        {
            _expression = expression;
        }

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.expressions.ExpressionException"/> class
        /// </summary>
        /// <param name="expression">expression that caused exception</param>
        /// <param name="message">message containing additional information</param>
        public ExpressionException (string expression, string message)
            : this (expression)
        {
            _message = message;
        }

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.expressions.ExpressionException"/> class
        /// </summary>
        /// <param name="expression">expression that caused exception</param>
        /// <param name="node">node where expression was found</param>
        /// <param name="context">application context</param>
        public ExpressionException (string expression, Node node, ApplicationContext context)
            : this (expression, null, node, context)
        { }

        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.expressions.ExpressionException"/> class
        /// </summary>
        /// <param name="expression">expression that caused exception</param>
        /// <param name="message">message providing additional information</param>
        /// <param name="node">node where expression was found</param>
        /// <param name="context">application context</param>
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
        public override string StackTrace {
            get {
                if (_node != null) {
                    Node convert = new Node ();
                    convert.AddRange (_node.Clone ().Children);
                    _context.Raise ("pf.hyperlisp.lambda2hyperlisp", convert);
                    return string.Format ("pf.lambda stack trace;\r\n{0}\r\n\r\nC# stack trace;\r\n{1}", convert.Get<string> (_context), base.StackTrace);
                } else {
                    return base.StackTrace;
                }
            }
        }

        /*
         * overriding Message to provide expression that malfunctioned as an additional piece of contextual information
         */
        public override string Message {
            get {
                string retVal = string.Format ("Expression '{0}' is not a valid expression.", _expression);
                if (_message != null)
                    retVal = _message + "\r\n" + retVal;
                return retVal;
            }
        }
    }
}
