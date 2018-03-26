Phosphorus Five - A Web Operating System
===============

Phosphorus Five is a Web Operating System and a full stack Web Application Development Framework, for consuming and developing rich and highly 
interactive and secure web apps. It contains an entirely unique programming language called _"Hyperlambda"_, which 
allows you to orchestrate your apps together, almost as if they were made out of LEGO bricks.

![alt screenshot](https://phosphorusfive.files.wordpress.com/2018/03/github-screenshot-desktop.png)

Out of the box, Phosphorus Five contains the following components.

* __Hyper IDE__ - A web based IDE, with support for 100+ programming languages
* __Camphora Five__ - A CRUD app generator allowing you to create rich CRUD apps in seconds
* __Magic Menu__ - A global _"navbar"_, with support for speech recognition and speech synthesis

... plus more!

## Installation

You can install it on a production Ubuntu/Linux server with an automated script, taking care of all dependencies, with 3 simple commands. Or
you can download its code version, and play around with it locally, on your Windows, Mac, or Linux machine. If you choose the latter, you will
have to [make sure you have MySQL Server installed somehow](https://dev.mysql.com/downloads/mysql/) on your computer, since Phosphorus Five
is dependent upon MySQL to function correctly. If you download the source version, make sure you edit the `/core/p5.webapp/web.config` file,
such that it contains the correct connection string for your MySQL installation (Hint; Provide your MySQL password in it).

* [Download and install Phosphorus Five here](https://github.com/polterguy/phosphorusfive/releases)

## 3 basic innovations

Phosphorus Five consists of three basic innovations.

* __Managed Ajax__
* __Active Events__
* __Hyperlambda__

The Ajax library is created on top of ASP.NET's Web Forms, allowing you to use them the same way you would create a web forms website.
Simply inject them declaratively into your markup, and change their properties and attributes in your codebehind. We say _"managed"_, because
it takes care of all state, Ajax serialization, and dynamic JavaScript inclusion automatically.

Active Events allows you to loosely couple your modules together, without having any dependencies between them. Active Events is the _"heart"_ of
Phosphorus Five, allowing for the rich plugin nature in P5. You can easily create your own Active Events, either in Hyperlambda, or in C# if you wish.

Hyperlambda is the natural bi-product of Active Events; A Turing complete execution engine, for orchestrating your apps 
together. By combining Active Events together with Managed Ajax and Hyperlambda - Your apps truly _"become alive"_, and creating rich web apps,
becomes much easier than what you would expect.

Combined, the above USPs arguably makes Phosphorus Five a _"5th Generation Programming Language"_.

## MSDN Magazine articles

P5 have been published three times in Microsoft's MSDN Magazine. Read the articles below written by yours truly.

1. [Active Events: One design pattern instead of a dozen](https://msdn.microsoft.com/en-us/magazine/mt795187)
2. [Make C# more dynamic with Hyperlambda](https://msdn.microsoft.com/en-us/magazine/mt809119)
3. [Could Managed AJAX Put Your Web Apps in the Fast Lane](https://msdn.microsoft.com/en-us/magazine/mt826343)

If you wish to read these articles, you might benefit from reading them sequentially, to make sure you understand Active Events, 
before you dive into Hyperlambda. However, all of the above articles are highly technical in nature, and intended for system
developers. You might still have good use of Phosphorus Five without being a system developer.

## Documentation

Phosphorus Five is _extremely well documented_, and to a large extent literate. It contains a context sensitive and interactive
help system as an integral part of itself, describing literally every single aspect of the system. Regardless of where you
are in the system, simply click the "?" button, and you can read in depth topics about any aspect of the system.

## License

Phosphorus Five is free and open source software, and licensed under the terms
of the Gnu Public License, version 3, in addition to that commercially license are available for a fee.