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

lambda.exe also have an immediate mode, which allows you to type in Hyperlambda as you go, into the console, 
and have it evaluated when you are finished. See the help instructions for the program, for details about how to do this.

The _"lambda.exe"_ is actually pathetically simple in its implementation, and illustrates how you could utilize Active Events, and
Active Event plugins, in your own projects, starting entirely from scratch. See the [Program.cs file](Program.cs) if you're interested in 
how this is done.
