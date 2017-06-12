Phosphorus Five documentation
===============

<img align="right" src="p5.png">

Phosphorus Five is a collection of libraries for developing complex and rich Ajax centric web apps.
Its _"Hello World"_, can be found in the [application-startup.hl](/core/p5.webapp/application-startup.hl) Hyperlambda file.

Notice, the primary documentation for Phosphorus Five can be found [here](https://github.com/polterguy/phosphorusfive-dox).
This README is just intended to give you a teaser of some of its features. An example of some Hyperlambda is given below.

```
create-widget:foo
  element:button
  innerValue:Click me!
  onclick
    set-widget-property:foo
      innerValue:I was clicked!
```

## Hyperlambda

The above code, is called _"Hyperlambda"_, and is simply a key/value/children tree-structure, allowing for you
to declare something, that P5 refers to as _"lambda"_ or _"Hyperlambda"_. Lambda is the foundation for an execution tree, or graph object,
that is a Turing complete opportunity to declare your apps, through a _"non-programming model"_.

I say _"non-programming"_, because really, there is no programming language in P5. Only a bunch of loosely
coupled Active Events, that happens to, in their combined result, create a Turing complete execution
engine, allowing you to orchestrate your components together, as if they were _"LEGO bricks"_.

In fact, if you wish, you could in theory declare your execution trees, by using XML or JSON. Although I recommend
using Hyperlambda, due to its much more condens syntax, and lack of overhead.

All this, while retaining your ability to create C#/VB/F# code, exactly as you're used to from before.

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

The paradox is, that due to neither using OOP nor inheritance, in any ways, Hyperlambda facilitates for perfect encapsulation, and polymorphism,
without even as much as a trace of classic inheritance, OOP or types.

## C# samples

For those only interested in using e.g. the Ajax library, and/or the Active Event implementation, there are some examples of this in 
the [samples folder](/samples/).

[C# examples] (/samples/)

## License

Phosphorus Five is free and open source software, and licensed under the terms
of the Gnu Public License, version 3, in addition to that commercially license are available for a fee. Read more about
our Quid Pro Quo license terms at our website at [Gaiasoul](https://gaiasoul.com/license/).

## Hire me

Need more training or personal assistance in regards to Phosphorus Five, don't hesitate to pass me an email.

Thomas Hansen; thomas@gaiasoul.com
