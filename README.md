Phosphorus Five - A Web Operating System
===============

Phosphorus Five is a .Net based Web Operating System and a full stack Web Application Development Framework, for consuming and
creating rich and secure web apps. It contains an entirely unique programming language called _"Hyperlambda"_,
which allows you to orchestrate your apps together, almost as if they were made out of LEGO bricks.

![alt screenshot](https://phosphorusfive.files.wordpress.com/2018/03/github-screenshot-desktop.png)

Out of the box, Phosphorus Five contains the following components.

* __Hyper IDE__ - A web based IDE, with support for 100+ programming languages
* __Camphora Five__ - A CRUD app generator allowing you to create rich CRUD apps in seconds
* __Magic Menu__ - A global _"navbar"_, with support for speech recognition and speech synthesis

... plus more!

## Installation

You can install Phosphorus Five on a production Ubuntu/Linux server with an automated script, taking care of all dependencies. Or
you can download its code version, and play around with it locally, on your Windows, Mac, or Linux machine. If you choose the latter, you will
have to [make sure you have MySQL Server installed somehow](https://dev.mysql.com/downloads/mysql/) on your computer.
In addition, you need [Visual Studio](https://www.visualstudio.com/vs/community/) or [Mono Develop](https://www.monodevelop.com/) to
use the source code version.

* [Download and install Phosphorus Five here](https://github.com/polterguy/phosphorusfive/releases) - Both binary release and source code

**Notice** - If you download the source version, make sure you edit the `/core/p5.webapp/web.config` file, such that it contains the correct
connection string for your MySQL installation. This normally implies simply adding your password to the existing connection string.
Phosphorus Five will run even without a valid MySQL database connection string - However, some of its apps will not function at all,
or at their peak feature set.

**Important** - If you use the source code version on Windows in Visual Studio, __make sure you turn off "browser sync" in Visual Studio__.

## 3 basic innovations

Phosphorus Five contains 3 basic innovations.

* __Managed Ajax__
* __Active Events__
* __Hyperlambda__

The Ajax library is created on top of ASP.NET's Web Forms, allowing you to use them the same way you would create a web forms website.
Simply inject them declaratively into your markup, and change their properties and attributes in your codebehind.

Active Events allows you to loosely couple your modules together, without having any dependencies between them. Active Events is the _"heart"_ of
Phosphorus Five, allowing for the rich plugin nature in P5. You can easily create your own Active Events, either in Hyperlambda, or in C# if you wish.

Hyperlambda is the natural bi-product of Active Events; A Turing complete execution engine, for orchestrating your apps 
together. By combining Active Events together with Managed Ajax and Hyperlambda - Your apps truly _"become alive"_, and creating rich web apps
becomes surprisingly easy.

Combined, the above USPs arguably makes Phosphorus Five a _"5th Generation Programming Language"_.

## MSDN Magazine articles

Phosphorus Five has been published 3 times in Microsoft's MSDN Magazine. Read the articles below, written by yours truly.

1. [Active Events: One design pattern instead of a dozen](https://msdn.microsoft.com/en-us/magazine/mt795187)
2. [Make C# more dynamic with Hyperlambda](https://msdn.microsoft.com/en-us/magazine/mt809119)
3. [Could Managed AJAX Put Your Web Apps in the Fast Lane](https://msdn.microsoft.com/en-us/magazine/mt826343)

## Documentation

Phosphorus Five is _extremely well documented_. It contains a context sensitive and interactive
help system as an integral part of itself, describing all aspects of the system. Regardless of where you
are in the system, simply click the "?" button, and you can read in depth topics about the module you need help for.

## License

Phosphorus Five is free and open source software, and licensed under the terms
of the Gnu Public License, version 3, in addition to that a proprietary enabling license is available for a fee.

* [Download and install Phosphorus Five here](https://github.com/polterguy/phosphorusfive/releases)
