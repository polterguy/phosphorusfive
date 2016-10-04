p5.lambda, the core "programming language" of Phosphorus Five
===============

p5.lambda is the core of the "programming language" in Phosphorus Five. Although, programming language
doesn't quite fit the description, since there is no "language" per se, but only a graph object hierarchy,
evaluated through the main execution engine, through the Node class being the execution tree structure, directly
evaluated as the execution graph.

However, for all practical concerns, it "feels like" a programming language, since it allows you to
create code in Hyperlambda, that will be evaluated, and it is Turing complete in regards to execution, and allows
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

What happens when you evaluate the above piece of Hyperlambda, is that *[eval]* is being invoked, with the root node
of your above Hyperlambda as the "current instruction pointer". To evaluate *[eval]* directly yourself, is quite simple,
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
this Active Event is for the most parts for keyword implementors, and rarely something you'd consume yourself from Hyperlambda.

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
but the node hierarchy is clearly changed, as you can see in the output, after evaluation of your Hyperlambda.

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

#### Notice!

Nodes starting with either an underscore (_) or a period (.), are _impossible_ to evaluate with p5.lambda. Hence, Active Events starting
with one of these characters, are considered "C# only Active Events", only possible to raise from C#.

This is by convention, to make it possible to create "p5.lambda hidden Active Events", which is only possible to raise from C#,
for security reasons for instance.

Realize though, you can evaluate an *[eval]* block, which has a name, starting with an underscore or period. However, the *[eval]* block
won't raise any Active Events starting with either of these two characters.

The convention is to use underscore (_) for "data segment", and period (.) for "invisible Active Events", although, you can choose this for yourself.
There is no semantic difference in whether or not you use an underscore or a period.

#### Evaluating stuff that's not a node

If you try to evaluate something that is somehow not a node, then this object will be attempted converted into a node, before it
is evaluated. This allows you to evaluate strings, values, and even integer values for that matter, as if they were a node structure,
making *[eval]* perform an automatic conversion (attempt) on your values before evaluating your object. Example below illustrates this.

```
_x:@"sys42.confirm-window
  _header:Howdy from string
  _body:Yup, I've got a body. Do you ...?
return:Returned from string!"
eval:x:/../*/_x?value
```

You can still return values and nodes as usual. There is actually no difference in regards to logic of evaluating a node or lambda object,
or evaluating a string, except that unless the string you try to evaluate does not for some reasons convert legally into a node, the Hyperlambda
parser will choke, and throw an exception.

#### Evaluating a block of lambda instead of an expression

If you do not provide a value to *[eval]*, then it will instead of handling its children as arguments, directly evaluate its children, as
a p5.lambda block. This is often useful in combination with for instance the *[set]* Active Event, which we will later dive into, further
down in the document here. However, to illustrate its simplest version, imagine this code.

```
eval
  create-literal-widget
    element:h3
    innerValue:Foo, bar!
```

In the above code, the p5.lambda execution engine will see that our *[eval]* invocation does not have any expression, or anything in its
value, that can be converted into a node in any ways. Therefor it will simply evaluate its own children nodes, as a piece of lambda block,
having still the root node of its evaluation being the *[eval]* node itself.

Later in our documentation we will dive into ssome use-cases where this is extremely useful. But for now, just keep it at the back of your mind.

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

This active event must be given an expression as its destination, and can optionally take many different forms of sources through its *[src]*.
For instance, to add up a static source, into some node destination, you could accomplish that doing the following.

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

Notice though that the syntax for p5.lambda, creates very "verbose" code, for complex conditions. This is due to that Hyperlambda is a simple
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

### Remove nodes or values with [set]

If you want to remove a node, or remove a value for that matter, then you can use *[set]* without a source at all. This will remove the 
destination, instead of changing it. For instance to remove a node you could use something like the code below.

```
_data:foo
set:x:/-
```

Notice that the p5.lambda execution engine perfectly tolerates tha above code, without cluttering the execution instrucction pointer, or
messing up the order of the nodes that are supposed to be evaluated. The same way it'll do if you add or change nodes just below its current
instruction pointer.

The p5.lambda execution engine is extremely tolerant in regards to changes in the tree. You can even remove the currently iterated pointer,
and still have the engine perfectly continue its normal execution flow. Consider this code.

```
_data
set:x:
set:x:/../*/_data?value
  src:foo
```

If you run the above code through the System42/executor, it'll perfectly evaluate, and change the *[_data]* node's value to "foo". Realize 
though, that since the tree is changed after our first *[set]* invocation, expressions might change as a result. Consider this code for 
instance.

