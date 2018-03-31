## Desktop API - Installation and Uninstallation of modules

In addition to the previously documented API parts, the desktop also contains helper Active Events to
install and uninstall modules and applications. You can find these events below.

* __[desktop.modules.install]__ - Installs a new module. Expects __[\_arg]__ being the path to a zip file, containing your module
* __[desktop.modules.uninstall]__ - Uninstalls the specified __[\_arg]__ module

### Installing modules

When you install a new module, using the **[desktop.modules.install]** event, you can optionally
supply a **[local-url]** argument. This will be the name of the module, and/or its URL. If you don't
supply a **[local-url]** argument, the zip file's name will be used by default. Notice, if your
zip file, containing your module has versioning numbers in it, or similar constructs, which often
is the case when you for instance release modules through GitHub - You might want to explicitly
supply a **[local-url]**, to avoid having multiple modules with different versions being installed
side by side - Which can create all sorts of _"weird problems"_ for you. Or, make sure you remove the
versioning information from your zip file, before you install it.

**Notice**, when you install a module, any previously installed modules with the same name will
be uninstalled before your module is installed.

### Uninstalling modules

To uninstall a module, you can use the **[desktop.modules.uninstall]** event, and pass in either
the fully qualified path to your module, or simply its name, for instance _"hello"_, if your module
is named _"hello"_. If the uninstallation process was successful, this event will return a boolean
_"true"_ value.

**Notice**, you can invoke this event with the path/name to a module that doesn't exist, at which
point the event will simply return _"false"_, and do nothing.

If you go through the _"Create new module"_ wizard in Hyper IDE, and create a module called _"hello"_,
you can afterwards uninstall this module by invoking the snippet below.

```hyperlambda-snippet
/*
 * Uninstalls the "hello" module, assuming you followed
 * the "Create new module" wizard in Hyper IDE, and
 * created a module called "hello".
 *
 * Notice, will not do anything unless a "hello" module
 * actually exists.
 */
desktop.modules.uninstall:hello

/*
 * Shows a modal window with the results of our
 * uninstallation process.
 *
 * Notice, if the modules doesn't exist, the event
 * result will be "false".
 */
eval-x:x:/+/*/*/*/*
create-widgets
  micro.widgets.modal
    widgets
      pre
        innerValue:x:/@desktop.modules.uninstall
```

### Module installation internals

Many modules will have a file called _"install.hl"_, in addition to a file called _"uninstall.hl"_ at their
root folder. These files are evaluated during installation and uninstallation, and they're expected to
initialize and uninitialize your module - Whatever that implies for your module. This might include
creating any databases the module is dependent upon, or doing other tasks that are necessary to perform
before the module can be used.

**Notice** - If you want to create custom Active Events as a part of your module, you
should do this in a file called _"startup.hl"_, since this file will be evaluated every time the web
server process reboots, or is being recycled somehow. If you look at any of the core modules in Phosphorus
Five, you will see how they create their Active Event in their _"startup.hl"_ file - Often indirectly,
by evaluating all files in some specific folder.
