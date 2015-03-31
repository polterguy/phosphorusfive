/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.Collections.Generic;
using phosphorus.core;
using phosphorus.expressions;

namespace phosphorus.web.ui.response.echo
{
    /// <summary>
    ///     Base class for all echo response types.
    /// </summary>
    public class EchoResponse
    {
        private static string _basePath;

        /// <summary>
        ///     Returns the parameters for the current echo response.
        /// </summary>
        /// <returns>The parameters.</returns>
        /// <param name="node">Node where to extract the parameters from.</param>
        protected IEnumerable<Node> GetParameters (Node node)
        {
            return node.FindAll (ix => ix.Name != "keep-session" && ix.Name != "encrypt" && ix.Name != "sign" && ix.Name != string.Empty);
        }

        /// <summary>
        ///     Helper to retrieve the base path of your application pool.
        /// 
        ///     Will return the base path for your application pool. Used when creating responses that are references to file attachments.
        /// </summary>
        /// <returns>The base path.</returns>
        /// <param name="context">Application context.</param>
        public static string GetBasePath (ApplicationContext context)
        {
            if (_basePath == null) {
                Node node = new Node ();
                context.Raise ("pf.core.application-folder", node);
                _basePath = node.Get<string> (context);
            }
            return _basePath;
        }
    }
}