```
_data
set:x:
set:x:/-2?value
  src:foo
```

The above code have some "weird consequences". To understand why, realize that as the execution engine reaches your second *[set]* invocation,
the first [set] is gone. Hence, the "2nd younger sibling iterator" (/-2) will not return the *[_data]* node, but instead traverse past the 
beginning of the tree, realizing it's don a round-trip, wrapping around to the eldest node in the tree, ending up with having the value of the 
second *[set]* node change its value, and not the *[_data]* as might be assumed at first glance.

This occurs since expressions are not evaluated before the instruction pointer reaches them. Which allowss you to reference nodes in your code,
which are later created, as a consequence of the code being evaluated. It is said that "expressions are lazily evaluated".

### Using an Active Event source for your [set], [add] or [insert-x] invocations

Both the *[set]*, *[add]* and *[insert-xxx]* Active Events can instead of taking a static source, use an Active Event as their source.
Imagine the following.

```
_data
  person
    first:Thomas
    last:Hansen
  person
    first:John
    last:Doe
_exe
  eval-x:x:/+/*
  return
    full-name:{0}, {1}
      :x:/../*/_dn/#/*/last?value
      :x:/../*/_dn/#/*/first?value
add:x:/../*/_data/*
  eval:x:/../*/_exe
```

In the above code, the *[_exe]* lambda object will be invoked once for each destination of our *[add]*. The invocation of our _exe block, will
have a *[_dn]* (data node) passed into it, each time, beving the destination source currently being iterated. This allowss us to have a 
"relative source" for each iteration, where what gets added into our destination, depends upon the destination itself.

You can use any Active Event you wish instead of a *[src]* node, including your custom Active Events. Regardless of which Active Event you 
choose to use as your source, each invocation of your Active Event will be passed in the *[_dn]* pointing to its current destination.

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
fully qualified class name of it from .Net/Mono, and the entire *[stack-trace]*. The type of an exception thrown from Hyperlambda, will always
be "p5.exp.exceptions.LambdaException".

When you *[throw]* an exception from Hyperlambda, you can also pass in an expression as its message.

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

### [apply], modifying your tree on steroids

With the *[apply]* Active Event, you are able to to "apply" one *[src]* node, expression or constant, to your "destination", acccording to 
a *[template]*, referencing nodes and values from your *[src]*. Let's look at an example.

```
_people
  thomas
  john
  jane
apply:x:/../*/create-widget/*/widgets
  src:x:/../*/_people/*
  template
    literal
      element:h3
      {innerValue}:x:?name
create-widget
  parent:content
  widgets
```

To understand the above code, realize that the *[apply]* Active Event will iterate over its *[src]* expresssion node-set, and "apply" the
*[template]* for each iteration into the "destination" expression, which is the value of the *[apply]* node itself. While each node,
having a name surrounded with braces "{}" inside of your *[template]*, will be expected to be dynamically fetched from the *[src]*, which
must be relative to the currently iterated *[src]* node-set.

Since the *[src]* in the above example, is iterating each children node beneath the *[_people]* node, this means that the ":x:?name" 
expression, for the above *[{innerValue}]* node, will fetch the names of these nodes ("thomas", "john" and "jane") as the loop iterates.

It sometimes helps to "unroll one iteration" in your mind, to understand the expressions inside of a *[template]* argument.

In the above example, our *[src]* expression yields 3 nodes as its result. Hence, the *[template]* is "applied" 3 times to the "destination",
which is the value expression of our *[apply]* node. Every time it "applies" the template, it will "unroll" each "databound" expression,
such as the one in *[{innerValue}]* above, with the expression being evaluated relatively to the currently iterated *[src]*.

Only nodes that have names surrounded by braces ({}) will have their values dynamically changed, according to the *[src]* node being iterated.
All other nodes, and expressions, will be left unchanged after evaluation. However, the braces ({}), will be removed after *[apply]*
has been evaluated. This means, that with the above syntax, you can only change _values_ of nodes. However, to change and/or create new nodes,
using *[apply]*, is achieved using another feature of *[apply]*. Before we look at that, let us first create a more complex apply
evaluation, to further visualize the iteration of a *[apply]* invocation.

```
_people
  person1
    name:Thomas Hansen
  person2
    name:John Doe
  person3
    name:Jane Doe
apply:x:/../*/create-widget/*/widgets
  src:x:/../*/_people/*
  template
    literal
      element:h3
      {innerValue}:x:/*/name?value
create-widget
  parent:content
  widgets
```

