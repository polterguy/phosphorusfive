Hyperdimensional boolean algebraic graph expressions
========

Puuh, that name was a mouthful. However, really, for most practical concerns, if you know
some XPath from before, you'll pick up P5 expressions in an hour or two. An expression
is type-declared as `:x:`, for then to have zero or more iterators, optionally having
a type declaration at the end, optionally ending with a conversion of its result.

Example given.

```
_data
  foo:bar
set:x:/../*/_data/*/foo?value
  src:New Value
```

In the above example `/../*/_data/*/foo?value` is an expression, and `:x:`
declares it as such for the type engine of P5.

It ends with the type declaration `?value`, which informs the expression engine, 
that we're interested in the values of whatever the expression yields as its result.

## An expression's type declaration

An expression can retreieve 4 basic values.

* Node - `?node` - The node itself, also the default value if no type is explicitly declared for your expression
* Name - `?name` - The name part of your nodes
* Value - `?value` - The value part of your nodes
* Count - `?count` - Number of matches for your expression, returns a single integer value

In addition, an expression can be type-converted. If you are for instance requesting a node's value, that is of type string, you can convert
the result into an integer, by appending `.int` after the expression, such as the following example illustrates.

```
_data:string:5
set:x:?value
  src:x:/../*/_data?value.int
```

For the record, an expression without any iterators, is the identity expression, yielding the current node, which for the above example, and its 
destination, means it will update the *[set]* node's value itself. The identity expression, when retreiving the identity node, is actually written 
like this; `:x:`, having no iterators, and no type declaration. The identity expression, which all expressions starts out with, will retrieve the 
this node for most cases. For instance, this code, will delete its own node, since our *[set]* has no source, and it uses the identity expression 
as its destination.

```
set:x:
```

The above code will not produce any results, since its only instruction, is deleting the node containing its only instruction.

If you wish to return the number of nodes having the name of for instance *"foo"* from within
your *[_data]* segment, you can use syntax such as the following.

```
_data
  foo:value 1
  foo:value 2
set:x:?value
  src:x:/../*/_data/*/foo?count
```

After evaluation of the above Hyperlambda, the value of the *[set]* node will be `:int:2`, which
signifies that it holds an integer value, and its value is "2".

Expressions consists of three basic parts

* `:x:` - Signifying it is an expression type
* Zero or more iterators, separated by `/`
* An optional type declaration, being any of `?name`, `?count`, `?value` or `?node`

If the type declaration is omitted, the type declaration is assumed to be `?node`.

Each iterator is separated by a slash `/`, resembling the syntax of XPath. An iterator will 
react upon the results from its previous iterator, mening they're piped together, left associatively, to create a combined result.
Left associately means that the expression engine evaluate them from the left to the right.

## Iterator types

There are 17 basic iterators, each yielding different results

