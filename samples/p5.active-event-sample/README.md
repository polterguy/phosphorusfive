Active Event consumer C# example
===============

This project shows you how to consume or "raise" an Active Event from C#. It will dynamically load up the "p5.active-event-sample-plugin" assembly,
and consume Active Events inside of that project, through the `ApplicationContext.Raise` method.

The Active Event design pattern, allows you to loosely couple modules together, without creating any dependencies between them in any ways.

Constructs demonstrated in this project are;

* `Loader.Instance.LoadAssembly` to dynamically load an assembly
* `Loader.Instance.CreateApplicationContext` to create an "application context"
* `ApplicationContext.Raise` to raise an Active Event

The project illustrates how you could create a program, that consumes plugins, where the plugin's internal implementation is completely hidden away
from the consumer of the plugin.

To run the app, make sure you set it as your startup project, and start your debugger. The app is a console program.



