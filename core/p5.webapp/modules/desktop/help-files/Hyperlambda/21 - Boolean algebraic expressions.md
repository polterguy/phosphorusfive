## Boolean algebra on expressions

To understand how boolean algebra works in regards to expressions, it might be feasable to start out with some boolean algebra theory. 
The easiest way to do such, is to define the 4 different boolean algebraic operators, using a simple figure. Below is a figure, we will 
refer to, during the rest of this chapter. Try to envision it, as you continue your reading.

https://phosphorusfive.files.wordpress.com/2018/01/boolean-algebra.png

The above is a visualization of two expressions, that partially overlaps. This creates 3 different sections, which we will refer to 
as *"a"*, *"b"* and *"c"*. *"a"* is the result of our first expression, *"c"* is the results of our second expression, and *"b"* is 
the parts of these two expressions that overlaps.

### Defining the boolean algebraic operators

There are four basic boolean algebraic operators in lambda expressions.

- &, boolean AND, union or *"intersection"* of two expressions
- |, boolean OR, product of two expressions
- ^, XOR, eXclusively OR'ed product of two expressions
- !, NOT, subtracting results from second expression from first expression

These four boolean algebraic operators, allows us to do *"logical math operations"* on our graph objects, resulting from two expression. 
This allows us to create one expression, and then apply another expression after our first, which we combine together with each other, 
using one of the four boolean algebraic operators, to produce some sort of result.

Notice, contrary to in traditional boolean algebra, where logical _"NOT"_ is an unary operator - In lambda expressions, the logical NOT 
operator is a binary operator, requiring both a left hand side, and a right hand side.

### Logical AND

The boolean algebraic AND operator, or & as it is written in lambda expressions, allows you to retrieve only the parts that overlaps each 
other. From our figure above, this would become the *"b"* parts.

Imagine I have one expression which yields all first names being *"John"*, and another expression yielding all last names being *"Doe"*. 
If I AND'ed these two expressions together, I would end up retrieving only the people having both a first name of *"John"*, AND a last 
name of *"Doe"*. Below you can find an example of such an expression.

```hyperlambda
_src
  John:Doe
  Jane:Doe
  John:Farmer
  Stanley:Kubrick
_dest
add:x:/@_dest
  src:x:/@_src/*/John&/@_src/*/=Doe
```

Notice the `&` parts in the middle of our **[src]** node's expression above. This will logically AND our two expressions together, and 
only return the results that can be found in BOTH of our expressions. Hence, the result becomes that of, our **[\_dest]**, ending up 
having one additional node; `John:Doe`.

Also notice that both our `Jane:Doe` and our `John:Farmer` nodes are ignored, because neither of them fullfilled the criteria of **both** 
expressions. Referring back to our original figure in the beginning of this article, this means we'll end up with only the *"b"* parts 
as our results.

https://phosphorusfive.files.wordpress.com/2018/01/boolean-algebra-and.png

AND is said to return only the UNION or the intersection of our two expressions. It doesn't matter which expression you start out with 
when you AND two expressions together. Even if you flip your expressions around, the result will still be the same.

### Logical OR

OR on the other hand, will yield anything found in EITHER of our two expressions. If you exchange the *"&"* in our above Hyperlambda, 
the results in our **[\_dest]** node will include *"John Doe"*, *"John Farmer"* and *"Jane Doe"*. Try to run the following code.

```hyperlambda
_src
  John:Doe
  Jane:Doe
  John:Farmer
  Stanley:Kubrick
_dest
add:x:/@_dest
  src:x:/@_src/*/John|/@_src/*/=Doe
```

Going back to our original figure, this means we will end up with our result being anything found in **either** *"a"*, *"b"* or *"c"*.

https://phosphorusfive.files.wordpress.com/2018/01/boolean-algebra-or.png

OR is said to return the *"product"* of our two expressions. It doesn't matter which expression you start out with, when you OR 
two expressions together. Even if you flip your expressions around, the result will still be the same.

### Logical XOR

