Users and roles
===============

This folder contains the Active Events related to users and roles management. It allows you to create new users and edit existing users,
and associate roles with them - In addition to creating _"access objects"_, which determines access to some resource, according to a user's
role. All users are stored in the _"auth.hl"_ file at the root of p5.webapp folder, but this can be overridden in your app/web.config file.
Passwords are stored as salted hashed values to increase security. To create a new user, you can use the following code.

```
p5.auth.users.create:john-doe
  password:foo-bar
  role:plain-user
```

The above code will create _"john-doe"_ as a user in your system, and store him into your _"auth.hl"_ file, with the role of _"plain-user"_.
Notice, only _"root"_ accounts can create new, edit, or delete existing users in your systems. Notice also, that we say root accounts in plural form. 
Contrary to on Linux, where there in general terms can only exist one root account - In P5 you can have as many root accounts as you wish, 
with different usernames. I still recommend you to keep your root accounts to a minimum though - Preferably, only one.

Also notice, that only the root account(s) can actually read or modify your _"auth.hl"_ file in your system directly, using for instance **[load-file]**
or **[save-file]** etc. Your _"auth.hl"_ file, or if you choose to use another filename for it, is in general terms, the by far best protected file
in your file system, together with your app/web.config.

## About passwords

Passwords in Phosphorus Five is not validated in any ways in regards to strength, or usage of special characters, etc. This means that you can supply
for instance _"x"_ or _"123"_ as your user's password. These are obviously not very good passwords. However, to avoid creating false security, by adding
constraints, that new research has shown us are actually **decreasing entropy** of passwords - I have chosen to avoid all forms of _"password strength requirements"_, 
since this according to new research, actually has shown us that that it results in passwords with **lower entropy**, than if we allow users to create any 
passwords they want to create.

This allows a user to create a password such as for instance _"ILoveMyDogBecauseItRuns4MeAndU2"_ - Which actually has a surprisingly large amount of entropy, 
compared to for instance _"xY$€%4qW"_. You are of course free to add any constraints you wish in forms and such, where you ask your users to provide new 
passwords for their accounts though.

## About usernames

All users will get their own home folder. This implies creating a folder, with the same name as the username of the user. This restricts the characters
you are legally allowed to use in usernames to the characters legal to use when creating a folder on disc of all the different operating systems that Phosphorus Five
supports running on. For simplicity reasons, and to avoid problems with these folders, we have restricted usernames to only legally be allowed to contain the characters
a-z, -, _ and the numbers 0-9. This is to avoid situations where we have users which only differs by for instance one uppercase character towards another user,
which would create two distinct different folders on Linux and Mac, while reference the same folder on Windows. To avoid such problems, we only allow lowercase
latin characters a-z, the numbers 0-9, "_" and "-" characters as legal characters in usernames.

## Users and your filesystem

When you create a new user, a new folder structure will automatically be created for your user, beneath the _"/core/p5.webapp/users/"_ folder. This is the user's
personal files, and is in general terms, protected such that only that user, and root accounts, can modify or read these files. Each user has a
_"public"_ and a _"private"_ folder. The public folder, is for files which the user doesn't care about is protected or not, and anyone with a direct URL
to the file, can easily download it. The private folder, can only be accessed by the user himself, and/or a root account.
In addition, each user gets his own personal _"temp"_ folder, which is used for temporary files, used whenever a temporary file needs to be created. 
To access the currently logged in user's folder, use the tilde `~`. To load the _"foo.txt"_ file from a user's folder, you can use for instance.

```
load-file:~/foo.txt
```

The tilde above (\~), will be substituted with _"/users/john-doe"_, if the user attempting to evaluate the above code is our _"john-doe"_ user from the 
above Hyperlambda. If your user is a _"guest"_ account (not logged in), the tilde "\~" will evaluate to the _"/common/"_ folder. The common folder, has a 
similar file structure, as the _"/users/"_ folder, and this folder substitution can be used transparently if you wish, without caring who is logged in, 
or whether or not any user is logged in at all. Tilde "~" basically means the _"home folder"_. The _"home folder"_ for guests, who are not logged in, is
the _"/common/"_ folder.

