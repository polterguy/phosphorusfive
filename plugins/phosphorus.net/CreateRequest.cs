/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using phosphorus.core;
using phosphorus.expressions;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

/// <summary>
///     Namespace wrapping Active Events related to networking.
/// 
///     Contains useful helper Classes for creating HTTP MIME/REST requests among other things.
/// </summary>
namespace phosphorus.net
{
    /// <summary>
    ///     Class wrapping the [pf.net.create-request] Active Event.
    /// 
    ///     Contains the [pf.net.create-request] Active Event, and its associated helper methods.
    /// </summary>
    public static class CreateRequest
    {
        /// <summary>
        ///     Creates a new HTTP MIME/URL-Encoded/REST request.
        /// 
        ///     Choose which type of request you wish to create as [method], which can be either 'get', 'put', 'post' or 'delete'. The default
        ///     value of [method], if you ommit it, is 'get'. You can add HTTP headers to your request as a key/value collection beneath [headers].
        ///     In addition you can transfer cookies to the server through [cookie] as key/value pair nodes.
        /// 
        ///     You can supply any number of arguments as [content] and/or [file], which will be transferred to the server end-point. If you create
        ///     a "multipart/xxx" request, signified by setting the 'Content-Type' header to "multipart/something", then you can supply MIME headers
        ///     for your [content] or [file] entities, directly beneath your [content] or [file] node, as a key/value pair of collection. If you 
        ///     supply a [file] to be transferred, then the filename and relative path is expected to be the value of your [file] node. If you supply
        ///     a [content] node, then the content you wish to transmit, is expected to be found as the value of your [content] node. All MIME headers 
        ///     set this way, will be correctly handled by this Active Event. This means that you can for instance choose a transfer encoding for your 
        ///     argument(s) and/or file(s) as 'Content-Transfer-Encoding', and content type as 'Content-Type', etc. You can for instance set the 
        ///     'Content-Transfer-Encoding' to 'binary', 'base64', or any of the other transfer encoding values supported by MimeKit, and this Active Event
        ///     will automatically encode your argument(s) correctly according to their headers.
        /// 
        ///     Any files passed in through [file], will not be loaded into memory, but directly transferred over the HTTP request, preserving 
        ///     memory on your client.
        /// 
        ///     If you do not set the 'Content-Type' header, and your request is either a 'POST' or 'PUT' type of request, then its default value will be 
        ///     'application/x-www-form-urlencoded', and the request create will be url-encoded. If your request is url-encoded, then each child node
        ///     will be considered a URL-encoded key/value pair, transmitted to the server end-point with the name being the name of your node, and
        ///     its value being the URL-encoded value of your node. Each child node of your main [pf.net.create-request] node, except [headers], [method],
        ///     and [cookies] will then be considered an argument, and serialized this way to your server end-point, as long as it has a value.
        /// 
        ///     If you create an 'application/x-www-form-urlencoded' request, either implicitly or explicitly, then you cannot directly pass in files,
        ///     unless you load them first, and put their values into an argument node's value.
        /// 
        ///     If your request is of type 'GET' or 'DELETE', then all arguments will be sent in using the HTTP URL, meaning they will be a part of 
        ///     the URL of your request.
        /// 
        ///     Example that will create a MIME multipart HTTP request;
        /// 
        ///     <pre>pf.net.create-request:"http://127.0.0.1:8080/echo"
        ///   method:post
        ///   headers
        ///     Content-Type:multipart/mixed
        ///     foo-header:foo header value
        ///   cookies
        ///     foo-cookie:foo cookie value
        ///   file:application-startup.hl
        ///     Content-Type:text/Hyperlisp
        ///   content:Howdy world
        ///     Content-Type:text/plain
        ///     Content-Disposition:form-data; name=foo-world
        ///   file:media/css/src/grid.png
        ///     Content-Type:image/png
        ///     Content-Transfer-Encoding:base64</pre>
        /// 
        ///     Example that will create an 'application/x-www-form-urlencoded' type of request;
        /// 
        ///     <pre>pf.net.create-request:"http://127.0.0.1:8080/echo"
        ///   method:post
        ///   foo-arg:Howdy world
        ///     this-bugger:will be ignored!!!
        ///   bar-arg:Yo Dude!</pre>
        /// 
        ///     Please notice that in a 'application/x-www-form-urlencoded' type of request, any children nodes of your arguments will be ignored. 
        ///     You can still supply [cookies] and/or [headers]. The same is true for any type of 'GET' or 'DELETE' request.
        /// 
        ///     Also notice, that this Active Event will automatically parse any multipart (MIME) messages returned from the server end point.
        /// 
        ///     If you create a request with the HTTP header 'Content-Type' being some sort of 'text/xxx' type, then this Active Event will append a CR/LF
        ///     sequence between all your arguments and/or files, but still pass them in as 'one object'. This allows you to for instance, create complex
        ///     Hyperlisp graph objects, by combining [files], and/or [content] together, having them concatenated into one piece of Hyperlisp, before 
        ///     sending the combined result to your server end-point.
        /// 
        ///     If you supply a [file], and your request is of type 'multipart/something', then unless you supply a 'Content-Disposition' header for your
        ///     file object yourself, then the default value of 'Content-Disposition' for your MIME entity, will be 'attachment; filename="your-file.ext"'.
        /// 
        ///     If you create a 'multipart/something' request, then the boundary of your mime message will be appended as an argument to your 'Content-Type'
        ///     header of your request, making the boundary accessible as a part of your HTTP 'Content-Type' header for your server end-point.
        /// 
        ///     If you create a 'multipart/something' type of request, you can also nest multiparts. If you nest multiparts, then you can choose
        ///     to instead of providing the multipart's content as a value of its [content] node, supply the inner mime entities as children nodes of
        ///     the multipart's [content] node.
        /// 
        ///     Example that will create a nested MIME multipart HTTP request;
        /// 
        ///     <pre>pf.net.create-request:"http://127.0.0.1:8080/echo"
        ///   method:post
        ///   headers
        ///     Content-Type:multipart/mixed
        ///   content:Howdy world
        ///     Content-Type:text/plain
        ///   content
        ///     Content-Type:multipart/form-data
        ///     content:foo
        ///       Content-Type:text/plain
        ///     content:bar
        ///       Content-Type:application/foo-bar
        ///   content:Yo World!
        ///     Content-Type:text/plain</pre>
        /// 
        ///     If you create a 'multipart/something', then unless you explicitly give it a boundary parameter, through its 'Content-Type' header,
        ///     it will have a highly unique boundary automatically assigned to it.
        /// 
        ///     If you load a 'multipart/something' from a file, by using a [file] parameter, then unless you explicitly set its boundary parameter,
        ///     in its 'Content-Type' header, then its 'Content-Type', including its 'boundary' parameter, will be loaded from the file you supply, which
        ///     is the only sane way of making sure your multipart is correctly loaded from your file.
        /// 
        ///     Example that creates a multipart request, where the multipart is in its entirety loaded from disc;
        /// 
        ///     <pre>pf.net.create-request:"http://127.0.0.1:8080/echo"
        ///   method:post
        ///   headers
        ///     Content-Type:multipart/mixed
        ///   file:multipart.txt
        ///     Content-Type:multipart/mixed</pre>
        /// </summary>
        [ActiveEvent (Name = "pf.net.create-request")]
        private static void pf_net_create_request (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value == null)
                return; // nothing to do here

            // figuring out which method to use, defaulting to 'GET'
            string method = XUtil.Single<string> (
                e.Args.GetChildValue<object> ("method", context, null), 
                e.Args ["method"], context, "get").ToLower ();

            // checking to see if this is our guy
            if (method != "get" && method != "post" && method != "put" && method != "delete")
                return;

            // iterating through every URL requested by caller
            foreach (var idxUrl in XUtil.Iterate<string> (e.Args.Value, e.Args, context)) {

                // creating our request
                IRequest request = RequestFactory.CreateRequest (context, e.Args, method, idxUrl);
                using (IResponse response = request.Execute (context, e.Args)) {
                    if (response != null)
                        response.Parse (context, e.Args);
                }
            }
        }
    }
}
