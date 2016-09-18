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

## [eval], the heart of p5.lambda

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

The above code creates a node structure, data-segment, containing p5.lambda instructions, which are evaluated as you invoke
*[eval]* on the *[_x]* node, passing in an expression leading to that node. To understand expressions, check out the 
documentation for the p5.exp project.

Notice that *[eval]* will _NOT_ evaluate any nodes, having names, starting with an underscore (_). This allows you to create
data-segments in your code, where these data-segments will not be raised as Active Events. All nodes having a name,
not starting with an underscore, will be attempted raised as Active Event by *[eval]*. This might produce weird results for you, 
if you do not start your data-segments with an underscore, and there just so happens to exist an Active Event, with the same name 
as the name of your data-segment.

A good rule of thumb, is to always start your "data-segments" with an underscore for the above reasons.

Notice though that if you explicitly invoke *[eval]* on a node, that starts with an underscore (_), then *[eval]* will evaluate that
node, making it become the root node of its execution context (stack of execution).

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

In the above code, logically the *[_x]* node is treated as an "inline function", before it is invoked twice, with different arguments.

*[eval]* can also return values, such as below.

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

#### Evaluating multiple lambda objects in one go

*[eval]* can also execute multiple sources in one evaluation. Imagine you have a node hierarchy, where
you have several nodes you wish to evaluate. This can actually be done with one *[eval]* invocation, since
each result of your expression will be evaluated in order, according to how they were fetched by your expression.
Try out the following code in your System42 evaluator to see this in action

```
_x
  create-literal-widget
    parent:content
    element:h1
    innerValue:Widget no 1
_x
  create-literal-widget
    parent:content
    element:h1
    innerValue:Widget no 2
eval:x:/../*/_x
```

The above expression will return the *[_x]* nodes in consecutive order, and *[eval]* will evaluate them
in the order returned by the expresssion. Hence, we end up with two new widgets on our page. Similar things
will happen if you have two *[eval]* invocations, returning some values back to caller, such as the following

```
_x
  return
    foo1:bar1
_x
  return
    foo2:bar2
eval:x:/../*/_x
```

Of course, if one of our *[eval]* invocations, returns a simple value, then only the latter's return value
will end up in our tree, after evaluation. Imagine the following.

```
_x
  return:foo1
_x
  return:foo2
eval:x:/../*/_x
```

In the above scenarion, after evalution of our *[eval]*, only the "foo2" string will be the value of our
eval invocation.

## "Keywords" in p5.lambda

p5.lambda contains several "keywords", which aren't really keywords, as previously explained, but in fact
simply Active Events, creating an extremely extendible environment for your programming needs. But the core
keywords, that should exist in a default installation of Phosphorus Five are as following.

### [eval-x], forward evaluating expressions

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

### [add], adding nodes to your trees

The *[add]* Active Event, allows you to dynamically add nodes into your p5.lambda objects (tree node hierarchy)

This active event must be given an expression as its destination, and can optionally take many different forms of sources through its *[src]*
or *[rel-src]* parameters. For instance, to add up a static source, into some node destination, you could accomplish that doing the 
following.

```
_x
  foo1:bar1
add:x:/-
  src
    foo2:bar2
```

A static source, can be as complex as you wish, and contain any tree hierarchy you can possibly declare in your p5.lambda structures.

And as previously explained at the top of this document, modifying the instruction pointer will work perfectly well, allowing you to
create code that modifies itself, before it evaluates the parts it nodified/added to itself. Imagine the following code

```
_x
  create-literal-widget
    parent:content
    element:h1
    innerValue:Original, static widget
  add:x:/..
    src:x:/../*/_arg/*
eval:x:/../*/_x
  _arg
    create-literal-widget
      parent:content
      element:h1
      innerValue:Dynamically injected into lambda object
```

