File IO authorization in Phosphorus Five
===============

This project is what determines if a user has access to read and write to a folder or file
on disc. It is for the most parts used indirectly, by p5.io for instance, when it is trying to 
determine if a user has access to reading and modifying some folder or file on disc. It consists
of 4 private Active Events, which is invoked by most other parts of the system, whenever 
a file should be read from, and/or saved to.

* __[.p5.io.authorize.read-file]__ - Verifies read access to file
* __[.p5.io.authorize.modify-file]__ - Verifies write access to file
* __[.p5.io.authorize.read-folder]__ - Verifies read access to folder
* __[.p5.io.authorize.modify-folder]__ - Verifies modify access to folder

The above four Active Events are invoked by other parts of the system, and will throw
an exception, if the currently logged in user does not have access to reading or
writing to some specific folder or file.

## Default file access in Phosphorus Five

* Root account(s) have access to reading and writing to all files
* Non-root accounts have read access to everything, except the _"auth.hl"_ file, main _"/web.config"_ file, _"/db/"_ folder, and other users' files. The exception is reading files in other users' _"/documents/public/"_ folder, and its sub-folders
* Non root accounts only have write access to their own files, in addition to all _"/common/"_ files

This allows any user to publicly share a file, with read-only access, such that all other users can read it but not modify it,
by putting the file in his or hers _"~/documents/public/"_ folder. In addition, it allows a user to share a file, such that
any user can both read and modify the file, by putting the file somewhere within the _"/common/"_ folder.

Files in any of the _"/documents/public/"_ folders (both _"/common"_ and _"/users/"_) are possible to download, with a direct link, for any random
visitor, having a direct link to the file. This is not true for files in the _"/common/documents/private/"_ folder, even though
the files becomes both readable, and possible to edit, by any logged in user in the system.

**Notice**; The latter depends upon how you have setup your web server. The default _"install.sh"_ script, which sets up your system on Apache/Debian, 
will setup apache such that it follows the above rules. If you are using another type of web server, you will have to make sure of configuring your 
server correctly in accordance to this yourself. All of these rules **can** however be overridden, by explicitly granting or denying access to roles, 
to one specific file or folder. Any explicitly created access object, that either grants or denies access to some file IO object, will override
these defaults.

## Overriding file IO access

Besides from the above, one or multiple roles can explicitly be given write access, 
and/or denied read access, to some specific file or folder. This must be done by creating 
an access object, by invoking **[p5.auth.access.add]** from p5.auth. If you'd like to prevent 
all roles, except the _"developer"_ role, to read from any files beneath some _"/foo/"_ folder 
for instance - You could accomplish something like that with the following code.

```
p5.auth.access.add
  *:deny-all-foo
    p5.io.read-file.deny:/foo/
  developer:allow-developer-foo
    p5.io.read-file.allow:/foo/
```

The above would deny all non-root users, except user belonging to the _"developer"_ role, 
read access to anything beneath the _"/foo/"_ folder. If you in addition would want
the _"developer"_ role to also be able to modify the contents of the _"/foo/"_ folder,
you could explicitly give it write access with something such as the following.

```
p5.auth.access.add
  developer:allow-write-developer-foo
    p5.io.write-file.allow:/foo/
```

The important parts above, is the

* __[p5.io.read-file.allow]__ - Allows roles to read from a file or folder
* __[p5.io.read-file.deny]__ - Denies roles to read from a file or folder
* __[p5.io.write-file.allow]__ - Allows roles to write to a file or folder
* __[p5.io.write-file.deny]__ - Denies roles to write to a file or folder

You can also deny for instance a role to read/write from a folder such as for instance _"/foo/"_,
yet still give the same role access to read/write from for instance _"/foo/bar/"_ or _"/foo/bar.md"_.
This would deny a user to read/write from a folder in general, while opening up sub parts
of that folder for read/write access to the same role. Notice though, the user cannot
list files in a folder he doesn't have access to though, so the above would require him
to know the exact path to the file.

Also notice, that even though you can override access rights to most folders on
disc, you cannot override access rights to anything inside of _"/common/"_ or _"/users/"_.
In addition, it is impossible to override access rights to both web.config, auth.hl,
and the _"/db/"_ folder.

