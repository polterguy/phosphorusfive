Console Hyperlisp executor
========

Allows you to execute Hyperlisp directly from a terminal window. If you start this
program without any command line arguments, its help instructions will be printed on
the main std output (Console window) for you.

To evaluate a single Hyperlisp file, pass it in as an "-f" argument. To for instance
evaluate a file called "hello.hl", run the following command in a terminal.

```
lambda.exe -f hello.hl
```

If you are on Linux or Mac OS X, then prepend the above command with "mono".

```
mono lambda.exe -f hello.hl
```

The lambda.exe program contains two special Active Events itself, which are;

* p5.console.write-line - Writes a line to the std output.
* p5.console.read-line - Reads a line of input from the std input, and returns as "value" of node.

This console program also have an "immediate mode", which allows you to type
in Hyperlisp as you go, into the console, and have it executed when you are finished.
See the help instructions for the program for details about how to do this.


