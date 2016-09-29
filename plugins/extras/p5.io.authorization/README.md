Authorization in Phosphorus Five
===============

This project, is really for internal use, and only used by other plugins, when the system wants to check if a user is authorized
to access or modify a file or folder. If you create your own disc IO Active Events though, you might want to invoke some of these
Active Events yourself, from within your own Active Events, to make sure the user is authorized to access or change your file(s)
or folder(s).

It contains 4 Active Events.

* [p5.io.authorize.read-file], checking if the ApplicationContext's "Ticket" is authorized to reading/loading a file
* [p5.io.authorize.modify-file], checking if the ApplicationContext's "Ticket" is authorized to modify/delete/overwrite a file
* [p5.io.authorize.read-folder], checking if the ApplicationContext's "Ticket" is authorized to reading/loading data from a specific folder
* [p5.io.authorize.modify-folder], checking if the ApplicationContext's "Ticket" is authorized to modify/delete a folder's content

All Active Events takes as their argument the file/folder name as the value of the e.Args node (main node of invocation). In addition, since these
Active Events throws exception if the user (Ticket) is not legally allowed to access a folder, all Active Events takes the invocation node by reference,
as an *[args]* child node. This is to keep contextual information, in case an exception is thrown, such that we can return the Hyperlisp "stack trace"
back to the caller, and supply contextual information about what raised the exception.


