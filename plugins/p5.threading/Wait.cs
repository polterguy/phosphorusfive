/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Threading;
using p5.core;

namespace p5.threading
{
    /// <summary>
    ///     Class wrapping the [wait] keyword.
    /// 
    ///     The [wait] keyword, allows you to create multiple threads beneath it, where all threads must finish, before execution leaves the [wait] block.
    /// </summary>
    public static class Wait
    {
        /// <summary>
        ///     Waits for all [lambda.fork] children to finish, before execution passes on.
        /// 
        ///     The [wait] keyword, allows you to create multiple threads beneath itself, where each thread must finish, before execution passes onwards,
        ///     past the [wait] keyword. In addition, when you create a [wait] statement, then the [wait] node itself, will be passed into all threads
        ///     created through <see cref="phosphorus.threading.LambdaFork.lambda_fork">[lambda.fork]</see> by reference. This allows you to pass in and return 
        ///     parameters to and from your threads.
        /// 
        ///     If you access the [wait] parameter from inside your threads, or you access other shared objects, then it is crucial that you do so, by
        ///     first using [lock] to lock access to your shared objects. Since otherwise, you might have a race-condition between your different threads,
        ///     where multiple threads accesses your shared object at the same time. This may create very unpredictable results. Such as for instance,
        ///     having expressions not working, since the nodes they're iterating, are changing in the middle of an expression, and so on. Use [lock]
        ///     to avoid this, whenever you're accessing a shared resource from inside your threads.
        /// 
        ///     The [wait] node, will be a children of the root node from inside of your threads, passsed in by reference, with the name of [_wait].
        /// 
        ///     Example code, that downloads the landing pages from Digg.com and StackOverflow.com, for then to return the <em>"title"</em> of both of
        ///     them to caller, in two threads, executed simultaneously, using a [wait] statement;
        /// 
        ///     <pre>wait
        ///   _titles
        ///   lambda.fork
        ///     p5.web.get:"http://digg.com"
        ///     p5.html.html2lambda:@/-/*?value
        ///     lock:downloader
        ///       append:@/../"*"/_wait/#/"*"/_titles?node
        ///         source
        ///           title
        ///       set:@/../"*"/_wait/#/"*"/_titles/+/<?value
        ///         source:@/././-/"**"/title?value
        ///   lambda.fork
        ///     p5.web.get:"http://stackoverflow.com"
        ///     p5.html.html2lambda:@/-/*?value
        ///     lock:downloader
        ///       append:@/../"*"/_wait/#/"*"/_titles?node
        ///         source
        ///           title
        ///       set:@/../"*"/_wait/#/"*"/_titles/+/<?value
        ///         source:@/././-/"**"/title?value</pre>
        /// 
        /// Notice how the above code uses the [lock] statement to lock access to [_titles], before it starts appending to its children nodes.
        /// 
        /// Notice also how the [lambda.fork] statement is the root node of both threads, and access to the [wait] node, must go through the
        ///     automatically injected children node of [lambda.fork], called [_wait], which is a reference to the outer [wait] node. This is because
        ///     whenever you create a new thread, then the [lambda.fork] node will be the root node of your execution tree, inside of your thread.
        /// 
        /// The above example is a great example of where you'd greatly benefit from multiple threads, since to download a page over HTTP, will normally
        ///     require your code to wait for the download to finish. Most of this wait time though, is spent waiting for the network to return your data,
        ///     and the remote server to process your request. This means that during this period, your CPU is "idle", and simply waiting for other servers
        ///     to finish what they're doing, before it can move onwards. By using multiple threads like we did above, this process is often significantly
        ///     hastened, since then you can create two requests simultaneously, not having to wait for the first request to finish what it is waiting for,
        ///     before the next request is created. And whenever they are both finished doing what they are supposed to do, the [wait] statement will
        ///     finish, and return the data the threads retrieved back to you.
        /// 
        ///     Typically for the above example, this reduces the time it takes to process the above logic, with almost 50%. If you tried to run the above
        ///     logic in a single thread, it would typically take almost twice as much time.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <param name="e">Parameters passed into Active Event.</param>
        [ActiveEvent (Name = "wait")]
        private static void lambda_wait (ApplicationContext context, ActiveEventArgs e)
        {
            try {
                context.Raise ("lambda", e.Args);
                if (e.Args ["__threads"] != null) {
                    foreach (var idx in e.Args ["__threads"].Children) {
                        var thread = idx.Get<Thread> (context);
                        thread.Join ();
                    }
                }
            } finally {
                if (e.Args ["__threads"] != null) {
                    e.Args ["__threads"].UnTie ();
                }
            }
        }
    }
}
