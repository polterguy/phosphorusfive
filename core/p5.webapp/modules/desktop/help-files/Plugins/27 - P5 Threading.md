
## p5.threading - Threading support

This project contains the thread supporting Active Events for Phosphorus Five. These Active Events, are plugins
to the core non-programming language, and allows you to create threads, wait for multiple threads, lock access
to shared objects, and most other things, expected from a modern multi-threaded library.

Notice that no GUI operations can be performed on any other threads, except the main GUI thread. If you spawn
multiple threads, you'll have to use for instance a **[wait]** invocation, to allow the GUI thread to modify
the GUI based on the results of some worker thread's results.

### [fork], creating a new worker thread

To spawn a new worker thread, you would do something like this.

```hyperlambda
fork
  set:x:?value
    src:foo
```



Notice, if you evaluate the above piece of lambda, in for instance Hypereval, it will not show you an updated
**[set]** node. The reason is, because what your thread is actually doing its work in regards to, is a _copy_
of your original lambda block. So although the thread is actually evaluating the above **[set]** invocation,
the results of its evaluation is never returned to you. First of all, because the GUI thread is highly
likely finished long before the spawned thread even gets to start. Secondly, to avoid race conditions and
similar problems, no thread returns its modified lambda, unless you explcitly use a **[wait]** block.

An invocation to **[fork]**, besides from the above mentioned problems, works similar to an **[eval]** invocation.
Except (of course), it returns nothing to the caller. You can even have an expression, leading to multiple
lambda blocks, and spawn of one thread for each lambda block, with one invocation. The following would spawn
_two_ threads.

```hyperlambda
_exe
  set:x:?value
    src:foo
_exe
  set:x:?value
    src:bar
fork:x:/../*/_exe
```

### [wait], waiting for threads to finish.

If you use a **[wait]** invocation, wrapping the above **[fork]** invocation though, then the GUI thread will
wait for all threads beneath the **[wait]** to finish, and update its **[fork]** lambda blocks, with the result,
after evaluation. Example given below.

```hyperlambda
wait
  fork
    set:x:?value
      src:foo
```

Contrary to our first example, not showing any results, the above will yield the following result.

```hyperlambda
wait
  fork
    set:foo
      src:foo
```

You can also use a **[wait]** block, wrapped around a **[fork]** invocation, having an expression leading to
multiple evaluation objects, like we did above. Example is given below.

```hyperlambda
_exe
  set:x:?value
    src:foo
_exe
  set:x:?value
    src:bar
wait
  fork:x:/../*/_exe
```

As with our first **[wait]** example, the results of our last example, will update the **[\_exe1]** and **[\_exe2]**
nodes, due to the single **[wait]** invocation, wrapping our single **[fork]** invocation, leading to our lambda
waiting for both threads to finish their work, before it proceeds, at which point the evaluated result node,
will replace the original nodes we had in **[\_exe1]** and **[\_exe2]**.

The result of the above evaluation hence becomes.

```hyperlambda
_exe
  set:foo
    src:foo
_exe
  set:bar
    src:bar
wait
  fork:x:/../*/_exe
```

Wait cannot have any other types of children except *[fork]* nodes.

### Waiting for a specified amount of time

The **[wait]** Active Event, optionally takes an integer value, which is the number of milliseconds, before it
stops waiting, and aborts the threads not having finished yet. To see this in action, we can create two threads,
where they both use the **[sleep]** Active Event, in such a way, that only one of them gets to finish, before
our **[wait]** block stops waiting, and kills our threads.

```hyperlambda
wait:500
  fork
    sleep:50
    set:x:?value
      src:foo
  fork
    sleep:1000
    set:x:?value
      src:foo
```

The above lambda, would first of all take .5 seconds to evaluate, since our **[wait]** block sets this as an
argument for how long to wait. Secondly, our last **[fork]** invocation, would not get to finish its job,
before our **[wait]** block no longer wants to wait for it to finish. Hence, only the results of our first
**[fork]** will get to finish its work, and get to return its results back to us. Leading us to the following
result.

```hyperlambda
wait:500
  fork
    sleep:50
    set:foo
      src:foo
  fork
    sleep:1000
    set:x:?value
      src:foo
```

Notice the second **[set]** invocation still having its original expression value, while our first **[set]**
gets to return its results.

The **[sleep]** Active Event sleeps the current thread, for a specified milliseconds amount of time,
before allowing the thread to proceed.

### [lock]

> "What do we want? Now! When do we want it? Fewer race conditions!"

The above joke is an example of the consequences of a race condition. Race conditions can be avoided,
by synchronizing access to each object, that are shared between multiple threads. This is accomplish with
the **[lock]** Active Event, which can sunchronize access to some shared resource across multiple threads.

Sometimes you have some shared object, or resource, which you cannot have multiple threads access at the
same time. For such cases, you can use a **[lock]** invocation, to make sure only one thread at the time
is able to access your shared object. An example is given below.

