Hyperlambda terminal/console executor
========

This project allows you to execute Hyperlambda directly from a terminal window. If you start this
program without any command line arguments, its help instructions will be printed on
the main std output (Console window) for you.

To evaluate a single Hyperlambda file, pass it in as an `-f` argument. To for instance
evaluate a file called _"hello.hl"_, execute the following command in a terminal.

```
lambda.exe -f hello.hl
```

If you are on Linux or Mac OS X, then prepend the above command with _"mono"_.

```
mono lambda.exe -f hello.hl
```

The lambda.exe program contains two special Active Events itself, which are;

* [p5.console.write-line] - Writes a line to the std output.
* [p5.console.read-line] - Reads a line of input from the std input, and returns it as _"value"_ of node.

lambda.exe also have an _"immediate mode"_, which allows you to type in Hyperlambda as you go, into the console, 
and have it evaluated by supplying en empty line. To start immediate mode on e.g. Linux, you could type in something such as the following.

```
mono lambda.exe -i
```

To load up additional plugins, before evaluating a Hyperlambda file, or entering immediate mode, you can pass in additional plugins you 
wish to load by supplying e.g. `-p plugins/my-plugin.dll`.

The _"lambda.exe"_ is actually pathetically simple in its implementation, and illustrates how you could utilize Active Events, and
Active Event plugins, in your own projects, starting entirely from scratch. See the [Program.cs file](Program.cs) if you're interested in 
how this is done.

In such a regard, it serves as an excellent starting ground, if you wish to include Hyperlambda support in your own projects, 
such as for instance having dynamic _"script"_ support in a game, or any other applications.
