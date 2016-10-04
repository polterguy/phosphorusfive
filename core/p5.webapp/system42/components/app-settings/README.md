System 42 settings events
===============

This directory contains Hyperlambda that creates "application specific settings" Active Events.
There are three basic Active Events defined in this directory.

* [sys42.get-app-setting]
* [sys42.set-app-setting]
* [sys42.list-app-settings]

All three Active Events must be given an application name through [_app]. This
must be a string literal that is unique for your app.

All three Active Events can also optionally take a [_username] argument, which if provided,
will return the settings for the specified username, instead of the currently logged in
user. Notice, if you provide an explicit [_username], you must be logged in as root.
Otherwise, the Active Event(s) will throw an exception.

## [sys42.get-app-setting]

Returns the setting named in [_arg] to the caller. The following code for instance

```
sys42.get-app-setting:my-setting-key
  _app:my-app
```

Would return something like this

```
sys42.get-app-setting
  my-setting-key:my-setting-value
```

Active Event can also return "composite setting values", where the setting instead of
being a single value, could be a complex graph object (node structure).

## [sys42.set-app-setting]

Works similar to [sys42.get-app-setting], except it updates or creates a setting value
for the given [_app] and the key given through [_arg]. The new value of your setting,
is provided as [_src], which might either contain a single value, or be a complex graph
object (node hierarchy).

```
sys42.set-app-setting:my-setting-key
  _app:my-app
  _src:Some value for key
```

## [sys42.list-app-settings]

Works similar to [sys42.get-app-setting], except it doesn't take an [_arg] argument, and 
will return _all_ settings for the specified application.

```
sys42.list-app-settings
  _app:some-app
```

These Active Events combined, creates a nifty shortcut for storing application specific settings and data.
Internally it relies upon [pd.data](/plugins/p5.data/), which means it is _not_ a general storage for 
_all_ data in your application, since p5.data is a memory based database storage. However, for the smaller
settings you've got in your app, these Active Events provides an excellent shortcut for you to store "settings".



