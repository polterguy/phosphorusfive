﻿﻿﻿Ajax widgets in Phosphorus Five
===============

p5.web is the Ajax web widget GUI library for Phosphorus Five and Hyperlambda. This is the part that makes it possible for you to use
Active Events such as *[create-widget]* and *[set-widget-property]*. In addition, it contains helper Active Events which
allows you to modify stuff such as the response HTTP headers, access the session object, and even entirely take control
over the returned response through events such as *[p5.web.echo]* and *[p5.web.echo-file]*. To a large extent, it wraps p5.ajax, 
allowing you to use Active Events to create, delete and manipulate Ajax widgets.

However, to start out with the obvious, let's first take a look at how we create a basic Ajax widget.

## Creating your first Ajax web widget

Below is a piece of code that allows you to create an Ajax web widget, which handles the "click" event on the server, and
modifies the widget's text property when clicked.

```
create-widget:my-widget
  element:h3
  parent:content
  position:0
  innerValue:Click me!
  onclick
    p5.types.date.now
    set-widget-property:my-widget
      innerValue:x:/@p5.types.date.now?value.string
```

The above lambda, will create an H3 HTML element, having the value of "Click me!", which when clicked, will change its value to the server's date and time.
The *[onclick]* callback to the server, is of course wrapped in an Ajax HTTP request automatically for you. Allowing you to focus on
your domain problems, and not on the Ajax internals.

There exists three basic Active Events for creating such mentioned Ajax widgets in P5. They are for the most parts almost identical, except for
some minor details which sets them apart.

### The common arguments to all create widget events

These arguments are common to all create widgets events, meaning *[create-literal-widget]*, *[create-container-widget]* 
and *[create-void-widget]*. Notice, if you use the generic *[create-widget]* event, your widget type will be automatically figured out, according to
whether or not you supply an *[innerValue]*, *[widgets]* collection, or neither.

The most important argument is the value of your create widget invocation node. This argument becomes the ID of your
widget, and how you will reference it, both on the server through Hyperlambda, and on the client through JavaScript.
The ID argument is optional, and if you do not supply it, then an automatically generated ID will be assigned your widget.

* [parent] - Defines the parent widget for your widget. Mutually exclusive with [after] and [before]
* [position] - Defines the position in the parent list of widgets, only applicable if you define [parent]
* [before] - An ID to a widget from where your widget should be injected before. Mutually exclusive with [parent], [after] and [position]
* [after] - An ID to a widget from where your widget should be injected after. Mutually exclusive with [parent], [before] and [position]

Notice, you can only declare one of *[parent]*, *[before]* or *[after]*, and only if you declare a *[parent]*, you can declare a *[position]*.
By cleverly using the above arguments, you can insert your Ajax widgets, exactly where you wish in your page's DOM structure.

In addition to the positional arguments, all widgets also optionally takes some other common arguments. These are listed below.

* [visible] - Boolean, defines if the widget is initially rendered as visible or invisible
* [element] - Defines which HTML element to render your widget with
* [events] - List of p5.lambda Active Events, that are coupled with your widget, and only alive as long as your widget exist

### HTML attributes and widget events

In addition to these especially handled arguments, you can in addition add up any argument you wish. Depending upon the name of your argument, 
it will either be handled as an HTML attribute to your widget, or an event of some sort. If your argument starts with the text "on", it will
be assumed to be some sort of client side event. Either a DOM JavaScript event, if your node has a value, or a server-side Ajax event, if it has no value,
but contains a lambda object of its own.

Below is an example of how to create a widget with a class attribute and a style attribute for instance.

```
create-widget:some-other-widget
  element:h3
  parent:content
  position:0
  style:"background-color:LightBlue;"
  class:some-css-class
  innerValue:Colors
```

Since neither of our custom arguments above (the style and class arguments) starts out with "on", they are treated as custom HTML
attributes, and not as events.

If you want to create an Ajax event instead, you could do this like the following code illustrates.

```
create-widget
  element:h3
  parent:content
  position:0
  innerValue:Colors, hover your mouse over me!
  onmouseover
    set-widget-property:x:/../*/_event?value
      style:"background-color:LightGreen"
```

Since our above argument starts out with "on", P5 automatically creates an Ajax server-side event, evaluating the associated lambda every time
this event is raised.

Notice one detail in the above lambda, which is that it does not declare an explicit ID. This means that the widget will have an automatically
assigned ID, looking something like this; "x3833968". This means that we do not know the ID of our widget from inside our *[onmouseover]* event.
However, this is not a problem for us, since the ID of the widget will be forwarded into all Ajax events, and lambda events automatically. 
Each time an event is raised, it will have an *[_event]* argument passed into it, which is the server-side and client-side 
ID of our widget. By referencing this *[_event]* argument in our *[set-widget-property]* expression above, we are able to modify the widget's
properties.

In fact, if you have a list of widgets, automatically created, inside for instance a loop, which creates rows and cells for a table for instance - 
Then you _should not_ give these widgets an explicit ID, but rather rely upon the automatically generated ID, to avoid the problem of having
multiple widgets on your page, with the same ID - Which would be a severe logical error! If you use your own explicit IDs, you should also take
great care making sure they are unique for your page. Which means that you would probably end up creating some sort of namespacing logic for 
things that are used on multiple pages, and injected as reusable controls into your page - Which is a common pattern when using P5.

### JavaScript and client-side DOM events

If you create an argument that starts out with the text "on", and have a value, instead of children nodes, you can put any arbitrary JavaScript you wish
into this value. This would inject your JavaScript into the attribute of your widget, as rendered on the client-side, allowing you to create
JavaScript hooks for DOM HTML events. Imagine something like this for instance.

```
create-widget
  element:h3
  parent:content
  position:0
  innerValue:JavaScript events - Hover your mouse over me!
  onmouseover:"alert ('foo');"
```

The above lambda, will render an HTML element for you which when the mouse hovers over it, creates an alert JavaScript message box. Any JavaScript
you can legally put into an "onmouseover" attribute of your HTML elements, you can legally put into the above value of *[onmouseover]*. Using this
logic, you could completely circumvent the server-side Ajax parts of Phosphorus Five, and roll your own logic - While still retaining the ability 
to use all the other nice features of the library.

### In-visible properties and events

Sometimes you have some value, or event for that matter, which you want to associate with your widget, but you don't want to render it back to the
client, but instead only access it from your server. Or as would be the case for in-visible events, not render them as your widget's HTML, but
be able to access them through the JavaScript API of P5.

