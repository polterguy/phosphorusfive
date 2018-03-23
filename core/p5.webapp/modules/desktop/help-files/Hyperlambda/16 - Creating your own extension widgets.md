## Creating your own extension widgets

In this chapter, we will bring the component approach from our previous chapter even further, 
and create our own _"extension widget"_. An extension widget, is a rich and complex, reusable 
GUI element, which you can include where ever you can include a _"native"_ widget.
So far, we have only used the core widgets. However, to create a custom and more complex widget 
in Hyperlambda, is actually surprisingly easy. As always, the answer is to create an Active 
Event. Execute the following code in Hypereval, or inline by clicking the _"flash"_ button.

```hyperlambda-snippet
/*
 * This Active Event will become an "extension widget",
 * which you can consume anywhere you can use a widget.
 */
create-event:examples.widgets.foo
  return
    container
      widgets
        literal
          innerValue:Hello world!
        literal
          innerValue:Hello even more dear world!
```

The above is actually a custom widget. To consume it, is equally simple. Just evaluate the
following code.

```hyperlambda-snippet
/*
 * Creates a modal widget with our extension widget.
 */
create-widgets
  micro.widgets.modal
    widgets
      examples.widgets.foo
```

The above should result in a simple **[container]** widget, containing two **[literal]** widgets. 
Anywhere you can use a literal, container, or void widget - You can also use 
any custom extension widget.

An Active Event that is an extension widget, needs to return exactly one widget. This widget can 
of course be a **[container]** widget, which allows us to create any amount of 
internal complexity within our widget we wish. The above widget, is probably not that useful. 
However, since Active Events can be parametrized, changing our lambda object as it executes - 
We can parametrize our custom widgets, any ways we see fit.

### Your first useful extension widget

Below is a slightly more useful widget, which will create a form, asking the user for both his
name, and his address.

```hyperlambda-snippet
/*
 * Creates a slightly more "useful" extension widget.
 */
create-event:examples.widgets.name-address
  eval-x:x:/+/**/value
  return
    container
      widgets
        container
          class:strip fill
          widgets
            label
              innerValue:Name
            input
              type:text
              .data-field:name
              placeholder:Your name ...
              value:x:/../*/name?value
        literal
          element:textarea
          class:fill
          .data-field:address
          placeholder:Your address ...
          rows:7
          value:x:/../*/address?value
```

If you evaluate the above code in Hypereval, or inline in the documentation system, for then to evaluate the
following - You can see it in action.

```hyperlambda-snippet
/*
 * Consumes our "name/address" extension widget, created
 * in the above snippet.
 */
create-widgets
  micro.widgets.modal
    widgets
      examples.widgets.name-address:name-of-your-form
        name:John Doe
        address:Foo bar st. 57
```

The above code will produce something resembling the following.

https://phosphorusfive.files.wordpress.com/2018/01/extension-widget-screenshot.png

The brilliance of this construct, is that we get to keep our code very _"DRY"_. DRY is another one of those
acronyms that developers love, and it means _"Don't Repeat Yourself"_. If you repeat a lot of code, then
your code becomes much more difficult to modify and maintain. Imagine we later wanted to expand on our 
original widget, and add a _"phone"_ textbox. In our above example, all we need to do, is to modify our
extension widget, and every place we consume this extension widget, will automatically have another textbox
within it.

In fact, most of Hyper IDE is exclusively built using extension widgets. The file explorer for instance, is
an extension widget. The CodeMirror editor, is an extension widget, and so on. Using these constructs, you
can significantly reduce the number of lines of code (LOC) of your projects. Hyper IDE's initial release
was only 2,200 LOC, and it arguably was a fully fledged IDE. Which of course was why I could 
[create it in 7 days](https://dzone.com/articles/how-i-created-a-web-based-ide-in-7-days).

A note on this though, is that you would probably want to put the name _"widgets"_ inside of your extension 
widgets as you create these events. Strictly speaking, this is not necessary, but it allows you to more easily
search for, and find, your widgets, and not mix them together with other Active Events.