What happens in the above lambda object, is that the *[eval]* invocation, passes in an *[_arg]* argument, which contains a *[create-literal-widget]*
invocation. After evaluating the first *[create-literal-widget]* node, inside of our *[_x]* node, the *[add]* invocation appends the content of
our *[_arg]* node, which was passed into *[eval]* into the "root node" for our tree, which during the evaluation of *[_x]* is actuall *[_x]*
itself. Then the instruction pointer moves on, realizing it has _ANOTHER_ (newly dynamically added) *[create-literal-widget]* node, which
is then evaluated finally, before the instruction pointer returns from *[_x]*.

This feature of course, is true for all Active Events that modifies the execution tree somehow. In the above code, you also see how to create
a "dynamic source" for your *[add]*, having an expression leading to the source, and not a constant hierarchy.

For the record, the above features I think is something completely unique to p5.lambda and P5, allowing injection of additional code,
into "methods and functions" (lambda objects), through modifying the execution tree it is currently executing directly, without any "parsing" 
of "code" occurring. This dynamic nature of p5.lambda objects, is as far as I know, completely unique to p5.lambda.

For the record, we could also have dropped the *[add]* invocation in the above code, since all arguments passed into an *[eval]*, automatically
becomes "root nodes" of the lambda object evaluated. Such as we show in the code below.

```
_x
  create-literal-widget
    parent:content
    element:h1
    innerValue:Original, static widget
eval:x:/../*/_x
  create-literal-widget
    parent:content
    element:h1
    innerValue:Dynamically injected into lambda object
```

However, if you evaluated the above code, you will see that now the "Dynamically injected" widget is created _FIRST_. This is because any
arguments passed into *[eval]*, will be inserted at the beginning of the lambda object, and not appended into it at the back.

### [if], [else-if], [else] - [while] my branches gently weeps

These Active Events is what allows you to control the flow of your program, using what's traditionally referred to as "branching" your code.
Logically they work roughly the same as most other branching keywords you've seen in other programming languages. With some subtle differences 
though. However, let's create the simplest if lambda object we possibly can, to see them in action first.

```
if:bool:true
  create-literal-widget
    parent:content
    element:h1
    innerValue:Yup, we branched!
```

The above *[if]* will yield true, since it is given a constant, having the value of boolean true. If we exchanged the value to false, it would
not create our widget. If you exchange the type declaration of your object though, to string, which is the implicit type, with the following code,
you would see something else.

```
if:false
  create-literal-widget
    parent:content
    element:h1
    innerValue:Yup, we branched!
```

This is because all string literals, automatically converts to _true_ for your conditional Active Events. Think of this as "implicit conversion"
to boolean values, according to "does there exist something". Meaning, not equals null is the implicit logic of your conditional events. You could
do this with any other types you wish, for instance integer numbers too.

```
if:int:1
  create-literal-widget
    parent:content
    element:h1
    innerValue:Yup, we branched!
```

The above code will even branch if you exchange the "1" with a "0", since there "exists something", which makes the condition yield true.
Notice though, that if you remove the ":int:1" part above, and exchange it with a "null node value", the branch will still occur.

```
if
  create-literal-widget
    parent:content
    element:h1
    innerValue:Yup, we branched!
```

The reason why the branch occurs, is because the conditional Active Events of P%, will if not given a condition as their values, use the
return value of the first node, treated as an Active Event itself, to check if the condition yields true. This allows you to embed event
invocations, as the conditions to conditional events. To understand how this works, try the folloing code.

```
if
  foo
  create-literal-widget
    parent:content
    element:h1
    innerValue:Yup, we branched!
```

In the above code, the *[foo]* node, will be evaluated as an Active Event, and if it returns true (or some sort of 'existence object'), the
condition as a whole will yields true. If you created an Active Event named "foo", and you made sure it returned for instance "true", then
the branch would occur, and we would have our widget created. Since *[foo]* above, is not an existing Active Event, it will not do anything,
making its value still being "null" after evaluation of itself, meaning the condition for *[if]* yields false, and no widget is created, since
the branch does not occur.

