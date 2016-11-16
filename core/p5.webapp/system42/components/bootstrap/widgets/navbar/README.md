An Ajax navbar widget
========

This folder contains the navbar Ajax widget, that allows you to create an Ajax menu, to allow your users to navigate your app. Even though the 
widget is 100% Ajax based, it still allows you to create a navigation menu, where the items are visible to search engines, and items are 
possible to bookmark.

To use it, simply add it to a container widget collection, through its widget creational Active Event *[sys42.widgets.navbar]*. Example below.

```
create-widget:foo
  widgets
    sys42.widgets.navbar:my-navbar

      // Settings the navbar into "SEO" mode.
      _crawl:true

      // These are the root menu items.
      _items
        Files

          // Any root item with its own [_items] collection, will
          // have children menu items of its own.
          _items
            Open:open
              .onclick
                sys42.windows.info-tip:You tried to open a file
            Close:close
              .onclick
                sys42.windows.info-tip:Closing file

            // If you add a menu item with the name of [_separator], it will
            // create a horizonat separator between your items.
            _separator
            Save:save
              .onclick
                sys42.windows.info-tip:And we're going to save ...
        Edit:edit
          .onclick
            sys42.windows.info-tip:Edit was clicked
        Windows
          _items
            First Window:first-window
              .onclick
                sys42.windows.info-tip:First window clicked
            Second Window:second-window
              .onclick
                sys42.windows.info-tip:Second window clicked
        View:view
          .onclick
            sys42.windows.info-tip:View was clicked
```

The above code will produce something like the following.

![alt tag](/core/p5.webapp/system42/components/bootstrap/widgets/navbar/screenshots/ajax-navbar-menu-example-screenshot.png)

The [_items] collection above is the most important argument, and declares your menu items in an hierarchically "Name"/"id" structure.
You can nest items, by having items contain their own [_items] collection, which will dropdown menus for you. The [.onclick] above, is
a lambda object of Hyperlambda, allowing you to do whatever you wish when users are clicking your items.

If you set the [_crawl] argument to "true", as we have done above, then an "unrolling" HTTP GET parameter will be automatically created for you,
to allow search engines to crawl your menu, and also users to bookmark them, by right clicking items, and choose "Open in new tab" for instance.
See the [tree widget](/../tree/) to understand how this work.

The menu is built on Bootstrap CSS' navbar, and will therefor render responsively, allowing devices with smaller resolution to show a more friendly
navigational widget. See [Bootstrap](http://getbootstrap.com/components/#navbar) to understand how this work. You can set most of the settings
from the Bootstrap Navbar throughchanging its *[_class]* property. For instance, to create a navbar that is attached to the bottom, instead of
fixed at the top, you could simply add the following *[_class]* properties to it when creating it.

```
create-widget:foo
  widgets
    sys42.widgets.navbar:my-navbar
      _class:navbar navbar-default navbar-fixed-bottom
      _crawl:true
      _items
        Files
          _items
            Open:open
              .onclick
                sys42.windows.info-tip:You tried to open a file
            Close:close
              .onclick
                sys42.windows.info-tip:Closing file
            _separator
            Save:save
              .onclick
                sys42.windows.info-tip:And we're going to save ...
        Edit:edit
          .onclick
            sys42.windows.info-tip:Edit was clicked
        Windows
          _items
            First Window:first-window
              .onclick
                sys42.windows.info-tip:First window clicked
            Second Window:second-window
              .onclick
                sys42.windows.info-tip:Second window clicked
        View:view
          .onclick
            sys42.windows.info-tip:View was clicked
```

## Responsive rendering

Below is a screenshot of how the first Ajax menu will look like on an iPhone 6.

![alt tag](/core/p5.webapp/system42/components/bootstrap/widgets/navbar/screenshots/ajax-navbar-menu-example-screenshot-responsive.png)

The menu will automatically "re-arrange" itself, and become a responsive popdown menu, if the screen resolution is smaller than some specific
threshold. This allows it to render on smaller devices, without forcing vertical scrolling.

