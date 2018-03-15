## Eval is not (necessarily) evil

Douglas Crockford, the inventor of JSON, once famously said *"eval is evil"*. When it comes to JavaScript, Douglas might be right. However, 
in Hyperlambda, eval is not necessarily evil - *Unless* you consume it mindlessly.

The **[eval]** Active Event is arguably the *definition of Hyperlambda* in fact. When you evaluate a lambda object, what you're doing, 
is passing in a lambda object to eval. So eval is probably the most important Active Event in P5.
In our previous chapter, we looked at Active Events. To understand eval, is to realize that an Active Event, such as the ones 
we created in our previous chapter - Is really nothing but a lambda object, stored in memory, easily evaluated through its name. 
This implies, that evaluating a lambda object, is (almost) identical to invoking an Active Event.

**Internals** - When you create an Active Event from Hyperlambda, then the event is stored as a lambda object in memory, as a key/lambda
dictionary object. When the eval event evaluates any given lambda later, it will lookup into this dictionary, to see if it finds a match for
the given node's name, and if it does, it will evaluate a *copy* of that lambda object, passing in arguments to it, and returning
whatever your event returns, back to the caller. Hence, an Active Event created with for instance **[create-event]**, is simply nothing
but a globally accessible lambda object, and 100% semantically similar to any other lambda objects you can possibly create.

### Eval examples

The eval Active Event, allows you to invoke a lambda object, and evaluate it, as if it was an Active Event. With that in mind, 
let's use eval in some code. Paste in the following code into Hypereval.

```hyperlambda
.exe
  micro.windows.info:x:/../*/txt?value
eval:x:/@.exe
  txt:Jo world!
```

As illustrated above, you can easily pass in arguments to your lambda objects. You can also return arguments from eval, 
the same way you do from your normal Active Events. In fact, when you invoke an Active Event, P5 internally uses the eval 
Active Event to evaluate your event.

Combined with the ease of transforming between *"plain text"* and lambda objects that the **[lambda2hyper]** and **[hyper2lambda]** 
Active Events gives you - This easily allows you to evaluate literally *anything!* If you have a Hyperlambda file somewhere for instance, 
you can easily evaluate it, and even pass in arguments into it, by doing something resembling the following.

```hyperlambda
/*
 * WARNING; This code will not evaluate correctly,
 * since the file "/foo.hl" probably doesn't exist in your system.
 * It is simply an example of what you COULD do, to "evaluate" a file, 
 * as if it was a "function".
 */
load-file:/foo.hl
eval:x:/@load-file/*
  some-argument:Foo bar
```

The execution of a file, like we illustrated above, might also in fact return arguments. This means that there is 
actually *no semantic difference* between a lambda object, a file, some text fetched from some database, or supplied by a web service 
invocation - Or for that matter, a piece of string, supplied by the user, through some input element in your app.

In fact, the last parts in the above paragraph, largely defines the implementation of Hypereval. Hypereval simply converts your input to 
a lambda object, using **[hyper2lambda]**, and invokes this lambda object, using the **[eval]** Active Event.

### [eval] overloads

There actually exists 3 different versions of eval.

* __[eval]__ - Plain version
* __[eval-whitelist]__ - _"Sandboxed"_ version of eval
* __[eval-mutable]__ - Allows you to access the entire root lambda object

The first one, which is probably the most important, actually creates a *copy* of the lambda you wish to evaluate, and evaluates this copy. 
This is crucial to its implementation, considering how the invocation of a lambda object, potentially changes the state of that object.
Unless eval had created this copy, and evaluated the copy of your lambda, it would imply that the execution of an Active Event, 
potentially mutated your Active Event - Which of course would make your system become extremely unpredictable.

You can easily evaluate multiple lambda objects with eval in one go. Below is an example.

```hyperlambda-snippet
.exe
  create-widget
    innerValue:foo
.exe
  create-widget
    innerValue:bar
eval:x:/../*/.exe
```

If you pass in any arguments to your **[eval]** above, then each lambda object invoked, will have its own copy of your arguments. 
You can also have multiple lambda objects return values, such as the following illustrates.

