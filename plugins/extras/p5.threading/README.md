Threading support in Phosphorus Five
===============

This project contains the thread supporting Active Events for Phosphorus Five. These Active Events, are plugins to the core "non-programming language",
and allows you to create threads, wait for multiple threads, lock access to shared objects, and most other things, expected from a modern 
multi-threaded toolkit.

Notice that no GUI operations can be performed on any other thread, except the main "GUI thread". If you spawn multiple threads, you'll have to use
for instance a *[wait]* invocation, to allow the GUI thread to modify the GUI based on the results of some worker thread's results.

## [fork], creating a new worker thread

To spawn a new worker thread, you would do something like this.

```
fork
  set:x:?value
    src:foo
```



Notice, if you evaluate the above piece of p5.lambda, in for instance the "system42/executor", it will not show you an updated *[set]* node. The reason
is, because what your thread is actually doing its work in regards to, is a _COPY_ of your original lambda block. So although the thread is actually
evaluating the above *[set]* invocation, the results of its evaluation, is never returned to you. First of all, because the GUI thread is highly
likely finished long before the spawned thread even gets to start. Secondly, to avoid race conditions and similar problems, no thread returns its 
modified p5.lambda, unless you use a *[wait]* block.

An invocation to *[fork]*, besides from the above mentioned problems, works similar to an *[eval]* invocation. Except (of course), it returns nothing
to the caller. You can even have an expression, leading to multiple lambda blocks, and spawn of one thread for each lambda block, with one invocation.
The following would spawn of _TWO_ threads.

```
_exe1
  set:x:?value
    src:foo
_exe2
  set:x:?value
    src:bar
fork:x:/../*/~_exe
```

## [wait], waiting for threads to finish.

If you use a *[wait]* invocation, wrapping the above *[fork]* invocation though, then the GUI thread will wait for all threads beneath the *[wait]*
to finish, and update its *[fork]* lambda blocks, with the result, after evaluation. Example given below.

```
wait
  fork
    set:x:?value
      src:foo
```

Contrary to our first example, not showing any results, the above will yield the following result.

```
wait
  fork
    set:foo
      src:foo
```

You can also use a *[wait]* block, wrapped around a *[fork]* invocation, having an expression leading to multiple evaluation objects, like we did above.
Example is given below.

```
_exe1
  set:x:?value
    src:foo
_exe2
  set:x:?value
    src:bar
wait
  fork:x:/../*/~_exe
```

As with our first *[wait]* example, the results of our last example, will update the *[_exe1]* and *[_exe2]* nodes, due to the single *[wait]*
invocation, wrapping our single *[fork]* invocation, leading to our lambda waiting for both threads to finish their work, before it proceeds, at which
point the evaluated result node, will replace the original nodes we had in *[_exe1]* and *[_exe2]*.

The result of the above evaluation hence becomes.

```
_exe1
  set:foo
    src:foo
_exe2
  set:bar
    src:bar
wait
  fork:x:/../*/~_exe
```

Wait cannot have any other types of children except *[fork]* nodes.

### Waiting for a specified amount of time

The *[wait]* Active Event, optionally takes an integer value, which is the number of milliseconds, before it stops waiting. To see this in action,
we can create two threads, where they both use the *[sleep]* Active Event, in such a way, that only one of them gets to finish, before our *[wait]*
block stops waiting.

```
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

The above p5.lambda, would first of all take .5 seconds to evaluate, since our *[wait]* block sets this as an argument for how long to wait. Secondly,
our last *[fork]* invocation, would not get to finish its job, before our *[wait]* block no longer wants to wait for it to finish. Hence, only the
results of our first *[fork]* will get to finish its work, and get to return its results back to us. Leading us to the following result.

```
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

Notice the seconds *[set]* invocation still having its original expression value, while our first *[set]* gets to return its results.

The *[sleep]* Active Event, "sleeps" the current thread, for a specified milliseconds amount of time, before allowing the thread to proceed.

## [lock], What do we want? Now! When? Fewer race conditions!

Sometimes you have some shared object, or resource, which you cannot have multiple threads access at the same time. For such cases, you can
use a *[lock]* invocation, to make sure only one thread at the time is able to access your shared object. An example is given below.

```
save-file:~/foo.txt
  src:initial value
wait
  fork
    lock:my-file
      load-file:~/foo.txt
      save-file:~/foo.txt
        src:"{0}\r\n{1}"
          :x:/..lock/*/load-file/*?value
          :first thread
  fork
    lock:my-file
      load-file:~/foo.txt
      save-file:~/foo.txt
        src:"{0}\r\n{1}"
          :x:/..lock/*/load-file/*?value
          :second thread
  fork
    lock:my-file
      load-file:~/foo.txt
      save-file:~/foo.txt
        src:"{0}\r\n{1}"
          :x:/..lock/*/load-file/*?value
          :third thread
load-file:~/foo.txt
```

In the above example, we want to ensure "atomic" access while loading and saving our file. Without this, we could in theory, run the risk of that
all threads starts at the same time, loads our file, and overwrites the file. This would mean that only the last thread saving our file, would
get to put its changes into the file, since it was loading the file before any of the previous threads was able to save their changes, then
the first two threads did their changes, saving their version of our file, but the last thread overwrites these changes, since it had loaded the 
file, at a point in time, when none of the other threads had gotten to save its changes.

To illustrate the problem, I simply adore this little joke, a friend of mine told me once; _"What do we want? Now! When? Fewer race conditions!"_ ;)


