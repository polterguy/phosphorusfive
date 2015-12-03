/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.IO;
using NUnit.Framework;
using p5.core;

namespace p5.unittests.lambda
{
    /// <summary>
    ///     unit tests for testing the threading features of p5.lambda
    /// </summary>
    [TestFixture]
    public class Threading : TestBase
    {
        public Threading ()
            : base ("p5.types", "p5.hyperlisp", "p5.threading", "p5.lambda", "p5.io") { }
        
        /// <summary>
        ///     Creates a "fire and forget" thread that saves a file, verifying that the thread is executed
        /// 
        ///     NOTICE!
        ///     Some times this test might produce an error, since it assumes that if the main thread waits for
        ///     10 milliseconds, this should be sufficient enough time for the forked thread to execute its logic, which
        ///     is to produce a file on disc called "thread-test.txt". If the test fails, then run it again, if it 
        ///     doesn't fail, then the test is valid, but 10 milliseonds was not previously enough for the main thread
        ///     to wait for the forked thread to execute its logic.
        /// </summary>
        [Test]
        public void FireAndForgetThread ()
        {
            // Making sure we're in root account
            Context.UpdateTicket (new ApplicationContext.ContextTicket("root", "root", false));

            if (File.Exists (GetBasePath () + "thread-test.txt")) {
                File.Delete (GetBasePath () + "thread-test.txt");
            }

            var result = ExecuteLambda (@"fork
  save-file:thread-test.txt
    src:hello world
sleep:10
if
  file-exist:thread-test.txt
  insert-before:x:/../0
    src:success");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual ("success", result [0].Name);
        }
        
        /// <summary>
        ///     Creates two "fire and forget" threads that saves a file each, verifying that each thread is executed,
        ///     and both threads are created with one fork statement, having an expression leading to two different
        ///     node results.
        /// 
        ///     NOTICE!
        ///     Some times this test might produce an error, since it assumes that if the main thread waits for
        ///     10 milliseconds, this should be sufficient enough time for the forked thread to execute its logic, which
        ///     is to produce a file on disc called "thread-test.txt". If the test fails, then run it again, if it 
        ///     doesn't fail, then the test is valid, but 10 milliseonds was not previously enough for the main thread
        ///     to wait for the forked thread to execute its logic.
        /// </summary>
        [Test]
        public void FireAndForgetTwoThreadByOneExpressions ()
        {
            if (File.Exists (GetBasePath () + "thread-test1.txt")) {
                File.Delete (GetBasePath () + "thread-test1.txt");
            }
            if (File.Exists (GetBasePath () + "thread-test2.txt")) {
                File.Delete (GetBasePath () + "thread-test2.txt");
            }

            var result = ExecuteLambda (@"_x1
  save-file:thread-test1.txt
    src:hello world 1
_x2
  save-file:thread-test2.txt
    src:hello world 2
fork:x:/../*/~_x
sleep:10
if
  file-exist:thread-test1.txt
  and
    file-exist:thread-test2.txt
  add:x:/..
    src:success");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual ("success", result [0].Name);
        }

        /// <summary>
        ///     Creates two threads that saves a file each, and waits for them to finish, 
        ///     verifying that both threads are executed
        /// </summary>
        [Test]
        public void ForkTwoThreadsAndWait ()
        {
            if (File.Exists (GetBasePath () + "thread-test1.txt")) {
                File.Delete (GetBasePath () + "thread-test1.txt");
            }
            if (File.Exists (GetBasePath () + "thread-test2.txt")) {
                File.Delete (GetBasePath () + "thread-test2.txt");
            }

            var result = ExecuteLambda (@"wait
  fork
    save-file:thread-test1.txt
      src:hello world 1
  fork
    save-file:thread-test2.txt
      src:hello world 2
_files
  thread-test1.txt
  thread-test2.txt
if
  file-exist:x:/./-/*?name
  insert-before:x:/../0
    src:success");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual ("success", result [0].Name);
        }
        
        /// <summary>
        ///     Creates two threads that have a [wait] statement, where the wait is 10 milliseconds, and
        ///     one of the threads sleeps for 20 milliseconds, before creating a file on disc, verifying the
        ///     file is not created as we leave the wait. Then having the main thread sleep for 20 milliseconds,
        ///     verifying the file then have been successfully created.
        /// </summary>
        [Test]
        public void ForkTwoThreadsAndWait10Milliseconds ()
        {
            if (File.Exists (GetBasePath () + "thread-test.txt")) {
                File.Delete (GetBasePath () + "thread-test.txt");
            }
            var result = ExecuteLambda (@"wait:10
  fork
    foo:bar
  fork
    sleep:20
    save-file:thread-test.txt
      src:foo bar
if
  file-exist:thread-test.txt
  insert-before:x:/../0
    src:error
else
  sleep:20
  if
    file-exist:thread-test.txt
    insert-before:x:/../0
      src:success");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual ("success", result [0].Name);
        }
        
        /// <summary>
        ///     Creates two threads that have a [wait] statement, where the wait an expression yielding 20
        /// </summary>
        [Test]
        public void ForkAndWaitExpression ()
        {
            if (File.Exists (GetBasePath () + "thread-test1.txt")) {
                File.Delete (GetBasePath () + "thread-test1.txt");
            }
            if (File.Exists (GetBasePath () + "thread-test2.txt")) {
                File.Delete (GetBasePath () + "thread-test2.txt");
            }
            var result = ExecuteLambda (@"_wait:10
wait:x:/-?value
  fork
    save-file:thread-test1.txt
      src:foo bar
  fork
    sleep:20
    save-file:thread-test2.txt
      src:foo bar
if
  file-exist:thread-test1.txt
  and
    file-exist:thread-test2.txt
  insert-before:x:/../0
    src:error
else
  sleep:20
  if
    file-exist:thread-test1.txt
    and
      file-exist:thread-test2.txt
    insert-before:x:/../0
      src:success");
            Assert.AreEqual (1, result.Count);
            Assert.AreEqual ("success", result [0].Name);
        }
    }
}
