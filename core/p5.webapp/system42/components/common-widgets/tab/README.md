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

