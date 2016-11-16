The Bootstrap Ajax Colorpicker widget
========

This folder contains an Ajax colorpicker, built upon the [Bootstrap colorpicker](https://itsjavi.com/bootstrap-colorpicker/), that 
allows your users to pick a color. Below is an example of how to use it.

```
create-widget
  parent:content
  class:col-xs-4
  widgets

    // Changes bg-color when colorpicker value changes.
    literal:output-widget
      class:prepend-bottom
      innerValue:Here comes color!

    // Actual colorpicker.
    sys42.widgets.colorpicker:my-color-picker
      _value:#ff00ff
      _label:My color
      .onchange
        set-widget-property:output-widget
          style:"background-color:{0};"
            :x:/../*/_value?value
```

The above code will create something like the following.

![alt tag](/core/p5.webapp/system42/components/bootstrap/widgets/colorpicker/screenshots/colorpicker-example-screenshot.png)


You can pass in the following arguments to the colorpicker.

* [_value] - Initial value, can be either '#xxyyzz', 'rgba(x,y,z,q)' or named color (e.g. 'yellow').
* [.onchange] - Lambda callback evaluated when value changes. [_value] and [_event] is passed into it.
* [_label] - An optional descriptive label for your colorpicker.
* [_class] - Optional CSS classes to use. Defaults to "input-group colorpicker-component colorpicker-element".

