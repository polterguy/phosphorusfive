/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using NUnit.Framework;
using p5.core;
using p5.exp;

namespace p5.unittests.plugins
{
    /// <summary>
    ///     unit tests for testing the [p5.file.xxx] namespace
    /// </summary>
    [TestFixture]
    public class Combination : TestBase
    {
        public Combination ()
            : base ("p5.io", "p5.hyperlisp", "p5.lambda", "p5.types", "p5.data")
        { }

        /// <summary>
        ///     making sure [save-file] works when given a constant as a filepath, and an 
        ///     Active Event invocation as [src]
        /// </summary>
        [Test]
        public void SaveDataResults ()
        {
            ExecuteLambda (@"delete-data:x:/*/*(/foo1|/foo2)
insert-data
  foo1:bar1
    foo-child:bar-child
  foo2:bar2
save-file:test1.txt
  select-data:x:/*/*(/foo1|/foo2)");
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual (@"foo1:bar1
  foo-child:bar-child
foo2:bar2", reader.ReadToEnd ());
            }
        }

        /// <summary>
        ///     making sure [save-file] works when given a constant as a filepath, and an 
        ///     Active Event invocation as [src]
        /// </summary>
        [Test]
        public void AddDataResults ()
        {
            var node = ExecuteLambda (@"_result
delete-data:x:/*/*(/foo1|/foo2)
insert-data
  foo1:bar1
    foo-child:bar-child
  foo2:bar2
add:x:/../0
  select-data:x:/*/*(/foo1|/foo2)");
            Assert.AreEqual (2, node [0].Count);
            Assert.AreEqual ("foo1", node [0] [0].Name);
            Assert.AreEqual ("bar1", node [0] [0].Value);
            Assert.AreEqual ("foo-child", node [0] [0] [0].Name);
            Assert.AreEqual ("bar-child", node [0] [0] [0].Value);
            Assert.AreEqual ("foo2", node [0] [1].Name);
            Assert.AreEqual ("bar2", node [0] [1].Value);
        }
        
        /// <summary>
        ///     making sure [save-file] works when given a constant as a filepath, and an 
        ///     Active Event invocation as [src]
        /// </summary>
        [Test]
        public void AddSplitResults ()
        {
            var node = ExecuteLambda (@"_result
add:x:/-
  split:x:/./+?value
    =:"" ""
_data:howdy world");
            Assert.AreEqual (2, node [0].Count);
            Assert.AreEqual ("howdy", node [0] [0].Name);
            Assert.AreEqual ("world", node [0] [1].Name);
        }
        
        /// <summary>
        ///     making sure [save-file] works when given a constant as a filepath, and an 
        ///     Active Event invocation as [src]
        /// </summary>
        [Test]
        public void SetJoinResults ()
        {
            var node = ExecuteLambda (@"_result
set:x:/-?value
  join:x:/./+/*
    =:,
    ==:-
_data
  foo1:bar1
  foo2:bar2");
            Assert.AreEqual ("foo1-bar1,foo2-bar2", node [0].Value);
        }
    }
}
