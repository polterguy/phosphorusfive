Hyperlambda editor for Phosphorus Five
==========

Notice, this module is built on top of CodeMirror, which is copyright (C) 2016 by Marijn Haverbeke <marijnh@gmail.com> and others.

CodeMirror is licensed under an MIT license. Read more [here](https://codemirror.net)

This component creates an Active Event for you, called *[sys42.widgets.codemirror]*, which wraps CodeMirror as a 
"custom widget" on your page, in "Hyperlambda mode". To use it, you could use something like the code below.

```
create-widget
  parent:content
  widgets
    sys42.widgets.codemirror:my_editor
```

The above will render something like the following.

![alt tag](screenshots/codemirror-example-screenshot.png)

To get to Hyperlambda created by it, simply use *[get-widget-property]*, and pass in *[value]* as the argument of what to retrieve.
Example code given below.

```
create-widget
  parent:content
  widgets
    sys42.widgets.codemirror:my_editor
    button
      innerValue:Get Hyperlambda
      class:btn btn-default btn-attach-top
      onclick
        get-widget-property:my_editor
          value
        sys42.windows.show-lambda:x:/..
```

Which of course will return the Hyperlambda as "plain text", and not transform it to p5.lambda. To make p5.lambda out of it, you can use
the *[hyper2lambda]* Active Event from [p5.hyperlambda](/plugins/p5.hyperlambda/).

To find out which arguments you can pass into it, you can use the generic lambda object, retrieving the *[_defaults]* section
of an Active Event, such as illustrated below.

```
sys42.widgets.ck-editor
  insert-before:x:
    src:x:/../*/_defaults/*
  return
```

The "arguments" you can pass into it are as follows.

* [_innerValue] - To set the initial Hyperlambda as the widget is loaded
* [_events] - To associate lambda events with the widget

All other properties are ignored, and to the most parts, don't really give any sense, since the HTML "textarea" widget rendered, is actually
completely replaced at the client-side of things, by the CodeMirror's internals.

## Skinning your CodeMirror widget

You can skin your CodeMirror widget with the following setting; _"sys42.code-mirror-default-theme"_. The setting is expected to be found in the "sys42"
app. To change youur CodeMirror's skin to something darker, you can evaluate the following code for instance, in your Hyperlambda Executor.

```
sys42.utilities.set-setting:sys42.code-mirror-default-theme
  _app:sys42
  _src:paraiso-dark
```

Refresh your System42/Executor after evaluating the above code, to see the "dark" skin.

There are only two skins distributed with P5 by default, these are;

* "paraiso" - A light skin
* "paraiso-dark" - A dark skin

But you can use any of the CodeMirror skins you wish. Check out the different skins you can use at the [CodeMirror](https://codemirror.net/demo/theme.html) website.

## Creating a Hyperlambda Executor

In addition to the above CodeMirror widget, there is also another codemirror widget, which allows you to inject the entire System42's 
Hyperlambda Executor into your page. This is sometimes useful for debugging purposes, since it allows you to change your page, as it 
is being executed, from within the page's context.

This widget is created by the *[sys42.widgets.codemirror-executor]* Active Event.

To see an example of it, create a new CMS "lambda" page, and paste in the following code.

```
create-widget
  parent:content
  widgets
    sys42.widgets.codemirror-executor
```
