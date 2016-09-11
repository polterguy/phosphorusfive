Applications
========

This folder contains all of your P5 applications. An application is a folder that at the very least
contains one file, "launch.hl". This file is the Hyperlisp file that is executed when the user choose
to start your app.

In addition an app can contain "startup.hl" directly within the main folder. This file is executed
when the web-server initially starts, and is useful for initializing your app.

This allows you to distribute your Phosphorus apps using x-copy deployment.
By default, Phosphorus Five contains several pre-built apps. If you wish to remove these apps,
simply delete the folder containing your app, *before* you start P5 for the first time, to make sure
your app doesn't run its initialization logic, which might put things into your database, or create some
other objects that hangs around, even after uninstallation.

The name of your app, becomes the name you choose to use for your app folder.
