CSS helper Active Events
===============

This directory contains Hyperlambda that creates some CSS helper Active Events.
These events, allows you to add, remove or toggle a CSS class to one or more widgets.
The events created are listed below.

* [sys42.utilities.add-css-classes] - Adds one or more CSS classes to one or more widgets, unless the class(es) exists from before
* [sys42.utilities.delete-css-classes] - Deletes one or more CSS classes from one or more widgets
* [sys42.utilities.toggle-css-classes] - "Toggles" one or more CSS classes from one or more widgets

Example code given below.

```
p5.web.widgets.create-container:my-widget
  parent:content
  position:0
  style:"width:400px;height:150px;border:solid 1px black;"
  widgets
    button
      class:btn btn-default
      innerValue:Add red
      onclick
        sys42.utilities.add-css-classes:my-widget
          class:bg-danger
    button
      class:btn btn-default
      innerValue:Delete red
      onclick
        sys42.utilities.delete-css-classes:my-widget
          class:bg-danger
    button
      class:btn btn-default
      innerValue:Toggle red
      onclick
        sys42.utilities.toggle-css-classes:my-widget
          class:bg-danger
```

Notice, all Active Events can take either a constant, or an expression leading to multiple IDs for existing widgets.
All Active Events can also take a list of classes, either separated by space " ", or comma (,).

