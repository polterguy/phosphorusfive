
## Desktop API I

The desktop contains a rich API, which allows you to perform all sorts of tasks, which are common
to all modules. This includes the responsibility of loading the help system, installation and
uninstallation of apps and modules, etc. Below you can find the documentation for all of these common
tasks.

### The help system

The help system can be consumed and displayed from anywhere you are in Phosphorus Five by invoking
the **[desktop.help.display]** Active Event. This event will load up the help files, which you
can interact with programmatically as you see fit. As the help system is loaded, it will invoke
the Active Event **[desktop.help.get-context]**, and if this invocation returns a path to a file,
this file will be displayed by default. You can also load the help system, and explicitly choosing
to load a default help file as the help system is loaded, by passing in a **[file]** argument -
Which will be treated as the default landing page when the help system is loaded. Below you can
find the list of lambda events that are at your disposal when the help system is loaded.

* __[desktop.help.is-visible]__ - Returns boolean _"true"_ if the help system is running
* __[desktop.help.close]__ - Closes the help system
* __[hyper-ide.help.go-previous]__ - Goes to the _"previous"_ help file
* __[hyper-ide.help.go-next]__ - Goes to the _"next"_ help file
* __[desktop.help.display-file]__ - Loads up the specified __[\_arg]__ help file

#### Help system internals

When you go back and forth in your help files, this implies sorting all help files from some folder
alphabetically, and moving to the file that are alphabetically before whatever file you're currently
displaying. This allows you to order your help files, by prepending an integer number in front of
your file in its filename, e.g. _"01 - Some help file.md"_. This ensures that you get to decide
which order your help files are displayed as you go back and forth.

The help system allows you to create your help files both as Markdown and as a Hyperlambda widget
hierarchy. This gives you 100% control over how your help files are possible to interact with, and
you can in fact if you want to, create a help _"page"_, which arguably becomes an application by
itself. And in fact, most of the _"index.hl"_ help files are created like this. Each folder you
have in your help files, are expected to have one such _"index.hl"_ file, which normally simply
lists the available files for your help folder, and allows the user to choose one specific file
from your folder. If you want to add to the help system, by creating your own help files that
are plugged into the main help system, you can start out by copying one of the existing _"index.hl"_
files, from for instance the _"/modules/hyper-ide/help-files/Hyper IDE/"_ folder, and modify it to
handle your folder. The folder name for your help files, will be used as the name of your help
section - Hence the last parts of the Hyper IDE folder which is called _"Hyper IDE"_.

As the main landing page for the help system is launched, it will raise all Active Events that
are named **[desktop.help-files.xxx]**, where _"xxx"_ is some piece of string you choose yourself,
and expect these events to return a path to some folder. These folders will be listed on the
main _"index.hl"_ file, and allows you to create your own help extension files.

If you launch the help system from within Hyper IDE, you will see that it features two additional
buttons - A _"pencil"_ button, that allows the user to edit the file directly, by clicking that
pencil icon - And a _"refresh"_ button, which allows you to reload the help file, once it has
been edited. This allows the end user to add his own comments and such, directly to the help
files themselves, and facilitates for a highly interactive help system.

#### Creating your own help files

If you want to create a help file as a widget hierarchy, then realize that what you're expected
to do, is to actually declare a **[widgets]** collection, which is appended dynamically into
the main widget of the help files. Below is an example of such a hierarchy.

```hyperlambda
/*
 * How a Hyperlambda help file might look like
 */
h2
  innerValue:Help about something
p
  innerValue:Some help concept
```

If you want to create your help files as Markdown files, which is probably more common, you have
a lot of additional helper constructs you can use, which are convenience features, allowing
you to write your help files in a highly interactive and intelligent manner. Among other things,
you can inject Hyperlambda snippets by creating a code block, with the language code of `hyperlambda-snippet`.
This will inject a button into your Hyperlambda code block, which once clicked, will evaluate the
Hyperlambda inside of your code block. Below is an example.

```hyperlambda-snippet
/*
 * Try clicking the button in the bottom right corner of this code block.
 */
micro.windows.info:Hello
```

If you view this specific help file, in e.g. Hyper IDE, you can click the _"pencil button"_ in
the help file widget, and see the code necessary to create such a code block. You can of course
also provide any CodeMirror mode in your code blocks that CodeMirror supports, to display
code in for instance C# or JavaScript instead. However, only Hyperlambda code blocks are possible
to evaluate, as our above code block demonstrates.

If you create your own help files, you can also embed images and videos, by simply
creating a paragraph with the URL to your YouTube video, and/or your image. Below is an example
of such a YouTube video. Yet again, to see how I embedded the video, feel free to edit this file
in Hyper IDE.

https://www.youtube.com/watch?v=9nAVSaVJZgU
