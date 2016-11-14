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
          .onupload

            // Sleeping the current thread, just 
            // to make sure user can see animation.
            sleep:5000
            sys42.windows.info-tip:File '{0}' uploaded
              :x:/../*/_filename?value
```

The above would produce something like the following.

![alt tag](screenshots/ajax-uploader-example-screenshot.png)

When a file is dropped unto the above uploader or dropzone, then a nice animation will give visual clues to the user, that the file is in the
process of being uploaded.

The uploader has the following arguments.

* [_css-file] - A CSS file to include when widget is displayed. Defaults to "uploader.min.css".
* [_filter] - Filter for file types to accept.
* [_allow-multiple] - If true, multiple files can be uploaded at the same time.
* [_class] - Default CSS class to use. Defaults to "uploader-widget".
* [_dragover-class] - CSS class to add when files are dragged unto the surface of the uploader. Defaults to "uploader-widget-dragover".
* [_drop-class] - CSS class to use when a file is dropped in the widget. Defaults to "uploader-widget-drop".
* [.onupload] - Lambda callback to invoke when a file has been uploaded.

The default CSS file, contains some confuration classes, which allows you to control the positioning of your uploader widget. These are listed below.

* "uploader-footer" - Makes sure the uploader is absolutely positioned at the bottom of your screen, in a fixed position.
* "uploader-faded" - Makes your widget semi-transparent, to not be so prominents on your form.
* "uploader-large" - Makes your widget become 300px tall.
* "uploader-small" - Makes your widget becomes smaller, 60px tall.
* "uploader-full-screen" - Makes your widget fill the entire screen, though behind your other widgets.

If you use "uploader-full-screen" and "uploader-faded", in addition to of course, the default CSS class of "uploader-widget", then your uploader
will look like the following.

![alt tag](screenshots/ajax-uploader-example-screenshot-fullscreen.png)

In the above example, which has a *[_class]* of "uploader-widget uploader-full-screen uploader-faded", your entire page will become a "dropzone".

## Receiving your files on your server

Your *[.onupload]* lambda callback, will be invoked with the following arguments.

* [_count] - Total number of files in current upload.
* [_current] - Current file number [0 .. _count>
* [_filename] - Filename as supplied by client.
* [_content] - Content of file.


