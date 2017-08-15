Active Event handler example in C#
===============

This project illustrates how you could create a plugin, which you dynamically consume from your app, using Active Events.

The project illustrates the following concepts;

* `ActiveEvent` attribute to declare methods as Active Events
* Both static Active Event handlers and instance handlers
* `ApplicationContext.RegisterListeningObject` to register objects as listening for Active Events
* `ApplicationContext.UnregisterListeningObject` to unregister objects as Active Event handlers

From an abstract, the project demonstrates how you could create a plugin assembly, that handles Active Events raised from
your app, 100% chemically cleansed of dependencies.





