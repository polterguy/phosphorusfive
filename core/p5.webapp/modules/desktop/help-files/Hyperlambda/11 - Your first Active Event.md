## Your first Active Event

So what is an Active Event? Short answer; __Everything__. In fact, the only thing you have done so far, is invoking Active Events. 
The **[micro.windows.info]** invocation from one of our previous chapters, is an Active Event. The **[set]** invocation we've used several 
times during the course of this book, also happens to be an Active Event. Active Events is the axiom, at which P5 and Hyperlambda evolves around.
Active Events almost entirely replaces functions and methods in Hyperlambda. To illustrate how powerful Active Events are, realise that
Phosphorus Five was almost entirely created without the use of functions and methods, using exclusively Active Events instead.

### Your first Active Event

You can easily create your own Active Events. Evaluate the following code in for instance Hypereval, or evaluate it
inline by clicking the _"flash"_ button.

```hyperlambda-snippet
/*
 * Creates an Active Event who's name is [examples.foo].
 */
create-event:examples.foo
  micro.windows.info:Foo was here!
```

Then evaluate the following code.

```hyperlambda-snippet
/*
 * Invokes our Active Event.
 */
examples.foo
```

Probably few surprises here. Congratulations for the record, you have created your first reusable Active Event. Already at this point, 
you can consume this Active Event, in your own applications.

**Warning**; Before you go berserk creating your own Active Events, please realize that if your web server process for some reasons is being 
restarted - _Your Active Event will vanish_. If you want to create _persistent_ Active Events, you'll need to declare them in e.g. a Hyperlambda 
file, and make sure this file somehow is executed, every time your web server process starts. At this point, it should probably become clear what our
initial _"startup.hl"_ file's purpose is, from our _"Hello World"_ example, which we created in one of our first chapters.

### Parametrizing your Active Events

To pass in arguments to an Active Event, is as easy as creating a child node in your lambda. Let's first create an Active Event, that can 
somehow handle our argument. Notice, we will be using the same name for our Active Event as our first example. This will ensure that our 
original Active Event becomes overwritten, and replaced with our new implementation, without any extra effort necessary from our side.

```hyperlambda-snippet
/*
 * Creates an Active Event who's name is [examples.foo].
 */
create-event:examples.foo
  micro.windows.info:{0} was here!
    :x:/../*/name?value
```

Afterwards you can invoke your Active Event with a **[name]** argument. Below is an example.

```hyperlambda-snippet
/*
 * Invokes our Active Event.
 */
examples.foo
  name:Thomas Hansen
```

Notice, this time our confirmation window is actually able to show the name of *"Thomas Hansen"*. Hence, it has obviously found our argument. 
To return arguments from your Active Events is equally easy. Notice, these next examples, which doesn't have
a _"flash"_ button, are easily evaluated in Hypereval. Make sure you view the output of your evaluation though,
such that you can see what goes on.

```hyperlambda
/*
 * Creates an Active Event who's name is [examples.foo].
 */
create-event:examples.foo
  return
    foo1:bar1
    foo2:bar2
```

Then invoke your **[examples.foo]** Active Event again. Hint, make sure you view the output while executing this code, to be able to see 
its results.

Sometimes you only wish to return a single *"thing"* from your Active Events, at which point you can do this, by using the **[return]** Active 
Event (yes, even the return *"keyword"* is an Active Event), and passing in whatever you wish to return, as the value of **[return]**. Below 
is an example.

```hyperlambda
/*
 * Creates an Active Event who's name is [examples.foo].
 */
create-event:examples.foo
  return:Hello World
```

At this point, the value **[return]**'ed from your Active Event, can be found as the value of your Active Event invocation node, like the following 
illustrates; `examples.foo:Hello World`.

### Referencing the "main" argument

If you pass in a value to your Active Events, you can reference this value as an **[\_arg]** node, inside of your Active Events. Evaluate this code, 
to see its effect.

```hyperlambda-snippet
/*
 * Creates an Active Event who's name is [examples.main-arg].
 */
create-event:examples.main-arg
  micro.windows.info:x:/../*/_arg?value

/*
 * Invokes our Active Event.
 */
examples.main-arg:Hello world!
```

The **[\_arg]** argument(s), are handled in a special manner though. If you pass in an expression for instance, as your main argument - Then this 
expression will be evaluated *before* your event is invoked. Hence, inside of your event, you will have access to the _result_ of your expression, 
and the expression will no longer exist. Consider this code.

```hyperlambda-snippet
/*
 * Evaluates our Active Event with an expression
 * as its [_arg] value.
 */
_name:Jo dude!
examples.main-arg:x:/@_name?value
```

