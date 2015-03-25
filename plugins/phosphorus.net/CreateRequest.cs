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
        ///     You can supply any number of arguments and/or files if you wish. Any nodes beneath the main node that's not [headers], [cookies]
        ///     or [method], will be assumed to be an argument you wish to transfer to the server somehow.
        /// 
        ///     If your request is either a 'GET' or a 'DELETE' request, then all arguments will be passed as key/values according to the node's
        ///     name and its value. The arguments will then be passed as a part of your query string, correctly URL-encoded. You cannot transfer
        ///     any files with a query string request, and all children nodes of your arguments will be ignored.
        /// 
        ///     If your request is either a 'PUT' or 'POST' type of request, then you have several options in regards to how you wish to 
        ///     transfer your arguments to the server end-point. If your request has a 'Content-Type' header of 'multipart/xxx-something', 
        ///     then your request will be transferred as a MIME message. If your 'Content-Type' is of type 'text/xxx', then your request will 
        ///     be transferred as a plain text message, having a carriage-return/line-feed injected between all your arguments. If your 
        ///     'Content-Type' is 'application/x-www-form-urlencoded', then your request will be transferred as a URL encoded form request, 
        ///     the same way a browser would serialize its form arguments. If your 'Content-Type' is neither of the previously mentioned values, 
        ///     then your request will be transferred using a binary serializer. The default value for the 'Content-Type' header for a 'POST' 
        ///     or 'PUT' request, is 'application/x-www-form-urlencoded'.
        /// 
        ///     If you choose to serialize your request as a MIME/multipart, then you can add any MIME headers you wish, for each part you wish
        ///     to transfer. If you set the 'Content-Disposition' header to something, and its 'filename' argument to anything, and you do not
        ///     supply a value for your MIME part, then the file you supply as its 'filename' parameter, will be serialized as a MIME part, 
        ///     directly from disc, without being loaded into the memory of your client. The only way you can serialize files, without loading 
        ///     them into memory first, is by serializing your request as a 'multipart/something', and setting its 'Content-Disposition' header
        ///     correctly.
        /// 
        ///     If you wish, you can change the transfer encoding for your MIME part(s), by setting its 'Content-Transfer-Encoding' MIME header to
        ///     for instance 'binary' or 'base64'.
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
