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
writing to some specific folder or path. The default access is as follows.

* Root account(s) have access to reading and writing to all files
* Non-root accounts have read access to everything, except the _"auth.hl"_ file, main _"/web.config"_ file, _"/db/"_ folder, and other users' files
* Non root accounts only have write access to their own files, in addition to all _"/common/"_ files

## Overriding file IO access

One or multiple roles can explicitly be given write access, and/or denied read access,
to some specific file or folder. This must be done through creating an access object
by invoking **[p5.auth.access.add]** from p5.auth. If you'd like to prevent all roles, 
except the _"developer"_ role, to read from any files beneath some _"/foo/"_ folder 
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
of that folder for read/write access to the same role.

Internally this project will use the **[p5.auth.has-access-to-path]** Active Event
to determine these overridden access rights, which is a helper event that can be used
also in your own code, to for instance determine if some user has access to for instance
some URL or something similar. The asterix (*) above, implies _"all roles"_, which
for the above example denies all roles access to _"/foo/"_, for then to afterwards
explicitly allow access to the developer role, through its **[developer]** argument.

## Rolling your own authorization logic

The file IO access rights of Phosphorus Five, is consciously kept naive and
simple, to make it easily understood. This might not fit your needs, and you may
want to have more features in your own projects. If that is so, then you can easily
replace this project with your own, as long as you implement all the above 4 Active
Events.

Most parts of Phosphorus Five that tries to access the file system directly on behalf of some user,
will invoke the relevant Active Events above, to determine if the currently logged in user
has read/write access to whatever file he is trying to read from or write to. This
implies that as long as you are able to implement your own versions of the Active Events
mentioned at the top of this document, and throw an exception if the user is not
authorized, it will automatically _"plugin"_ to all existing logic.

You might also want to check out the [p5.auth](/plugins/extras/p5.auth) project,
for more details about roles and access rights in Phosphorus Five in general.
