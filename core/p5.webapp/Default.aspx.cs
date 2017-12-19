/*
 * Phosphorus Five, copyright 2014 - 2017, Thomas Hansen, thomas@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check
 * out our website at http://gaiasoul.com for more details.
 */

using System;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using p5.core;
using p5.webapp.code;

namespace p5.webapp
{
    /// <summary>
    ///     Main ASP.NET web page for your Phosphorus Five application.
    /// </summary>
    public class Default : PhosphorusPage
    {
        /*
         * This one is necessary to make sure any HTTP requests is accepted if they're
         * using SSL, such as for instance when a POP3 server does an TLS handshake, etc.
         */
        static Default ()
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate (
                object s,
                X509Certificate certificate,
                X509Chain chain,
                SslPolicyErrors sslPolicyErrors) {
                    return true;
                };
        }

        protected override void OnInit (EventArgs e)
        {
            // Making sure our form gets the correct action attribute.
            Form.Action = HttpContext.Current.Request.RawUrl;

            // Mapping up our Page_Load event for initial loading of web page.
            Load += delegate {

                // Checking if page is not postback.
                if (!IsPostBack) {

                    // Raising our [p5.web.load-ui] Active Event, creating the node to pass in first,
                    // where the [url] node becomes the name of the form requested.
                    var url = HttpContext.Current.Request.RawUrl;

                    // Making sure we pass in [url] to our [p5.web.load-ui] event.
                    var args = new Node ("p5.web.load-ui");
                    args.Add (new Node ("url", url));

                    // Invoking the Active Event that actually loads our UI, now with a [_url] node being the URL of the requested page.
                    // Making sure we do it, in such a way, that we can handle any exceptions that occurs.
                    try {

                        ApplicationContext.RaiseEvent ("p5.web.load-ui", args);

                    } catch (Exception err) {

                        // Oops, an exception occurred.
                        // Passing it into PhosphorusPage for handling.
                        if (!HandleException (err))
                            throw;
                    }

                    // Making sure base is set for page.
                    var baseUrl = ApplicationContext.RaiseEvent ("p5.web.get-root-location").Get<string> (ApplicationContext, null);
                    var baseCtrl = new LiteralControl ();
                    baseCtrl.Text = string.Format (@"<base href= ""{0}""/>", baseUrl);
                    Page.Header.Controls.Add (baseCtrl);
                }
            };

            base.OnInit (e);
        }
    }
}
