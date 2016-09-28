Plugins for Phosphorus Five
===============

Here you can find the different "plugins" for Phosphorus Five. Although, really, _everything_ is a plugin in P5.
There are some "core" plugins, which are probably useful for all types of projects. The most important one
being [/plugins/p5.lambda/](p5.lambda), which is the "non-programming language" in P5, containing the Turing complete
execution engine in P5.

Below is a list of all the plugins in the system as of today.

* [/plugins/p5.events/](p5.events), allowing yout to create Active Events in p5.lambda
* [/plugins/p5.hyperlisp/](p5.hyperlisp), the Hyperlisp parser and creator
* [/plugins/p5.io/](p5.io), giving you file/folder Active Events, to create, modify, delete, etc files and folders
* [/plugins/p5.lambda/](p5.lambda), the core "non-programming language instruction set", such as *[add]* and *[set]*
* [/plugins/p5.math/](p5.math), containing all the math operators and events, such as *[+]*, *[-]*, *[*]* etc
* [/plugins/p5.strings/](p5.strings), containing the events necessary to manipulate strings
* [/plugins/p5.types/](p5.types), containing the default type conversion Active Events of P5

In addition, there are several more plugins accessible in the [/extras/](/plugins/extras/) folder.

To create your own plugins for P5, is actually ridiculously easy, due to the Active Event design pattern. If you
wish, you can easily "extend" p5.lambda, creating your own "programming instructions", or more specifically Active Events,
which you can access easily, either from Hyperlisp and p5.lambda, or from C#.

To see an example of how to create your own Active Event plugin in C#, check out the C# example 
called [p5.active-event-sample-plugin](/samples/p5.active-event-sample-plugin/)

To see an example of how to consume (raise) Active Events from C# code, check out the C# example 
called [p5.active-event-sample](/samples/p5.active-event-sample/)



