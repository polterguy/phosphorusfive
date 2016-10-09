The System42 CMS
========

The CMS application is a pretty nifty Content Management System, allowing you to create and publish your own websites and pages.
In addition to being able to create HTML in a WYSIWYG environment, it also allows for you to create "lambda pages".

A lambda page, is a page that instead of passing raw HTML to the client, allows you to create Hyperlambda that is evaluated
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

When you create a page, it is stored in the [p5.data](/plugins/p5.data/) database as a *[p5.page]* object.

You can set several properties to your pages, such as their virtual URL, Name, which template to use, and so on. By default, System42's 
CMS comes with a handful of pre-defined templates, allowing you to create pages with a navbar navigation menu, a completely "empty" 
page, etc.

The CMS of System42 is also a reference/example implementation of a website/webapp created with Phosphorus Five.

## Folder structure

Below is the description of the main folders within System42, and what they do.

### The "initial-pages" folder

This folder contains the initially created pages for your system. If you wish to distribute P5 and System42 with different initial
pages, feel free to modify this as you see fit.

By default, only a single HTML page, in addition to the page necessary to start "apps" are created.

### The "installation" folder

This folder contains the files that will be evaluated during the initial setup of your server, and guides you through setting up a
server salt, root password, and other things necessary to "initialize" your server.

### The "media" folder

This folder contains CSS and JavaScript files, that System42 is dependent upon. System42 is built upon Bootstrap CSS, and uses
jQuery some places. The files for these libraries can be found in the media folder.

### The "page-editor" folder

This folder contains the actual CMS editor, and all of the plugins for the different types of pages that are in the system.
By default, there are only two types of pages in System42; HTML pages and "lambda" pages. Lambda pages allows the user to instead
of loading up some static HTML, evaluate his own lambda script, to entirely create the page himself as he see fit. 

It still utilizes the templates, URL system, name, and so on. Unless you explicitly override these in your lambda page. You would
probably want to take advantage of these features, if you create your own lambda pages, which means making sure whatever widgets
you load into your page, using lambda scripts, should be loaded within the "container" widget - Which is the default "root widget"
for your CMS.

If you wish to create your own page type, you'll need to add up a "specialized editor", "new page template", in addition to creating
your own "loader logic" in a script within the "page-loader" folder.

### The "page-loader" folder

This folder, contains all the specialized "loaders" for your pages. See above the documentation for the "page-editor" folder to
understand how it works.

### The "page-templates" folder

This folder contains the "skins" for your pages, or "templates" for your page objects. Whenever you create a new page in the CMS,
you can choose which "template" to use. Some templates have menus, some black, others white, others are completely empty, etc.
Exactly what templates your system has, dependes upon your installation, and how it is modified.

### The "startup" folder

All files inside of this folder, are automatically evaluated during startup of your server, and typically creates Active Events, 
and does other types of initialization the system is dependent upon to function accurately.

The most important file in this folder is the "p5.web.load-ui.hl". This file creates an Active Event, which is raised by the
P5 core, and expected to serve documents according to URLs. It is given a single argument *[_url]*, which is the relative URL to the
page requested by the client.

If you wish to create your own system, entirely deleting System42, you need to make sure you have the *[p5.web.load-ui]* event
yourself, somehow.


