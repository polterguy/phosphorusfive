Hyperlisp, an alternative to JSON and XML
========

Hyperlisp is actually _not_ a "programming language". In fact, at its core, it is nothing more than JSON or XML you might argue. 
A file format for describing data, in a key/value/children relationship. In fact, there does not, from a fundamental point of view, 
exist any "programming languages" in Phosphorus Five. Hyperlisp however, do describe the relational execution tree structure, which p5.lambda
relies upon, when executing its executional graph objects. So Hyperlisp can be used for describing anything really. Just like XML can.

The reason why Hyperlisp was invented though, was because XML is too verbose to be useful for describing this "execution tree structure",
while JSON contains too much "unnecessary syntax". Hyperlisp however, perfectly balances the needs for Phosphorus Five, being a relational
file format, allowing for describing the tree structure, which p5.lambda is evaluating, using its *[eval]* Active Event. In addition, Hyperlisp
is easily "typed", allowing for the type structure, the underlaying execution engine depends upon, to be preserved for values of nodes.

Anyway, enough of the abstract, let's see it in action!

```
foo:int:5
  child-of-foo:Its value!
```

The above Hyperlisp, describes one node who's name is "foo". This node has a value, which is of type "int", and contains the value of "5".
In addition, the "foo" node, has one child node, called "child-of-foo", that has a string value of "Its value!".

Notice, that all children of a node, is appended underneath the node, with two consecutive spaces in front of its name. The colon (:) separates
the name of the node and its value. If you want to explicitly declare the value's type, you can do so by injecting an additional colon (:) 
between the name and the value, as the Hyperlisp above shows. For then to put the "type declaration" between the two colons, like this ":int:" 
to describe an integer.

You can continue like this, as many levels inwards, as you wish. Below is a more complex tree structure.

```
foo
  child1-of-foo:Its value!
     child1-of-foos-child:JO DUDE!
  child2-of-foo:Another value
```

Above we have 4 nodes in total. "foo" has two children, and "child1-of-foo" has one child of its own. The "foo" node in the above example, 
does not have a value at all, which means its value becomes "null". Notice how for most cases, the number of lines in a Hyperlisp file, 
almost perfectly describes the number of nodes in total in your execution tree. Also notice the equivalent syntax of the above writte in XML.

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

Hyperlisp reduces the above node declaration from 13 lines of XML, to 4 lines of Hyperlisp. As you can see, Hyperlisp is for most 
practical concerns, far less verbose than XML. A similar effect, though not as extreme, would be shown if we converted the above Hyperlisp
to JSON. This is even before we've started added "type declarations", using only the "implicit type" of string. If we created a more
complex node hierarchy, with several different types, its XML syntax would literally _explode_!

### String declarations in Hyperlisp

If you wish, you can create more complex strings in Hyperlisp, than what we've done above. For instance, what happens if you have a string
that contains a colon (:) ...?

Well, then we use the same syntax as we would use in C# and/or JavaScript. We put our string into double quotes, like this.

```
foo:"Some string with a colon : in it"
```

The above declares one node, with a string value, containing a colon inside of its value.

Hyperlisp strings can also be escaped the same way you'd escape a C# string, using for instance "\r" and "\n" to signify CR and/or LF sequences.
In addition, you can create "multiline strings", using the Hyperlisp equivalent of the "@" character notation from C#. Consider this code.

```
foo:@"Hello World, now comes a newline
and here is the continuation of the string"
```

The above Hyperlisp still only contains _one_ node. This node has a string literal, spanning multiple lines, where each newline automatically 
becomes substituted with a CRLF sequence (\r\n). Notice that the CRLF sequence is substituted the same way, regardles of which underlaying
platform you're using.

### Types in Hyperlisp

As previously mentioned, Hyperlisp supports types, by adding an additional value between the "name", and its "value", inbetween colons (:).
Below is an example of some of the types in Hyperlisp.

```
some-integer:int:5
some-decimal:decimal:5.5
some-boolean:bool:true
```

The above Hyperlisp, declares on integer value, one decimal value, and one boolean value. Notice, if you do not obey by the expected string
representation for the type's value, according to the C# "Invariant Culture" rules, an exception will occur, during parsing of your Hyperlisp.

The default (implicit) type in Hyperlisp is "string", unless explicitly overriden by another type declaration. Hence, the two values below, 
both have the same type, and value.

```
foo1:thomas
foo2:string:thomas
```

The above ":string:" part is actually redundant, since "string" is the default type, if your type declaration is omitted.

All the different types in Hyperlisp, are documented, and declared, in the "p5.types" project. See the documentation for this project, for
a complete reference of all the types Hyperlisp supports. You can also easily create your own "type extensions" for the Hyperlisp parser, 
by creating Active Event handlers, that parses from the string representation, to object representation, and vice versa, for your types,
in for instance C#. To understand how to do this, use any one of the pre-existing types in "p5.types" as an example starting ground.

Realize though, that in order to create your own "type extension", your type must somehow, support being serialized from object to string, 
and vice versa.

### Converting from Hyperlisp to p5.lambda, and vice versa

Sometimes you are given a piece of text, which you want to convert into p5.lambda, or vice versa. For such case you can use the *[lisp2lambda]*
and *[lambda2lisp]* Active Events. Let us illustrate with an example.

```
_hl:@"_foo:bar1
  _nother-foo:bar2"
lisp2lambda:x:/-?value
lambda2lisp:x:/-/*
```

The invocation of *[lisp2lambda]*, will convert the string value of the *[_hl]* node, into a lambda node structure, where each node inside 
of the value of *[_hl]*, will be appended as children to *[lisp2lambda]*. The second invocation, the one called *[lambda2lisp]*, will reverse 
this process, and return the corresponding Hyperlisp, as the value of the *[lambda2lisp]* node, after invocation.

Notice though, that conversion from Hyperlisp to lambda, will _REMOVE_ all comments and additional spacing, leaving nothing but the "raw nodes",
which are understood by the p5.lambda engine, as actual nodes. All comments and such, will be ignored, and lost during parsing. Try the code
below, to understand how this affects your parsing.

```
_hl:@"/*
 * This is some comment, that will disappear after parsing of this Hyperlisp
 */
_foo:bar1
  _nother-foo:bar2"
lisp2lambda:x:/-?value
lambda2lisp:x:/-/*
```

This means that if you convert Hyperlisp to p5.lambda, for then to convert it back to Hyperlisp, all comments, and additional spacing, is
lost during the translation.

This is a conscious choice, for among other things, preserving bandwidth and CPU resources, when evaluating p5.lambda, and also transmitting
p5.lambda over a network, etc. If you wish to keep comments and additional spacing, you will have to handle your Hyperlisp as Hyperlisp, and not
transform it to p5.lambda, for then to return it to Hyperlisp again.






