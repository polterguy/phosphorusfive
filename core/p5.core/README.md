The main Active Event implementation
========

This project contains the main Active Event design patter implementation.
This design pattern allows you to completely eliminate _all_ dependencies
between your different projects, creating an event based communication model,
based upon Active Event, instead of hardlinked references and interfaces.

If you pass around only "POD data objects", this allows you to completely
eliminate all dependencies between your different components, facilitating 
for an extremely loosely coupled structure of your web apps.

## Example usage

Include a reference to p5.core in your project, and make sure you dynamically
load the DLLL containing the below class somehow. For instance, by using the 
"Loader.Instance.LoadAssembly" method from p5.core, loading up your plugin 
DLLs dynamically.

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
    e.Args.Value = 5 + 5;
}

/* ... end of some class declaration ... */
```

Then in any other parts of your code, you can dynamically load up this plugin
assembly using.

```
Loader.Instance.LoadAssembly ("name-of-your-dll");
```

The context argument passed into your Active Event handlers, is the application
context, from which the Active Event was raised from within. The ActiveEventArgs,
contains the name of the Active Event, and more importantly the "Node", containing
the arguments, passed back and forth, into your handler. Whatever you change in your
"Args" property of your ActiveEventArgs from within your Active Event handler, will 
be returned back to the caller by reference.

Notice, if you use p5.core in combination with p5.webapp, then each request has 
its own application context. And the application context contains the user's
"Ticket", which defines what kind of things the request/user is authorized to
performing.

With Active Events, you don't invoke the method directly, but rather indirectly,
through its *"Name"* property declared in your *"ActiveEvent"* attribute. This
means that you never have dependencies between the caller, and the handler of
your Active Events. This allows you to dynamically easily replace any functionality 
in your system, with other modules and pieces of functionality, without needing 
the caller and handler, to know anything about each other. Instead of replacing 
OOP, p5.core replaces the very way you invoke functions.

Notice, Active Events can easily be used in combination with traditional
OO and method invocations. This means that you can still utilize your existing
OO and C# knowhow. However, it gives you an alternative path to implement 
"plugins" in your project, which you will probably find far superior to
the traditional way of using "interfaces" and such, for communication between
modules.

To understand just how powerful this design pattern is, realize that the
entirety of p5.lambda, the "programming language", is entirely implemented
using Active Events. This means that the "keywords" themselves, are in fact
nothing but "Active Event sinks", which you can invoke from any parts of
your system.

To raise an Active Event from C#, you use the *"Raise"* method on your 
ApplicationContext object, passing in a Node, which will become the arguments 
passed into your Active Event.

```csharp
ApplicationContext ctx = Loader.Instance.CreateApplicationContext ();
Node node = new Node ();
ctx.Raise ("foo", node);

/*
 * At this point, the node.Value should contain the integer value of "10".
 * If you declared the Active Event handler we previously looked at,
 * in some class, who's assembly you loaded up dynamically using the Loader.
 */
```

Notice, with the above logic, there is no hardlinked references between your assembly
implementing the *[foo]* Active Event, and the assembly invoking your "foo" Active Event. 
Neither are there any types from neither of the assemblies, dependent upon the other assembly.

So all "hard linking" from one of your assemblies, to the other, completely vanishes, allowing
you to build applications more like you would assemble "LEGO bricks", than the traditional
way of creating a plugin architecture.

The way it internally works, is by creating a hashtable, or dictionary, of dynamically linked
methods, using reflection, allowing you to invoke these methods through their Active Event "name", 
instead of directly through a hard linked object reference. Then this dictionary is partially
create during your "Loader.Instance.LoadAssembly" invocations, and partially during 
your "RegisterListeningObject" invocation. Which depends upon whether or not your Active
Event handler is a static method or a member method.

You can also create instance Active Event handlers, at which point you will have to "register"
your objects as Active Event listeners, through the "RegisterListeningObject" method on your
ApplicationContext. If you do, then your Active Events will be raised within the context
of the object you registered as an "event listener", and have access to all private member fields,
properties, and methods of your object. You can register as many instances of the same class
as you wish to be "instance listeners", but it is up to you yourselves, to make sure you 
"unregister" your instances.

Notice, if you register objects as "event listeners", you will have to "unregister" them, when
you no longer want them to listen for events, using the "UnregisterListeningObject" method on
your ApplicationContext. Otherwise, you will end up with "dangling objects", that the garbage 
collector won't be able to collect, and you will have these "dangling objects", handle any
Active Events they are listening to, when they are invoked.

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

If you check out the "plugins" projects in Phosphorus Five, you will see that apart from p5.core
and p5.exp, none of them have any references to any other projects, except the p5.webapp, which
only have the references to the projects it references, for simplicity during building and debugging,
since a reference here, will automatically copy the assemblies into the "bin" folder of your website.

However, the references to the assemblies in the web app (p5.webapp), is actually only there
for convenience, since it automatically copies your project into your web apps "bin" folder.
If we wanted to, we could completely remove or add any of these plugins, update our web.config,
to reflect which assemblies to load, and such completely modify our app's behaviour.

For the p5.webapp, the actual plugins you use, is actually defined in your "web.config" file,
which has a list of assemblies, which are dynamically loaded, using the "Loader" class of p5.core.

In theory, the only assembly you'll ever have to include a reference to, when using Active Events,
is the "p5.core" assembly. Still you could easily have your different modules, perfectly communicate
together, as if they were "hard linked" together. Needless to say, but this gives you an _extreme_
amount of flexibility, and literally _exterminates_ all of your dependencies, if done correctly.

Active Events gives you a flexibility and agility, I'd claim that no other plugin library would ever
be able to give you on this planet! A bold statement, but I dare you to prove me wrong!

## Dynamically loading up plugin assemblies

To dynamically load up an assembly which contains Active Event handlers, you would use the "Loader"
class, and its method(s) "LoadAssembly". This will load the specified assembly into your "AppDomain",
register all (static) Active Event handlers, and let you invoke Active Events handlers in it by raising 
them using ApplicationContext.Raise.

Notice, you must create your ApplicationContext _AFTER_ you have loaded your assemblies, since your
ApplicationContext is created according to which assemblies where dynamically loaded before it was 
created.

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

## More examples

If you check out the project called "p5.active-event-sample", then it illustrates an example of a 
console program, that dynamically loads a plugin assembly, called "p5.active-event-sample-plugin",
which handles some Active Events, through some Active Event handlers - Both static event handlers, 
and member event handlers.

To run the example, make sure you set the "p5.active-event-sample" as your startup project.

This examples shows dynamic loading of plugins, creating static event handlers, in addition to
instance event handlers, and registering and unregistering your instance event listeners.

It also illustrates how you can have multiple handlers for the same Active Event.

## Even more examples

The entirety of everything inside of the "plugins" folder in Phosphorus Five, including the 
p5.lambda "programming language itself", is entirely created as Active Events.

If you wish to see more examples of how to use Active Events, I encourage you to look at the 
source for P5 itself, and more specifically the "plugins" folder, since it is the primary example 
of how to use Active Events in itself.



