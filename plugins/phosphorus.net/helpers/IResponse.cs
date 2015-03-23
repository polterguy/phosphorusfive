/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using phosphorus.core;

namespace phosphorus.net.helpers
{
    /// <summary>
    ///     Common interface for all responses that phosphorus.net can create.
    /// </summary>
    public interface IResponse : IDisposable
    {
        /// <summary>
        ///     Parses the request and puts the parsed values into the specified node.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="node">Node.</param>
        void Parse (ApplicationContext context, Node node);
    }
}
