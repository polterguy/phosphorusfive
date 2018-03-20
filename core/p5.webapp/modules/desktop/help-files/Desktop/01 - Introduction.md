## Introduction

The _"Desktop"_ module, is normally the root of your Phosphorus Five installation, and responsible for URL resolving,
in addition to providing other services to your modules, and Phosphorus Five in general. Also the Desktop module
is completely loosely coupled from all other modules, and the core itself, and can actually be exchanged with your
own logic, if you wish. It looks like the following, depending upon which apps you've installed, and whether
or not you're logged into Phosphorus Five or not.

https://phosphorusfive.files.wordpress.com/2018/03/desktop-screenshot.png

As you can see above, the Desktop module will create one _"desktop launcher icon"_ for each app you've
installed in your installation. This launcher icon is actually dynamically created from your apps'
_"desktop.hl"_ file, and can to a large extent be modified as you see fit. See other parts of this
documentation for how to create your own desktop icon.

### Installing and uninstalling modules

To install a new module, simply click the _"+"_ button in your toolbar, on your Desktop, while you're
logged in as _"root"_. This will allow you to either browse to a zip file on your local computer -
Or use an URL to some zip file on the web, which will be automatically downloaded and installed for
you. Below is a screenshot of the installation process.

https://phosphorusfive.files.wordpress.com/2018/03/desktop-install-new-module-screenshot.png

Uninstallation is equally easy, simply click the _"trashcan"_ button, at which point Phosphorus will
display all your installed modules, and allow you to uninstall these. Below is a screenshot.

https://phosphorusfive.files.wordpress.com/2018/03/desktop-uninstall-module.png

### Settings and skinning your Desktop

The desktop module allows you to change the skin for your Phosphorus Five installation, which is a global
setting, used for all (most) modules you install. You can access your settings, and change your skin,
by clicking the _"cog"_ icon at the top right corner while you have your desktop module loaded. Below
is a screenshot of how this will look.

https://phosphorusfive.files.wordpress.com/2018/03/desktop-settings-screenshot.png

There is a whole range of skins in Phosphorus Five, that is distributed out of the box, that
you can choose from. All of these skins are created such that they are rendered _"responsively"_,
which implies that they should perfectly well work, also on your phone or your tablet, in addition
to your main laptop or desktop computer.

It is also very easy to create your own skin, due to some highly intuitive CSS tricks applied
by Phosphorus Five, or the core Micro CSS framework to be more accurate. We will look at how to
create our own skins in the documentation for _"Micro"_ later in these documentation files. Below
is how Hyper IDE will look like, if you choose the _"Magic Forrest"_ skin.

https://phosphorusfive.files.wordpress.com/2018/03/hyper-ide-magic-forrest-skin-screenshot.png

Below you can see how Camphora Five will look like if you choose the _"Aztec"_ skin.

https://phosphorusfive.files.wordpress.com/2018/03/camphora-screenshot-aztec.png