XOR means _eXclusively OR_, and translates into English like *"give me anything found only in __one__ of the following expressions"*. 
Try to exchange the *"&"* in our original Hyperlambda with a *"^"* character, to use the XOR operator. Notice how this returns 
only *"John Farmer"* and *"Jane Doe"*, but **not** in fact *"John Doe"*, because he can be found in both result sets.

Going back to our original figure, this means what we are extracting from our result set, is only the *"a"* and *"c"* parts, 
but **not** the *"b"* parts. Anything that is the result of an intersection between both of our expressions is discarded. 
Besides from that, it is similar to OR, which is why it is called *"eXclusive OR"*.

https://phosphorusfive.files.wordpress.com/2018/01/boolean-algebra-xor.png

XOR is said to return the *"product"* of our expressions, minus the UNION or the *"intersection"*. It doesn't matter which 
expression you start out with, and which you end with when you XOR two expressions together. Even if you flip your expressions 
around, the result will still be the same.

### Logical NOT

NOT subtracts the result of its second expression, from its first expression, and returns only the parts that can only be 
found in the first expression. Try exchanging the *"&"* parts of our original Hyperlambda with an exclamation mark *"!"*, and 
see the results. This of course, will return all people with a first name of *"John"*, **except** those also having a second name 
of *"Doe"*. Resulting in only *"John Farmer"* being our result.

Going back to our original figure, this results in only the *"a"* parts.

https://phosphorusfive.files.wordpress.com/2018/01/boolean-algebra-not.png

NOT is said to return the first expression, minus the UNION or *"intersection"* of our second expression.

When you NOT two expressions together, the order of your expressions is important, and if you flip your two expressions around, 
you will achieve a different result. Hence, with NOT, order counts. In all the other boolean algebraic expression operators, 
order is _not_ important, the same way order is not important when adding or multiplying numbers together. Logically, you can 
think of NOT as _"subtracting"_ the right hand expression from the left hand side.


### Grouping sub-expressions

A lambda expression can contain _"sub expressions"_. These are inner expressions, using the results of their outer expression 
as their data-set as they are being evaluated. This is highly useful when combining with boolean algebraic operators, since 
it allows us to create much more condense expressions, more easy to read, as our expressions becomes more complex in nature. 
Below is an example.

```hyperlambda
_src
  John:Doe
  Jane:Doe
  John:Farmer
  Stanley:Kubrick
_dest
add:x:/@_dest
  src:x:/@_src/*(/John|/Jane)
```

To understand the above expression, realise first of all that there is in fact not one expression in the above **[src]** node. 
Rather in fact, there are actually **3** expressions. Two of them are grouped, inside of the parantheses. While the outer expression 
becomes the _"initial result set"_ the inner expressions are starting out with.

Translated into plain English, what the above expression actually does, can be summed up as follows; *"Give me all nodes beneath [_src] who's names are either 'John' or 'Jane'"*. 
Creating grouped sub-expressions, and using the boolean algebraic features of expressions, can give you incredible dense syntax, 
and *"tight"* code. Below is a more useful use-case.

```hyperlambda
_data
  foo:foo1
    foo:foo2
    howdy:howdy
      foo:foo3
      jo-there:jo-there
        bar:bar1
        howdy-2:howdy-2
          foo:foo4
      jo:jo
        foo:foo5
    bar:bar2
for-each:x:/@_data/**(/foo|/bar)
  create-widget
    innerValue:x:/@_dp/#?value
```

If we ignore the above **[\_data]** segment, which technically is not a part of our code, but rather its data - 
We have **3** lines of actual code in the above Hyperlambda. Trying to even create something as the above, in 
any other programming language, including C# and LINQ - Would require recursive function invocations, and 
possibly dozens of lines of code. The above code is **3 lines of code**! And the code is relatively easy to 
grasp, once you've got an understanding of lambda expressions. And, more importantly, the above code happens 
to be an example of something that would be considered a useful scenario, and something you'd probably highly 
likely run into, when creating your own projects.

By intelligently mastering lambda expressions, you can often describe with a single line of code, what requires 
sometimes hundreds of lines of code, in other programming languages.

You can create as many groups as you wish for your lambda expressions. Unless you explicitly declare a boolean 
operator for your first sub-expression, logical OR as assumed.

