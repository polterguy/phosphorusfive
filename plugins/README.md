Plugins for Phosphorus Five
===============

Here you can find the plugins for Phosphorus Five. Although, really, _everything_ is a plugin in P5,
there are some main plugins, which are probably useful for most projects. The most important one
being [/plugins/p5.lambda/](p5.lambda), which contains the main _"keywords"_ in Hyperlambda.

Below is a list of all the plugins in the system as of today. Each plugin is documented in its own folder.

* [/plugins/p5.config/](p5.config) - Configuration settings
* [/plugins/p5.data/](p5.data) - A memory based,super fast, database
* [/plugins/p5.events/](p5.events) - Create dynamic Active Events
* [/plugins/p5.hyperlambda/](p5.hyperlambda) - Hyperlambda parser
* [/plugins/p5.io/](p5.io) - File input and output
* [/plugins/p5.lambda/](p5.lambda) - Keywords in Hyperlambda
* [/plugins/p5.math/](p5.math) - Math events
* [/plugins/p5.strings/](p5.strings) - String events
* [/plugins/p5.types/](p5.types) - Hyperlambda types
* [/plugins/p5.web/](p5.web) - Hyperlambda core Ajax Widgets

In addition, there are several [more plugins in the extras](/plugins/extras/) folder.

To create your own plugins for P5, is actually ridiculously easy, due to the Active Event design pattern. If you
wish, you can easily extend Hyperlambda, creating your own _"programming keywords"_, or more specifically Active Events,
which you can easily consume, either from Hyperlambda, or from C#.

To see an example of how to create your own Active Event plugin in C#, check out the C# example 
called [p5.active-event-sample-plugin](/samples/p5.active-event-sample-plugin/)

If you included the above sample plugin project in your website, you can raise the *[foo]* sample Active Event from any web page,
or Hyperlambda file, etc, in your system.

To see an example of how to consume (raise) Active Events from C# code, check out the C# example 
called [p5.active-event-sample](/samples/p5.active-event-sample/)



