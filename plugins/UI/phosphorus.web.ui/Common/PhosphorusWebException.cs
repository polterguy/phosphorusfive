/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;

namespace phosphorus.web.ui.Common
{
    /// <summary>
    ///     exception for something going wrong in regards to web plugin
    /// </summary>
    public class PhosphorusWebException : Exception
    {
        public PhosphorusWebException (string msg)
            : base (msg) { }
    }
}