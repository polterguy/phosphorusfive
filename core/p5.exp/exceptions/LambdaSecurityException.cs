/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using p5.core;

namespace p5.exp.exceptions
{
    /// <summary>
    ///     Exception thrown when p5 lambda security breach is encountered
    /// </summary>
    public class LambdaSecurityException : LambdaException
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LambdaSecurityException" /> class
        /// </summary>
        /// <param name="message">Message for exception, describing what went wrong</param>
        /// <param name="node">Node where expression was found</param>
        /// <param name="context">Application context Necessary to perform conversion from p5 lambda to Hyperlambda to show Hyperlambda StackTrace</param>
        public LambdaSecurityException (string message, Node node, ApplicationContext context, Exception innerException = null)
            : base (message, node, context, innerException)
        { }
    }
}
