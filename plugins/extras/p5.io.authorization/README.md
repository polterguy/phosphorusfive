Authorization in Phosphorus Five
===============

This project, is really for internal use, and only used by other plugins, when the system wants to check if a user is authorized
to access or modify a file or folder. If you create your own disc IO Active Events in C#, you might want to invoke some of these
Active Events yourself, from within your own Active Events, to make sure the user is authorized to access or change your file(s)
or folder(s).

It contains 4 Active Events.

* [.p5.io.authorize.read-file], checking if the ApplicationContext's "Ticket" is authorized to reading/loading a file
* [.p5.io.authorize.modify-file], checking if the "Ticket" is authorized to modify/delete/overwrite a file
* [.p5.io.authorize.read-folder], checking if the "Ticket" is authorized to reading/loading data from a folder
* [.p5.io.authorize.modify-folder], checking if the "Ticket" is authorized to modify/delete a folder's content

All Active Events above, requires as their argument, the file/folder name as the value of the e.Args node (main node of invocation). In addition, since these
Active Events throws exception if the user ("Ticket") is not legally allowed to access a folder, all Active Events takes the invocation node by reference,
as an *[args]* child node. This is to keep contextual information, in case an exception is thrown, such that we can return the Hyperlisp "stack trace"
back to the caller, and supply contextual information about what raised the exception.

To see an example of how to raise these Active Events yourself, you can check out the code for [p5.io](/plugins/p5.io/).

The logic of authorizating IO operations, is pretty simple to understand. Only root accounts can access and modify everything. Besides from that, a
user can only write to his own files and folders, which are files inside of his personal "/users/some-username/" folder. Any user can read from almost
all other files, which (of course) is necessary to be able to execute Hyperlisp files in the system, except for these files.

* The "auth" file or "users" database file
* The p5.data database files
* Any file ending with ".config"

Now of course, this is the "low level disc IO authorization" features, and simply there as an additional layer of protection for disc IO. In addition
to these authorization features, a user still needs some ways to evaluate some arbitrary piece of p5.lambda in your system, to be able to invoke Active
Events, that actually is able to read these files, and return them to the user. Which for most cases, would be impossible, unless you create some sort
of bug in your system, allowing any "normal" user to gain access to for instance the "System42/executor", or some similar piece of Hyperlisp evaluator.

Even if you created such a bug though, to access files and/or folders, or modify files and/or folders, the user is not authorized to read/modify,
would still be prevented by this project, as long as it is included in your end result as a "plugin".



