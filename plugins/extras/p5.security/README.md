Authorization and authentication
===============

This folder contains the Active Events related to users and roles management. It allows you to create new users, associate roles with them, and 
all associated concepts. All users are stored in the _"auth.hl"_ file at the root of p5.webapp, but this can be overridden in your web.config.
Passwords are stored in salted hashed values, to increase security. To create a new user, you can use the following code.

```
create-user
  username:john-doe
  password:foo-bar
  role:plain-user
```

The above code will create "john-doe" as a user in your system, and store him into your _"auth.hl"_ file.

Notice, only _"root"_ accounts can create new, edit or delete users in your systems. Notice, we say root accounts in plural form. Contrary to on Linux,
where there in general terms only exist one root account, in P5 you can have as many root accounts as you wish. Though, I encourage you to keep your
root accounts to a minimum. Preferably, only one!

Also notice, that only the root account(s) can actually read or modify your _"auth.hl"_ file in your system directly, using for instance *[load-file]*
or *[save-file]* etc.

When you create a new user, a new folder structure will automatically be created for your user, beneath the p5.webapp/users/ folder. This is the user's
personal files, and is in general terms, protected such that only that user, and a root account, can actually control these files. Each user has a
"public" and a "private" folder. The public folder, is for files which the user doesn't care about is protected or not. The private folder, can only
be accessed by the user himself, and/or a root account.

In addition, each user gets a "temp" folder, which are used for temporary files, used whenever a temporary file needs to be created. To access the 
currently logged in user's folder, use the tilde "~". To load the _"foo.txt"_ file from a user's folder, you can use for instance.

```
load-file:~/foo.txt
```

The tilde above (~), will be substituded with _"/users/john-doe"_, if the user attempting to evaluate the above code is our _"john-doe"_ user from above.

Notice, the above *[load-file]* Hyperlambda, will throw an exception, unless you actually have a "foo.txt" file in your user's folder.

To edit a user, use the *[edit-user]* Active Event. To change the role and password of our _"john-doe"_ user from above, you could do something like
the following.

```
edit-user:john-doe
  role:some-other-role
  password:xyz-qwerty
```

The username of a user, _cannot_ be changed, after you have created your user(s).

The complete list of operations you can perform on your user objects, are as following.

* [create-user] - Creates a new user
* [edit-user] - Edits existing user (changes his or her password, settings, role)
* [list-users] - List all usernames, and their roles, in your system
* [get-user] - Returns one or more named user(s)
* [delete-user] - Deletes a user (WARNING! This will delete all his files!)

The above Active Events, can only be evaluated by a root account.

## User settings

In addition to *[role]* and *[password]*, you can associate any data you wish with your user, such as settings and such, by simply adding these as nodes
beneath you *[edit-user]* and *[create-user]* invocations. Consider this for instance.

```
edit-user:john-doe
  role:some-other-role
  password:xyz-qwerty
  some-data:foo-bar
  some-complex-data
    name:foo
    value:bar
```

If you evaluate tha above Hyperlambda, then both *[some-data]* and *[some-complex-data]* will be associated with the _"john-doe"_ user, as settings. To
retrieve the settings for the currently logged in user, you can use the *[user-settings]* Active Event. The user's settings are a convenient place to
store small pieces of user related data, which are unique to each user in your system.

You can also change your currently logged in user's settings, with the following code.

```
set-my-user-settings
  some-data:foo-bar
  some-complex-data
    name:foo
    value:bar
```

The difference is that *[edit-user]* can only be invoked by root accounts, while the latter, *[set-my-user-settings]* can be invoked by all users, except
"guest" users of course (which actually aren't "real users").

To get your user's settings, use the *[get-my-user-settings]* Active Event.

To change the passsword of the currently logged in user, use the following code.

```
change-password:bar
```

The above Hyperlambda will change the passsword for the currentlylogged in user to _"bar"_.

To see who you are, you can invoke *[whoami]*, like the following code illustrates.

```
whoami
```

The above Hyperlambda will return the following.

```
whoami
  username:root
  role:root
  default:bool:false
```

For the default guest user, meaning somebody who is not logged in, it will return the following.

```
whoami
  username:guest
  role:guest
  default:bool:true
```

Notice the *[default]* node above, which is the correct way to check if a user is logged in or not, since the name of the guest role, at least in theory,
can be changed to something else than "guest". Although, this is not recommended, it is possible to do.

Notice, if you wish, you can invoke an Active Event that deletes the currently logged in user. This Active Event is called *[delete-my-user]*, and
will completely delete the currently logged in user, including his or hers files. This will (obviously), also log you out of the system. Notice, this
Active Event will not destroy the session associated with the user though. Make sure you remove all session values if you choose to use it.

## Logging in and out

To log in, use *[login]*. The login Active Event takes 3 parameters;

* [username]
* [password]
* [persist]

If you set *[persist]* to true, then a persistent cookie, will be created, with the duration of your web.config setting called _"p5.security.credential-cookie-valid"_
number of days, making sure the client you're using to login, don't have to login again, before that timespan has passed.

To logout, simply invoke *[logout]*.

### Creating lambda callbacks for [login] and [logout]

If you wish, you can create an *[.onlogin]* and/or an *[.onlogout]* lambda callback setting(s), which will be ivoked, when the user logs in our out. An 
example of a callback, creating a dummy textfile when your currently logged in account is logging out, can be found below.

```
set-my-user-settings
  .onlogin
    save-file:~/foo-bar-file-login.txt
      src:"This was created when my user logged in!!"
  .onlogout
    save-file:~/foo-bar-file-logout.txt
      src:"This was created when my user logged out!!"
```

If you evaluate the above Hyperlambda, for then to logout and back into your system, you will find two files in your user's folder 
called "foo-bar-file-login.txt", and "foo-bar-file-logout.txt". These files were created when your user logged in and out of the system.

Notice, these are user specific lambda callbacks, and not globally for all users.

## The role system

The default role system in Phosphorus Five, is actually quite naive. In fact, the above *[role]* string, will become your user's role. There are no
role lists in P5. The list of roles, is defined as the distinct roles from all users in your system. Still, you can associate pages and other objects
with one or more roles.

There are two special roles in the system though.

* [root] - Root accounts, can do everything
* [guest] - Guest accounts. This is the default role every request belongs to, unless it has explicitly logged in

To list all roles in your system, you can execute the following code.

```
list-roles
```

The above will result in something similar to the following.

```
list-roles
  guest
  root:int:1
```

The integer number behind the role's name, is the number of users belonging to that role. Besides from that, there's not really much to say about the
rrole system in Phosphorus Five. If you wish, you can roll your own, much more complex role system though. This is easily done, by exchanging this entire
project. If you do, you will still probably want to at least provide all Active Events, following at least as close to as possible, the same rough API
as this project does, to make sure you don't break existing code.

If you want a more advanced access right system, you might also want to replace the [p5.io.authorization](../p5.io.authorization/) project.

In addition, p5.security also defines some "protected" Active Events, which are only accessible for C# code. These events are beyond the scope
of this documentation. Check out the source code if you're interested.
