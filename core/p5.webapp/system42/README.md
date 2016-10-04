System42, the answer to life, the universe and everything
========

System42 is an example minimalistic CMS, built on top of Phosphorus Five. It also includes 
several "helper components".

The purpose of System42, is to provide a starting point, and example/reference implementation,
when developing P5 apps. In addition to being a quite useful tool by itself, implementing a 
fully fledged CMS for you, in a nice graphical environment.

System42, is also a "host" for your web apps, by allowing you to install your apps.
To install an app into System42, is as simple as an x-copy operation, pasting your "app" 
into the "/system42/apps/" folder, with optionally one file called "launch.hl",
and another file called "startup.hl" - And your app should be ready for use.

The "startup.hl" file is evaluated when the server is started, and used for initializing
your app, by creating supporting Active Events, and so on. The "launch.hl" file,
is expected to "launch" your app, whatever that means for your specific app.

If your app has a "startup.hl" file, you'd probably want to evaluate this file after the x-copy
operation.

## Structure

All files belonging to System42 is contained in the "system42" folder. The "application-startup.hl" 
file is evaluated when your application pool starts in your web server. This file creates a couple 
of helper Active Events, for evaluating Hyperlambda files, and folders. In addition, it evaluates all 
files in the "/system42/startup/" folder.

When it is done with the above tasks, it will evaluate all "application specific" and "components specific"
startup files. These are files inside of any "/system42/apps/xxx/" and "/system42/components/xxx/" folders, 
who's names are "startup.hl".

This allows you to create startup Hyperlambda scripts, both on system level, and on application/component level, 
for apps and components, that needs some sort of startup/initialization scripts to be evaluated during installation.

### The "apps" folder

This is the System42 version of your windows "C:/Program Files/" folder, and contains 
all apps in your System42 installation. See the documentation for the [apps](/core/p5.webapp/system42/apps/)
folder for details about this folder.

### The "components" folder

This is the System42 version of your windows "COM objects", and contains components in your System42 installation, 
which you can reuse in your apps. See the documentation for the [components](/core/p5.webapp/system42/components/) 
folder for details about this folder.

### The "startup" folder

This folder contains files that are evaluated during application startup of your server, either because of your 
server rebooting, or because of the web-server process being restarted for some reasons.

See above for a description of what this folder contains.

### Helper Active Events

Even if you choose to entirely delete System42, and the CMS, to create your own host from scratch. There are still 
some helper Active Events you should consider keeping.

Some of them are mentioned below.

* [sys42.execute-hyper-file] - Executes one or more Hyperlambda files
* [sys42.execute-hyper-folder] - Executes all Hyperlambda files in a folder, recursively
* [sys42.get-event] - Returns the p5.lambda code for one or more dynamically created Active Events
* [sys42.get-widget] - Returns all p5.lambda associated with a widget, including events
* [sys42.include-default-javascript-files] - Which includes jQuery and Bootstrap's JavaScript files for you
* [sys42.include-default-stylesheet-files] - Which includes Bootstrap's CSS files, and the "/media/main.css" CSS files for you
* [sys42.empty-user-tmp-folder] - Which empties a user's "temp" folder

In addition, System42 contains a web based Hyperlambda editor, built on top of CodeMirror, an HTML editor,
built on top of CKEditor, and other things, that might help you out, when creating your own apps.

System42 also contains a suite of Unit Tests, to test the integrity of Phosphorus Five. Which is highly
extendible, such that you can create your own tests, and plug them into the existing suite.

### Rolling your own from scratch

If you wish to use Phosphorus Five as a stand alone framework, you should delete the entire System42 folder, 
and modify the ".p5.webapp.application-startup-file" setting in your web.config, beneath your p5.webapp folder. 
This allows you to create web apps from scratch, having complete control over every aspect of your systems.

However, you are probably better off simply deleting the apps and components you do not want to use, since 
there are many useful "helper components" in System42. Such as "app-settings" and web widgets for instance.



