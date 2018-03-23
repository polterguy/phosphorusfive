## Lambda Expressions

**Notice**, the next two chapters are significantly more theoretical in nature than the previous chapters. If
it becomes boring or difficult to understand, feel free to skip this chapter, and the next chapter, and return
to these chapters later as your understanding and hunger for more has increased.

Lambda expressions are what truly makes Hyperlambda unique. Hyperlambda is nothing like your traditional
programming language. For instance, Hyperlambda doesn't even contain _"variables"_. This is because everything
is changeable, and potentially a variable - Including Hyperlambda execution instructions. This allows you to
create lambda objects, that change their execution trees, during their execution. This opens up a whole new
way of thinking in regards to programming, and does not, as far as I know, exist in any other programming
languages out there. Basically, every time you want to change something in Hyperlambda, you'll need to
use a lambda expression.

### An overview of lambda expressions

A lambda expression is declared with the type declaration of `:x:`. If you wish to create a lambda expression,
you will have to make sure your node containing your expression, resembles something like the following;
`_foo:x:/iterator1/iterator2/etc...` - The `:x:` parts, makes sure the Hyperlambda parser, understands that
the value of the previously defined **[\_foo]** node is an expression.

The expression itself, can contain 3 different segments, all of which are optional.

* Zero or more _"iterators"_
* An expression's type declaration
* A type conversion

All of the above segments are optional, and the shortest possible legal expression you can create, is in fact
completely empty `_foo:x:`. An empty expression like this, is often referred to as the _"identity expression"_,
and (almost) always returns the node where it is declared as a value. Hence, the expression in this code
`_bar:x:` will yield the **[\_bar]** node itself.

The iterators of your expressions are said to be _"left associative"_, because they are evaluated in order
of appearance, from left to right. Hence, you start out with the identity node, and apply zero or more
iterators to it, to retrieve whatever result you are interested in, _relative to the identity node_. There
are many different types of iterators, and in theory, they might even vary from implementation to
implementation. However, the most common ones, are listed in one of our appendixes at the end of this book.

Each iterator reacts upon the results of its previous iterator, starting from left to right. Whenever an
iterator yields a _"null result"_, the rest of the expression is discarded, and the expression as a whole
will yield a _"null result"_. Each iterator starts out with a forward-slash `/`. The first iterator in
your expression, starts out with the _"identity node"_ as its initial result set. This implies that
the expression will return the results of its evaluation, relative to its _"identity node"_.

Below is a piece of Hyperlambda, which demonstrates the construct. Notice, at this time, it might be
beneficial for you to evaluate the next example in _"Hypereval"_. Hypereval is a module for Phosphorus Five,
which allows you to execute Hyperlambda in _"immediate"_ mode, and see the results of your evaluation.
Make sure you open up _"Hypereval"_ by clicking the _"lightning"_ button at the top of Hyper IDE for instance,
and click the _"eye"_ button beneath Hypereval, to enable the _"output"_ view. Then paste in the code below into
Hypereval's CodeMirror instance, and click the _"flash"_ button beneath Hypereval, which will evaluate your
Hyperlambda.

```hyperlambda
_foo
set:x:/@_foo?value
  src:Jo dude
```

The above code is using the **[set]** Active Event, to change the `?value` of the first _"elder relative node"_,
having the name of _"\_foo"_, to become _"Jo dude"_. After execution of the above Hyperlambda, your lambda
object will have changed, and turned into the following;

```hyperlambda
_foo:Jo dude
set:x:/@_foo?value
  src:Jo dude
```

Notice how the value of **[\_foo]** changed.

### Your expression's type declaration

All of your expressions have a type declaration. If omitted, a type of `?node` will be assumed. The type
declaration, informs the expression engine, which part of your resulting node-set you are interested in
retrieving. There are four different possible type declarations for your expressions.

* `?node` - The nodes' themselves in their entirety
* `?value` - The nodes' values
* `?name` - The nodes' names
* `?count` - The number of nodes your result set contains

If you changed the above Hyperlambda, such that it had a `?name` type declaration for its expression,
it would result in the following.

```hyperlambda
Jo dude
set:x:/@_foo?name
  src:Jo dude
```

Using **[set]**, you can also change the entire node, by using `?node` as your expression's type declaration.
The node itself is the default type declaration, if you do not supply an explicit type declaration - Hence
you don't need to supply the `?node` type declaration. Below is an example.

```hyperlambda
_foo
set:x:/@_foo
  src:"_bar:howdy"
```

Notice, the **[src]** node's value above, will be automatically converted into a node, since the destination
for our **[set]** is a node. In general, P5 will automatically convert between types, as it needs to. Hence,
the above src, will actually be handled as an inline Hyperlambda piece of string, and converted to lambda,
before applied to its destination. Notice also in the above code, that we had to explicitly wrap our
`_bar:howdy` value inside double quotes. This is because it contains a colon `:` in its value. If we hadn't
done this, the Hyperlambda parser would assume that `_bar` was a type declaration for the src node's value.

An expression of type `?count` is read only, and cannot be used as a destination for a set invocation.
It can however be used as the src. Try the following code to see an example of this.

```hyperlambda
_foo
set:x:/@_foo?value
  src:x:/../*?count
```

### Some common iterators

