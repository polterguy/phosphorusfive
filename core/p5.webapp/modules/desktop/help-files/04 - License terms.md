## License

<img class="desktop-help-icon-image" src="/modules/desktop/media/logo.svg" />

Phosphorus Five and all related modules are distributed under the terms of the GPL version 3, as published
by the Free Software Foundation. However, to make a long story short, this implies that you are legally
obliged to release your own work, created on top of Phosphorus Five, as Free Software. If you don't want
to release your own work as Free Software and Open Source, there exists proprietary licensing options,
allowing you to create closed source modules and apps. However, due to the technical implementation
details of Phosphorus Five, this creates a couple of legal _"licensing anomalies"_, which you might be
interested in understanding.

#### With Managed Ajax the GPL becomes Affero GPL

Since Phosphorus Five, and more explicitly the _"Managed Ajax"_ parts distributes JavaScript to the client,
and this JavaScript is an integral part of the core framework itself - This implies that all who are visiting
a website and/or a webapp that you have created with Managed Ajax are in fact _"executing GPL code"_. Since
your own code is built around Managed Ajax, this results in that you are legally obliged to providing the
source code for your own derived works, to everyone who asks you for a copy - Unless you have a proprietary
license. So for all practical concerns, Phosphorus Five's choice of GPL, effectively results in becoming
an _"Affero GPL license"_.

**Notice**, this has been verified by licensing experts working for the Free Software Foundation!

#### With Active Events the GPL becomes LGPL

Another peculiarity, is that since the Active Events design pattern, creates an extremely loosely coupled
binding, between your code, and the Active Events that are implemented in C# - This results in that parts
of the GPL license is effectively _"invalidated"_, since your derived work, can easily function without
_"linking"_ to GPL code. This results in that you can consume a GPL library, in for instance C#, and
the effective result, becomes that this library you create in C#, can for all practical concerns be
consumed more or less like an LGPL library. Implying you only have to distribute the C# code that wraps
this GPL library, and not necessarily your entire solution's code. This is because of the loosely
binding between Active Event assemblies, does not create a _"derivated work"_, or in any ways _"links"_
between the different assemblies.

So basically, you can consume GPL CLR libraries and modules as if they were LGPL libraries.

#### Quid pro quo and dual licensing options

Phosphorus Five is distributed under the idea of _"Quid Pro Quo"_, which translates into
_"Something for Something"_. The idea is that either you create Free and Open Source Software yourself, or
you pay me money to help sustain further development and maintenance of the framework.
This implies that unless you want to create Free Software, you _have to obtain a proprietary license_.
Phosphorus Five will ask you if you want to purchase such a
_"commercial license"_ every now and then, in addition to informing your visitors about their rights. If
you purchase a commercial license, Phosphorus Five will automatically remove any links on your modules,
and stop asking you about purchasing a license.

Of course, the fix for the above, if you are uncertain about your legal status in these regards, is
to simply purchase a commercial license. This will also give you _"extended support"_, in addition
to access to a backup module that allows you to easily create backups of your system. It is also the
ethical and morally right thing to do, once you realise the amount of work I have put into the system!

### The license terms

The exact wording of the license can be seen [here](https://www.gnu.org/licenses/gpl-3.0.txt).
