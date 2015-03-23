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
    public static class CreateRequest
    {
        [ActiveEvent (Name = "pf.net.create-request")]
        private static void pf_net_create_request (ApplicationContext context, ActiveEventArgs e)
        {
            if (e.Args.Value == null)
                return; // nothing to do here

            // checking what type of request this is, to see if we should handle it, or let another guy handle it
            string type = XUtil.Single<string> (e.Args.GetChildValue ("type", context, "http"), e.Args ["type"], context);
            if (type != "http" && type != "https")
                return; // not our guy ...

            // iterating through every URL requested by caller
            foreach (var idxUrl in XUtil.Iterate<string> (e.Args.Value, e.Args, context)) {

                // creating our request
                IRequest request = RequestFactory.CreateRequest (context, e.Args);
                if (request != null) {
                    using (IResponse response = request.Execute (context, e.Args, idxUrl)) {
                        response.Parse (context, e.Args);
                    }
                }
            }
        }
    }
}
