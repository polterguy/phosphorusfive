Phosphorus Five _"core"_ components
===============

Here you can find the core parts of Phosphorus Five. There are five different projects in this folder;

* [lambda.exe](/core/lambda.exe/) - Allows you to execute Hyperlambda in a console/terminal window.
* [p5.ajax](/core/p5.ajax/) - The Ajax library for ASP.NET.
* [p5.core](/core/p5.core/) - The Active Event design pattern implementation.
* [p5.exp](/core/p5.exp/) - The expression parser.
* [p5.webapp](/core/p5.webapp/) - The main _"application pool"_ for your web app/web site.

When playing with P5, you should set _"p5.webapp"_ as your startup project.

Normally, when you play with Phosphorus Five, you don't need to think about the parts that are inside of this folder.

The _"p5.webapp"_, is actually just an almost entirely empty website. By default, it comes with [System42](/core/p5.webapp/system42/) 
pre-installed, which is a GUI front-end/back-end for your system, containing among other things, a fully fledged CMS, or publishing 
system. Everything you see when you start P5, can be found in System42, which taps into the Active Events in the [plugins folder](/plugins/).

In fact, when you create web apps and/or websites, then the only projects you'd normally need to include a reference to, are these;

* p5.ajax, if you wish to have Ajax functionality in your project
* p5.core, if you wish to consume Active Events in your project
* p5.exp, if you wish to use expressions in your project

In addition, you could also choose to use the p5.webapp, either as a starting ground for your own ASP.NET website project, or hosting
your entire application, adding up all your own functionality as plugins. However, all projects are actually optional, and you can use
any single part of Phosphorus Five, without including any other parts.



