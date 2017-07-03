JSON support in Phosphorus Five
===============

**This document describes features not yet released in P5, but coming up in the next release!**

This folder contains the Active Events necessary to parse and create JSON. There are 2 Active Events in this project.

* __[json2lambda]__ - Creates one or more lambda objects from one or more JSON snippets
* __[lambda2json]__ - Creates a JSON snippet from one or more lambda objects.

Both of these Active Events can take expressions, leading to what you'd like to transform. Below you can see an example of 
using **[json2lambda]** to convert a JSON snippet to a lambda object.

```
json2lambda:@"{
  'name':'John Doe',
  'address':{
    'zip':5789,
    'str':'Dunbar Road'
  },
  'list':[
    57,
    77, {
      'foo':'bar'
    }
  ]
}"
```

The above will result in something like the following.

```
json2lambda
  result
    name:John Doe
    address
      zip:long:5789
      str:Dunbar Road
    list
      :long:57
      :long:77
      foo:bar
```

Notice, each JSON snippet you transform like this, will end up in a separate **[result]** node as illustrated above. This is because you can
use **[json2lambda]** to transform multiple JSON objects at the same time, by supplying an expression leading to multiple JSON snippets.

Below you can see an example of using **[lambda2json]** to go the opposite way.

```
lambda2json
  name:John Doe
  address
    zip:long:5789
    str:Dunbar Road
  list
    :long:57
    :long:77
```

Which of course results in the following.

```
lambda2json:@"{""name"":""John Doe"",""address"":{""zip"":5789,""str"":""Dunbar Road""},""list"":[57,77]}"
```

You can also use expressions leading to multiple node results for the **[lambda2json]** event. However, if you do, it will create only
one JSON object for you.

## Creating arrays

When the parser checks to see if it should create an array or a complex object, it will check the name of the first node, and if empty, it will
assume the caller wants to create an array.

```
lambda2json
  :bar
  :other-bar
```

Which will result in the following.

```
lambda2json:@"[""bar"",""other-bar""]"
```

Alternatively something like the following.

```
lambda2json
  foo
    :bar
    :other-bar
```

Which would result in a complex object having a property named _"foo"_, being a JSON array. Below is the JSON result of the above code.

```
lambda2json:@"{""foo"":[""bar"",""other-bar""]}"
```

## HOWTO create a specific JSON structure

If you wish to create a simple JSON array, you can do that by simply appending the values of your array as the value of the root node.

```
lambda2json
  :long:1
  :long:2
  :long:3
```

Which will result in the following.

```
lambda2json:[1,2,3]
```

You can also create a property of another node being an array, like the following illustrates.

```
lambda2json
  foo:bar
  array
    :thomas
    :hansen
```

Which results in the following.

```
lambda2json:@"{""foo"":""bar"",""array"":[""thomas"",""hansen""]}"
```

If you wish to create an array of complex objects, you can do that with the following.

```
lambda2json
  ""
    prop1:val1
    prop2:val2
  ""
    prop1:val1
    prop2:val2
```

Which results in the following

```
lambda2json:@"[{""prop1"":""val1"",""prop2"":""val2""},{""prop1"":""val1"",""prop2"":""val2""}]"
```

Of course, you can have an array of complex objects being a member of another object, like the following illustrates

```
lambda2json
  foo:bar
  array
    ""
      prop1:val1
      prop2:val2
    ""
      prop1:val1
      prop2:val2
```

Which results in the following.

```
lambda2json:@"{""foo"":""bar"",""array"":[{""prop1"":""val1"",""prop2"":""val2""},{""prop1"":""val1"",""prop2"":""val2""}]}"
```

You can also mix different types in your arrays, such as the following illustrates.

```
lambda2json
  foo:bar
  array
    :hello
    :world
    prop-x:val-y
    ""
      prop1:val1
      prop2:val2
```

Which results in the following.

```
lambda2json:@"{""foo"":""bar"",""array"":[""hello"",""world"",{""prop-x"":""val-y""},{""prop1"":""val1"",""prop2"":""val2""}]}"
```

Notice, the above example, also illustrates how you can create a simple object as an array member, having only one property, by shorthanding
the array object with its single property being the value of the array member, and the name of the property its name. This is because once
the **[lambda2json]** event has started iterating an array JSON construction, it will assume every single child of the root array node is
an object in that array. You can see this effect on the _"prop-x"_ above. Hence, if you wanted to create a JSON array of complex objects,
having only one property, you could do that by explicitly typing out your first array as a rich object, and have all consecutive array
objects being simple key/value pairs. The following illustrates how to accomplish this.

