The Bootstrap Ajax DateTimePicker
========

This folder contains the Ajax DateTimePicker for Phosphorus Five. The widget is created on top of 
[Eonasdan](https://twitter.com/Eonasdan)'s [Bootstrap DateTimePicker](https://eonasdan.github.io/bootstrap-datetimepicker/), as an extension widget.
Below is an example of how to use it.

```
create-widget:foo
  parent:content
  class:col-xs-12
  widgets

    // Will have its innerValue changed when dtp value changes.
    literal:my-lit
      innerValue:Watch me change!

    // Actual DateTimePicker widget.
    sys42.widgets.datetimepicker:my-date

      // Norwegian "bokmål" locale.
      _locale:nb

      // Invoked when widget is closed.
      .onchange
        sys42.widgets.datetimepicker.get-value:my-date
        set-widget-property:my-lit
          innerValue:x:/@sys42.widgets.datetimepicker.get-value?value
```

The above code will create something like the following.

![alt tag](/core/p5.webapp/system42/components/bootstrap/widgets/datetimepicker/screenshots/datetimepicker-example-screenshot.png)

It features the following options.

* [_label] - Descriptive label.
* [_class] - Optional CSS classes to use. Defaults to "input-group date".
* [_icon] - CSS class for the icon rendered, defaults to "glyphicon-calendar".
* [_locale] - Locale to use. Basically a reference to the JS files in the "media/js/locale/" folder.
* [_format] - Format for displaying date.
* [_defaultDate] - Default value the widget will get when DateTimePicker is shown.
* [_view-mode] - View mode, can be any of; "decades", "years", "months", "days"
* [_min-date] - Minimum valid date for selecting new dates.
* [_max-date] - Maximum valid date for selecting new dates.
* [_show-today-button] - If true, will show the "select today date button".
* [.onchange] - Lambda callback invoked when the DateTimePicker is hidden.

In addition, the widget features the following Active Events;

* [sys42.widgets.datetimepicker.get-value] - Retrieves its current value.
* [sys42.widgets.datetimepicker.set-value] - Sets its current value. Pass in new value as [_value].

Both of the two above Active Events requires you to pass in the ID of your DateTimePicker as *[_arg]*. The setter above, also requires you to
pass in the new value as *[_value]*. An example of how to change the date, and retrieve it, by the click of a button, can be found beneath.

```
create-widget:foo
  parent:content
  class:col-xs-12
  widgets
    sys42.widgets.datetimepicker:my-date
      _locale:nb
    button
      class:btn btn-primary
      innerValue:Set date
      onclick
        sys42.widgets.datetimepicker.set-value:my-date
          _value:"24.11.2016 09:43"
    button
      class:btn btn-primary
      innerValue:Get date
      onclick
        sys42.widgets.datetimepicker.get-value:my-date
        sys42.windows.info-tip:Date was '{0}'
          :x:/@sys42.widgets.datetimepicker.get-value?value
```

## Understanding the arguments

To understand the options above, please refer to the documentation for the [Bootstrap DateTimePicker](https://eonasdan.github.io/bootstrap-datetimepicker/Options/),
since the DateTimePicker is actually just a wrapper around this JavaScript component.

The arguments to the P5 DateTimePicker are slightly differently written though. For instance, the argument *[_max-date]* wraps the argument to
the Bootstrap DateTimePicker which is called _"maxDate"_. Basically, we eliminate camel-casing, removing capital letters, injecting a hyphen "-"
between words, and prepend a slash "_" in front of argument.

If you have the above in mind while looking through the documentation for Eonasdan's DateTimePicker, you can easily figure out which argument in
the P5 DateTimePicker, refers to which argument in Eonasdan's DateTimePicker.

