The main website, or _"application pool"_
========

This is the main web site's folder of your Phosphorus Five installation. This is the project 
you would normally set as your "startup project" when testing P5.

Normally when developing Phosphorus Five apps, you would never change the code for your actual website.
You would usually create your project as "plugins", which you consume through Hyperlambda and p5.lambda. 
Then you would "declare" your website, in ".hl" files. And have your Hyperlambda invoke Active Events 
from your plugins - While keeping the actual website completely unchanged. Regardless of how different your
projects are in character.

P5 as a general rule, does not have access to any files outside of this folder. And it usually needs 
complete control (read/write/delete/etc) of files inside of this folder to function optimally.

## File structure

Below you can find an explanation of most of the files and/or folders inside of your
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

To retrieve files from your "private" folder, the Active Event **[p5.web.echo-file]** can be
used, for instance. However, all Active Events that read files from the system, will 
throw an exception, if you try to access files from another user's private folder, unless
you're logged in as root.

### The "auth.hl" file

Your "auth" file is automatically created during first time installation of your system,
and contains the "server salt", in addition to all users in your system, and user specific
settings.

However, passwords and such, are stored in this file as salted hash values, so figuring
out passwords of accounts from this file, even in plain text format, should be very
difficult, if not close to impossible, depending upon how complex your server-salt is.
Hence, using a rainbow table to do reverse lookup of passwords, should in general terms 
be too hard to be practically solvable.

Besides, this file is also protected, such that no user account, except root accounts,
can neither modify the file, nor retrieve its content.

### The "web.config" file

This file is a normal ASP.NET web.config file, with some additional P5 specific settings.
See the web.config example for an explanation of each of these settings.

### The "common" folder

Contains files common for all users, and does not belong to one specific user in general.
It stills contains both a "private" and a "public" folder, which semantically works like
the private and public folders of user specific document folders. The private folder will
not serve documents, even if the user has a direct link to a specific file, unless the 
document is requested using some sort of Hyperlambda re-direct handler, which indirectly 
serves it, by explicitly echoing the document back to client.

The "public" folder will serve all documents, as long as the user has a direct link to them.

This allows you to create your own authorization system for retrieving and serving documents, 
not belonging to one specific user, but rather the server as a whole.

To serve files from the private folder, use for instance the **[p5.web.echo-file]** Active Event.

### The "Default.aspx" file

This is the only physical wep page in your system, and responsible for serving all URLs indirectly,
through invoking the **[p5.web.load-ui]** Active Event.

When initially accessing a virtual URL in your system, the **[p5.web.load-ui]** Active Event
will be raised. It is your responsibility of creating some sort of hook, for instance in 
Hyperlambda, to handle this Active Event.

Notice, by default, only URLs _not_ containing a period "." in them, will be rewritten. Other
requests, will pass into the ASP.NET/IIS/Apache document web server, and be treated in whatever
ways your web server wants to treat them. Which means, you among other things, don't have to
write a mapper for CSS files and JavaScript files etc.

If you wish to change this logic, you'd have to edit the Global.asax.cs class, and more specifically,
the `Application_BeginRequest` method.

### The "code" folder

This folder should normally not be deployed in your end product, since it only contains C# code associated
with the website. If you see this folder in a production website, then the website has not been deployed correctly.

## Active Events

The p5.webapp exposes some Active Events. Some of the more important ones, are listed below.

* __[p5.web.viewstate.set]__ - Sets a "page value", think of a "page value" as a ViewState entry. Has a "private" override
* __[p5.web.viewstate.get]__ - Opposite of above
* __[p5.web.viewstate.list]__ - Lists all "page keys" for current page
* __[p5.web.page.set-title]__ - Changes the title of your page
* __[p5.web.page.get-title]__ - Returns the title of page

In addition to these protected events, which can only be accessed through C#.

* __[.p5.core.application-folder]__ - Returns the root "p5.webapp" folder
* __[.p5.auth.get-auth-file]__ - Returns the filepath to the "auth" file
* __[.p5.auth.get-default-context-role]__ - Returns the default Ticket role for users not logged in
* __[.p5.auth.get-default-context-username]__ - Returns the default Ticket username for users not logged in

The "page-value" Active Events above, obeys by the same rule-set as the "collection values" in p5.web.
And accepts similar types of arguments as for instance **[p5.web.session.get]** etc do. See the documentation 
for p5.web.session.get for instance, in the "Plugins" parts of the documentation, to understand their arguments.

The page values however, are stored in the ViewState, which is stored on the server, and hence are "page related data", which 
disappears, if the user navigates to another page, or refreshes his browser.

Most of the above mentioned Active Events are either defined in Global.asax.cs or PhosphorusPage.cs in the "code" folder.
