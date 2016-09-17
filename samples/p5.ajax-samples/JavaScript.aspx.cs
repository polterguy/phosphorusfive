/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using p5.ajax.core;

namespace p5.samples
{
    using p5 = p5.ajax.widgets;

    public partial class JavaScript : AjaxPage
    {
        [WebMethod]
        protected void javascript_widget_onclicked (p5.Literal literal, EventArgs e)
        {
            // Here we extract the "custom_data" pushed from our JavaScript handler on the client side,
            // and prepend that string to the server-time. before returning the combined results to our client again as "custom_return_value".
            Manager.SendObject ("custom_return_data", Page.Request.Params["custom_data"] + "Your server speaks the server-time " + DateTime.Now + ".\r\n");
        }
    }
}