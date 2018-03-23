## Expression iterators

There exists 19 expression iterators in P5. Although you can most of the time get away with only knowing a handful of these, 
obviously all of them have to be documented. Below you can find the documentation for each of these iterators, listed in no 
particular order.

**Notice**, these parts are not necessary to understand in order to be productive with Hyperlambda and Phosphorus
Five - But rather intended as _"additional study material"_, in case you want to dive deeper into it, and
understand the entire platform.

### Named elder relative iterator

This iterator is sometimes also referred to as the _"variable scope iterator"_, since it's the closest thing you come to 
referencing variables in P5. It will look for an elder sibling matching the name, and if not found, traverse up to its first 
ancestor, checking if its name matches, for then to check its ancestors' elder sibling nodes for a match. Then continue this 
process, until it finds the first match, or yields a null result.

This iterator will never return more than a single match for each of its previous result set nodes.

```hyperlambda
_foo:first-foo
_foo:second-foo
  _foo:third-foo
src:x:/@_foo?value
```

Which results in the following.

```hyperlambda
src:second-foo
```

Notice how it ignores the **[third-foo]**  and the **[first-foo]** above. The **[third-foo]** is ignored, because it's not 
within the _"scope"_ of the **[src]** identity node. The **[first-foo]** is ignored, because the iterator found another match, 
before it reached the first foo node.

### Children iterator

This iterator always looks like the following `/*`, and will yield the children of its previous result set. Below it's our 
second iterator, at the end of our **[src]** invocation, just before its `?name` parts.

```hyperlambda
_foo
  foo1
    foo1_1
    foo1_2
  foo2
    foo2_1
    foo2_2

// Returns the children of the above [_foo] node.
src:x:/@_foo/*?name
```

Which results in the following.

```hyperlambda
src
  foo1
  foo2
```

### Descendants iterator

This iterator always looks like the following `/**`, and will yield all descendants of its previous result set.

```hyperlambda
_foo
  foo1
    foo1_1
    foo1_2
  foo2
    foo2_1
    foo2_2

// Returns all descendants of the above [_foo] node.
src:x:/@_foo/**?name
```

Which results in the following.

```hyperlambda
src
  foo1
  foo1_1
  foo1_2
  foo2
  foo2_1
  foo2_2
```

Notice the difference in the result set of the descendants iterator, and the children iterator above. The children 
iterator never yielded its **[foo1_1]** node, while the descendant iterator did.

### Distinct name iterator

This iterator always looks like the following `/$`, and will yield all distinct names it can find from its previous 
result set, yielding the first it finds, ignoring all consecutive matches having the same names as a previous found. 
The comparison is case sensitive, using the invariant culture to compare its matches - Like all iterators do in P5.

```hyperlambda
_foo
  foo1:foo1-hit
    foo1_1:foo1_1-hit
    foo1_2:foo1_2-hit
  foo1:foo1-MISS
    foo2_1:foo2_1-hit
    foo1_2:foo1_2-MISS

// Returns all values of the distinctly named nodes from the above [_foo] node.
src:x:/../*/_foo/**/$?value
```

Which results in the following.

```hyperlambda
src
  :foo1-hit
  :foo1_1-hit
  :foo1_2-hit
  :foo2_1-hit
```

### Distinct value iterator

Same as above, but will look for distinct values instead of names. Looks like the following `/=$`.

```hyperlambda
_foo
  foo1-HIT:thomas
    foo1_1-HIT:hansen
    foo1_2-MISS:thomas
  foo2-HIT:john
    foo2_1-MISS:hansen
    foo2_2-MISS:john

// Returns all names of the distinct values from the above [_foo] node.
src:x:/../*/_foo/**/=$?name
```

Which results in the following.

```hyperlambda
src
  foo1-HIT
  foo1_1-HIT
  foo2-HIT
```

### Modulo iterator

This iterators yields the _"modulo"_ result of its previous result set.

