Phosphorus Five, significantly reducing the complexity of the art of creating web apps
===============

Phosphorus Five is a collection of libraries, for developing complex and rich Ajax centric web apps.
Its "Hello World" can be executed by starting your debugger (making sure you've turned _OFF_ "Browser Link"
in Visual Studio), open up the "apps/CMS", choose to create a "New page ..." - Choose *[lambda]* page, 
and paste in the following code.

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
have to change your browser settings for popups being allowed on localhost.

The above code, is called "Hyperlisp", and is simply a key/value/children tree-structure, allowing for you
to declare something which P5 refers to as "p5.lambda", which is the basis for an "execution tree", that is
a Turing complete way of "declaring" your apps, through a (very) rich "non-programming model".

We say "non-programming", because really, there is no "programming language" in P5, only a bunch of loosely
coupled Active Events, that happens to, in their combined results, create a Turing complete execution
environment.

In fact, if you wish, you can "declare" your execution trees, by using XML or JSON. Although we recommend
using Hyperlisp, due to its much more condens syntax, and lack of "overhead" compared to XML and JSON.

## 3 basic innovations

Phosphorus Five consists of three basic innovations.

* A "managed" Ajax library, which allows you to create apps almost as you would in for instance ASP.NET Web Forms
* A design pattern called "Active Events", which allows you to dynamically link together plugins, _without_ dependencies
* Hyperlisp, facilitating for "p5.lambda", which is an "evolutionary execution engine", being the obvious results of the Active Events design pattern

It is entirely created in C#, and should work perfectly with both Visual Studio, Xamarin and MonoDevelop.
It creates 100% conforming HTML5 markup, and uses many of the "industry standards" behind its hood, such as the Bootsstrap CSS framework.

The three USPs mentioned above though, creates a unique model for development, which allows you to combine your existing C# skills,
creating a "plugin environment", where you can assemble your apps, almost as if they were made out of "clay" or LEGO. This is in stark
contrast to the traditional way of assembling apps together, using interfaces for plugins, which requires a much higher degree of
dependencies between your app's different components.

This just so happens to facilitate for an environment, where you can reuse your code, to a much higher extent, than what you're used
to being able to do in most other "Frameworks".

At a fundamental level, it might be argued that Active Events, which is really the core of the brilliance of P5, replaces the way
you invoke "methods" and "functions". Although, you can still keep on leveraging your existing C# knowledge and OOP knowledge, this
design pattern has huge implications, allowing you to loosely couple your logic together, in a way that far superseeds what you're 
probably used to from before.

In fact, the above "Hello World" example, is simply an invocation to an Active Event, who's name is *[create-literal-widget]*, which
happens to take a set of arguments, that allows you to create an Ajax control on your page, which once clicked, changes its own "innerHTML",
through the *[innerValue]* property.

To understand its abilities, realize that also the *[controls]* parts of the "CMS", which is a complete Visual IDE, with RAD development
qualities, is entirely created using "Hyperlisp" and "p5.lambda". To see its powers, choose to for instance edit the pre-existing page 
called "Email Harvester - Rater" and try to click around on the design surface, and watch how the surface changes, and allows you
to build your apps, and a "RAD environment".

Of course, creating apps in a "RAD environment" is _optional_, and you can completely ditch the Visual IDE if you wish, and develop your 
apps 100% completely in p5.lambda, or even C# if you wish for that matter. What you will discover though, is that most of the problems
you need to implement, before starting on your "domain problems", are already implemented for you, out of the box in P5.

## Features

P5 contains a whole range of features, which are all optional to use, and can be "cherry picked" from when creating your apps. Below is
a non-exhaustive list of these features.

* p5.ajax, an Ajax library for ASP.NET that builds on top of "Web Forms"
* p5.core, the main Active Event design pattern implementation that allows you to raise and handle Active Events
* p5.exp, an expression language, that allows you to select sub-sections of tree-structures (graph objects) through a syntax similar to "XPath"
* lambda.exe, a console program that allows you to execute Hyperlisp and p5.lambda, directly from the console, without having a front-end web based GUI
* p5.webapp, a simple web app that is the "host" of your programs when developing your web apps
* p5.data, an extremely fast memory based, yet persistent database, which through expressions allows you to store, retrieve, modify and delete tree-structures (p5.lambda nodes)
* p5.html, allowing you to transform between HTML and p5.lambda (Hyperlisp)
* p5.mail, built on the marvelous MimeKit, allowing you to send and retreieve email, using POP3 and SMTP protocols
* p5.mime, allowing you to parse and create MIME messages, including PGP encrypted messages
* p5.net, allowing you to create HTTP requests entirely as you wish, to for instance invoke web services, or download files over your network
* p5.threading, allowing you to spawn of threads, and serialize access to shared resources, etc
* p5.web, allowing you to entirely create your UX in an extremely simple way, declaring it through Hyperlisp, and modifying it using p5.lambda
* p5.events, allowing you to dynamically create Active Events, using Hyperlisp and p5.lambda
* p5.hyperlisp, allowing you to parse Hyperlisp and create a "Node" structure out of it (p5.lambda/graph object)
* p5.io, allowing you to perform most tasks from the System.IO namespace of .Net, such as saving files, loading files, traversing directories, etc
* p5.lambda, which is the Turing Complete execution platform, or "non-programmming language based" execution engine of P5, if you wish
* p5.math, math operations or Active Events, allowing you to do most tasks related to math operations, such as +, -, /, * etc.
* p5.strings, giving you the most common string operations
* p5.types, containing the type support for Hyperlisp, which is extendible, allowing you to create your own types if you wish

Plus even more ...

The above list might seem overwhelming, especially when you couple it with Hyperlisp (a new concept), p5 expressions (a new concept) and 
p5.lambda (a new concept) - But really, these are only optional plugins, which you do not have to use, unless you wish. If you want to, you could
create your web app, entirely in ASP.NET and C#, and add up for instance only p5.ajax, to have a Web Forms based Ajax control library, and nothing else.
Then you could start incorporating Active Events in your project, when you feel for it, for then to add one plugin at the time, as you feel for it.
Slowly building up knowledge about how P5 works.

But of course, the most benefit will be aquired, if you go "all in", and entirely develop your apps in Hyperlisp and p5.lambda, using p5.web,
and p5.ajax, combined with all other plugins of P5. Even possibly also using the CMS, which is a part of the "front-end" parts, called "System42".

This would in theory allow you to develop your apps,at least to some extent, from within your browser, using the Hyperlisp/p5.lambda CodeMirror
editor, which is included in the "CMS".

For the record; Really, it's not "CMS", more in fact an IDE, and even a Visual IDE, but in lack of a better word, I keep on referring to it as a "CMS".

## Documentation

To see the documentation for the different projects, open up the folder for whatever project's dox you wish to read, and check out the embedded README.md
file. The dox for the "core" can be found in the projects folders inside of the "core" folder, while the dox for the different plugins, can be found
within the different project folders inside of the "plugin" folders.

There are also many YouTube videos, showing of some aspect about P5 at my YouTube channel.

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