Inside of your **[examples.main-arg]** event, the **[\_name]** node is no longer accessible, since it's outside of the scope of the event itself. 
However, since the main argument to our invocation is an expression, this expression is evaluated *before* the event is invoked. This means that 
the **[\_arg]**'s value, inside of our **[examples.main-arg]** event, is a simple constant value.

**Notice**, if you pass in an expression leading to multiple results, you will have multiple **[\_arg]** items inside of your event.

You can also pass in references to nodes as your arguments, allowing you to gain access to these nodes from within your event. Below is an example.
Run this example through Hypereval, and make sure you view the output resulting code to see what happens.

```hyperlambda
/*
 * Creates an event, expecting a node by reference.
 */
create-event:examples.foo
  set:x:/../*/_arg/#?value
    src:Yup, we were invoked!

/*
 * Invokes our event.
 */
_out
examples.foo:x:/@_out
```

The above example is the closest you come to _"closure"_ in Hyperlambda.

### Hyping Hyperlambda

It may be easy to believe that the name *"Hyperlambda"* is simply a marketing trick, in an attempt at trying to hype the language. However, as we 
will see in our next example, the word hyper is in fact well deserved.

```hyperlambda-snippet
/*
 * Creates an Active Event, taking a lambda object,
 * and evaluating that lambda object twice.
 */
create-event:examples.two-times
  eval:x:/../*/.exe
  eval:x:/../*/.exe
```

After having executed the above code, you can execute this code in your Executor.

```hyperlambda-snippet
/*
 * Invokes our Active Event with a lambda object.
 */
examples.two-times
  .exe
    create-widget
      element:button
      innerValue:Echo
      onclick
        micro.windows.info:Echo was here!

/*
 * Notifying user that he must scroll to the bottom
 * of the dox page to see the results.
 */
micro.windows.info:Scroll to the bottom of your page to see the result
```

What happens in our above example, is that we pass in a lambda object, intended to be evaluated. The **[eval]** invocations, inside of 
our **[examples.two-times]** event, evaluates the specified lambda twice. The simplicity of passing around such *"evaluation objects"* to other 
parts of your code, providing callbacks to other lambda objects, is another reason why Hyperlambda got its name. You can easily pass in such lambda 
objects, to web service endpoints, completely reversing the responsibility of the client and the server. For the record, you can also do this *safely*.

### Ignoring arguments

**Warning**; As you may have intuitively understood by now, you can supply whatever arguments you wish, to any Active Event in your system - And the 
Active Event will simply *ignore* your arguments, unless it was explicitly implemented to handle your arguments.

Doing such a thing, is probably not wise though - Since ignored arguments, must still be passed into your Active Events, rendering your system's state, 
such that it contains *dead code*. In addition, of course, this makes your system more difficult to understand, since others will have difficulties 
in understanding your code and intentions, by looking at it, believing that arguments that are *"dead"*, carries semantic meaning. In comparison to a 
strongly typed programming language, such as C#, which gives you compiler errors if you did the same - This might sometimes be a challenge for you, 
as you modify existing Active Events, removing old arguments, not in use anymore. However, this is a price you'll have to pay, for the added flexibility
that Active Events gives you.

This is also arguably the strength of Hyperlambda, since it allows you to modify existing events, taking additional arguments, without breaking existing 
code. Compare the above to the _"interface nightmare"_ from e.g. DirectX or other statically typed programming languages and/or frameworks, which needs a 
new version, of every single interface, every time they add a feature to their components.

There are several helper events in Micro which helps you reduce the implications of this problem. They are based upon _"lambda contracts"_, which allows
you to declare which arguments your event, and/or file, and/or lambda objects in general can legally handle. Check up the documentation for Micro to see
the details of this.

### Non-existing Active Events

Another peculiarity of Active Events, is that you can easily invoke an Active Event which doesn't exist. Your invocation, will however not find any 
existing events with the specified name, and simply do *nothing*. It will become a *null invocation*. This is a really nifty pattern in Hyperlambda in fact, 
which allows you to anticipate the existence of some *"future event"*, without having to implement it yourself. This allows you to use non-existent *"hooks"* 
in your code, which you document, and which the consumers of your events later can *"hook into"*. This is implemented a lot of different places in P5 in fact, 
and allows you to *"inject"* your own Hyperlambda logic, into the *"core kernel parts"* of P5, completely modifying its behavior.

### Naming conventions

There is nothing preventing you from creating the following Active Event.

```hyperlambda
/*
 * This is NOT a good name for an Active Event!
 */
create-event:57
  return:42
```

Whether or not this is a smart thing to do though, is probably *"debatable"*, to say the least. I often find myself encouraging others to use some sort of 
intelligent namespacing convention. Often this should be as unique as possible, to make sure your Active Events does not clash with Active Events created 
by others.

