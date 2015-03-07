/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
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
        /*
         * helper class for encapsulating echo response
         */
        private class EchoFilter : MemoryStream
        {
            private readonly StringBuilder _builder = new StringBuilder ();

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
        ///     Discards the current response, and echos the given piece of text back to client.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.echo")]
        private static void pf_web_echo (ApplicationContext context, ActiveEventArgs e)
        {
            // retrieving a reference to our EchFilter
            var echoFilter = HttpContext.Current.Response.Filter as EchoFilter;

            // if [pf.web.echo] is invoked before on this request, therefor we reuse the previously created filter
            if (echoFilter == null) {
                // not invoked before, creating new filter, discarding the old
                echoFilter = new EchoFilter ();
                HttpContext.Current.Response.Filter = echoFilter;
            }

            foreach (var idx in XUtil.Iterate<string> (e.Args, context)) {
                echoFilter.Append (idx);
            }
        }
    }
}