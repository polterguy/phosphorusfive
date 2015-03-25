/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using phosphorus.core;

namespace phosphorus.net
{
    /// <summary>
    ///     Common interface for all responses that phosphorus.net can create.
    /// </summary>
    public interface IResponse : IDisposable
    {
        void Parse (ApplicationContext context, Node node);
    }
}
