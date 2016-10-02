System42, the answer to life, the universe and everything
========

System42 is an example minimalistic CMS, built on top of Phosphorus Five. It also includes 
several "helper components". If you wish to use Phosphorus Five as a stand alone framework, you
should delete the entire System42 folder, and modify the "p5.webapp.application-startup-file"
setting in your web.config, beneath your p5.webapp folder. This allows you to create web
apps from scratch, having complete control over every aspect of your systems.

However, you are probably better off simply deleting the apps and components you do not want 
to use, since there are many useful "helper components" in System42. Such as "app-settings"
for instance.

The purpose of System42, is to provide a starting point and example implementation,
when developing P5 apps. In addition to being a quite useful by itself, implementing a CMS
for you, in a nice graphical environment.

System42, is also a "host" for your web apps, by allowing you to install your apps.
To install an app into System42, is as simple as an x-copy operation, pasting your "app" 
into the "/system42/apps/" folder, with optionally one file called "launch.hl",
and another file called "startup.hl" - And your app should be ready for use.

The "startup.hl" file is evaluated when the server is started, and used for initializing
your app, by creating supporting Active Events, and so on. The "launch.hl" file,
is expected to "launch" your app, whatever that means for your specific app.

## Structure

All files belonging to System42 is contained in the "system42" folder. The 
"application-startup.hl" file is executed when application pool starts in web server.
This file creates a couple of helper Active Events, for executing Hyperlisp files, and
folders. In addition, it executes all files in the "/system42/startup/" folder.

When it is done with the above tasks, it will evaluate all "application specific"
startup files, which are files inside of any "/system42/apps/xxx/" folders who's names
are "startup.hl". Before it evaluates all "component specific" startup file, found
in "/system42/components/xxx/", and named "startup.hl".

This allows you to create startup Hyperlisp scripts, both on system level, and on
application level, for apps and components that needs some sort of startup/initialization 
scripts to be evaluated.

### The "apps" folder

This is the System42 version of your windows "C:/Program Files/" folder, and contains 
all apps in your System42 installation. See the documentation for the [apps](/core/p5.webapp/system42/apps/)
folder for details about this folder.

### The "components" folder

This is the System42 version of your windows "COM objects", and contains components in 
your System42 installation, which you can reuse in your apps. See the documentation for 
the [components](/core/p5.webapp/system42/components/) folder for details about this folder.

### The "installation" folder

This folder contains scripts that are to be evaluated when server is initially
setup. It allows you to create a server-seed, which is used to seed the cryptographic random
number generator, and salt your hashed passwords etc. In addition, it creates a root
password, and a superuser normal account, for everyday use of your system.

### The "startup" folder

This folder contains files that are evaluated during application startup of your server,
either because of your server rebooting, or because of the web-server process being restarted
for some reasons.

The most important file in this folder is the file called "p5.web.load-ui.hl", which
declares the Active Event called *[p5.web.load-ui]*, which is the Active Event that maps
URLs to *[p5.page]* objects in the P5 database. This event is invoked from the core of Phosphorus 
Five during an initial loading of a page's URL. If you create your own system, from scratch, without
any of the System42 features - This is the Active Event you are expected to create yourself,
to load URLs from HTTP requests.

### Helper Active Events

Even if you choose to entirely delete System42, and the CMS, to create your own host from
scratch. There are still some helper Active Events you should consider keeping.

Some of them are mentioned below.

* [sys42.get-event] - Returns the p5.lambda code for one or more dynamically created Active Events
* [sys42.get-widget] - Returns all p5.lambda associated with a widget, including events
* [sys42.execute-lisp-file] - Executes one or more Hyperlisp files
* [sys42.execute-lisp-folder] - Executes all Hyperlisp files in a folder, recursively
* [sys42.include-default-javascript-files] - Which includes jQuery and Bootstrap's JavaScript files for you
* [sys42.include-default-stylesheet-files] - Which includes Bootstrap's CSS files, and the "default" CSS files for you
* [sys42.empty-user-tmp-folder] - Which empties a user's "temp" folder. Invoked when a user logs out

In addition, System42 contains a web based Hyperlisp editor, built on top of CodeMirror, an HTML editor,
built on top of CKEditor, and other things, that might help you out, when creating your own apps.

System42 also contains a suite of Unit Tests, to test the integrity of Phosphorus Five. Which is highly
extendible, such that you can create your own tests, and plug them into the existing suite.



