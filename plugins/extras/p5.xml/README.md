XML parsing
===============

This folder contains the Active Events necessary to parse and create XML files and snippets. There is one Active Event in this project,
that allows you to parse an XML snippet, and create a lambda structure out of it. Its name is *[xml2lambda]*. BBelow is an example of using it.

```
_xml:@"<x>
  <y attr=""foo1"" />
  <z>Some value</z>
  <y2 attr=""foo2"">
    <z2>Another value</z2>
  </y2>
</x>"
xml2lambda:x:/-?value
```

The above will result in something like the following.

```
/* ... rest of code ... */
xml2lambda
  x
    y
      @attr:foo1
    z:Some value
    y2
      @attr:foo2
      z2:Another value
```

Each XML node becomes a lambda node, with its textual value, if any, as its value. Each attribute is added as a child node, prefixed with "@",
and each children node besides that, is added as a normal lambda node child, with its name being the XML node name.

