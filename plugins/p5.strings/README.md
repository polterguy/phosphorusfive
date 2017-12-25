﻿﻿﻿String manipulation in Phosphorus Five
===============

p5.strings contains the core string manipulation Active Events of Phosphoris Five. Below you can see them all, together with examples
of how to use them.

## [index-of]

The **[index-of]** Active Event, returns the index of some search query, from within a string, or optionally, an expression's value, converted
into a string somehow. It requires a **[src]** argument, which is what to look for. Consider this code.

```
_data:Thomas Hansen is the creator of Phosphorus Five
index-of:x:/-?value
  src:Hansen
```

If you evaluate the above Hyperlambda, it will result in the following.

```
_data:Thomas Hansen is the creator of Phosphorus Five
index-of
  Hansen:int:7
```

As you can see, the original arguments are gone, and the index of the value _"Hansen"_ is returned as a child node. If there was multiple
occurrencies of the value _"Hansen"_, it would result in multiple return values. One for each occurrency of the value _"Hansen"_.

```
_data:Thomas Hansen is the creator of Phosphorus Five. Hansen's email is mr.gaia@gaiasoul.com
index-of:x:/-?value
  src:Hansen
```

Which of course results in.

```
_data:Thomas Hansen is the creator of Phosphorus Five. Hansen's email is mr.gaia@gaiasoul.com
index-of
  Hansen:int:7
  Hansen:int:49
```

The **[src]** argument, can also be an expression. For instance like this.

```
_data:Thomas Hansen is the creator of Phosphorus Five. Hansen's email is mr.gaia@gaiasoul.com
_src
  no1:Phosphorus
index-of:x:/@_data?value
  src:x:/@_src/*?value
```

The above lambda, would result in the following result.

```
_data:Thomas Hansen is the creator of Phosphorus Five. Hansen's email is mr.gaia@gaiasoul.com
_src
  no1:Phosphorus
index-of
  :int:32
```

The search is case-sensitive. If you wish to do a case-insensitive search, you need to resort to a regular expression type of source.

### Regular expressions in [index-of]

You can also use a regular expression as your source. Consider this code.

```
_data:Thomas Hansen are the same letter as hansen, thomas
index-of:x:/-?value
  src:regex:/hansen/i
```

The above matches both occurrencies of _"Hansen"_, since the regex we supplied, is created with the _"case-insensitive"_ flag set to true, du to the
last `/i` part of our regular expression. Any regular expressions you wish to use, can be used here. The above is just a simple example. If you create 
very complex regular expressions, containing for instance a _":"_, etc, you might have to put your regular expression value double quotes. You can also 
have your source being any Active Event you wish, that somehow returns one or more valid sources for **[index-of]**. Consider this.

```
_data:Hansen are the same letters as hansen, but only one will suffice here
index-of:x:/-?value
  eval
    return:hansen
```

## [join]

The **[join]** Active Event, allows you to join the results of an expression for instance, into a single string value. Consider this code.

```
_data
  foo1:Hello
  foo2:" "
  foo3:World
join:x:/-/*?value
```

The above would produce _"Hello World"_ after being evaluated. You can also pass in two optionally arguments, **[wrap]** and **[sep]**, which are
_"wrapper character(s)"_ and _"separator character(s)"_. The separator characters is a piece of string, which is added between your entries, while 
the _"wrapper"_ mostly makes sense if you also supply a _"separator"_, since it wraps each entity. Consider this code.

```
_data
  foo1:bar1
  foo2:bar2
  foo3:bar3
join:x:/-/*?value
  sep:,
  wrap:'
```

The above lambda, will produce the following result.

```
... rest of code ...

join:'bar1','bar2','bar3'
```

This is useful for creating for instance CSV files, etc. Notice that both the value of **[join]**, and both of its arguments, can be both 
constants and expressions. You can also join results of expressions yielding multiple results. Consider this.


```
_data
  foo1:bar1
  foo2:bar2
_data
  foo3:bar3
  foo4:bar4
join:x:/../*/_data/*?value
  sep:,
  wrap:'
```

In the above lambda, the expression given to **[join]**, will yield multiple results. The Active Event doesn't care, but treats it as a single
result.

