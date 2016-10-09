Dynamically creating Active Events in p5.lambda
===============

In Phosphorus Five, you can either create static Active Events in C#, by adding the *ActiveEvent* attribute to your C# methods. As
you can see an example of [here](/samples/p5.active-event-sample-plugin/). You can also use any p5.lambda object as an "anonymous function". 
In addition, you can also easily execute Hyperlambda files, the same way you'd invoke a function or method, passing in arguments, 
and returning values, just like you would with a normal function. The latter could be accomplished using e.g. *[sys42.execute-lambda-file]*.

However, there is also the possibility of dynamically creating globally accessible Active Events, through the *[create-event]*
Active Event. This Active Event, allows you to modify the underlaying Active Event dictionary directly, by either modifying existing, 
adding new, or deleting existing entries within it. Almost like you would do with any other dictionary/hash-table object.
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

Notice the "Hello there Thomas" value of the *[foo]* node above. This was the return value, from the Active Event *[foo]* we created.

When you create a dynamic Active Event, this event will be accessible for any p5.lambda object in your application pool, for the life 
span of your application pool. This means that if something resets your server, somehow, forcing the web-server application pool process
to restart, for some reasons (reboot of server or recycling your app pool for instance), you need to re-create your dynamic Active Events,
if you wish for them to still be acccessible.

In System42, this is easily done, by creating a "startup" file, which is executed during startup somehow. Either by putting your *[create-event]*
invocations inside a Hyperlambda file, inside of your "/system42/startup/" folder, or by putting your event creational logic, inside your _app's_
"startup.hl" file, inside of your "/system42/your-app/" folder. The latter is the preferred way, since it creates better encapsulation and cohesion of
your app.

This dynamic trait of p5.lambda, allows you to dynamically create Active Events, during runtime of your application, without requiring a 
recompilation, or anything similar for your code. It also allows you to "install" new apps in your system, without requiring a reboot of
the web-server process, etc.

Besides from that they're globally accessible, there is no real difference between a "normal lambda object", and a dynamically created
Active Event - They behave exactly the same way in all regards. When you invoke a dynamic Active Event, you are given a "shallow copy" of your
event's lambda object, which is evaluated, as if it was a normal lambda object. Making it possible to pass in arguments, and return nodes and 
values, as if it was a normal lambda object, evaluated using the *[eval]* Active Event.

