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
    ///     Unit tests for testing combinations of modules in Phosphorus Five
    /// </summary>
    [TestFixture]
    public class Combination : TestBase
    {
        public Combination ()
            : base ("p5.io", "p5.hyperlisp", "p5.lambda", "p5.types", "p5.data", "p5.security")
        { }

        /// <summary>
        ///     Making sure [save-file] works when given a constant as a filepath, and a select-data
        ///     Active Event invocation as [src]
        /// </summary>
        [Test]
        public void SaveDataResults ()
        {
            // Making sure we're in root account
            Context.UpdateTicket (new ApplicationContext.ContextTicket("root", "root", false));

            ExecuteLambda (@"delete-data:x:/*/*(/foo1|/foo2)
insert-data
  foo1:bar1
    foo-child:bar-child
  foo2:bar2
save-file:test1.txt
  select-data:x:/*/*(/foo1|/foo2)
insert-before:x:/../0
  src:x:/../*(!/insert-before)");
            using (TextReader reader = File.OpenText (GetBasePath () + "test1.txt")) {
                Assert.AreEqual (@"foo1:bar1
  foo-child:bar-child
foo2:bar2", reader.ReadToEnd ());
            }
        }

        /// <summary>
        ///     Making sure [add] works when given an expression as destination, and a select-data
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
  select-data:x:/*/*(/foo1|/foo2)
insert-before:x:/../0
  src:x:/../*(!/insert-before)");
            Assert.AreEqual (2, node [0].Count);
            Assert.AreEqual ("foo1", node [0] [0].Name);
            Assert.AreEqual ("bar1", node [0] [0].Value);
            Assert.AreEqual ("foo-child", node [0] [0] [0].Name);
            Assert.AreEqual ("bar-child", node [0] [0] [0].Value);
            Assert.AreEqual ("foo2", node [0] [1].Name);
            Assert.AreEqual ("bar2", node [0] [1].Value);
        }
        
        /// <summary>
        ///     Making sure [add] works when given a [split] 
        ///     Active Event invocation as [src]
        /// </summary>
        [Test]
        public void AddSplitResults ()
        {
            var node = ExecuteLambda (@"_result
add:x:/-
  split:x:/./+?value
    =:"" ""
_data:howdy world
insert-before:x:/../0
  src:x:/../*(!/insert-before)");
            Assert.AreEqual (2, node [0].Count);
            Assert.AreEqual ("howdy", node [0] [0].Name);
            Assert.AreEqual ("world", node [0] [1].Name);
        }
        
        /// <summary>
        ///     Making sure [set] works when given a [join]
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
  foo2:bar2
insert-before:x:/../0
  src:x:/../*(!/insert-before)");
            Assert.AreEqual ("foo1-bar1,foo2-bar2", node [0].Value);
        }

        /// <summary>
        ///     Making sure [if] works when given two [and] [file-exist] as condition
        /// </summary>
        [Test]
        public void IfFileExist ()
        {
            var node = ExecuteLambda (@"save-file:foo1.txt
  src:foo
save-file:foo2.txt
  src:bar
if
  file-exist:foo1.txt
  and
    file-exist:foo2.txt
  set:x:/..?value
    src:success", "eval-mutable");
            Assert.AreEqual ("success", node.Value);
        }
    }
}
