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
        ///     Creates a new HTTP Encrypted/Signed/MIME/URL-Encoded/REST request.
        /// 
        ///     Choose which type of request you wish to create as [method], which can be either 'get', 'put', 'post' or 'delete'. The default
        ///     value of [method], if you ommit it, is 'get'. You can add HTTP headers to your request as a key/value collection beneath [headers].
        ///     In addition you can transfer cookies to the server through [cookies] as key/value pair nodes.
        /// 
        ///     You can supply any number of arguments and/or files if you wish. Any nodes beneath the main node that's not [headers], [cookies]
        ///     or [method], will be assumed to be a parameter you wish to transfer to the server somehow.
        /// 
        ///     Example of how to query Reddit for the text "foo bar" using an HTTP/GET request;
        /// 
        ///     <pre>pf.net.create-request:"http://reddit.com/search"
        ///   method:get
        ///   q:foo bar</pre>
        /// 
        ///     If your request is either a 'get' or a 'delete' request, then all parameters will be passed as key/values according to the node's
        ///     name and its value. The arguments will then be passed as a part of your query string, correctly URL-encoded.
        /// 
        ///     If your request is either a 'put' or 'post' type of request, then you have several options in regards to how you wish to 
        ///     transfer your arguments to the server end-point, including signing and encryption. If your request has a 'Content-Type' header 
        ///     of 'multipart/xxx-something', then your request will be transferred as a MIME message. If your 'Content-Type' is 
        ///     'application/x-www-form-urlencoded', then your request will be transferred as a URL encoded form request, the same way a browser 
        ///     would serialize its form arguments. If your 'Content-Type' is neither of the previously mentioned values, then your request will 
        ///     be transferred using the default content serializer, which will simply transfer whatever parameters you supply 'raw'.
        /// 
        ///     In addition, you can choose to encrypt, and/or sign, your HTTP requests, using PGP and GnuPG, if your message is a multipart message. 
        ///     The way you'd digitally sign a multipart message, is by adding a [sign] parameter, and set its value to the email address of the private 
        ///     key you wish to use for signing your multipart message. If your GnuPG storage requires a password for releasing the private key you wish
        ///     to use for signing, then you can add this as a [password] parameter beneath your [sign] parameter.
        /// 
        ///     To encrypt your multipart messages, add up an [encrypt] parameter, and add up one child node for each encryption certificate you wish 
        ///     to encrypt your message with, where the value of these nodes are the email address of the certificates you wish to use for encrypting
        ///     your multipart message.
        /// 
        ///     Example of how to create a signed and encrypted MIME HTTP request;
        /// 
        ///     <pre>pf.net.create-request:"http://127.0.0.1:8080/echo"
        ///   sign:thomas@magixilluminate.com
        ///     password:_your_password_
        ///   encrypt
        ///     :jane@doe.com
        ///     :john@doe.com
        ///   method:post
        ///   headers
        ///     Content-Type:multipart/mixed
        ///   item1:foo
        ///   item2:foo bar
        ///   file
        ///     Content-Disposition:attachment; filename=application-startup.hl</pre>
        /// 
        ///     Example of how to create a url-encoded HTTP POST request passing in 'q1' and 'q2' with the values of 'foo' and 'bar';
        ///     <pre>pf.net.create-request:"http://127.0.0.1:8080/echo"
        ///   method:post
        ///   headers
        ///     Content-Type:application/x-www-form-urlencoded
        ///   q1:foo
        ///   q2:bar</pre>
        /// 
        ///     Notice that in the above scenario, you could have completely ommitted the 'Content-Type' header, since this is the default type of request
        ///     created when creating a 'post' or 'put' request, and not explicitly adding a 'Content-Type' header.
        /// 
        ///     If you choose to serialize your request as a 'multipart/xxx', then you can add any MIME headers you wish, for each part you wish
        ///     to transfer. If you set the 'Content-Disposition' header to something, and its 'filename' argument to anything, and you do not
        ///     supply a value for your MIME part, then the file you supply as its 'filename' parameter, will be serialized as a MIME part, 
        ///     directly from disc, without being loaded into the memory of your client.
        /// 
        ///     Example of transmitting one text parameter, and one png file, as a multipart, using base64 encoding for the png file;
        /// 
        ///     <pre>pf.net.create-request:"http://127.0.0.1:8080/echo"
        ///   method:post
        ///   headers
        ///     Content-Type:multipart/mixed
        ///   q1:foo
        ///     Content-Type:text/plain
        ///   q2
        ///     Content-Type:image/png
        ///     Content-Disposition:attachment; filename=media/css/src/grid.png
        ///     Content-Transfer-Encoding:base64</pre>
        /// 
        ///     If you wish, you can change the transfer encoding for your MIME part(s), by setting its 'Content-Transfer-Encoding' MIME header to
        ///     for instance 'binary' or 'base64'.
        /// 
        ///     Notice that if you create a multipart type of request, then unless you explicitly add a boundary parameter yourself, then a 
        ///     highly unique and random boundary will be automatically created for you, as you can see an example of above. Also notice, that
        ///     when creating a multipart request, you do not have to serialize files or binary content as base64, you can also choose to serialize
        ///     your content as 'binary' type, which means that compared to traditional XML/SOAP based web services, you'll save a lot of both
        ///     bandwidth and client resources when creating your HTTP requests.
        /// 
        ///     The response from the server, will be treated according to what 'Content-Type' header the server returns. If the server returns a
        ///     'multipart/something' in its 'Content-Type' header, then the response will be assumed to be a multipart, and parsed using MimeKit.
        ///     For such cases, each MIME entity the server returns, will be put into the [result] node as children nodes themselves. A multipart
        ///     response will also automatically have inner multiparts parsed, which means you can create tree structures, parsed from multipart
        ///     return values. If the response is a multipart response, then each MIME header will be parsed, and returned as child nodes of the 
        ///     specified MIME entity wrapper node. If one MIME entity is a multipart in itself, then all of its children MIME entities will be
        ///     returned inside a [children] node, being a child of the outer multipart MIME entity.
        /// 
        ///     If the response is either 'text/xxx' or anything else, then the entire response will be put into a [content] node, as either text,
        ///     or a byte array, beneath the main [result] node for your request.
        /// 
        ///     If you do not create a multipart request, you can still transfer files to the server end-point, without having to load these files
        ///     into the memory of your client, by adding [is-file] child node beneath your parameter node, and set its value to 'true'. Below is
        ///     an example of how to transfer a file binary using an HTTP request, while also passing in an HTTP cookie.
        /// 
        ///     <pre>pf.net.create-request:"http://127.0.0.1:8080/echo"
        ///   method:post
        ///   headers
        ///     Content-Type:application/octet-stream
        ///   q2:media/css/src/grid.png
        ///     is-file:true</pre>
        /// 
        ///     If you remove the [is-file] node in the above example, or set its value to 'false', then instead of transferring the file 'grid.png',
        ///     the text-string value of the [q2] node's value will be transferred instead.
        /// 
        ///     You can pass in as many arguments as you wish with the default content serializer. Below is an example that will first transfer 
        ///     the binary value of the integer '2' to a server end-point, then a file, for then to pass in the text 'xxx-qqq-zzz', for then to 
        ///     finally pass in a date object. All of these values will be 'concatenated' in the order given, and sent to the server as a 
        ///     'binary request'.
        /// 
        ///     <pre>pf.net.create-request:"http://127.0.0.1:8080/echo"
        ///   method:post
        ///   headers
        ///     Content-Type:application/octet-stream
        ///   :int:2
        ///   :media/css/src/grid.png
        ///     is-file:true
        ///   :"\r\n--==xxx-qqq-zzz==--\r\n"
        ///   :date:"2015-01-15T23:11:57"</pre>
        /// 
        ///     In the example above, the integer value will be converted to its four bytes, and transmitted as a byte array. then the file will be transmitted,
        ///     binary, as it exists on disc, for then to have the value "\r\n--==xxx-qqq-zzz==--\r\n" transferred, before finally the 64 bit binary representation
        ///     of the DateTime object is being sent. Using this approach, you can probably simulate most types of HTTP request you wish, by combining any types
        ///     of values and/or files you wish.
        /// 
        ///     Unless you pass in [allow-auto-redirect] and set its value to 'false', then this Active Event will automatically redirect
        ///     the HTTP request if necessary, such as when the server you're requesting a document from redirects with a 301 redirect response.
        /// 
        ///     By combining this Active Event with [pf.web.response.echo], you can both consume and create web services, without breaking the
        ///     foundation the web was built on, which is REST.
        /// </summary>
        [ActiveEvent (Name = "pf.net.create-request")]
        private static void pf_net_create_request (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value == null)
                return; // nothing to do here

            // figuring out which method to use, defaulting to 'GET'
            string method = e.Args.GetExChildValue ("method", context, "get").ToLower ();

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
