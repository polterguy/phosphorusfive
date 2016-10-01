System42, the answer to life, the universe and everything
========

System42 is an example minimalistic CMS, built on top of 
Phosphorus Five. If you wish to use Phosphorus Five as a stand alone framework, you
should delete the entire System42 folder, and modify the "p5.webapp.application-startup-file"
setting in your web.config, beneath your p5.webapp folder. This allows you to create web
apps from scratch, having complete control over every aspect of your systems.

The purpose of System42, is to provide a starting point and example implementation,
when developing P5 apps, in addition to being a quite useful by itself, implementing a CMS
for you, in a nice graphical environment.

System42, is also a "host" for your web apps, by allowing you to install your apps.
To install an app into System42, is as simple as an x-copy operation, pasting you "app 
folder" into your "/system42/apps/" folder, with optionally one file called "launch.hl",
and another file called "startup.hl", and your app should be ready for use.

## Structure

All files belonging to System42 is contained in the "system42" folder. The 
"application-startup.hl" file is executed when application pool starts in web server.
This file creates a couple of helper Active Events, for executing Hyperlisp files, and
folders. In addition, it executes all files in the "startup" folder.

When it is done with the above tasks, it will execute all "application specific"
startup files, which are files inside of any "system42/apps/xxx" folders who's names
are "startup.hl".

This allows you to create startup Hyperlisp scripts, both on system level, and on
application level, for apps that needs some sort of startup/initialization scripts.

### The "apps" folder

This is the System42 version of your windows "C:/Program Files/" folder, and contains 
all apps in your System42 installation. See the documentation for the [apps](/core/p5.webapp/system42/apps/)
folder for details about this folder.

### The "editors" folder

This folder contains helper files for CKEditor and CodeMirror, which allows for 
editing HTML and Hyperlisp, among other things.

### The "installation" folder

This folder contains scripts that are to be evaluated when server is initially
setup. It creates a server-seed, which is used to seed the cryptographic random
number generator, and salt your hashed passwords etc. In addition, it creates a root
password, and a superuser normal account, for everyday use of your system.

### The "startup" folder

This folder contains files that are evaluated during application startup of your server,
either because of your server rebooting, or because of the web-server process being restarted
for some reasons.

The most important file in this folder is the file called "pf.web.load-ui.hl", which
declares the Active Event called *[pf.web.load-ui]*, which is the Active Event that maps
URLs to *[p5.page]* objects in the P5 database.

### Helper Active Events

Even if you choose to entirely delete System42, and the CMS, to create your own host from
scratch. There are still lots of helper Active Events you should consider not removing.

Some of them are mentioned below.

* [sys42.get-app-setting] - Retrieves a setting related to a specific application
* [sys42.set-app-setting] - Sets a setting for app
* [sys42.list-app-settings] - List all settings for a single app
* [sys42.confirm-window] - Shows a confirmation modal dialogue
* [sys42.info-window] - Shows small notifications in a notification window
* [sys42.show-code-window] - Shows a window with whatever p5.lambda object you wish to display
* [sys42.wizard-window] - Like "confirm-window", but allows you to associate input widgets with it
* [sys42.add-css-classes] - Adds a CSS class to a widget
* [sys42.delete-css-classes] - Deletes a CSS class from a widget
* [sys42.toggle-css-classes] - Toggles a CSS class from a widget
* [sys42.get-event] - Returns the p5.lambda code for one or more dynamically created Active Events
* [sys42.get-widget] - Returns all p5.lambda associated with a widget, including events




