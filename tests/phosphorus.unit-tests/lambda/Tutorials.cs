/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using NUnit.Framework;
using phosphorus.core;

namespace phosphorus.unittests.lambda
{
    /// <summary>
    ///     unit tests for most of our tutorial code at http://magixilluminate.com/blogs
    /// </summary>
    [TestFixture]
    public class Tutorials : TestBase
    {
        public Tutorials ()
            : base ("phosphorus.lambda", "phosphorus.types", "phosphorus.hyperlisp") { }

        /// <summary>
        ///     unit test verifying this tutorial works;
        ///     http://magixilluminate.com/2015/02/16/phosphorus-five-the-set-keyword-and-variables-part-1/
        /// </summary>
        [Test]
        public void Tutorial1_1 ()
        {
            var node = ExecuteLambda (@"_foo:bar
set:@/-/?value
  source:Howdy World");

            Assert.AreEqual ("Howdy World", node [0].Value);
        }

        /// <summary>
        ///     unit test verifying this tutorial works;
        ///     http://magixilluminate.com/2015/02/16/phosphorus-five-the-set-keyword-and-variables-part-1/
        /// </summary>
        [Test]
        public void Tutorial1_2 ()
        {
            var node = ExecuteLambda (@"_data
  foo1:bar1
  foo2:bar2
set:@/../*/_data/*/?value
  source:Hello World");

            Assert.AreEqual ("Hello World", node [0] [0].Value);
            Assert.AreEqual ("Hello World", node [0] [1].Value);
        }

        /// <summary>
        ///     unit test verifying this tutorial works;
        ///     http://magixilluminate.com/2015/02/16/phosphorus-five-the-set-keyword-and-variables-part-1/
        /// </summary>
        [Test]
        public void Tutorial1_3 ()
        {
            var node = ExecuteLambda (@"_data
  foo1:bar1
  foo2:bar2
_data
  foo3:bar3
set:@/../*/_data/*/?value
  source:Hello World");

            Assert.AreEqual ("Hello World", node [0] [0].Value);
            Assert.AreEqual ("Hello World", node [0] [1].Value);
            Assert.AreEqual ("Hello World", node [1] [0].Value);
        }

        /// <summary>
        ///     unit test verifying this tutorial works;
        ///     http://magixilluminate.com/2015/02/16/phosphorus-five-the-set-keyword-and-variables-part-1/
        /// </summary>
        [Test]
        public void Tutorial1_4 ()
        {
            var node = ExecuteLambda (@"_data
  foo1:bar1
  foo2:bar2
set:@/../*/_data/*/?node
  source:""foo-update:bar-update""");

            Assert.AreEqual (2, node [0].Count);
            Assert.AreEqual ("foo-update", node [0] [0].Name);
            Assert.AreEqual ("bar-update", node [0] [0].Value);
            Assert.AreEqual ("foo-update", node [0] [1].Name);
            Assert.AreEqual ("bar-update", node [0] [1].Value);
        }

        /// <summary>
        ///     unit test verifying this tutorial works;
        ///     http://magixilluminate.com/2015/02/16/phosphorus-five-the-set-keyword-and-variables-part-1/
        /// </summary>
        [Test]
        public void Tutorial1_5 ()
        {
            var node = ExecuteLambda (@"_data
  foo1:bar1
  foo2:bar2
set:@/../*/_data/*/foo1/?value
  source:@/../*/_data/*/foo2/?node");

            Assert.AreEqual (2, node [0].Count);
            Assert.AreEqual ("foo2", node [0] [0].Get<Node> (Context).Name);
            Assert.AreEqual ("bar2", node [0] [0].Get<Node> (Context).Value);
        }

        /// <summary>
        ///     unit test verifying this tutorial works;
        ///     http://magixilluminate.com/2015/02/17/phosphorus-five-the-set-keyword-expressions-and-variables-part-2/
        /// </summary>
        [Test]
        public void Tutorial2_1 ()
        {
            var node = ExecuteLambda (@"_data:int:555
_result
set:@/-/?value
  source:@/../*/_data/?value");

            Assert.AreEqual (555, node [1].Value);
        }

        /// <summary>
        ///     unit test verifying this tutorial works;
        ///     http://magixilluminate.com/2015/02/17/phosphorus-five-the-set-keyword-expressions-and-variables-part-2/
        /// </summary>
        [Test]
        public void Tutorial2_2 ()
        {
            var node = ExecuteLambda (@"_data:555
_result
set:@/-/?value
  source:@/../*/_data/?value.int");

            Assert.AreEqual (555, node [1].Value);
        }

        /// <summary>
        ///     unit test verifying this tutorial works;
        ///     http://magixilluminate.com/2015/02/17/phosphorus-five-the-set-keyword-expressions-and-variables-part-2/
        /// </summary>
        [Test]
        public void Tutorial2_3 ()
        {
            var node = ExecuteLambda (@"_data
  source1:su
  source2:cc
  source3:ess
_result
set:@/-/?value
  source:@/../*/_data/*/?value");

            Assert.AreEqual ("success", node [1].Value);
        }

        /// <summary>
        ///     unit test verifying this tutorial works;
        ///     http://magixilluminate.com/2015/02/17/phosphorus-five-the-set-keyword-expressions-and-variables-part-2/
        /// </summary>
        [Test]
        public void Tutorial2_4 ()
        {
            var node = ExecuteLambda (@"_data
  source1:su
  source2:cc
  source3:ess
_result
set:@/-/?value
  source:{0}{1}{2}
    :@/../*/_data/0/?value
    :@/../*/_data/1/?value
    :@/../*/_data/2/?value");

            Assert.AreEqual ("success", node [1].Value);
        }

        /// <summary>
        ///     unit test verifying this tutorial works;
        ///     http://magixilluminate.com/2015/02/17/phosphorus-five-the-set-keyword-expressions-and-variables-part-2/
        /// </summary>
        [Test]
        public void Tutorial2_5 ()
        {
            var node = ExecuteLambda (@"_data
  source1:su
  source2:cc
  source3:ess
_result
set:@/-/?value
  source:{0}-error-{1}{2}
    :@/../*/_data/0/?value
    :@/../*/_data/1/?value
    :@/../*/_data/2/?value");

            Assert.AreEqual ("su-error-ccess", node [1].Value);
        }

        /// <summary>
        ///     unit test verifying this tutorial works;
        ///     http://magixilluminate.com/2015/02/17/phosphorus-five-the-set-keyword-expressions-and-variables-part-2/
        /// </summary>
        [Test]
        public void Tutorial2_6 ()
        {
            var node = ExecuteLambda (@"_data
  foo1:bar1
  foo2:bar2
set:@/../*/_data/*/?value
  rel-source:@?name");

            Assert.AreEqual (2, node [0].Count);
            Assert.AreEqual ("foo1", node [0] [0].Value);
            Assert.AreEqual ("foo2", node [0] [1].Value);
        }

        /// <summary>
        ///     unit test verifying this tutorial works;
        ///     http://magixilluminate.com/2015/02/17/phosphorus-five-the-set-keyword-expressions-and-variables-part-2/
        /// </summary>
        [Test]
        public void Tutorial2_7 ()
        {
            var node = ExecuteLambda (@"_data
  foo1:bar1
    foo1_child:Howdy
  foo2:bar2
    foo2_child:World
set:@/-/*/?value
  rel-source:@/*/?node.string
set:@/-2/*/*/?node");

            Assert.AreEqual (2, node [0].Count);
            Assert.AreEqual (0, node [0] [0].Count);
            Assert.AreEqual (0, node [0] [1].Count);
            Assert.AreEqual ("foo1_child:Howdy", node [0] [0].Value);
            Assert.AreEqual ("foo2_child:World", node [0] [1].Value);
        }

        /// <summary>
        ///     unit test verifying this tutorial works;
        ///     http://magixilluminate.com/2015/02/18/phosphorus-five-the-append-keyword-and-hyper-dimensional-boolean-algebraic-graph-expressions/
        /// </summary>
        [Test]
        public void Tutorial3_1 ()
        {
            var node = ExecuteLambda (@"_destination
append:@/../*/_destination/?node
  source
    foo1:bar1
    foo2:bar2");

            Assert.AreEqual (2, node [0].Count);
            Assert.AreEqual (0, node [0] [0].Count);
            Assert.AreEqual (0, node [0] [1].Count);
            Assert.AreEqual ("foo1", node [0] [0].Name);
            Assert.AreEqual ("bar1", node [0] [0].Value);
            Assert.AreEqual ("foo2", node [0] [1].Name);
            Assert.AreEqual ("bar2", node [0] [1].Value);
        }

        /// <summary>
        ///     unit test verifying this tutorial works;
        ///     http://magixilluminate.com/2015/02/18/phosphorus-five-the-append-keyword-and-hyper-dimensional-boolean-algebraic-graph-expressions/
        /// </summary>
        [Test]
        public void Tutorial3_2 ()
        {
            var node = ExecuteLambda (@"_source
  foo1:bar1
  foo2:bar2
_destination
append:@/../*/_destination/?node
  source:@/../*/_source/*/?node");

            Assert.AreEqual (2, node [0].Count);
            Assert.AreEqual (2, node [1].Count);
            Assert.AreEqual (0, node [1] [0].Count);
            Assert.AreEqual (0, node [1] [1].Count);
            Assert.AreEqual ("foo1", node [1] [0].Name);
            Assert.AreEqual ("bar1", node [1] [0].Value);
            Assert.AreEqual ("foo2", node [1] [1].Name);
            Assert.AreEqual ("bar2", node [1] [1].Value);
        }

        /// <summary>
        ///     unit test verifying this tutorial works;
        ///     http://magixilluminate.com/2015/02/18/phosphorus-five-the-append-keyword-and-hyper-dimensional-boolean-algebraic-graph-expressions/
        /// </summary>
        [Test]
        public void Tutorial3_3 ()
        {
            var node = ExecuteLambda (@"_destination
append:@/-/?node
  source:node:""foo1:bar1""
append:@/-2/?node
  source:@""foo2:bar2
foo3:bar3""");

            Assert.AreEqual (3, node [0].Count);
            Assert.AreEqual ("foo1", node [0] [0].Name);
            Assert.AreEqual ("bar1", node [0] [0].Value);
            Assert.AreEqual ("foo2", node [0] [1].Name);
            Assert.AreEqual ("bar2", node [0] [1].Value);
            Assert.AreEqual ("foo3", node [0] [2].Name);
            Assert.AreEqual ("bar3", node [0] [2].Value);
        }

        /// <summary>
        ///     unit test verifying this tutorial works;
        ///     http://magixilluminate.com/2015/02/18/phosphorus-five-the-append-keyword-and-hyper-dimensional-boolean-algebraic-graph-expressions/
        /// </summary>
        [Test]
        public void Tutorial3_4 ()
        {
            var node = ExecuteLambda (@"_source
  success:Howdy
  error:oops....!
  success:World
_destination
append:@/-/?node
  source:@/../*/_source/*/(!/error/)?node");

            Assert.AreEqual (3, node [0].Count);
            Assert.AreEqual (2, node [1].Count);
            Assert.AreEqual ("success", node [1] [0].Name);
            Assert.AreEqual ("Howdy", node [1] [0].Value);
            Assert.AreEqual ("success", node [1] [1].Name);
            Assert.AreEqual ("World", node [1] [1].Value);
        }

        /// <summary>
        ///     unit test verifying this tutorial works;
        ///     http://magixilluminate.com/2015/02/18/phosphorus-five-the-append-keyword-and-hyper-dimensional-boolean-algebraic-graph-expressions/
        /// </summary>
        [Test]
        public void Tutorial3_5 ()
        {
            var node = ExecuteLambda (@"_mammal
  sea
    dolphins
      smart
    salmon
    killer-whales
      smart
  land
    ape
      smart
    dogs
    humans
      smart
    donkey
_result
append:@/-/?node
  source:@/../*/_mammal/*/(/**/smart/./!/land/*/humans/)?node");

            Assert.AreEqual (3, node [1].Count);
            Assert.AreEqual ("dolphins", node [1] [0].Name);
            Assert.AreEqual (1, node [1] [0].Count);
            Assert.AreEqual ("smart", node [1] [0] [0].Name);
            Assert.AreEqual ("killer-whales", node [1] [1].Name);
            Assert.AreEqual (1, node [1] [1].Count);
            Assert.AreEqual ("smart", node [1] [1] [0].Name);
            Assert.AreEqual ("ape", node [1] [2].Name);
            Assert.AreEqual (1, node [1] [2].Count);
            Assert.AreEqual ("smart", node [1] [2] [0].Name);
        }

        /// <summary>
        ///     unit test verifying this tutorial works;
        ///     http://magixilluminate.com/2015/02/18/phosphorus-five-the-append-keyword-and-hyper-dimensional-boolean-algebraic-graph-expressions/
        /// </summary>
        [Test]
        public void Tutorial3_6 ()
        {
            var node = ExecuteLambda (@"_mammal
  sea
    dolphins
      smart
    salmon
    killer-whales
      smart
  land
    ape
      smart
    dogs
    humans
      smart
    donkey
_result
append:@/-/?node
  source:@/../*/_mammal/*/(/*/!/**/smart/./)?node");

            Assert.AreEqual (3, node [1].Count);
            Assert.AreEqual ("salmon", node [1] [0].Name);
            Assert.AreEqual ("dogs", node [1] [1].Name);
            Assert.AreEqual ("donkey", node [1] [2].Name);
        }

        /// <summary>
        ///     unit test verifying this tutorial works;
        ///     http://magixilluminate.com/2015/02/20/phosphorus-five-the-for-each-statement-web-widgets-and-even-more-expressions/
        /// </summary>
        [Test]
        public void Tutorial4_1 ()
        {
            var node = ExecuteLambda (@"_data
  foo1:Thomas Hansen
  foo2:John Doe
  foo2_5:""Error \r\nHansen""
  foo3:Jane Doe

for-each:@/../*/_data/*(!/""=/Error/"")?value
  _literal
    literal
      class:span-24 prepend-top
      innerValue
  set:@/./*/_literal/*/literal/*/innerValue?value
    source:@/././*/__dp?value
  append:@/../*/pf.web.create-widget/*/controls?node
    source:@/././*/_literal/*?node

pf.web.create-widget:example1
  class:span-24
  controls
    literal
      innerValue:Hello World
      element:h1");

            var result = Utilities.Convert<string> (node.Children, Context);
            Assert.AreEqual (@"_data
  foo1:Thomas Hansen
  foo2:John Doe
  foo2_5:@""Error 
Hansen""
  foo3:Jane Doe
for-each:@/../*/_data/*(!/""=/Error/"")?value
  _literal
    literal
      class:span-24 prepend-top
      innerValue
  set:@/./*/_literal/*/literal/*/innerValue?value
    source:@/././*/__dp?value
  append:@/../*/pf.web.create-widget/*/controls?node
    source:@/././*/_literal/*?node
pf.web.create-widget:example1
  class:span-24
  controls
    literal
      innerValue:Hello World
      element:h1
    literal
      class:span-24 prepend-top
      innerValue:Thomas Hansen
    literal
      class:span-24 prepend-top
      innerValue:John Doe
    literal
      class:span-24 prepend-top
      innerValue:Jane Doe", result);
        }

        /// <summary>
        ///     unit test verifying this tutorial works;
        ///     http://magixilluminate.com/2015/02/23/phosphorus-five-iterators-and-expressions-dissected-part-1/
        /// </summary>
        [Test]
        public void Tutorial5_1 ()
        {
            var node = ExecuteLambda (@"set:@?name
set:@?value
set:@?node");

            var result = Utilities.Convert<string> (node.Children, Context);
            Assert.AreEqual (":@?name\r\nset", result);
        }

        /// <summary>
        ///     unit test verifying this tutorial works;
        ///     http://magixilluminate.com/2015/02/23/phosphorus-five-iterators-and-expressions-dissected-part-1/
        /// </summary>
        [Test]
        public void Tutorial5_2 ()
        {
            var node = ExecuteLambda (@"lambda
  lambda
    lambda
      set:@/..?value
        source:Hello World");

            Assert.AreEqual ("Hello World", node.Value);
        }

        /// <summary>
        ///     unit test verifying this tutorial works;
        ///     http://magixilluminate.com/2015/02/23/phosphorus-five-iterators-and-expressions-dissected-part-1/
        /// </summary>
        [Test]
        public void Tutorial5_3 ()
        {
            var node = ExecuteLambda (@"lambda
  lambda
    lambda
      set:@/../0?name
        source:Hello World");

            Assert.AreEqual ("Hello World", node [0].Name);
        }

        /// <summary>
        ///     unit test verifying this tutorial works;
        ///     http://magixilluminate.com/2015/02/23/phosphorus-five-iterators-and-expressions-dissected-part-1/
        /// </summary>
        [Test]
        public void Tutorial5_4 ()
        {
            var node = ExecuteLambda (@"lambda
  lambda
    lambda
      set:@/../*/*?name
        source:Hello World");

            Assert.AreEqual ("Hello World", node [0] [0].Name);
        }

        /// <summary>
        ///     unit test verifying this tutorial works;
        ///     http://magixilluminate.com/2015/02/23/phosphorus-five-iterators-and-expressions-dissected-part-1/
        /// </summary>
        [Test]
        public void Tutorial5_5 ()
        {
            var node = ExecuteLambda (@"lambda
  _x
  lambda
    lambda
      set:@/../*/*?name
        source:Hello World");

            Assert.AreEqual ("Hello World", node [0] [0].Name);
            Assert.AreEqual ("Hello World", node [0] [1].Name);
        }

        /// <summary>
        ///     unit test verifying this tutorial works;
        ///     http://magixilluminate.com/2015/02/23/phosphorus-five-iterators-and-expressions-dissected-part-1/
        /// </summary>
        [Test]
        public void Tutorial5_6 ()
        {
            var node = ExecuteLambda (@"lambda
  _x
  lambda
    lambda
      set:@/../0/1?name
        source:Hello World");

            Assert.AreEqual ("_x", node [0] [0].Name);
            Assert.AreEqual ("Hello World", node [0] [1].Name);
        }

        /// <summary>
        ///     unit test verifying this tutorial works;
        ///     http://magixilluminate.com/2015/02/23/phosphorus-five-iterators-and-expressions-dissected-part-1/
        /// </summary>
        [Test]
        public void Tutorial5_7 ()
        {
            var node = ExecuteLambda (@"lambda
  _x
  lambda
    lambda
      set:@/../*/*/_x?name
        source:Hello World");

            Assert.AreEqual ("Hello World", node [0] [0].Name);
            Assert.AreEqual ("lambda", node [0] [1].Name);
        }

        /// <summary>
        ///     unit test verifying this tutorial works;
        ///     http://magixilluminate.com/2015/02/23/phosphorus-five-iterators-and-expressions-dissected-part-1/
        /// </summary>
        [Test]
        public void Tutorial5_8 ()
        {
            var node = ExecuteLambda (@"lambda
  _x:success
  _x:error
  lambda
    lambda
      set:@/../*/*/_x/=success?name
        source:Hello World");

            Assert.AreEqual ("Hello World", node [0] [0].Name);
            Assert.AreEqual ("_x", node [0] [1].Name);
        }

        /// <summary>
        ///     unit test verifying this tutorial works;
        ///     http://magixilluminate.com/2015/02/23/phosphorus-five-iterators-and-expressions-dissected-part-1/
        /// </summary>
        [Test]
        public void Tutorial5_9 ()
        {
            var node = ExecuteLambda (@"=
555
*
set:@/../*/\=?value
  source:success 1
set:@/../*/\555?value
  source:success 2
set:@/../*/\*?value
  source:success 3");

            Assert.AreEqual ("success 1", node [0].Value);
            Assert.AreEqual ("success 2", node [1].Value);
            Assert.AreEqual ("success 3", node [2].Value);
        }

        /// <summary>
        ///     unit test verifying this tutorial works;
        ///     http://magixilluminate.com/2015/02/23/phosphorus-five-iterators-and-expressions-dissected-part-1/
        /// </summary>
        [Test]
        public void Tutorial5_10 ()
        {
            var node = ExecuteLambda (@"""foo\r\nbar""
set:@/../*/""foo\r\nbar""?name
  source:success");

            Assert.AreEqual ("success", node [0].Name);
        }
    }
}