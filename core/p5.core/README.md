The Active Event design pattern implementation
========

This project contains the main Active Event design pattern implementation.
This design pattern, allows you to completely eliminate _all_ dependencies
between your projects, creating an event based communication model,
based upon Active Events, instead of hardlinked references and interfaces.

If you pass around only "POD data objects", this allows you to completely
eliminate all dependencies between your different components, facilitating 
for an extremely loosely coupled structure of your apps.

Notice, although Phosphorus Five primarily is for "web applications", the 
Active Event design pattern, and hence this project, can easily be incorporated
into any other types of projects.

## Example usage

Create two projects, one "plugin project" and one "main app project". Then create 
a reference to "p5.core" in both of them. Afterwards, create the Active Event 
below inside a class in your "plugin DLL project", compile the project, and 
simply copy the DLL into the "bin" folder of your "main app" project.

Make sure you load up your "plugin DLL", using `Loader.Instance.LoadAssembly`,
into your "main app", in some method in your "main application".

After you've done so, simply create an `ApplicationContext`, using for instance
the `Loader.Instance.CreateApplicationContext` method, and invoke your Active Event,
using the `ApplicationContext.Raise` method to invoke your "plugin's" Active Event (method)

Code for your "plugin project".

```csharp
using p5.core;

/* ... some class declaration ... */

[ActiveEvent (Name = "foo")]
protected static void foo_method (ApplicationContext context, ActiveEventArgs e)
{
    /* Do stuff with e.Args here.
     * Notice that e.Args contains a reference to a Node, which
     * allows you to pass in and return any number and/or types 
     * of arguments you wish.
	 */
    e.Args.Value = "Hello " + e.Args.Value;
}

/* ... end of some class declaration ... */
```

Code to load up your "plugin" DLL dynamically from your "main app".

```
Loader.Instance.LoadAssembly ("name-of-your-dll");
```

Code to create an `ApplicationContext`, and raise an Active Event. Normally,
you will keep your ApplicationContext object around, for the lifespan of your
app, and reuse it, every time you want to raise an event.

```csharp
ApplicationContext ctx = Loader.Instance.CreateApplicationContext ();
Node node = new Node ("", "Thomas");
ctx.Raise ("foo", node);

/*
 * At this point, the node.Value should contain the integer value of "Hello Thomas".
 * If you declared the Active Event handler we previously looked at,
 * in some class, who's assembly you loaded up dynamically using the Loader.
 */
```

The context argument passed into your Active Event handlers, is the application
context, from which the Active Event was raised from within. The ActiveEventArgs,
contains the name of the Active Event, and more importantly the `Node`, through 
its `e.Args` property, containing the arguments, passed back and forth, into your 
handler.

Whatever you change in your `e.Args` property of your `ActiveEventArgs` from within 
your Active Event handler, will be returned back to the caller by reference to
the caller.

Also, whatever you pass into your `e.Args` before you raise your Active Event, 
will be passed into the handler(s) for your Active Events.

## Parametrizing your Active Events

All arguments passed in and out of an Active Event, must be passed in through the "Node" class.
The Node class is a key/value/children graph object, or tree-structure, able to hold any objects 
you wish for it to hold. The "key" (or Name property) is a string, and the "value" (Value 
property) is an object reference. You can put in any objects you wish as the "Value" of your
Node instances. And every Node, can have any amount of children as you wish. Allowing you
to _really_ pass in any data you wish, and return any data you wish from your handlers.

As a general rule, you should avoid as much as possible, to pass back and forth complex objects,
and try to as much as possible, to stick with only the System types from .Net - Such as "int", 
"string" and so on. This is to make sure you avoid creating unnecessary dependencies between
your modules, by having to "hard link" in some project, which both the invoker and the caller 
becomes dependent upon.

## Multiple handlers for the same Active Event

When you invoke a method or function in for instance C#, then normally you would only invoke one
single method. For Active Events, this is not necessarily true. An Active Event invocation, might 
have anywhere from zero to an infinite number of handlers.

This means that you can have pieces of code, that "listens in" on the communication in your system,
to for instance dynamically inject logging, and similar types of functionality. This feature also
allows you to chain event handlers together, although in an undetermined order, having multiple
"methods" handle thee same Active Event.

Notice, that when you raise an Active Event, there also might be _zero_ handlers for it, meaning
that "nothing is ever done" when raising it. There would be no ways for you to know how many
handlers you have, unless you keep track of this yourself, in the returned Node, after invoking
your Active Events.

In such a regard, Active Events are more similar to plain C# events and event handlers, although the
implementation is very different. The consequences of dynamically loading plugins, instead
of having to know about them during compile time, is staggeringly interesting for the art of 
de-coupling and loosely binding your assemblies together.

## "Null" handlers handling _all_ events

If you create an Active Event handler in C#, which has "" as its name, then this event handler will be
invoked for every single Active Event raised in your system. Needless to say probably, but if you
have fairly complex code in these types of event handlers, your system's performance will be drastically
reduced, as a _whole_! However, some times this is highly useful, for doing things such as logging or profiling.

For instance, the "dynamic Active Events features" of the p5.events projects, is entirely created
using this logic. When invoked, it does a lookup into its registered "dynamic Active Events", to see if there
exists a (dynamic) handler for the specified event. If it does, it raises that dynamic event, as a 
piece of lambda object.

## Additional helper classes

The "Utilities" class contains some additional helper methods and/or classes, which helps
your out, when creating your Active Events. Among other things, it contains a "Convert" method,
which will use the internal conversion Active Events, often declared in p5.types, to convert
back and forth between object and string representation, vice versa. If you have an object,
and you wish to make sure it becomes a string, you should use this method.

In addition, it is a general design pattern, that most "get" Active Events should delete
their original arguments. This is easily achieved, using the class "Utilities.ArgsRemover"
class, which iimplements IDisposable, which cleans up arguments from a node when being
disposed.

## More examples

If you check out the project called [p5.active-event-sample](/samples/p5.active-event-sample/), 
then it illustrates an example of a console program, that dynamically loads a plugin assembly, 
called [p5.active-event-sample-plugin](/samples/p5.active-event-sample-plugin/), which handles 
some Active Events, through some Active Event handlers - Both static event handlers, and member 
event handlers.

To run the example, make sure you set the "p5.active-event-sample" as your startup project.

This examples shows dynamic loading of plugins, creating static event handlers, in addition to
instance event handlers, and registering and unregistering your instance event listeners.

It also illustrates how you can have multiple handlers for the same Active Event.

Notice, for convenience, the main app project in these examples, holds a reference to the
plugin project. This is only to avoid having to copy the plugin DLL into your app's bin
folder, and have no other significancy besides from that.

## Even more examples

The entirety of everything inside of the [/plugins/](plugins) folder in Phosphorus Five, including the 
p5.lambda "programming language itself", is entirely created as Active Events.

If you wish to see more examples of how to use Active Events, I encourage you to look at the 
source for P5 itself, and more specifically the "plugins" folder, since it is the primary example 
of how to use Active Events in itself.