```hyperlambda
_data
  foo1
  foo2
  foo3
  foo4
src:x:/@_data/*/%2
```

Which results in the following.

```hyperlambda
src
  foo2
  foo4
```

Notice how only nodes being a modulo of 2 are yielded in the above result.

### Named iterator

Will yield only nodes matching the specified name. Notice, this is the default iterator, if no other match 
can be found. You can also escape your specified name, if it clashes with other iterators. E.g. `:x:/\0` - 
Which will return a node matching the **name** of _"0"_, and not necessary the zeroth child.

```hyperlambda
_data
  foo1
  foo2
  foo3:foo3-1
  foo4
  foo3:foo3-2
src:x:/@_data/*/foo3?value
```

Which results in the following.

```hyperlambda
src
  :foo3-1
  :foo3-2
```

### Named ancestor iterator

Returns the first ancestor mathching the specified name. Always starts out with `/..`, then followed by the name 
you're looking for. Iterator will never return more than a single match.

```hyperlambda
_data
  foo1:result
    foo2
    foo3
      foo4
        starting
src:x:/../**/starting/..foo1?value
```

Which results in the following.

```hyperlambda
src:result
```

### Numbered child iterator

Yields the n'th child of the previous result set, where n must be an integer.

```hyperlambda
_data
  foo1
  foo2
  foo3
src:x:/@_data/1
```

Which results in the following.

```hyperlambda
src
  foo2
```

### Parent iterator

Yields the parent of its previous result set. Always looks like the following `/.`.

```hyperlambda
_data
  foo1
    foo2
      foo3
src:x:/../**/foo3/.?name
```

Which results in the following.

```hyperlambda
src:foo2
```

### Range iterator

Returns a range of its previous result set. Looks like the following `/[x,y]` where x and y are integers, defining the intersection
of nodes you wish to retrieve.

```hyperlambda
_data
  foo1
  foo2
  foo3
  foo4
  foo5
src:x:/@_data/*/[1,3]
```

Which results in the following.

```hyperlambda
src
  foo2
  foo3
```

### Reference iterator

Yields the value of the previous result set, converted into a node, preferably by reference, 
allowing you to change the node accordingly if you wish.

```hyperlambda
_data:node:"foo-inner:INITIAL-VALUE"
set:x:/@_data/#?value
  src:SUCCESS
```

Which results in the following.

```hyperlambda
_data:node:"foo-inner:SUCCESS"

/* ... rest of lambda ... */
```

Notice how we were able to change the value of our inner node in the above **[set]** invocation.

### Reverse iterator

Reverses the previous result set.

```hyperlambda
_data
  foo1
  foo2
  foo3
src:x:/@_data/*/<-
```

Which results in the following.

```hyperlambda
src
  foo3
  foo2
  foo1
```

### Root iterator

Yields the root iterator. Will never yield more than one match, being the root of the current node. Will also for
obvious reasons, never yield a _"null result"_.

```hyperlambda
src:x:/..
```

Which results in the following.

```hyperlambda
src
  exe
    src:x:/..
```

### Left shift iterator

_"Left shifts"_ the previous result set.

```hyperlambda
_data
  foo1
    foo1_child
  foo2
  foo3
  foo4
src:x:/@_data/*/%2/<
```

Which results in the following.

```hyperlambda
src
  foo1_child
  foo3
```

Notice how the modulo operator's result is _"left shifted"_. This process will not necessarily result 
in finding the younger sibling, but rather the _"previous node"_, as you can see an example of above, 
where we retrieved the **[foo1_child]** node, instead of the **[foo1]** node.

### Right shift iterator

Right shifts the previous result set. Opposite of the above.

```hyperlambda
_data
  foo1
  foo2
  foo3
  foo4
  foo5
src:x:/@_data/*/%2/>
```

Which results in the following.

```hyperlambda
src
  foo3
  foo5
```

See the comments for the left shift iterator, to understand what _"shifting"_ a node result actually means.

### Younger sibling iterator

