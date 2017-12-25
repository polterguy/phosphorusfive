p5.types, types in Phosphorus Five
========

This library's Active Events are rarely used directly, but rather used when converting from one type to another, indirectly.
Among other things, the Hyperlambda parser is dependent upon this library, to convert from string representation, to object representation
for your objects, and vice versa. To extend the typing system, you will normally have to implement three different Active Events

* __[.p5.hyperlambda.get-object-value.my-type]__
* __[.p5.hyperlambda.get-string-value.MyTypeNamespace.MyTypeClassName]__
* __[.p5.hyperlambda.get-type-name.MyTypeNamespace.MyTypeClassName]__

The first Active Event above, is supposed to convert a string representation to the object version of your type.
The second Active Event above, is only necessary to implement if your implementation of "ToString" for your .Net type, does
not return the string representation of your type, the way it should be serialized into for instance Hyperlambda.
The third Active Event above, should simply return the value _"my-type"_, and nothing else, and is used when serialising your lambda object
into Hyperlambda. Possibly one of the better examples, you can start out with, if you wish to implement your own type, is the ExpressionType.cs 
class, which implements the "x" (p5.exp expressions). Copy this class, and implement it in your own plugin assembly if you wish to
extend the type system to include your own type.

## Types supported in Phosphorus Five

Out of the box, P5 supports most native types you'll need. Below is a list of thee types supported in P5.

* byte[], typename "blob"
* bool, typename "bool"
* byte, typename "byte"
* char, typename "char"
* DateTime, typename "date"
* decimal, typename "decimal"
* double, typename "double"
* p5 expression, typename "x"
* float, typename "float"
* Guid, typename "guid"
* int, typename "int"
* long, typename "long"
* Node, typename "node"
* Regex, typename "regex"
* TimeSpan, typename "time"
* uint, typename "uint"
* ulong, typename "ulong"

In addition to supporting the above types, p5.types also have some helper Active Events, such as **[p5.globalization.get-language-name]**, which will
transform from a culture definition, such as e.g. _"en-AU"_ into _"English, Autralian"_ etc.

### Helper Active Events for the date type

Some types also have additional helper Active Events, such as for instance DateTime, which has the **[p5.types.date.now]** and 
the **[p5.types.date.format]** Active Event, which allows you to get the current date, and format a date object, supplying 
a **[cultur]** and a **[format]**. Example code below.

```
p5.types.date.now
p5.types.date.format:x:/-?value
  format:"dddd, yyyy:MM:dd HH:mm"
p5.types.date.format:x:/-2?value
  culture:NO
```

### Helper Active Events for the guid type

The _"guid"_ type can create a new Guid. This will simply invoke `Guid.NewGuid ();` for you. Below is an example.

```
p5.types.guid.new
```

### Common helper Active Events

The **[p5.types.can-convert]** Active Event, determines if some value can somehow legally be converted into something else, or if a conversion attempt
will throw an exception. Example can be found below.

```
_foo:no-conversion-possible
p5.types.can-convert:x:/-?value
  type:date
_foo2:555
p5.types.can-convert:x:/-?value
  type:int
```

In the above example the first invocation to **[p5.types.can-convert]** will yields false, while the second invocation will yield true.
