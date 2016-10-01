The System42 CMS
========

The CMS application is a pretty nifty Content Management System, allowing you to create and publish your own websites and pages.
In addition to being able to create HTML in a WYSIWYG environment, it also allows for you to create "lambda pages".

A lambda page, is a page that instead of passing raw HTML to the client, allows you to create Hyperlisp that is evaluated
on the server. Ususally, you would want to create some wort of Ajax Widget hierarchy when creating your lambda pages.

Below is an example of content you might want to put into your lambda page, that creates a simple "literal" widget for you,
which once clicked, changes its innerHTML to "Hello World!".

```
create-literal-widget
  element:h3
  parent:content
  class:col-xs-12
  innerValue:Click me!
  onclick
    set-widget-property:x:/../*/_event?value
      innerValue:Hello World!
```

To run the CMS, you need to be logged in as "root".



