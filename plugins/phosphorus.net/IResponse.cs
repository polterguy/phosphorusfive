/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using phosphorus.core;

namespace phosphorus.net
{
    /// <summary>
    ///     Common interface for all responses that Phosphorus.Net can create.
    /// </summary>
    public interface IResponse : IDisposable
    {
        /// <summary>
        ///     Will parse the current response.
        /// 
        ///     Parses the current response, and puts the result into the given node.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="node">Node to put results into.</param>
        void Parse (ApplicationContext context, Node node);
    }
}
