## Your first Ajax form

Arguably, among your most important tasks in an Ajax application, is the gathering of input from your users. This is easily done with P5. 
Below is an example. If you wish, you can simply paste the code below into your _"Hello World"_ application, from our previous chapter,
and test it out without creating a new app. Or you can simply click the _"lightning"_ button, to evaluate it immediately.

**Notice**, to close a modal window from Micro, you can simply click outside of its main surface.

```hyperlambda-snippet
/*
 * Includes CSS for our module.
 */
p5.web.include-css-file
  @MICRO/media/main.css
  @MICRO/media/fonts.css
  @MICRO/media/skins/serious.css

/*
 * Creating main wire frame for module.
 */
create-widget
  parent:hyper-ide-help-content
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
                p5.web.widgets.property.get:your_name
                  value
                p5.web.widgets.property.get:your_adr
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
                          :x:/../*/p5.web.widgets.property.get/*/your_name/*?value
                          :x:/../*/p5.web.widgets.property.get/*/your_adr/*?value
```

There are several new concepts in the above piece of Hyperlambda. Let's walk through them all, to get a grasp of exactly what is going on here.
First of all, we don't care about giving neither our main root **[container]** widget, nor our button widget any explicit IDs. This ensures 
that our widgets will have *"automatically assigned IDs"*. You can see these IDs if you view the HTML of your page.
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
  p5.web.widgets.property.get:your_name
    value
  p5.web.widgets.property.get:your_adr
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
            :x:/../*/p5.web.widgets.property.get/*/your_name/*?value
            :x:/../*/p5.web.widgets.property.get/*/your_adr/*?value
```

Initially we retrieve the values of our *"your_name"* input widget, and our *"your_adr"* textarea widget. This is done with our 
two **[p5.web.widgets.property.get]** invocations. Then we show a modal confirmation window with the data supplied by the user. However, 
before we show this modal confirmation window, there's an invocation to **[eval-x]**. This simply *"forward evaluates"* the expressions found 
in our p/innerValue node. Since this is a formatted string, with its formatting values pointing to the results of our p5.web.widgets.property.get
invocations - This means that when **[micro.widgets.modal]** is evaluated, 
the innerValue of our p element, will be a static string, being the product of our formatting expressions, having its *"{n}"* parts, 
exchanged with its n'th child node's result.

**Notice**, only nodes without names, will be considered when creating such _"formatted strings"_.

**Notice**, the above `:x:` parts of our Hyperlambda, are in fact what we refer to as *"lambda expressions"*. These allows you to reference other nodes 
in your lambda structure. If you have some knowledge of XPath, the similarities might be obvious. We will diver deeper into lambda expressions
in later chapters.

The **[micro.widgets.modal]** above, is an extension widget from Micro, which will be created as we invoke **[create-widgets]** (plural form).
