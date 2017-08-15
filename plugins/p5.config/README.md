Configuration settings in P5
===============

This project allows you to access configuration settings, from either app.config, or web.config, from Hyperlambda.

It consists of two Active Events, the first being; *[p5.config.get]* - Which has a protected alias; *[.p5.config.get]*, that
allows you to access configuration settings, that are "protected", meaning starting with an underscore "_", or a period ".".

If you try to access settings starting with an underscore or a period from p5.lambda, an exception will be thrown. This allows
you to create "protected" settings, which are not possible to access from p5.lambda, which for security reasons never should be
exposed to a programming language of the "dynamic nature" as p5.lambda is. Examples are database connections, passwords, etc ...

The above is a general pattern for all "collection type of Active Events, such as *[p5.web.session.get]* from [p5.web](/plugins/p5.web/).

## Listing all configuration settings

In addition to the above Active Event, there is also an event that will list all configuration settings for you called; *[p5.config.list]*.
Also this Active Event has a "protected" alias, which will not list settings starting with "_" or ".".

## Example

```
p5.config.get:some-appSetting-config-key
```

The above will retrieve the _"some-appSetting-config-key"_ configuration setting from your web.config.
