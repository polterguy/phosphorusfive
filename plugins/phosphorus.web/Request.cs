/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using phosphorus.core;
using phosphorus.expressions;
using phosphorus.web.helpers;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace phosphorus.web
{
    public static class Request
    {
        [ActiveEvent (Name = "pf.web.create-request")]
        private static void pf_web_create_request (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value == null)
                return; // nothing to do here

            // iterating through every URL requested by caller
            foreach (var idxUrl in XUtil.Iterate<string> (e.Args.Value, e.Args, context)) {

                // creating our request
                IRequest request = RequestFactory.CreateRequest (context, e.Args);
                if (request != null) {
                    IResponse response = request.Execute (context, e.Args, idxUrl);
                    response.Parse (context, e.Args);
                }
            }
        }
    }
}
