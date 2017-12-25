Authorization in Phosphorus Five
===============

This project, is mostly for internal use, and only used by other plugins, when the system wants to check if a user is authorized
to access or modify a file or folder. If you create your own disc IO Active Events in C#, you might want to invoke some of these
Active Events yourself, from within your own Active Events, to make sure the user is authorized to access or change your file(s)
or folder(s).

It contains 4 Active Events.

* __[.p5.io.authorize.read-file]__, checking if the ApplicationContext's "Ticket" is authorized to reading/loading a file
* __[.p5.io.authorize.modify-file]__, checking if the "Ticket" is authorized to modify/delete/overwrite a file
* __[.p5.io.authorize.read-folder]__, checking if the "Ticket" is authorized to reading/loading data from a folder
* __[.p5.io.authorize.modify-folder]__, checking if the "Ticket" is authorized to modify/delete a folder's content

All Active Events above, requires as their argument, the file/folder name as the value of the e.Args node (main node of invocation). In addition, since these
Active Events throws exception if the user _"ticket"_ is not legally allowed to access a folder, all Active Events takes the invocation node by reference,
as an **[args]** child node. This is to keep contextual information, in case an exception is thrown, such that we can return the Hyperlambda stack trace
back to the caller, and supply contextual information about what raised the exception.
To see an example of how to raise these Active Events yourself, you can check out the code for [p5.io](/plugins/p5.io/).

The logic of authorizating IO operations, is pretty simple to understand. Only root accounts can access and modify everything. Besides from that, a
user can only write to his own files and folders, which are files inside of his personal _"/users/some-username/"_ folder. Any user can read from almost
all other files, which (of course) is necessary to be able to execute Hyperlambda files in the system, except for these files.

* The "auth" file or "users" database file
* The p5.data database files
* web.config and app.config

Now of course, this is the low level disc IO authorization features, and simply there as an additional layer of protection for disc IO. In addition
to these authorization features, a user still needs some ways to evaluate some arbitrary piece of lambda in your system, to be able to invoke Active
Events, that actually is able to read these files, and return them to the user. Which for most cases, would be impossible, unless you create some sort
of bug in your system, allowing any normal user to gain access to for instance evaluating the **[eval]** event. You can explicitly override which files a user
is allowed to write to though, by adding an access right object to a role - Which would allow all users belonging to that role, to write/overwrite
files in some specified folder of choice.

Even if you created such a bug though, to access files and/or folders, or modify files and/or folders, the user is not authorized to read/modify,
would still be prevented by this project, as long as it is included in your project as a plugin.

## Creating explicit write access for users to some folder

If you have a user, which you want to create extended rights for, but not allow to have full root access to your system - You can use the p5.auth project's
events to explicitly give write access to some specific folder on disc, at which point this project will check if there exists a **[write-folder]**
access right for the role of the user trying to modify a file or folder - And only if such an access right exists, the user will be allowed to modify the
content of some specified folder. The structure of such an **[access]** object should look like the following.

```
// Role name
developer:some-unique-string-id
  write-folder:/modules/
```

The above access right object for instance, would give all users belonging to the *"developer"* role, write access to the _"/modules/"_ folder.

## Rolling your own

The authorisation logic in P5 is intentionally kept highly naive, to make it more eaily understood. If you wish to have more fine-grained authorisation,
you might want to exchange this project, in addition to also probably the [p5.auth](/plugins/extras/p5.auth) plugin. By doing this correctly, you could 
completely exchange the entire logic of authorisation as you see fit, and still retain compatibility with existing code.