```
lambda2json
  ""
    prop1:val-x
  prop1:val-y
  prop1:val-z
```

Which would result in the following.

```
lambda2json:@"[{""prop1"":""val-x""},{""prop1"":""val-y""},{""prop1"":""val-z""}]"
```

### Nicely indenting your JSON

There is an override to **[lambda2json]** which is called **[p5.json.lambda2json.indented]**. The latter will do the exact same thing as the
first, except it will return the JSON nicely indented. Compare the results of these two different invocation, having the exact same arguments.

```
lambda2json
  :foo
  :bar
p5.json.lambda2json.indented
  :foo
  :bar
```

The results of the above code should resemble the following.

```
lambda2json:@"[""foo"",""bar""]"
p5.json.lambda2json.indented:@"[
  ""foo"",
  ""bar""
]"
```

The last invocation of course, being nicely indented, and more easy to read on the eye. Which might be helpful during debugging sessions among other things.

## Concerns

**Notice**; There's a mismatch between the structure of a JSON object and a lambda object. A JSON object is a simple key/value object, while
a lambda object is a key/value/children object. This mismatch implies that not everything that is possible to describe in lambda, is possible 
to describe with the same structure in JSON. For instance, if a lambda object has both a value and a children collection, the value must be
stored in JSON, together with its children properties. Below is an example of how this might look like if converting from lambda to JSON.

```
lambda2json
  foo:bar-value
    child-1:value-1
    child-2:value-2
```

Which will end up looking like the following.

```
lambda2json:@"{""foo"":{""__value"":""bar-value"",""child-1"":""value-1"",""child-2"":""value-2""}}"
```

Notice how the *"__value"* node above becomes a property of our _"foo"_. When you go the other way, from JSON to lambda, a *"__value"* JSON
property, will be automatically assumed to be the value of your lambda object. Hence, transforming a piece of lambda to JSON, and back to 
lambda again, will always return the same resulting structure as you started out with.

There are also other difficulties, such as JSON not preserving any type information. This implies that if you convert from a lambda object having
an integer value to JSON, and then back again - During the conversion back to lambda, the JSON parser will assume your integer value
is actually a _long_ type. Hence, all integer values when parsed from JSON to lambda will be typed as _long_ types, and all floating point values 
will be typed as _double_.

This is an integral weakness with JSON, which is difficult if not impossible to solve, when going from JSON to lambda, since JSON doesn't in 
any ways preserve type information for its values.

Another weakness with JSON which doesn't apply to lambda, is that you can't have two properties with the same name. For instance, the following
will throw an exception during evaluation for you.

```
lambda2json
  foo:bar
  foo:other-bar
```

These weaknesses ignored, the JSON support in Phosphorus Five is in general terms quite strong, using Newtonsoft's JSON.Net library beneath its 
hoods - And should be able to create any JSON object you would want to create, and/or parse any JSON object you'd encounter out there.

Notice, we could of course have serialized the lambda structure directly, re-creating the entire hierarchy, to defeat the above problems.
However, since the **[lambda2json]** event will probably mostly be used to create JSON which is meant to interact with other services, I feel at the
moment, that being able to create a JSON structure, resembling the JSON you'd eventually would want to end up with, is more
important than being able to create a perfect serialization method for lambda objects as such. This makes it easy for you to create JSON structures,
which you for instance use as return values from web services and such, interacting with JSON clients - While at the same time, it makes 
it impossible to by the very definition of these mismatches to be able to serialize *all* lambda objects due to these structural deifferences, 
and you'll have to be careful when structuring your lambda objects, if you want them to be serialized to JSON.

Basically what this means, is that **any JSON object can be de-serialized to a lambda object, but the opposite is not necessarily true**.
In future versions, there might be created an override, which perfectly preserves the lambda object's structure, which would be possible by 
serializing the lambda object directly as is. However, since this would create a much more _"noisy"_ JSON structure, more difficult to understand for
clients consuming it - I have postponed this for future versions of Phosphorus Five, to focus on what I feel is of most importance at the moment.

**Rule of thumb**

* You can create any JSON structure you wish by carefully structuring your lambda
* You can parse any JSON structure into a lambda
* You can _not necessarily_ transform all lambda objects into JSON
* You can _not necessarily_ create any lambda object structure you wish from JSON

However, these traits are on my TODO list for future versions, which might have some sort of alternative serialization events,
for solving also the latter two above problems from the above list.