By carefully choosing a unique namespace for your Active Events, you make sure your code works, an often times even collaborates, side by side other 
people's code. My convention here, is to encourage others to use their company name, or some other *very unique piece of string*, as the first parts of 
your Active Events - For then to add the application name as the second - Then at the end, provide the actual name for your Active Event, providing 
some meaningful clue about what your Active Event actually does.

If we were to rewrite the above Active Event, with this in mind, creating a more intelligent naming convention - We could create something that resembles 
the following instead.

```hyperlambda
/*
 * This is (possibly) a better naming convention.
 */
create-event:gaiasoul.the-42-answer.what-is-57
  return:42
```

Notice the "." separating the different components of our *"namespace"*. This *"namespace"* is only a *"convention"* though, and carries no actual semantics.
You could also name your Active Events **[What is the number of $ I would get, for 57€ ...?]**, but I wouldn't recommend it. First of all, if the 
consumer of your Active Event forgets to add as much as a single space " " between the last EURO sign, and the "..." parts, his invocation would not 
invoke your event. You could also supply special characters in your event names, making it extremely difficult for others to invoke them. Examples 
includes carriage returns,"", Japanese characters, Greek letters, TAB, ASCII+7, etc. I wouldn't claim doing such a thing would never be wise - For 
instance, using Japanese or Chinese characters as event names, or Greek letters for that matter, could probably sometimes provide good contextual 
meaning. I am simply saying; *be careful*!

Active Events are carefully created to facilitate for *more* contextual meaning, and *improved* understanding of your systems. Not to set a new 
world record in obfuscated coding olympics - Although, you'd probably easily win such a contest, if you tried attending it with Hyperlambda. For 
instance, this is perfectly valid Hyperlambda, and creates an Active Event named **[]**, taking a **[∂]** argument - Which of course makes no 
sense at all.

```hyperlambda
/*
 * And the winner of obfuscated code olympics is ...
 */
create-event:
  return:{0} is the new ≈
    :x:/../*/∂?value
```

Why the above becomes almost absurd, can probably be understood, as you try to consume the above Active Event.

```hyperlambda
/*
 * Winning ... NOT!!
 */

  ∂:¸
```

### Restrictions to Active Event names

There are only 3 restrictions to what you can name your Active Events.

* You cannot start your event name with an underscore "\_"
* You cannot start your event name with a period "."
* You cannot create an event who's name is "" (empty string)

These restrictions applies only to Hyperlambda though. The reason is that Active Events starting with either a ".", or a "\_", are 
considered *"private core Active Events"*. You can create such events, but only from C#. In addition, you cannot invoke such Active Events 
from Hyperlambda, since the **[eval]** Active Event ignores all nodes starting with either "." or "\_", rendering your events useless, 
if you could create them. The Hyperlambda **[eval]** event, will neither attempt to raise any events having an empty name. Notice, 
you can however create Active Events in C# starting with ".", "\_", or having an empty name "". This is often useful too in fact.

The "" for Active Event, is a special Active Event, that can only be created from C#. This event will handle *all* Active Events, 
and allows you to tap into the core Active Event *"kernel"*, and modify its behavior.

Active Events starting with "." or "\_", can only be created in C#, and only consumed from C#. This allows you to create events, 
that can only be consumed from C#, which allows you to create *"internal C# code"*, which is not intended to be consumed by 
Hyperlambda directly. Sometimes this is useful for security reasons for instance. Phosphorus Five contains many such *"internal C# Active Events"*, 
in its C# core, which have been hidden, to make it more difficult to *"crack"* the system using Hyperlambda.

In addition, it is not possible to create an Active Event in Hyperlambda, which already has a C# implementation, such as **[create-event]**. 
Which would render your system close to useless in its entirety, if it was possible. If you want to exchange the implementation of any
of the *core* events, this is easily done, but only from C# (or VB.NET or F# etc).

### Stay away from my stuff!

You should also have an extremely good argument for creating Active Events starting with *"p5."*, since this is _my namespace_, 
intended for the system itself. I also highly encourage you to *not* create Active Events that have 
no namespace, such as for instance `create-event:foo-event`. The risk of having your event crash with other people's events, 
is simply too great - Rendering your system's ability to collaborate with other people's code impossible.

The convention I recommend, is to use something like the following.

```hyperlambda
create-event:company-name.project-name.event-name
```

There are some rare exceptions to the above rules though, such as if you create a specialized widget type in C# - At which point you'll 
need to break the above rules. But unless you are really certain about that you need to break the rules, _don't_!

### Argument conventions

I often encourage people to create lambda objects, intended for execution, by starting their names with a ".". First of all, this will 
make the syntax highlighter parser of the Hyperlambda code editor mark your lambda object as an *"execution object"*. Secondly, it makes such 
execution objects more easily tracked, and increases the readability of your code. So even though this is not technically a prerequisite, 
I find this to be a useful convention.