* `/.` - Parent iterator, extracts parent nodes from previous result set
* `/..` - Root node iterator, extracts the root node from previous result set
* `/..xxx` - Named ancestor iterator, extracts all ancestor with the specified (xxx) name
* `/@xxx` - Named elder relative iterator, extracts first elder relative with the specified (xxx) name
* `/*` - Children iterator, extracts all children of previous result set
* `/**` - Descendants iterator, extracts all descendents of previous result set (children and childrens' children, etc)
* `/xxx` - Named iterator, extracts all nodes with the specified "xxx" name
* `/=xxx` - Value iterator, extracts all nodes with the specified "xxx" value
* `/-n` - Younger sibling iterator, retreieves the younger sibling from previous result set. "n" must be an integer, if supplied. If no "n" is supplied, the value of "1" will be assumed
* `/+n` - Elder sibling iterator, opposite of above
* `/++` - Older iterator, yields all older nodes
* `/--` - Younger iterator, yields all younger nodes
* `/n` - Numbered child iterator, extracts the n'th child from the previous result set, where n must be an integer value
* `/%n` - Modulo iterator, extracting every node matching the given (n) modulo, where n must be an integer number
* `/[n1, n2]` - Range iterator, extracts a range of values from previous result set, where n1 and n2 must be numbers, n2 larger than n1
* `/$` - Distinct name iterator, extracts all nodes from previous result set, excluding nodes with duplicate names
* `/=$` -  Distinct value iterator, extracts all nodes from previous result set, excluding nodes with duplicate values
* `/#` - Reference iterator, casts/converts the previous nodes' values, into a node, yielding that node back to caller
* `/<` - Left shift iterator, lefts shifts the nodes from the previous result set
* `/>` - Right shift iterator, opposite of above

## Examples

To understand expressions, realize that they're actually just "declared iterators", or "loops". They are acting upon the `Node` class, which
you can find [here](/core/p5.core/Node.cs). The Node class again, is really just a name/value/children collection, with some helper methods. It is
also the foundation for Hyperlambda. Think of the Node as the "DOM of Hyperlampda". (Document Object Model)

With that knowledge arming us, let's look at some examples of how to use expressions.

### Extracting the "root node" from a node-set

The root node iterator `/..` will always yield one result. This single node will be the "root node".

Every time you construct a lambda object from Hyperlambda, what you are actually doing, is evaluating a single node, 
using [eval](/plugins/p5.lambda#eval-the-heart-of-p5lambda). This node is the equivalent of the document node from XML, 
except in Hyperlambda, it is invisible.

What *[eval]* does, is to simply evaluate all of this node's children nodes, raising them as Active Events, sequentially in order from top 
to bottom.

The root node iterator, `/..`, simply yields back this node, once, and only once. Meaning, regardless of how many nodes you have in your previous
result-set - After having evaluated the root iterator, there will be exactly one left. Unless one of the previous iterators yielded nothing, at which
point also the root iterator will yield nothing, since iterators are "left associative", reacting upon previous iterators, and once there are no
more results, the iteration stops, and a "null result" is returned.

Most expressions we use in this example, will start out with the root iterator. Have this explanation in the back of your mind, as you read through
the rest of this documentation.

### Extracting nodes with a specific name from a node-set

Imagine we have a p5.lambda structure (node set), and we wish to change the value of each node that has the name of "foo" to "New value".
Then we could do something like this.

```
_data
  foo:Old value 1
  not-foo:Old value 2
  foo:Old value 3
set:x:/../*/_data/*/foo?value
  src:New value
```

The above Hyperlambda will retreieve all nodes that have the name of "foo", and set their values to "New value". This
is because of the iterator `/foo` parts above, in addition to the `_data` parts.

Our first iterator above is the root node iterator `/..`, which will extract the "invisible root node". Then our next iterator is the 
children iterator `/*`, which will be explained later, which yields *[_data]* and *[set]*. Then comes another named iterator `/_data`, 
excluding everything not having the name of "_data". Afterwards, another children iterator `/*`, and another named iterator `/_foo`. 
The resulting node-set, will consist of both nodes inside our *[_data]* segment, having the name of _"foo"_.

In total, there are 5 iterators in the above example.

Let's walk through the above expression, once more

* The `/..` iterator retrieves the invisible root node
* The `/*` iterator retrieves all children of the previous node-set, resulting in *[_data]* and *[set]*
* The `/_data` iterator excludes all nodes having another name but "_data", resulting on only *[_data]* as the current result-set
* The `/*` iterator extracts all children nodes of *[_data]*
* Then finally, the `/foo` excludes all nodes not having the name of "foo", resulting in the 1st and third child of *[_data]* being our result-set.

After evaluation of the above Hyperlambda, your resulting node-set will look like this. Notice the values of your *[foo]* nodes.

```
_data
  foo:New value
  not-foo:Old value 2
  foo:New value
set:x:/../*/_data/*/foo?value
  src:New value
```

Notice how the *[not-foo]* node was _not_ updated! If you wish to also have the *[not-foo]* node updated, you could accomplish
this by making sure the "name iterator" looks for a "contains comparison", by prepending a tilde "~" in front of the value of the iterator.

```
_data
  foo:New value
  not-foo:Old value 2
  foo:New value
set:x:/../*/_data/*/~foo?value
  src:New value
```

The above will look for any nodes within the *[_data]* segement of your p5.lambda object, having a name, _containing_
the value of _"foo"_. Meaning, it will _also_ match the *[not-foo]* node. A tilde `~`, signifies a "like" comparison, 
and can be used in front of the value of both a "named iterator", and a "valued iterator".

### Extracting all children of a node-set

If we instead want to update _all_ children of the *[_data]* node above, we could use the *children iterator*, and drop the last name iterator.

```
_data
  foo:Old value 1
  not-foo:Old value 2
  foo:Old value 3
set:x:/../*/_data/*?value
  src:New value
```

The above Hyperlambda, will extract all children of the *[_data]* node, and set their values to "New value". Resulting
in the following p5.lambda structure as its result. 

```
_data
  foo:New value
  not-foo:New value
  foo:New value
set:x:/../*/_data/*?value
  src:New value
```

Since the *[set]* Active Event can handle multiple destinations, it will set the values of all children nodes of our *[_data]* node to the constant
of "New value", which was the constant source provided as a *[src]* argument to *[set]*.

The end result being that both the two *[foo]* nodes, in addition to the *[not-foo]* node, have their values updated.

### Extracting a specific range of nodes

Imagine you wish to extract all node from the 2nd to the 3rd node from within this data segment

```
_data
  foo1:bar1
  foo2:bar2
  foo3:bar3
  foo4:bar4
```

The above task could be accomplished with something like this for instance

```
_data
  foo1:bar1
  foo2:bar2
  foo3:bar3
  foo4:bar4
set:x:/../*/_data/*/[1,3]?value
  src:New value
```

After evaluation of the above Hyperlambda, your node-set would look like this

```
_data
  foo1:bar1
  foo2:New value
  foo3:New value
  foo4:bar4
set:x:/../*/_data/*/[1,3]?value
  src:New value
```

This is because the `/*` node returns all children of the *[_data]* segment, but the iterator after
our last children iterator, will make sure only the nodes from (including) the 1st node, to (not including) the 3rd
node will be returned. The range iterator returns nodes according to [n1..n2>, to use math terminology.

Both the start and the end of your range iterator are optional arguments to it, and if not supplied, 
will have the default values of "zeroth node" and "last node", whatever they are. For instance, to change the
value of all nodes, except the zeroth child node of the above *[_data]* segment, you could use something like
this

```
_data
  foo1:bar1
  foo2:bar2
  foo3:bar3
  foo4:bar4
set:x:/../*/_data/*/[1,]?value
  src:New value
```

Or to change all nodes, up until (but not including) the 3rd node, you could use something like this

```
_data
  foo1:bar1
  foo2:bar2
  foo3:bar3
  foo4:bar4
set:x:/../*/_data/*/[,2]?value
  src:New value
```

Both the start value and the end value of the "range iterator" are zero-indexed, meaning the value of your 1st node is actually "0", 
the second node "1", and so on ...

### Extracting the n'th child of a node

Imagine you only wish to extract the 2nd child node of some node-set. This could be accomplished using a "numbered child iterator".

```
_data
  foo1:bar1
  foo2:bar2
  foo3:bar3
set:x:/../*/_data/*/1?value
  src:New value
```

After evaluation of the above p5.lambda structure, your node-set will look like this

```
_data
  foo1:bar1
  foo2:New value
  foo3:bar3
set:x:/../*/_data/*/1?value
  src:New value
```

Notice that the only difference between a "numbered child" iterator, and a "named iterator", is
that the numbered child iterator consists of only integer numbers, meaning [0..9] characters.

This creates a problem for us, if we wish to retrieve a node who's _name_ is *[1]* for instance. Imagine
for instance the scenario below.

```
_data
  1:bar1
  foo2:bar2
  foo3:bar3
set:x:/../*/_data/*/1?value
  src:New value
```

The above p5.lambda would perfectly evaluate. However, the results would probably not be what you
wanted, since the "foo2" node would have its value updated, and not the *[1]* node. If you have nodes
with integer names, such as above, and you wish to explicitly name these nodes, you need to escape your
name, using a back-slash, like code below illustrates.

```
_data
  1:bar1
  foo2:bar2
  foo3:bar3
set:x:/../*/_data/*/\1?value
  src:New value
```

Notice the back slash in front of our "1" above. You can also escape other characters, having nodes with really funny names, such as "/" etc. 
Imagine this Hyperlambda for instance.

```
_data
  *:bar1
  foo2:bar2
  foo3:bar3
set:x:/../*/_data/*/\*?value
  src:New value
```

The above Hyperlambda would change the value of the node with the name of "*".

### Extracting the first elder relative node

This is possibly one of the most useful iterators in P5. First of all, it allows you to refactor
your node hierarchy if you use it, later, without the same probability of you breaking your expressions.
Secondly, it tends to replace a whole range of iterators due to its syntax and logic.

Basically, it allows you to retrieve the first node with a specified name, from the dataset of previous siblings, 
and/or parent nodes, until it finds a match, or yields a null result if no match is found. Example is given below.

```
_foo
_foo
_bar
  _foo
set:x:/@_foo?value
  src:CHANGED!
```

The above code will result in the following.

```
_foo
_foo:CHANGED!
_bar
  _foo
set:x:/@_foo?value
  src:CHANGED!
```

Notice how the iterator skips the second *[_foo]*, inside of our *[_bar]*. This is because it is neither an elder
sibling, nor a parent of *[set]*. Notice also how only the second *[_foo]* is changed, and not the first. This iterator
will always yield zero or one result, and only yield the first node matching the specified name.

Consider this code.

```
_foo
_bar
if:true
  if:true
    set:x:/@_foo?value
      src:CHANGED!
```

What happens above, is that the expression first evaluates our second *[if]*, which doesn't match the specified name of "_foo",
then it moves on the its parent, to the first *[if]*. Neither this is a match, so it moves on to the *[_bar]*, which doesn't
yield a match either. Therefor it moves on to *[_foo]*, which yields a match, and the result becomes.

```
_foo:CHANGED!
_bar
if:bool:true
  if:bool:true
    set:x:/@_foo?value
      src:CHANGED!
```

Logically, the named elder relative iterator, is the closest you come to easily refer to "variables" in P5. Simply because
it will look for anything it has previously seen, which is a "named elder relative" you might think. Not considering parts
of the tree, which does not also include an entire subset of the tree that the expression's node belongs to.

### Extracting all younger or older nodes

These iterators are allso pretty useful, in when looking for a specific node, either forward or backwards in your lambda 
hierarchy. In isolation, they might seem not so very interesting, but when combining them with for instance a named iterator,
or a value iterator, you can easily find a specific node either foward in your hierarchy (older iterator) or backwards (younger iterator).
Consider this showing the "younger iterator".

```
_foo
_bar
_howdy
set:x:/--?value
  src:SUCCESS!
_foo2
_bar2
_howdy2
```

The above code will result in the following lambda.

```
_foo:SUCCESS!
_bar:SUCCESS!
_howdy:SUCCESS!
set:x:/--?value
  src:SUCCESS!
_foo2
_bar2
_howdy2
```

Then consider this code, showing the "older iterator".

```
_foo
_bar
_howdy
set:x:/++?value
  src:SUCCESS!
_foo2
_bar2
_howdy2
```

Which results in the following results.

```
_foo
_bar
_howdy
set:x:/++?value
  src:SUCCESS!
_foo2:SUCCESS!
_bar2:SUCCESS!
_howdy2:SUCCESS!
```

Often when you create Hyperlambda, you want to parametrize some node, before you invoke it, as an Active Event. When using
the older or younger iterators, you can easily do so, and still allow for refactoring or changing your lambda structure later,
since you do not depend upon a specific location for your nodes.

Imagine this for instance.

```
set:x:/++/innerValue?value
  src:Foo bar
create-literal-widget
  innerValue
```

If you later want to move your *[create-literal-widget]* invocation, to for instance an *[eval]* invocation or something, then
you do not need to change your expression in your *[set]* invocation. Consider this.

```
set:x:/++/innerValue?value
  src:Foo bar
_eval
  create-literal-widget
    innerValue
eval:x:/-
```

Notice, we did not have to update our original *[set]* destination expression, it still works.

This give you more flexibility when refactoring, at the cost though. Realize though, that if you later create 
another *[innerValue]*, behind your original *[set]* invocation, than also this innerValue node will have its value changed.
Be alert for these types of scenarios when using these iterators!

### Extracting the previous or next sibling node

A sibling node, is a node which belongs to the same parent node. Sometimes you want to retrieve the previous node, or the next node. These nodes
are often referred to as "younger sibling nodes", and "elder sibling nodes". For such cases you can use the `/-` (younger sibling) or the `/+`
(elder sibling) iterators. Both of these iterators, optioally takes an integer argument, being the "offset". If no offset is specified, it will 
default to "1". Below is an example of the "younger sibling" iterator in use.

```
_data
  foo1:bar1
  foo2:bar2
  foo3:bar3
  foo4:bar4
set:x:/../*/_data/*/foo3/-?value
  src:UPDATED!
```

This would result in the following.

```
_data
  foo1:bar1
  foo2:UPDATED!
  foo3:bar3
  foo4:bar4
set:x:/../*/_data/*/foo3/-?value
  src:UPDATED!
```

To understand how the "offset" parts work, imagine if you replaced the expression above with the following `/../*/_data/*/foo3/-2?value`. Then this
would yield the following result.

```
_data
  foo1:UPDATED!
  foo2:bar2
  foo3:bar3
  foo4:bar4
set:x:/../*/_data/*/foo3/-2?value
  src:UPDATED!
```

If you append the "elder sibling" iterator, at the end of your expression from above, like this; `/../*/_data/*/foo3/-2/+2?value` - Then you're back
where you started, resulting in the following result.

```
_data
  foo1:bar1
  foo2:bar2
  foo3:UPDATED!
  foo4:bar4
set:x:/../*/_data/*/foo3/-2/+2?value
  src:UPDATED!
```

### Changing every even child node (modulo iterator)

Imagine you have some Hyperlambda, where you wish for only the even values to be updated. This could easily be
accomplished using the "modulo iterator". Imagine the following Hyperlambda

```
_data
  foo0:bar0
  foo1:bar1
  foo2:bar2
  foo3:bar3
set:x:/../*/_data/*/%2?value
  src:New value
```

After evaluating the above Hyperlambda, your node-structure would look like this.

```
_data
  foo0:New value
  foo1:bar1
  foo2:New value
  foo3:bar3
set:x:/../*/_data/*/%2?value
  src:New value
```

This is because only the nodes matching the specified modulo will be returned, due to the `/%2` iterator
at the end of the above expression.

### Extracting all even children

To extract all even iterators, you could for instance use the "left shift iterator", in combination with the modulo iterator, 
such as the code below demonstrates.

```
_data
  foo0:bar0
  foo1:bar1
  foo2:bar2
  foo3:bar3
set:x:/../*/_data/*/%2/<?value
  src:New value
```

This would instead change all "even children" of your *[_data]* segment.

### Extracting only the last child

Imagine you want to extract only the last child node of some hierarchy, and you do not
know how many children your segment have. This can easily be accomplished using the "younger sibling iterator",
since this iterator will "round trip" your nodes, extracting the last node, if you use it on the first child node
of some result-set. Imagine the following code.

```
_data
  foo0:bar0
  foo1:bar1
  foo2:bar2
  foo3:bar3
set:x:/../*/_data/0/-?value
  src:New value
```

What the above code will do, is to first retrieve the first child (zeroth child) of your *[_data]* segment.
Then it will ask for the "younger sibling of this node". Since this node does _NOT_ have any younger sibling,
it will "wrap around" to the last node, returning only the last children node of your *[_data]* segment.

Using this technique, you can easily retrieve the "last child" of some node-set.

### Extracting all unique node names and values

Imagine you want to extract all unique node names or values from some node set. This can be done using either the `/$`´or the `=$`
iterators. These iterators returns respectively, only the nodes having unique names, or the nodes having unique values. Example is given below.

```
_input
  foo1:value1
  foo1:value2
  foo2:value2
  foo2:value3
_res1
add:x:/-
  src:x:/../*/_input/*/$
_res2
add:x:/-
  src:x:/../*/_input/*/=$
```

The above code would produce the following result.

```
_input
  foo1:value1
  foo1:value2
  foo2:value2
  foo2:value3
_res1
  foo1:value1
  foo2:value2
add:x:/-
  src:x:/../*/_input/*/$
_res2
  foo1:value1
  foo1:value2
  foo2:value3
add:x:/-
  src:x:/../*/_input/*/=$
```

Our first *[add]* invocation, will only extract the children nodes from our *[_data]* node, that have unique names - Discarding every node, 
having a name, that matches the name of a node it has previously extracted. Our second invocation to *[add]*, does the same, except it only
adds nodes with unique values.

### Extracting a node from the value of a node

Sometimes you have a node, who's value itself, contains another node. This pattern is quite common, and is referred to as "reference nodes".
If you wish to retrieve this "inner node", in expressions, this can be done using the "reference iterator", which will automatically convert the value
of the iterated nodes to a node itself. Either by conversion, or by casting if possible. Imagine the following code.

```
_data:node:@"inner1
  inner2:old value
  inner3:old value"
set:x:/-/#/*/inner2?value
  src:UPDATED!
```

After evaluating the above code, your result will look like this.

```
_data:node:@"inner1
  inner2:UPDATED!
  inner3:old value"
set:x:/-/#/*/inner2?value
  src:UPDATED!
```

### Extracting all "descendant" nodes

The `/**` iterator, extracts all "descendants", meaning every children, and their children's children nodes, and so on inwards, for as deep as your
node hierarchy is. To illustrate with an example.

```
_data
  n1:old value
    n2:old value
    n3:old value
      n4:old value
        n5:old value
set:x:/../*/_data/**?value
  src:UPDATED!
```

After evaluation of the above code, you will get the following result.

```
_data
  n1:UPDATED!
    n2:UPDATED!
    n3:UPDATED!
      n4:UPDATED!
        n5:UPDATED!
set:x:/../*/_data/**?value
  src:UPDATED!
```

### Extracting a named ancestor node

The `/..` (root) iterator is quite useful for retrieving the root node of your lambda object. However, sometimes you rather want to traverse upwards
in your hierarchy, until you encounter a node, who's name you know, and not traverse all the way to the root node. For such cases, the "named ancestor"
node is quite useful. It looks like this `/..xxx`, where the "xxx" parts are replaced with whatever name you are looking for. An example is given below.

```
_data
  n1:old value
    n2:old value
    n3:old value
      n4:old value
        n5:old value
set:x:/../*/_data/**/n5/..n3?value
  src:UPDATED!
```

The above code will first extract all descendants of the *[_data]* node, then discard all nodes not having the name of "n5", before it finally extracts
that node's named ancestor, having the name of "n3". The result will look like this.

```
_data
  n1:old value
    n2:old value
    n3:UPDATED!
      n4:old value
        n5:old value
set:x:/../*/_data/**/n5/..n3?value
  src:UPDATED!
```

### Extracting the parent node

Sometimes you rather want to extract the current node-set's parent node(s). This is easily achieved with the `/.` iterator, which only return the 
direct parent(s), of whatever node-set you're currently iterating. Imagine this code.

```
_data
  no1:old value
    foo
  no2:old value
    foo
set:x:/-/**/foo/.?value
  src:UPDATED!
```

The above code will first retrieve all descendants of the *[_data]* node, then discard all nodes not having the name of "foo", before it finally 
retrieves all of the parent nodes to its previous result set. The result becomes like this.

```
_data
  no1:UPDATED!
    foo
  no2:UPDATED!
    foo
set:x:/-/**/foo/.?value
  src:UPDATED!
```

### Formatting expressions

In addition to p5.lambda expressions, you can also create a "formatting expression". This works similarly to how you format a 
string in C#, and is evaluated _BEFORE_ any p5.lambda expressions are evaluated. This allows you to create a p5.lambda expression,
where parts of it is substituted with some constant or dynamically fetched string, with either a constant value, or another expression, 
leading to some other node, value or name.

Imagine the following code.

```
_foo:foo2
_data
  foo1:howdy
  foo2:world
_output
set:x:/../*/_output?value
  src:x:/../*/_data/*/{0}?value
    :x:/../*/_foo?value
```

What occurs above, is that first the inner expression is evaluated. This is the node at the bottom of the above code, with noe name.
Then the `{0}` parts in the *[src]* expression is substituted with the evaluated return value of the outer expression, resulting in
that the [src] expression looks like this `src:x:/../*/_data/*/foo2?value`, since the inner expression leads to the *[_foo]* node's value,
which happens to be "foo2".

Formatting expressions substitute their braced integer values, with their corresponding child node's, having no name, and its "value", after
evaluating it, if it is an expression. You can also use a constant, instead of an expression as your nameless substitutions.

This allows you to create both normal string, and/or complex expressions, which is "parametrized", and changes according to how your lambda
object changes. Allowing you to pass in "parameters" to your expressions, in addition to that it allows you to concatenate strings of course.

### Boolean algebra on expressions

p5.exp support boolean algabra on your expressions. This allows you to use all the four boolean algebraic
operators to combine results. The four algebraic operators are as following

* `&` - AND
* `|` - OR
* `!` - NOT
* `^` - XOR

Combining these in your expressions, allows you to for instance return the "union" of two expressions. Imagine
wanting to retrieve all nodes have the value of either "jane" or "john" from the Hyperlambda below.

```
_data
  foo0:jane
  foo1:john
  foo2:bar2
  foo3:jane
```

Accomplish the above, could easily be done using an expression like below

```
_data
  foo0:jane
  foo1:john
  foo2:bar2
  foo3:jane
set:x:/../*/_data/*/=jane|/../*/_data/*/=john?value
  src:hansen
```

The above expression would use the `|` boolean algebraic operator, to "join" the results of the above
two expressions, separated by the pipe sign (|). This would result in the following code after evaluation.

```
_data
  foo0:hansen
  foo1:hansen
  foo2:bar2
  foo3:hansen
set:x:/../*/_data/*/=jane|/../*/_data/*/=john?value
  src:hansen
```

Then imagine returning only nodes having both the name of "jane", and the value of "doe" as below

```
_data
  jane:doe
  john:doe
  jane:hansen
  jane:doe
```

To accomplish the above task, you would have to use the AND boolean operator `&` such as below.

```
_data
  jane:doe
  john:doe
  jane:hansen
  jane:doe
set:x:/../*/_data/*/=jane&/../*/_data/*/doe?value
  src:UPDATED!
```

After evaluation, the above Hyperlambda would look like this

```
_data
  jane:UPDATED!
  john:doe
  jane:hansen
  jane:UPDATED!
set:x:/../*/_data/*/=jane&/../*/_data/*/doe?value
  src:UPDATED!
```

Notice how only the nodes matching _BOTH_ the name of "jane" AND the value of "doe" are updated above.

If you wish to return the ones having either _ONE_ of a name being "jane" or a value of "doe", this
could be accomplished using the XOR `^` boolean operator, such as below.

```
_data
  jane:doe
  john:doe
  jane:hansen
  jane:doe
set:x:/../*/_data/*/=jane^/../*/_data/*/doe?value
  src:UPDATED!
```

After evaluation of the above Hyperlambda, the result would look like this

```
_data
  jane:doe
  john:UPDATED!
  jane:UPDATED
  jane:doe
set:x:/../*/_data/*/=jane^/../*/_data/*/doe?value
  src:UPDATED!
```

Notice that the nodes matching _BOTH_ criteria is _NOT_ updated, but only the ones matching only _one_ of
the criteria in our XOR operands.

### Grouping expressions

Sometimes, especially when using boolean algebra in your expressions, your expressions might become very long 
and tedious to read. For such scenarios, you can "group" your expressions, to create "sub-expressions",
that reacts upon their "outer expressions". The OR statement we started out with in this section for instance, 
could be re-written to the following expressions, using "grouping" parantheses.

```
_data
  foo0:jane
  foo1:john
  foo2:bar2
  foo3:jane
set:x:/../*/_data/*(|/=jane|/=john)?value
  src:hansen
```

What the above syntax means, is basically that the two sub-expressions, inside of our parantheses, are reacting upon the expression 
_OUTSIDE_ of the paranthese themselves. Meaning, the outer expression becomes the "root" expression result-set, both of these two 
expressions reacts upon.

This allows you to create fairly complex expressions, reacting upon the results of their previous "outer expressions", yet still keep your expression
short and understandable.

### Extracting typed values

Sometimes you wish to create a value match for something that is not of type "string". This creates a problem, due to the
typing system of Phosphorus Five, and your p5.lambda structures, since the integer value of 5 does _NOT_ equal the string value of "5". 
This can easily be done though, since the value iterator supports types, the same way Hyperlambda itself supports types, by adding a `:my-type:`
in front of the value. Since however, the ":" is a special character in Hyperlambda, this means you'll have to first declare
your entire expression inside of a "string literal", the same way you'd create a string literal in C#. Example is given below.

```
_data
  foo0:int:5
  foo1:5
  foo2:5
  foo3:5
set:x:"/../*/_data/*/=:int:5?value"
  src:hansen
```

Notice that if you remove the ":int:" parts of your expression above, then the only node _NOT_ being updated,
is the one with the integer value of 5. You can use any type your installation of Phosphorus Five happens to support
in your expressions, including any special types you might have declared yourself. Notice also that since we're using
a colon (:) inside of the value of a node, we need to put the entire node inside of double quotes (").

Expressions, and iterators, can also be created spanning multiple lines, using the @"my-string" literal type of strings, 
such as below

```
_data
  foo0:int:5
  foo1:@"5
5"
  foo2:5
  foo3:5
set:x:@"/../*/_data/*/@""=5
5""?value"
  src:hansen
```

However, the above syntax doesn't exactly _increase_ the readability of your expressions, and you should be careful with
it, and try to avoid it altogether if you can.

For those rare occassions, when you really need it though, you can put your iterators inside of "string literals", and
as in the above example, even inside of @"multi line string literals". But as previously stated, do not do this, unless it
is absolutely necessary to do so, since it tends to dramatically reduce the readability of your code.

### Regular Expression iterators

Both the "name iterator" and the "value iterator" also support regular expression matches, which might be useful
for doing for instance case-insensitive matches, looking for occurrencies of both "thomas" and "Thomas" for instance. 
Imagine something like this for instance.

```
_data
  foo0:thomas
  foo1:Thomas
  foo2:john
  foo3:jane
set:x:@"/../*/_data/*/""=:regex:/thomas/i""?value"
  src:COOL GUY
```

Notice though that since the regular expression iterator starts and ends with a slash (/), the entire iterator
needs to be put inside of double quotes ("). In addition, since the expression as a whole therefor contains double quotes,
the expression as a whole needs to also be double-quoted, creating a fairly complex syntax for your expression, with 
double-quotes inside of double-quotes. You should in general term avoid regular expression iterators, due to among
other things the above problem. In addition, these iterators are much slower in execution than the native "p5 iterators",
due to the regular expression matching requiring potentially lots of CPU time, etc.

However, a regular expression iterator starts with a slash "/" and ends with a slash "/". After the ending slash, you
can supply regular expression options, which takes the following form

* `o` - Compiled regular expression
* `c` - Invariant culture
* `e` - Use ecma style syntax
* `i` - Ignore case
* `m` - Multiline
* `r` - Right to left evaluation
* `s` - Single line
* `w` - Ignore whitespace pattern
* `x` - Use explicit capture

These are the same options you can supply when you create a regular expression in .Net, and should be fairly well
documented at the MSDN website, if you wish to dive into these options. They can be combined the same way you combine regular expression options in
the .Net framrwork too.

## Warning!

The p5.exp expression engine is fairly powerful. It can also in very dense expressions, return a relatively rich 
result-set back to you. This power comes with a cost though, which is that if you go "berserk" in your expressions, then
the readability of your applications will be significantly reduced, and your apps might becomes more difficult to maintain,
since it contains too complex expressions.

Sometimes, it is better for readability to create a couple of more *[for-each]* loops, and *[if]* branchings, than
it is to go completely over board with the most complex expressions you can imagine.

Be careful with your expressions. The goal is not to create code making your app as a whole resemble "regular expressions".

The technical term for the p5.exp expressions are *"Hyperdimensional Boolean Algebraic Graph Object Expressions"*, for 
those interested in the mathematical theory behind them.

Using expressions, also requires some advanced visualization skills from the developer, since it creates a "hyperplane", 
through your tree structures. Which increases the complexity of your app, and reduces the ability to understand the code for
a beginner.

They are however, extremely powerful when it comes to extracting sub-sections of your tree structures, graph objects, which are
really just nodes.

## Consuming p5.exp from C# in your own code

p5.exp is extremely easy to incorporate in your own projects and/or Active Events. If you take a look at for instance the
p5.lambda project, which contains the ghist of the p5.lambda "non-programming language", you will see that it heavily
consumes the `XUtil` static methods, allowing for automatic traversion, and iteration, over your p5.lambda nodes.

You can easily create Active Event handlers that takes expressions as their input, using the same pattern in your own code.

Imagine you have some Active Event called *[foo]*, which should take an expression, leading to possibly multiple
nodes, where you should do something with these nodes. This could easily be accomplished with the following code.

```csharp
using p5.exp;
using p5.core;

namespace your_namespace
{
    public static class YourClass
    {
        [ActiveEvent (Name = "foo")]
        public static void foo (ApplicationContext context, ActiveEventArgs e)
        {
              foreach (var idx in XUtil.Iterate<Node> (context, e.Args)) {
                  /*
                   * Do something with "idx" here ...
                   */
                  Utilities.Convert<Node> (context, idx.Value).Value = 5;
              }
        }
    }
}
```

The above code, would somehow transform each iterated value of the expression you pass into it, to a "Node" somehow.
Either by casting, or by conversion, if they're not already nodes from before.

If you invoked the above Active Event from Hyperlambda, using the following code for instance.

```
_data
  n1:value 1
  n2:value 2
foo:x:/../*/_data/*
```

Then your node-set would look like the following afterwards.

```
_data
  n1:int:5
  n2:int:5
foo:x:/../*/_data/*
```

See the project called [p5.active-event-sample-plugin](/samples/p5.active-event-sample-plugin/) for more details about 
how to create Active Events in C#.



