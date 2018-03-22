## Changing your tree

Hyperlambda and Phosphorus Five is a unique programming platform, since it among other things, doesn't contain the concept of a *"variable"*. 
This is because everything can be modified, as you execute your lambda objects. Hence, arguably, everything becomes a potential variable in P5.
However, often you need to store some kind of temporary variable, which you change during the execution of your lambda. For such scenarios, 
you have 4 basic Active Events you can use.

- **[set]**, changes nodes, its values, or its names
- **[add]**, appends children nodes to existing nodes
- **[insert-before]**, inserts nodes, *before* some specified nodes
- **[insert-after]**, inserts nodes, *after* some specified nodes

These four Active Events, allows you to modify your lambda objects, during the execution of your lambda. Notice, the **[set]** event, which 
in addition to changing a node, also can completely remove nodes, values, or names for you. Hence, with these four events, you have all the 4 
basic *"CRUD"* operations available for your lambda objects, and its individual nodes.

**Definition**; *"CRUD"* is an acronym, and means *"Create, Remove, Update and Delete"*, and is a reference to all the 4 basic operations 
necessary to be able to dynamically *"mold"* a collection of objects, in any of its existing axis, to be able 
to *"create anything you wish, starting from any other given starting point"*.
All of these Active Events, can be given multiple destinations. This means that a single invocation, might potentially change multiple nodes.

### The [set] event

This is the most basic Active Event for modifying your lambda objects. You can change a node, its value, or its name by using this event. 
Consider the following code. Yet again, for the examples in this chapter, please use Hypereval if you want to try them out for yourself.

```hyperlambda
_foo
set:x:/@_foo?value
  src:I was changed
```

After executing the above Hyperlambda, your **[\_foo]** node's value, will be *"I was changed"*. By combining the **[set]** event, with what 
we refer to as *"formatting expressions"*, you can easily concatenate strings. Consider the following code.

```hyperlambda
_foo:Old value
set:x:/@_foo?value
  src:{0}, and additional value
    :x:/@_foo?value
```

After execution of the above Hyperlambda, you will have concatenated the **[\_foo]** node's existing value, with the 
string *", and additional value"*. This is because of that the `{0}` parts, will be substituted with the *"zeroth"* child node's value of 
our **[src]** argument having an *empty name*. Since this in turn, is an expression, this expression will be evaluated first. This 
expression leads to **[\_foo]**'s value, being *"Old value"*, which in the end, results in that the `{0}` parts are substituted with the 
string *"Old value"*. These formatting expressions resembles `String.Format` from C#, if you're coming from a C# background.

Let's create a form, where we use the **[set]** event, in combination with some *"formatting expressions"*, to ask the user for his name, 
and create a modal confirmation window.

```hyperlambda-snippet
/*
 * Creating a modal widget, containing our form.
 */
create-widgets
  micro.widgets.modal
    widgets
      void:first_name
        element:input
        placeholder:Give me your first name ...
        class:fill
      void:last_name
        element:input
        placeholder:Give me your last name ...
        class:fill
      literal
        element:button
        class:right
        innerValue:Submit
        onclick

          /*
           * Retrieves first and last name of the user.
           */
          .widgets
            first_name
            last_name
          get-widget-property:x:/@.widgets/*?name
            value

          /*
           * Sets the [p/innerValue] further down.
           */
          set:x:/../**/micro.widgets.modal/**/p/*/innerValue?value
            src:Hello there {0}, {1}
              :x:/@get-widget-property/0/*?value
              :x:/@get-widget-property/1/*?value

          /*
           * Creates another modal widget, now with a first and last name greeting.
           */
          create-widgets
            micro.widgets.modal
              widgets
                h3
                  innerValue:Hello world
                p
                  innerValue
```

The first obvious thing you might see in the above **[onclick]** event handler, is that the **[innerValue]** argument to our **[p]** widget,
in our **[micro.widgets.modal]** invocation, is actually initially *empty*. Still, when our confirmation window is shown, it is perfectly
able to show our full name.

This is because of that our **[set]** invocation concatenates the results from our **[get-widget-property]** invocation, and 
prepends the text *"Hello there "* in front of the first and last name. To understand how this happens, realise that the `{0}` and 
the `{1}` parts of our **[src]** node's value, are substituted with the result of the zeroth and first nodes' children values. Since 
these nodes are expressions pointing to the returned value from our **[get-widget-property]** invocation, we end up with an 
initial greeting, followed by the user's first name and second name.

**Notice**, in programming languages we always start counting at *ZERO*! Hence, the first item, becomes the _zero'th_ item in programming.

