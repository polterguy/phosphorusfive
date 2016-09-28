Phosphorus Five, simplifying the creation of web apps
===============

Phosphorus Five is a collection of libraries, for developing complex and rich Ajax centric web apps, among other things.
Its "Hello World" can be executed by making sure you have the "p5.webapp" as your startup project, start your debugger 
(making sure you've turned _OFF_ "Browser Link" in Visual Studio), open up the "apps/cms" in the menu at the top, choose 
to create a "New page ..." - Choose *[lambda]* page, and paste in the following code.

```
create-literal-widget
  element:h3
  parent:content
  class:col-xs-12
  innerValue:Click me!
  onclick
    set-widget-property:x:/../*/_event?value
      innerValue:Hello World!
```

Save your page, and click "Preview". If you get a browser notification saying "a popup was blocked", you might
have to change your browser settings for popups being allowed on localhost. If you do not have Visual Studio, P5
works just as well on both Xamarin (Mac OS X) and MonoDevelop (Linux).

Tip!
A nice place to test out constructs shown in the documentation files for P5, is the menu item "apps/executor",
which allows you to execute Hyperlisp and p5.lambda directly from within your browser. Most examples here
assumes you're using the "executor" application to evaluate your code.

The above code, is called "Hyperlisp", and is simply a key/value/children tree-structure, allowing for you
to declare something which P5 refers to as "p5.lambda". p5.lambda is the basis for an "execution tree", that is
a Turing complete opportinuty to "declare your apps", through a (very) rich "non-programming model".

We say "non-programming", because really, there is no "programming language" in P5. Only a bunch of loosely
coupled Active Events, that happens to, in their combined version, create a Turing complete execution
environment.

In fact, if you wish, you can "declare" your execution trees, by using XML or JSON. Although we recommend
using Hyperlisp, due to its much more condens syntax, and lack of "overhead" compared to XML and JSON.

This trait of P5, allows you to "orchestrate" your apps, by combining your parts together, more like how a conductor 
is orchestrating his musicians. It hence becomes an extremely high abstraction, allowing you to focus more on the 
big picture, without having to care about the implementation details (unless you want to)

All this, while retaining your ability to create C# code, exactly as you're used to from ASP.NET.

## 3 basic innovations

Phosphorus Five consists of three basic innovations.

* A managed Ajax library
* A design pattern called Active Events
* Hyperlisp or p5.lambda

The Ajax library is entirely created on top of ASP.NET's web controls, allowing you to use them the same way you would create a web forms website.
Simply inject them declaratively into your markup, and set their properties and attributes in your codebehind.

Active Events allows you to loosely couple your modules together, without having any dependencies between them.

Hyperlisp or p5.lambda is the natural bi-product of Active Events; A Turing complete execution engine, for orchestrating your apps together almost
as if they were LEGO bricks.

P5 is entirely created in C#, and should work perfectly with Visual Studio, Xamarin and MonoDevelop.
It creates 100% conforming HTML5 markup, and comes with many of the "industry best practices" included, such as Bootsstrap CSS and jQuery.

The three USPs mentioned above though, creates a unique model for development, which allows you to combine your existing C# skills,
creating a "plugin environment", where you can assemble your apps, almost as if they were made out of LEGO. This is in stark
contrast to the traditional way of "carving" apps together, using interfaces for plugins, which requires a much higher degree of
dependencies between your app's different components.

Phosphorus Five facilitates for an extremely Agile environment. Where any piece can in theory, be interchanged with any other piece you wish.

This just so happens to facilitate for an environment, where you can reuse your code, to a much higher extent, than what you're used
to being able to do in most other "frameworks".

At a fundamental level, it might be argued that Active Events, which is really the core of the brilliance of P5, replaces the way
you invoke "methods" and "functions". Although, you can still keep on leveraging your existing C# knowledge and OOP knowledge, this
design pattern has huge implications, allowing you to loosely couple your logic together, in a way that far superseeds what you're 
probably used to from before.

In fact, the above "Hello World" example, is simply an invocation to an Active Event, who's name is *[create-literal-widget]*, which
happens to take a set of arguments, that allows you to create an Ajax control on your page, which once clicked, changes its "innerHTML",
through the *[innerValue]* property.

## Documentation

To see the documentation for the different projects, open up the folder for whatever project's dox you wish to read, and check out the embedded README.md
file. The dox for the "core" can be found in the projects folders inside of the "core" folder, while the dox for the different plugins, can be found
within the different project folders inside of the "plugin" folders.

* [Documentation for the core](/core/)
* [Documentation for the plugins](/plugins/)

There are also many YouTube videos, showing of some aspect about P5 at [my YouTube channel] (https://www.youtube.com/channel/UCmZvIkxA08v9O6oi3NtwUjA/videos)

## About System42

System42 is really just a folder within the "/core/p5.webapp/" folder in Phosphorus Five. It serves two purposes. First of all it is a use-case
of the core C# parts of Phosphorus Five, showing the capability of P5. Secondly, it is a "front-end" for your system, solving most back-end tasks
for your system, such as browsing your system's file structure, creating a CMS, handling PGP keys, handling users, etc, etc, etc.

If you do not wish to use System42, simply delete it entirely, and modify your web.config file, to invoke a different "startup file" during
startup. This would give you a completely "clean" install, with zero overhead, allowing you to entirely create your own "front-end"/"back-end"
as you see fit.

However, before you do, feel free to see the power of Phosphorus Five, which to a large extent is show-cased in System42. System42 is in fact, entirely
built in Hyperlisp and p5.lambda, and shows what kind of powers you get, when going "all in", and doing everything in Hyperlisp/p5.lambda - Using
all the plugins of the system.

## Removing features/plugins

If you create your own plugins, and/or want to remove existing plugins, you do this by modifying your "web.config", where it says "plugin assemblies".
This will change the DLLs dynamically loaded during startup of your website, and allow you to invoke your own custom made Active Events, possibly 
created in for instance C#, VB.NET, F#, or "Boo" for that matter.

## C# samples

Although I recommend people to use the entire "framework", for those only interested in using e.g. the Ajax library, and/or the Active Event design
pattern implementation, there can be found some examples of this in the "samples" folder.

## License

Phosphorus Five is free and open source software, and licensed under the terms
of the MIT license, which basically allows you to do anything you wish with it, 
except from removing the copyright notice, and sue me for problems arising from
the use of the software.

The MIT License (MIT)

Copyright (c) 2014-2016 Thomas Hansen, phosphorusfive@gmail.com

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
