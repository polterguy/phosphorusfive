/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using System.IO;
using System.Text;
using System.Web;
using phosphorus.core;
using phosphorus.expressions;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace phosphorus.web.ui.response
{
    /// <summary>
    ///     helper to echo any piece of text back to client as HTTP response
    /// </summary>
    public static class Echo
    {
        /// <summary>
        ///     discards the current response, and echos any piece of text back to client as HTTP response
        /// </summary>
        /// <param name="context"><see cref="phosphorus.core.ApplicationContext" /> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.echo")]
        private static void pf_web_echo (ApplicationContext context, ActiveEventArgs e)
        {
            // retrieving a reference to our EchFilter
            var echoFilter = HttpContext.Current.Response.Filter as EchoFilter;

            // if [pf.web.echo] is invoked before, we reuse the previous filter
            if (echoFilter == null) {
                // not invoked before, creating new filter, discarding the old
                echoFilter = new EchoFilter ();
                HttpContext.Current.Response.Filter = echoFilter;
            }

            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {
                echoFilter.Append (idx);
            }
        }

        /*
         * helper class for encapsulating echo response
         */

        private class EchoFilter : MemoryStream
        {
            private readonly StringBuilder _builder = new StringBuilder ();
            public void Append (string txt) { _builder.Append (txt); }

            public override void Close ()
            {
                HttpContext.Current.Response.Write (_builder.ToString ());
                base.Close ();
            }
        }
    }
}