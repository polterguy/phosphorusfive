Changes
===============

## Version 8.1 - Not yet released

### Slider checkbox in Micro

Support for _"slider"_ CSS class in Micro on checkbox HTML elements,
which allows for creating an _"iOS type of checkbox"_.

### Improved documentation

Improved documentation, in addition to some of the embedded videos, focusing
on making them shorter and more explicit in the way the system is being documented.
The documentation is now also arguably _"literate"_, implying that you can use
the documentation system to extract meta information about modules, significantly
improving its quality.

### Improved Hyperlambda parser

More fault tolerant Hyperlambda parser, which allows for parsing Hyperlambda
directly from a stream, as a hidden override Active Event.

### Support for [keep-comments] in [hyper2lambda]

If you pass in **[keep-comments]**, and set its value to true during your
invocation of **[hyper2lambda]**, the parser will semantically keep your comments,
and add these as a **[..comment]** node, allowing you to keep them, and see their
contents - Yet still creating a lambda object out of your Hyperlambda. This allows
you to semantically retrieve comments from a Hyperlambda snippet if you wish.

### Support for [comments] argument in [lambda2hyper]

If you pass in **[comments]**, you can override how the Hyperlambda is generated.
Legal values are as follows.

* _"unroll"_ - Will transform all __[..comment]__ nodes to its actual comment
* _"delete"_ - Will ignore all __[..comment]__ nodes
* _"keep"_ - Will treat __[..comment]__ nodes in any special ways, but keep them as they are in your lambda. This is the default value.

**Notice**, this feature only works if you supply an expression to your **[lambda2hyper]**
invocation.

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

### Meta information in documentation

There is now significantly improved meta information capabilities in the documentation parts
of your modules, allowing you to see information about your modules much more
detailed. Among other things, you can see a graph displaying comments to nodes ratio,
the module's relative size compared to other modules, see all Active Events
created by the module, and their comments and lambda contracts, display all
Hyperlambda files in your module, with Markdown comment support, etc, etc, etc.

In fact, improved documentation in general, has been the main focal point of this
release.

### Graph widgets

Created graph widgets, more specificall **[micro.widgets.chart.pie]** to display
pie charts - In addition to **[micro.widgets.chart.bar]** to display bar charts.
See the documentation for Phosphorus Five to understand how to use these widget.

### Allowing for capital letters in widget tagName

Supporting capital letter in p5.ajax project. This was necessary to allow for
things such as __[foreignObject]__ widgets, inside of for instance SVG widgets.

### Fixed bugs in p5.mime documentation

Fixed a bug that wouldn't render the documentation for the p5.mime plugins correctly.

### Parameter support to the [micro.widgets.file] widget

The **[files]** and **[folder]** arguments to the **[micro.widgets.file]** widget
can now be parametrized. See the documentation of these events for how to use
this feature.

### Changes in Aztec skin

Making _"pre"_ and _"code"_ elements slightly lighter.

