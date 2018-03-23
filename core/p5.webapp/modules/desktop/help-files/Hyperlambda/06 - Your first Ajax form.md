## Your first Ajax form

Arguably, among your most important tasks in an Ajax application, is the gathering of input from your users.
This is easily done with P5. Below is an example. If you wish, you can simply paste the code below into your
_"Hello World"_ application, from our previous chapter, and test it out without creating a new app. Or you
can simply click the _"lightning"_ button, to evaluate it immediately.

```hyperlambda-snippet
/*
 * Includes the Micro CSS files.
 */
micro.css.include

/*
 * Creating main wire frame for module.
 *
 * This simple creates a "container/row/col" hierarchy of widgets,
 * necessary to have Micro do its magic.
 */
create-widget
  class:container
  widgets
    div
      class:row
      widgets
        div
          class:col
          widgets

            /*
             * Our app's actual widgets, first a textbox.
             */
            void:your_name
              element:input
              type:text
              class:fill
              placeholder:Give me your name ...

            /*
             * Then a multiline textarea widget.
             */
            literal:your_adr
              element:textarea
              placeholder:Give me your address ...
              class:fill
              rows:7

            /*
             * Finally a button
             */
            button
              innerValue:Submit
              onclick

                /*
                 * Retrieves our form values.
                 */
                get-widget-property:your_name
                  value
                get-widget-property:your_adr
                  value

                /*
                 * "Forward evaluates" our [p/innerValue] node.
                 */
                eval-x:x:/+/**/innerValue

                /*
                 * Shows a "modal window".
                 */
                create-widgets
                  micro.widgets.modal
                    widgets
                      h3
                        innerValue:Thx dude!
                      p
                        innerValue:So, I guess your name is '{0}', and your address is '{1}'
                          :x:/../*/get-widget-property/*/your_name/*?value
                          :x:/../*/get-widget-property/*/your_adr/*?value

/*
 * Providing some info, in case you
 * evaluate this snippet directly from
 * the documentation system of Phosphorus Five.
 *
 * NOTICE, just remove this if you have copied this code into
 * your own "Hello World" app's "launch.hl" file.
 */
micro.windows.info:Scroll to the bottom of your page to see your app
```

### Analyzing our code

First of all, we don't care about giving neither our main root **[container]**
widget, nor our button widget any explicit IDs. This ensures that our widgets will have
_"automatically assigned IDs"_. You can see these IDs if you _"inspect"_ your page, using e.g. Google Chrome,
and right clicking the widgets.

Secondly we create a simple *"input"* widget, followed by a *"textarea"* widget. We have give both of these
widgets explicit IDs, since we want to refer to them inside our **[onclick]** event handler. The _"input"_
element is created as a **[void]** widget, while our _"textarea"_ element is created as a **[literal]** widget.
This is important for these particular types of widgets. Our button is using _"automatic type inference"_, which
allows us to declare it simply as **[button]**. This is possible since it has an **[innerValue]** property,
which makes it redundant to declare it as a **[literal]** widget.

### Retrieving form data

Probably the most complex parts above, is the stuff that's happening in our **[onclick]** event handler.
To make it clear, let's isolate the lambda of our **[onclick]** Ajax event.

```hyperlambda
/*
 * Retrieves our form values.
 */
get-widget-property:your_name
  value
get-widget-property:your_adr
  value

/*
 * "Forward evaluates" our [p/innerValue] node.
 */
eval-x:x:/+/**/innerValue

/*
 * Shows a "modal window".
 */
create-widgets
  micro.widgets.modal
    widgets
      h3
        innerValue:Thx dude!
      p
        innerValue:So, I guess your name is '{0}', and your address is '{1}'
          :x:/../*/get-widget-property/*/your_name/*?value
          :x:/../*/get-widget-property/*/your_adr/*?value
```

Initially we retrieve the values of our *"your_name"* input widget, and our *"your_adr"* textarea widget. This is
done with our two **[get-widget-property]** invocations. Then we show a modal confirmation window with the data
supplied by the user. However, before we show this modal confirmation window, there's an invocation to
**[eval-x]**. This simply _"forward evaluates"_ the expressions found in our **[p]**/**[innerValue]** node.
Since this is a formatted string, with its formatting values pointing to the results of our get-widget-property
invocations - This means that when **[micro.widgets.modal]** is evaluated, the innerValue of our p element,
will be a static string, being the product of our formatting expressions, having its *"{n}"* parts, exchanged
with its n'th child node's result.

**Notice**, the above `:x:` parts of our Hyperlambda, are in fact what we refer to as _"lambda expressions"_.
Such expressions allows you to reference other nodes in your lambda structure. If you have some knowledge of
XPath, the similarities might be obvious. We will diver deeper into lambda expressions in later chapters.

### Wrapping up

In this chapter, we created our first web form, accepting user input. The next two chapters will be fairly
theoretical in nature, and might feel more difficult to grasp initially - However, please hang in there, since
in the chapter after the next two chapters, we will create a fully fledged _"database driven app"_, leveraging
the knowledge we go through in our next two chapters.
