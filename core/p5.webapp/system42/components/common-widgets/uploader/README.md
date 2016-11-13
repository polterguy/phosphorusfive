Drag and drop Ajax uploader
========

This folder contains the Ajax uploader widget of Phosphorus Five. The uploader widget, allows the user to drag and drop files
unto the surface of some part of his page, for then to automatically upload these files to the server.

Below is an example of how to use it.

```
create-widget:foo
  parent:content
  class:col-xs-12
  widgets
    literal:bar
      innerValue:Uploader example
      element:h1
    sys42.widgets.uploader
```

The above would produce something like the following.

![alt tag](screenshots/ajax-uploader-example-screenshot.png)

