Phosphorus Five documentation
===============

Phosphorus Five is a collection of libraries, for developing complex and rich Ajax centric web apps, among other things.
Its _"Hello World"_, can be evaluated by making sure you have the "p5.webapp" as your startup project, start your debugger, 
click the _"Apps/CMS"_ menu item, create a new page by clicking the _"+"_ - Choose *[lambda]* page, and paste in the following code.

```
p5.web.widgets.create-literal
  element:button
  parent:content
  class:btn btn-default
  innerValue:Click me!
  onclick
    p5.web.widgets.property.set:x:/../*/_event?value
      innerValue:Hello World!
```

If you get weird errors when running P5, then stop your debugger, _TURN OFF_ "Browser Link" in Visual Studio, and restart your debugger.

Save your page, and click _"View page"_. If you get a browser notification saying _"a popup was blocked"_, you might
have to change your browser settings for popups being allowed on localhost. If you do not have Visual Studio, P5
works just as well on Xamarin (Mac OS X), and MonoDevelop (Linux).

Notice, the main, and extensive documentation for Phosphorus Five can be found [here](https://github.com/polterguy/phosphorusfive-dox).

## What is that code ...?

The above code, is called _"Hyperlambda"_, and is simply a key/value/children tree-structure, allowing for you
to declare something, that P5 refers to as _"lambda"_ or _"Hyperlambda"_. Lambda is the foundation for an execution tree, or graph object,
that is a Turing complete opportunity to declare your apps, through a _"non-programming model"_.

I say _"non-programming"_, because really, there is no programming language in P5. Only a bunch of loosely
coupled Active Events, that happens to, in their combined result, create a Turing complete execution
engine, allowing you to orchestrate your components together, as if they were _"LEGO bricks"_.

In fact, if you wish, you could in theory declare your execution trees, by using XML or JSON. Although I recommend
using Hyperlambda, due to its much more condens syntax, and lack of overhead.

All this, while retaining your ability to create C#/VB/F# code, exactly as you're used to from ASP.NET.

## 3 basic innovations

Phosphorus Five consists of three basic innovations.

* Managed Ajax
* Active Events
* Hyperlambda

The Ajax library is created on top of ASP.NET's Web Forms, allowing you to use them the same way you would create a web forms website.
Simply inject them declaratively into your markup, and change their properties and attributes in your codebehind. We say _"managed"_, because
it takes care of all state, Ajax serialization, and dynamic JavaScript inclusion automatically. In fact, when you use the Ajax library, you can
create your web apps, the same way you would normally create a desktop application.

Active Events allows you to loosely couple your modules together, without having any dependencies between them. Active Events is the _"heart"_ of
Phosphorus Five, allowing for the rich plugin nature in P5. You can easily create your own Active Events, either in Hyperlambda, or in C# if you wish.

Hyperlambda, and p5.lambda, is the natural bi-product of Active Events; A Turing complete execution engine, for orchestrating your apps 
together, as shown above in the Hello World example.

## Perfect encapsulation and polymorphism

The 3 USPs mentioned above, facilitates for a development model, which allows you to combine your existing C# skills,
creating plugins, where you can assemble your apps, in a loosely coupled architecture. This is in stark
contrast to the traditional way of _"carving out"_ apps, using interfaces for plugins, which often creates a much higher degree of
dependencies between your app's different components.

In fact, the above Hello World example, is simply an invocation to an Active Event, who's name is *[p5.web.widgets.create-literal]*, which
happens to take a set of arguments, that allows you to create an Ajax control on your page, which once clicked, changes 
its *[innerValue]* property to; _"Hello World!"_.

The paradox is, that due to neither using OOP nor inheritance, in any ways, Hyperlambda facilitates for perfect encapsulation, and polymorphism,
without even as much as a trace of inheritance, OOP or objects.

You can easily create your own Active Events, incrementally building on top of your previously created Active Events, that creates much richer
Ajax widgets than what's shown above. Below is a piece of Hyperlambda, that creates an Ajax TreeView, which allows you to browse your folders on disc.
This [extension widget](/core/p5.webapp/system42/components/common-widgets/tree/) is entirely created in Hyperlambda, but you can also create Active 
Events in C#.

```
p5.web.widgets.create-container
  parent:content
  widgets
    sys42.widgets.tree
      _crawl:true
      _items
        root:/
      .on-get-items
        p5.io.folder.list-folders:x:/../*/_item-id?value
        for-each:x:/-/*?name
          p5.io.folder.list-folders:x:/./*/_dp?value
          p5.string.split:x:/./*/_dp?value
            =:/
          add:x:/../*/return/*
            src:@"{0}:{1}"
              :x:/..for-each/*/p5.string.split/0/-?name
              :x:/..for-each/*/_dp?value
          if:x:/./*/p5.io.folder.list-folders/*
            not
            add:x:/../*/return/*/_items/0/-
              src
                _class:tree-leaf
        return
          _items
```

Below is a screenshot of how the above piece of Hyperlambda might look like, if you paste it into a CMS/lambda page.

![alt tag](/core/p5.webapp/system42/components/common-widgets/tree/screenshots/ajax-treeview-widget-example-screenshot.png)

Notice, the above Ajax TreeView is even search engine friendly, and allows for spiders to crawl your items. Try seeing this in action, by right-clicking
any node in it, and choose _"Open in new tab"_.

This _"incremental"_ development model, allows you to reuse almost everything you create in your future projects, to an extent difficult to understand,
before you have experienced it.

## Documentation

To see the documentation for the different projects, open up the folder for whatever project's dox you wish to read, and check out the 
embedded README.md file. The dox for the core, can be found in the projects folders inside of the _"core"_ folder, while the dox for the 
different plugins, can be found within the different project folders inside of the _"plugin"_ folders.

* [Documentation for the core](/core/) - The Ajax library, the website and the Active Event design pattern implementation.
* [Documentation for the plugins](/plugins/) - All _"plugins"_, such as the Hyperlambda keywords, creation of Ajax widgets, etc.

There also exists [a book](https://github.com/polterguy/phosphorusfive-dox), which is probably the easiest way to start learning
P5. This is a relatively *"interactive book"*, containing links to tutorial videos, and probably the easiest way for you to
start learning P5.

## About System42

System42 is really just a folder within the _"/core/p5.webapp/"_ folder in Phosphorus Five. It serves 3 purposes. First of all it 
is a use-case for the C# Active Events in Phosphorus Five, showing the capability of P5. Secondly, it is a relatively complete CMS 
by itself, and could probably be used standalone as a CMS for your website. Last, but not least, it contains a lot of components, which
you can use in your own systems, such as the above Ajax TreeView for instance.

[Browse the Hyperlambda files in System42](/core/p5.webapp/system42/) to get a feel for Hyperlambda, and see some more example code.

## Removing features/plugins

If you create your own plugins, and/or want to remove existing plugins, you do this by modifying your _"web.config"_, 
in the [p5.website folder](/core/p5.webapp/). This will change the DLLs dynamically loaded during startup of your website, 
and allow you to invoke your own custom made Active Events, possibly created in for instance C#, VB.NET, F#, or Boo for that matter.

## C# samples

For those only interested in using e.g. the Ajax library, and/or the Active Event implementation, there are many examples of this in 
the [samples folder](/samples/).

[C# examples] (/samples/)

## License

Phosphorus Five is free and open source software, and licensed under the terms
of the Gnu Public License, version 3, in addition to that commercially license are available for a fee. Read more about
our Quid Pro Quo license terms at our website at [Gaiasoul](http://gaiasoul.com).

