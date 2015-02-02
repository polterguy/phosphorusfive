
/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using NUnit.Framework;
using phosphorus.core;

namespace phosphorus.unittests
{
    [TestFixture]
    public class TestBase
    {
        protected ApplicationContext _context;

        protected Node ExecuteLambda (string hyperlisp)
        {
            Node node = new Node ();
            node.Value = hyperlisp;
            _context.Raise ("pf.hyperlisp.hyperlisp2lambda", node);
            return _context.Raise ("lambda", node);
        }
    }
}