This is easily done, by simply prepending an underscore (_) in front of your widget's attribute or event. This would ensure that this attribute or
event is not rendered as a part of your markup, but only accessible on the server (for attributes) or through the JavaScript API (for events).

For instance, to create a value, which you can only access on the server, you could do something like this.

```
create-widget
  element:h3
  parent:content
  position:0
  innerValue:Click me to see the server-side value of [_foo]
  _foo:foo value
  onclick
    get-widget-property:x:/../*/_event?value
      _foo
    set-widget-property:x:/../*/_event?value
      innerValue:Value of [_foo] is '{0}'
        :x:/@get-widget-property/*/*?value
```

Notice how the *[get-widget-property]* retrieves the *[_foo]* value, while the *[set-widget-property]* sets the *[innerValue]* of the widget
to a string formatted value, where the "{0}" parts becomes the value retrieved from the widget's *[_foo]*'s value. Notice also how this value is
only acessible from the server, and not visible or possible to retrieve on the client. Neither by inspecting the DOM, HTML or by using JavaScript.

By starting an attribute with an underscore (_), it is completely invisible for the client, in every possible way.

If you prepend an event with underscore (_), or a period (.) - The results are similar. Consider this code.

```
create-widget:some-invisible-event
  element:h3
  parent:content
  position:0
  innerValue:Click me to see the server-side value of [_foo]
  .onfoo
    set-widget-property:x:/../*/_event?value
      innerValue:[.onfoo] was raised
  onclick:@"p5.$('some-invisible-event').raise('.onfoo');"
```

Notice that if you inspect the DOM or HTML of the above output, the *[.onfoo]* widget event is completely invisible in all regards. Only when you
attempt to raise it, you realize it's there, since it is invoked, and no exception occurs. Semantically, there is no difference between prepending 
a period (.), or an underscore (_) in front of neither your widget's properties or events. However, by convention, I recommend people use periods (.)
for in-visible events, and underscores (_) for in-visible property values.

The above code also uses some parts of P5's JavaScript API in its *[onclick]* value, which is documented in [p5.ajax](../../core/p5.ajax), 
if you're interested in the details.

For the record, almost all arguments to your widgets are optional. We've added in our examples the *[element]*, *[parent]* and *[position]* simply
to make sure it stands out if you evaluate it in the System42/executor.

### Container widgets

So far we have only used the *[create-literal-widget]*, indirectly though. The literal widget, can only contain text or HTML, through 
its *[innerValue]* property. However, a container widget, allows you to create an entire hierarchy of widgets. Instead of having an 
*[innerValue]* property, it contains a collection of children widgets, through its *[widgets]* property. Let's illustrate with an example.

```
create-widget
  element:ul
  parent:content
  position:0
  widgets
    literal
      element:li
      innerValue:Element no 1
    literal
      element:li
      innerValue:Element no 2
```

Notice, we still use the same *[create-widget]* event, but this time we supply a *[widgets]* collection, instead of a *[innerValue]* property.

Another thing to notice in the above code, is that the children nodes of the *[widgets]* property of our container widget, can have the following 3 
values (actually 4 values, but our 4th widget is special, and explained later)

* [literal] - Declares a literal widget
* [container] - Declares an inner container widget
* [void] - Declares a void widget

These 3 values maps to the *[create-literal-widget]*, *[create-container-widget]* and *[create-void-widget]* - And you could, if you wanted to, 
create a single widget at the time, and then fill it manually with its children widgets, by using one of the create Active Events. However, you
will probably find the above syntax more easy and simple to use, for creating rich hierarchies of widgets, where all your widgets are known during
the initial creation.

And of course, each widget above could also have its own set of events and attributes, both hidden and visible. An example is given below.

```
create-widget
  element:ul
  parent:content
  position:0
  style:"font-size:large"
  widgets
    literal
      element:li
      innerValue:Element no 1
      style:"background-color:LightGreen;"
      onclick
        sys42.windows.info-tip:Widget 1 was clicked
    literal
      element:li
      innerValue:Element no 2
      style:"background-color:LightBlue;"
      onclick
        sys42.windows.info-tip:Widget 2 was clicked
```

You can also of course create container widgets that have container widgets as their children widgets, as deeply as you see fit for your personal needs.

### Void widgets

The void widget is useful for HTML elements that have neither content nor children widgets. Some examples are the HTML "br" element, "input" elements,
and "hr" elements. These HTML elements are characterized by that they completely lack content. They are often used to create HTML form elements,
such as is given an example of below.


```
create-widget:some-void-widget
  element:input
  parent:content
  position:0
  style:"width:400px;"
  placeholder:Type something into me, and click the button ...
  class:form-control
create-widget
  element:input
  parent:content
  position:1
  type:button
  value:Click me!
  class:btn btn-default
  onclick
    get-widget-property:some-void-widget
      value
    sys42.windows.info-tip:You typed; '{0}'
      :x:/@get-widget-property/*/*?value
```

In the above example, we create two form elements. One textbox and one button. Both are created using the void widget type, since they have
neither any *[innerValue]*, nor any *[widgets]* collection - Hence, they implicitly become void widgets. Above you also see an
example of how you can add arbitrary attributes, that affects the rendering of your widgets, in some regards. The example above for instance, is
using the *[type]* and *[placeholder]* attributes from HTML5, in addition to the *[value]* attribute for our button. To use
the p5.web project in Phosphorus Five, requires some basic knowledge about HTML, and preferably HTML5 - In addition to some basic knowledge about
CSS of course.

### Ninja tricks for declaring HTML elements of your widgets

When you create a container widget, you can optionally declare its children widgets' HTML element as their name. This saves you a couple of
lines of code for each widget. If you do, the "type" of widget (container, literal, void), will be implicitly figured out according to whether or 
not you've added an *[innerValue]* argument, *[widgets]* collection, or none of the previously mentioned. For instance, to create a container widget, 
that has one "p" element child, you could do something like the following.

```
create-widget
  parent:content
  widgets
    p
      innerValue:Some text
```

The widget is still a fully fledged widget, and you can add event handlers, lambda events, and so on to it - Just like as if it was declared like 
the following.

```
create-widget
  parent:content
  widgets
    literal
      element:p
      innerValue:Some text
```

However, you save one line of code, and your lambda widget declarations becomes more easily understood and read, since they reflect the HTML
hierarchy they create more directly.

### The default HTML elements for widgets

By default, all create widget Active Events, will use the following HTML elements for rendering the widgets on the client.

* [create-container-widget] - "div"
* [create-literal-widget] - "p"
* [create-void-widget] - "input"

