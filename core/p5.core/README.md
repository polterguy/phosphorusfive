p5.ajax core, the main Active Event implementation
========

This project contains the main Active Event design patter implementation.
This design pattern allows you to completely eliminate *ALL* dependencies
between your different projects, creating an event based communication protocol,
based upon Active Event names, instead of hardlinked type references.

If you pass around only "POD data objects", this allows you to completely
aliminate all dependencies between your different components, that creates
your project, facilitating for an extremely loosely coupled structure of your
web apps.

## Example usage

Include a reference to p5.core in your project, and make sure you dynamically
load the DLLL containing the below class somehow. For instance, by using the 
"Loader.Instance.LoadAssembly" method from p5.core, loading up your DLL dynamically.

```csharp
using p5.core;

/* ... some class ... */

[ActiveEvent (Name = "foo")]
protected static void foo_method (ApplicationContext literal, ActiveEventArgs e)
{
    /* Do stuff with e.Args here.
       Notice that e.Args contains a reference to a Node, which
       allows you to pass in and return any number and/or types 
       of arguments you wish */
    e.Args.Value = 5 + 5;
}

/* ... end of some class ... */
```

With Active Events, you don't invoke the method directly, but rather indirectly,
through its *"Name"* property declared in your *"ActiveEvent"* attribute. This
means that you never have dependencies between the caller and the implementor of
your functions/methods/Active Events. This allows you to dynamically easily 
replace any functionality in your system, with other modules and pieces of 
functionality, without needing the caller and invoker to know anything about 
each other. Instead of replacing OOP, p5.core replaces the very way you invoke
functions.

To raise an Active Event from C#, you use the *"Raise"* method on your 
ApplicationContext object, passing in a Node, which will become the arguments 
passed into your Active Event.

```csharp
ApplicationContext ctx = Loader.Instance.CreateApplicationContext ();
Node node = new Node ();
ctx.Raise ("foo", node);

/*
 * At this point, the node.Value should contain the integer value of "10".
 */
```

Notice, with the above logic, there is no hardlinked references between your assembly
implementing the *[foo]* Active Event, and the assembly invoking this Active Event. Neither
are there any types from neither of the assemblies dependent upon the other assembly.

So all "hard linking" from one of your assemblies, to the other, completely vanishes, allowing
you to build applications more like you would assemble "LEGO bricks", than the traditional
way of creating a plugin architecture.

This design pattern is at the core of Hyperlisp and p5.lambda, which is the "programming
language" of Phosphorus Five.

The way it internally works, is by creating a hashtable or dictionary of dynamically linked
methods, allowig you to invoke these methods through the Active Event "name", instead of
directly through a hard linked reference. Then during application startup, this
dictionary is created, by traversing all types having methods with the "ActiveEvent" attribute.

You can also create instance Active Event handlers, at which point you will have to "register"
your objects as Active Event listeners through the "RegisterListeningObject" method on your
ApplicationContext instance.



