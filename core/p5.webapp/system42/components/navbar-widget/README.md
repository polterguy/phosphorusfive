An SEO Ajax tree widget
========

This folder contains an Ajax extension tree widget, that can be traversed by search engines, and even allows you to bookmark
items in it, by for instance by right clicking and item, and choose "Open in new tab".

To use it, simply add it to a container widget collection, through its widget creational Active Event [sys42.widgets.tree]. Example below.

```
create-container-widget
  parent:content
  widgets
    sys42.widgets.tree
      _crawl:true
      _items
        root:/
      _on-get-items
        list-folders:x:/../*/_item-id?value
        for-each:x:/-/*?name
          list-folders:x:/./*/_dp?value
          split:x:/./*/_dp?value
            =:/
          add:x:/../*/return/*
            src:@"{0}:{1}"
              :x:/..for-each/*/split/0/-?name
              :x:/..for-each/*/_dp?value
          if:x:/./*/list-folders/*
            not
            add:x:/../*/return/*/_items/0/-
              src
                _class:tree-leaf
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

You can also add your own item root CSS class for your items. In the above example, we add the class "tree-leaf", which removes the
expand icon on items, through the [_class] argument on each item inside of [_items]. In our next example we will see this more accurately, using
a bunch of statically loaded items.

The [_items] collection, is the "initial items" for it, when the control is created. If wish, you can supply an initial set of [_items],
that have children items themselves, such as this example shows you.

```
create-container-widget
  parent:content
  class:col-xs-12
  widgets
    sys42.widgets.tree
      _items
        Foo:foo
          _items
            Foo 1:foo-1
              _class:tree-leaf
            Foo 2:foo-2
              _class:tree-leaf
        Bar:bar
          _items
            Bar 1:bar-1
              _items
                Howdy World:tjobing-1
                  _class:tree-leaf
            Bar 2:bar-2
              _class:tree-leaf
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
      _items
        Foo:foo
          _class-close:glyphicon glyphicon-wrench
          _class-open:glyphicon glyphicon-cog
          _items
            Foo 1:foo-1
              _class-close:glyphicon glyphicon-time
              _class:tree-leaf
            Foo 2:foo-2
              _class-close:glyphicon glyphicon-headphones
              _class:tree-leaf
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
                  _class:tree-leaf
            Bar 2:bar-2
              _class-close:glyphicon glyphicon-fast-backward
              _class:tree-leaf
```

The default icons used if no CSS class is explicitly added is "glyphicon glyphicon-folder-close" and "glyphicon glyphicon-folder-open" from 
Bootstrap CSS.

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
                    _class:tree-leaf
                  Foo 1's child 2:foo-1-2
                    _class:tree-leaf
                  Foo 1's child 3:foo-1-3
                    _class:tree-leaf
              Bar 1:bar-1
                _items
                  Bar 1's child 1:bar-1-1
                    _class:tree-leaf
                  Bar 1's child 2:bar-1-2
                    _class:tree-leaf
```

## Handling selected event

If you wish, you can supply an *[_on-select]* lambda callback, which will be invoked when the user is selecting items in your tree.
Your lambda callback will be given a collection of *[_items]*, where the name property of the node, is the ID of the item selected.

Below is an example of a tree widget that simply shows an "info tip box" as the user selects items.

```
create-container-widget
  parent:content
  class:col-xs-12
  widgets
    sys42.widgets.tree
      _on-select
        sys42.windows.info-tip:You selected '{0}'
          :x:/../*/_items/0?name
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

## Selecting multiple items

Although there is no user interface for the widget by default, to allow for the user to select multiple items, this is still possible to
achieve using its API. The *[sys42.widgets.tree.select-items]* lambda widget event, allows you to select multiple items. Below is an example.
Click the button to select both the "foo-1" and the "bar-2" item.

```
create-container-widget
  parent:content
  class:col-xs-12
  widgets
    sys42.widgets.tree:my-tree
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
    literal
      class:btn btn-default
      innerValue:Select two items
      onclick
        sys42.widgets.tree.select-items:my-tree
          _items
            foo-1
            bar-2