This means you can omit the *[element]* arguments if you are creating the above HTML elements.

### Creating custom reusable widgets

To create a custom widget, all you have to do, is to create an Active Event, containing a period (.).
Then make sure this Active Event returns exactly _one_ valid widget for a *[widgets]* argument to a container widget. Now you 
can use your Active Event as an extension widget.

```
// This Active Event becomes a "custom widget type"
create-event:foo.bar
  return
    literal
      innerValue:Howdy world

create-widget
  parent:content
  position:0
  widgets

    // At this point, we can use our "custom widget" as if it was any other widget type
    foo.bar:some-id-to-widget
```

The above lambda, first declares an Active Event with the name of *[foo.bar]*. This Active Event returns one *[literal]* widget. Then we use
our *[foo.bar]* as if it was just another widget type, in our *[widgets]* collection of our root container widget, created 
using *[create-widget]*.

The above Ninja trick, allows you to create reusable complex widgets, as Active Events, which you can include in any other parts of your program.
Just as if they were normal widgets. By creating rich hierarchies of widgets, through Active Events as above, you can create any amount of complexity
in your extension widgets, such as TreeViews, DataGrids, etc. Your Active Event extension widgets can also take arguments, allowing for you to
parametrize your widgets during creation.

It is important that such custom widget Active Events, returns _one_ and _exactly one_ widget back to the caller. This single widget however, 
can be a *[container]* widget, containing several children widgets of itself.

Notice, the single return value from your Active Event, will have the widget id passed in as value to your invocation, automatically becoming
the id for the return widget from your Active Event. If no ID is given, an automatic ID will be generated, just like for any other widgets.

### The [text] widget

There exist a fourth widget, although it is not technically a widget. It is simply the ability to inject text into your resulting HTML at some
specific position. This can be useful for instance when you wish to inject inline JavaScript or CSS into your resulting HTML for some reasons.
This widget has no properties or attributes, and cannot be de-referenced in any ways - Neither in JavaScript nor on the server side.
Imagine the following.

```
create-widget
  parent:content
  position:0
  widgets
    text:@"<style type=""text/css"">
.my-class {
    background-color:rgba(255,255,0,.8);
    font-size:large;
    font-family:Comic Sans MS
}</style>"
    literal
      class:my-class
      innerValue:This should be rendered with yellow background and some 'funny' font
```

Notice that this is not actually a widget, but simply a piece of text, rendered into the resulting HTML, at the position you declare it.
This means, that it cannot be de-referenced in any ways on the server side, or the client side, and the only way to update it, is to remove it
entirely, deleting its parent widget. In general terms, it is considered advanced, due to the previously mentioned reasons - 
And you should not use it, unless you are 100% certain about that you understand its implications. Besides, using inline JavaScript or CSS, is 
anyways considered an anti-pattern when composing your HTML. And doing just that, is one of the few actual use-cases for it. However, it's there for
those cases when a normal widget simply won't cut it for you.

### [get-widget-property] and [set-widget-property]

These two Active Events, allows you to retrieve and change any property you wish. The serialization is 100% automatic back
to the client, and all the nitty and gritty details are taken care of for you automatically. We have alread used both of them already,
but in this section, we will further dissect them, to gain a complete picture of how they work.

The *[get-widget-property]* allows you to get as many properties as you wish, in one go. You declare each property you wish to retrieve as the name 
of children nodes of the invocation, and P5 automatically fills out the values accordingly. Consider this.

```
create-widget
  parent:content
  position:0
  element:h3
  innerValue:Foo
  class:bar
  onclick
    get-widget-property:x:/../*/_event?value
      innerValue
      class
    sys42.windows.show-lambda:x:/..
```

The *[sys42.windows.show-lambda]* invocation above, is a helper event in System42, and allows you to inspect any section of your currently evaluated 
lambda object. We use it as a shortcut event, for seeing the results of our *[get-widget-property]* invocation. After evaluating the above code, 
in System42/executor, you should see something like this at the top of your main browser window.

```
onclick
  _event:xa886f5b
  get-widget-property
    xa886f5b
      innerValue:Foo
      class:bar
  sys42.windows.show-lambda:x:/..
```

As you can see above, the *[get-widget-property]* returns the ID for your widget, and beneath the ID, each property requested as a "key"/"value" 
child node. Another interesting fact you see above, is that we use a root iterator expression, still it displays only the *[onclick]* node. This is because
during the evaluation of your *[onclick]* event, the onclick node _is_ your root node. This is similar to how it works when you invoke a lambda object,
or dynamically created Active Event, using for instance *[eval]*. The rest of your lambda object, is not accessible at this point.

You can also retrieve multiple widget's properties in one invocation, by supplying an expression leading to multiple IDs. Consider this.

```
create-widget
  parent:content
  position:0
  element:h3
  widgets
    literal:first-literal
      innerValue:Some value
      class:bar1
      element:span
    literal:second-literal
      innerValue:Click me!
      class:bar2
      element:span
      onclick
        _widgets
          first-literal
          second-literal
        get-widget-property:x:/../*/_widgets/*?name
          innerValue
          class
        sys42.windows.show-lambda:x:/..
```

The *[set-widget-property]* works similarly, except of course, instead of retrieving the properties, it changes them. Consider this.

```
create-widget
  parent:content
  position:0
  element:h3
  widgets
    literal:first-literal
      innerValue:Some value
      class:bar1
      element:span
    literal:second-literal
      innerValue:Click me!
      class:bar2
      element:span
      onclick
        _widgets
          first-literal
          second-literal
        set-widget-property:x:/@_widgets/*?name
          innerValue:Your second widget was clicked
          class:bar-after-update
```

Normally, you will only update a single widget's properties though. But for those rare occassions, where you for instance wish to update the CSS
class of multiple widgets in one go, being able to do such in a single go, is a nifty feature.

Notice, you can use *[set-widget-property]* to set a property which does not previously exist on your widget. This will simply add the property.
You can also use the *[delete-widget-property]* event to entirely delete a property or attribute from your widget. A widget
with a null value in a property, still actually have the property. Its value is simply null. This is necessary to be able to render HTML5 attributes
such as "controls" on a video element, etc. Hence, you cannot supply a null value to remove a widget's attribute using *[set-widget-property]*.

You can also change a widget's visibility using the *[set-widget-property]* Active Event. But these types of properties cannot be removed.
Only attributes rendered to the client as HTML attributes can be removed.

Hint!
There is also an event called *[p5.web.widgets.property.list]*, which returns a list of _all_ properties, for one or more specified widget(s). This 
Active Event can either be given an expression, leading to multiple IDs, or a constant, showing the properties of only one widget at the time.

