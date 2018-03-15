## P5 Hyperlambda


Hyperlambda is actually **not** technically a _"programming language"_. In fact, at its core, it is
arguably more similar to JSON or XML you might, than it is similar to for instance C# or JavaScript.
A file format for describing data, in a key/value/children relationship. In fact, there does not,
from a fundamental point of view, exist any _"programming languages"_ in Phosphorus Five. Hyperlambda
however, do describe the execution tree structure, which P5 relies upon, when executing its graph
objects. So Hyperlambda can be used for describing anything really. Just like XML and JSON can.
Hyperlambda is among other things, used internally within p5.data, as the file format for the database.

The reason why Hyperlambda was created, was because XML is too verbose to be useful for describing
this _"execution tree structure"_, while JSON contains too much unnecessary syntax, and has no type
support. Hyperlambda however, perfectly balances the needs for Phosphorus Five, being a relational
file format, allowing for describing the tree structure, which P5 is evaluating, using its lambda
construct - In addition, Hyperlambda is easily typed, allowing for the type structure the underlaying
execution engine depends upon, to be preserved for values of nodes. Below is an example of some Hyperlambda.

```hyperlambda
foo:int:5
  child-of-foo:Its value!
```

The above Hyperlambda, describes one node who's name is **[foo]**. This node has a value, which is
of type _"int"_, and contains the value of _"5"_. In addition, the **[foo]** node, has one child node,
called **[child-of-foo]**, that has a string value containing _"Its value!"_.

For the record, the convention of documenting nodes in Hyperlambda, in these files, is by adding
square brackets around them, such as this shows; **[foo]**. We also usually make sure nodes are **strong**,
except when it is obvious that we're talking about a node.

Notice, that all children of a node, is appended underneath the node, with two consecutive spaces
in front of its name. The colon (:) separates the name of the node, and its value. If you want to
explicitly declare the value's type, you can do so by injecting an additional colon (:) between
the name and the value, as the Hyperlambda above shows. For then to put the _"type declaration"_
between the two colons, like this `:int:` to describe an integer. You can continue like this,
inwards as many levels, as you wish. Below is a more complex tree structure.

```hyperlambda
foo
  child1-of-foo:Its value!
    child1-of-foos-child:JO DUDE!
  child2-of-foo:Another value
```

Above we have 4 nodes in total. **[foo]** has two children, and **[child1-of-foo]** has one child
of its own. The **[foo]** node in the above example, does not have a value at all, which means its
value becomes _"null"_. Notice how for most cases, the number of lines in a Hyperlambda file,
(almost) perfectly describes the number of nodes in total in your execution tree. Also notice the
equivalent syntax of the above writte in XML.

```xml
<foo>
  <child1-of-foo>
    <value>Its value!</value>
    <children>
      <child1-of-foos-child>
        <value>JO DUDE!</value>
      </child1-of-foos-child>
    </children>
  </child1-of-foo>
  <child2-of-foo>
    <value>Another value</value>
  </child2-of-foo>
</foo>
```

Hyperlambda _reduces the above node declaration from 13 lines of XML, to 4 lines of Hyperlambda_.
As you can see, Hyperlambda is for most practical concerns, far less verbose than XML. A similar
effect, though not as extreme, would be shown if we converted the above Hyperlambda to JSON.
This is even before we've started added _"type declarations"_, using only the _"implicit type"_
of string. If we created a more complex node hierarchy, with several different types, its XML
syntax would literally *explode*!

### String declarations in Hyperlambda

If you wish, you can create more complex strings in Hyperlambda, than what we've done above.
For instance, what happens if you have a string that contains a colon (:)?
Well, then we use the same syntax as we would use in C# and/or JavaScript. We put our string
into double quotes, like this.

```hyperlambda
foo:"Some string with a colon : in it"
```

The above declares one node, with a string value, containing a colon inside of its value.
Hyperlambda strings, can also be escaped the same way you'd escape a C# string, using for
instance _"\r"_ and _"\n"_ to signify CR and/or LF sequences. In addition, you can create
multiline strings, using the Hyperlambda equivalent of the `@` character notation from C#.
Consider the following.

```hyperlambda
foo:@"Hello World, now comes a newline
and here is the continuation of the string"
```

The above Hyperlambda still only contains _one_ node. This node has a string literal,
spanning multiple lines, where each newline automatically becomes substituted with a CR/LF
sequence (_"\r\n"_). Notice that the CRLF sequence is substituted the same way, regardles of which underlaying
platform you're using. Phosphorus Five will **always use CR/LF for Hyperlambda, regardless of your platform**.
When you handle Hyperlambda, you can also expect it to *always be encoded as UTF8*.

