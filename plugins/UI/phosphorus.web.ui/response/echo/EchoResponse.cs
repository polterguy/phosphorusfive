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
    public class EchoResponse
    {
        protected IEnumerable<Node> GetParameters (Node node)
        {
            return node.FindAll (ix => ix.Name != "keep-session");
        }

        /*
         * returns base path of application
         */
        private static string _basePath;
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
