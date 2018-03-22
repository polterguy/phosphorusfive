## Your first Ajax form

Arguably, among your most important tasks in an Ajax application, is the gathering of input from your users. This is easily done with P5. 
Below is an example. If you wish, you can simply paste the code below into your _"Hello World"_ application, from our previous chapter,
and test it out without creating a new app. Or you can simply click the _"lightning"_ button, to evaluate it immediately.

**Notice**, to close a modal window from Micro, you can simply click outside of its main surface. Also notice that if
you evaluate the snippet inline by clicking the _"flash"_ button - You _will have to scroll to the bottom of your page_
to find your app.

```hyperlambda-snippet
/*
 * Includes the Micro CSS files.
 */
micro.css.include

/*
 * Creating main wire frame for module.
 */
create-widget
  class:container
  widgets
    div
      class:row
      widgets
        div
          class:col-100
          widgets
            void:your_name
              element:input
              type:text
              class:fill
              placeholder:Give me your name ...
            literal:your_adr
              element:textarea
              placeholder:Give me your address ...
              class:fill
              rows:7
            literal
              element:button
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
 */
micro.windows.info:Scroll to the bottom of your page to see your app
```

There are several new concepts in the above piece of Hyperlambda. Let's walk through them all, to get a grasp of exactly what is going on here.
First of all, we don't care about giving neither our main root **[container]** widget, nor our button widget any explicit IDs. This ensures 
that our widgets will have *"automatically assigned IDs"*. You can see these IDs if you _"inspect"_ your page, using e.g.
Google Chrome, and right clicking the widgets
.
Secondly, we create a simple *"input"* widget, followed by a *"textarea"* widget. Both of these widgets, we have given an explicit ID, since
we want to be able to easily retrieve their values later. The *"input"* element is created as a **[void]** widget, while our *"textarea"* 
element is created as a **[literal]** widget. This is important for these particular types of widgets.

### Retrieving form data

Probably the most complex parts above, is the stuff that's happening in our **[onclick]** event handler. To make it clear, let's isolate the 
lambda of our **[onclick]** Ajax event.

```hyperlambda
// Our [onclick] Ajax event lambda from the above code.
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
```

Initially we retrieve the values of our *"your_name"* input widget, and our *"your_adr"* textarea widget. This is done with our 
two **[get-widget-property]** invocations. Then we show a modal confirmation window with the data supplied by the user. However, 
before we show this modal confirmation window, there's an invocation to **[eval-x]**. This simply *"forward evaluates"* the expressions found 
in our **[p]**/**[innerValue]** node. Since this is a formatted string, with its formatting values pointing to the results of our get-widget-property
invocations - This means that when **[micro.widgets.modal]** is evaluated, 
the innerValue of our p element, will be a static string, being the product of our formatting expressions, having its *"{n}"* parts, 
exchanged with its n'th child node's result.

**Notice**, only nodes without names, will be considered when creating such _"formatted strings"_.

**Notice**, the above `:x:` parts of our Hyperlambda, are in fact what we refer to as *"lambda expressions"*. These allows you to reference other nodes 
in your lambda structure. If you have some knowledge of XPath, the similarities might be obvious. We will diver deeper into lambda expressions
in later chapters.
