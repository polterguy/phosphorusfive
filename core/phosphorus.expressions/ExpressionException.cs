/*
 * Phosphorus.Five, Copyright 2014 - 2015, Thomas Hansen - thomas@magixilluminate.com
 * Phosphorus.Five is licensed under the terms of the MIT license.
 * See the enclosed LICENSE file for details.
 */

using System;
using phosphorus.core;

namespace phosphorus.expressions
{
    public class ExpressionException : Exception
    {
        public ExpressionException ()
        { }
        
        public ExpressionException (string message)
            : base (message)
        { }
    }
}