### Retrieving an entire hierarchy of widget properties

Sometimes, you wish to retrieve every single widget property, recursively, from some specified root widget. For these occassions, you have 
the *[p5.web.widgets.properties.get]* Active Event (plural form). This Active Event will return all properties of the requested name(s), below one or 
more specified root widget.

To see all form element values for instance, in your form, in one go, you could use something like this for instance.

```
p5.web.widgets.properties.get:cnt
  value
```

The above is actually a "Ninja trick" to serialize all form elements from some specified parent widget recursively.

### Changing and retrieving a widget's Ajax events dynamically

The same way you can update and change properties of widgets, you can also change their lambda object for what occurs when some Ajax event is raised.
To do this, you would use the *[p5.web.widgets.ajax-events.get]* and the *[p5.web.widgets.ajax-events.set]* Active Events. Consider this code.

```
create-widget
  parent:content
  position:0
  innerValue:Click me!
  onclick
    set-widget-property:x:/../*/_event?value
      innerValue:I was clicked, click me once more! It tickles!
    p5.web.widgets.ajax-events.set:x:/../*/_event?value
      onclick
        set-widget-property:x:/../*/_event?value
          innerValue:I was clicked one more time! Thank you!
```

The above code dynamically changes the lambda that is evaluated as the widget is clicked, when you click it the first time.
The *[p5.web.widgets.ajax-events.get]* returns its existing lambda object. Try out the following.

```
create-widget
  parent:content
  position:0
  innerValue:Click me!
  onclick
    p5.web.widgets.ajax-events.get:x:/../*/_event?value
      onclick
    sys42.windows.show-lambda:x:/..
```

Hint!
You can also explicitly raise a widget's Ajax events from your own p5.lambda objects, by invoking *[p5.web.widgets.ajax-events.raise]*. 
To raise the *[onclick]* event on three widgets for instance, you could use code similar to this (which won't evaluate without throwing exceptions, 
since these widgets does not exist).

```
_widgets
  no1-widget-id
  no2-widget-id
  no3-widget-id
p5.web.widgets.ajax-events.raise:x:/-/*?name
  onclick

// Or to raise the "onmouseover" for a single widget
p5.web.widgets.ajax-events.raise:no4-widget-id
  onmouseover
```

Phosphorus will not discriminate in any ways between events dynamically raised by the user clicking some object himself, or an event raised by your 
lambda in the manner shown above, except it won't (obviously) have to do an explicit client/server Ajax roundtrip.

### Lambda events for widgets

A widget can also have dynamically declared Active Events associated with it. These are Active Events which are accessible, for any
context within your page - But only as long as the widget is existing on your page. This feature allows you to declare dynamically 
created Active Events, which only exists, as long as your widget exist. Which again makes it easier for your separate parts of your
page, to communicate with other parts of your page, and create a more "object oriented feel" to your widgets. Consider this code.

```
create-widget
  parent:content
  position:0
  events
    my-namespace.foo-event
      sys42.windows.confirm
        header:Howdy foo world
        body:Watch the other label as you click OK!
        .onok
          set-widget-property:the-other-guy
            innerValue:The other guy was clicked!
  widgets
    literal:the-other-guy
      innerValue:Initial value!
    literal
      innerValue:Click me!
      onclick
        my-namespace.foo-event
```

Such lamba events are working identically to any other dynamically created Active Events, or lambda objects, and can take arguments, and return values
and nodes to the caller. They're simply Active Events, which only lives as long as your widget lives, and only for your page!

Also these lambda events can be dynamically changed, the same way you could with Ajax events, using the *[p5.web.widgets.lambda-events.set]*, 
and the *[p5.web.widgets.lambda-events.get]*. Both of these Active Events, works similarly to their Ajax events counterparts.

Hint!
You can also use *[p5.web.widgets.lambda-events.list]* and *[p5.web.widgets.ajax-events.list]* to inspect which lambda or Ajax events are declared 
for a specific widget.

### [oninit], initializing your widgets

The *[oninit]* is a special type of event, which is only raised when your widget is initially created. It is useful for initialization and such
of your widget, but besides from its semantics, it works similarly to an Ajax event. Though none of its internals, not even its existence, is ever
rendered back to the client in any ways.

It can be useful for populating a "select" HTML widget, with "option" items, created dynamically from data in your database, for instance. But there
are many other useful scenarios for this event. Semantically, it behaves similar to an Ajax event, but it cannot be raised from the client. And it 
renders no additional data back to the client neither. When *[oninit]* fires, all other events and properties have already been created for your widget.
This allows you to invoke widget lambda events associated with your widget, and retreieve their properties, from within your *[oninit]* lambda object.

Below is an example of *[oninit]* in action.

```
create-widget
  parent:content
  innerValue:This text will never show up on your page!
  oninit
    set-widget-property:x:/../*/_event?value
      innerValue:Dynamically changed property during [oninit]
      element:h3
```

### Deleting widgets and clearing widget collections

In addition to the above mentioned events, you also have the *[delete-widget]* and *[clear-widget]* events. The first one entirely deletes
a widget from your page, while the second one empties its children collection. If your run this through your System42/executor for instance, 
then your entire page will go "blank".

```
clear-widget:cnt
```

If you wish to instead for instance delete the main menu, you could accomplish that with the following code.

```
delete-widget:main-navbar-wrapper
```

Yet again, don't panic, but refresh your page, and your menu is back!

### Retrieving widgets

There are also several helper Active Events to retrieve widgets, according to some criteria. These are listed below.

* [p5.web.widgets.get-parent] - Returns the parent widget(s) of the specified widget(s)
* [p5.web.widgets.get-children] - Returns the children widgets of the specified widget(s)
* [p5.web.widgets.find] - Returns the widgets that have the properties listed, with optionally, the values listed
* [p5.web.widgets.find-like] - Same as above, but doesn't require an exact match, only that the widget's properties contains your value(s)
* [p5.web.widgets.find-ancestor] - Returns the first ancestor widget with the specifed properties and values
* [p5.web.widgets.find-ancestor-like] - Same as above, but is happy as long as the value contains the value(s) specified
* [p5.web.widgets.list] - List all widgets that have IDs matching the specified string(s)
* [p5.web.widgets.list-like] - Same as above, but is happy as long as the ID(s) contains the value(s) specified
* [p5.web.widgets.exists] - Yields true for each widget that exists matching the specified ID(s)

All the above mentioned events, returns the same structure back to caller, which looks like the following;

