/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, thomas@gaiasoul.com
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
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using p5.core;
using p5.webapp.code;

namespace p5.webapp
{
    /// <summary>
    ///     Main ASP.NET web page for your phosphorus five application
    /// </summary>
    public partial class Default : PhosphorusPage
    {
        protected HtmlGenericControl baseElement;

        protected override void OnInit(EventArgs e)
        {
            // Rewriting path to what was actually requested, such that HTML form element's action doesn't become garbage.
            // This ensures that our HTML form element stays correct. Basically "undoing" what was done in Global.asax.cs
            // In addition, when retrieving request URL later, we get the "correct" request URL, and not the URL to "Default.aspx"
            HttpContext.Current.RewritePath ((string) HttpContext.Current.Items ["_p5_original_url"]);

            // Mapping up our Page_Load event for initial loading of web page
            Load += delegate {

                // Checking if page is not postback
                if (!IsPostBack) {

                    // Raising our [p5.web.load-ui] Active Event, creating the node to pass in first,
                    // where the [_URL] node becomes the name of the form requested
                    var args = new Node("p5.web.load-ui");
                    args.Add(new Node("_url", HttpContext.Current.Items["_p5_original_url"]));

                    // Invoking the Active Event that actually loads our UI, now with a [_url] node being the URL of the requested page.
                    // Making sure we do it, in such a way, that we can handle any exceptions that occurs.
                    try {
                        ApplicationContext.Raise ("p5.web.load-ui", args);
                    } catch (Exception err) {

                        // Oops, an exception occurred.
                        // Passing it into PhosphorusPage for handling.
                        if (!HandleException (err))
                            throw;
                    }
                }

                // Making sure base is set for page.
                var baseUrl = ApplicationContext.Raise("get-application-location").Get<string> (ApplicationContext, null);
                ((IAttributeAccessor)baseElement).SetAttribute("href", baseUrl);
            };
            base.OnInit(e);
        }
    }
}
