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

After evaluating the above code, the *[+]* node will contain the integer value of "5".