Notice, the above **[load-file]** Hyperlambda, will throw an exception, unless you actually have a _"foo.txt"_ file in your user's home folder. 
In general, you should put files you want for everyone to see, but that still belongs to a specific user, including guests visitors, into 
the _"~/documents/public/"_ folder(s), while protected files, into the _"~/documents/private/"_ folder.
If you wish to create files that are accessible to all users of your system, you should put these files in your _"/common/"_ folder, which is
accessible to all users in your system.

To understand the details of how different roles have access to, or is denied access to, specific files or folders, you might
want to check out the documentation for [p5.io.authorization](/plugins/extras/p5.io.authorization).

## Editing users

To edit a user, use the **[p5.auth.users.edit]** Active Event. To change the role and password of our _"john-doe"_ user from above, you could do something like
the following.

```
p5.auth.users.edit:john-doe
  role:some-other-role
  password:xyz-qwerty
```

The username of a user, _cannot_ be changed, after you have created your user(s). The complete list of operations you can perform on your user objects, 
are as following.

* __[p5.auth.users.create]__ - Creates a new user
* __[p5.auth.users.edit]__ - Edits existing user (changes his or her password, settings, role)
* __[p5.auth.users.list]__ - List all usernames, and their roles, in your system
* __[p5.auth.users.get]__ - Returns one or more named user(s)
* __[p5.auth.users.delete]__ - Deletes one or more user(s) (WARNING! This will delete all files for user(s)!)

The above Active Events, can only be evaluated by a root account.

## User settings

In addition to **[role]** and **[password]**, you can associate any data you wish with your user, such as a user's personal settings, by simply adding these 
as nodes beneath you **[p5.auth.users.edit]** and **[p5.auth.users.create]** invocations. This is a relatively secure placemenet of user data, not intended to 
be publicly visible, such as POP3 account passwords, etc. Consider this for instance.

```
p5.auth.users.edit:john-doe
  role:some-other-role
  password:xyz-qwerty
  some-data:foo-bar
  some-complex-data
    name:foo
    value:bar
```

If you evaluate the above Hyperlambda, then both **[some-data]** and **[some-complex-data]** will be associated with the _"john-doe"_ user as user settings. To
retrieve the settings for the currently logged in user, you can use the **[p5.auth.my-settings.get]** Active Event. The user's settings are a convenient place to
store small pieces of user related data, which are unique to each user in your system, such as name, email address, physical address, etc.

You can also change your currently logged in user's settings, with the following code.

```
p5.auth.my-settings.set
  some-data:foo-bar
  some-complex-data
    name:foo
    value:bar
```