The above p5.lambda, will produce roughly the same output. However, now you can more easily see how the expressions inside of the databound 
nodes (nodes having names surrounded by braces ({})), are dynamically fetching their values, having expressions being "relative" to the
currently iterated node, since you see the expression in the above *[{innerValue}]* node, is referencing a child *[name]* node.

Normally an "identity" expression will point to the node where the expression is declared. In *[apply]*, the "identity expression" will
instead lead to the "currently iterated node", from its currently iterated *[src]* node-set value.

If this was the complete feature list of *[apply]*, it wouldn't be very powerful though. First of all, we cannot decide what names nodes
are created dynamically. The *[template]* seems to be static for all items. Secondly, we cannot return different nodes, according to the 
data result-set currently being iterated. This too though is easily accomplished using *[apply]*. To understand how, realize that you can 
invoke any Active Event you wish during an apply iteration, including also for instance *[eval]*. This allows you to evaluate some lambda 
object, for each iteration of an apply iteration, passing in the currently iterated node-set. This is being done byt prepending an alpha 
character in front of the name of your databound node, at which point your entire databound node will be discarded, and replaced by whatever 
(if anything), returned from the evaluation of your lambda object. Let us illustrate with some code.

```
_people
  person1
    name:Thomas
    surname:Hansen
  person2
    name:John
    surname:Doe
  person3
    name:Jane
    surname:Doe
_exe
  eval-x:x:/+/*/*/innerValue
  return
    literal
      element:h3
      innerValue:{0}, {1}
        :x:/../*/_dn/#/*/surname?value
        :x:/../*/_dn/#/*/name?value
apply:x:/../*/create-widget/*/widgets
  src:x:/../*/_people/*
  template
    {@eval}:x:/../*/_exe
    hr
create-widget
  parent:content
  widgets
```

Basically, to invoke an Active Event for a databound node, use syntax like this; "{@some-active-event}". The Active Event you invoke, can
be any Active Event you wish. And whatever your Active Event returns as nodes, will replace the databound node entirely after evaluation 
of your Active Event. If you watch the output of the above code in e.g. System42/executor, you will see that there are no traces of
our original *[{@eval}]* node after evaluation.

You can combine Active Event invocation sources, with plain databound sources, as we started out with, and/or static nodes, as you see with
the *[br]* node in the above example.

Notice that although we are creating a widget hierarchy above, you can create any node structure you wish with *[apply]*, including
insertions into your database, etc, etc, etc. Any node structure you want to create, is easily accomplished using *[apply]*.
Databinding in p5.lambda is not in any ways restricted to graphical objects, such as you're probably used to with "databinding" from 
other programming languages.

#### Escaping databound template value

Sometimes, you have a node, which for some reasons, should be named "{xxx-something}". This creates a problem, since it'll be assumed
to be a databound node. Use cases for such scenarios, might be for instance nested *[apply]* lambda objects. For such cases, all you need
to do, is to for instance prepend your node's name with a back slash (\). Imagine the following code, which contains nested apply lambda
blocks.

```
_data
  person:1
    first:Thomas
    last:Hansen
  person:2
    first:John
    last:Doe
  person:3
    first:Jane
    last:Doe
_output1
_output1-exe
  eval-x:x:/+/*
  return
    innerValue:{0}, {1}
      :x:/../*/_dn/#/*/last?value
      :x:/../*/_dn/#/*/first?value
apply:x:/../*/_output1
  src:x:/../*/_data/*
  template
    create-literal-widget
      element:h3
      {@eval}:x:/../*/_output1-exe
      onclick
        {_foo}:x:/*/first?value
        apply:x:/../*/_exe
          src:x:/../*/_foo
          template
            \{sys42.info-window}:Hello {0}
              :x:?value
        _exe
        eval:x:/-
eval:x:/../*/_output1
```

The above code might seem difficult to understand when you first look at it. The important point though, is that we have two nested *[apply]*
invocations above. Unless we had added the back-slash at line 29, then this databound node would have been databound in the first apply
invocation. Which obviously was not our intention. Hence, we add a back-slash in front of it, which will be removed during the first databound,
but also make sure our node is not databound during the first apply invocation.

The code creates one *[create-literal-widget]* invocation, for each child node of our *[_data]* segment, having the *[innerValue]* created
as a combination of the *[last]* and *[first]* name of our persons.

