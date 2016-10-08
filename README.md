Phosphorus Five, simplifying the creation of web apps
===============

Phosphorus Five is a collection of libraries, for developing complex and rich Ajax centric web apps, among other things.
Its _"Hello World"_ can be evaluated by making sure you have the "p5.webapp" as your startup project, start your debugger, 
click the "Apps/CMS" menu item, create a "New page ..." - Choose *[lambda]* page, and paste in the following code.

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

If you get weird errors when debugging, thebn stop your debugger, _TURN OFF_ "Browser Link" in Visual Studio, and restart debugging.

Save your page, and click "Preview". If you get a browser notification saying "a popup was blocked", you might
have to change your browser settings for popups being allowed on localhost. If you do not have Visual Studio, P5
works just as well on both Xamarin (Mac OS X) and MonoDevelop (Linux).

Tip!
A nice place to test out constructs shown in the documentation files for P5, is the menu item "Apps/Executor",
which allows you to execute Hyperlambda and p5.lambda, directly from within your browser. Most examples here
assumes you're using the "Executor" application to evaluate your code.

## What is that code ...?

The above code, is called "Hyperlambda", and is simply a key/value/children tree-structure, allowing for you
to declare something which P5 refers to as "p5.lambda". p5.lambda is the basis for an "execution tree", that is
a Turing complete opportunity to "declare" your apps, through a (very rich) "non-programming model".

I say "non-programming", because really, there is no programming language in P5. Only a bunch of loosely
coupled Active Events, that happens to, in their combined result, create a Turing complete execution
engine, allowing you to orchestrate your components together as if they were _"LEGO bricks"_.

In fact, if you wish, you could in theory declare your execution trees, by using XML or JSON. Although I recommend
using Hyperlambda, due to its much more condens syntax, and lack of overhead, compared to XML and JSON.

All this, while retaining your ability to create C#/VB/F# code, exactly as you're used to from ASP.NET.

## 3 basic innovations

Phosphorus Five consists of three basic innovations.

* A managed Ajax library
* A design pattern called Active Events
* Hyperlambda or p5.lambda

The Ajax library is created on top of ASP.NET's web controls, allowing you to use them the same way you would create a web forms website.
Simply inject them declaratively into your markup, and change their properties and attributes in your codebehind.

Active Events allows you to loosely couple your modules together, without having any dependencies between them.

Hyperlambda, and p5.lambda, is the natural bi-product of Active Events; A Turing complete execution engine, for orchestrating your apps 
together, almost as if they were LEGO bricks.

The three USPs mentioned above, facilitates for a development model, which allows you to combine your existing C# skills,
creating plugins, where you can assemble your apps, in a loosely coupled architecture. This is in stark
contrast to the traditional way of "carving out" apps, using interfaces for plugins, which often creates a much higher degree of
dependencies between your app's different components.

In fact, the above Hello World example, is simply an invocation to an Active Event, who's name is *[create-literal-widget]*, which
happens to take a set of arguments, that allows you to create an Ajax control on your page, which once clicked, changes 
its *[innerValue]* property to; "Hello World!".

## Documentation

To see the documentation for the different projects, open up the folder for whatever project's dox you wish to read, and check out the 
embedded README.md file. The dox for the core, can be found in the projects folders inside of the "core" folder, while the dox for the 
different plugins, can be found within the different project folders inside of the "plugin" folders.

* [Documentation for the core](/core/)
* [Documentation for the plugins](/plugins/)

## About System42

System42 is really just a folder within the "/core/p5.webapp/" folder in Phosphorus Five. It serves two purposes. First of all it 
is a use-case for the C# Active Events in Phosphorus Five, showing the capability of P5. Secondly, it is a relatively complete CMS 
by itself, and could probably be used as the basis for a CMS for your website.

If you do not wish to use System42, simply delete it, and modify your web.config file, to invoke a different ".p5.webapp.application-startup-file" 
during startup. This would give you a completely clean install, with zero overhead, allowing you to entirely create your own 
front-end/back-end as you see fit.

[Browse the Hyperlambda files in System42](/core/p5.webapp/system42/) to get a feel for Hyperlambda, and see some more example code.

## Removing features/plugins

If you create your own plugins, and/or want to remove existing plugins, you do this by modifying your "web.config", in its "phosphorus" 
section. This will change the DLLs dynamically loaded during startup of your website, and allow you to invoke your own custom made 
Active Events, possibly created in for instance C#, VB.NET, F#, or "Boo" for that matter.

## C# samples

For those only interested in using e.g. the Ajax library, and/or the Active Event implementation, there can be found some examples in 
the "samples" folder.

[C# examples] (/samples/)

## License

Phosphorus Five is free and open source software, and licensed under the terms
of the Affero GPL license, in addition to a commercial license. Read more about
our Quid Pro Quo license terms at our website at [Gaiasoul](http://gaiasoul.com).

