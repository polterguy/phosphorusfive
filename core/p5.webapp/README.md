Phosphorus Five "application pool" (main website)
========

This is the main web site's folder of your Phosphorus Five installation. This is the project 
you would normally set as your "startup project" when testing P5.

Normally when developing Phosphorus Five apps, you would never change the code for your actual website.
You would usually create your project as "plugins", which you consume through Hyperlambda and p5.lambda. 
Then you would "declare" your website, in ".hl" files. And have your Hyperlambda invoke Active Events 
from your plugins. While keeping the actual website completely unchanged. Regardless of how different your
projects are in character.

This project is that website project.

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

To retrieve files from your "private" folder, the Active Event *[echo-file]* can be
used, for instance. However, all Active Events that read files from the system, will 
throw an exception, if you try to access files from another user's private folder. 
You must be logged in as the user whom you are trying to retrieve files from, to be 
able to retrieve these files.

If you wish to store documents and files, that are accessible for all users, you
should use the "common" folder.

### The "auth.hl" file

Your "auth" file is automatically created during first time installation of your system,
and contains the "server salt", in addition to all users in your system, and user specific
settings.

However, passwords and such, are stored in this file as salted hash values, so figuring
out passwords of accounts from this file, even in plain text format, should be very
difficult, if not close to impossible, dependeing upon how complex your server-salt is.
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

To serve files from the private folder, use for instance the *[echo-file]* Active Event.

### The "system42" folder

Contains "System42". See the documentation for [System42](/core/p5.webapp/system42/).

### The "Default.aspx" file

This is the only physical wep page in your system, and responsible for serving all URLs indirectly,
through invoking the *[p5.web.load-ui]* Active Event.

When initially accessing a virtual URL in your system, the *[p5.web.load-ui]* Active Event
will be raised. It is your responsibility of creating some sort of hook, for instance in 
Hyperlambda, to handle this Active Event.

When the *[p5.web.load-ui]* Active Event is raised, the actual URL requested by the client,
will exists in the *[_url]* argument, passed into your Active Event handler.

If you use Phosphorus Five in combination with System42, this Active Event is automatically
handled, and will retrieve the *[p5.page]* from the p5.data database, with the ID being
the URL requested.

Notice, by default, only URLs _not_ containing a period "." in them, will be rewritten. Other
requests, will pass into the ASP.NET/IIS/Apache document web server, and be treated in whatever
ways your web server wants to treat them. Which means, you among other things, don't have to
write a mapper for CSS files and JavaScript files etc.

If you wish to change this logic, you'd have to edit the Global.asax.cs class, and more specifically,
the `Application_BeginRequest` method.

However, none of this holds any relevance, as long as you use P5 in combination with System42.
System42 automatically handles the *[p5.web.load-ui]* Active Event, and serves documents to the
end user, from the P5 database.

The pages in your system, can be retrieved with the following code.

```
p5.data.select:x:/*/*/p5.page
```

Which of course will retrieve all *[p5.page]* objects from your database.

### The "code" folder

This folder should normally not be deployed in your end product, since it only contains C# code associated
with the website. If you see this folder in a production website, then the website has not been deployed correctly.

## Active Events

The p5.webapp exposes some Active Events. Some of the more important ones, are listed below.

* [set-page-value] - Sets a "page value", think of a "page value" as a ViewState entry. Has a "private" override
* [get-page-value] - Opposite of above
* [list-page-keys] - Lists all "page keys" for current page
* [set-title] - Changes the title of your page
* [get-title] - Returns the title of page

In addition to these protected events, which can only be accessed through C#.

* [.p5.core.application-folder] - Returns the root "p5.webapp" folder
* [.p5.security.get-auth-file] - Returns the filepath to the "auth" file
* [.p5.security.get-default-context-role] - Returns the default Ticket role for users not logged in
* [.p5.security.get-default-context-username] - Returns the default Ticket username for users not logged in

The "page-value" Active Events above, obeys by the same rule-set as the "collection values" in [p5.web](/plugins/p5.web/).
And accepts similar types of arguments as for instance *[get-session-value]* etc do. See the documentation 
for [get-session-value](/plugins/p5.web/) for instance, to understand their arguments.

The page values however, are stored in the ViewState, which is stored on the server, and hence are "page related data", which 
disappears, if the user navigates to another page, or refreshes his browser.

Most of the above mentioned Active Events are either defined in Global.asax.cs or PhosphorusPage.cs in the "code" folder.


