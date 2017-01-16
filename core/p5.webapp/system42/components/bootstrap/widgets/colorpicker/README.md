The Bootstrap Ajax ColorPicker widget
========

This folder contains an Ajax ColorPicker widget, built upon [Stefan Petre](https://twitter.com/stefanpetre/)'s 
and [Javi Aguilar](https://itsjavi.com/)'s [Bootstrap ColorPicker](https://itsjavi.com/bootstrap-colorpicker/), that 
allows your users to pick a color. Below is an example of how to use it.

```
p5.web.widgets.create-container
  parent:content
  class:col-xs-4
  widgets

    // Changes bg-color when colorpicker value changes.
    literal:output-widget
      class:prepend-bottom
      innerValue:Here comes color!

    // Actual colorpicker.
    sys42.widgets.colorpicker:my-color-picker
      value:#ff00ff
      label:My color
      .onchange
        p5.web.widgets.property.set:output-widget
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

## Active Events

The ColorPicker also contains the following Active Events.

* [sys42.widgets.colorpicker.get-value] - Returns the value of the ColorPicker
* [sys42.widgets.colorpicker.set-value] - Sets the value of the ColorPicker. Pass in [_value] as new value.

To use them, make sure you pass in the ID of your ColorPicker as *[_arg]*, such as the following illustrates.

```
p5.web.widgets.create-container
  parent:content
  class:col-xs-4
  widgets
    sys42.widgets.colorpicker:my-color-picker
      value:#ff00ff
      label:My color
p5.web.widgets.create-container
  parent:content
  class:col-xs-8
  widgets
    button
      class:btn btn-default
      innerValue:Get value
      onclick
        sys42.widgets.colorpicker.get-value:my-color-picker
        sys42.windows.info-tip:Your color was '{0}'
          :x:/@sys42.widgets.colorpicker.get-value?value
    button
      class:btn btn-default
      innerValue:Set value
      onclick
        sys42.widgets.colorpicker.set-value:my-color-picker
          value:#ffff00
```