Internally this project will use the **[p5.auth.has-access-to-path]** Active Event
to determine these overridden access rights, which is a helper event that can be used
also in your own code, to for instance determine if some user has access to for instance
some URL or something similar. The asterix (*) above, implies _"all roles"_, which
for the above example denies all roles access to _"/foo/"_, for then to afterwards
explicitly allow access to the developer role, through its **[developer]** argument.

The **[p5.auth.has-access-to-path]** event is implemented and documented in [p5.auth](/plugins/extras/p5.auth).

### File types and folder restrictions

You can also optionally supply a **[file-type]** argument to your access objects, and/or a **[folder]** argument if you wish.
This has the effect of restricting the file type (or that your path must be a folder) for your access objects. For instance,
an access object such as the following, would create a _"create folder access object"_ and a _"modify/create file access"_, 
but only for _".js"_ types of files, and for creating folders. All other file changes to the _"/common/"_ folder would be denied.

```
guest
  p5.io.write-file.deny:/common/
guest
  p5.io.write-file.allow:/common/documents/public/micro-javascript-cache/
    folder:bool:true
guest
  p5.io.write-file.allow:/common/documents/public/micro-javascript-cache/
    file-type:js
```

The above is an actual example of a relevant type of access object, where you deny all _"guest"_ accounts to write
anything to the _"/common/"_ folder, unless it's beneath the _"//common/documents/public/micro-javascript-cache/"_ folder,
and it it is a _".js"_ type of file. This is useful if you wish to restrict all write access to your disc, which creates a dilemma,
since some of the core system Active Events, such as the minify JavaScript event, needs to create cache files, in the above folder,
and this event will be evaluated within the context of the user that is trying to access your site. In a demo server which I have setup
myself for instance, I have the following access objects, which basically denies _"everything"_ for _"guest"_ accounts, except
the bare minimum the system is dependent upon, to actually function.

```
*
  p5.module.allow:/modules/hyper-ide/
guest
  p5.io.write-file.deny:/common/
guest
  p5.io.write-file.allow:/common/documents/public/micro-codemirror-cache/
    folder:bool:true
guest
  p5.io.write-file.allow:/common/documents/public/micro-codemirror-cache/
    file-type:js
guest
  p5.io.write-file.allow:/common/documents/public/micro-css-cache/
    folder:bool:true
guest
  p5.io.write-file.allow:/common/documents/public/micro-css-cache/
    file-type:css
guest
  p5.io.write-file.allow:/common/documents/public/micro-javascript-cache/
    folder:bool:true
guest
  p5.io.write-file.allow:/common/documents/public/micro-javascript-cache/
    file-type:js
```

The above gives _"execution"_ rights for Hyper IDE, also for _"guest"_ accounts, such that any random visiting user
can test Hyper IDE, without having to login. While at the same time, it denies these users to create any files
beneath the _"/common/"_ folder, except _".js"_ and _".css"_ files, which is necessary to have my JavaScript and
CSS minify logic work correctly, which is actually minifying and bundling JavaScript files and CSS files _"on demand"_.

So the above, basically allows any random visitor to test Hyper IDE, without being able to create anything but CSS 
and JavaScript files, and only beneath my _"cache"_ folders. Preventing arguably any harm that could in theory occur,
by giving random visitors access to read my server's files and folders.

## Rolling your own authorization logic

The file IO access rights of Phosphorus Five, is consciously kept naive and
simple, to make it easily understood. This might not fit your needs, and you may
want to have more features in your own projects. If that is so, then you can easily
replace this project with your own, as long as you implement all of the above 4 Active
Events.

Most parts of Phosphorus Five that tries to access the file system directly on behalf of some user,
will invoke the relevant Active Events above, to determine if the currently logged in user
has read/write access to whatever file he is trying to read from or write to. This
implies that as long as you are able to implement your own versions of the Active Events
mentioned at the top of this document, and throw an exception if the user is not
authorized, it will automatically _"plugin"_ to all existing logic.

You might also want to check out the [p5.auth](/plugins/extras/p5.auth) project,
for more details about roles and access rights in Phosphorus Five in general.
