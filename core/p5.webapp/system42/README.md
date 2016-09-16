System42
========

System42 is an example minimalistic Web Operating System, built on top of 
Phosphorus Five. If you wish to use Phosphorus Five as a stand alone framework, you
should delete the entire System42 folder, and modify the "application-startup-file"
setting in your web.config underneath p5.webapp. This allows you to create web
apps from scratch, having complete control over every aspect of your systems.

The purpose of System42 is to provide a starting point and example implementation 
when developing p5 apps, in addition to being a useful Web Operating System by itself,
implementing most common tasks for you, in a nice graphical environment.

System42 contains a minimalistic and extendible Web Operating System, allowing you 
to transparently use your web server as an extension of your other devices, sharing, 
storing and having backup of your files, in addition to all other things you'd normally 
use cloud services for. In such a way, System42 becomes an extension of your Intranet,
allowing you access to your private files and data, on evey device you own, such
as your iPhone, Android, Tablets, PCs, Mac OSX computers, etc, etc, etc.

System42 can also be used as an IDE for developing apps in Hyperlisp and p5 Lambda.

## Structure

All files belonging to System42 is contained in the "system42" folder. The 
"application-startup.hl" file is executed when application pool starts in web server.
This file creates a couple of helper Active Events for executing Hyperlisp files and
folders. In addition it executes all files in the "startup" folder.

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

### The "installation" folder

This folder contains scripts that are to be evaluated when server is initially
setup. It creates a server-seed, which is used to seed the cryptographic random
number generator, and salt your hashed passwords etc. In addition, it creates a root
password, and a superuser normal account, for everyday use of your system.

### The "tests" folder

This folder contains unit tests for your system, and checks the integrity of
both Phosphorus Five and System42, by evaluating several assumptions that should
evaluate to "true". If something goes wrong, then after evaluation the tests
will show up in "red" color, indicating that at least one assumption did not
evaluate as expected. If everything is successful, there will be a green color,
indicating success.




