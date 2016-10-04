Accessing your app's configuration settings
===============

This project allows you to access configuration settings, from either app.config, or web.config, from p5.lambda.

It consists of one single Active Event; *[get-config-setting]* - Which has a protected alias; *[.get-config-setting]* which 
allows you to access configuration settings that starts with an underscore "_", or a period ".".

If you try to access settings starting with an underscore or a period from p5.lambda, an exception will be thrown. This allows
you to create "protected" settings, which are not possible to access from p5.lambda, which for security reasons never should be
exposed to a programming language of the "dynamic nature" as p5.lambda is. Examples are database connections, passwords, etc ...

## Listing all configuration settings

In addition to the above Active Event, there is also an event that will list all configuration settings for you called; *[list-config-settings]*.
Also this Active Event has a "protected" alias, which will not list settings starting with "_" or ".".