```
some-widget-retrieval-event
  bar1
    container:foo
  bar2
    container:foo
```

The first level of children, are the currently iterated criteria passedin as *[_arg]*. Then comes one or more children nodes, having a name defining
which type of widget this is, and a value being the ID of our widget. Have that in mind as we look at these Active Events and how they work.

#### [p5.web.widgets.get-parent]

Returns the parent widget of one or more specified widget(s). Example given below.

```
p5.web.widgets.create-container:foo
  parent:content
  position:0
  widgets
    literal:bar1
      innerValue:bar1
    literal:bar2
      innerValue:bar2
p5.web.widgets.get-parent:x:/../0/**/literal?value
```

The above code, uses an expression leading to each value of each *[literal]* node within the *[widgets]* part of the *[p5.web.widgets.create-container]* 
invocation. This means that it will return the parent widget for both the "bar1" widget and the "bar2" widget, which happens to be the same. 
The output should look something like this, showing the type of widget as the name, and the ID of the parent widget as its value.

```
/* ... rest of code ... */
p5.web.widgets.get-parent
  bar1
    container:foo
  bar2
    container:foo
```

Notice how *[p5.web.widgets.get-parent]* optionally can take an expression as its argument. This means we have to return an additional "layer" before returning
the actual parent widget(s), since we need to return "parent widget to which widget" to the caller. If we had simply returned the parent as the first
node of *[p5.web.widgets.get-parent]*, we wouldn't know who this was a parent to, since we can supply multiple widgets as arguments. This is a general pattern
for many of the widget retrieval events.

#### [p5.web.widgets.get-children]

Returns the children widgets of one or more specified widget(s). Example below.

```
p5.web.widgets.create-container:foo
  parent:content
  position:0
  widgets
    literal:bar1
      innerValue:bar1
    literal:bar2
      innerValue:bar2
p5.web.widgets.get-children:foo
```

The above code uses a constant, instead of an expression as our previous example, and should return something like this.

```
/* ... rest of code ... */
p5.web.widgets.get-children
  foo
    literal:bar1
    literal:bar2
```

Even though we do _not_ use an expression in the above example, we still return the children widgets in a similar structure as we did with our previous
example of *[p5.web.widgets.get-parent]*, which used an expression. This is to make sure we always return data to the caller in the same format. If we had
used an expression leading to multiple source widgets, instead of the constant of "foo", we would have had multiple return nodes beneath *[p5.web.widgets.get-children]*
in our result.

Notice!
Most of these retrieval Active Events will actually throw an exception if the queried widget does not exist. If you supplied "does-not-exist" to 
your *[p5.web.widgets.get-children]* invocation for instance, it would throw an exception.

#### [p5.web.widgets.find] and [p5.web.widgets.find-like]

These events takes (optionally) the "root widget from where to start your search", and require you to parametrize them with children nodes, 
having at least a name, and optionally a value. The name of the node, is some attribute that must exist on your widget, for it be returned as 
a "match". The value, is an optionally "value" for that attribute, which the widget must either have an exact match of (*[p5.web.widgets.find]*), 
or "contain" (for the *[p5.web.widgets.find-like]* event).

Let's see an example.

```
p5.web.widgets.create-container:foo
  parent:content
  position:0
  widgets
    literal:bar1
      innerValue:bar1
      class:foo-class
    literal:bar2
      innerValue:bar2
p5.web.widgets.find:foo
  innerValue:bar1
  class:foo-class
p5.web.widgets.find:foo
  innerValue:bar
p5.web.widgets.find-like:foo
  innerValue:bar
```

The above code has two invocations to *[p5.web.widgets.find]* and one to *[p5.web.widgets.find-like]*. The first invocation, requires an exact match for 
the *[innerValue]*, and the *[class]* attributes, being respectively "bar1", and "foo-class". This invocation will return only the "bar1" widget. 
The second invocation won't yield any matches, since it requires an exact match and no widgets have the innerValue of "bar" exactly. 
The third invocation will yield both widgets as its return value, since they both have an *[innerValue]* which _contains_ the text "bar".
The result will look something like this.

```
/* ... rest of code ... */
p5.web.widgets.find
  foo
    literal:bar1
p5.web.widgets.find
p5.web.widgets.find-like
  foo
    literal:bar1
    literal:bar2
```

These Active Events can also take expressions as their arguments.

```
p5.web.widgets.create-container:foo1
  parent:content
  position:0
  widgets
    literal:bar1
      innerValue:bar1
      class:foo-class
    literal:bar2
      innerValue:bar2
p5.web.widgets.create-container:foo2
  parent:content
  position:0
  widgets
    literal:bar3
      innerValue:bar1
      class:foo-class
    literal:bar4
      innerValue:bar2
p5.web.widgets.find:x:/../*/[0,2]?value
  innerValue:bar1
```

Which of course result in this result.

```
/* ... rest of code ... */
p5.web.widgets.find
  foo1
    literal:bar1
  foo2
    literal:bar3
```

#### [p5.web.widgets.find-ancestor] and [p5.web.widgets.find-ancestor-like]

These two Active Events are similar to the *[p5.web.widgets.find]* events, except of course, instead of searching "downwards" in the hierarchy from the root
widget supplied, they search upwards in the ancestor chain for a match, from the currently iterated widget. These Active Events requires (for obvious 
reasons) the caller to actually supply a value, and will not tolerate a "default" value as our previously mentioned widget retrieval Active Events did.
Example code given below.

```
p5.web.widgets.create-container:foo1
  parent:content
  position:0
  class:foo-class-1
  widgets
    container:bar1
      widgets
        literal:starting-widget-1
          innerValue:Foo bar
p5.web.widgets.create-container:foo2
  parent:content
  position:0
  class:foo-class-2
  widgets
    container:bar2
      widgets
        literal:starting-widget-2
          innerValue:Foo bar
p5.web.widgets.find-ancestor-like:x:/../**/literal?value
  class:foo-class
```

Which of course will yield the following result.

```
p5.web.widgets.find-ancestor-like
  starting-widget-1
    container:foo1
  starting-widget-2
    container:foo2
```

#### [p5.web.widgets.list] and [p5.web.widgets.list-like]

These Active Events simply lists all widgets, with an ID matching some (optional) filter supplied. Notice, if you do not supply a filter, this Active
Event will yield every single widget in your page, starting from the root "cnt" widget, declared in your Default.aspx markup. Example code is
given below.

