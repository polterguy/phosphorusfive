File IO authorization in Phosphorus Five
===============

This project is what determines if a user has access to read and write to a folder or file
on disc. It is for the most parts used indirectly, by p5.io for instance, when it is trying to 
determine if a user has access to reading and modifying some folder or file on disc. It consists
of 4 private Active Events, which is invoked by most other parts of the system, whenever 
a file should be read, and/or saved.

* __[.p5.io.authorize.read-file]__
* __[.p5.io.authorize.modify-file]__
* __[.p5.io.authorize.read-folder]__
* __[.p5.io.authorize.modify-folder]__

The above four Active Events are invoked by other parts of the system, and will throw
an exception, if the currently logged in user does not have access to reading or
writing to some specific folder or path. The default access is as follows.

* Root account(s) have access to reading and writing to all files
* Non-root accounts have read access to everything, except the _"auth.hl"_ file, main _"/web.config"_ file, _"/db/"_ folder, and other users files
* Non root accounts only have write access to their own files, in addition to all common files

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
you could explicitly give him write access with something such as the following.

```
p5.auth.access.add
  developer:allow-write-developer-foo
    p5.io.write-file.allow:/foo/
```

Internally this project will use the **[p5.auth.has-access-to-path]** Active Event
to determine these overridden access rights, which is a helper event that can be used
also in your own code, to for instance determine if some user has access to for instance
some URL or something similar.

