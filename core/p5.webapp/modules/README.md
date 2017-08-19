Modules
========

This folder contains all the installed applications and modules of Phosphorus Five. It is the equivalent of _"Program Files"_ on a windows system.
If you wish to pre-distribute specific modules, and/or apps, this is the folder where you'd like to put these apps. At which point, you could create
your own _"distribution"_, with whatever apps you'd like to have installed. This is where apps installed through the Bazar are stored.

There are three special files, and app optionally would have

* startup.hl - A file which is expected to initialize your app, by creating its events, and/or doing other types of initialization logic.
* launch.hl - A file which would become your app's "launcher", allowing the user to launch your app, kind of like the "exe" file or "icon" of your app.
* desktop.hl - A file which is responsible for creating your app's desktop icon.

All of the above files are optional, but of course, if all of them are omitted, your app probably wouldn't do much. Sephia Five, which is the
reference implementation of an "app", contains all three files, and can be used as an example of how to create such files, and/or create your
own apps.
