## Branching

Branching is at the core of any Turing complete programming language. Although Hyperlambda is not per se a programming language, 
it is still Turing complete, and features all common branching constructs. For those new to programming, branching implies that 
your execution instruction pointer _"jumps"_, often in accordance to some condition. The most common form of branching is an _"if"_ 
statement. Below is an example of how to branch in Hyperlambda using an _"if/else"_ block.

```hyperlambda
_data:foo
if:x:/@_data?value
  =:foo
  micro.windows.info:Foo was here
else
  micro.windows.info:Foo has left the building!
```

If you execute the above code in Hypereval, you will see its result. Try changing the value of the **[\_data]** node, to 
e.g. _"bar"_, and execute the code again, and notice how this shows a different info window.

### Comparison operators

All branching Active Events in Phosphorus Five obeys by the same logic. Basically, there are a handful of comparison Active Events 
which you can use.

- **[=]** checks for equality
- **[!=]** checks for inequality
- **[>]** checks for more than
- **[<]** checks for less than
- **[>=]** checks for more than or equals
- **[<=]** checks for less than or equals
- **[~]** checks for _"contains"_ (only useful when comparing strings)
- **[!~]** checks for _"not contains"_ (only useful when comparing strings)

All the above _"operators"_ are actually Active Events themselves, and these operators can easily be extended with your own 
comparison events. If you do, your comparison event must return boolean true, if it is supposed to be evaluated to true. You 
can also use any Active Event in place of a condition event, at which its return value, will determine whether or not
it evaluates to true or not. Below is an example that checks for the existence of a file.

```hyperlambda-snippet
if
  fetch:x:/0/0?value
    file-exists:/foo.txt
  micro.windows.info:Foo exists
else
  micro.windows.info:No foo today!
```

### Boolean algebraic compound conditions

In addition to the operator Active Events, there are also boolean algebraic events, allowing you to create compound conditions, 
where you have multiple conditions. Below is an example.

```hyperlambda-snippet
_data1:foo
_data2:bar
if:x:/@_data1?value
  =:foo
  and:x:/@_data2?value
    =:bar
  micro.windows.info:Foo and bar where happily drinking orange juice!
else
  micro.windows.info:Either foo, bar, or both, are tired of orange juice!
```

The above syntax might seem weird to developers who are used to C# or JavaScript, but remember that a lambda object, is a tree 
structure, or a graph object - And that this creates some syntactic differences, which has some advantages, and some disadvantages.
The main disadvantage, is that creating compound conditions for your branching invocations, becomes slightly more _"verbose"_. 
The advantage, is that it becomes much more easy to figure out, also in automated processes, what your branching invocations 
are actually doing - And in fact, also modify them, using the meta cognitive capabilities of lambda objects. To translate that
into something that is actually useful, realise that you can actually created automated processes, that creates your branching
lambda objects. And you can create automated processes, that inspects what an existing piece of branching lambda object does.
The previous sentence might sound weird, until you've had some experience with Hyperlambda - But basically it allows your computer
to generate code for you, and to maintain existing code for you - To some extent.

I often say that _"Hyperlambda understands Hyperlambda"_, which is actually quite unique in the programming world.

Below is a list of all the boolean algebraic compound Active Events in P5.

* __[or]__ - One of the conditions must evaluate to true
* __[and]__ - Both conditions must evaluate to true
* __[not]__ - Negates the previous _"conclusion"_

These boolean algebraic operators works similarly to how they work in other programming languages. For a cognitive understanding 
of what boolean algebra actually is, you can read the appendix on lambda expressions, which explains the theory of boolean algebra.

**Notice**, the **[and]** Active Event has presedence when doing branching, just like it has in other programming languages.

### The [while] loop

The **[while]** loop is a special case of branching. Instead of simply evaluate some lambda object a single time, it will keep on 
evaluating your lambda object, for as long as the condition is true. Below is an example.

```hyperlambda
.data
  foo1
  foo2
  foo3
while:x:/@.data/0
  create-widget
    innerValue:x:/@.data/0?name
  set:x:/@.data/0
```

The above **[while]** simply evaluates its lambda object, for as long as there exists a zero'th child beneath **[.data]** node. Then 
at the end of each iteration, it will remove the zero'th child of **[.data]** - Resulting in the creation of three widgets on your page.

The above shows a crucial point for the record, which is implicit conversion due to existence of some value, name, or node. Basically, 
the rule-set will check to see if the result of the expression it is given exists, and if it does, it will evaluate to true, 
unless it is of type boolean, and has the value of false. The above **[while]** example, could have been created like the following.

```hyperlambda
.data
  foo1
  foo2
  foo3
while:x:/@.data/*?count
  >:int:0
  create-widget
    innerValue:x:/@.data/0?name
  set:x:/@.data/0
```

