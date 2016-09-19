p5.events, dynamically creating global Active Events
===============

In Phosphorus Five you can either create static Active Events in C#, by tagging your C# methods with
the *ActiveEvent* attribute. You can also use any p5.lambda structure as an "anonymous function", or lambda object.
In addition you can easily execute Hyperlisp files, the same way you'd invoke a function or method, passing in arguments,
and returning values, just like you would with a normal function.

However, there is also the possibility of dynamically creating globally accessible Active Events through the *[create-event]*
Active Event. This Acttive Event, allows you to modify the underlaying Active Event dictionary directly, by either modifying, 
adding or deleting entries within it, almost like you would do with any other dictionary/hash-table object. Imagine the following code.

```
create-event:foo
  eval-x:x:/+
  return:Hello there {0}
    :x:/../*/_arg?value
foo:Thomas
```

When executed, the above code will look like this.

```
create-event:foo
  eval-x:x:/+
  return:Hello there {0}
    :x:/../*/_arg?value
foo:Hello there Thomas
```

Notice the "Hello there Thomas" value of the *[foo]* node.

When you create a dynamic Active Event, this event will be accessible for any p5.lambda object in your application pool, for the life 
span of your application pool. This means that if something resets your server, somehow, forcing the web-server application pool process
to restart for some reasons (reboot of server or recycling your app pool for instance), you need to re-create your dynamic Active Events
if you wish for them to be acccessible.

In System42, this is easily done by creating a "startup" file, which is executed during startup somehow. Either by putting your *[create-event]*
invocations inside a Hyperlisp file, inside of your "system42/startup/" folder, or by putting your event creational logic inside your app's
"startup.hl" file, inside of your "/system42/your-app/" folder.

This dynamica trait of p5.lambda, allows you to dynamically create Active Events during runtime of your application, without requiring a 
recompilation, or anything similar for your code. It also allows you to "install" new apps in your system, without requiring a reboot of
the web-server process, etc.

Besides from that they're globally accessible, there is no real difference between a "normal lambda object", and a dynamically created
Active Event. They behave exactly the same way in all regards. When you invoke a dynamic Active Event, you are given a "shallow copy" of your
event's lambda object, which is evaluated as if it was a normal lambda object, making it possible to pass in arguments, and return nodes and 
values, as if it was a normal lambda object.

### Stateful Active Events

Sometimes, it might be of value to create a "stateful" Active Event. Meaning an Active Event, that somehow is able to remember state, across
multiple invocations. The way you'd do this in most other OOP programming languages, is by creating a static data field, or something. However,
in Phosphorus Five, no such thing exists. However, if you have a node, who's value is of type node, then this value will actually be remembered
across multiple invocations of your event. Consider this.

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

If you now evaluate the following Hyperlisp, you will see that clearly your *[foo]* Active Event is able to remember its "invocation count" 
across multiple invocations.

```
foo
foo
foo
```

For the record, the above "Ninja Trick" is something you should be _careful_ with, since stateful Active Events, in most cases is _NOT_ 
considered "good practice". However, for those rare occassions where it makes sense, it really makes good sense though. A good example
is for instance caching, and similar type of problems.

### Protected dynamic Active Events

Sometimes, you want to create an Active Event, which you do not want others to be able to tamper with, change, or delete, for some reasons.
A good example of why, might be security reasons, where you want to have guarantees, of that nothing can modify your Active Event. This
is easily done by using the *[create-protected-event]* Active Event, which is a "one way street", meaning once initially created, nobody can 
neither "uncreate" it, nor change your event in any ways.