The difference is that **[p5.auth.users.edit]** can only be invoked by root accounts, and can modify any user's settings - While **[p5.auth.my-settings.set]**
can be invoked by all users, except guest users of course (which actually aren't real users, and hence have no actual settings object).
To get your user's settings, use the **[p5.auth.my-settings.get]** Active Event. To change the passsword of the currently logged in user, use the following code.

```
p5.auth.misc.change-my-password:bar
```

The above Hyperlambda will change the passsword for the currently logged in user to _"bar"_.

## [whoami], figuring out who you are

To see who you are, you can invoke **[whoami]**, like the following code illustrates.

```
whoami
```

The above Hyperlambda will return the following. Notice, it will not return settings or the password for the user.

```
whoami
  username:root
  role:root
  default:bool:false
```

For the default guest user, meaning somebody who is not logged in, it will return something like the following, unless you've modified the default guest context
in your web/app.config.

```
whoami
  username:guest
  role:guest
  default:bool:true
```

Notice the **[default]** node above, which is the correct way to check if a user is logged in or not, since the name of the guest role, at least in theory,
can be changed to something else than _"guest"_. Although, this is not recommended, it is possible to do.
If you wish, you can invoke an Active Event that deletes the currently logged in user. This Active Event is called **[p5.auth.misc.delete-my-user]**, and
will completely delete the currently logged in user, including his or hers files. This will (obviously), also log you out of the system. Notice, this
Active Event will not destroy the session associated with the user though. Make sure you remove all session values if you choose to use it.

## Logging in and out

To log in, use **[login]**. The login Active Event takes 3 parameters;

* __[username]__
* __[password]__
* __[persist]__

If you set **[persist]** to true, then a persistent cookie will be created if possible, with the duration of your web/app.config setting 
called _"p5.auth.credential-cookie-valid"_ number of days, making sure the client you're using to login, don't have to login again, 
before that timespan has passed. To logout, simply invoke **[logout]**.

### Creating lambda callbacks for [login] and [logout]

If you wish, you can create an **[.onlogin]** and/or an **[.onlogout]** lambda callback setting(s), which will be invoked, when the user logs in our out. An 
example of a callback, creating a dummy textfile when your currently logged in account is logging out, can be found below.

```
p5.auth.my-settings.set
  .onlogin
    save-file:~/foo-bar-file-login.txt
      src:"This was created when my user logged in!!"
  .onlogout
    save-file:~/foo-bar-file-logout.txt
      src:"This was created when my user logged out!!"
```

If you evaluate the above Hyperlambda, for then to logout and back into your system, you will find two files in your user's folder 
called _"foo-bar-file-login.txt"_, and _"foo-bar-file-logout.txt"_. These files were created when your user logged in and out of the system.
Notice, these are user specific lambda callbacks, and not global for all users.

## The role system

The default role system in Phosphorus Five, is actually quite naive. In fact, the above **[role]** string, will become your user's role. There are no
role definitions in P5. The list of roles, is defined as the distinct roles from all users in your system. Still, you can easily associate pages, 
and access to such, and other objects, with one or more roles. There are two special roles in the system though.

* __[root]__ - Root accounts, can do everything
* __[guest]__ - Guest accounts. This is the default role every request belongs to, unless it has explicitly logged in. It basically imples _"random user, not logged in at all"_

To list all roles in your system, you can execute the following code.

```
p5.auth.roles.list
```

The above will result in something similar to the following.

```
p5.auth.roles.list
  guest
  root:int:1
```

The integer number behind the role's name, is the number of users belonging to that role. Besides from that, there's not really much to say about the
role system in Phosphorus Five. If you wish, you can roll your own, much more complex role system though. This is easily done, by exchanging this entire
component out with your own. If you do, you will still probably want to at least provide all Active Events, following at least as close to as possible, 
the same rough API as this project does, to make sure you don't break existing code. If you want a more advanced access right system, you might also want 
to replace the [p5.io.authorization](../p5.io.authorization/) project. This project is also a part of the authentication/authorisation logic of P5, and
contains helper events for allowing users access to things such as reading and modifying files, etc. It also allows to create access objects, for roles that somehow 
should have extended rights, to doing some sort of operation in P5, such as saving and modifying files in some specific folder, etc.

### Explicitly granting or denying access to roles

The role system is extendible, and allows you to create your own access objects, which will be serialized into the _"auth"_ file. These
additional access objects will be physically stored in your auth file beneath an **[access]** node. The access object parts of Phosphorus Five
has 4 helper active events to do this.

* __[p5.auth.access.list]__ - Returns all access objects
* __[p5.auth.access.add]__ - Adds a new access object
* __[p5.auth.access.delete]__ - Deletes an existing access object
* __[p5.auth.has-access-to-path]__ - Returns whether or not a user has access to some _"path"_ or not

**Important** - Only root accounts can modify the access object(s). The **[p5.auth.has-access-to-path]** Active Event from above,
is a helper event, that allows you to easily determine if a user has access to something that resembles a _"path"_ or not.
It is consumed in [p5.io.authorization](../p5.io.authorization/) for instance, when determining if a user has access to files and folders -
But can also be used to for instance verify that a user has access to some URL, or other parts of your system, that can be sequentially built,
with the same _"tree/graph"_ semantics as URLs and file/folder paths.

The access objects is in such a regards extendible, and allows you to create your own access objects, necessary to implement
authorization for your own applications. To see an example of how to do this, check out for instance 
the [p5.io.authorization](/plugins/extras/p5.io.authorization) project, and its code.
To create an access object, you could do something such as the following.

```
p5.auth.access.add
  developer:allow-write-developer-foo
    p5.io.write-file.allow:/foo/
```

The above access object for instance, would give all users belonging to the _"developer"_ role write access to the _"/foo/"_ folder
on your disc. But you can can create any types of access objects you wish. Below is an example.

```
p5.auth.access.add
  *:allow-something-for-all
    your-namespace.do-some-action.allow:on-some-object
```

Whatever the above implies, is up to you to decide for yourself though. The access object really only has one restriction, which is that
the value parts of each access object you create, must have a unique name - Otherwise deleting a single specified access object, would
be impossible. You can of course create these names any ways you see fit of course, including using randomly generated Guids as your IDs.
And in fact, if you do not supply an explicit name, then a random GUID will be used, after having been converted into a string though.
In fact, it is probably better to not supply an ID when creating a new access object, since this will ensure a randomly and unique GUID
becomes its ID.

Notice, the access object system does not verify that the role or object you try to grant or deny access to actually exists. It
is quite possible for you, to grant access to a non-existing role, to a non-existing folder.


## Rolling your own authorization/authentication system

The p5.auth project is explicitly implemented as a _"naive"_ and simple project. It is highly secure though, and for instance stores
passwords as salted hash values, etc - But it is not complex in nature, and does not feature advanced things, such as a tree based role
system, etc. If this does not fit your needs, and you need a more complex role based system for instance - Then it is probably easily replaced,
by your own authorization/authentication system, which you can build any ways you see fit. For instance, in p5.auth a _"role"_ is actually
just a simple string, and there doesn't exist any true _"roles"_ in Phosphorus Five, with referential integrity and such. To create a new
role, implies just assigning some role string to a user, as you save it, and that will create a new _"role"_ in the system. If this does not
fit your needs, creating your own _"auth"_ system, and pluging this into Phosphorus Five in general, should probably be easily achieved,
as long as you simply create (at least) all the Active Events that exists in this project, with different implementations though.

**Notice** - Even though p5.auth is _"naive"_ and _"simple"_ in nature, it should still be considered **highly secure**!


## Active Events, and sample usage

Below you can find some examples, and the extensive documentation, of all Active Events from the p5.auth project.

### [p5.auth.users.create]

Creates a new user. Can only be invoked by a root user, and requires the username to be the value of the root node, in addition to
a **[role]** and **[password]** argument.

```
p5.auth.users.create:foo-bar
  role:foo
  password:bar
```

### [p5.auth.users.get]

Returns the specified user's role and settings. Can only be invoked by a root user, and can take either a constant, or an expression as
its value, to retrieve multiple users if needed.

```
.data
  root
p5.auth.users.get:x:/-/*?name
```

### [p5.auth.users.edit]

Edits the specified user. Can only be invoked by a root user, and requires the username to be the value of the main node of the invocation.
Optionally pass in **[password]** and **[role]**, which if given, will update the user's role and password. If you don't supply a new
password, or a new role, the existing password and/or role will not be changed. Notice, this event will also change the user's settings,
to whatever is supplied besides **[password]** and **[role]** - Which implies that unless you pass in the old settings to it, or some new 
settings values, then all settings for the user will actually be deleted. You can though invoke it with the alias of **[p5.auth.users.edit-keep-settings]**, 
which will not touch the user's settings in any ways.

### [p5.auth.users.delete]

Deletes one or more users. Can only be invoked by a root account. Pass in user(s) to delete, either as a constant, or as an expression leading
to multiple usernames.

```
/*
 * Creating two dummy users, which will be deleted immediately.
 */
p5.auth.users.create:foo
  password:foo
  role:user
p5.auth.users.create:bar
  password:bar
  role:user

/*
 * Deleting both users created above.
 */
.users
  foo
  bar
p5.auth.users.delete:x:/-/*?name
```

### [p5.auth.users.list]

Lists all users in system, and their associated role. Can only be invoked by a root account.

```
p5.auth.users.list
```

### [p5.auth.my-settings.get]

Retrieves the currently logged in user's settings. Can be invoked by any user, except the default _"guest"_ user.

```
p5.auth.my-settings.get
```

### [p5.auth.my-settings.set]

Updates the currently logged in user's settings. Can be invoked by any user, except the default guest user. Notice, this will
update all settings, which implies that you'll need to explicitly take care when invoking it, such that you don't accidentally
delete settings for other applications besides the one(s) you're creating yourself. Normally, you'd invoke **[p5.auth.my-settings.get]**
first, then delete your app's settings from its return value, add the updated settings for your app, and pass in all settings
from your get invocation to your set invocation, to make sure you don't accidentally delete another app's settings.

**Warning** - The next piece of code, will delete your user's existing settings. To understand why, read the previous paragraph.

```
p5.auth.my-settings.set
  foo:bar
  bar
    howdy:world
```

### [p5.auth.roles.list]

Lists all roles in the system, and the number of user's belonging to each role. Can only be invoked by a root account.

```
p5.auth.roles.list
```

### [p5.auth.misc.whoami]

Returns the **[role]**, **[username]** and **[default]** value of the current logged in user. Can be invoked by anyone.
Has an alias with the name of **[whoami]**. To determine if a user is logged in at all, check the **[default]**
return value from this event, and verify it is _"false"_, at which point the current context ticket, is an actual logged in
user. If **[default]** is _"true"_, it implies that the user is not logged in at all, but simply a _"guest"_ visitor.

```
whoami
```

### [p5.auth.misc.change-my-password]

Changes the password for the currently logged in user. Can be invoked by anyone, as long as it is an actual logged in user.
Pass in the new password as the value of the main invocation node.

```
p5.auth.misc.change-my-password:some-new-password-goes-here
```

### [p5.auth.misc.delete-my-user]

Deletes the currently logged in user. Cannot be invoked for a root account. If you want to delete a root account, you'll have to
first make it a non-root account, and then afterwards delete it. This is a security feature, to prevent accidental deletion of
root accounts, such that the system becomes impossible to maintain and administer.

```
p5.auth.misc.delete-my-user
```

### [p5.auth.login]

Logs in a user. Requires a **[username]**, **[password]**, and optionally a **[persist]** argument. Has a **[login]** alias.
If **[persist]** is true, the user will be persistently logged in, implying he won't have to login again, from the same
terminal, for a specific amount of days. See your app's config file to change the number of days a user becomes persistently 
logged in, before he has to login again.

```
login
  username:foo
  password:bar

  // Optional
  persist:bool:true
```

### [p5.auth.logout]

Logs out the currently logged in user. Has the **[logout]** alias. Requires no arguments.

```
logout
```

### [p5.auth.access.list]

List all access objects in system. Notice, this will by default return all access objects if the user invoking
the event is a root account, and only access objects relevant for the currently logged in user's role if the user
is not a root user. If you are logged in as root, you can however supply an additional argument as **[role]**, and pass
in the name of the role you want to retrieve access objects on behalf of - Effectively only returning access objects
delevant for a user belonging to the specified **[role]**. The latter will throw an exception though, if you invoke
it with a **[role]** argument, and your user is not a root user.

```
p5.auth.access.list
```

### [p5.auth.access.add]

Creates a new access object. Can only be invoked by a root user. This event will create a new access object, whatever that
implies, since the access object system of Phosphorus Five is extendible. Read further up in this document to understand what
that implies. Each access object must have a unique ID, which becomes the value of the access object, and its name becomes
the name of the role the access object is referencing. You can use an asterix (*) though, to imply _"all roles"_. Beneath
the main access object's node, you must supply at the very least one argument, with a name being the _"action"_, and its
value being some _"resource"_. See further up in this document, for an example of how for instance p5.io is using the
access object(s), to allow or deny read/write access to files and folders.

```
p5.auth.access.add
  user:some-unique-id
    some-action:some-resource
```

### [p5.auth.access.delete]

Deletes some named access object. Can only be invoked by a root account. To delete the access object we created above for instance,
you could use something such as the following.

```
p5.auth.access.delete
  user:some-unique-id
```

### [p5.auth.access.set-all]

Sets the entire access object list, deleting its old values. Can only be invoked by root. This will delete all existing access objects,
and set the access object list to whatever you supply as arguments.

```
p5.auth.access.delete
  user
    some-action:some-resource
  another-user
    some-other-action:some-other-resource
```

### [p5.auth.has-access-to-path]

Determines if the currently logged in user has access to some _"path"_ or not. Can be invoked by any user. This is primarily
used when determining access to files and folders on disc, but has been explicitly been made public, since similar use cases
are quite useful, also for other types of scenarios, such as URLs, etc. It requires two arguments; **[filter]** and **[path]**.
Filter is some sort of namespace, such as for the file system for instance _"p5.io.read-file"_ or _"p5.io.write-file"_.
The **[path]** argument, is some sort of relational path type of argument, such as a URL or a filename, which both are
in fact tree structures, allowing for access to objects of such types to actually _"cascade"_.

This event will fetch all access objects relevant for the user's role, having either the value of _"your-filter.deny"_ 
and _"your-filter.allow"_. For instance, for the above file IO example, it will retrieve _"p5.io.read-file.allow"_ access
rights, in addition to _"p5.io.read-file.deny"_ access rights. Then it will sort these alphabetically according to their values,
and then traverse them sequentially, to determine if a user has access to, or does not have access to, the **[path]** specified.

To see an example of usage of this, feel free to check up the [p5.io.authorization](/plugins/extras/p5.io.authorization) project.
Below is an example of usage.

```
p5.auth.has-access-to-path
  filter:p5.url.has-access
  path:/foo/bar
```

If you create two access objects with the following code.
```
p5.auth.access.add
  *:its-unique-id
    p5.url.has-access.deny:/foo/bar
  developer:its-other-unique-id
    p5.url.has-access.allow:/foo/bar
```

And you invoke **[p5.auth.has-access-to-path]** with the following example code.

```
p5.auth.has-access-to-path
  filter:p5.url.has-access
  path:/foo/bar
```

Then it will return true if the currently logged in user is in the role of _"developer"_, but return
false for all other users. Notice though, it will always return true for a root account. This event
is quite useful for a lot of your own extension scenarios, where you have some sort of relational object,
which you need to give conditional access to, according to whatever role the user belongs to.


## Warning

Yet again, the authorization logic in p5.auth has a relatively _"naive"_ and simple to understand
implementation. Among other things, it persists all of its access objects, users, and roles, in a single
file on disc. If you have thousands of users, hundreds of roles, with dozens of access objects each,
and each user having dozens of settings each - You might for all practical concerns risk to have the 
system simply not scale, since it's based upon a single file, among other things. In addition, the 
roles are simple strings, etc.

So even though it is highly secure, it probably doesn't scale into _"infinity"_. If you want to however,
replacing it with your own access system, is probably quite easy. Have this in mind as you
extend your system. I'd probably use it by default if I were you, for later to replace it if
the needs arrives - Which you can probably easily see, if as you start adding users, settings, 
access, and role objects to the system - The system as a whole starts becoming more sluggish and
less responsive, due to your memory simply having been consumed by your _"auth.hl"_ file.
