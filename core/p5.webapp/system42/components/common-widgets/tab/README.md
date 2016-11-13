An Ajax TabControl widget
========

This folder contains the Ajax TabControl widget in Phosphorus Five. This widget, allows you to create a tabbed view, with multiple tabs,
showing some aspecct of your page. Kind of like the TabControl in for instance windows. Below is an example.

```
create-widget:foo
  parent:content
  class:col-xs-8
  widgets
    sys42.widgets.tab
      _items
        First tab
          widgets
            literal
              innerValue:Foo bar
        Second tab
          widgets
            literal
              innerValue:Some other value
```

The above example, produces the following.

![alt tag](screenshots/ajax-tabcontrol-widget-example-screenshot.png)

As you switch the tab in the above TabControl widget, the tabview you switch to,becomes visible. Each node beneath the *[_items]* collection,
becomes one _"TabView"_. Each item inside of your *[_items]* collection, is expected to have its own set of *[widgets]* again. You can really put
any widget you wish into these, including complex widgets.

The TabControl has the following arguments.

* [_skin] - Skin to use, default value is "default".
* [_items] - TabView items, each having its own separated *[widgets]* collection.
* [_crawl] - If true, will allow for search engines, and crawlers, to "crawl" your TabControl.
* [_crawl-get-name] - HTTP GET parameter used when crawling items.

Notice, if you set the TabControl into _"crawlable mode"_, you must also provide a value for all you *[_items]*. This value becomes the
value of the HTTP GET parameter used when crawling your TabControl. Below is an example of a _"SEO friendly and crawlable TabControl"_.

```
create-widget:foo
  parent:content
  class:col-xs-8
  widgets
    sys42.widgets.tab
      _crawl:true
      _crawl-get-name:tab-view
      _items
        First tab:item-1
          widgets
            literal
              innerValue:Foo bar
        Second tab:item-2
          widgets
            literal
              innerValue:Some other value
```

Create a "lambda" page in your CMS, and paste in the code above, and then when you view your page, click the second tab, and choose "Open in new window".
If you do, you will see that it opens up with the URL being something like the following `http://localhost:1176/change-this-5?tab-view=item-2`.

This ensures that the TabControl is crawlable, since search engines, and other crawlers, will simply see your TabControl header buttons as hyperlinks.
While still preserving the Ajax functionality when a TabView slector is clicked.