```

Hint, you can also, by using its API, "unroll" items, or "toggle" items, by invoking the *[sys42.widgets.tree.toggle-items]* Active Event.
Which takes the exact same set of parameters as the *[sys42.widgets.tree.select-items]* event. Below is an example of toggling (collapsing)
two items in your tree, through clicking a button.

```
create-container-widget
  parent:content
  class:col-xs-12
  widgets
    sys42.widgets.tree:my-tree
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
    literal
      class:btn btn-default
      innerValue:Toggle two items
      onclick
        sys42.widgets.tree.toggle-items:my-tree
          _items
            foo
            bar-1
```

The *[sys42.widgets.tree.toggle-items]* Active Event, optionally take a *[_force-expand]* argument, which if set to true, will not collapse
items that are already expanded, but exclusively open already collapsed items.

## Retrieving currently selected items

The tree widget also supports retrieving currently selected items, through its *[sys42.widgets.tree.get-selected-items]* Active Event.
Below is an example of usage. Select any item in your tree, and then click the button.

```
create-container-widget
  parent:content
  class:col-xs-12
  widgets
    sys42.widgets.tree:my-tree
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
    literal
      class:btn btn-default
      innerValue:Get selected item(s)
      onclick
        sys42.widgets.tree.get-selected-items:my-tree
        sys42.windows.info-tip:You selected '{0}'
          :x:/../*/sys42.widgets.tree.get-selected-items/0/0?name
```

Notice, the *[sys42.widgets.tree.get-selected-items]* event, might return also 0 or multiple items, depending upon how many items
user has selected, and if you have somehow selected multiple items through its API or something similar.

## Styling your Ajax tree widget

You can also override the skin used for the TreeView widget. To use another skin, pass in the skin you wish to use as a *[_skin]* argument. 
The default value here is "default", which is the CSS file found in the "default/default.css" file.

Besides from this file, the widget is not dependent upon any CSS files in any ways, except of the default icons for closed and opened
icon items, which are taken from the "glyphicons" from Bootstrap CSS. If you wish to use other glyphicons, you can see the entire list
of available icons at the [Bootstrap CSS website](http://getbootstrap.com/components/). You can of course use your own icons, independently
of the glyphicons from Bootstrap, at which case you no longer need to include Bootstrap.

If you use the glyphicons, you are yourself responsible for making sure you include Bootstrap CSS, which can be done, by reading the 
documentation for the System42 [Bootstrap CSS module](/core/p5.webapp/system42/components/bootstrap/). You only need to include the CSS file
though, and not any of the JavaScript files.

Below is the exact same example we started out with, except with a different skin, which creates a larger TreeView, more suitable for
smartphones and smaller devices.

```
create-container-widget
  parent:content
  widgets
    sys42.widgets.tree
      _skin:large
      _items
        root:/
      _on-get-items
        list-folders:x:/../*/_item-id?value
        for-each:x:/-/*?name
          list-folders:x:/./*/_dp?value
          split:x:/./*/_dp?value
            =:/
          add:x:/../*/return/*
            src:@"{0}:{1}"
              :x:/..for-each/*/split/0/-?name
              :x:/..for-each/*/_dp?value
          if:x:/./*/list-folders/*
            not
            add:x:/../*/return/*/_items/0/-
              src
                _class:tree-leaf
        return
          _items
```

## Bandwidth usage

The Ajax TreeView widget actually does not itself use any custom JavaScript, besides the core JavaScript from p5.ajax, which in its
minified and GZipped version, is roughly 5KB of JavaScript. In addition, it uses only three tiny images by default, and one tiny CSS
file. So in its absolutely minimum version, without Bootstrap included, the entire download for your clients, is less than 10KB for the
initial loading. And as you expand items, it loads an addition ~1KB for each Ajax request, depending upon how many items you fetch.

Everything is transferred from the server as JSON (which is the default behavior of [p5.ajax](/core/p5.ajax/)), and the bandwidth usage for
expanding two items with 3 and 2 children items each, becomes 1.2KB of JSON transferred from your server.

## Server resource consumption

The widget will only request new items when an item is initially expanded through your supplied *[_on-get-items]* lambda callback. On consecutive
expansions for the same items, it will simply remove a "hide" CSS class on the client, never invoking your get items lambda callback.

This means it is also very cheap in regards to server resource usage, if the user is expanding and hiding the same items, looking for 
some specific node, in your tree.




