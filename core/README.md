Phosphorus Five _"core"_ components
===============

Here you can find the core parts of Phosphorus Five. There are five different projects in this folder;

* [lambda.exe](/core/lambda.exe/) - Allows you to execute Hyperlambda in a console/terminal window.
* [p5.ajax](/core/p5.ajax/) - The Ajax library for ASP.NET.
* [p5.core](/core/p5.core/) - The Active Event design pattern implementation.
* [p5.exp](/core/p5.exp/) - The expression engine.
* [p5.webapp](/core/p5.webapp/) - The main _"application pool"_ for your web app/web site.

When playing with P5, you should set _"p5.webapp"_ as your startup project.

Normally, when you play with Phosphorus Five, you don't need to think about the parts that are inside of this folder, unless you wish to extend
System42 with your own apps or components.

## p5.webapp, the only website project you need

The _"p5.webapp"_, is actually just an almost entirely empty website. By default, it comes with [System42](/core/p5.webapp/system42/) 
pre-installed, which is a GUI front-end/back-end for your system, containing among other things, a fully fledged CMS, or publishing 
system. Everything you see when you start P5, can be found in System42.

If you wish, you can entirely delete System42, to start out with a completely _"empty website"_, and create your own back-end/front-end
entirely as you see fit.

## p5.core, the Active Event design patternt implementation

_"p5.core"_ contains the Active Event design pattern. If you wish to create your own Active Events, this is the only project you'll need to
reference. It allows you to raise Active Events and create Active Event handlers yourself. When you use it in combination with _"p5.webapp"_,
then all your plugins are loaded automatically according to the configuration settings found in your web.config file of p5.webapp.

This project contains the main axiom, which everything else evolves around; _Active Events_.

## p5.exp, lambda expressions

In _"p5.exp"_, the expression engine for lambda expressions can be found. This is what allows you to reference nodes inside your lambda objects,
with the funny `:x:/foo?value` syntax. If you create your own custom Active Events, and you want them to be able to consider expressions, you'll
need to reference this project, in addition to _"p5.core"_.

## p5.ajax, the last Ajax library you'll ever need

In _"p5.ajax"_ you can find the Ajax widgets/controls, which all Ajax functionality is built upon in P5. It is created such that it only
really contains 1 widget (3 actually, but they're just variations over the base widget), which allows you to decide which element and attributes
you want to use for your widgets. This allows you to gain 100% perfect control over how your HTML is rendered to the client, while automatically
taking care of the bitty gritty stuff, such as Ajax serialization, file inclusions, etc.

Every single Ajax widget in P5, is somehow built, by using these widgets as its _"building blocks"_.

## lambda.exe, a console program for executing Hyperlambda

The _"lambda.exe"_ project, is a simple console program, that allows you to execute Hyperlambda from the console.