```
p5.web.widgets.create-container:test-foo1
  widgets
    literal:test-bar1
      innerValue:Bar 1
    literal:test-bar2
      innerValue:Bar 2

// Will only return "test-bar2"
p5.web.widgets.list:test-bar2

// Will not return anything
p5.web.widgets.list:test

// Will return all three widgets
p5.web.widgets.list-like:test

// Will return only the two literals
_lit
  test-bar1
  test-bar2
p5.web.widgets.list:x:/-/*?name
```

The above code will yield something like this.

```
/* ... rest of code ... */
p5.web.widgets.list
  literal:test-bar2
p5.web.widgets.list
p5.web.widgets.list-like
  container:test-foo1
  literal:test-bar1
  literal:test-bar2
_lit
  test-bar1
  test-bar2
p5.web.widgets.list
  literal:test-bar1
  literal:test-bar2
```

One point though, as in many of the other widget retrieval Active Events, is the fact that you are also getting the "type" of widget returned, as
the name of the node - While you get the ID of the widget, returned as the value - Which might be useful sometimes.

Notice though that [p5.web.widgets.list] does not return an "injected node" for the widget you start out your search from, simply since there are no such
node(s) or criteria given to it. In addition, the same widget might match several criteria, making it impossible to put into one specific filter.
This means it yields one level less than all other widget retrieval Active Events.

#### [p5.web.widgets.exists]

This Active Event allows you to check for the existence of one or more specified widget(s). It can take either a constant, or an expression.
Below is an example.

```
p5.web.widgets.create-container:test-foo1
  widgets
    literal:test-bar1
      innerValue:Bar 1
    literal:test-bar2
      innerValue:Bar 2
p5.web.widgets.exists:test-bar2
```

Which will yield the following result.

```
/* ... rest of code ... */
p5.web.widgets.exists
  test-bar2:bool:true
```

### Some facts about p5.web and its Ajax widgets

Warning, here comes the "marketing pitch" ...

For those developers out there whom are seasoned C#/ASP.NET developers, it is probably clear at this point, that p5.web is able to keep its state
automatically, mirroring what goes on in server-land, automagically back to the client, and vice versa. In addition, there is no weird syntax, and 
simply Hyperlambda, combined with expressions. The end result becoming that of, as long as you understand Hyperlambda, expressions, and have some few 
Active Events in your knowledge belt, plus optionally some basic HTML and CSS - Then creating fairly complex web apps, actually becomes ridiculously 
easy. And the Ajax parts, "simply happens". In such a way, it could be argued that Phosphorus Five, to some extent, is a _"fifth generation programming
language"_. Taking away all the nitty gritty details from your burdon, allowing you to focus solely on your domain problems.

And all of these features, simply are there for you, for free, without preventing you from rolling your own cutting edge C# and/or ASP.NET code.

Although I tend to refer to P5 as a web operating system, since it solves so many problems - At its core, it is actually nothing but a bunch 
of loosely coupled libraries, which can be used either as a combined product, or included in your own projects, to give you only some small
subset of functionality. p5.ajax is a primary example of this, being a "pure" C#/ASP.NET library, which you can consume completely alone, without
having to bring in any other parts at all. Allowing you to create Web Controls, almost the same way as you would with traditional ASP.NET.

This creates a ladder of transition for you, which allows you to easily start using some smaller sub-sets of P5, without completely forcing unto
you an entirely new system development paradigm.

## Storing stuff and state in a web context

In ASP.NET you have lots of different "storage objects" which you can use, such as the Session object, Application object, Cache, Cookies, 
and so on. These same storage facilitates are mapped into Phosphorus Five, through the C# Active Events you can find in the [storage](storage/) 
folder of this project.

They all more or less obey by the same "API" (Active Event syntax), which allows you to easily change between any one of them, later in your project,
as you see fit. This gives you a flexible environment for development, where you can easily move your data storage around, from different objects, 
as you see fit later.

Below is a complete list of all of these Active Events.

* [p5.web.session.set] - Has private (.) alias
* [p5.web.session.get] - Has private (.) alias
* [p5.web.session.list] - Has private (.) alias
* [p5.web.application.set] - Has private (.) alias
* [p5.web.application.get] - Has private (.) alias
* [p5.web.application.list] - Has private (.) alias
* [p5.web.context.set] - Has private (.) alias
* [p5.web.context.get] - Has private (.) alias
* [p5.web.context.list] - Has private (.) alias
* [p5.web.cache.set] - Has private (.) alias
* [p5.web.cache.get] - Has private (.) alias
* [p5.web.cache.list] - Has private (.) alias
* [p5.web.cookie.set] - Has private (.) alias
* [p5.web.cookie.get] - Has private (.) alias
* [p5.web.cookie.list] - Has private (.) alias
* [p5.web.header.set]
* [p5.web.header.get]
* [p5.web.header.list]
* [p5.web.params.get]
* [p5.web.params.list]

Not all of them can be interchanged with each other, since for instance things like HTTP headers, cannot tolerate complete node structures and more
complex objects, in addition to that storing stuff you'd normally store in your session, obviously does not at all make sense storing in an HTTP
header. But all of the above events, obeys by the same set of rules, and "API".

Most of the above Active Events also have "native alias versions", which allows you to invoke them with a "." in front of their name. If you do,
then you are allowed to retrieve, list and modify "protected data", which is really data starting with either an underscore (_) or a period (.).

This ensures that you can store things into these objects, which is only accessible through C#. Which is an additional security feature, for things
you do not wish some random piece of p5.lambda to access.

Notice, there are also three Active Events defined in the [p5.webapp](/core/p5.webapp/) project, which allows you to store "page values" (ViewState).

* [p5.web.viewstate.set] - Has private (.) alias
* [p5.web.viewstate.get] - Has private (.) alias
* [p5.web.viewstate.list] - Has private (.) alias

The above three mentioned events, also obeys by the same "API" as the once listed below.

### Accessing the ASP.NET Session object

If we start out with the "Session" object from ASP.NET, there are three basic Active Events which allows you to use your session. 

* [p5.web.session.set]
* [p5.web.session.get]
* [p5.web.session.list]

For those not aquinted with how the session object works, it is often a memory based in-process storage, for objects you wish to associate with a
single session (user activity), for as long as a user is actively using your web site. When the server is rebooted, the user leaves your website, 
or some other disaster occurs, all session variables are discarded and lost.

The session object _can_ (optionally, through configuring your web.config for instance), be stuffed into a database, making it cross-process enabled,
and so on. But by default, it is in memory. All objects stored into the session, are only accessible for the currently visiting client, and only
for the lifespan of his "session". Often iit times out after some minutes (20 minutes by default) of inactivity, which means the user will loose
all his session variables if he does not somehow, interact with your website, for 20 minutes.

