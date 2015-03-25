/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System.IO;
using System.Net;
using phosphorus.core;

/// <summary>
///     Namespace wrapping helpers for serializing requests.
/// 
///     Primary helpers for serializing requests can be found in this namespace.
/// </summary>
namespace phosphorus.net
{
    /// <summary>
    ///     Common interface for serializing requests.
    /// </summary>
    public interface ISerializer
    {
        void Serialize (ApplicationContext context, Node node, Stream stream, HttpWebRequest request);
    }
}
