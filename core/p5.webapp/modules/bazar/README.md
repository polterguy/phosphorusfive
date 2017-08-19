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
