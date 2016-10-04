System42 CSS helper Active Events
===============

This directory contains Hyperlambda that creates "CSS helper" Active Events.
These events, allows you to add, remove or toggle a CSS class to one or more widgets.
The events are listed below.

* [sys42.add-css-classes] - Adds one or more CSS classes to one or more widgets, unless the class(es) exists from before
* [sys42.delete-css-classes] - Deletes one or more CSS classes from one or more widgets
* [sys42.toggle-css-classes] - "Toggles" one or more CSS classes from one or more widgets

Example code given below.

```
create-widget:my-widget
  parent:content
  position:0
  style:"width:400px;height:150px;border:solid 1px black;"
  widgets
    button
      class:btn btn-default
      innerValue:Add red
      onclick
        sys42.add-css-classes:my-widget
          _class:bg-danger
    button
      class:btn btn-default
      innerValue:Delete red
      onclick
        sys42.delete-css-classes:my-widget
          _class:bg-danger
    button
      class:btn btn-default
      innerValue:Toggle red
      onclick
        sys42.toggle-css-classes:my-widget
          _class:bg-danger
```

Notice, all Active Events can take either a constant, or an expression leading to multiple IDs for existing widgets.
All Active Events can also take a list of classes, either separated by space " ", or comma (,).