Then we handle the *[onclick]* Ajax event for our widgets, from where we invoke *[apply]* towards an invocation of *[sys42.info-window]*.
Our info window is then databound towards the *[_foo]*'s value, which was previously databound in our first *[apply]* invocation towards
only the *[first]* name. Resulting in that when our widgets are clicked, they will speak "Hello 'first-name'".

The above is probably not a relevant example, and the same result could have been easily achieved with other means. However, it is an example
of how you must escape your inner *[apply]* invocations databound nodes, if you embed them inside of outer *[apply]* invocations.

If you remove the back-slash at line 29 in the above code, instead of showing the "first name" in an info window, it will show the "ID" of
your person (1, 2 or 3) when you click the widget. This is because the *[sys42.info-window]*, will be databound byt the first invocation to
our *[apply]* Active Event, which obviously was not our intention.

Notice!
Sometimes you need to have the results of a *[apply]* invocations create a node who's name starts with a alpha character (@). If the value
of this node is databound itself though, this would instead of creating a node starting with @ in its name, expect an Active Event who's name
was that of your node. You can accomplish this though creating a lambda object, returning a node, who's name starts with an "@" though, and 
then use *[eval]* as an Active Event invocation for your databound node. Imagine the following code.

```
_people
  person1
    name:Thomas Hansen
  person2
    name:John Doe
  person3
    name:Jane Doe
_exe
  eval-x:x:/+/*
  return
    @name:x:/../*/_dn/#/*/name?value
apply:x:/../*/_out
  src:x:/../*/_people/*
  template
    {@eval}:x:/../*/_exe
_out
```

#### Warning!

The *[apply]* Active Event is extremely powerful. But this power, comes with a cost, which is that it is very easy to create extremely 
difficult to understand code, unless you are careful. Understanding how *[apply]* works, also requires some very good visualization skills,
to understand what happens, and when it happens.

### [fetch], evaluating lambda and fetching the requested result

The *[fetch]* Active Event, allows you to declare a piece of lambda, have it evaluated, and fetch some parts of its result into the value
of your *[fetch]* node. This is highly useful when you for instance have a *[set]* of *[if]* invocation, where you have a piece of code,
that is to be evaluated, for then to use one or more nodes deep inside of the result of your lambda evaluation, being used as the source 
for your *[if]* or *[set]* invocation. Consider the following code for instance.

```
_exe
  return
    person
      name:Thomas
      surname:Hansen
fetch:x:/0/0/*/surname?value
  eval:x:/../*/_exe
```

After evaluation of the above code, the value of *[fetch]* will be "Hansen". This allows you to "pre-fetch" one or more node's values, which
again plugs perfectly into concepts such as *[if]* and similar Active Events, expecting a node's value, and not a complete node hierarchy.

Consider this for instance, where we have injected an if inbetween our root node and our *[fetch]* node.

```
_exe
  return
    person
      name:Thomas
      surname:Hansen
if
  fetch:x:/0/0/*/surname?value
    eval:x:/../*/_exe
  =:Hansen
  sys42.confirm-window
    _header:Are you my brother?
    _body:Looks like we have the same surnames ...
```

What happens in the above lambda, is that first the *[if]* Active Event is invoked. Since our if node does not have a value, its first
child is considered to be "what is compared on our left hand side" parts. The logic of *[if]* is that if there's an Active Event invocation
used to retrieve the left-hand-side, then this Active Event is onvoked before the two sides are compared for equality, as we're doing in the
above example. This raises our *[fetch]* Active Event.

The *[fetch]* Active Event again, will evaluate its children, almost like an *[eval]* invocation would, with one crucial difference, which is
that after evaluation of its block, the expression in the value of *[fetch]* will be evaluated, and the results of that expression, will become
the value of *[fetch]*. After the fetch has been evaluated, the comparison in the *[if]* occurs. Since the value of fetch now is "Hansen", 
and this value will be compared against the constant of "Hansen", our evaluation yields true, and if allows branching of evaluation to enter
into its lambda block, evaluating our *[sys42.confirm-window]* Active Event.

Intelligently using *[fetch]*, allows you to nest logical pieces of code, in a more logical way, the same way you would using functions and 
methods in traditional programming languages.

Since *[fetch]* will put the results of its expression into its own value after evaluation, you can also use it in combination with
for instance *[add]* and the *[insert-xxx]* Active Events. The following code illustrates this.

```
_exe
  return
    person
      name:Thomas
      surname:Hansen
      address:Foo str. 24
_result
add:x:/../*/_result
  fetch:x:/0/0/*(!/address)
    eval:x:/../*/_exe
```

### [sort]ing your nodes

