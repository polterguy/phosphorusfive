Common Bootstrap Modal Windows
===============

This folder contains the most commonly used Bootstrap modal windows, allowing for most common scenarios when you need to
display modal windows to your users. There are three basic windows, which are kind of incrementally built on top of each other.

* [sys42.windows.confirm] - The most basic window, allowing for letting user to confirm some action.
* [sys42.windows.modal] - Slightly more advanced version, allowing for you to inject custom widgets into its modal body.
* [sys42.windows.wizard] - Automatically creates widgets from its [_data] segment for you.


## [sys42.windows.confirm] a modal confirmation window

This Active Event, creates a modal confirmation window, which by default simply contains a simple "OK" button, which once clicked,
can (optionally) evaluate some piece of lambda, declared through its [_onok] lambda callback. An example is show below.

```
sys42.windows.confirm
  _header:Please confirm action!
  _body:Sorry, I won't do this until you confirm you really want to do it!
  _onok
    your-namespace.some-dangerous-active-event-invocation
    sys42.windows.info-tip:Something dangerous just happened!!
```

The above code, will display the following.

