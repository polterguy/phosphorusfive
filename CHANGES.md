Changes
===============

## Version 8.4 - Released the 22th of June 2018

### General information

The main focal points of this release have been security and PGP/MIME. This is
a major release, with more than 52 commits since the previous release. Lots of
interesting PGP features, in addition to lambda web services, and better security.

### Cleaning up some event names in p5.auth - *BREAKING CHANGE*

Changing some of the p5.auth related Active Event names. Notice, this unfortunately
does provide some backward compatibility problems in case you happen to be using
these events. Make sure you test your existing code for backward compatibility
problems, and refer to the documentation for p5.auth for these events new names.

Hoperfully I'll get to keep these types of changes to a minimum in the future,
though I figured the existing user base wasn't too large for this to pose a
problem at the time being, while also feeling it's justified to create more intuitive
event names.

### Cleaning up some event names in p5.crypto - *BREAKING CHANGE*

Changing some of the p5.crypto Active Event names to make them obey by the general
naming convention in Phosphorus Five. Sorry about this ...

### Not allowing "funny" filenames or folder names - *BREAKING CHANGE*

Disallowing file and folder names with non Latin characters. Alowing a-z, 0-1, -,
\_ and / (obviously) in file names and folder names. This is to avoid adversaries
attempting to add funny paths to be able to access file objects outside of the
normal file system accessible for a Phosphorus Five application, such as files
with the name of _"/../"_ and similar constructs. Unfortunately this implies you
can no longer use UNICODE characters, or non-Latin letters in filenames.

### Password entropy changes

Changed the default password regime to require at least 25 characters, ancouraging
the users to create entire sentences instead of using simply characters.
This can be overridden though by easily changing the password rules in the web.config,
if you're determined to use a weaker type of password entropy regime.

### Using Blowfish slow hashing for passwords

Implemented slow hashing with bcrypt for passwords in auth file.

### Echoing MIME directly to the response stream

Allowing for using **[p5.web.echo-mime]** to echo a MIME envelope directly unto
the HTTP response stream. Also beefed up the Hypereval lambda web service example
to illustrate this idea.

### Refactoring and cleaning up p5.auth

Cleaned up p5.auth, and refactored it, making its code more easily understood,
and cleaner. Removed several issues in the process, that might create problems.

### Precedence of access objects in p5.auth

There were some flaws in how p5.auth determined precedence of access object,
which is now fixed.

### Removing Guids in Peeples

Removed the automatically generated Guid IDs of access objects in Peeples, since
they simply add to the cognitive noise of the module, and provide no important
contextual value for the end user.

### Convenience authorization events

Added 4 convenience Active Events to check if a user is allowed to read or
write to a file or folder, which is useful if you need to check this, before
some IO operation is attempted. See the documentation for p5.io.authorization
for details.

### PGP lambda web services

This is the by far most important change in this release, since it allows you
to easily create and consume PGP _"lambda web services"_, where the client
supplies the code to be executed on the web service endpoint securely, by
enforcing the invocation to be a cryptographically PGP signed MIME envelope.

Both client side wrappers and server side wrapper (Hypereval) have been created
for this.

### Better management of PGP keys

There are several new helper methods, among other things URL resolvers in Micro,
to allow clients to more easily retrieve PGP keys from the a P5 server. In addition,
the internal PGP fetures of Phosphorus Five will now automatically contact a
key server, if it receives a MIME envelope that has been cryptographically signed,
to download the public PGP key to verify the signature. Which key server you want
to use can be configured in web.config - But the default value uses Ubuntu's key
server. During installation of the server, you can also choose to explicitly submit
your server's public PGP key to the same key server.

## Version 8.3 - Released the 6th of June 2018

### Security

Security has been significantly tightened for this release, making sure the
password file (_"auth.hl"_ file) is being encrypted among other things. In
addition one potential SQL injection hole has been fixed in the core. In addition
the system has been changed such that it no longer communicates that it is an
ASP.NET webapp. This is done to give a potential adversary less to work with
when attempting to crack the system.

## Version 8.2 - Released the 26th of May 2018

### Hyper SQL

Hyper SQL was added in this release. Hyper SQL is a small MySQL admin type of
application, allowing you to administrate your MySQL databases.

## Version 8.1.1 - Released 19th of May 2018

### Core

#### Fixed severe access control bug

Fixed severe bug in access control logic of URL resolver for Desktop module,
that would allows access to any URL, as long as you have access to the root
module's URL.

#### Changed default connection string

Changed the default connection string in web.config, to make it work
automatically in source code version with the latest release of MySQL.

#### Fixed build script

The build script wouldn't include the font files for Micro. This should now
be fixed.

### Documentation

#### Documentation improvements

Removed references to deleted YouTube videos.

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