## [length]

The Active Event **[length]**, simply returns the length of a string. If it is given something which is not a string, it will convert it into 
a string, and return its length anyway, as if it was a string. Consider this code.

```
_data:This is a string
length:x:/-?value
```

Which of course yields the value of 16. Like **[join]**, you can give it an expression leading to multiple results.

## [match]

The **[match]** Active Event is similar to **[index-of]** in its regular expression implementation, except instead of returning the index of some
string, it returns the string it matches. Consider this code.

```
_data:Thomas Hansen
match:x:/-?value
  src:regex:/hansen/i
```

Which will return the following.

```
_data:Thomas Hansen
match
  Hansen
```

Notice the _"i"_ parameter passed into the regular expression, informing it that we want to perform a case-insensitive search.
The **[match]** Active Event will also automatically convert anything in its value to a single string. Just like **[length]** and **[join]** will.

## [replace]

The **[replace]** Active Event takes a **[src]**, and a **[dest]** argument. It basically replaces every occurrency of some source string, found
in the expression or constant of its value, with whatever you supply as a **[dest]** argument, and return it immutably as the value of **[replace]**.
Consider this code.

```
_data:Phosphorus Five is a stupid framework
replace:x:/-?value
  src:stupid
  dest:cool
```

## [split]

Split does the opposite of **[join]**, and splits a string, according to some condition, into multiple resulting strings. Consider this.

```
_data:My name is Thomas-Hansen
split:x:/@_data?value
  =:" "
```

Split can take multiple **[=]** arguments, such as the following illustrates.

```
_data:My name is Thomas-Hansen
split:x:/@_data?value
  =:" "
  =:-
```

At which point, it will split on each character supplied as a splitting character. Both the arguments, and its source, can be either an expression,
or a constant value.

## [to-lower]

The **[to-lower]** Active Event, will make sure any given string is turned into lower case. Consider this.

```
_data:Thomas Hansen is COOL!
to-lower:x:/-?value
```

Which of course will result in the following.

```
to-lower:thomas hansen is cool!
```

## [to-upper]

This event does the opposite of **[to-lower]**. Below is an example of usage.

```
_data:Thomas Hansen is COOL!
to-upper:x:/-?value
```

Notice, both the **[to-lower]** and the **[to-upper]** will use the _"invariant culture"_ when converting your strings. In fact, so does every single
string manipulation Active Event in P5 by default.

## [trim], [trim-left] and [trim-right]

These events does exactly what you think they do. They trim a string, either on its left side, right side, or both sides. Consider the following code.

```
_data:"   Thomas Hansen foo bar     "
trim:x:/@_data?value
trim-left:x:/@_data?value
trim-right:x:/@_data?value
```

All of these events can optionally take a **[chars]** argument, which if given, will be the characters removed. The default value of this argument
is _" \r\n\t"_, which will simply trim all whitespace characters. However, if overridden, it will not remove its default characters, but rather
the characters you explicitly specify. An example is given below.

```
_data:--==--Thomas Hansen--==--
trim:x:/-?value
  chars:-=
```

Which of course results in the following

```
trim:Thomas Hansen
```

## Base64 encoding and decoding

The **[p5.string.decode-base64]** Active Event, allows you to decode a piece of base64 encoded data. This can be useful for scenarios
where you are somehow given base64 encoded data, and need to retrieve its original value. Usage is as simple as the following illustrates.

```
_data:dGhvbWFzIGhhbnNlbiB3YXMgaGVyZQ==
p5.string.decode-base64:x:/-?value
src:x:/@p5.string.decode-base64?value.string
```

Notice, since **[p5.string.decode-base64]** will actually return a byte array, or a _"blob"_, we need to convert this blob into a string to actually
see its result. The above example hence simply base64 decodes a value, which contains the text _"thomas hansen was here"_ into a blob, for then
to use a **[src]** invocation, with an expression, converting its result into a string.

To encode a blob into a base64 encoded string, is just as easy.

```
_data:blob:dGhvbWFzIGhhbnNlbiB3YXMgaGVyZQ==
p5.string.encode-base64:x:/-?value
```
