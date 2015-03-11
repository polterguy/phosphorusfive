/*
 * Phosphorus.Five, copyright 2014 - 2015, Mother Earth, Jannah, Gaia - YOU!
 * phosphorus five is licensed as mit, see the enclosed LICENSE file for details
 */

using NUnit.Framework;

namespace phosphorus.unittests.lambda
{
    /// <summary>
    ///     unit tests for testing the threading features of pf.lambda
    /// </summary>
    [TestFixture]
    public class Threading : TestBase
    {
        public Threading ()
            : base ("phosphorus.file", "phosphorus.types", "phosphorus.hyperlisp", "phosphorus.threading") { }

        /// <summary>
        ///     forks [multiple] threads using [lambda.fork], using a [wait] and a [lock] inside of thread lambda,
        ///     making sure all threads are capable of returning values through the [_wait] reference node
        /// </summary>
        [Test]
        public void Threading01 ()
        {
            var node = ExecuteLambda (@"_exe
  lock:job
    append:@/././*/_wait/#/*/_foos?node
      source:@/../*/""/foo/""?node
wait
  lambda.fork:@/../*/_exe?node
    foo1:bar1
  lambda.fork:@/../*/_exe?node
    foo2:bar2
  lambda.fork:@/../*/_exe?node
    foo3:bar3
  lambda.fork:@/../*/_exe?node
    foo4:bar4
  _foos");
            Assert.AreEqual (4, node [1] [4].Count);
        }
        
        /// <summary>
        ///     Forks a single thread, accessing shared object, making sure [lambda.fork] works as it should, and
        ///     passing by reference into threads works as it should.
        /// </summary>
        [Test]
        public void Threading02 ()
        {
            var node = ExecuteLambda (@"_foo
set:@/+/0?value
  source:@/./-?node
lambda.fork
  _foo
  lock:foo
    append:@/./-/#?node
      source
        bar:thread
sleep:1
lock:foo
  append:@/../""*""/_foo?node
    source
      bar:main");
            Assert.AreEqual (2, node [0].Count);
        }
    }
}