System42's CMS page templates
========

This folder contains the Hyperlambda scripts, containing your "templates" for the CMS. A template is kind of like a "skin", only much
more powerful, being able to evaluate Hyperlambda on the server side, while still retaining a completely detached "skin", not in
any ways dependent upon your pages vice/versa.

By default System42's CMS comes with a handful of pre-built templates, which you can use as the basis for your own. Some of these
will have a menu bar at the top, other will not. Some will make sure jQuery and Bootstrap CSS is loaded, others will not.

At the very least, you must have one "default.hl" template in this directory, which the system assumes is your default template.

The "common" folder, is where you would put common logic, which multiple templates shares among themselves. The creation of our
navbar for instance, is common among multiple templates, and hence can be found in this directory.

