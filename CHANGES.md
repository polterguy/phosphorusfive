Changes
===============

## Version 8.1

### Slider checkbox in Micro

Support for _"slider"_ CSS class in Micro on checkbox HTML elements,
which allows for creating an _"iOS type of checkbox"_.

### Improved documentation

Improved documentation, in addition to some of the embedded videos, focusing
on making them shorter and more explicit in the way the system is being documented.

### Improved Hyperlambda parser

More fault tolerant Hyperlambda parser, which allows for parsing Hyperlambda
directly from a stream, as a hidden override Active Event.

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