However, to use constants as your conditions for your conditional events, often makes little or no sense. A more useful approach, would be to use
an expression, yielding the results of some node-set, such as the following.

```
_x:bool:true
if:x:/../*/_x?value
  create-literal-widget
    parent:content
    element:h1
    innerValue:Yup, we branched!
```

If you change the value of the above *[_x]* node to false, or "null", the branch will not occur. If you change it to for instance the integer
5, the branch will occur though, since there "exists an object", which means the implicit "not equals null" logic kicks in.

Now a more useful approach, would be to compare it against, either some sort of constant value, or the results of another expression. Imagine 
the following code.

```
_x:int:5
if:x:/../*/_x?value
  =:int:5
  create-literal-widget
    parent:content
    element:h1
    innerValue:Yup, we branched!
```

In the above code, the return value of the expression in if, will be compared against the constant of "5", and the result will yield a match.
Hence, the branch occurs.

Notice, that branching conditions in p5.lambda are type sensitive, which means if you run the following code, where you compare an integer to
a string, it will _NOT_ yield true, since their types are different.

```
_x:int:5
if:x:/../*/_x?value
  =:5
  create-literal-widget
    parent:content
    element:h1
    innerValue:Yup, we branched!
```

You could however type cast the results of your expression above, to make them become equal, like the following code demonstrates.

```
_x:int:5
if:x:/../*/_x?value.string
  =:5
  create-literal-widget
    parent:content
    element:h1
    innerValue:Yup, we branched!
```

The ".string" parts at the end of our expression above, will convert the results of our expression, into a string, which of course will create 
a match for our conditional statement, since this value of our expression, will equal the constant string value of "5" in our "=" condition.

There exists 8 comparison "operators" in p5.lambda. "=", "!=", ">", "<", ">=", "<=", in addition to the two "like comparison" operators 
"~" and "!~". The first 6 of these "operators", does what you'd expect them to do, and works the same way in most other programming languages,
while the two latter "like comparison" operators, works the same way as the tilde (~) works in expressions, and are logically the equivalent 
of asking "does this string exist in the condition". Imagine the following code as an illustration.

```
_x:thomas hansen
if:x:/../*/_x?value
  ~:hans
  create-literal-widget
    parent:content
    element:h1
    innerValue:Yup, we branched!
```

The above condition will yield "true", since the string of "hans" truly do exist in the return value of the initial expression. Notice, that this
is a case sensitive string comparison, and only works for string comparisons. Or to be more precise, the values of both the constant, and the 
expression, will be automatically converted to "strings" before they're compared for a match.

For the record, all "comparison operators" are actually Active Events, and you can easily create your own "extension comparison operators",
creating your own Active Events, simply making sure you return "true" if they are supposed to evaluate to true.

#### More conditions, compound comparisons

In addition to these comparison operators, there is also all four boolean algebraic operations on comparisons. Allowing you to do things logically
equivalent to "if x equals some-value _AND_ y not-equals some-other-value", for instance. Imagine the following code.

```
_foo1:bar1
_foo2:bar2
if:x:/../*/_foo1?value
  =:bar1
  and:x:/../*/_foo2?value
    =:bar2
  create-literal-widget
    parent:content
    element:h1
    innerValue:Yup, we branched!
```

In the above code, both the conditions for the *[if]* and the *[and]* comparisons, must yield true, for the condition as a whole to yield true.
Meaning, if you change for instance the *[_foo2]*'s value to "bar3", the branch will not occur. This allows you to create "compound conditions", 
where the entire condition as a whole, must yield true, for the branch to occur.

The four basic algebraic operations are as following

* [and]
* [or]
* [xor]
* [not]

Each boolean operator, must be a direct child of the comparison it is supposed to elgraicly fuse with. This becomes the equivalent of paratheses in
traditional programming languages, allowing you to "scope" together conditions, the same way you'd use parantheses in other languages. Imagine the
following code for instance.