The second thing you may already have noticed, is that instead of supplying a *"static ID"* to which widgets we wish to retrieve 
the **[value]** of, we supply an expression, leading to two values; *"first_name"* and *"last_name"*. These happens to be the IDs 
of our two input elements in our form, which will result in that our invocation to **[get-widget-property]** will retrieve
both of these widgets' **[value]** in one invocation. Almost all Active Events in P5 can take expressions, and hence 
do *"multiple things at the same time"*.

With this in mind, realise that the expression we're using as the zeroth formatting expressions to our **[src]** node, is first retrieving 
the **[get-widget-property]** node. Then it is retrieving its zeroth child, which is the node named **[first_name]** from our image 
above. Then it retrieves this node's children. Since there only happens to be one child beneath this node, it will return this node's value, 
which happens to be your first name, for the example above.

Then, after having evaluated both children expressions beneath our **[src]** node, it will substitute the `{0}` and `{1}` parts of 
its **[src]** node's value, with the strings *"Thomas"* and *"Hansen"* respectively, if these were the names you supplied in your form.
After it has created its **[src]** node's value, by concatenating our strings, it will move the results of this operation into its destination, 
which is our **[p]** node's **[innerValue]** argument. Hence, our **[innerValue]** node's value will end up being *"Hello there Thomas, Hansen"*.
Using this type of logic, we can change parts of our lambda object, that we still haven't execute yet, resulting in that once we execute 
this lambda, it will have a value, that we dynamically created, before we evaluated it.

You can of course also create execution instructions too, with similar constructs, by dynamically changing the name of some *"future node"* 
this way. Still confused? Watch the following video.

https://www.youtube.com/watch?v=f4ZUwlbIdaE

## [add]ing things to your tree

The **[add]** event, allows you to append new children into other nodes. However, let's start out with an example.

```hyperlambda-snippet
/*
 * Creating a modal widget, containing our form.
 */
create-widgets
  micro.widgets.modal
    widgets
      void:first_name2
        element:input
        placeholder:Give me your first name ...
        class:fill
      literal
        element:button
        innerValue:Submit
        onclick

          /*
           * The content of this node, is dynamically created, according to
           * the input supplied by the user
           */
          .exe

          /*
           * Retrieving the input supplied by the user
           */
          get-widget-property:first_name2
            value

          /*
           * This is called "branching".
           * Only if the result of the expression in our "if" node matches
           * the string "Thomas", the lambda object inside of our "if"
           * will be executed.
           * Otherwise our "else" lambda object will execute.
           */
          if:x:/@get-widget-property/*/*?value
            =:Thomas

            /*
             * Adding a confirmation window to our [.exe] node above.
             */
            add:x:/@.exe
              src
                micro.windows.info:Yo boss!

          else

            /*
             * Adding an info-tip window to our [.exe] node above.
             */
            add:x:/@.exe
              src
                micro.windows.info:Howdy stranger

          /*
           * [eval] will now execute our [.exe] node, almost invoking it, as
           * if it was a "function" or a "method".
           */
          eval:x:/@.exe
```

The above example, will actually dynamically _"mold"_ your lambda object, according to what you type into the textbox. Depending
upon what you supply in the textbox, it will **[add]** a different lambda object into its **[.exe]** lambda destination.

To understand what's happening in this code, realise we're *"branching"* our lambda object, according to the return value of 
our **[get-widget-property]** invocation. We will take a deeper look at the concept of *"branching"* later, but basically 
if the value of our textbox is *"Thomas"*, it will **[add]** a different lambda into our **[.exe]** node, than it will with all
other input values.

Finally, it will evaluate this **[.exe]** node, and execute it as a lambda object. Hence, what we have actually done, is to 
dynamically create a lambda object, depending upon the value of our textbox - For then to evaluate our dynamically created lambda. 
The above code is a naive example, but the concept is extremely powerful, since it allows you to dynamically create your lambda 
objects during runtime. This results in that you can have the computer both create its own lambda objects, in addition to modify 
existing lambda objects.

### Still confused?

The above code might be challenging to visualize in the beginning, especially for experienced developers for some reasons, 
since there are no similar concepts in main stream use out there as far as I know. However, if you imagine Hyperlambda as HTML, 
and lambda as the DOM your HTML creates. Then imagining how JavaScript can change your DOM dynamically, and how this becomes 
the equivalent of dynamically *"molding"* your lambda objects during runtime. Then realize that the same way JavaScript can 
dynamically _"mold"_ your DOM, Hyperlambda can dynamically mold its _"lambda"_ objects, using constructs such as _"insert"_, 
_"add"_, _"set"_, etc. This might create a mental model you can use to visualise this more easily.

## [insert-before] and [insert-after]