To show you an example of it, imagine the following code.

```
p5.web.session.set:test.my-session-variable
  src:Some piece of data goes here
p5.web.session.get:test.my-session-variable
```

In the above code, we first "set" a session variable, name it "test.my-session-variable", and give it the static value of "Some piece of data goes here",
before we retrieve it again. The retrieval does not have to be in the same request though. As long as your session has not timed out, you can refresh
your browser, and evaluate the following code, and your original value will still be returned back to you.

```
p5.web.session.get:test.my-session-variable
```

All of the "object storage" Active Events in p5.web, takes a "source" argument, the same way *[add]* and *[set]* does. Which allows you to create
fairly complex sources, using Active Event sources, and so on. To store a slightly more complex object into your session, you could use something 
like this.

```
p5.web.session.set:test.my-session-variable
  src
    my-node:My value
      my-other-node
        some-integer:int:5
p5.web.session.get:test.my-session-variable
```

Above we see an example of adding typed objects into the session, which of course is no problem.

Like with *[set]* though, you can only have _one_ source. This means that if you have complex hierarchies of nodes, you have to make sure you have 
one "root node". This is because you're setting "one item" in your session. If you wish to put "arrays" into your session, you'll have to have them 
as children of a single "root node". The following code will raise an exception ...

```
// Throws an exception!!
p5.web.session.set:test.my-session-variable
  src
    my-node:My value
    my-other-node
```

As with *[set]*, you can also set multiple values using expressions, and have the source be a relative Active Event invocation, having the *[_dn]*
node passed into each invocation of your source. Consider this code for instance.

```
_values
  test-session-1:Some value
  test-session-2:Some other value
p5.web.session.set:x:/-/*?name
  eval
    return:x:/../*/_dn/#?value
p5.web.session.get:test-session-1
p5.web.session.get:test-session-2
```

The above code will create two session variables for you, one called "test-session-1", and another called "test-session-2", with the values from
these nodes as the values of your session objects.

#### Listing your session keys using [p5.web.session.list]

Sometimes it can be useful to have a list of which session keys you have in your current session object. This is easily done using *[p5.web.session.list]*.
An example of usage is shown below.

```
p5.web.session.list
```

If you provide a value, or an expression as its value, this will be used as a "filter" that your keys must match, in order to be returned. Example
is given below.

```
p5.web.session.list:test
```

The above would yield, assuming you've still got your session objects from our first example in your session, the following result.

```
p5.web.session.list
  test-session-1
  test-session-2
```

To retrieve all values, from all session objects, could easily be done combining this invocation with a *[p5.web.session.get]* invocation.

```
p5.web.session.list:test
p5.web.session.get:x:/-/*?name
```

### Accessing the application object

The "application" object has an API which is 100% identical to the "session" object, and consists of these three Active Events.

* [p5.web.application.get]
* [p5.web.application.set]
* [p5.web.application.list]

The only difference is the underlaying implementation, which stores your values in the "global application object" instead of your session object.
The "application" object i global for all users of your website. Besides from that, it is really quite similar to the session object, and only
"lives" for the life time of your server process, being stored in memory, the same way as your session values.

To see some examples of it in use, see the session examples, and simply replace "session" with "global".

### Accessing your "context" object

The HttpContext object is also identical to both the session and the application ("global") object, except of course, it stores things in the 
HttpContext object instead. To understand the difference, check out the documentation for the HttpContext class in .Net Framework. Replace 
the parts "session" with "context" to use the HttpContext object instead.

### Accessing your cache object

Also this object is identical to both your session object, application object, and context object, except it uses the keyword "cache" instead of
"session" and "application" etc. It carries one additional argument though, which is *[minutes]*, which defined for how many minutes the cache
object you insert should be valid for. This argument is only applicable when invoking *[set-cache-object]*, and has no relevance for any of the
other cache Active Events.

### Cookies in P5

The cookies Active Events are similar to the session Active Events, except you cannot retrieve a cookie that you set in the same response. This is 
because when you "set" a cookie, you are modifying the HTTP response object, while when you "retrieve" a cookie, you are retrieving it from the 
HTTP request. Hence after setting a new cookie value, then it won't be accessible before you return the response back to the client, and the client
makes another request to your server.

In addition, type information is "partially lost" when you set a cookie. To illustrate with an example, run these two piece of code in two different
evaluations in your System42/executor.

```
p5.web.cookie.set:some-cookie
  src
    foo:bar
      some-integer:int:5
```

Then run this code.

```
p5.web.cookie.get:some-cookie
```

As you can see in your last piece of code, your cookie value is returned as a "string". It is quite easily converted back into a node though, by
running it through *[hyper2lambda]* with the following code.

```
p5.web.cookie.get:some-cookie
hyper2lambda:x:/-/*?value
```

This is because although you can store "objects" in your session, application and cache objects, etc - The cookie collection of your browser, can
only handle "string". However, your node(s) are converted correctly into Hyperlambda before stored into your cookie collection, which allows you to
convert it easily back to p5.lambda using *[hyper2lambda]*.

### HTTP headers

Now you could if you wanted to, store p5.lambda code in your HTTP headers, however, doing such a thing, makes absolutely no sense what-so-ever!!
HTTP headers are for informing the browser, and/or potential proxies, about some state of your application. And using it as a "general storage", would
be like using a crocodile to hunt raindeers! Don't do it!

However, setting an HTTP header obeys by the same API as all other "storage events". To have your web app return a "custom header" to your client,
you could do something like this.

```
p5.web.header.set:foo
  src:bar
```

If you make sure you inspect your HTTP request, before you click evaluate in your System42/executor, you will see that the return value from your server,
contains one additional HTTP header called "foo" having the value of "bar".

HTTP headers "read" and "write" operations are sufffering from the same problem as cookies for the record, which is that "read" gets its data from the
request, while "write" puts its data into the response. Which means that stuff you put into an HTTP header using a "set" operation, is not accessible
for a consecutive "get" operation. For obvious reasons ...

### HTTP GET parameters

To access your HTTP GET parameters, you can use *[p5.web.params.get]* or *[p5.web.params.list]*. Since the GET parameter of a request, is a read-only 
type of collection, there exists no "setter". Try to add up the following string at the end of your URL; "&foo-key=bar-value". Then run the following code.

```
p5.web.params.get:foo-key
```

