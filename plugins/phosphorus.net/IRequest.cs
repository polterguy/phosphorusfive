/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using phosphorus.core;

namespace phosphorus.net
{
    /// <summary>
    ///     Common interface for all types of requests Phosphorus.Net can create out of the box.
    /// </summary>
    public interface IRequest
    {
        /// <summary>
        ///     Executes the current request, and returns the response.
        /// 
        ///     Will transmit the current HTTP request, and return a response back to caller.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="node">Node to put results into.</param>
        IResponse Execute (ApplicationContext context, Node node);
    }
}
