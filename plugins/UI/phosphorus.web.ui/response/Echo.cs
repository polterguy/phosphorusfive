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

/// <summary>
///     Main namespace for Active Events that fiddles with the HTTP response.
/// 
///     Wraps all Active Events that manipulates the HTTP response, such as changing HTTP headers, and such.
/// </summary>
namespace phosphorus.web.ui.response
{
    /// <summary>
    ///     Class encapsulating the [pf.web.echo] Active Event
    /// 
    ///     Class wrapping the Active Events necessary to echo, or write, a specific piece of text, or nodes
    ///     back to client over the HTTP response.
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
                if (_builder.Length > 0)
                    _builder.Append ("\n");
                _builder.Append (txt);
            }

            public override void Close ()
            {
                HttpContext.Current.Response.Write (_builder.ToString ());
                base.Close ();
            }
        }

        /// <summary>
        ///     Returns the given text or object back to client.
        /// 
        ///     Discards the current response, and writes the given piece of text, nodes or objects(s)
        ///     back to client over the HTTP response.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "pf.web.echo")]
        private static void pf_web_echo (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value == null)
                return; // nothing to do here ...

            // retrieving a reference to our EchFilter
            var echoFilter = HttpContext.Current.Response.Filter as EchoFilter;

            // if [pf.web.echo] is invoked before on this request, we reuse the previously created filter
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
