Applications
========

This folder contains all of your P5 applications. An application is a folder that at the very least
contains one file, "launch.hl". This file is the Hyperlisp file that is executed when the user choose
to start your app.

In addition, an app can optionally contain a "startup.hl" Hyperlisp file, directly within its main 
folder. This file is evaluated when the web-server initially starts, and/or restarts, and is useful 
for initializing your app.

This allows you to distribute your Phosphorus apps using x-copy deployment.
By default, Phosphorus Five contains several pre-built apps. If you wish to remove these apps,
simply delete the folder containing your app, _before_ you start P5 for the first time, to make sure
your app doesn't run its initialization logic, which might put things into your database, or create some
other objects hanging around, even after uninstallation.

The name of your app, becomes the name you choose to use for your app folder.

All apps installed in this folder, that have a "launch.hl" file in its folder, will have an
automatic menu item created for it, beneath "Apps", if you use a template for your page, which
creates a menu.




