Common Bootstrap Modal Windows
===============

This folder contains the most commonly used Bootstrap modal windows, allowing for most common scenarios when you need to
display modal windows to your users. There are three basic windows, which are kind of incrementally built on top of each other.

* [sys42.windows.confirm] - The most basic window, allowing for letting user to confirm some action.
* [sys42.windows.modal] - Slightly more advanced version, allowing for you to inject custom widgets into its modal body.
* [sys42.windows.wizard] - Automatically creates widgets from its [_data] segment for you.


## [sys42.windows.confirm] a modal confirmation window

This Active Event, creates a modal confirmation window, which by default simply contains a simple "OK" button, which once clicked,
can (optionally) evaluate some piece of lambda, declared through its [.onok] lambda callback. An example is show below.

```
sys42.windows.confirm
  _header:Please confirm action!
  _body:Sorry, I won't do this until you confirm you really want to do it!
  .onok
    sys42.windows.info-tip:Something dangerous just happened!!
```

The above code, will display the following.

![alt tag](/core/p5.webapp/system42/components/bootstrap/windows/sys42-windows-confirm-screenshot.png)

Your *[.onok]* lambda callback, will only be evaluated if the user clicks the "OK" button. If he closes the window, by for instance
clicking the "X" or clicking outside of the modal window's main surface, then the window will simply close, without evaluating
the associated lambda callback.

In addition to an *[.onok]* lambda callback, you can also supply an *[.oncancel]* lambda, which will evaluate only if the window
is closed _without_ user clicking the "OK" button. Below is en example showing this. Try to close the window, both by clicking
the "OK" button, and by clicking the "X" in the top right corner, and watch the difference.

```
sys42.windows.confirm
  _header:Please confirm action!
  _body:Sorry, I won't do this until you confirm you really want to do it!
  .onok
    sys42.windows.info-tip:Something dangerous just happened!!
  .oncancel
    sys42.windows.info-tip:Puuh, you just avoided the dangerous stuff!!
```





The modal window contains the following public Active Events.

* [sys42.windows.confirm.cancel] - Closes the window, evaluating the specified *[.oncancel]* lambda callback.

