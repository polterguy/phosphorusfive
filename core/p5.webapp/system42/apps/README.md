Applications in System42
========

This folder contains all of your System42 applications. An application is a folder that normally, at the very least,
contains one file, _"launch.hl"_. This file is the Hyperlambda file that is executed when the user choose to start your app.
If you create an app with a _"launch.hl"_ file, then a menu item will automatically be created for your app, beneath _"Apps"_,
which will evaluate your _"launch.hl"_ file when clicked. This creates an easy to grasp deployment model for System42 apps, 
allowing you to distribute your apps, largely through x-copy.

In addition, an app can optionally also contain a _"startup.hl"_ Hyperlambda file, directly within its main folder. This file 
is evaluated when the web-server initially starts, and/or restarts, and is useful for initializing your app. If you install a new
app without restarting your server, which is possible by the way, you should make sure that you evaluate its _"startup.hl"_ file,
if any.

By default, Phosphorus Five contains several pre-built apps. If you wish to remove these apps, simply delete the folder containing 
your app, preferably _before_ you start P5 for the first time, to make sure your app doesn't run its initialization logic, which 
might put things into your database, or create some other objects hanging around, even after uninstallation.

The name of your app, becomes the name you choose to use for your app's folder.
