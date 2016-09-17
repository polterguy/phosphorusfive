p5.lambda, the core "programming language" of Phosphorus Five
===============

p5.lambda is the core of the "programming language" in Phosphorus Five. Although, programming language
doesn't quite fit the description, since there is no "language" per se, but only a graph object hierarchy,
evaluated through the main execution engine, through the Node class being the execution tree structure, directly
evaluated as the execution graph.

However, for all practical concerns, it "feels like" a programming language, since it allows you to
create code in Hyperlisp, that will be evaluated, and it is Turing complete in regards to execution, and allows
you to do everything you can do in any other programming languages.

p5.lambda contains the basic Active Events, that implements the "programming constructs" of Phosphorus Five
though.

## *[eval]*, the heart of p5.lambda

The main Active Event in p5.lambda, and most important event in P5, is definitely *[eval]*. With *[eval]*,
you can evaluate any Node hierarchy, such as the one illustrated below

```
_x
  foo1:hello
  foo2:there
  foo3:stranger
  foo4:do you wish to become my friend?
for-each:x:/../*/_x/*?value
  set:x:/+/*/innerValue?value
    src:x:/..for-each/*/_dp?value
  create-literal-widget
    parent:content
    innerValue
```

If you run the above code, through the "apps/executor" in System42, you will see that it creates 4 web widgets for you.

The two Active Events *[for-each]* and *[set]* are declared in the p5.lambda project, and really as you can see in the code, 
actually simply Active Events, and nothing you cannot implement yourself, to extend the programming language if you wish.

The third Active Event called *[create-literal-widget]* is declared in p5.web.

What happens when you evaluate the above piece of Hyperlisp, is that *[eval]* is being invoked, with the root node
of your above Hyperlisp as the "current instruction pointer". To evaluate *[eval]* directly yourself, is quite simple,
and can be achieved with the following code

```
_x
  create-literal-widget
    parent:content
    position:0
    element:h1
    innerValue:foo bar
eval:x:/../*/_x
```

The above code creates a node structure, containing p5.lambda instructions, which are evaluated as you invoke
*[eval]* on the *[_x]* node, passing in an expression leading to that node. To understand expressions, check out the 
documentation for the p5.exp project.

There also exists an *[eval-mutable]* Active Event, which allows you access to the entire main root graph object, but
this Active Event is for the most parts for keyword implementors, and rarely something you'd consume yourself from Hyperlisp.

The difference between *[eval]* and *[eval-mutable]* is that the first creates a new root node hierarchy, not allowing the
execution to gain access to any nodes from the outside of itself, except nodes explicitly passed in as parameters, such
as the below code demonstrates

```
_x
  set:x:/+/*/innerValue?value
    src:x:/../*/_argument?value
  create-literal-widget
    parent:content
    position:0
    element:h1
    innerValue
eval:x:/../*/_x
  _argument:foo
eval:x:/../*/_x
  _argument:bar
```

*[eval]* can also return values, such as below

```
_x
  return:Hello World
set:x:/+/*/innerValue?value
  eval:x:/../*/_x
create-literal-widget
  parent:content
  position:0
  element:h1
  innerValue
```

To return more than one node, simply insert your return values into the root node, somehow, such as below for instance

```
_x
  return
    foo1:bar1
    foo2:bar2
eval:x:/../*/_x
```

Notice in the latter example, how there are two new children nodes of *[eval]* after it has evaluated.

*[eval]* is also extremely flexible, and allows for you to add, remove and change nodes, including at the
current instruction pointer, or just before it, or after it, if you wish. Consider the following code

```
insert-after:x:
  src
    create-literal-widget
      element:h1
      innerValue:Foo bar
```

