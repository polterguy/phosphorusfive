/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Net;
using System.Web;
using phosphorus.core;
using phosphorus.expressions;
using MimeKit;

namespace phosphorus.web.helpers
{
    public class HttpUriFormattedRequest : HttpRequest
    {
        protected override string GetUri (ApplicationContext context, Node node, string url)
        {
            bool first = url.IndexOf ("?") == -1;
            foreach (var idxNode in node.FindAll (idx => idx.Name == "content" || idx.Name == "files")) {
                if (idxNode.Name == "content") {

                    // looping through each argument in current segment
                    foreach (var idxArg in XUtil.Iterate <Node> (idxNode, context)) {

                        // making sure our first argument starts with a "?", and all consecutive arguments have "&" prepended in front of them
                        if (first) {
                            first = false;
                            url += "?" + idxArg.Name + "=" + HttpUtility.UrlEncode (XUtil.Single<string> (idxArg.Value, idxArg, (context)));
                        } else {
                            url += "&" + idxArg.Name + "=" + HttpUtility.UrlEncode (XUtil.Single<string> (idxArg.Value, idxArg, (context)));
                        }
                    }
                } else  {

                    // looping through each file in current segment
                    foreach (var idxFile in XUtil.Iterate <Node> (idxNode, context)) {

                        // making sure our first argument starts with a "?", and all consecutive arguments have "&" prepended in front of them
                        if (first) {
                            first = false;
                            url += "?" + idxFile.Name + "=";
                        } else {
                            url += "&" + idxFile.Name + "=";
                        }
                        using (StreamReader reader = new StreamReader (File.OpenRead (GetBasePath (context) + XUtil.Single<string> (idxFile.Value, idxFile, context)))) {
                            url += HttpUtility.UrlEncode (reader.ReadToEnd ());
                        }
                    }
                }
            }

            // returning Uri to caller
            return url;
        }

        protected override void Decorate (ApplicationContext context, Node node, HttpWebRequest request, ContentType type)
        {
            // nothing to do here, since parameters are already handled in URL
            request.Method = "GET";
        }
    }
}
