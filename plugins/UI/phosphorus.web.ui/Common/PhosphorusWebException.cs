
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using System;

namespace phosphorus.web.ui
{
    /// <summary>
    /// exception for something going wrong in regards to web plugin
    /// </summary>
    public class PhosphorusWebException : Exception
    {
        public PhosphorusWebException (string msg)
            : base (msg)
        { }
    }
}