### Types in Hyperlambda

As previously mentioned, Hyperlambda supports types, by adding an additional string between
its _"name"_, and its _"value"_ for nodes, inbetween colons (:). Below is an example of some
of the types in Hyperlambda.

```hyperlambda
some-integer:int:5
some-decimal:decimal:5.5
some-boolean:bool:true
```

The above Hyperlambda, declares on integer value, one decimal value, and one boolean value. Notice,
if you do not obey by the expected string representation for the type's value, according to the
C# _"Invariant Culture"_ rules - Then an exception will occur as your Hyperlambda is parsed.
InvariantCulture is implicit in Hyperlambda and Phosphorus Five, and you must explicitly override
it using e.g. **[p5.types.date.format]** to get something else. The default (implicit) type in
Hyperlambda is _"string"_, unless explicitly overriden by another type declaration. Hence, the two
values below, are semantically the same.

```hyperlambda
foo1:thomas
foo2:string:thomas
```

The above `:string:` part is actually redundant, since string is the default type.
All the different types in Hyperlambda, are documented, and declared, p5.types. See the documentation
for this project, for a complete reference of all the types Hyperlambda supports. You can also easily
create your own type extensions for the Hyperlambda parser, by creating Active Event handlers, that
parses from the string representation, to object representation, and vice versa. To understand how
to do this, use any one of the pre-existing types in p5.types project as your starting ground.
This would have to be done in C# though.

Realize though, that in order to create your own type extensions, your type must somehow, support
being serialized from object to string, and vice versa. The most important Active Events to create
your type is as follows.

* __[.p5.hyperlambda.get-object-value.XXX]__ - Expected to return the object version of your instance
* __[.p5.hyperlambda.get-string-value.YourCSharpNamespace.YourCSharpClassName]__ - Expected to return the string serialised version of your object instance
* __[.p5.hyperlambda.get-type-name.YourCSharpNamespace.YourCSharpClassName]__ - Expected to return the Hyperlambda typename of your instance (the `XXX` from our event above)

### Converting from Hyperlambda to lambda, and vice versa

Sometimes you are given a piece of text, which you want to convert into lambda, or vice versa.
For such case you can use the **[hyper2lambda]** and **[lambda2hyper]** Active Events. Let us
illustrate with an example.

```hyperlambda
_hl:@"_foo:bar1
  _nother-foo:bar2"
hyper2lambda:x:/-?value
lambda2hyper:x:/-/*
```

The invocation of **[hyper2lambda]**, will convert the string value of the **[\_hl]** node, into a
lambda node structure, where each node inside of the value of **[\_hl]**, will be appended as
children to **[hyper2lambda]**. The second invocation, the one called **[lambda2hyper]**, will reverse
this process, and return the corresponding Hyperlambda, as the value of the **[lambda2hyper]**
node after invocation.

Notice though, that conversion from Hyperlambda to lambda, will _remove_ all comments and additional
spacing, leaving nothing but the raw nodes, which are understood by P5 as actual nodes. All comments
and such, will be ignored, and lost during parsing. Try the code below, to understand how this affects
your parsing.

```hyperlambda
_hl:@"/*
 * This is some comment, that will disappear after parsing of this Hyperlambda
 */
_foo:bar1
  _nother-foo:bar2"
hyper2lambda:x:/-?value
lambda2hyper:x:/-/*
```

This means that if you convert Hyperlambda to lambda, for then to convert it back to Hyperlambda,
then all comments and additional spacing is lost during the conversion. This is a conscious choice,
for among other things, preserving bandwidth and CPU resources, when evaluating Hyperlambda,
and also transmitting lambda over a network, etc. If you wish to keep comments and additional
spacing, you will have to handle your Hyperlambda as Hyperlambda, and not transform it to lambda,
for then to re-create its originating Hyperlambda again.

To create a mental model for understanding Hyperlambda, you can yhink of Hyperlambda as XML,
and lambda as its DOM (Document Object Model). The DOM is the result of parsing XML. lambda is
the result of parsing Hyperlambda.

The evaluation engine of Phosphorus Five, exclusively understands lambda, and knows nothing about
Hyperlambda in fact. This project, is the only project, that in any ways, knows about the existence
of Hyperlambda. If you wanted to, you could entirely change the underlaying file format of P5 to
for instance XML, by exchanging this project, and its Active Events, with something that parses
XML instead.