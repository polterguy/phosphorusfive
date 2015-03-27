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
    /// <summary>
    ///     Interface for all echo response types.
    /// 
    ///     All echo response types should implement this interface.
    /// </summary>
    public interface IEchoResponse
    {
        /// <summary>
        ///     Actual implementation of echo.
        /// 
        ///     Will echo the given parameters and/or files back to client over the current HTTP response.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="node">Node containing parameters and/or files to echo.</param>
        /// <param name="response">HTTP Response.</param>
        void Echo (ApplicationContext context, Node node, HttpResponse response);
    }
}
