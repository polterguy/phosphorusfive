Hyperlisp editor for Phosphorus Five
==========

Notice, this module is built on top of CodeMirror, which is copyright (C) 2016 by Marijn Haverbeke <marijnh@gmail.com> and others.

CodeMirror is licensed under an MIT license. Read more [here](https://codemirror.net)

This "app" creates a single Active Event for you, called *[sys42.widgets.codemirror]*, which wraps CodeMirror as a 
"custom widget" on your page, in "Hyperlisp mode". To use it, you could use something like the code below.

```
create-widget
  parent:content
  widgets
    sys42.widgets.codemirror:my_editor
```

To get to Hyperlisp created by it, simply use *[get-widget-property]*, and pass in *[value]* as the argument of what to retrieve.
Example code given below.

```
create-widget
  parent:content
  widgets
    sys42.widgets.codemirror:my_editor
    button
      innerValue:Get Hyperlisp
      class:btn btn-default btn-attach-top
      onclick
        get-widget-property:my_editor
          value
        sys42.show-code-window:x:/..
```

Which of course will return the Hyperlisp as "plain text", and not transform it to p5.lambda. To make p5.lambda out of it, you can use
the *[lisp2lambda]* Active Event from [p5.hyperlisp](/plugins/p5.hyperlisp/).

To find out which arguments you can pass into it, you can use the generic lambda object, retrieving the *[_defaults]* section
of an Active Event, such as illustrated below.

```
sys42.widgets.ck-editor
  insert-before:x:
    src:x:/../*/_defaults/*
  return
```

In addition to the "arguments" you can pass into it, you can also pass in the following.

* [innerValue] - To set the initial Hyperlisp as the widget is loaded
* [events] - To associate lambda events with the widget

All other properties are ignored, and to the most parts, don't really give any sense, since the HTML "textarea" widget rendered, is actually
completely replaced at the client-side of things, by the CodeMirror's internals.