Now of course, since modifying the HTTP GET parameter collection of a request, requires changing the URL of a request, there exists no "setter".
Notice that since the parameter collection in ASP.NET also returns POST variables, in addition to lots of other types of objects, you will also find
for instance stuff like your cookies hen invoking for instance *[p5.web.params.list]*, and even its raw value when using *[p5.web.params.get]*. In addition,
you will also find the HTTTP headers, and all sort of other "unexpected stuff" in your parameter collection. However, this is the way ASP.NET is 
implemented, and kept in P5 to remain consistant towards the underlaying implementtation of .Net.


## Accessing and modifying the "raw HTTP request" and the "raw HTTP response"

If you wish, you can completely bypass the default HTTP serialization and deserialization, and instead, take complete control of every aspect of
both the rendering and the parsing of HTTP requests and responses. This is useful if you are creating web services or returing files to the caller
for instance. For such cases, you have the *[p5.web.echo]* and *[p5.web.echo-file]* Active Events for creating your own response. And you have the *[p5.web.request.get-body]*,
and the *[p5.web.request.get-method]* events. These Active Events allows you to access the "raw" HTTP request, as sent by the client, and create your own response, 
exactly as you see fit.

The default HTTP request/response model in P5, uses POST requests for each Ajax request, and returns JSON to the caller. However, if you wish, you
can completely bypass this model, and create your own. Imagine if you wish to return a file to the caller for instance. This is easily achieved by
creating a page, which contains logic like this.

```
p5.web.echo-file:/system42/README.md
```

If you evaluate the above code in for instance the System42/executor, then it will throw an exception, of course, since the Ajax method invoked when
you click the "Evaluete" button expects the server to return JSON. However, if you create a page, which during initial loading, instead of creating
an Ajax web widget hierarchy, evaluates the above lambda - The you will download the above file to the client when  you load that page.

To see an example, create a *[lambda]* page in the CMS of System42/executor with the URL of "/download-my-file". Then change its code to the following.

```
p5.web.echo-file:/system42/README.md
p5.web.header.set:Content-Type
  src:text/plain
```

Then save your page and click "Preview", and you should see your file. If you change the code for your page to the following.

```
p5.web.echo:@"This is foo calling!"
p5.web.header.set:Content-Type
  src:text/plain
```

... then you will see some statically created text instead.

Notice, once you invoke *[p5.web.echo]* or *[p5.web.echo-file]*, then you cannot invoke it again. This means that if you want to dynamically build up your content, 
then you have to build it first, and have *[p5.web.echo]* be the last piece of logic in your page. The *[p5.web.echo-file]* event will also throw an exception if the
currently logged in user is not authorized to reading the file you supply to it.

Both *[p5.web.echo]* and *[p5.web.echo-file]* can optionally take expressions, leading to either one or more pieces of text, or one file.

Creating web services, using *[p5.web.echo]*, which instead of returning HTML to the client, returns some other piece of data, is quite easy using this
technique.

Hint!
You can return Hyperlambda to the client using the *[p5.web.echo]* event, since it will automatically convert whatever expression it is given to text, which
allows you to return some sub-set of your tree to the caller.

### Getting the raw HTTP request

You can also retrieve the "raw" HTTP request, by using the *[p5.web.request.get-body]* Active Event. This Active Event will return the raw HTTP request sent
by the client, giving you complete access to do whatever you wish with it.

Hint!
This is a quite useful feature of P5, since it allows you to create "web service end-points", where you can for instance pass in Hyperlambda, or some
other piece of "machine readable type of request", which is intended to be used by machines and web services, instead of browser clients.

If you wish to directly save the request, without first putting it into memory, you can save some memory and CPU cycles by directly saving the request
body using the *[p5.web.request.save-body]* Active Event, which takes a constant or expression leading to a filename on your server. Notice, this event
requires the currently logged in user context to be able to write to the path supplied.

Hint!
Use the *[login]* Active Event to change the current "user context ticket" if you wish to save your files to a "restricted folder".

### Additional request helper events

You can also get the HTTP method of your request, using the *[p5.web.request.get-method]* Active Event, in addition to that you can have P5 make its best "guess"
of whether or not the request originated from a "mobile device" using the *[p5.web.request.is-mobile]* Active Event. The latter is not 100% perfect,
since a mobile device is not required to identify itself as such to your server. But it is good enough for most cases, and will do a decent job, 
determining if the client is some sort of "mobile device" or not.

### Modifying your HTTP status code and text

You can modify the "status message" and the "status code" of your response using these two Active Events.

* [p5.web.response.set-status-code] - Sets the "status code", requires an integer, or an expression leading to an integer as its input
* [p5.web.response.set-status-description] - Sets the "status message", tolerates anything that is convertible into text

### Ninja tricks when creating web services

One thing you should realize about *[p5.web.echo]* and *[p5.web.request.get-body]*, is that you can both pass in, and return Hyperlambda, which you then convert to 
p5.lambda, for then to evaluate it as such. This feature of P5, allows you to pass "code" from your client, to a server, and have the server evaluate 
your "code", for then to return "code" back again, which the client evaluates on its side.

This allows the client to decide what code is to be evaluated on your web-server endpoints, which at least in theory, makes it possible for you to
create one single web service point, for all your web-service needs. In addition, it lets you "massage" the output from some web service endpoint,
before it is returned, which can possibly reduce your response, and network traffic, significantly compared to a "specialized web service endpoint",
which possibly yields values back to the client, which the client is not interested in, etc.

This feature has some serious security issues, which you should consider before you go down this path though. But if you are 100% certain of that you
trust the client that initiated the request (due to being a part of your own intranet for instance), then you can safely use this feature, and allow
your clients to invoke "code" in your web service endpoints.

If you combine this feature, with the PGP cryptographic features of P5, requiring having your web service invocations cryptographically signed, before
you evaluate them as Hyperlambda, you can create an additional layer of protection, further safe-guarding you against malicious requests. Which is 
probably a "must", if you choose to use this "Ninja trick", in a production environment. This way you have a cryptographically secure context, giving
your guarantees of that the invocation towards your web service, was created by some client, which you trust 100%, since the signing process of
an invocation, makes sure it has not beeen tampered with in any ways after leaving the client. And only the client owning the private key that was
used to sign the invocation, can create a signature matching your expectations.

To use this feature, you would have to pass in your invocations as MIME messages, using the p5.mime library of P5. This would also allow you to
encrypt your invocations, making it impossible for an adversary, to listen in on the "conversation", between your client(s) and your server(s).

If you encrypt your web service invocations, you can even use features such as the Active Event *[login]*, in your embedded Hyperlambda,
to change the Application Context user ticket for your web service invocations, giving you further rights, having your invocation being evaluated 
in an explicit user context.



