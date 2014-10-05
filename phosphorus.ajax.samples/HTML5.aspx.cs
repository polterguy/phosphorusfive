/*
 * phosphorus five, copyright 2014 - thomas@magixilluminate.com
 * phosphorus five is licensed as mitx11, see the enclosed license.txt file for details
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
        protected void video_ondblclick (pf.Literal video, EventArgs e)
        {
            if (video ["width"] == "320") {
                video ["width"] = "1024";
            } else {
                video ["width"] = "320";
            }
        }
    }
}

