Math operations in P5
========

This plugin contains all the math operators. There are 6 basic math operators, which does more or less what you'd expect them to
do from other traditional programming languages.

* [+], adds
* [-], subtracts
* [\*], multiplies
* [/], divides
* [\^], exponent
* [%], modulo

They all logically work the same way, which means that they're Active Events, taking one or more arguments. All of them can have both
their primary value, and their arguments being expressions. An example of adding two numbers can be found below.

```
_no:int:4
+:x:/@_no?value
  _:int:1
```

After evaluating the above code, the *[+]* node will contain the integer value of "5". Below is the resulting lambda object after having evaluated
the above code.

```
_no:int:4
+:int:5
```

Notice, you can also add up multiple arguments to each math operator invocation, such as the following illustrates.

```
_no:int:2
+:x:/@_no?value
  _:int:1
  _:int:2
```

Which will yield the same result as our first example.

Each argument you supply to your math operator invocation, must have a name being *[_]*. The value can be a constant, or an expression, such as 
the following illustrates.

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

The above code will first subtract the number 1 from *[_no2]*'s value, then add that result to *[_no1]*, and put the end result into the value of 
our *[+]* node.

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
in the [p5.exp](../../core/p5.exp) project.