To understand dynamically created Active Events, means first understanding the [eval Active Event](/plugins/p5.lambda#eval-the-heart-of-p5lambda/)
Active Event, since they obey by the same rules.

## Deleting events

The *[delete-event]* Active Event, allows you to completely delete a dynamic Active Event. You can either provide an expression, leading to multiple
names, or a constant, deleting only one event.

However, if you invoke *[create-event]* with no lambda object as its children, you will also delete any existing events with the given name.
The *[delete-event]* allows you to delete multiple events at the same time though, which the *[create-event]*, without a lambda object does _not_.
Besides, it is probably better to be more "clear" in your usage of vocabulary, and explicitly use the Active Event *[delete-event]*, to communicate
your intent more precisely.

## Stateful Active Events

Sometimes, it might be necessary to create a "stateful" Active Event. Meaning, an Active Event, that somehow is able to remember state, across
multiple invocations. The way you'd do this in most other OOP programming languages, is by creating a data field, or something similar. However,
in Phosphorus Five, no such thing exists. In fact, P5 doesn't even have OO any mechanism at all!

However, if you have a node, who's value is of type node, then its value will actually be remembered across multiple invocations of your event. 
That's why we said "shallow copy" above. When you invoke an Active Event, then you get a copy of its lambda to evaluate. However, since all _values_ 
are copied by reference, and not cloned, this means that each invocation will act upon the same value object. For the .Net types, this has no 
consequence. However, for the Node class, which is the underlaying structure of a p5.lambda object, this has side effects, which you can benefit 
from, to create "stateful events". 

Simply because the copy of the Active Event you are evaluating, will have a "shallow copy" of its values, allowing you to _share_ node references,
inside of values of Active Events, across multiple invocations to the same event.

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

If you now evaluate the following Hyperlambda, you will clearly see your *[foo]* Active Event is able to remember its "invocation count", 
across multiple invocations. This is because the `_static:node:"count:int:0"` node's value will be shared for all invocations to your event.

Try to run this code.

```
foo
foo
foo
```

Notice, this feature, might require you to synchronize access to the p5.lambda that is accessing your nodes, to avoid race-conditions and similar. This is because
the value of our *[_static]* node above, is actually a shared resource. Synchronizing acccess to the *[_static]* node above, can easily be done by wrapping your 
p5.lambda in a *[lock]* block, which is documented in the "p5.threading" project. An example is given below.

```
create-event:foo
  _static:node:"count:int:0"
  lock:lock.foo
    set:x:/../*/_static/#?value
      +:x:/../*/_static/#?value
        _:int:1
    eval-x:x:/+
    return:Lambda invocation count was; {0}
      :x:/../*/_static/#?value
```

For the record, the above "Ninja Trick", is something you should be _careful_ with, since stateful Active Events, in most cases, is _not_ 
considered "good architecture". Besides, synchronizing access to shared resources, is often quite expensive, in terms of scalability. However, 
for those rare occassions, where it makes sense, it _really_ make sense. A good example is for instance caching.

Hint, if you re-create the above *[foo]* Active Event, you will reset the "hit count". Do you understand why ...?

## Ninja tricks

Below are some more "Ninja tricks" for dynamically created Active Events. Most of whom depends upon the extremely dynamic nature of Phosphorus Five,
and its ability to traverse meta-information about itself.

### Inspecting a dynamically created Active Event

You can easily inspect the lambda objects for a dynamically created Active Event. This works quite easily, by "injecting" code into the Active
Event invocation, and then simply return the root node, before letting the event evaluate its own logic. Consider the following code.

```
create-event:foo
  sys42.windows.confirm
    _header:Foo bar
    _body:Hello world
```

The above code, will create an Active Event, which simply displays a "confirmation window" once invoked. If you invoke it with *[foo]*, you will
see it in action. However, if you wish to inspect it, instead of invoking it, this can easily be done, by injecting code into it, as arguments,
which simply returns the root node's children. To see this in action, evaluate this code. Make sure you've created the Active Event first, by 
evaluating the above Hyperlambda first.

```
foo
  add:x:/+
    src:x:/../*/[2,]
  return
```

To understand what goes on in the above piece of p5.lambda, realize that arguments passed into an Active Event, will be prepended at the top of
the event. Since what we pass in, are Hyperlambda "keywords", this means that these keywords will be evaluated, from inside of our Active Event,
before the original code for our *[foo]* Active Event is evaluated. Since our code, contains a return invocation as its last Active Event,
this means that we will "return early" from our event, before the event lambda object itself, is even evaluated. 

Though, when our "injected" code is evaluated, it is from within the scope of the *[foo]* Active Event, which means that we have access to the 
lambda object of our dynamically created Active Event. Since our *[return]* invocation, is created such that it returns everything from the 3rd 
node (the `/[2,]` iterator), this means that only the original code of our Active Event *[foo]*, will be returned. And thus we can "inspect" our
Active Event, and its lambda object, without seeing the "injected code" in our returned result.

End result being that you get the entire lambda object for the original Active Event returned back to you. Pretty nifty ...

This is true, regardless of how complex an Active Event is. It only works for dynamically created Active Events though, and not C# declared 
events (for obvious reasons).

Notice, System42 implements an Active Event helper, that does exactly the above, called *[sys42.get-event]*.

Other things you can do with this, is "monitor the complexity" of your Active Events. For instance, it could very well be argued, that the 
more lambda nodes an event has, the more complex it is. By counting the nodes within an Active Event, we can at least to some extent, evaluate
its complexity. Imagine this code (which also depends upon you having created the *[foo]* Active Event from above previously).

```
foo
  return:x:/../**!/../*/[1,]?count
```

The above code, returns the "node count" of your Active Event's lambda object. Except the first node, which is our dynamically 
injected "return" node. Hence, we can with the above code, know exactly how many nodes, or lambda objects, any dynamically created
Active Event has.

You can of course extend this as you see fit, and for instance count all *[add]* invocations in your Active Events, and so on. This allows you
to acquire really rich and detailed meta-data information, about your dynamically created Active Events, in your system. To a much higher
extent, than what most other programming languages allows you to do.

p5.lambda is extremely "Agile" and "dynamic" in nature in such regards.

The unit tests for p5.lambda, which you can find [here](/core/p5.webapp/system42/apps/Unit tests/) actually implements something similar to
create a coverage report of Active Events, that are covered by the test suite.

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

The *[vocabulary]* event has a protected alias, called *[.vocabulary]* (of course), which will also return all protected events, but (of course) must be
raised from C#.

Also the *[create-event]* and *[delete-event]* have similar protected alias versions.

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

Which will return the p5.lambda object, for every single event, that has a name containing the "confirm" word, somehow, in its name. The reason why this works,
is because we're de-referencing the *[_arg]* argument for our Active Event, which is the value of the node used to invoke our *[test.retrieve-event-code]*,
in our invocation to *[vocabulary]*. This means that when *[vocabulary]* is evaluated, it will filter away every single event, not containing the word
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

Easy ...

```
eval
  test.retrieve-event-code
  return:x:/-/*/**/add?count
```

Or even better, how many times are every single p5.lambda "core" keyword, used in all Active Events, of your system ...?

Yet again, easy ...

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
you have in your system. It will even sort them, such that the most frequently used "keywords", are being returned first.

Ask yourself how difficult it would be to find out exactly how many "if" statements you have in a C# program for instance. Sure you could do a "Search all".
Maybe even add up some intelligent regular expressions, to exclude comments and such - But the possibility of false positives, would be huge. In p5.lambda,
you can create such constructs _semantically_, allowing you to retrieve the actual and absolute result, without any false positives having the opportunity to
sneak in!

In theory, this means you could for instance accept Hyperlambda from unknown sources, yet still relatively safely, evaluate the code, by first traversing its
usage of Active Events, to determine it does not invoke any dangerous events, which might compromise your system in any ways. Hyperlambda not only
allows you to easily implement "sandboxed execution", but at its core, in fact _is_ a sandboxed execution ennvironment. Facilitating for things difficult to
imagine, before you've seen them in action.

### Defaults to lambda objects

Sometimes you have for instance an Active Event, that takes a set of arguments, where you wish to use some default value, if the argument
is not supplied by the caller. This can easily be done, by combining the *[add]* Active Event, with some boolean agebraic expression trickery.
Example is given below.

```
_lambda
  _defaults
    _auto-focus:false
  _options
  add:x:/../*/_options
    src:x:@"(/../*/"":regex:/^_/""|/../*/_defaults/*)(!/_defaults!/_options)/$"
  insert-before:x:
    src:x:/../*/_options/*
eval:x:/../*/_lambda
```

The above lambda simply evaluates the *[_lambda]* node, which again first applies every argument given into *[_option]*. If an argument is
not given, it will fetch the value for the argument from the *[_defaults]* block.

When it adds the arguments, and their default values, into *[_options]*, it will exclude the *[_defaults]* node, and the *[_options]* node. 
Besides from that, it will consider ever node starting with an underscore (_) to be an argument. Due to our expression ending with the 
"unique name" iterator (/$), and that it fetches the arguments, before it logically ORs these arguments together, with the *[_defaults]* values - 
After evaluating the *[add]*, only root nodes for the lambda object starting with "_", or if not supplied, taken from the *[_defaults]* segment, 
will exist within the *[_options]* node.

Hint!
You can further simplify the above code, by avoiding the *[_options]* segment altogether, by using the boolean algebraic expression logic shown above,
every time you de-reference your argument. Such that if the argument is given, it uses the argument. If not, it uses the *[_defaults]* value.

The above lambda then simply returns everything afterwards from the *[_option]* node, to show what the node contains, after applying the 
arguments. If you invoke it with the *[_auto-focue]* argument set, you will see it returns your value. If you do not supply _auto-focus, it will
return the default value.

```
_lambda
  _defaults
    _auto-focus:false
  _options
  add:x:/../*/_options
    src:x:@"(/../*/"":regex:/^_/""|/../*/_defaults/*)(!/_defaults!/_options)/$"
  insert-before:x:
    src:x:/../*/_options/*
eval:x:/../*/_lambda
  _auto-focus:ARGUMENT SUPPLIED
```

The above technique, is quite useful for both having arguments in your p5.lambda objects, and/or Active Events, that have default values. In
addition, it serves the purpose of helping you to "document" your Active Events, since if used consistently by convention, any consumer of your
Active Events, can easily retrieve every single argument your Active Event acccepts, by invoking your lambda with some "add/return" trickery.
Such as shown below.

```
_lambda
  _defaults
    _auto-focus:false
  _options
  add:x:/../*/_options
    src:x:@"(/../*/"":regex:/^_/""|/../*/_defaults/*)(!/_defaults!/_options)/$"
  insert-before:x:
    src:x:/../*/_options/*
eval:x:/../*/_lambda
  insert-before:x:
    src:x:/../*/_defaults/*
  return
```

The above lambda, will work for any lambda object, including also Active Events. It will return every *[_defaults]* nodes you have within your 
lambda, and not in fact evluate the lambda itself at all.

If you wish to create something similar for your own Active Events, it would look something like this.
```
create-event:test.my-foo-bar-event
  _defaults
    _some-argument-1:foo
    _some-argument-2:bar
    _some-argument-3:bool:true
  _options
  add:x:/../*/_options
    src:x:@"(/../*/"":regex:/^_/""|/../*/_defaults/*)(!/_defaults!/_options)/$"
```

4 lines of code, plus all default values, and you have a 100% generic way of using default arguments in your Active Events, and in addition,
to help you document your Active Events, semantically, allowing consumers to easily retrieve meta-data, about the arguments your events accepts.

### Documenting your Active Events

You could even take this one step further, by adding documentation to your Active Event, being a node inside of your events themselves.
Then your code's documentation would always follow your code, even after conversion from Hyperlambda to p5.lambda, which would remove all comments and such, 
allowing any consumer of your code, to simply invoke your event with some custom *[add]* trickery, to retrieve your code's documentation.An example is given below.

```
create-event:foo-documented
  _dox:@"This is the foo method, it does something intelligent"
foo-documented
  return:x:/../*/_dox?value
```

As shown in this document, Hyperlambda is truly _Agile_!


