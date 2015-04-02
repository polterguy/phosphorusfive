/*
 * Phosphorus.Five, Copyright 2014 - 2015, Thomas Hansen - thomas@magixilluminate.com
 * Phosphorus.Five is licensed under the terms of the MIT license.
 * See the enclosed LICENSE file for details.
 */

using System;

namespace phosphorus.core
{
    public class CoreException : Exception
    {
        public CoreException ()
        { }

        public CoreException (string message)
            : base (message)
        { }
    }
}

