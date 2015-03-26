/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System.IO;
using System.Net;
using phosphorus.core;

namespace phosphorus.net
{
    /// <summary>
    ///     Common interface for serializing requests.
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        ///     Will serialize the current request.
        /// 
        ///     Will serialize the current HTTP request and its parameters over the given HttpWebRequest, and its request stream.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="node">Node that contains parameters to serialize.</param>
        /// <param name="request">Request to serialize parameters into.</param>
        void Serialize (ApplicationContext context, Node node, HttpWebRequest request);
    }
}
