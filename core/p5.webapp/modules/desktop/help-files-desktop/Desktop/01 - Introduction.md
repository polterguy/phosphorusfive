## Introduction

The _"Desktop"_ module, is normally the root of your Phosphorus Five installation, and responsible for URL resolving -
In addition to providing other services to your modules, and Phosphorus Five in general. The Desktop module
is also completely loosely coupled from all other modules, and the core itself, and can actually be exchanged with your
own module. It looks like the following, depending upon which apps you've installed, and whether
or not you're logged into Phosphorus Five or not.

https://phosphorusfive.files.wordpress.com/2018/03/desktop-screenshot-five-apps.png

As you can see above, the Desktop module will create one _"desktop launcher icon"_ for each app you've
installed in your installation. This launcher icon is actually dynamically created from your apps'
_"desktop.hl"_ file, and can to a large extent be modified as you see fit. See the _"Hyperlambda"_ section
of the documentation for a more thorough description of how to create your own desktop icons.

### Installing and uninstalling modules

To install a new module, simply click the _"+"_ button in your toolbar, on your Desktop, while you're
logged in as _"root"_. This will allow you to browse to a zip file on your local computer, and install
a new module. Below is a screenshot of the installation process, assuming you've just uploaded a zip file called
_"hello.zip"_, but not yet chosen to install it.

https://phosphorusfive.files.wordpress.com/2018/03/install-new-module-new-screenshot.png

Uninstallation is equally easy, simply click the _"-"_ button, at which point Phosphorus will
display all your installed modules, and allow you to uninstall these. Below is a screenshot.

https://phosphorusfive.files.wordpress.com/2018/03/desktop-uninstall-module.png

**Notice**, if you have a zip file which you are not entirely sure is safe, you can upload it using Hyper IDE,
and unzip it, to your _"/modules/"_ folder, and then read its code. When you have confirmed it is safe, you
can evaluate its _"startup.hl"_ file, before you allow non-root accounts to access the module.

### Settings and skinning your Desktop

The desktop module allows you to change the skin for your Phosphorus Five installation, which is a global
setting, used for all (most) modules you install. You can access your settings, and change your skin,
by clicking the _"cog"_ icon at the top right corner while you have your desktop module loaded. Below
is a screenshot of how this will look.

https://phosphorusfive.files.wordpress.com/2018/03/desktop-settings-screenshot.png

There is a whole range of skins in Phosphorus Five, that is distributed out of the box. All of
these skins are created such that they are rendered _"responsively"_,
which implies that they should perfectly well work, also on your phone or your tablet, in addition
to your main laptop or desktop computer.

It is also very easy to create your own skin, due to some highly intuitive CSS tricks applied
by Phosphorus Five, or the core Micro CSS framework to be more accurate. And in fact, in Hyper IDE there
is even a _"wizard"_ you can use to create your own skin. We will look at how to
create our own skins in the documentation for _"Micro"_ later in these documentation files. Below
is how Hyper IDE will look like, if you choose the _"Magic Forrest"_ skin.

https://phosphorusfive.files.wordpress.com/2018/03/hyper-ide-magic-forrest-skin-screenshot.png

Below you can see how Camphora Five will look like if you choose the _"Aztec"_ skin.

https://phosphorusfive.files.wordpress.com/2018/03/camphora-screenshot-aztec.png

Some skins have larger fonts than others, while some have darker colors. As you install Phosphorus Five,
you might benefit from taking some time initially to pick your skin, according to your needs. You can also
create your own skin, entirely from scratch, if none of the skins that comes out of the box seems to fit
your needs. You can also select your own personal CodeMirror theme, from for instance Hyper IDE's settings,
to further customize how Phosphorus Five appears.

### Advanced - Installation

**Notice**, sometimes you want to install your modules with a different name than what the name of your
zip file happens to be. For such scenarios, you can override the **[local-url]** property, by after having
uploaded your file, choose to provide your own module name. Maybe you've got a file named _"foo-bar-version-5.zip"_
for instance, but you'd like to install this as _"foo-bar"_ module. Most of the times though, Phosphorus Five
will intelligently figure out the module name by itself, and this property is _not_ something you should
change - Unless you understand the consequences. Among other things, you might run the risk of having
multiple versions of the same module installed side by side if you get this wrong - Which creates all sorts
of weird problems for you! If you get funny bugs, make sure you don't have installed the same module multiple
times in different folders. This _will_ create weird errors for you, as you use your Phosphorus Five
installation. Phosphorus Five does not support having the same module installed multiple times,
particularly not with different versions.

You can use Hyper IDE to delete folders, and you can use the following Hyperlambda snippet to _"re-initialize"_
your server, forcing it to go through its _"installation process"_ again.

```hyperlambda-snippet
/*
 * Will re-initialize your server.
 * Warning, might take some time to finish.
 */
micro.evaluate.file:/startup.hl

/*
 * Notifying user.
 */
micro.windows.info:Done!
```