Returns the n'th younger sibling from its previous result set.

```hyperlambda
_data
  foo1
  foo2
src:x:/@_data/1/-1
```

Which results in the following.

```hyperlambda
src
  foo1
```

Notice, both the younger and elder sibling iterators will actually roundtrip to the beginning, if you 
provide a number, which is higher/lower than that which can be found in the result set. This means 
that you can use it to find for instance the _"last child node"_, by doing something like the following `/0/-`.

Also notice that the number is optional, and if not supplied, will default to a value of 1. This is 
true for both the younger and the elder sibling iterator.

### Elder sibling iterator

Returns the n'th younger sibling from its previous result set.

```hyperlambda
_data
  foo1
  foo2
src:x:/@_data/0/+1
```

Which results in the following.

```hyperlambda
src
  foo2
```

See the comments for the younger sibling iterator for a detailed explanation.

### Value iterator

Yields the nodes matching the specified value.

```hyperlambda
_data
  foo1:bar1
  foo2:bar2
  foo3:bar3
src:x:/@_data/*/=bar2
```

Which results in the following.

```hyperlambda
src
  foo2:bar2
```

Notice, this iterator can also take regular expressions, such as the following illustrates - 
At which point the entire expression needs to be wrapped inside of a string literal, due 
to having added a `:` in its value.

```hyperlambda
_data
  foo1:xxx
  foo2:bar1
  foo3:bar2
  foo4:yyy
src:x:@"/@_data/*/""=:regex:/bar/"""
```

Which will result in the following.

```hyperlambda
src
  foo2:bar1
  foo3:bar2
```

### The recursive nature of iterators

By intelligently combining your iterators, you can often reduce what would require hundreds 
of lines of code in e.g. traditional C#, to a single expression in P5 - Removing all 
recursive method/function invocations in the process.

Imagine you have a tree, where you wish to update every single value part of your tree, 
to a single value, depending upon some criteria. This is easily done with a single expression 
in Hyperlambda - While in C# it would require reursive method invocations, and lots of 
complicated code, that is especially difficult to understand for beginners. Below is an example 
that updates every single CSS class, of all of our _"buttons"_ HTML elements, recursively, 
without actually using recursion.

```hyperlambda
.elements
  widgets
    container
      literal
        element:button
        class:OLD-CSS-CLASS
      literal
        element:p
        class:NOT-CHANGED
      container
        class:NOT-CHANGED
        widgets
          literal
            element:button
            class:OLD-CSS-CLASS

/*
 * Retrieves all [literal] widget, having an [element] of type "button",
 * and updates their [class] attribute.
 * Notice; Two lines of code!
 */
set:x:/@.elements/**/literal/*/element/=button/./*/class?value
  src:NEW-CLASS
```

In most traditional programming languages, the above would require dozens of lines of code, 
possibly more - In addition to probably also recursive function or method invocations. In 
Hyperlambda it's 2 lines of code.

Lambda expressions, and its iterators, might seem difficult to grasp when you start out with 
them. But after a while, you will notice how they allow you to _do a lot with very little effort_. 
This trait is something that lambda expressions share with e.g. SQL. SQL is often said to be 
a _"what"_ language, and not a _"how"_ language. This is why you can get away with tiny SQL 
statements, doing a lot of things for you. The same is true for lambda expressions, and its 
iterators - Except, where SQL selects from tables or two dimensional matrixes, lambda expressions 
selects from graph objects and 3 dimensional tree structures.

In fact, the scientific theory they build upon, is the construction of _"hyperplanes"_, through 
another pre-existing 3 dimensional graph object. Simply because they create another _"dimension"_, 
cutting through your existing 3 dimensional structures, extracting parts of the original result 
set. Don't let these difficult mathematical theories scare you though, since it's simply the theory 
behind them, and is not necessary in order to understand them, or use them in your own software.

Combined with the boolean algebraic properties of expressions, you can literally move mountains 
with lambda expressions.
