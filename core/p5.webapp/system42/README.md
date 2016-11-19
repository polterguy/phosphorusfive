System42, a non-CMS
========

System42 is an example minimalistic CMS, built on top of Phosphorus Five. It also includes 
several helper components.

The purpose of System42, is to provide a starting point, and example/reference implementation,
when developing Phosphorus Five applications.

System42, is also a "host" for your web apps, by allowing you to install your apps.
To install an app into System42, is as simple as an x-copy operation, pasting your app 
into the "/system42/apps/" folder, with optionally one file called "launch.hl",
and another file called "startup.hl" - And your app should be ready for use.

The "startup.hl" file is evaluated when the server is started, and used for initializing
your app, by creating supporting Active Events, and so on. The "launch.hl" file,
is expected to "launch" your app, whatever that means for your specific app.

If your app has a "startup.hl" file, you'd probably want to evaluate this file after the x-copy
operation.

The "components" folder works similarly, except it doesn't install an "app", but rather a "component", which
might be either a visual widget, or a supporting Active Event, or anything in between.

## Structure

All files belonging to System42 is contained in the _"/system42/"_ folder. The _"application-startup.hl"_ 
file is evaluated when your application pool starts. This file creates a couple of helper Active Events, 
for evaluating Hyperlambda files, and folders. In addition, it evaluates all files in the "/system42/startup/" folder.

When it is done with the above tasks, it will evaluate all "application specific" and "components specific"
startup files. These are files inside of any "/system42/apps/xxx/" and "/system42/components/xxx/" folders, 
who's names are "startup.hl".

This allows you to create startup Hyperlambda scripts, both on system level, and on application/component level, 
for apps and components, that needs some sort of startup/initialization scripts, to be evaluated during installation,
or during startup.

### The "apps" folder

This is the System42 version of your windows "C:/Program Files/" folder, and contains 
all apps in your System42 installation. See the documentation for the [apps](apps/)
folder for details about this folder.

### The "components" folder

This is the System42 version of your windows "COM objects", and contains components in your System42 installation, 
which you can reuse in your apps. See the documentation for the [components](components/) folder for details about this folder.

### The "startup" folder

This folder contains files that are evaluated during application startup of your server, either because of your 
server rebooting, or because of the web-server process being restarted for some reasons.

### Helper Active Events

Even if you choose to entirely delete System42, and the CMS, to create your own host from scratch - There are still some helper 
Active Events you should consider keeping.

Some of them are mentioned below.

* [sys42.utilities.execute-lambda-file] - Executes one or more Hyperlambda files
* [sys42.utilities.execute-lambda-folder] - Executes all Hyperlambda files in a folder, recursively
* [sys42.utilities.get-event] - Returns the p5.lambda code for one or more dynamically created Active Events
* [sys42.utilities.get-widget] - Returns all lambda associated with a widget, including events (reverse engineers a widget)
* [sys42.utilities.empty-user-temp-folder] - Which empties a user's "temp" folder

System42 might also contain other supporting Active Events, depending upon which types of apps and/or components you have in your specific
installation. In addition, System42 contains a web based Hyperlambda editor, built on top of CodeMirror, an HTML editor,
built on top of CKEditor, and other things, that might help you out, when creating your own apps. Including many helper widgets, such
as Ajax TreeViews, Ajax TabControls, Ajax DataGrids, and so on. System42, also by default, includes jQuery and Bootstrap for your
convenience.

System42 also contains a suite of Unit Tests, to test the integrity of Phosphorus Five. Which is highly extendible, such that you can 
create your own tests, and plug them into the existing suite.

### Rolling your own from scratch

If you wish to use Phosphorus Five as a stand alone framework, you should delete the entire _"/system42/"_ folder, 
and modify the _".p5.webapp.application-startup-file"_ setting in your web.config, beneath your p5.webapp folder. 
This allows you to create web apps from scratch, having complete control over every aspect of your systems.

If you do, you are responsible for creating your own *[p5.web.load-ui]* Active Event, to load pages, according to URLs.

However, you are probably better off simply deleting the apps and components you do not want to use, since 
there are many useful helper components in System42. Such as app settings supporting Active Events, and extension web widgets for instance.



