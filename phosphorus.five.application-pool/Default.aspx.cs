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
            Init += delegate {
                if (!IsPostBack) {
                    _context.Raise ("pf.init", null);
                } else {
                    _context.Raise ("pf.init-postback", null);
                }
            };
            Load += delegate {
                if (!IsPostBack) {
                    _context.Raise ("pf.load", null);
                } else {
                    _context.Raise ("pf.load-postback", null);
                }
            };
            PreRender += delegate {
                if (!IsPostBack) {
                    _context.Raise ("pf.prerender", null);
                } else {
                    _context.Raise ("pf.prerender-postback", null);
                }
            };
            base.OnInit (e);
        }
    }
}

