p5.events, dynamically creating Active Events
===============

In Phosphorus Five, you can either create static Active Events in C#, by tagging your C# methods with
the *ActiveEvent* attribute. You can also use any p5.lambda structure as an "anonymous function", or p5.lambda object.
In addition, you can also easily execute Hyperlisp files, the same way you'd invoke a function or method, passing in arguments,
and returning values, just like you would with a normal function.

However, there is also the possibility of dynamically creating globally accessible Active Events, through the *[create-event]*
Active Event. This Active Event, allows you to modify the underlaying Active Event dictionary directly, by either modifying existing, 
adding new, or deleting existing entries within it, almost like you would do with any other dictionary/hash-table object.
Imagine the following code.

```
create-event:foo
  eval-x:x:/+
  return:Hello there {0}
    :x:/../*/_arg?value
foo:Thomas
```

When executed, the *[foo]* invocation above, will look like this.

```
/* ... other code ... */
foo:Hello there Thomas
```

Notice the "Hello there Thomas" value of the *[foo]* node above. This was the return value, from the global Active Event we created above.

When you create a dynamic Active Event, this event will be accessible for any p5.lambda object in your application pool, for the life 
span of your application pool. This means that if something resets your server, somehow, forcing the web-server application pool process
to restart, for some reasons (reboot of server or recycling your app pool for instance), you need to re-create your dynamic Active Events,
if you wish for them to still be acccessible.

In System42, this is easily done by creating a "startup" file, which is executed during startup somehow. Either by putting your *[create-event]*
invocations inside a Hyperlisp file, inside of your "system42/startup/" folder, or by putting your event creational logic, inside your _app's_
"startup.hl" file, inside of your "/system42/your-app/" folder.

This dynamic trait of p5.lambda, allows you to dynamically create Active Events, during runtime of your application, without requiring a 
recompilation, or anything similar for your code. It also allows you to "install" new apps in your system, without requiring a reboot of
the web-server process, etc.

Besides from that they're globally accessible, there is no real difference between a "normal lambda object", and a dynamically created
Active Event. They behave exactly the same way in all regards. When you invoke a dynamic Active Event, you are given a "shallow copy" of your
event's lambda object, which is evaluated, as if it was a normal lambda object, making it possible to pass in arguments, and return nodes and 
values, as if it was a normal lambda object, evaluated using the *[eval]* Active Event.

### Stateful Active Events

Sometimes, it might be necessary to create a "stateful" Active Event. Meaning, an Active Event, that somehow is able to remember state, across
multiple invocations. The way you'd do this in most other OOP programming languages, is by creating a static data field, or something. However,
in Phosphorus Five, no such thing exists. In fact, P5 doesn't even have OO any mechanism at all! However, if you have a node, who's value is 
of type node, then its value will actually be remembered across multiple invocations of your event. That's why we said "shallow copy" above.
Consider this Active Event.

```
create-event:foo
  _static:node:"count:int:0"
  set:x:/../*/_static/#?value
    +:x:/../*/_static/#?value
      _:int:1
  eval-x:x:/+
  return:Lambda invocation count was; {0}
    :x:/../*/_static/#?value
foo
foo
foo
```

If you now evaluate the following Hyperlisp, you will clearly see your *[foo]* Active Event is able to remember its "invocation count", 
across multiple invocations.

```
foo
foo
foo
```

For the record, the above "Ninja Trick" is something you should be _careful_ with, since stateful Active Events, in most cases, is _NOT_ 
considered "good architecture". However, for those rare occassions, where it makes sense, it really makes sense. A good example is for 
instance caching, and similar types of concepts.

### Protected dynamic Active Events

Sometimes, you want to create an Active Event, which you do not want others to be able to tamper with, change, or delete, for some reasons.
A good example of why, might be security reasons, where you want to have guarantees, of that nothing can modify your Active Event. This
is easily done, by using the *[create-protected-event]* Active Event, which is a "one way street", meaning once initially created, nobody can 
neither "uncreate" it, nor change it in any ways. Consider this code, which will throw an exception during evaluation.

```
create-protected-event:demo.protected-event
  foo
create-event:demo.protected-event
  foo2
```

The second *[create-event]* invocation above, will throw an exception.

### Ninja tricks

Below are some "Ninja tricks" for dynamically created Active Events. Most of whom depends upon the extremely dynamic nature of PPhosphorus Five,
and its ability to traverse meta-information about itself.

#### Inspecting a dynamically created Active Event

You can easily inspect the lambda objects for a dynamically created Active Event. This works quite easily, by "injecting" code into the Active
Event invocation, and then simply return the root node, before letting the event evaluate its own logic. Consider the following code.

```
create-event:foo
  sys42.confirm-window
    _header:Foo bar
    _body:Hello world
```

The above code, will create an Active Event, which simply displays a "confirmation window" once invoked. If you invoke it with *[foo]*, you will
see it in action. However, if you wish to inspect it, instead of invoking it, this can easily be done, by injecting code into it, as arguments,
which simply returns the root node. To see this in action, evaluate this code. Make sure you've created the Active Event first, by evaluating the
above Hyperlisp first though.

```
foo
  add:x:/+
    src:x:/../*/[2,]
  return
```

To understand what goes on in the above piece of p5.lambda, realize that arguments passed into an Active Event, will be prepended at the top of
the event. Since what we pass in, are Hyperlisp "keywords", this means that these keywords will be evaluated, from inside of our Active Event,
before the original code for our *[foo]* Active Event is evaluated. Since our code, contains a return invocation as its last piece of code,
this means that we will "return early" from our event, before the event lambda object itself, is even evaluated. 

Though, when our "injected" code is evaluated, it is from within the scope of the *[foo]* Active Event, which means that we have access to the 
lambda object of our dynamically created Active Event. Since our *[return]* invocation, is created such that it returns everything from the 3rd 
node (the /[2,] iterator), this means that only the original code of our Active Event *[foo]*, will be returned. And thus we can "inspect" our
Active Event, and its lambda object, without seeing the "injected code" in our result.

This is true, regardless of how complex an Active Event is. It only works for dynamically created Active Events though, and not C# declared 
events (for obvious reasons).

Other things you can do with this, is "monitoring the complexity" of your Active Events. For instance, it could very well be argued, that the 
more lambda nodes an event has, the more complex it is. By counting the nodes within an Active Event, we can at least to some extent, evaluate
its complexity. Imagine this code. (Which also depends upon you having created the *[foo]* Active Event from above previously)

```
foo
  return:x:/../**!/../*/[1,]?count
```

The above code, returns the "node count" of your Active Event's lambda object. Except the first node, which is our dynamically 
injected "return" node. Hence, we can with the above code, know exactly how many nodes, or lambda objects, any dynamically created
Active Event has.

You can of course extend this as you see fit, and for instance count "all [add] invocations" in your Active Events, and so on. This allows you
to acquire really rich and detailed meta-data information, about your dynamically created Active Events, in your system. To a much higher
extent, than what most other programming languages allows you to do.





