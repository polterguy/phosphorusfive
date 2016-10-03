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

When you create a page, it is stored in the [p5.data](/plugins/extras/p5.data/) database as a *[p5.page]* object.

You can set several properties to your pages, such as their virtual URL, Name, which template to use, and so on. By default, System42's CMS
comes with a handful of pre-defined templates, allowing you to create pages with a navbar navigation menu, a completely "empty" page, etc.

The CMS of System42 is also a reference/example implementation of a website/webapp created with Phosphorus Five.


