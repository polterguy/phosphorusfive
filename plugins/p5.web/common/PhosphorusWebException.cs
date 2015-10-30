/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;

namespace p5.web.ui.common
{
    /// <summary>
    ///     Exception thrown when a web error occurs.
    /// 
    ///     Exception thrown when something goes wrong in phoshorus.web.ui project, during for instance creation of
    ///     web widgets.
    /// </summary>
    public class PhosphorusWebException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.web.ui.common.PhosphorusWebException"/> class.
        /// </summary>
        /// <param name="msg">Error message.</param>
        public PhosphorusWebException (string msg)
            : base (msg) { }
    }
}