```
_name:thomas
_surname:hansen
if:x:/../*/_name?value
  =:thomas
  and:x:/../*/_surname?value
    =:hansen
  or:x:/../*/_name?value
    =:john
    and:x:/../*/_surname?value
      =:doe
  create-literal-widget
    parent:content
    element:h1
    innerValue:Yup, we branched!
```

In the above example, the branch will only occur if either the *[_name]* is "thomas" and the *[_surname]* is "hansen", or the *[_name]* is
"john" and the *[_surname]* is "doe". If the values are for instance "thomas" and "doe", then the branch will not occur. This is because the last
*[and]* operator, is a child of the *[or]*, and hence these are scoped together.

If you have both *[and]* and *[or]* operators, in the same scope, then *[and]* will get presedence, as in other programming languages.

Notice though that the syntax for p5.lambda, creates very "verbose" code, for complex conditions. This is due to that Hyperlisp is a simple
key/value/children file format, and in fact not a "programming language" per se. This means that each node can only have one name, and one value. In
addition to a list of children.

This is intentionally created this way, to allow for intelligently and semantically make it possible for the machine itself, to understand the code
from a semantic point of view, before it evaluates the lambda objects.

So even though this sometimes creates relatively "verbose" code, compared to other more consice programming languages, it also has some pretty
amazing side-effects. Such as making it very easy for your machine to understand what it is evaluating, by intelligently traversing the execution
tree, before it evaluates your code. Allowing for it to modify the execution tree, directly itself, before it evaluates your lambda object, etc, 
etc, etc.

However, the negative side-effect, is that sometimes the code becomes more verbose, and longer, for tasks that in other programming languages would
require far less code. On average though, the amount of code necessary to create some piece of logic, would often be far less in p5.lambda, than
what it would be in other programming languages. The above code example though, is the "trade-off price" you pay for this feature.

To understand why, realize that there literally "is no programming language, only the execution tree", so what you see in p5.lambda, is what a 
compiler, or an interpreter would produce, after having parsed your "code". In p5.lambda, "code" does in fact not really exist! You are modifying
the execution tree directly, without any steps inbetween you, and your machine, as would be the case in all other programming languages. Both 
compiled languages, and interpreted languages.

Some of the consequences of this, is that it to a much larger extent, allows the machine to produce its own code, in the long run, facilitating for
off-loading the burdon from the programmer, creating a much more "intention driven" environment for the programmer, where the programmer can simply
state his "intentions", and the machine creates the code that implements his intentions.

If ythe above paragraphs sounds like lots of "mumbo jumbo" to you, simply ignore it, run with p5.lambda, and watch your future change, to the better,
and making you more productive, as you start realizing how you can reuse code, to an extent never seen before in other programming languages after
a while.

One example of this, is the "control type of page" in the CMS parts of System42, where the machine is literally doing all the programming for you.
This can of course be taken _MUCH_ further than what I have done so far. And we will take it much further too. In a traditional programming language,
you have to play all the instruments. In p5.lambda, you are the conductor, having a complete symphonical orchestra at your disposal, allowing
you to "conduct" your pieces of logic together, where the machine to a much larger extent takes care of the "programming" for you.

However, before you can come to that point, you must first understand how, and why, the machine does what it does.

#### [not], slightly special, not ...

The *[not]* operator, due to the above reasoning, is slightly special in p5.lambda. This is because it needs to be the "name" of a node. Hence,
it will as a result always have to _FOLLOW_ the condition it is supposed to negate. Let us illustrate with an example.

```
_x:bool:false
if:x:/../*/_x?value
  not
  create-literal-widget
    parent:content
    element:h1
    innerValue:Yup, we branched!
```

In the above example, the return value of the expression in the *[if]* invocation, would normally return "false". However, this value is "negated"
due to the *[not]* boolean we're adding up on the line afterwards. Hence, the condition as a whole, will in fact yield "true", and the branching
occurs.

You can also "not" more complex conditions, such as the following is an example of.