These two Active Events functions identically to **[add]**, except they don't append nodes to its destination(s), but rather *"injects"* 
its **[src]** argument's, either *"before"* or *"after"* its destination nodes. Execute the following code in your Hypereval module, 
and watch its result.

```hyperlambda
_foo
insert-before:x:/@_foo
  src
    _insert:before
insert-after:x:/@_foo
  src
    _insert:after
```

Probably exactly as you'd expect. The above *"injects"* one node before **[\_foo]**, and another node after **[\_foo]**. Besides from 
that fact, **[insert-before]** and **[insert-after]** shares most of its functionality with **[add]**. Hence, learning how to use any 
one of these Active Events, very effectively teaches you how to use all of them.

## [add], [insert-before] and [insert-after]

Besides from inserting before/after, and appending children nodes, these three Active events functions exactly the same. So let us have 
a look at how they actually work, and how we can parametrize them. So far, we have only used *"static sources"*. A static source, 
is a bunch of static nodes, being children of the **[src]** node. However, you can also supply an expression, leading to another 
node-set as its **[src]**. Imagine having a node-set that you want to copy into some destination.

```hyperlambda
_src
  foo1:bar1
  foo2:bar2
_dest
add:x:/@_dest
  src:x:/@_src/*
```

After evaluating the above Hyperlambda in Hypereval, the **[\_dest]** node, will have a copy of all children nodes from **[\_src]**.

Both the **[add]** and the **[insert-xxx]** Active Events, can be given multiple **[src]** arguments. Imagine something like the 
following.

```hyperlambda
_src1
  foo1:bar1
_src2
  foo2:bar2
_dest
add:x:/@_dest
  src:x:/@_src1/*
  src:x:/@_src2/*
```

The above could also be accomplished by using a boolean algebraic expression instead. However, the simplified syntax of using 
multiple **[src]** nodes, often outweighs the benefits of the condensed syntax using algebra. Boolean algebraic expressions are 
covered in one of the appendixes. Notice though that **[set]** can only handle one **[src]** argument.

### Multiples destinations

All of the above Active Events can also be given multiple destinations. This is a general pattern in P5, which allows you to 
do multiple things with one invocation. We saw this further up in this chapter, where we retrieved the values of two widgets, 
with a single invocation. Both **[set]**, **[add]** and **[insert-xxx]** have similar semantics. Try to run the following 
Hyperlambda in your Hypereval module to see this for yourselves.

```hyperlambda
_data
_data
set:x:/../*/_data?value
  src:SUCCESS!
```

This feature, although powerful, also implies that you must be careful, to make sure you are only referencing nodes you 
actually want to reference - Unless you want to end up having unintentional side-effects. On the positive side, it is also a 
feature that allows you to *"do a lot, with a little"*. One of my favourite examples in these regards, is loading all Hyperlambda 
files from a folder, and evaluating them in order - Which can actually be done with 4 lines of code (don't evaluate this code though).

#### Example of evaluating all Hyperlambda files in some folder

```hyperlambda
list-files:/foo
  filter:.hl
load-file:x:/-/*?name
eval:x:/-/*
```

In fact, arguably due to Hyperlambda's syntax, the file system becomes an _"object"_ or a _"list of objects"_, where files
are equally easy to evaluate and parametrize, as conventional functions and methods. This will become apparent as you
get more hands on experience with Hyperlambda.

## Deleting stuff

This leaves us with only one remaining concept we'll need to visit, before we can wrap up this chapter, having all four CRUD operations 
within our toolbelt - Which is deleting stuff. Deletion is extremely easy. Simply use the **[set]** event, without a **[src]** argument. 
Consider the following Hyperlambda.

```hyperlambda
_foo:bar
set:x:/@_foo
```

The above code will delete the **[\_foo]** node entirely. If you supply a type declaration for your expression pointing to its value 
instead, its value will become *"null"*. If you point it to its name, its name will become the *"empty string"*. Consider the following.

```hyperlambda
_foo1:bar
set:x:/@_foo1?value
_foo2:bar
set:x:/@_foo2?name
```

Notice the tiny semantic differences between *"nullifying a node's name and its value"*, which is there since a part of the node's 
specification, is that a node cannot have a *"null name"*. It can however have an *"empty name"*. A node's value can be null though.

### Wrapping up

You should now have a basic understanding of all four *"CRUD operations"* for lambda objects. You can now dynamically create and change 
lambda objects as you see fit. However, as we started out this chapter with, no other programming languages contains any similar 
constructs. Hence, the largest problem you're now faced with, is creating a mental model for understanding this unique idea.
This is an old video, but illustrates the concepts we have covered in this chapter fairly well.

https://www.youtube.com/watch?v=w7A4TcWHolk
