/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using System;
using NUnit.Framework;
using phosphorus.core;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local

namespace phosphorus.unittests.plugins
{
    /// <summary>
    ///     [pf.html.xxx] unit tests
    /// </summary>
    [TestFixture]
    public class Html : TestBase
    {
        public Html ()
            : base ("phosphorus.types", "phosphorus.hyperlisp", "phosphorus.lambda", "phosphorus.html")
        { }
        /// <summary>
        ///     tries to update an item without submitting a [source] or [rel-source]
        /// </summary>
        [Test]
        public void Html2Lambda01 ()
        {
            Node node = ExecuteLambda (@"pf.html.html2lambda:@""<html>
    <head>
        <title>foo bar</title>
    </head>
    <body>
        <form>
            <input type=""""text"""" id=""""foobar"""" />
            <textarea id=""""barfoo"""">success textarea</textarea>
            <div id=""""div1"""">
                <p id=""""par1"""">success 1</p>
            </div>
        </form>
        <p>success2</p>
    </body>
</html>""");
            Assert.AreEqual (1, node [0].Count);
            Assert.AreEqual ("#document", node [0] [0].Name);
            Assert.AreEqual (1, node [0] [0].Count);
            Assert.AreEqual ("html", node [0] [0] [0].Name);
            Assert.AreEqual (2, node [0] [0] [0].Count);
            Assert.AreEqual ("head", node [0] [0] [0] [0].Name);
            Assert.AreEqual ("title", node [0] [0] [0] [0] [0].Name);
            Assert.AreEqual ("foo bar", node [0] [0] [0] [0] [0].Value);
            Assert.AreEqual ("body", node [0] [0] [0] [1].Name);
            Assert.AreEqual ("form", node [0] [0] [0] [1] [0].Name);
            Assert.AreEqual ("input", node [0] [0] [0] [1] [0] [0].Name);
            Assert.AreEqual ("@type", node [0] [0] [0] [1] [0] [0] [0].Name);
            Assert.AreEqual ("text", node [0] [0] [0] [1] [0] [0] [0].Value);
            Assert.AreEqual ("@id", node [0] [0] [0] [1] [0] [0] [1].Name);
            Assert.AreEqual ("foobar", node [0] [0] [0] [1] [0] [0] [1].Value);
            Assert.AreEqual ("textarea", node [0] [0] [0] [1] [0] [1].Name);
            Assert.AreEqual ("success textarea", node [0] [0] [0] [1] [0] [1].Value);
            Assert.AreEqual ("@id", node [0] [0] [0] [1] [0] [1] [0].Name);
            Assert.AreEqual ("div", node [0] [0] [0] [1] [0] [2].Name);
            Assert.AreEqual ("@id", node [0] [0] [0] [1] [0] [2] [0].Name);
            Assert.AreEqual ("div1", node [0] [0] [0] [1] [0] [2] [0].Value);
            Assert.AreEqual ("p", node [0] [0] [0] [1] [0] [2] [1].Name);
            Assert.AreEqual ("success 1", node [0] [0] [0] [1] [0] [2] [1].Value);
            Assert.AreEqual ("@id", node [0] [0] [0] [1] [0] [2] [1] [0].Name);
            Assert.AreEqual ("par1", node [0] [0] [0] [1] [0] [2] [1] [0].Value);
            Assert.AreEqual ("p", node [0] [0] [0] [1] [1].Name);
            Assert.AreEqual ("success2", node [0] [0] [0] [1] [1].Value);
        }
    }
}