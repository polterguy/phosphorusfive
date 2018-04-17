Changes
===============

## Version 8.1 - Not yet released

### Slider checkbox in Micro

Support for _"slider"_ CSS class in Micro on checkbox HTML elements,
which allows for creating an _"iOS type of checkbox"_.

### Improved documentation

Improved documentation, in addition to some of the embedded videos, focusing
on making them shorter and more explicit in the way the system is being documented.

### Improved Hyperlambda parser

More fault tolerant Hyperlambda parser, which allows for parsing Hyperlambda
directly from a stream, as a hidden override Active Event.

### Support for [keep-comments] in [hyper2lambda]

If you pass in **[keep-comments]**, and set its value to true during your
invocation of **[hyper2lambda]**, the parser will semantically keep your comments,
and add these as a **[..comment]** node, allowing you to keep them, and see their
contents - Yet still creating a lambda object out of your Hyperlambda. This allows
you to semantically retrieve comments from a Hyperlambda snippet if you wish.

### [micro.windows.confirm] convenience wrapper

Created a convenience wrapper for confirmation types of [modal.widgets.modal]
widgets, which simply takes an **[onok]** lambda callback, in addition to (optionally)
a **[header]** and a **[body]**. If user clicks its _"Yes"_ button, only then
its associated **[onok]** lambda callback is evaluated.

### Pulled in unit tests from System42

Pulled in Unit Tests from System42, as a separate sub project, which can be
found [here](https://github.com/polterguy/phosphorus-unit-tests). This makes
it easier to evaluate the unit tests, verifying the integrity of the system's
core functionality. This is one of the last remaining pieces from System42 that
has relevance for the project, and hence System42 will probably soon be completely
removed.

### More meta information in documentation

There is now additional meta information capabilities in the documentation parts
of the Desktop module, allowing you to see information about the module, much more
detailed.

### Graph widgets

Created graph widgets, more specificall **[micro.widgets.graph.pie]**, to display
pie charts.
