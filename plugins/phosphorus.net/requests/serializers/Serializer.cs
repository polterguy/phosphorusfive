/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Web;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using phosphorus.core;
using phosphorus.expressions;
using MimeKit;

namespace phosphorus.net.requests.serializers
{
    public abstract class Serializer : ISerializer
    {
        public abstract void Serialize (ApplicationContext context, Node node, HttpWebRequest request);

        private static string _basePath;
        protected static string GetBasePath (ApplicationContext context)
        {
            if (_basePath == null) {
                Node node = new Node ();
                context.Raise ("pf.core.application-folder", node);
                _basePath = node.Get<string> (context);
            }
            return _basePath;
        }

        protected IEnumerable<Node> GetArguments (Node node)
        {
            return node.FindAll (ix => ix.Name != "headers" && ix.Name != "cookies" && ix.Name != "method");
        }
    }
}
