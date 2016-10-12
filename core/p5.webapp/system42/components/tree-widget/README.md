An SEO Ajax tree widget
========

This folder contains an Ajax extension tree widget, that can be traversed by search engines, and even allows you to bookmark
items in it, by for instance by right clicking and item, and choose "Open in new tab".

To use it, simply add it to a container widget collection, through its widget creational Active Event [sys42.widgets.tree]. Example below.

```
create-container-widget
  parent:content
  class:col-xs-12
  widgets
    sys42.widgets.tree
      _crawl:true
      _crawl-get-name:folder
      _items
        root:/
      _on-get-items
        list-folders:x:/../*/_item-id?value
        for-each:x:/-/*?name
          split:x:/./*/_dp?value
            =:/
          add:x:/../*/return/*
            src:@"{0}:{1}"
              :x:/..for-each/*/split/0/-?name
              :x:/..for-each/*/_dp?value
        return
          _items
```

The above example allows you to traverse your folders in your Phosphorus Five installation, using the Ajax tree widget.

Notice if you right click an item, and choose "Open in new tab" for instance, then the URL opened, will "unroll" the tree view,
to whatever item you choose. This is due to the [_crawl] parameter being set to true. This works by adding an HTTP GET parameter to
the current URL, which contains the IDs of every single item, separated by "|", necessary to toggle, to open up the tree, to the point
you requested. This is done on the server, and hence makes the tree view very "SEO friendly". In addition to allowing your users to
bookmark items. If you wish to override the name of the HTTP GET parameter used for this operation, you can do so with 
the [_crawl-get-name] argument while creating your tree.

The [_items] collection, is the "initial items" for it, when the control is created. If wish, you can supply an initial set of [_items],
that have children items themselves, such as this example shows you.

```
create-container-widget
  parent:content
  class:col-xs-12
  widgets
    sys42.widgets.tree
      _crawl:true
      _crawl-get-name:folder
      _items
        Foo:foo
          _items
            Foo 1:foo-1
            Foo 2:foo-2
        Bar:bar
          _items
            Bar 1:bar-1
              _items
                Howdy World:tjobing-1
            Bar 2:bar-2
```

As the above example also illustrates, you can also have multiple "root items". There must however be at least _one_ item, otherwise the
widget will throw an exception during initialization.

You can also override the CSS class(es) used for specific items, both their "open state" and their "closed state" classes. Below is an 
example of this.

```
create-container-widget
  parent:content
  class:col-xs-12
  widgets
    sys42.widgets.tree
      _crawl:true
      _crawl-get-name:folder
      _items
        Foo:foo
          _class-close:glyphicon glyphicon-wrench
          _class-open:glyphicon glyphicon-cog
          _items
            Foo 1:foo-1
              _class-close:glyphicon glyphicon-time
            Foo 2:foo-2
              _class-close:glyphicon glyphicon-headphones
        Bar:bar
          _class-close:glyphicon glyphicon-trash
          _class-open:glyphicon glyphicon-home
          _items
            Bar 1:bar-1
              _class-close:glyphicon glyphicon-picture
              _class-open:glyphicon glyphicon-edit
              _items
                Howdy World:tjobing-1
                  _class-open:glyphicon glyphicon-picture
            Bar 2:bar-2
              _class-close:glyphicon glyphicon-fast-backward
```

The default icons used if no CSS class is explicitly added is "glyphicon glyphicon-folder-close" and "folder-open" from Bootstrap CSS.

The widget is not dependent upon [Bootstrap CSS](http://getbootstrap.com/components/) in any ways, but will use the glyphicon classes as
the default classes for icons, unless overridden. This means you'll need to include the Bootstrap component on your page, unless you override
the [_class-close] and [_class-open] values of each item in your [_items] collection.

To see how to do this, check out the documentation for the [Bootstrap CSS](/core/p5.webapp/system42/components/bootstrap/) module.

Notice, all arguments are optional, except the *[_items]* collection, that must have, at the very least, minimum _one_ item. Making the
smallest possible code to use the Tree view look something like this.

```
create-container-widget
  widgets
    sys42.widgets.tree
      _items
        Foo:foo
```

Of course, unless you supply an *[_on-get-items]* callback lambda, then no new items can possibly be appended to your tree view, and it will
only show the items you initially feed it with. If you wish to being able to dynamically feed your widget with new items, you must add
the *[_on-get-items]* lambda callback, where you are expected to return an *[_items]* collection, resembling the *[_items]* collection you
initially gave it when the widget was created.

You can also toggle multiple items at the same time, by returning a nested *[_items]* collection, as the following demonstrates.

```
create-container-widget
  parent:content
  widgets
    sys42.widgets.tree
      _items
        Root:root
      _on-get-items
        if:x:/../*/_item-id?value
          =:root
          return
            _items
              Foo 1:foo-1
                _items
                  Foo 1's child 1:foo-1-1
                  Foo 1's child 2:foo-1-2
                  Foo 1's child 3:foo-1-3
              Bar 1:bar-1
                _items
                  Bar 1's child 1:bar-1-1
                  Bar 1's child 2:bar-1-2
```

## Styling your Ajax tree widget

If you wish, you can explicitly specify a CSS file to use with your widget, which is used as your widget's "skin CSS file". The default
value for this, is "tree.css", which can be found in the "/media/" folder of the component, but this can be overridden by you, 
by supplying a *[_css-file]* property, leading to your custom CSS file.

You can also override the CSS class used for the root widget. To use another CSS class, pass in the CSS class you wish for your widget 
to use as a *[_class]* argument. The default value here is "tree-widget", which is the class found in the "tree.css" file.

If you wish to create your own style/skin, you should create another CSS class, instead of just changing the values of your CSS file(s).
This is due to that you might want to include multiple tree widgets on the same page, having different styles.

A good way to start, is to copy the "tree.css" file, change the root CSS class of your widget through the *[_class]* property,
include your custom CSS file through *[_css-file]*, and do a find and replace through your custom CSS file for ".tree-widget", and rename 
it to something else, before you start actually modifying the class content of your custom file.

All CSS classes in the default skin, is prepended with ".tree-widget", which means once you start creating your own class for the widget,
then none of the pre-existing classes will be "in your way" in any ways.

Besides from this file, the widget is not dependent upon any CSS files in any ways, except of the default icons for closed and opened
icon items, which are taken from the "glyphicons" from Bootstrap CSS. If you wish to use other glyphicons, you can see the entire list
of available icons at the [Bootstrap CSS website](http://getbootstrap.com/components/). You can of course use your own icons, independently
of the glyphicons from Bootstrap, at which case you no longer need to include Bootstrap.

If you use the glyphicons, you are yourself responsible for making sure you include Bootstrap CSS, which can be done, by reading the 
documentation for the System42 [Bootstrap CSS module](/core/p5.webapp/system42/components/bootstrap/). You only need to include the CSS file
though, and not any of the JavaScript files.

## Bandwidth usage

The Ajax tree view control/widget actually does not itself use any JavaScript, besides the core JavaScript from p5.ajax, which in its
minified and GZipped version, is roughly 5KB of JavaScript. In addition, it uses only three tiny images by default, and one tiny CSS
file. So in its absolutely minimum version, without Bootstrap included, the entire download for your clients, is less than 10KB for the
initial loading. And as you expand items, it loads an addition ~1KB.

Everything is transferred from the server as JSON (which is the default behavior of [p5.ajax](/core/p5.ajax/)), and the bandwidth usage for
expanding two items with 3 and 2 children items each, becomes 1.2KB of JSON transferred from your server.


