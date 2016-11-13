The Bootstrap Ajax DateTimePicker
========

This folder contains the Ajax DateTimePicker for Phosphorus Five. The widget is created on top of 
the [Bootstrap DateTimePicker](https://eonasdan.github.io/bootstrap-datetimepicker/) as an extension widget.
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
      _onchange
        sys42.widgets.datetimepicker.get-value:my-date
        set-widget-property:my-lit
          innerValue:x:/@sys42.widgets.datetimepicker.get-value?value
```

The above code will create something like the following.

![alt tag](/core/p5.webapp/system42/components/bootstrap/widgets/datetimepicker/screenshots/datetimepicker-example-screenshot.png)

It features the following options.

* [_label] - Descriptive label.
* [_icon] - CSS class for the icon rendered, defaults to "glyphicon-calendar".
* [_locale] - Locale to use. Basically a reference to the JS files in the "media/js/locale/" folder.
* [_format] - Format for displaying date.
* [_defaultDate] - Default value the widget will get when DateTimePicker is shown.
* [_view-mode] - View mode, can be any of; "decades", "years", "months", "days"
* [_min-date] - Minimum valid date for selecting new dates.
* [_max-date] - Maximum valid date for selecting new dates.
* [_show-today-button] - If true, will show the "select today date button".
* [_onchange] - Lambda callback invoked when the DateTimePicker is hidden.

In addition, the widget features the following Active Events;

* [sys42.widgets.datetimepicker.get-value] - Retrieves its current value.
* [sys42.widgets.datetimepicker.set-value] - Sets its current value.

To understand the options above, please refer to the documentation for the [Bootstrap DateTimePicker](https://eonasdan.github.io/bootstrap-datetimepicker/Options/),
since the DateTimePicker is actually just a wrapper around this JavaScript component.