```hyperlambda
p5.io.file.save:~/foo.txt
  src:initial value
wait
  fork
    lock:my-file
      load-file:~/foo.txt
      save-file:~/foo.txt
        src:"{0}\r\n{1}"
          :x:/@load-file/*?value
          :first thread
  fork
    lock:my-file
      load-file:~/foo.txt
      save-file:~/foo.txt
        src:"{0}\r\n{1}"
          :x:/@load-file/*?value
          :second thread
  fork
    lock:my-file
      load-file:~/foo.txt
      save-file:~/foo.txt
        src:"{0}\r\n{1}"
          :x:/@load-file/*?value
          :third thread
load-file:~/foo.txt
```

In the above example, we want to ensure synchronized access while loading and saving our file. Without this,
we could in theory, run the risk of that all threads starts at the same time, loads our file, and overwrites the
file. This would mean that only the last thread saving our file, would get to put its changes into the file,
since it was loading the file before any of the previous threads was able to save their changes. This would imply
the first two threads did their changes, saving their version of our file, but the last thread overwrites these
changes, since it had loaded the file, at a point in time, when none of the other threads had gotten to save
its changes.

To illustrate the problem, and more easily grasp it - I simply adore this little joke,
a friend of mine told me once; _"What do we want? Now! When do we want it? Fewer race conditions!"_

Notice, the above "foo.txt" file, will have the 3 different threads, modify it in an undetermined order.
Meaning, you might just as well have the third thread add its changes, then the first thread, before finally
the second thread. Ending up with something like the following.

```hyperlambda
initial value
third thread
first thread
second thread
```

However, all threads will get to add their modifications - Which would not necessarily be the case without our lock.

Every time you execute the above lambda, you might have different results, since there is no way to determine
which threads starts and ends its job, before the next one gets going. It basically depends upon which thread
invokes its **[lock]** invocation first- But thanx to the above lock invocation, you do have a guarantee of
that all threads will have their changes applied, and stored into the shared "foo.txt" file.

### Example usage

Threads are notoriously difficult to use correctly, and as a general rule, you should be extremely careful
when using them, and preferably avoid them altogether if you can. Multiple threads also carries some overhead,
when switching your CPU's context. However, sometimes, having a multi-threaded solution, simply yields too
much benefit for you, to avoid implementing threading in your app. Some examples are given below.

### Creating multiple HTTP requests

Sometimes you have a list of HTTP requests, where you wish to download the files, found at each location. If
you were to do this sequentially, this would mean that you for every single request, had to wait until your
request was finished, before starting your next request. If you instead created your network logic, such that
each request was initiated from a different thread, then they would all be downloaded simultaneously, and you
wouldn't have to wait for one to finish downloading, before you start the next one. Which could prove orders
of magnitudes faster, than sequentially downloading each document - Depending upon how much overhead each of
your server endpoints have in handling your requests.

Imagine the following code.

```hyperlambda
p5.http.get:"http://google.com"
p5.http.get:"http://digg.com"
p5.http.get:"http://facebook.com"
p5.http.get:"http://reddit.com"
p5.http.get:"http://twitter.com"
```

On my system, this takes about 10 seconds to evaluate using localhost as my P5 server. If I instead created
these requests in parallel, in different threads, the difference would be very easy to notice.

```hyperlambda
wait
  fork
    p5.http.get:"http://google.com"
  fork
    p5.http.get:"http://digg.com"
  fork
    p5.http.get:"http://facebook.com"
  fork
    p5.http.get:"http://reddit.com"
  fork
    p5.http.get:"http://twitter.com"
```

The above code, takes about 3 seconds on my system to evaluate. Basically, 3-4 times as fast. This is because
I don't have to wait for one server to finish my request, before I start my request towards the next server.

### Thread safety in Phosphorus Five

With threading support in P5, the obvious question becomes; _"Is Phosphorus Five thread safe?"_

The short answer is; _"YES!!"_

Things like for instance p5.data, is 100% thread safe, and you never risk race conditions while using it.
Other parts, such as p5.io, is not (necessarily) 100% thread safe, and if multiple processes/threads are
modifying the same file, you might get race conditions. If you have multiple processes updating the same
files in your system when using p5.io, then it is up to you to make sure you don't get in trouble. This
can be accomplished by **[lock]**'ing access to the code that modifies your shared files.

Other parts again, that might not necessary be thread safe, are the Active Events, where I use underlaying
.Net technology, such as the **[p5.web.application.set]**, which is using the ASP.NET
`HttpContext.Current.Application` object. Whether or not these events are thread safe, depends upon the
underlaying .Net classes themselves. If you don't know, you should consult the MSDN website, and see what
the different classes implements, in regards to thread safety.

Most other parts, should be, as a general rule, 100% thread safe - Simply because they do not share any
common objects in any ways. If you're in doubt, realise that most immutable objects, are by default thread
safe, as long as they do not share some common object of some sort.
