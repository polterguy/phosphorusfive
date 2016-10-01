Phosphorus Five "application pool" (main website)
========

This is the main web root folder of your Phosphorus Five installation. This is the project 
you would normally set as your "startup project" when testing P5.

P5 as a general rule, does not have access to any files outside of this folder. P5 needs 
complete control (read/write/delete/etc) of files inside of this folder to function optimally.

## File structure

Below you can find explanation of most of the files and/or folders inside of your
main "www root folder".

### The "users" folder

This folder contains one folder for each user in your system, carrying the same
name as your user's username. Inside of your user's folders, are both
a "temp" folder, which is used for holding temporary files for the specific user,
in addition to a "documents" folder, which stores a user's documents.

Each "documents" folder has at the very least one "private" folder, and a "public"
folder. The first is for the user's private documents, and can in general not be 
read or manipulated by any other user than the user who owns the folder. The latter
contains a user's public files, and each file in here, can be retrieved by anyone with
a direct link to the file.

To retrieve files from your "private" folder, the Active Event *[echo-file]* must be
used. However, this Active Event will throw an exception, if you try to access files
from another user's private folder. And you must be logged in as the user whom you
are trying to retrieve files from to be able to retrieve these files.

If you wish to store documents and files, that are accessible for all users, you
should use the "common" folder.

### The "auth" file

Your "auth" file is automatically created during first time installation of your system,
and contains the "server salt", in addition to all users in your system, and user specific
settings.

If you have GnuPG installed, and you associate a PGP keypair with your server through
the "Marvin" settings in your web.config file, this file will be encrypted on disc,
using your private PGP key.

However, passwords and such, are stored in this file as salted hash values, so figuring
out passwords of accounts from this file, even in plain text format, should be very
difficult, if not close to impossible, dependeing upon how complex your server-salt is.
Hence, using a rainbow table to do reverse lookup of passwords, should in general terms 
not work.

Besides, this file is also protected such that no user account, including even your 
root account, in general terms, cannot neither modify the file, nor retrieve its content 
in any ways directly.

### The "web.config" file

This file is a normal ASP.NET web.config file, with some additional P5 specific settings,
for your system. See the web.config example for an explanation of each of these settings.

### The "media" folder

Contains global images, JavaScript and CSS files, which are an integral part of P5. P5 
depends upon Boootstrap.CSS, and jQuery, among other things, which you can find in this folder.

### The "common" folder

Contains files common for all users, and does not belong to one specific user in general.
It stills contains both a "private" and a "public" folder, which semantically works like
the private and public folders of user specific document folders. The private folder will
not serve documents, even if the user has a direct link to a specific file, unless the 
document is requested using some sort of Hyperlisp re-direct handler, which indirectly 
serves it, by explcitily echoing the document back to client.

This allows you to create your own authorization system for retrieving and serving documents 
not belonging to one specific user, but rather the server as a whole.

To serve files from this folder, use for instance the *[echo-file]* Active Event.

### The "system42" folder

Contains "System42". See the documentation for [System42](/core/p5.webapp/system42/).

### The "Default.aspx" file

This is the only physical file in your system, and responsible for serving all URLs indirectly,
through using the main Hyperlisp engine.

When initially accessing a virtual URL in your system, the *[p5.web.load-ui]* Active Event
will be raised. It is your responsibility of creating some sort of hook, for instance in 
Hyperlisp, to handle this Active Event.

When the *[p5.web.load-ui]* Active Event is raised, the actual URL requested by the client,
will exists in the *[_form]* argument, passed into your Active Event handler, in case you wish
to override this logic.

If you use Phosphorus Five in combination with System42, this Active Event is automatically
handled, and will retrieve the *[p5.page]* from the p5.data database, with the ID being
the URL requested.

## Active Events

Also the web app itself exposes some few Active Events. Some of the more important ones, are listed below.

* [.p5.core.application-folder] - Returns the root "p5.webapp" folder
* [.p5.security.get-auth-file] - Returns the filepath to the "auth" file
* [p5.security.get-default-context-role] - Returns the default Ticket role for users not logged in
* [p5.security.get-default-context-username] - Returns the default Ticket username for users not logged in
* [set-page-value] - Sets a "page value", think of a "page value" as a ViewState entry. Has a "private" override
* [get-page-value] - Opposite of above
* [list-page-keys] - Lists all "page keys" for current page
* [set-title] - Changes the title of your page
* [get-title] - Returns the title of page

The "page-value" Active Events above, obeys by the same rule-set as the "collection values" in [p5.web](/plugins/extras/p5.web/).
And take similar types of arguments as for instance *[get-session-value]* etc do.

Most of the above mentioned Active Events are either defined in Global.asax.cs or PhosphorusPage.cs in the "code" folder.


