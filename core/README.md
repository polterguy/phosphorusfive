Phosphorus Five _"core"_ components
===============

Here you can find the core parts of Phosphorus Five. There are five different projects in this folder;

* [lambda.exe](/core/lambda.exe/) - Allows you to execute Hyperlambda in a console/terminal window.
* [p5.ajax](/core/p5.ajax/) - The Ajax library for ASP.NET.
* [p5.core](/core/p5.core/) - The Active Event design pattern implementation.
* [p5.exp](/core/p5.exp/) - The expression engine.
* [p5.webapp](/core/p5.webapp/) - The main _"application pool"_ for your web app/web site.

When playing with P5, you should set _"p5.webapp"_ as your startup project.

Normally, when you play with Phosphorus Five, you don't need to think about the parts that are inside of this folder much.

## p5.webapp

The _"p5.webapp"_, is an almost entirely empty website. This is your _"application pool"_ when developing P5 web apps, 
and should always be set as your startup project in Visual Studio, Xamarin or MonoDevelop. It contains no logic at all. All the CLR components
are dynamically loaded up during startup. If you create your own components and plugins in C#, you need to edit your web.config file, to include
a reference to your component.

Notice, this web app, raises an Active Event called *[p5.web.load-ui]*, which has the responsibility of loading the UI somehow. To see an example
of how you could implemenet this Active Event in Hyperlambda, please check out the _"application-startup.hl"_ file inside of the _"p5.webapp"_ folder.

## p5.core

_"p5.core"_ contains the Active Event design pattern. If you wish to create your own Active Events, this is the only project you'll need to
reference. It allows you to raise Active Events and create Active Event handlers yourself. When you use it in combination with _"p5.webapp"_,
then all your plugins are loaded automatically according to the configuration settings found in your web.config file of p5.webapp.

This project contains the main axiom, which everything else evolves around; _Active Events_.

## p5.exp

In _"p5.exp"_ you can find the expression engine for P5. This is what allows you to reference nodes inside your lambda objects,
with the `:x:/@_foo?value` syntax. If you create your own custom Active Events, and you want them to be able to consider expressions, you'll
need to reference this project, in addition to _"p5.core"_.

## p5.ajax

In _"p5.ajax"_ you can find the Ajax widgets/controls, which all Ajax functionality is built upon in P5. It is created such that it only
really contains 1 widget (3 actually, but they're all simply variations over the base widget class), which allows you to decide which element 
and attributes you want to use for your widgets. This allows you to gain 100% perfect control over how your HTML is rendered to the client, 
while automatically taking care of the bitty gritty stuff, such as Ajax serialization, file inclusions, etc.

Every single Ajax widget in P5, is somehow built, by using these widgets as its _"building blocks"_.

This is an extremely extendibe Ajax library, allowing you to create any extension Ajax widget you wish, by combining multiple widgets together, 
and build your extension widgets incrementally on top of each other.

## lambda.exe

The _"lambda.exe"_ project, is a simple console program, that allows you to execute Hyperlambda from the console. It features the ability
to evaluate Hyperlambda files, passing in parameters - In addition to immediate mode, allowing you to type in Hyperlambda, directly into the console.