What happens in the above example, is that as *[eval]* comes to the *[insert-after]* Active Event invocation,
there exists no *[create-literal-widget]* in the execution tree. The invocation to the *[insert-after]* Active Event
however, injects this node just after the *[insert-after]* node in the execution tree. When the instruction pointer
is done evaluating *[insert-after]*, it finds a new Active Event invocation, being our newly added *[create-literal-widget]*
Active Event reference, which in turn evaluates, creating our Literal widget.

If you change the above code to using *[insert-after]* instead, there will be no Literal widget after evaluation, 
but the node hierarchy is clearly changed, as you can see in the output, after evaluation of your Hyperlisp.

```
insert-before:x:
  src
    create-literal-widget
      element:h1
      innerValue:Foo bar
```

Still the instruction pointer for *[eval]*, managed to perfectly keep track, of where in the execution
tree it currently is, without messing up the order of your instructions. To prove it, run the following code

```
insert-before:x:
  src
    create-literal-widget
      element:h1
      innerValue:Foo bar
create-literal-widget
  element:h1
  innerValue:Foo bar 2
```

For the record, the above ":x:" expression, is the *"identity expression"* in the p5 expression engine, 
and simply means the "current node". This is where all expressions starts out.

For a more thourough explanation of expressions, yet again, check out the documentation for the p5.exp
project.


## Keywords in p5.lambda

p5.lambda contains several "keywords", which aren't really keywords, as previously explained, but in fact
simply Active Events, creating an extremely extendible environment for your programming needs. But the core
keywords, that should exist in a default installation of Phosphorus Five are as following.

### *[eval-x]*

This Active Event allows you to forward evaluate expressions, and is useful when you want to use expressions,
in Active Event invocations, where the Active Event itself does not in general terms support expressions, or
you need to evaluate expressions for some other reasons, before the event is invoked. Consider the following
code for instance

```
_y:hello world
_x
  return:x:/../*/_y?value
eval:x:/../*/_x
```

At the point where *[_x]* is evaluated, it no longer has access to the *[_y]* node, ssince this
node is outside th scope of the *[_x]* node, and we chose to use *[eval]* and not *[eval-mutable]*.
Now we could of course use *[eval-mutable]*, but this would create potentially other problems. Among
other things, having the evaluated code having access to the entire execution tree, which might be a security risk.

Instead we could choose to forward the expression inside of our *[_x]* node, in our *[return]* invocation, such
as the following code does.

```
_y:hello world
_x
  return:x:/../*/_y?value
eval-x:x:/../*/_x/*
eval:x:/../*/_x
```

When we then invoke our *[eval]* Active Event, the value of our *[return]* Active Event invocation is no
longer an expression, but contains the constant value of that expression, as evaluated during our invocation of our
*[eval-x]* Active Event.

The above example might not seem so very useful, besides, a better way of accomplishing the above, would be to pass in values
as arguments to our *[eval]* invocation. However, a more useful example, could imply using for instance *[create-literal-widget]*,
which is an Active Event, that takes a lot of properties and children nodes, where the invocation does not support expressions in any way.
Consider this code.

```
_x:Hello World
create-literal-widget
  parent:content
  position:0
  element:h1
  innerValue:x:/../*/_x?value
```

If you simply evaluate it as is, you will get the expression as a string literal, being the *[innerValue]* of your widget. However,
a little bit of *[eval-x]* magic, and we're all set for the type of result we really wanted.

```
_x:Hello World
eval-x:x:/+/*
create-literal-widget
  parent:content
  position:0
  element:h1
  innerValue:x:/../*/_x?value
```

Notice how we run *[eval-x]* on all nodes above, and not only the *[innerValue]*. This is because *[eval-x]* will simply
ignore nodes that does not have expressions as their values.

Notice also that *[eval-x]* _ALWAYS_ expects a Node result set, and not a "?value" or "?name". *[eval-x]* can also only
change values of nodes, and not names or other parts of them. This is because the type system in P5, only works for values, 
and *[eval-x]* can only change expression types, and nothing else. Everything that is not an expression, it will leave alone as is.


