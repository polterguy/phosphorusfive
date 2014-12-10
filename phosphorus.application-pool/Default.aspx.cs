/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

namespace phosphorus.five.applicationpool
{
    using System;
    using System.Configuration;
    using System.Web;
    using System.Web.UI;
    using phosphorus.core;
    using phosphorus.ajax.core;
    using pf = phosphorus.ajax.widgets;

    public partial class Default : AjaxPage
    {
        private ApplicationContext _context;

        protected override void OnInit (EventArgs e)
        {
            _context = Loader.Instance.CreateApplicationContext ();
            if (!IsPostBack) {
                Load += delegate {

                    // retrieving "form name", and passing it into [pf.form-load], which for web is the local path and name of webpage
                    // executing, minus ".aspx" parts, in lower characters
                    var formName = Request.Url.LocalPath.TrimStart ('/').ToLower ();
                    formName = formName.Substring (0, formName.LastIndexOf ("."));
                    // TODO: pass in http GET parameters

                    // raising our [pf.form-load] Active Event
                    _context.Raise ("pf.load-form", new Node (string.Empty, formName));
                };
            }
            base.OnInit (e);
        }
    }
}

