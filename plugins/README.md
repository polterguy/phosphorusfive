Plugins for Phosphorus Five
===============

Here you can find the different "plugins" for Phosphorus Five. Although, really, _everything_ is a plugin in P5,
there are some "core" plugins, which are probably useful for most projects. The most important one
being [/plugins/p5.lambda/](p5.lambda), which is the "non-programming language" in P5, containing the Turing complete
execution engine in P5.

Below is a list of all the plugins in the system as of today. Each plugin is documented in its own folder.

* [/plugins/p5.config/](p5.config), allowing you to access app.config or web.config settings through p5.lambda
* [/plugins/p5.data/](p5.data), the internal memory based database in P5
* [/plugins/p5.events/](p5.events), allowing you to create dynamic Active Events in p5.lambda
* [/plugins/p5.hyperlambda/](p5.hyperlambda), the Hyperlambda parser and creator
* [/plugins/p5.io/](p5.io), giving you file/folder Active Events, to create, modify, delete, etc, files and folders in your system
* [/plugins/p5.lambda/](p5.lambda), the core "non-programming language" instruction set, such as *[add]* and *[set]*
* [/plugins/p5.math/](p5.math), containing all the math operators and events, such as *[+]*, *[-]*, *[*]* and *[/]*, which yes, are Active Events
* [/plugins/p5.strings/](p5.strings), containing the events necessary to manipulate strings
* [/plugins/p5.types/](p5.types), containing the default type conversion Active Events of P5
* [/plugins/p5.web/](p5.web), allows you to create Ajax web widgets, and access the session. Besides from p5.webapp, actually the only project in P5 which is "web specific"

In addition, there are several more plugins in the [extras](/plugins/extras/) folder.

To create your own plugins for P5, is actually ridiculously easy, due to the Active Event design pattern. If you
wish, you can easily extend p5.lambda, creating your own "programming instructions", or more specifically Active Events,
which you can access easily, either from Hyperlambda and p5.lambda, from C# or both.

To see an example of how to create your own Active Event plugin in C#, check out the C# example 
called [p5.active-event-sample-plugin](/samples/p5.active-event-sample-plugin/)

If you included the above sample plugin project in your website, you can raise the *[foo]* sample Active Event from any web page,
or Hyperlambda file, etc, in your system.

To see an example of how to consume (raise) Active Events from C# code, check out the C# example 
called [p5.active-event-sample](/samples/p5.active-event-sample/)



