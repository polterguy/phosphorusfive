/*
 * phosphorus five, copyright 2014 - thomas@magixilluminate.com
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

namespace phosphorus.ajax.samples
{
    using System;
    using System.Web;
    using System.Web.UI;
    using phosphorus.ajax.core;
    using pf = phosphorus.ajax.widgets;

    public partial class HTML5 : AjaxPage
    {
        [WebMethod]
        protected void video_onclick (pf.Literal video, EventArgs e)
        {
            if (video.HasAttribute ("controls"))
                video.RemoveAttribute ("controls");
            else
                video ["controls"] = null;
        }
    }
}

