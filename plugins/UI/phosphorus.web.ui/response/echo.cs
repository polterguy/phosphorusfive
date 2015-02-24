
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Web;
using System.Text;
using phosphorus.core;
using phosphorus.expressions;

namespace phosphorus.web.ui
{
    /// <summary>
    /// helper to echo any piece of text back to client as HTTP response
    /// </summary>
    public static class echo
    {
        /*
         * helper class for encapsulating echo response
         */
        private class EchoFilter : MemoryStream
        {
            private StringBuilder _builder = new StringBuilder ();

            public void Append (string txt)
            {
                _builder.Append (txt);
            }

            public override void Close ()
            {
                HttpContext.Current.Response.Write (_builder.ToString ());
                base.Close ();
            }
        }

        /// <summary>
        /// discards the current response, and echos any piece of text back to client as HTTP response
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.echo")]
        private static void pf_web_echo (ApplicationContext context, ActiveEventArgs e)
        {
            // if [pf.web.echo] is invoked before, we reuse the previous filter
            if (!(HttpContext.Current.Response.Filter is EchoFilter)) {

                // not invoked before, creating new filter, discarding the old
                HttpContext.Current.Response.Filter = new EchoFilter ();
            }

            // retrieving a reference to our EchFilter
            EchoFilter echoFilter = HttpContext.Current.Response.Filter as EchoFilter;

            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {
                echoFilter.Append (idx);
            }
        }
    }
}
