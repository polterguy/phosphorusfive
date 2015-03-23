/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using phosphorus.core;

namespace phosphorus.net.helpers
{
    /// <summary>
    ///     Common interface for all types of requests phosphorus.net can create.
    /// </summary>
    public interface IRequest
    {
        /// <summary>
        ///     Executes the request specified through the node parameters given.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="node">Node.</param>
        /// <param name="url">URL.</param>
        IResponse Execute (ApplicationContext context, Node node, string url);
    }
}
