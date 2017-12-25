Authorization in Phosphorus Five
===============

This project, is mostly for internal use, and only used by other plugins, when the system wants to check if a user is authorized
to access or modify a file or folder. If you create your own disc IO Active Events in C#, you might want to invoke some of these
Active Events yourself, from within your own Active Events, to make sure the user is authorized to access or change your file(s)
or folder(s).

This project contains 4 Active Events, which are invoked by other events, during file IO.

* __[.p5.io.authorize.read-file]__, checking if the ApplicationContext's "Ticket" is authorized to reading/loading a file
* __[.p5.io.authorize.modify-file]__, checking if the "Ticket" is authorized to modify/delete/overwrite a file
* __[.p5.io.authorize.read-folder]__, checking if the "Ticket" is authorized to reading/loading data from a folder
* __[.p5.io.authorize.modify-folder]__, checking if the "Ticket" is authorized to modify/delete a folder's content

All Active Events above, requires as their argument, the file/folder name as the value of the e.Args node (main node of invocation). In addition, since these
Active Events throws exception if the user _"ticket"_ is not legally allowed to access a folder, all Active Events takes the invocation node by reference,
as an **[args]** child node. This is to keep contextual information, in case an exception is thrown, such that we can return the Hyperlambda stack trace
back to the caller, and supply contextual information about what raised the exception.
To see an example of how to invoke these Active Events yourself, you can check out the code for [p5.io](/plugins/p5.io/).

The logic of authorizating IO operations, is pretty simple to understand. Root account can read and write to everything. All other accounts can only write
to their own files, and the common files and folders. A normal user (non-root), can read from everything, excepth these files.

* The _"auth.hl"_ file, or whatever it happens to be named in your system
* The p5.data database files, or the _"/db/"_ folder, which is its default value
* Any file ending with _".config"_

Unless explicitly overridden, these are the default access rights for file IO in P5. 
Now of course, this is the low level disc IO authorization features, and simply there as an additional layer of protection for disc IO. In addition
to these authorization features, a user still needs some ways to evaluate some arbitrary piece of lambda in your system, to be able to invoke Active
Events, that actually is able to read these files, and return them to the user. Which for most cases, would be impossible, unless you create some sort
of bug in your system, allowing any normal user to gain access to for instance evaluating the **[eval]** event.
Even if you created such a bug, to access files and/or folders, or modify files and/or folders the user is not authorized to read/modify -
Would still for the most parts be prevented by this project, as long as it is included in your project as a plugin - But only for the events distributed 
in Phosphorus Five by default out of the box. If you create your own events in C#, which reads and writes from disc, you'll either need to invoke these 
authorize events yourself, or create your own logic to make sure you don't accidentally give access to something a user shouldn't have access to.

You can explicitly override which files a user is allowed to write to, by adding an access right object to a role - Which would allow all users belonging 
to that role, to write/overwrite files in some specified folder of choice.

## Creating explicit write access for users to some folder

If you have a user, which you want to create extended rights for, but not allow to have full root access to your system - You can use the p5.auth project's
events to explicitly give write access to some specific folder on disc, at which point this project will check if there exists a **[write-folder]**
access right for the role of the user trying to modify a file or folder - And only if such an access right exists, the user will be allowed to modify the
content of some specified folder. The structure of such an **[access]** object should look like the following.

```
/*
 * Role name is "developer".
 *
 * You can also use "*" as role name, to denote all roles (except root acounts).
 */
developer:some-unique-string-id
  write-folder:/modules/
```

The above access right object for instance, would give all users belonging to the *"developer"* role, write access to the _"/modules/"_ folder.

## Denying users read access to specific folders

You can also further restrict a user's access to reading files and folders, by creating an access object like the following.

```
*:some-other-unique-string-id
  deny-folder:/foo/
```

## Deny/allow access precedence

The above will deny all users to read (*and write*) from any files beneath _"/foo/"_, in addition to deny the
user to list the files inside of the same folder. The **[deny-folder]** has precedence though, implying that it doesn't matter if you allow
a role access to write to for instance _"/foo/bar/"_, if the user is denied access to _"/foo/"_ in general. The user will still not be able to modify files inside
of the _"/foo/bar/"_ folder, as long as he doesn't have access to _"/foo/"_. You can however deny all roles, for then to allow a specific role access, doing something
such as the following.

```
// Denies all roles by default (except root users)
*:yet-another-unique-string-id
  deny-folder:/foo/

/*
 * Allowing "developer" users access to the same folder we denied access to above.
 *
 * Below we are using [allow-folder], but we could also have used [write-folder]
 * to also give write access to our "developer" role.
 */
developer:yet-another-again-unique-id
  allow-folder:/foo/
```

In the above example, all roles except the _"developer"_ role are denied read access to the _"/foo/"_ folder, but the _"developer"_ role does not have write access, 
only read. This will only work if the first **[deny-folder]** is using the asterix deny (*). This allows you to deny access by default to reading files in some 
specific folder, for afterwards to open up explicitly for some specific roles in your system. There are three different access objects relevant to file IO operations
in Phosphorus Five, these are as follows.

* __[deny-folder]__ - Denies access to some folder
* __[allow-folder]__ - Allows access again (undo above declaration for a specifically named role)
* __[write-folder]__ - Gives write access for some specific role (or all roles)

## Rolling your own

The authorisation logic in P5 is intentionally kept _naive_, to make it more eaily understood. If you wish to have more fine-grained authorisation,
you might want to exchange this project, in addition to also probably the [p5.auth](/plugins/extras/p5.auth) plugin. By doing this correctly, you could 
completely exchange the entire logic of authorisation as you see fit, and still retain compatibility with existing code.
