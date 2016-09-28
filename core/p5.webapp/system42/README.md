System42, the answer to life, the universe and everything
========

System42 is an example minimalistic Web Operating System, built on top of 
Phosphorus Five. If you wish to use Phosphorus Five as a stand alone framework, you
should delete the entire System42 folder, and modify the "application-startup-file"
setting in your web.config underneath the p5.webapp folder. This allows you to create web
apps from scratch, having complete control over every aspect of your systems.

The purpose of System42 is to provide a starting point and example implementation 
when developing P5 apps, in addition to being a useful Web Operating System by itself,
implementing most common tasks for you, in a nice graphical environment.

System42, really, contains "everything"! A fully fledged CMS, a file explorer for 
browsing your files on your server, user management for adding, editing and deleting
users in your system, a GnuPG (Gnu Privacy Guard) front-end for managing PGP keypairs,
an "executor" that allows you to inline execute Hyperlisp code, a Visual IDE for creating
web apps in a "RAD" environment, etc, etc, etc.

To install an app into System42, is as simple as an x-copy operation, dragging and dropping 
a new folder into your "/system42/apps/" folder, with at the very least one file called "launch.hl",
optionally another file called "startup.hl", and your app should be ready for use.

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
all apps in your System42 installation. Each app can have two special files;

* startup.hl - Which is executed when application pool for webserver starts
* launch.hl - Which is the file that executes when user launches your app

### The "editors" folder

This folder contains helper files for CKEditor and CodeMirror, which allows for 
editing HTML and Hyperlisp, among other things.

### The "installation" folder

This folder contains scripts that are to be evaluated when server is initially
setup. It creates a server-seed, which is used to seed the cryptographic random
number generator, and salt your hashed passwords etc. In addition, it creates a root
password, and a superuser normal account, for everyday use of your system.

### The "misc" folder

This folder contains some helper files, which does things, that doesn't really belong
to a specific concept. One example is the different ContentTypes P5 supports out of
the box.

### The "startup" folder

This folder contains files that are evaluated during application startup of your server,
either because of your server rebooting, or because of the web-server process being restarted
for some reasons.

The most important file in this folder is the file called "pf.web.load-ui.hl", which
declares the Active Event called *[pf.web.load-ui]*, which is the Active Event that maps
URLs to *[p5.page]* objects in the P5 database.

### The "application-startup.hl" file

This file is executed during startup of your web-server process, and/or server, and its
main responsibility is to create a couple of helper Active Events, in addition to executing
all Hyperlisp files within the "startup" folder.