You could probably get away with understanding only a handful of iterators, and never bother your mind with
the boolean algebraic parts of expressions - And still be able to create anything you wish to create using P5.
The most common iterators, you probably should at least learn, are listed below.

* `/..` - Returns the root node of your lambda object
* `/*` - Returns all children nodes of the previous result set
* `/**` - Returns all descendants of the previous result set
* `/xxx` - Named nodes, filtering away anything not matching the specified *"xxx"* name
* `/n` - Numbered child node, returning the *n'th* child of the previous result set
* `/=xxx` - Nodes having the specified *"xxx"* value
* `/-` - Elder sibling iterator
* `/+` - Younger sibling iterator

In addition to the above mentioned iterators, possibly the most important iterator, is the
_"named elder relative"_ iterator. This iterator starts out with an `@`, for then to contain the name of
the node you wish to retrieve. It is also sometimes called _"variable scoped iterator"_, since it almost
works like variable references in traditional programming languages. This iterator, will look for the first
node, amongst its elder sibling nodes, matching the given name. If it does not find any nodes matching the
name given, it will traverse upwards in its ancestor node hierarchy, and yield the first node matching the
specified name. It will repeat this process, until either a match is found, or the expression yields a 
_"null result"_.

Think of this iterator as an easy way to retrieve the first node, matching the specified name, within the
_"scope"_ of your currently executed lambda object. Below is an example of its use.

```hyperlambda
_foo
_bar
  _foo
set:x:/@_foo?value
  src:SUCCESS
_foo
```

Notice, after evaluating the above Hyperlambda, only the first **[\_foo]** node will have its value changed.
This is because the second \_foo, inside of our **[\_bar]** node, is not an elder sibling, or direct elder
relative in any ways, of the identity node of **[set]**. Hence, it is not found _"within the scope"_ of
our identity node, where our expression starts looking for a match.

If you tried something like the following though, only the last \_foo node would have its value changed.
This is because the _"named elder relative"_ iterator, will stop iterating, once it finds its first match.
This iterator always returns exactly one node, or a null result if it cannot find a match.

```hyperlambda
_foo
_foo
set:x:/@_foo?value
  src:SUCCESS
```

The named elder relative iterator, is arguably the closest you come in P5 to something allowing you to
reference nodes like _"variables"_.

To create a mental model for understanding lambda expressions, it might be useful to perceive them as exactly
what they are. Think of them like reusable enumerators, that can be chained together, filtering your result
set, retrieving sub portions of your original lambda. They're kind of like the equivalent of LINQ for
Phosphorus Five.

### Converting your expression's result

Optionally, you can convert the results of an expression, by appending a ".", and the type you wish to convert
the results of your expression into. Imagine the following code, that copies the value from **[\_foo]**,
and puts it into **[\_bar]**'s value, but converting it from a string to an integer.

```hyperlambda
_foo:5
_bar
set:x:/@_bar?value
  src:x:/@_foo?value.int
```

The most common types are listed below;

* `.bool` - Boolean, *"true"* or *"false"*
* `.int` - Integer numbers, e.g. *"42"*
* `.double` - Real numbers, 64 bits floating point, e.g. _"42.57"_
* `.string` - String, which is the default type in P5
* `.node` - Hyperlambda, with a single root node

In our appendixes, you can find a complete reference, of all the built-in types in P5. The type system for
Hyperlambda is extendible though, and you can easily create your own types, by adding a couple of Active
Events with special names and namespaces into your application pool.

### Creating a mental model for expressions

One way of realising what lambda expressions are, is to imagine them as the _"tree version of SQL"_. Where
SQL allows you to extract two dimensional tables and data-sets, lambda expressions allows you to extract
n dimensional relational sub-trees. If you have some experience with XPath, they might come natural to you.

### The scientific definition

The correct scientific name for lambda expressions is -
_"Hyperdimensional boolean algebraic graph object expressions"_, because they allow you to use boolean algebra,
to create sub graph objects, out of another graph object. This results in creating a _"hyperplane"_ through
your graph objects, which again results in retrieving a sub-portion of your tree structures. If you imagine your
graph objects as an n dimensional tree structure, then lambda expressions allows you to create n+1 dimensions
through these graph objects. These expressions extracts a sub-portion of your tree, and yields its results,
as a new tree. Each expression, creates a new _"dimension"_ through your tree. Such dimensions are often
referred to as _"hyperplanes"_. To make this easier to visualise, think of lambda expressions as
_"SQL for tree structures"_. A lambda expression creates a new tree structure, from another source tree
structure. This resulting tree structure can have zero or more nodes in it.

For the record, __the above definition is completely irrelevant for your understanding of expressions__,
but necessary to create a formalized scientific definition. They're actually quite easily understood,
once you dive into them. Think of them as _"folder paths"_ or XPath, if it helps.

### Wrapping up

In this chapter we looked at lambda expressions, and how they can reference nodes in your lambda object.
If this chapter didn't make much sense, then don't worry, since these parts of Phosphorus Five will
be intuitively understood as your knowledge of the platform increases.

These parts of Phosphorus Five tends to be the most difficult parts to understand. However, as you get a
more _"hands on approach"_ with expressions, they seem to fall into place more naturally. Don't despair
if you're more confused now, than you were before you started reading this chapter ... ;)

The next chapter will also be fairly theoretical in nature. If it doesn't make sense, simply scan it,
and return back to it, as you have created some more code.
