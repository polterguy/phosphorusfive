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
        public abstract void Serialize (ApplicationContext context, Node node, Stream stream, HttpWebRequest request);

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

        /*
         * returns the ContentDisposition for the given node, if there is any
         */
        protected ContentDisposition GetDisposition (ApplicationContext context, Node node)
        {
            var cntNode = node ["Content-Disposition"];
            if (cntNode != null)
                return ContentDisposition.Parse (XUtil.Single<string> (cntNode.Value, cntNode, (context)));
            return null;
        }
        
        /*
         * returns the ContentDisposition for the given node, if there is any
         */
        protected ContentType GetContentType (ApplicationContext context, Node node)
        {
            var cntNode = node ["Content-Type"];
            if (cntNode != null)
                return ContentType.Parse (XUtil.Single<string> (cntNode.Value, cntNode, (context)));
            return null;
        }
    }
}
