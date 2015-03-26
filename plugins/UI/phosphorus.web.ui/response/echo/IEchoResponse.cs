/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * Phosphorus.Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.Web;
using phosphorus.core;
using phosphorus.expressions;
using MimeKit;

namespace phosphorus.web.ui.response.echo
{
    public interface IEchoResponse
    {
        void Echo (ApplicationContext context, Node node, HttpResponse response);
    }
}
