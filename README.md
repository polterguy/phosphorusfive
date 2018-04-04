Phosphorus Five - A Web Operating System
===============

Phosphorus Five is a .Net based Web Operating System and a full stack Web Application Development Framework, for consuming and
creating rich and secure web apps. It contains an entirely unique programming language called _"Hyperlambda"_,
which allows you to orchestrate your apps together, almost as if they were made out of LEGO bricks.

<p align="center">
<a href="https://www.youtube.com/watch?v=BLll2Wx0yFo">
<img alt="A one minute introduction video about Phosphorus Five" title="A one minute introduction video about Phosphorus Five" src="https://phosphorusfive.files.wordpress.com/2018/03/screenshot-youtube-infomercial.png" />
</a>
</p>

Out of the box, Phosphorus Five contains the following components.

* __Hyper IDE__ - A web based IDE, with support for 100+ programming languages
* __Camphora Five__ - A CRUD app generator allowing you to create rich CRUD apps in seconds
* __Magic Menu__ - A global _"navbar"_, with support for speech recognition and speech synthesis

... plus more!

## Installation

You can install Phosphorus Five on a production Ubuntu/Linux server with an automated script, taking care of all dependencies. Or
you can download its code version, and play around with it locally, on your Windows, Mac, or Linux machine. If you choose the latter, you will
have to [make sure you have MySQL Server installed somehow](https://dev.mysql.com/downloads/mysql/) on your computer.
In addition, you need [Visual Studio/Xamarin](https://www.visualstudio.com/vs/community/) or [Mono Develop](https://www.monodevelop.com/) to
use the source code version.

* [Download and install Phosphorus Five here](https://github.com/polterguy/phosphorusfive/releases) - Both binary release and source code

**Notice - Source Code** - If you download the source version, make sure you edit the `/core/p5.webapp/web.config` file, such that it contains the correct
connection string for your MySQL installation. This normally implies simply adding your password to the existing connection string.
Phosphorus Five will run without a valid MySQL database connection string - However, some of its apps will not function at all,
or at their peak performance.

**Notice - Binaries** - The automatic Linux script has only been tested on Ubuntu Server version 16.04.4, but might also work on other versions. This script
will also _sigificantly_ increase the security of your box, in addition to patching your box, updating it, and making sure it's using the latest
stable versions of all software it installs - Such as for instance Mono version 5.10. The script expects a _"vanilla"_ Linux Ubuntu Server, and will
_remove_ any existing websites you have configured for your Apache folder.

**Notice - Source Code version on Windows** - If you use the source code version on Windows in Visual Studio, _make sure you turn off "browser sync"
in Visual Studio_.

## MSDN Magazine articles about Phosphorus Five

1. [Active Events: One design pattern instead of a dozen](https://msdn.microsoft.com/en-us/magazine/mt795187)
2. [Make C# more dynamic with Hyperlambda](https://msdn.microsoft.com/en-us/magazine/mt809119)
3. [Could Managed AJAX Put Your Web Apps in the Fast Lane](https://msdn.microsoft.com/en-us/magazine/mt826343)

## License

Phosphorus Five is free and open source software, and licensed under the terms
of the Gnu Public License, version 3, in addition to that a proprietary enabling license is available for a fee.

* [Download and install Phosphorus Five here](https://github.com/polterguy/phosphorusfive/releases)

## FAQ

* [FAQ](FAQ.md) - With a little dash of humor ... ;)
