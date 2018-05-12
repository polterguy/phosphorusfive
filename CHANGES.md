Changes
===============

## Version 8.1 - Released 11th of May 2018

### Core changes

#### Improved Hyperlambda parser

More fault tolerant Hyperlambda parser, which allows for parsing Hyperlambda
directly from a stream, as a hidden override Active Event.

#### Support for [keep-comments] in [hyper2lambda]

If you pass in **[keep-comments]**, and set its value to true during your
invocation of **[hyper2lambda]**, the parser will semantically keep your comments,
and add these as a **[..comment]** node, allowing you to keep them, and see their
contents - Yet still creating a lambda object out of your Hyperlambda. This allows
you to semantically retrieve comments from a Hyperlambda snippet if you wish.

#### Support for [comments] argument in [lambda2hyper]

If you pass in **[comments]**, you can override how the Hyperlambda is generated.
Legal values are as follows.

* _"unroll"_ - Will transform all __[..comment]__ nodes to its actual comment
* _"delete"_ - Will ignore all __[..comment]__ nodes
* _"keep"_ - Will treat __[..comment]__ nodes in any special ways, but keep them as they are in your lambda. This is the default value.

**Notice**, this feature only works if you supply an expression to your **[lambda2hyper]**
invocation.

#### New Active Events in p5.io

Created a set of new events in p5.io, which can be found below.

- __[p5.io.folder.get-creation-time]__
- __[p5.io.folder.get-last-access-time]__
- __[p5.io.folder.get-last-write-time]__
- __[p5.io.folder.get-length]__

These functions similarly to their _"file"_ counterparts, and their documentation
can be found in the documentation for p5.io.

#### Allowing for capital letters in widget tagName

Supporting capital letter in p5.ajax project. This was necessary to allow for
things such as __[foreignObject]__ widgets, inside of for instance SVG widgets.

#### Improved documentation

Improved documentation, in addition to some of the embedded videos, focusing
on making them shorter and more explicit in the way the system is being documented.
The documentation is now also arguably _"literate"_, implying that you can use
the documentation system to extract meta information about modules.

Also fixed several _"bugs"_ in different parts of the documentation, such as
the documentation for p5.mime, which had an error in it.

### Micro changes

#### Slider checkbox in Micro

Support for _"slider"_ CSS class in Micro on checkbox HTML elements,
which allows for creating an _"iOS type of checkbox"_.

#### Deletion of [micro.lambda.create-timeout] lambda objects

Support for explicitly naming a timer lambda, which allows for deleting it
by deleting its associated widget. This event also now optionally takes
a __[parent]__ argument, allowing for _"auto destruction"_ of the timer,
as its container widget is deleted.

#### [micro.windows.confirm] convenience wrapper

Created a convenience wrapper for confirmation types of [modal.widgets.modal]
widgets, which simply takes an **[onok]** lambda callback, in addition to (optionally)
a **[header]** and a **[body]**. If user clicks its _"Yes"_ button, only then
its associated **[onok]** lambda callback is evaluated.

#### Automatic cover widget for [micro.widgets.upload-button]

Implemented a __[micro.widgets.cover]__ widget while uploading files to the server,
since this is an operation that highly likely will require some time.

#### Graph widgets

Created graph widgets, more specificall **[micro.widgets.chart.pie]** to display
pie charts - In addition to **[micro.widgets.chart.bar]** to display bar charts.
See the documentation for Phosphorus Five to understand how to use these widget.

#### MySQL 'datagrid' wrapper widget

The __[micro.widgets.mysql.datagrid]__  which is new in this release, allows you
to wrap a MySQL database table, easily, by simply declaring your columns.

#### Parameter support to the [micro.widgets.file] widget

The **[files]** and **[folder]** arguments to the **[micro.widgets.file]** widget
can now be parametrized. See the documentation of these events for how to use
this feature.

#### Improvements in skins

Fixed lots of minor design artifacts in several of the skins.

Changed the way skins are automatically created in Hyper IDE,
allowing for minifying your _"micro.css"_ file directly, without
this having any consequences for you during the _"create new skin wizard"_.

### Camphora Five changes

#### Completely refactored

Camphora Five is now completely refactored, and has tons of new features.
Among other things, a Camphora Five app can now be instantiated as an extension
widget, allowing you to inject a Camphora app into your own modules, by simply
instantiating a Camphora app as an extension widget. See the documentation for
Camphora Five for more details.

Camphora Five also now takes advantage of the __[micro.widgets.mysql.datagrid]__
extension widget, significantly simplyfying it. In addition, a Camphora Five app
is now literally _"generated"_, and not relying upon the _"app-manifest.hl"_
file anymore, making it much easier to modify a Camphora app after having generated
it, to customise it as you see fit.

#### Create (complex) views without coding

Camphora Five now also have the ability to create _"views"_ by following a
_"wizard"_, almost completely eliminating the need to create code yourself.
In general, Camphora is now a fully fledged _"Code Builder"_.

There are several views now in Camphora, solving all sorts of needs, ranging
from displaying statistical data, to creating _"micro blogs"_.

#### Rich API for Camphora Five

Camphora Five apps now have rich APIs automatically generated, allowing you to
directly interact with your app, using widget lambda events, allowing you to
implement your own support for paging, filtering, etc.

### Hyper IDE

#### Fixed visual artifacts

Fixed two bugs which the first one made the file toolbar disappear when you
renamed a file. The second bug that was fixed was when deleting a folder,
and you had one or more files open from that folder in an editor, at which
point the secondary toolbar would be disabled.

### Misc.

#### Pulled in unit tests from System42

Pulled in Unit Tests from System42, as a separate sub project, which can be
found [here](https://github.com/polterguy/phosphorus-unit-tests). This makes
it easier to evaluate the unit tests, verifying the integrity of the system's
core functionality. This is one of the last remaining pieces from System42 that
has relevance for the project, and hence System42 will probably soon be completely
removed.

#### Source code build script

There is now a _"Cake build"_ script for Phosphorus Five, allowing you to (reproduce)
a source code release, by executing a simple script. This file is called _"build.cake"_.
