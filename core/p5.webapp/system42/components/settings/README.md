Application settings helper Active Events
===============

This directory contains Hyperlambda files that creates application specific settings Active Events. These
Active Events, gives you easily access to store settings for your own System42 apps and/or components, that will persist in 
the [p5.data](/plugins/p5.data/) database. There are three basic Active Events defined in this directory.

* [sys42.settings.get] - Returns a setting.
* [sys42.settings.set] - Creates or changes an existing setting.
* [sys42.settings.list] - List all settings, and their values.

All 3 Active Events must be given an application name as *[_app]*. This must be a string literal that is 
unique for your app, and serves as a _"namespace"_ for your app. Examples of a good namespace is _"my-company.my-product"_.

All 3 Active Events can also optionally take a *[_username]* argument, which if provided,
will return the settings for the specified username, instead of the currently logged in
user. Notice, if you provide an explicit *[_username]*, you must be logged in as root, otherwise an exception will be thrown.

## [sys42.settings.get]

Returns the [_arg] setting to caller. Example code below.

```
sys42.settings.get:my-setting-key
  app:my-app
```

The above invocation, would return something similar to the following, assuming you have a setting for _"my-app"_ called _"my-setting-key"_.

```
sys42.settings.get
  my-setting-key:my-setting-value
```

The Active Event can also return composite setting values, where the setting instead of being a single value, 
could be a hierarchical lambda object by itself.

## [sys42.settings.set]

Works similar to *[sys42.settings.get]*, except it updates an existing, or creates a new setting value
for the given *[_app]*, and the key given through *[_arg]*. The new value of your setting, is provided as *[_src]*,
which might either contain a single value, or a hierarchical lambda object.

```
sys42.settings.set:my-setting-key
  app:my-app
  src:Some value for key
```

To create more complex settings, which are graph/lambda objects by themselves, you could do something such as the following.

```
sys42.settings.set:my-setting-key
  app:my-app
  src
    name:John
    address
      adr1:Foo Bar st. 23
      state:CA
```

Then to retrieve the above setting, you could do something such as the following.

```
sys42.settings.get:my-setting-key
  app:my-app
```

Which would return the above created lambda object setting.

## [sys42.settings.list]

Works similar to *[sys42.settings.get]*, except it doesn't take any *[_arg]* arguments, and will return _all_ 
settings for the specified application, and their values.

```
sys42.settings.list
  app:some-app
```

These Active Events combined, creates a nifty shortcut for storing application specific settings and data.
Internally it relies upon [pd.data](/plugins/p5.data/), which means it is _not_ a general storage for 
_all_ data in your application, since p5.data is a memory based database storage. However, for the settings you've got in your app, 
these Active Events provides an excellent shortcut for you.



