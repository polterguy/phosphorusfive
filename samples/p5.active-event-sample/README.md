Active Event consumer C# example
===============

This project shows you how to consume or "raise" an Active Event from C#. It will dynamically load up the "p5.active-event-sample-plugin" assembly,
and consume Active Event handlers inside of that project through the ApplicationContext.Raise method.

The Active Event design pattern, allows you to loosely couple modules together, without creating any dependencies between them in any ways.

Constructs demonstrated in this project are;

* `Loader.Instance.LoadAssembly` to dynamically load an assembly
* `Loader.Instance.CreateApplicationContext` to create an "application context"
* `ApplicationContext.Raise` to raise an Active Event



