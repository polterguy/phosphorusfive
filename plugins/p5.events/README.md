Dynamically creating Active Events in p5.lambda
===============

In Phosphorus Five, you can either create static Active Events in C#, by tagging your C# methods with
the *ActiveEvent* attribute. You can also use any p5.lambda object as an "anonymous function".
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

Notice the "Hello there Thomas" value of the *[foo]* node above. This was the return value, from the Active Event *[foo]* we created above.

When you create a dynamic Active Event, this event will be accessible for any p5.lambda object in your application pool, for the life 
span of your application pool. This means that if something resets your server, somehow, forcing the web-server application pool process
to restart, for some reasons (reboot of server or recycling your app pool for instance), you need to re-create your dynamic Active Events,
if you wish for them to still be acccessible.

In System42, this is easily done, by creating a "startup" file, which is executed during startup somehow. Either by putting your *[create-event]*
invocations inside a Hyperlisp file, inside of your "/system42/startup/" folder, or by putting your event creational logic, inside your _app's_
"startup.hl" file, inside of your "/system42/your-app/" folder.

This dynamic trait of p5.lambda, allows you to dynamically create Active Events, during runtime of your application, without requiring a 
recompilation, or anything similar for your code. It also allows you to "install" new apps in your system, without requiring a reboot of
the web-server process, etc.

Besides from that they're globally accessible, there is no real difference between a "normal lambda object", and a dynamically created
Active Event - They behave exactly the same way in all regards. When you invoke a dynamic Active Event, you are given a "shallow copy" of your
event's lambda object, which is evaluated, as if it was a normal lambda object, making it possible to pass in arguments, and return nodes and 
values, as if it was a normal lambda object, evaluated using the *[eval]* Active Event.

## Stateful Active Events

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

Notice, this feature, might require you to serialize access to the p5.lambda that updates your nodes, to avoid multi-thread race-conditions. This is because
the value of our *[_static]* node above, is actually a shared resource. To serialize acccess to the *[_static]* node, can easily be done by wrapping your 
p5.lambda in a *[lock]* block, which is documented in the "p5.threading" project.

For the record, the above "Ninja Trick", is something you should be _careful_ with, since stateful Active Events, in most cases, is _NOT_ 
considered "good architecture". However, for those rare occassions, where it makes sense, it _really_ makes sense. A good example is for 
instance caching, and similar types of concepts.

## Ninja tricks

Below are some "Ninja tricks" for dynamically created Active Events. Most of whom depends upon the extremely dynamic nature of Phosphorus Five,
and its ability to traverse meta-information about itself.

### Inspecting a dynamically created Active Event

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
node (the `/[2,]` iterator), this means that only the original code of our Active Event *[foo]*, will be returned. And thus we can "inspect" our
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

You can of course extend this as you see fit, and for instance count "all *[add]* invocations" in your Active Events, and so on. This allows you
to acquire really rich and detailed meta-data information, about your dynamically created Active Events, in your system. To a much higher
extent, than what most other programming languages allows you to do.

### Listing all Active Events in your system

If you wish to see a list of all Active events in your system, this can easily be done using the *[vocabulary]* Active Event. Try it out in your
"system42/executor", and see how it returns a list of all Active Events in your system. Both those dynamically created, and those whom are statically
declared in C#. Example given below.

```
vocabulary
```

After evaluating the above code, you will get a list of nodes, where the name of the node says either "static" or "dynamic", where "dynamic" are your
dynamically created Active Events, vice versa. If you wish to exclude all "static" Active Events, this can easily be done with an additional *[set]*
invocation, like the following code illustrates.

```
vocabulary
set:x:/-/*/static
```

#### Ninja-Ninja tricks!

To see the code or p5.lambda structure for every single dynamically created Active Event in your system, you could do something like this.

```
vocabulary
set:x:/-/*/static
_invocations
add:x:/-
  src:x:/../*/vocabulary/*?value
add:x:/-2/*
  src
    add:x:/+
      src:x:/../*/[2,]
    return
add:x:/-3
  src
    insert-before:x:/../0
      src:x:/../*!/../*/insert-before
eval:x:/-4
set:x:/../*!/../*/eval
```

The above piece of p5.lambda, will actually retrieve every single dynamically created Active Event in your system, and allow you to see how they're composed.
If you want to create an Active Event that returns the code for every single Active Event in your system, optionally taking a "filter" argument, you
could create your Active Event like this.

```
create-event:test.retrieve-event-code
  vocabulary:x:/../*/_arg?value
  set:x:/-/*/static
  _invocations
  add:x:/-
    src:x:/../*/vocabulary/*?value
  add:x:/-2/*
    src
      add:x:/+
        src:x:/../*/[2,]
      return
  add:x:/-3
    src
      insert-before:x:/../0
        src:x:/../*!/../*/insert-before
  eval:x:/-4
  insert-before:x:
    src:x:/./-/*
```

Then you could invoke it like this for instance.

```
test.retrieve-event-code:confirm
```

Which would return the p5.lambda object, for every single event, that has a name containing the "confirm" word, somehow. The reason why this works,
is because we're de-referencing the *[_arg]* argument for our Active Event, which is the value of the node used to invoke our *[test.retrieve-event-code]*,
in our invocation to *[vocabulary]*. This means that when *[vocabulary]* is evaluated, it will filter away every single event not containing the word
"confirm" in its name, somehow.

If you consumed your newly declared Active Event with the following lambda instead.

```
test.retrieve-event-code:sys42.
```

Then it will return all Active Events from "System42", since they're all namespaced within the "sys42." namespace ...

If you pass in no argument to *[test.retrieve-event-code]*, then it will return all events for you.
You can even pass in an argument such as "test.retrieve-event-code", which would result in the invocation, returning the code for itself.

The ability to view "meta data" about your system, is really _key_ with Phosphorus Five. By intelligently creating your p5.lambda, you can answer
questions you didn't even realize you were wondering about. For example, how many p5.lambda nodes are there in all Active Events in my system?

Easy ...

```
eval
  test.retrieve-event-code
  return:x:/-/*/**?count
```

Or how many invocations do I have to my *[add]* Active Event?

Easy ... ;)

```
eval
  test.retrieve-event-code
  return:x:/-/*/**/add?count
```

Or even better, how many times are evey single p5.lambda "core" keyword used in my the Active Events of system ...?

Yet again, easy ... ;)

```
eval
  _result
  _keywords
    add
    set
    if
    else-if
    else
    break
    continue
    eval-x
    try
    catch
    finally
    throw
    for-each
    insert-before
    insert-after
    return
    src
    switch
    case
    default
    while
    apply
    fetch
    sort
  test.retrieve-event-code
  for-each:x:/../*/_keywords/*?name
    add:x:/../0
      src:"{0}:int:{1}"
        :x:/..for-each/*/_dp?value
        :x:/../*/test.retrieve-event-code/*/**/{0}?count
          :x:/..for-each/*/_dp?value
  sort-desc:x:/../0/*
  insert-before:x:
    src:x:/../*/sort-desc/*
```

The above lambda, will return exactly how many times, every "keyword" from p5.lambda is being used, in total, in every single dynamic Active Event
you have in your system. It will even sort them, such that the most frequently used "keywords" are being returned first.

Hint!
Implementing something similar on your Unit Test files, within your "/system42/apps/tests/" folder, allows you to create a "profile" of how many
of your keywords are being used within your tests. This requires loading up all files within your "tests" folder, instead of retrieving the lambda
objects for your dynamic Active Events.

This is left for you as an excersise though ...