```
_x:thomas
if:x:/../*/_x?value
  =:thomas
  not
  create-literal-widget
    parent:content
    element:h1
    innerValue:Yup, we branched!
```

The above branch would normally occur, since the constant of "thomas" in the *[_x]* node's value, and the constant of our *[=]* operator, 
would ccompare to true, normally yielding "true" as the results of the comparison. However, since the entire comparison is "negated" due
to our *[not]* operator at the end of our comparison, the branch does not occur, since the "true" value of our comparison, becomes "negated"
to false, yielding "false" as the product of our condition as a whole.

The *[not]* operator, should always be _the last_ operator in the scope where it appears. It cannot take any arguments. Neither constants, nor 
expressions.

#### [else-if] and [else]

The *[else-if]* is similar to the *[if]* Active Event, except it will only evaluate its condition(s) if the matching *[if]* yields false.
*[else]* will evaluate to true, if all of its matching and previously declared *[if]* and *[else-if]*s before it evaluated to false.

#### [while] my guitar gently weeps

The *[while]* Active Event, is similar to the *[if]* and *[else-if]*, except it will check its conditions upon every iteration. And only when
the condition yields false, it will break out of its scope, evaluating the next node in its scope. A *[while]* loop, can be explicitly stopped, 
using the *[break]* Active Event.

*** [for-each], iterating over a resulting node-set

The *[for-each]* Active Event, will iterate once for each resulting node/value/name in its expression. For each iteration, it will
inject a *[_dp]* node as its child, having the value of whatever is iterated over. For instance, consider the following code.

```
_data
  foo1:thomas
  foo2:hansen
for-each:x:/../*/_data/*?value
  eval-x:x:/+/*
  create-literal-widget
    parent:content
    element:h1
    innerValue:x:/..for-each/*/_dp?value
```

The above code, will iterate the values of each children node of the *[_data]* node, creating one literal widget for each iteration.
If you add more nodes beneath the *[_data]* segment, you will have more iterations, and hence more widgets in your page.

Notice that the *[_dp]* is special, according to what type of result-set you're iterating over. If you iterate over a node, instead of a 
value and/or a name, as we do above, the *[_dp]* node will store the currently iterated node by reference, in the value of itself. To illustrate
with some code.

```
_data
  foo1:thomas
  foo2:hansen
for-each:x:/../*/_data/*
  set:x:/..for-each/*/_dp/#?value
    src:COOL GUY!!
```

If you watch the output of the above code in your System42/executor, you will see that after evaluation, all values of the children nodes of 
*[_data]* will become "COOL GUY!!" after evaluation.

The "#" (hash character), is a special iterator in your expressions, yielding the value of the currently iterated node, as a node by itself,
somehow. Since this node is passed into our *[for-each]* by reference, we can change this node accordingly in our *[set]* invocation.

For more about iterators, check out the documentation for the "p5.exp" project.

To summarize; If you're iterating a value or name, the iterated value will be in the value of the *[_dp]* node. If you're iterating a node-set
in your expressions instead, then the node will be passed in by reference to your *[for-each]*, through its *[_dp]* node.

Every place where a *[_dp]* node is being used somehow, this is the logic to expect.

### [insert-before] and [insert-after]

These two Active Events works similar to the *[add]* Active Event, except instead of adding to a node's children collection, they will either 
insert before or after whatever node(s) you're pointing to in their destination expressions.

Imagine the following code.

```
_data
  foo1:thomas
  foo2:hansen
add:x:/../*/_data/*/foo1
  src
    this-was-add
insert-before:x:/../*/_data/*/foo1
  src
    this-was-insert-before
insert-after:x:/../*/_data/*/foo1
  src
    this-was-insert-after
```

The results of the above p5.lambda, if evaluated in the System42/executor will look like this.

```
_data
  this-was-insert-before
  foo1:thomas
    this-was-add
  this-was-insert-after
  foo2:hansen
... rest of your code ...
```

Nothing unexpected here hopefully ...

