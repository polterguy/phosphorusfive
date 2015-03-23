/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using phosphorus.core;

namespace phosphorus.web.helpers
{
    public interface IRequest
    {
        IResponse Execute (ApplicationContext context, Node node, string url);
    }
}