However, the first example, creates less code, and becomes more readable. Logically, they're doing the exact same thing though. Another 
way to describe the exact same thing, would be to use another operator Active Event, such as the following illustrates.

```hyperlambda
.data
  foo1
  foo2
  foo3
while:x:/@.data/*?count
  !=:int:0
  create-widget
    innerValue:x:/@.data/0?name
  set:x:/@.data/0
```

All the three above examples does the exact same thing. Which you choose, depends upon your style of coding, and what you 
find to be more readable. Notice, all of these three examples depends upon their last **[set]** invocation to not enter what 
is often referred to as an infinite loop. P5 however, contains _infinite loop protection_ by default, which helps you to 
prevent evaluating a lambda object, that would normally enter an infinite loop using **[while]**. Try evaluating the above 
code for instance.

```hyperlambda
.data
  foo1
  foo2
  foo3
while:x:/@.data/0
  create-widget
    innerValue:x:/@.data/0?name
  // COMMENTED OUT, enters an "infinite loop"
  // set:x:/@_data/0
```

As you can see, after 5.000 iterations, our above **[while]** loop will throw an exception, and stop executing. If you have 
a loop, where you actually need more than 5.000 iterations, you can add up an **[\_unchecked]** argument to your **[while]** 
loop, and set its value to boolean true. Don't do it with the above code though, unless you want to crash your web server.

### The "contains" operators

Both the **[~]** and the **[!~]** Active Events are special, and only intended for string handling. They will basically check 
some string, to see if another string exists within your source string - And if so, yield either true or false, depending upon 
whether or not you are using the negated version or not.

The following code illustrates an example.

```hyperlambda
_data:Thomas Hansen was here
if:x:/@_data?value
  ~:Hansen
  micro.windows.info:Yo boss!
else
  micro.windows.info:Yo stranger!
```

As long as the above **[\_data]** node's value contains the string *"Hansen"*, the **[~]** operator event above will yield true. 
If you try changing *"Hansen"* to *"Doe"* in the above **[\_data]** node's value, it will not evaluate to true.

### Regular expression matching with the contains operators

The contains operators can also be given regular expressions, instead of simple strings. Below is an example looking for any _"sen"_ name.

```hyperlambda
_data:Thomas Hansen was here
if:x:/@_data?value
  ~:regex:/[A-Z]{1,1}[a-z]*sen/
  
  /*
   * Xxxsen name found.
   */
  micro.windows.info:Yo boss!

else

  /*
   * No "Xxxsen" name found.
   */
  micro.windows.info:Yo stranger!
```

The above regular expression will match anything starting with a capital letter, followed by x lower case letters, ending with 
the string _"sen"_. Basically, matching any name ending with the string _"sen"_, which is a common surname in Norway.

How to create regular expressions will be dealt with in a later appendix, but basically, a regular expression starts and ends 
with a "/", followed by optionally some regex options. In addition, a regular expression has the type declaration of `:regex:`.

**Warning**; Regular expressions are notoriously difficult to read and understand! Be careful with them!

### To branch or not to branch

A lot of times, you can choose between using branching Active Events, or using more complex lambda expressions. Often 
this is a choice, which you will have to do in accordance to the problem at hand. For instance, in the above code, we 
are looping through a bunch of nodes, and using an **[if]**, to extract only the nodes having some specified name.

```hyperlambda
.data
  foo:foo1
  bar:XXX
  foo:foo2
for-each:x:/@.data/*
  if:x:/@_dp/#?name
    =:foo
    create-widget
      innerValue:x:/@_dp/#?value
```

The above will create a widget for each node inside of our **[.data]** segment, having the name of *"foo"*. The exact same 
logic could have been much more easily created using a slightly more complex lambda expression for your **[for-each]** invocation. 
This would significantly reduce the complexity of your code, and make it much more readable. Below is an example of doing just that.

```hyperlambda
.data
  foo:foo1
  bar:XXX
  foo:foo2
for-each:x:/@.data/*/foo
  create-widget
    innerValue:x:/@_dp/#?value
```

Using a single additional iterator in our above **[for-each]**, we have gotten entirely rid of our original **[if]** invocation, 
making the code significantly more easy to read, and much smaller. Often you can get rid of entire hierarchies of conditional 
event invocations by adding some tiny additional amount of intelligence into your lambda expression - Eliminating your branching 
invocations completely. Above for instance, if we ignore the **[.data]** segment, we had 5 lines of code, 
which we were able to reduce to 3 lines of code, while at the same time making our code more readable.

Often a lot of tasks in Hyperlambda becomes significantly smaller, and surprisingly dense in syntax, compared to other programming
languages. Things that may require hundreds, and sometimes thousands of lines of code in Java or C#, can often be done with 5-10 
lines of Hyperlambda code.