Besides from that, both *[insert-before]* and *[insert-after]* works similarly to *[add]*, and can take all the same sets of sources as
[add] can.

### [set], changing name or values of nodes, or the nodes themselves

The *[set]* Active Event requires an existing node, which it will modify somehow. You can set the name, value or the node itself as you please.
The *[set]* Active Event, is in these regards probably the most basic and fundamental Active Event in p5.lambda, besides from *[add]*.
Imagine the following code.

```
_data:foo
set:x:/-?value
  src:bar
```

As previously said, *[set]* can also change names of nodes, it can also change the nodes themselves, such as this example illustrates.

```
_data:foo
set:x:/-
  src:node:"howdy:world"
```

After evaluating the above p5.lambda, the results will look like this.

```
howdy:world
set:x:/-
  src:node:"howdy:world"
```

Notice that the *[_data]* node is now completely gone, and replaced with a *[howdy]* node, having the value of "world". This also works for entire
node hierarchies, and regardless of how many children the original *[_data]* node had, the entire node, including its children, would be replaced 
by the above p5.lambda object.

Notice in the above code, that the static *[src]* declaration of *[set]*, is actually type declared as a "node" itself. Meaning, the value of 
*[src]*, is itself a node, with the name of "howdy" and the value of "world".

*[set]* can also change the names of nodes, if the destination expression is type declared as "?name".

### [switch], a shorthand for multiple if invocations

If you have a bunch of values, which you want to compare some value towards, then you can use a *[switch]* statement, such as the below example
illustrates.

```
_foo:thomas
switch:x:/-?value
  case:john
    sys42.confirm-window
      _header:Hello there Mr
      _body:John Doe
  case:jane
    sys42.confirm-window
      _header:Hello there Mrs
      _body:Jane Doe
  case:thomas
    sys42.confirm-window
      _header:Hello there Guru
      _body:Thomas Hansen
  default
    sys42.confirm-window
      _header:Hello there Mr/Mrs
      _body:Stranger ...?
```

In the above example, unless the name os either "thomas", "jane" or "john" in the value of the *[_foo]* node, then the *[default]* lambda block
will be evaluated. If the name is "thomas", then the first *[case]* block will be evaluated. And so on ...

The *[default]* block is what is evaluated, if none of the other alternatives applies.

In a *[switch]* Active Event invocation, you can also create what's commonly referred to as "fall through". By having no block in one or more of
your *[case]* nodes, the lambda block of the next *[case]*, that has a lambda block, will be evaluated for a match. Consider this code.

```
_foo:john
switch:x:/-?value
  case:john
  case:jane
    sys42.confirm-window
      _header:Hello there Mrs/Mr
      _body:Doe
  case:thomas
    sys42.confirm-window
      _header:Hello there Guru
      _body:Thomas Hansen
```

In the above example, the lambda block of "jane" will evaluate, for both the value of "jane", and the value of "john". As you can see in the 
above example, the *[default]* block is optional. If you provide a *[default]* block however, a common convention is to put it as the last 
block in your *[switch]* logic.

### Exception handling in p5.lambda

No programming language is complete, without at least some form of exception handling. p5.lambda support exceptions through these Active Events

* [try]
* [catch]
* [finally]
* [throw]

They do what you'd expect them to do from other programming languages. The *[try]* Active Event creates a "safe block", where no exceptions will
penetrate through, without invoking any coupled *[catch]* blocks. The *[throw]* Active Event, raises an exception, unwinding the stack, all the
way to the next *[catch]* block, evaluating the lambda code inside of that catch. The *[finally]* block evaluates, regardless of whether or not an 
exception occurs. If an exception occurs, the exception is re-thrown after evaluation of the *[finally]* block though.

Consider the following.

```
try
  throw:Darn it, my head hurts!!
catch
  sys42.confirm-window
    _header:ERROR!
    _body:Something went wrong
```