```hyperlambda
.exe
  return
    first-name:Thomas
.exe
  return
    last-name:Hansen
eval:x:/../*/.exe
```

If you wish, you can instead of providing an expression to a lambda object, also supply the lambda object *"inline"* to your **[eval]** 
invocation, such as the following illustrates. This might sometimes be useful, since it allows you to evaluate a piece of Hyperlambda,
without this lambda object having access to the rest of your lambda.

```hyperlambda
eval
  micro.windows.info:Hello world!
```

Or for that matter ...

```hyperlambda
eval:"micro.windows.info:Hello world!"
```

### Sandboxing your lambda with [eval-whitelist]

The **[eval-whitelist]** version, works similarly to the plain eval - Except that it expects a *"whitelist"* supplied as 
an **[events]** node list, that is a list of events that your lambda object can legally invoke. This creates a *"sandbox"* environment 
for you, where you can evaluate a lambda object, supplied over for instance an HTTP web service, by an untrusted client, _without_ running 
the risk of having the client executing malicious events. Imagine the following.

```hyperlambda-snippet
.exe
  return:Safely evaluated by [eval-whitelist]
eval-whitelist:x:/@.exe
  events
    return
```

If you tried to add any Active Event into the above **[.exe]** lambda object, that does not exist in the **[events]** argument, 
an exception would be raised, and the code would be aborted. This allows you to provide an *explicit* set of legal *"whitelisted"* 
Active Events to some lambda object, enforcing it to obey by a subset of your server's vocabulary of Active Events. The code below 
for instance, will abort the execution, once it reaches the **[foo]** node, since foo is not in the list of legal events.
Notice, this is true, even though you don't even have a foo Active Event in your system.

**Warning**, the following code will throw an exception. But you can still try to evaluate it, and see how **[eval-whitelist]** 
prevents an adversary from injecting malicious code into your system.

```hyperlambda-snippet
.exe
  foo
  return:This node will never be reached!
eval-whitelist:x:/@.exe
  events
    return
```

When you have a piece of code, that you're not entirely sure if you should trust - Then you should definitely run it through some whitelist, 
using something similar to the above. This ensures the code does not harm your system in any ways.

Notice though, that sometimes one Active Event invokes another Active Event. Imagine if we wanted to whitelist the **[micro.windows.info]** Active 
Event for instance. Well, this event, invokes a whole range of other events. So simply whitelisting our micro.windows.info event, would not be 
enough. Below is an example of some piece of code, that will *not* evaluate. This code will also throw an exception.

```hyperlambda-snippet
.exe
  micro.windows.info:Doesn't work!
eval-whitelist:x:/@.exe
  events
    micro.windows.info
```

If you wish to have the above code evaluate legally, you'll have to *"whitelist"* all events that your **[micro.windows.info]** event invokes 
internally. This ensures, that as your system grows, and changes - You do not risk having malicious, unintentional code, evaluate as a consequence. 
Which is hopefully something you will come to appreciate, after you've used the system for a while.

### [eval-mutable] for keyword developers

The last overload, namely **[eval-mutable]**, is for the most parts for keyword developers, and rarely something you'd use much yourself. Simply 
because it works in a completely different way, than both of our previously mentioned versions.

First of all, passing in arguments, or returning arguments from it, is meaningless. Because it has access to the entire lambda object anyway. 
This point makes it also quite dangerous in day to day use, since its execution can potentially change any parts of your lambda object. For 
an Active Event such as **[add]** or **[set]**, this is _necessary_ and wanted behavior. However, for a lambda object, you want to evaluate 
yourself, this might create some really weird side-effects.

The **[eval-mutable]** does for one, *not* evaluate a copy of the lambda object, but evaluates the object directly, which is why it is 
called *"mutable"*. Because it potentially *"mutates"* your code.

**Warning**, unless you are certain about what you are doing, and you understand the consequences, then stay away from **[eval-mutable]**. Besides from 
the *"keywords"* of P5, there's only one place in Phosphorus Five I use it myself. And even here, I take great care, to make sure it doesn't 
produce unwanted side effects.
