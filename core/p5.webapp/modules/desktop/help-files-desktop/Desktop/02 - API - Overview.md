## Desktop API Overview

The Desktop module contains a rich API, which allows you to perform all sorts of tasks, which are common
to all modules. This includes the responsibility of loading the help system, installation and
uninstallation of apps and modules, logging in and out of the system, etc. Below is a snippet of
Hyperlambda that lists all your installed modules.

```hyperlambda-snippet
/*
 * Lists all installed modules.
 */
desktop.modules.list

/*
 * Removing [description] to not flood our window.
 */
set:x:/@desktop.modules.list/*/*/description

/*
 * Creates a modal widget with the results
 * from above invocation.
 */
create-widgets
  micro.widgets.modal
    widgets
      pre
        innerValue:x:/../*/desktop.modules.list
```

### Evaluating Hyperlambda on next pageload

The Desktop module contains a helper Active Event, that allows you to store some piece of Hyperlambda in
memory, and evaluate this Hyperlambda on the next pageload. This is useful in a lot of scenarios, such as
for instance if you need to reload the location, and provide feedback to the user after having reloaded
the location. This event is called **[desktop.evaluate.on-next-pageload]**, and it expects you to supply
a **[lambda]** lambda callback. Below is an example of usage.

**Warning** - This snippet will reload your location!

```hyperlambda-snippet
/*
 * Creates a "future lambda" object, and reloads
 * the current location.
 */
desktop.evaluate.on-next-pageload
  lambda
    micro.windows.info:Thank you for reloading me!
p5.web.reload-location
```

### URL resolving

When a URL is requested Phosphorus Five will raise the **[p5.web.load-ui]** Active Event.
The Desktop module will handle this event, and evaluate its URL resolver logic,
to figure out which module the client is requesting. Then the Desktop module will evaluate the _"launch.hl"_
file, associated with your module, and your module will take over the request from that point onwards.
This implies that a URL such as for instance `/foo` will evaluate the file `/modules/foo/launch.hl`.
A URL such as `/foo/bar/howdy?hello=world` will _also_ evaluate the `/modules/foo/launch.hl` file.
This allows you to easily create your own modules, such that they contain their own URL resolver logic,
to load up for instance _"virtual"_ pages and such, according to the URL specified by the client. URL
resolving is _"native"_ to Phosphorus Five, and something that _"automagically happens"_.

To retrieve the current URL from within your own module, you can use for instance
the **[p5.web.get-relative-location-url]** event.

### Authentication

You can use the API events to login and log out of the system. The names of these events are as follows.

* __[desktop.authentication.login]__ - Shows a modal window, allowing the user to login to the system
* __[desktop.authentication.logout]__ - Logs out the current user, and deletes his or hers temporary files

**Hint**; You can use the **[whoami]** event to figure out the username and role of the current request.

### Removing the Desktop module

If you want to, you can in its entirety exchange the Desktop module, by editing your web.config
setting called `p5.core.default-app`. This is the preferred way to exchange the Desktop module, since
it doesn't actually remove the module, but rather overrides the default application that is loaded
at your server's root URL - At which point you still keep the URL resolver from the Desktop, in addition to
all of its supporting Active Events. If you choose to remove the Desktop module entirely, and exchange
it with your own module, you must at the very least handle the **[p5.web.load-ui]** event.
If you exchange the default app that is loaded at the root URL by editing your web.config file, then the
desktop will still be available at the [/desktop](/desktop) URL.

**Notice**, there is nothing preventing you from entirely removing the Desktop module from your Phosphorus Five
installation, in addition to all other modules too for that matter. If you do, you _must_ handle the
**[p5.web.load-ui]** event in your own Hyperlambda or C#. If you do this, then Phosphorus Five will
still provide an excellent framework for your needs, allowing you to entirely create your own web apps,
without any of the _"core"_ Phosphorus Five components available though.

### Exchanging the default skin

The default skin to use, for users that haven't overridden the skin themselves explicitly, can be
changed in your web.config, by editing the `p5.desktop.guest-skin` setting.

### Desktop plugins

The Desktop module will add two buttons on all modules automatically, as long as your module has
a widget with the CSS class of _"toolbar"_ associated with it. These buttons allows the user to
launch the help system, in addition to allowing the user to login and logout of the system.
If you don't want these buttons to be automatically injected into your app's toolbar, then you
can do so by simply making sure you do _not_ have any widgets in your page, with the CSS class
of _"toolbar"_. The names of the Active Events that creates these two buttons are as follows.

* __[desktop.plugins.post.create-dox-button]__ - Creates the _"launch documentation"_ button
* __[desktop.plugins.post.create-logout-button]__ - Creates the _"login/logout"_ button

**Notice**, the Desktop module will automatically invoke all Active Events that starts out with the
name `desktop.plugins.pre.` and `desktop.plugins.post.`. These are considered to be global
plugin events, that either should be evaluated before any modules are loaded (_"pre"_), or after
any modules have been loaded (_"post"_). This allows you to create your own global plugins, which
would be accessible from any module in your installation. The Magic Menu exploits this feature, to
inject its _"launch"_ button (the magic wand button) in all modules.
