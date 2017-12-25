Math operations in Phosphorus Five
========

This plugin contains all the math operators. There are 6 basic math operators, which does more or less what you'd expect them to
do from other traditional programming languages.

* __[+]__, adds
* __[-]__, subtracts
* __[\*]__, multiplies
* __[/]__, divides
* __[\^]__, exponent
* __[%]__, modulo

They all logically work the same way, which means that they're Active Events, taking one or more arguments. All of them can have both
their primary value, and their arguments being expressions. An example of adding two numbers can be found below.

```
_no:int:4
+:x:/@_no?value
  _:int:1
```

After evaluating the above code, the **[+]** node will contain 5. Below is the resulting lambda object after having evaluated
the above code.

```
_no:int:4
+:int:5
```

You can also add up multiple arguments to each math operator invocation, such as the following illustrates.

```
_no:int:2
+:x:/@_no?value
  _:int:1
  _:int:2
```

Which will yield the same result as our first example. Each argument you supply to your math operator invocation, must have a name 
being **[_]**. The value can be a constant, or an expression, such as the following illustrates.

```
_no1:int:5
_no2:int:2
+:x:/@_no1?value
  _:x:/@_no2?value
```

You can also nest multiple operator invocations, at which point they're evaluated in a _"depth first order"_ - Which means the inner most invocation will
be evaluated first. An example can be found below.

```
_no1:int:5
_no2:int:3
+:x:/@_no1?value
  -:x:/@_no2?value
    _:int:1
```

The above code will first subtract the number 1 from **[_no2]**'s value, then add that result to **[_no1]**, and put the end result into the value of 
our **[+]** node.

## Types and math

In theory, you can supply any type from your supported types into a math operation, as long as it has the operator overload for the specific
math operator you are using. To add two strings for instance, can be done with the following code.

```
_foo:thomas
_bar:" hansen"
+:x:/@_foo?value
  _:x:/@_bar?value
```

However, for strings, such as our above example - Probably a more suitable solution would be to use formatting expressions, which you can read about
in the [p5.exp](../../core/p5.exp#formatting-expressions) project. A more useful example, would be if you created your own type, wrapping for instance
a System type from .Net such as BigInt, or something similar - Or if you add a TimeSpan to a DateTime object.
Notice, the type of the value of whatever math operator Active Event you're using, become the _"controlling type"_, and decides the type of your result. If
you wish to make sure it has a specific type, you can use a type conversion in your expressions, referencing the value you're using as the main object.
An example can be found below.

```
_str:4
+:x:/@_str?value.int
  _:1
```

Notice also in the above example, that the string value of _"1"_, will be automatically converted into the type expected from the value of 
our **[+]** invocation. Since the expression in our **[+]** is type converted into an _"int"_, this means that the string value of _"1"_, is converted
into an integer, and added to the integer converted value of the **[_str]** node's value. Try removing the type conversion, to see a very much different
result.

```
_str:4
+:x:/@_str?value
  _:1
```

The above will simply add both the left hand side and the right hand side as simple strings. The same would be true, even if you explicitly
told the execution engine that the right hand side is of type int. Such as the following illustrates.

```
_str:4
+:x:/@_str?value
  _:int:1
```

The above illustrates how the _"left hand side"_ decides the type for the operation. The left hand side here, meaning the value of the operator
being used.

## Active Event sources

The math operators does not in any way discriminate between any Active Event and the internal math operators. If you wish, you could for instance
easily use an Active Event you create yourself, as the source for any parts of your math operators. The following illustrates an example.

```
.x
  return:1
+:int:4
  eval:x:/@.x
```

Of course, using your own Active Events here, is just was easily accomplished - As long as your event returns some simple value, convertible into
something that can legally be the right hand side of your math operation.
