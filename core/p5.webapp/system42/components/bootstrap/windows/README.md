Common Bootstrap Modal Windows
===============

This folder contains the most commonly used Bootstrap modal windows, allowing for most common scenarios when you need to
display modal windows to your users. There are three basic windows, which are kind of incrementally built on top of each other.

* [sys42.windows.confirm] - The most basic window, allowing for letting user to confirm some action.
* [sys42.windows.modal] - Slightly more advanced version, allowing for you to inject custom widgets into its modal body.
* [sys42.windows.wizard] - Automatically creates widgets from its [_data] segment for you.


## [sys42.windows.confirm] a modal confirmation window

This Active Event, creates a modal confirmation window, which by default simply contains a simple "OK" button, which once clicked,
can (optionally) evaluate some piece of lambda, declared through its *[.onok]* lambda callback. An example is show below.

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

Your *[.oncancel]* lambda callback will also evaluate if the user clicks anywhere outside of the modal window.

In addition to simply using the default "OK" button, you can also supply your own collection of buttons that will be used instead
of the "OK" button. This is done by adding any buttons you wish inside of a *[_buttons]* argument. Consider the following.

```
sys42.windows.confirm
  _header:Please confirm action!
  _body:Sorry, I won't do this until you confirm you really want to do it!
  _buttons
    button
      class:btn btn-default
      innerValue:Yup
      oninit
        sys42.windows.confirm.initial-focus:x:/../*/_event?value
      onclick
        sys42.windows.confirm.ok
    button
      class:btn btn-default
      innerValue:Nope
      onclick
        sys42.windows.confirm.cancel
  .onok
    sys42.windows.info-tip:Something dangerous just happened!!
  .oncancel
    sys42.windows.info-tip:Puuh, you just avoided the dangerous stuff!!
```

Notice the invocations to *[sys42.windows.confirm.ok]*, *[sys42.windows.confirm.cancel]* and *[sys42.windows.confirm.initial-focus]* above.
The *[sys42.windows.confirm.ok]* Active Event will evaluate your *[.onok]* lambda callback, while the *[sys42.windows.confirm.cancel]*
will evaluate your *[.oncancel]* callback. You could of course entirely bypass these if you wish, by simply providing your own lambda
in the *[onclick]* event handler for your buttons. However, these Active Events are there for your convenience, to allow you to create
more explicit, and more understandable code.

Notive also the invocation to *[sys42.windows.confirm.initial-focus]* in the *[oninit]* event of your "Yup" button. This makes sure the
button gains focus initially, when displayed. Since the modal window is shown hidden initially, you cannot simply give focus to your
buttons, by attaching some custom JavaScript, since your JavaScript would fissle, unless it is attached such that it evaluates after your
modal window has been displayed. This Active Event ensures that the focus JavaScript sent to the client, is not evaluated before the window
has been shown.

Notice also that you can put any type of widget into the *[_buttons]* argument, but since they will be appended into the footer of your modal,
it will probably look pretty unintuitive if you add something else besides buttons into it. If you wish to create more complex modal windows,
with support for your own widgets, you should probably rather use the *[sys42.windows.modal]* or the *[sys42.windows.wizard]* Active Events.