In the above example, we explicitly throw an exception, with the message of "Darn it, my head hurts!!". If you wish, you can retrieve
this error message from inside of your *[catch]* block, through a *[message]* argument passed into your *[catch]*. To illustrate this with 
some code.

```
try
  throw:Darn it, my head hurts!!
catch
  set:x:/+/*/_body?value
    src:x:/..catch/*/message?value
  sys42.confirm-window
    _header:ERROR!
    _body
```

In addition to the *[message]* argument passed into your *[catch]* block, you also get the *[type]* of exception thrown, which is the 
fully qualified class name of it from .Net/Mono, and the entire *[stack-trace]*. The type of an exception thrown from Hyperlisp, will always
be "p5.exp.exceptions.LambdaException".

When you *[throw]* an exception from Hyperlisp, you can also pass in an expression as its message.

### [return],  returning from a lambda object evaluation

Functions or methods does not really exist in p5.lambda. However, you can treat any lambda object or node structure, as if it was a "function",
and invoke it using for instance *[eval]*. Sometimes when you do, you might want to return early from the evaluation of your lambda objects.
This can be accomplished with the *[return]* Active Event invocation.

This Active Event will return whatever you choose to return to the caller of the lambda evaluation, and end the execution of the outer most
lambda evaluation. To illustrate with some code.

```
_exe
  return:foo
  sys42.confirm-window
    _header:Never evaluated
eval:x:/-
```

After evaluating the above code in e.g. the System42/executor, the value of the *[eval]* node will contain the string of "foo". In addition,
the *[sys42-confirm-window]* invocation will never be invoked, since when the evaluation of *[return]* is done, no further evaluation inside of
the outer most evaluated scope will be done. Since the outer most evaluated scope in the above example, is the evaluation of the *[_exe]* node,
this means that the control will be returned back to the *[eval]*, invoking the *[_exe]* node.

You can also return multiple nodes back to caller, and complex tree structures, using something like this for instance.

```
_exe
  return
    foo1:bar1
      foo2:bar2
    foo3:bar3
eval:x:/-
```

After evaluation of the above code, every child node of the above *[return]* invocation, will be appended into the *[eval]* node that invoked
the *[_exe]* block.

### [break], stopping the execution of loops

*[break]* works similar to the *[return]* Active Event, except it only carries meaning inside of either a *[while]* or a *[for-each]*. Instead
of returning though, it stops all further execution of all code within the first ancestor *[for-each]* or *[while]* block, and returns control
back to the next sibling node, after the loop node currently being breaked.

Imagine the following code.

```
_data
  foo1:bar1
  foo2:bar2
  foo3:STOP
  foo4:bar4
_dest
for-each:x:/../*/_data/*
  if:x:/./*/_dp/#?value
    =:STOP
    break
  add:x:/../*/_dest
    src:x:/..for-each/*/_dp/#
```

The above code iterates the *[_data]* segment's children, appending each child node into *[_dest]*, stopping further execution if it encounters
a value of "STOP". After evaluation of the above lambda, the *[_dest]* node will contain the "bar1" node, and the "bar2" node, but none of the other
two nodes from the *[_data]* segment.

*[break]* works similarly for *[while]* loops.

### [continue], continuing the next iteration of a loop

The *[continue]* Active Event will instead of breaking the entire loop, stop further execution of its current iteration, and continue on the
_NEXT_ iteration for the loop. Imagine the exact same code as in our *[break]* example, except we use a *[continue]* instead.

```
_data
  foo1:bar1
  foo2:bar2
  foo3:STOP
  foo4:bar4
_dest
for-each:x:/../*/_data/*
  if:x:/./*/_dp/#?value
    =:STOP
    continue
  add:x:/../*/_dest
    src:x:/..for-each/*/_dp/#
```

If you execute the above code, you will have the *[foo4]* node appended into your *[_dest]* node, because this time, instead of stopping all
further execution of your loop, the *[continue]* invocation instead simply stops the currently iterated execution, and continues with the next
iteration.


