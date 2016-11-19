System42's startup Hyperlambda files
========

This folder contains all of your System42 startup Hyperlambda files, which are Hyperlambda evaluated when the server starts.
Often these files will create some helper Active Events, and do other types of initialization necessary for your system to work.

Notice, if you build your own "component" or "app", you should rather put your startup script into your component/application
specific startup folder, and not into this folder, which are for "globally" necessary startup scripts.

The core Active Events created by evaluating the files in this folder are as following;

* [p5.io.unroll-path.@SYS42-APPS] - Makes sure "@SYS42-APPS" becomes an alias usable for file IO operations towards "/system42/apps/".
* [p5.io.unroll-path.@SYS42-COMPONENTS] - Makes sure "@SYS42-COMPONENTS" becomes an alias usable for file IO operations towards "/system42/components/".
* [sys42.utilities.empty-user-temp-folder] - Empties user's temporary folder for temporary files.
* [sys42.utilities.get-event] - Returns the lambda object for one or more dyncamically created Active Event(s).
* [sys42.utilities.get-widget] - Returns the entire lambda object necessary to re-create one or more widgets.

