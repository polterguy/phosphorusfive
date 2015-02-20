
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Collections.Generic;
using phosphorus.core;
using phosphorus.expressions;
using phosphorus.ajax.core;

namespace phosphorus.web
{
    /// <summary>
    /// wraps [pf.web.transfer-file] Active Event
    /// </summary>
    public static class transfer_file
    {
        /*
         * helper class to discard HTML/JSON response, without having to resort to ending response.
         * basically simply transfers file to client, and nothing else
         */
        private class FileFilter : MemoryStream
        {
            private List<string> _files = new List<string> ();

            public void AppendFile (string fileName)
            {
                _files.Add (fileName);
            }

            public override void Close ()
            {
                // notice, discarding all other output, and returning all files as "one" ...
                foreach (var idxFile in _files) {
                    using (FileStream stream = File.OpenRead (idxFile)) {
                        stream.CopyTo (HttpContext.Current.Response.OutputStream);
                    }
                }
                base.Close ();
            }
        }

        /// <summary>
        /// discards the current response, and transfers the file(s) given through its args
        /// </summary>
        /// <param name="context"><see cref="phosphorus.Core.ApplicationContext"/> for Active Event</param>
        /// <param name="e">parameters passed into Active Event</param>
        [ActiveEvent (Name = "pf.web.transmit-file")]
        private static void pf_web_transmit_file (ApplicationContext context, ActiveEventArgs e)
        {
            // checking to see if it's even possible to transmit a file
            IAjaxPage ajaxPage = HttpContext.Current.CurrentHandler as IAjaxPage;
            if (ajaxPage.Manager.IsPhosphorusRequest) {
                throw new PhosphorusWebException ("You cannot transfer files in an an Ajax Request");
            }

            // if [pf.web.transmit-file] is invoked before, we re-usue that same object, obviously!
            if (!(HttpContext.Current.Response.Filter is FileFilter)) {
                HttpContext.Current.Response.Filter = new FileFilter ();
            }

            // retrieving Response Filter
            FileFilter filter = HttpContext.Current.Response.Filter as FileFilter;

            foreach (var idxFile in XUtil.Iterate<string> (e.Args, context)) {
                filter.AppendFile (idxFile);
            }
        }
    }
}
