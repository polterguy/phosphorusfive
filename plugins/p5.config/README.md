Configuration settings in P5
===============

This project allows you to access configuration settings, from either your app.config file, or your web.config file, 
through Active Events. It consists of two Active Events, the first being; **[p5.config.get]** - Which has a protected 
alias; **[.p5.config.get]**, that allows you to access configuration settings, that are "protected", meaning starting 
with an underscore "_", or a period ".".

## About "protected" events

If you try to access settings starting with an underscore or a period from Hyperlambda, an exception will be thrown. This allows
you to create _"protected"_ settings, which are not possible to access from Hyperlambda, which for security reasons never should be
exposed to a programming language of the _"dynamic nature"_ of Hyperlambda. Examples are database connections, passwords, etc ...

The above is a general pattern for all "collection type of Active Events, such as **[p5.web.session.get]** from [p5.web](/plugins/p5.web/).
To create a _"protected"_ setting, make sure you start its key with a ".".

## Listing all configuration settings

In addition to the above Active Event, there is also an event that will list all configuration settings for you called; **[p5.config.list]**.
Also this Active Event has a "protected" alias, which will not list settings starting with "_" or ".".

## Example

```
p5.config.get:some-appSetting-config-key
```

The above will retrieve the _"some-appSetting-config-key"_ configuration setting from your web.config. The next one will
list all your settings, and then retrieve the value of your first key.

```
p5.config.list
p5.config.get:x:/-/0?name
```