In p5.lambda there is an Active Event which allows you to sort your nodes, and supply your own "sort callback". You callback will be invoked
with an *[_lhs]* and an *[_rhs]* node, asking you to determine which node comes "before the other" of these two given arguments. Both 
the *[_lhs]* and the *[_rhs]* node passed into your lambda block, will be passed by reference, allowing you to traverse the tree, both up and
down from the nodes you wish to sort on.

Imagine the following code, and let us sort them according to their names.

```
_data
  person:1
    name:Thomas Hansen
  person:2
    name:John Doe
  person:3
    name:Abraham Lincoln
sort:x:/../*/_data/*
  if:x:/../*/_lhs/#/*/name?value
    <:x:/../*/_rhs/#/*/name?value
    return:int:-1
  else-if:x:/../*/_lhs/#/*/name?value
    >:x:/../*/_rhs/#/*/name?value
    return:int:1
  else
    return:int:0
```

After evaluating the above p5.lambda, "Abraham Lincoln" will be the first child beneath *[sort]*. Then you will find "John Doe", before
finally "Thomas Hansen". This is the alphabetical sort order of them, according to their names.

Notice that your *[sort]* callback expects you to return either -1, 1 or 0. -1 for those cases when *[_lhs]* should come before *[_rhs]*,
1 for vice versa, and 0 for when the nodes are supposed to be handled as equals.

Also notice that *[sort]* does not change the original node-set, but handles it immutable, and returns the sorted version as children of itself,
after evaluation.

After evaluation of the above code for instance, your resulting node-set will look like this.

```
_data
  person:1
    name:Thomas Hansen
  person:2
    name:John Doe
  person:3
    name:Abraham Lincoln
sort
  person:3
    name:Abraham Lincoln
  person:2
    name:John Doe
  person:1
    name:Thomas Hansen
```

Notice how your supplied callback lambda object is now entirely gone, and only the sorted nodes are left in your node-tree.

Notice also that providing a lambda block to *[sort]* is optional. If you do not, then the default sorting will be used. The "default 
sorting", sorts ascending by value of your nodes. If you wish to sort descending, you can use the alias of *[sort-desc]* which
your *[sort]* Active Event implements.

## Ninja tricks

Below you can find some "tricks" with p5.lambda, helping you achieve some special thing, maybe not completely intuitive unless explained.

### Having a relative [src] node with Active Event sources

Sometimes you wish to either *[add]*, *[set]* or use one of the *[insert-xxx]* Active Events, with a "relative source", which is relative
to the destination. This can actually easily be achieved, using one of the p5.lambda "Ninja tricks" with a little help from our *[eval]*
friend.

Imagine you have a node list, containing first name and last names. Then you wish to create a third node, which is the last name combined
with the first name, for some reasons. Let's show that with some code.

```
_data
  person
    first-name:Thomas
    last-name:Hansen
  person
    first-name:John
    last-name:Doe
add:x:/../*/_data/*
  eval
    eval-x:x:/+
    return:"full-name:{0}, {1}"
      :x:/../*/_dn/#/*/last-name?value
      :x:/../*/_dn/#/*/first-name?value
```

After evaluating the above lambda block, the output will look like this.

```
_data
  person
    first-name:Thomas
    last-name:Hansen
    full-name:Hansen, Thomas
  person
    first-name:John
    last-name:Doe
    full-name:Doe, John
/* ... rest of code ... */
```

The above example allows us to create a "relative source" for our *[set]*, *[add]* and *[insert-xxx]* invocations. Where the source 
is "relative" to the destination of our operation.

### Parametrizing your expressions

Sometimes you do not know the exact expression during runtime, but want to parametrize your expressions, according to some argument passed
into your code. Imagine something like this for instance, which is probably a relatively common use-case when consuming Phosphorus Five.

```
_parameter:last
_new-value:Doe
_data
  person
    first:Thomas
    last:Hansen
set:x:/../*/_data/*/*/{0}?value
  :x:/../*/_parameter?value
  src:x:/../*/_new-value?value
```

In the above code, we do not know which node beneath our *[person]* is supposed to be updated, but the actual node is passed in as a 
parameter through the *[_parameter]* node's value. By creating a formatted lazy evaluated expression, which takes the value of 
our *[_parameter]* node as an argument, we can deduct exactly which node beneath *[person]* is supposed to be updated to runtime.

Now of course, in the above example, the *[_parameter]* node is statically declared, but it doesn't matter if it is passed into a lambda
object by value, or used as a constant, which is shown in the above code.

To understand how formatting expressions work, check out the documentation for the "p5.exp" project.


