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
    container
      class:col-xs-8
      widgets
        sys42.widgets.uploader
          _onupload
            sleep:5000
            sys42.windows.info-tip:File '{0}' uploaded
              :x:/../*/_filename?value
```

The above would produce something like the following, when a file is dragged onto your page.

![alt tag](screenshots/ajax-uploader-example-screenshot.png)

