The Bazar
========

This folder contains everything related to the Bazar. If you do not want to have your users access the Bazar, you can simply
delete the entire folder, at which point the entirety of the Bazar will be removed from the system. The Bazar will look like
the following on your system when you install a new app.

![alt screenshot](screenshots/screenshot-1.png)

The Bazar will only install apps that have been cryptographically signed with a PGP key, who's fingerprint can be found
in the _"/configuration/trusted-app-distributors.hl"_ file. Files you distribute in your Bazar, must hence be cryptographically
signed zip files, which you can do by for instance using the **[p5.mime.save]** Active Event.

A user can however add a new Bazar to his list of Bazars, which will add the Bazar's URL to the _"/configuration/bazars.hl"_ file,
which will traverse the bazar declaration file, for all of its trusted app distributors' fingerprints, and ask the user to
confirm that he wants to trust the fingerprint, to allow a new developer and/or distributor to install apps in his system.

You can also of course distribute P5 out of the box with your own fingerprint, and your own Bazar declaration(s). To create
your own Bazar file, see the _"/bazar/apps.hl"_ file, which is the default Bazar in P5. If you don't want your users to
be allowed to add new Bazars, you can do so simply by changing the _"web.config"_ appSetting called _"bazar.allow-users-adding-bazar"_.
This will disable the "add new Bazar button", and only allow the user to install applications from his current list of Bazars.

When the Bazar is started, it will retrieve all of the Bazar declaration files from their respective URLs, and list all available
apps, in all available Bazars, and allow the user to click a simple _"icon"_ to install your app. The contents of these files,
will be cached, to allow for the Bazar to more easily load on consecutive rounds. This allows you to create your own Bazar,
as long as you're able to somehow host your Bazar app declaration file, on some server, some where. Basically, a zero infrastructure
_"AppStore"_, allowing you to host your Bazar at e.g. GitHub, WordPress or Google Disc for that matter.

## Automatic PayPal integration

Each app you declare in your Bazar, can also optionally have a **[paypal-id]**, which if supplied, will lead your users
to PayPal when they try to install the app, instead of directly to your app's cryptographically signed zip file. This PayPal ID,
must be the ID of your PayPal button. Only after a user performs a valid purchase, the app's URL will be downloaded, and
automatically installed in your user's _"modules"_ folder.

You should clearly mark your apps in your _"install app icon"_, whether or not they're free/GPL licensed products, or
commercial/proprietary products. In addition, it would probably be considered polite, to also supply the price in your
Bazar app declaration file, such that users aren't tricked to doing a PayPal request, without realising they'll be asked
to purchase your product.

However, all in all, the Bazar is basically everything you need, more or less, to become a productive _"app developer"_, having your
own _"AppStore"_, distributing whatever types of apps you'd like to distribute for yourself. If you've created a really
cool app yourself, and you want me to distribute it for you, in my default Bazar declaration file, I am willing to do
so, as long as you obey by [my personal Bazar rules](https://gaiasoul.com/2017/08/16/bazar-rules-of-engagement/), which
admittedly are kind of _"Nazi"_, since I do not want to allow myself to compromise my user's systems in any ways. This might
give you additional downloads/purchases, since it'll be distributed in each upgrade/download of the main Phosphorus Five
core. However, you're also perfectly welcome to host your own Bazar, and distribute your own version of Phosphorus Five,
having a default Bazar declaration of your own.

If you create your own app, it's probably also considered polite, to allow your users to give you feedback, by either
providing a link to your support forum, and/or providing your email address, such that they can contact you, if they need help.

## The Bazar is simply an app itself

Notice, the Bazar is simply an app itself, which though comes pre-installed in the Phosphorus Five main download. This allows
me to upgrade it automatically, the same way I'd upgrade any apps automatically, that are hosted in my Bazar. This implies
that any missing features, which admittedly there are, will be added in future versions of the Bazar. Removing trusted app
distributors and/or Bazars comes to mind for instance. Search is another feature that comes to mind. Also more robust
downloading of apps, and/or Bazar declarations, are features that are on my TODO list.

I will also add more developer tools in the future, that allows you to automatically package your own cryptographically
signed apps, and further reduce the pain-level as you create your own apps. In addition to developer tools, allowing you
to create your apps entirely from within the Phosphorus Five main environment. A R.A.D. IDE for instance, is among some
of the features I am considering.

However, the reasons why I am saying this, is because that's possibly the coolest feature of the Bazar, allowing users
to automatically upgrade their system, with never versions of their tools, adding to their existing environment,
even after I have distributed the main P5 core myself!

For an example of how to create an app, please look at _"Sephia Five"_, which is to be considered the _"reference implementation"_
for a Bazar type of app. Sephia Five can be automatically downloaded and installed from your Bazar.
