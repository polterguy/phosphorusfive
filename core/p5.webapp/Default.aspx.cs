/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
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

        protected override void OnInit(System.EventArgs e)
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
                    // where the [_form] node becomes the name of the form requested
                    var args = new Node("p5.web.load-ui");
                    args.Add(new Node("_form", HttpContext.Current.Items["_p5_original_url"]));

                    // Invoking the Active Event that actually loads our UI, now with a [_form] node being the URL of the requested page
                    ApplicationContext.RaiseNative("p5.web.load-ui", args);
                }

                // Making sure base is set for page
                var baseUrl = ApplicationContext.RaiseNative("get-application-location").Get<string> (ApplicationContext);
                ((IAttributeAccessor)baseElement).SetAttribute("href", baseUrl);
            };
            base.OnInit(e);
        }
    }
}
