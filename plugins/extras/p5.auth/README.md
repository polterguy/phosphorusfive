Users and roles
===============

This folder contains the Active Events related to users and roles management. It allows you to create new users and associate roles with them.
All users are stored in the _"auth.hl"_ file at the root of p5.webapp folder, but this can be overridden in your app/web.config.
Passwords are stored in salted hashed values to increase security. To create a new user, you can use the following code.

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

## Users and your filesystem

When you create a new user, a new folder structure will automatically be created for your user, beneath the _"/p5.webapp/users/"_ folder. This is the user's
personal files, and is in general terms, protected such that only that user, and root accounts, can modify or read these files. Each user has a
_"public"_ and a _"private"_ folder. The public folder, is for files which the user doesn't care about is protected or not, and anyone with a direct URL
to the file, can easily download it. The private folder, can only be accessed by the user himself, and/or a root account.
In addition, each user gets his own personal _"temp"_ folder, which is used for temporary files, used whenever a temporary file needs to be created. 
To access the currently logged in user's folder, use the tilde `~`. To load the _"foo.txt"_ file from a user's folder, you can use for instance.

```
load-file:~/foo.txt
```

The tilde above (\~), will be substituted with _"/users/john-doe"_, if the user attempting to evaluate the above code is our _"john-doe"_ user from the 
above Hyperlambda. If your user is a _"guest"_ account (not loggedd in), the tilde "\~" will evaluate to the _"/common/"_ folder. The common folder, has a 
similar file structure, as the _"/users/"_ folder, and this folder substitution can be used transparently if you wish, without caring who is logged in, 
or whether or not any user is logged in at all. Tilde "~" basically means the _"home folder"_. The _"home folder"_ for guests, who are not logged in, is
the _"/common/"_ folder.

Notice, the above **[load-file]** Hyperlambda, will throw an exception, unless you actually have a _"foo.txt"_ file in your user's home folder. 
In general, you should put files you want for everyone to see, but that still belongs to a specific user, including guests visitors, into 
the _"~/documents/public/"_ folder(s), while protected files, into the _"~/documents/private/"_ folder.
If you wish to create files that are accessible to all users of your system, you should put these files in your _"/p5.webapp/common/"_ folder, which is
accessible to all users in your system.

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
store small pieces of user related data, which are unique to each user in your system, such as name, email address, twitter handler, etc.

You can also change your currently logged in user's settings, with the following code.

```
p5.auth.my-settings.set
  some-data:foo-bar
  some-complex-data
    name:foo
    value:bar
```

The difference is that **[p5.auth.users.edit]** can only be invoked by root accountsn and can modify any user's settings - While **[p5.auth.my-settings.set]**
can be invoked by all users, except guest users of course (which actually aren't real users, and hence have no settings).
To get your user's settings, use the **[p5.auth.my-settings.get]** Active Event. To change the passsword of the currently logged in user, use the following code.

```
p5.auth.misc.change-my-password:bar
```

The above Hyperlambda will change the passsword for the currentlylogged in user to _"bar"_.

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

* [username]
* [password]
* [persist]

If you set **[persist]** to true, then a persistent cookie will be created if possible, with the duration of your web/app.config setting 
called _"p5.auth.credential-cookie-valid"_ number of days, making sure the client you're using to login, don't have to login again, 
before that timespan has passed. To logout, simply invoke **[logout]**.

### Creating lambda callbacks for [login] and [logout]

If you wish, you can create an **[.onlogin]** and/or an **[.onlogout]** lambda callback setting(s), which will be ivoked, when the user logs in our out. An 
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
contains helper events for allowing users access to things such as reading files, etc. It also allows to create access objects, for roles that somehow 
should have extended rights, to doing some sort of operation in P5, such as saving and modifying files in some specific folder, etc.