As an interesting trait, realise that by simply adding **two characters, and removing one character** to the 
above code, we can completely *"negate"* what it is originally doing. Try running the following through your 
executer to see what I mean.

```hyperlambda
_data
  foo:foo1
    foo:foo2
    howdy:howdy
      foo:foo3
      jo-there:jo-there
        bar:bar1
        howdy-2:howdy-2
          foo:foo4
      jo:jo
        foo:foo5
    bar:bar2
for-each:x:/@_data/**(!/foo!/bar)
  create-widget
    innerValue:x:/@_dp/#?value
```

In our above code, we added two `!` operators, and removed the existing `|` operator, and as we did, we completely 
negated the result of our expression.

### Ninja tricks

An example of just how useful these features of P5 is, can be illustrated with the following.

```hyperlambda
.exe
  .defaults
    foo:Thomas Hansen
  create-widget
    innerValue:x:/../*(/foo|/.defaults/*/foo)/$?value
eval:x:/@.exe
  foo:John Doe
```

What the above example does, is to use the _"unique name iterator"_ (the `$` iterator), to make sure it 
only selects the first node from its previous result set, having the name of _"foo"_. This means that if you 
supply an argument named **[foo]** to your above **[eval]**, then your **[.exe]** lambda object will use 
this argument. If you do not supply any **[foo]** argument, the lambda will use the value inside of 
its **[.defaults]** segment. Try removing the **[foo]** node at the bottom of your code, and see the 
difference.

The above is a commonly used lambda expression, for applying default arguments to lambda objects and 
Active Events. Another really nifty Ninja trick, is to defer the boolean algebraic operators, and make 
them become arguments. For instance, imagine we create an Active Event such as the following.

```hyperlambda
create-event:examples.foo-bar-example
  for-each:x:/@_arg/#/**({0}/foo{1}/bar)
    :x:/@operator1?value
    :x:/@operator2?value
    create-widget
      innerValue:x:/@_dp/#?value
```

For then to invoke our above Active Event with the following code.

```hyperlambda
_data
  foo:foo1
    foo:foo2
    howdy:howdy
      foo:foo3
      jo-there:jo-there
        bar:bar1
        howdy-2:howdy-2
          foo:foo4
      jo:jo
        foo:foo5
    bar:bar2
examples.foo-bar-example:x:/@_data
  operator2:|
```

If you remove the above **[operator2]** argument, and add the following arguments instead, you have completely 
negated the result, dynamically, as an argument to your invocation.

```hyperlambda
examples.foo-bar-example:x:/@_data
  operator1:!
  operator2:!
```

This is accomplished by using _"formatting expressions"_ for our operators, instead of handcoding our operators 
directly into our expression. This allows us to make our boolean algebraic operators of choice, be deferred as 
an argument to our lambda objects and Active Events. This entirely changes the logic of our expressions, 
dynamically during runtime, such that we can reuse our lambda expressions, and be able to _"parametrize"_ them. 
You can of course parametrize any parts of your lambda expressions as you see fit. Try to imagine what the 
following does for instance.

```hyperlambda
create-event:examples.foo-bar-example-2
  for-each:x:/@_arg/#/**/{0}?value
    :x:/@name?value
    create-widget
      innerValue:x:/@_dp?value
```

Hint, try to invoke it with the following code.

```hyperlambda
_data
  foo:foo1
    foo:foo2
    howdy:howdy
      foo:foo3
      jo-there:jo-there
        bar:bar1
        howdy-2:howdy-2
          foo:foo4
      jo:jo
        foo:foo5
    bar:bar2
examples.foo-bar-example-2:x:/@_data
  name:foo
```

### Don't go berserk!

It is easy to create extremely rich lambda expressions, with very dense syntax sometimes. However, it is also 
very easy to create extremely complex lambda expressions, which you could probably spend a lot of time trying 
to figure out what actually do. Any _"obfuscated code olympic contestant"_ without at least one lambda expression, 
would probably not be able to even make it to the finals.

Be careful with your expressions. They're intended to **ease** syntax, not to prove to the world that you can 
visualize hundreds of recursive conditions, with dozens of nested boolean algebraic operators, and grouped 
sub-expressions.
