Creating web widgets with Phosphorus Five
===============

p5.web is the Ajax web widget "GUI library" for Phosphorus Five. This is the part that makes it possible for you to use
Active Events such as *[create-widget]* and *[set-widget-property]*. In addition, it contains helper Active Events which
allows you to modify stuff such as the response HTTP headers, access the session object, and even entirely take control
over the returned response through events such as *[echo]* and *[echo-file]*.

However, to start out with the obvious, let's first take a look at how we create a basic Ajax widget.

## Creating your first Ajax web widget

Below is a piece of code that allows you to create an Ajax web widget, which handles the "click" event on the server, and
modifies the widget's text property when clicked, to becomes the server-time.

```
create-literal-widget:my-widget
  element:h3
  parent:content
  position:0
  innerValue:Click me!
  onclick
    date-now
    set-widget-property:my-widget
      innerValue:x:/../*/date-now?value.string
```

The above p5.lambda, will create an H3 HTML element, having the value of "Click me!", which when clicked, will change its value to the date and time
at the server. The "onclick" callback to the server, is of course wrapped in an Ajax HTTP request automatically for you. Allowing you to focus on
your domain problems, and not on the Ajax details.

There exists three basic Active Events for creating such mentioned Ajax widgets in P5. They are for the most parts almost identical, except for
some small details which sets them apart.

### The common arguments to all create widget events

These arguments are common to all create widgets events, meaning *[create-literal-widget]*, *[create-container-widget]* and *[create-void-widget]*.

The most important "argument" is the value of your create widget invocation node. This should be a string, if defined, and becomes the ID of your
widget. Both on the client side, if you wish to access your widget using JavaScript, and on the server-side if you wish to de-reference your widget
from your server. This "argument" is optional, and if you do not supply it, then an "automatically generated ID" will be assigned your widget.

* [parent] - Defines the parent widget for your widget. Mutually exclusive with [after] and [before]
* [position] - Defines the position in the parent list of widgets, only applicable if you define [parent]
* [before] - An ID to a widget from where your widget should appear "before of". Mutually exclusive with [parent], [after] and [position]
* [after] - An ID to a widget from where your widget should appear "after". Mutually exclusive with [parent], [before] and [position]

Notice, you can only declare one of *[parent]*, *[before]* or *[after]*. Only if you declare a *[parent]*, you can declare a *[position]*.
By cleverly using the above arguments, you can insert your Ajax widgets, exactly where you wish in your page's DOM structure.

In addition to the positional arguments, all widgets also _optionally_ takes some other common arguments. These are listed below.

* [visible] - Boolean, defines if the widget is initially rendered as visible or invisible
* [element] - Defines which HTML element to render your widget with
* [events] - List of p5.lambda Active Events, that are coupled with your widget, and only alive as long as your widget exist

### HTML attributes and widget events

In addition to these especially handled arguments, you can in addition add up any argument you wish. Depending upon the name of your argument, 
it will either be handled as an "HTML attribute" to your widget, or an event of some sort. If your argument starts with the text "on", it will
be assumed to be some sort of "event". Either a DOM JavaScript event, if your node as a "value", or a server-side Ajax event, if it has no value,
but children nodes instead.

Below is an example of how to create a widget with a "class" attribute and a "style" attribute for instance.

```
create-literal-widget:some-other-widget
  element:h3
  parent:content
  position:0
  innerValue:Colors
  style:"background-color:LightBlue;"
  class:some-css-class
```

Since neither of our "custom arguments" above (the "style" argument and "class" argument) starts out with "on", they are treated as custom HTML
attributes, and not as events.

If you want to create an Ajax event instead, you could do this like the following code illustrates.

```
create-literal-widget
  element:h3
  parent:content
  position:0
  innerValue:Colors, hover your mouse over me!
  onmouseover
    set-widget-property:x:/../*/_event?value
      style:"background-color:LightGreen"
```

Since our above argument starts out with "on", P5 automatically creates an Ajax server-side event, evaluating the associated p5.lambda every time
this event is raised.

Notice one detail in the above p5.lambda, which is that it does not declare an explicit ID. This means that the widget will have an "automatically
assigned ID", looking something like this; "x3833968". This means that we do not know the ID of our widget inside of our *[onmouseover]* event.
However, this is not a problem for us, since the ID of the widget will be forwarded into all Ajax events, and lambda events (which we will speak 
about later) automatically. Each time an event is raised, it will have an *[_event]* argument passed into it, which is the server-side and client-side 
ID of our widget. By referencing this *[_event]* argument in our *[set-widget-property]* expression above, we are able to modify the widget's
properties.

In fact, if you have a list of widgets, automatically created, inside for instance a loop, which creates rows and cells for a table for instance - 
Then you _should not_ give these widgets an "explicit ID", but rather rely upon the automatically generated ID, to avoid the problem of having
multiple widgets on your page, with the same ID - Which would be a severe logical error! If you use your own "explicit IDs", you should also take
great care making sure they are unique for your page. Which means that you would probably end up creating some sort of "namespacing logic" for 
things that are used on multiple pages, and injected as "reusable controls" into your page. Which is a very common pattern for development in P5.

### JavaScript and client-side DOM events

If you create an argument that starts out with the text "on", and have a value, instead of children nodes, you can put any arbitrary JavaScript you wish
into this value. This would inject your JavaScript into the attribute of your widget, as rendered on the client-side, allowing you to create
"JavaScript hooks" for DOM HTML events. Imagine something like this for instance.

```
create-literal-widget
  element:h3
  parent:content
  position:0
  innerValue:JavaScript events - Hover your mouse over me!
  onmouseover:"alert ('foo');"
```

The above p5.lambda, will render an HTML element for you which when the mouse hovers over it, creates an "alert" JavaScript message box. Any JavaScript
you can legally put into an "onmouseover" attribute of your HTML elements, you can legally put into the above value of *[onmouseover]*. Using this
logic, you could completely circumvent the "server-side Ajax parts" of Phosphorus Five, if you wish. Still get to use all the other nice features
of the library, by rolling your own JavaScript handlers, doing whatever you wish for your widgets to do.

### "In-visible" properties and events

Sometimes you have some value or event for that matter, which you want to associate with your widget, but you don't want to render it back to the
client, but instead only access it from your server. Or as would be the case for "in-visible events", not render them as your widget's HTML, but
be able to access them through the JavaScript API of P5.

This is easily done, by simply prepending an underscore (_) in front of your widget's attribute or event. This would ensure that this attribute or
event is not rendered as a part of your markup, but only accessible on the server (for attributes) or through the JavaScript API (for events).

For instance, to create a value, which you can only access on the server, you could do something like this.

```
create-literal-widget
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
        :x:/../*/get-widget-property/*/*?value
```

Notice how the *[get-widget-property]* retrieves the *[_foo]* value, while the *[set-widget-property]* sets the *[innerValue]* of the widget
to a string formatted value, where the "{0}" parts becomes the value retrieved from the widget's *[_foo]*'s value. Notice also how this value is
only acessible from the server, and not visible or possible to retrieve on the client. Neither by inspecting the DOM, HTML or by using JavaScript.

By starting an attribute with an underscore (_), it is completely invisible for the client, in every possible way.

If you prepend an event with underscore (_), the results are similar. Consider this code.

```
create-literal-widget:some-invisible-event
  element:h3
  parent:content
  position:0
  innerValue:Click me to see the server-side value of [_foo]
  _onfoo
    set-widget-property:x:/../*/_event?value
      innerValue:[_onfoo] was raised
  onclick:@"p5.$('some-invisible-event').raise('_onfoo');"
```

Notice that if you inspect the DOM or HTML of the above output, the *[_onfoo]* widget event is completely invisible in all regards. Only when you
attempt to raise it, you realize it's there, since it is invoked, and no exception occurs.

The above code also uses some parts of P5's JavaScript API in its *[onclick]* value, which is documented in p5.ajax, if you're interested in the details.
But basically, it raises an Ajax widget's server side event through the JavaScript API, instead of automatically mapping up the event on your behalf.

For the record, almost all arguments to your widgets are optional. We've added in our examples the *[element]*, *[parent]* and *[position]* simply
to make sure it stands out if you evaluate it in the System42/executor.

### [create-container-widget], widget's having children widgets

So far we have only used the *[create-literal-widget]*. The literal widget, can only contain text or HTML, through its *[innerValue]* property. However,
a "container widget", allows you to create an entire hierarchy of widget. Instead of having an "innerValue" property, it contains a collection
of children widgets, through its *[widgets]* property. Let's illustrate with an example.

```
create-container-widget
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

One thing to notice in the above code, is that the children nodes of the *[widgets]* property of our container widget, can have the following 3 
values (actually 4 values, but our 4th widget is "special", and explained later)

* [literal] - Declares a "literal widget"
* [container] - Declares an inner "container widget"
* [void] - Declares a "void widget" (explained later)

Thes three values maps to the *[create-literal-widget]*, *[create-container-widget]* and *[create-void-widget]* - And you could, if you wanted to, 
create a single widget at the time, and then fill it manually with its children widgets, by using one of the "create Active Events". However, you
will probably find the above syntax more easy and simple to use, for creating rich hierarchies of widgets, where all your widgets are known during
the initial creation.

And of course, each widget above could also have its own set of events and attributes, both hidden and visible. An example is given below.

```
create-container-widget
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
        sys42.info-window:Widget 1 was clicked
    literal
      element:li
      innerValue:Element no 2
      style:"background-color:LightBlue;"
      onclick
        sys42.info-window:Widget 2 was clicked
```

You can also of course create container widgets that have container widgets as their children widgets, as deeply as you see fit for your personal needs.

### [create-void-widget], a widget with no content

The void widget is useful for HTML elements that have neither content nor children widgets. Some examples are the HTML "br" element, "input" elements,
"hr" elements, etc. These HTML elements are characterized by that they completely lack content. They are often used to create HTML form elements,
such as is given an example of below.


```
create-void-widget:some-void-widget
  element:input
  parent:content
  position:0
  style:"width:400px;"
  placeholder:Type something into me, and click the button ...
create-void-widget
  element:input
  parent:content
  position:1
  type:button
  value:Click me!
  onclick
    get-widget-property:some-void-widget
      value
    sys42.info-window:You typed; '{0}'
      :x:/../*/get-widget-property/*/*?value
```

In the above example, we create two form elements. One textbox and one button. Both are created using the "void" widget type. Above you also see an
example of how you can add arbitrary attributes, that affects the rendering of your widgets, in some regards. The example above for instance, is
using the *[type]* attributes from HTML and the *[placeholder]* attribute from HTML5, in addition to the *[value]* attribute for our button. To use
the p5.web project in Phosphorus Five, requires some basic knowledge about HTML, and preferably HTML5 - In addition to some basic knowledge about
CSS of course.

### The [text] widget, for injecting stuff into your HTML

There exist a fourth "widget", although it is not actually a "widget", it is simply the ability to "inject text" into your resulting HTML at some
specific position. This can be useful for instance when you wish to inject inline JavaScript or CSS into your resulting HTML for some reasons.
This widget has no properties or attributes, and cannot be de-referenced in any ways - Neither in JavaScript nor on the server side.
Imagine the following.

```
create-container-widget
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

Notice that this is not a "widget" per se, but simply a piece of text, rendered into the resulting HTML, at the position you declare it.
This means, that it cannot be de-referenced in any ways on the server side, or the client side, and the only way to update it, is to remove it
entirely, deleting its parent widget, and re-render it. In general terms, it is considered "advanced", due to the previously mentioned reasons,
and you should not use it, unless you are 100% certain about that you understand its implications. Besides, using inline JavaScript or CSS, is 
considered an "anti-pattern" when composing your HTML. And doing just that, is one of the few actual use-cases for it. However, it's there for
those cases when a "normal" widget just won't "cut it" for you.

### Changing and retrieving widget properties using [get-widget-property] and [set-widget-property]

These two Active Events, allows you to retrieve and set any property you wish, on any widget you wish. The serialization is 100% automatic back
to the client, and all the nitty and gritty details, are taken care of for you automatically. We have alread used both of them already,
but in this section, we will further dissect them, to gain a complete picture of how they work.

The *[get-widget-property]* allows you to get as many properties as you wish, in one go. You declare each property you wish to retrieve as the name 
of children nodes of the invocation, and P5 automatically fills out the values accordingly. Consider this.

```
create-literal-widget
  parent:content
  position:0
  element:h3
  innerValue:Foo
  class:bar
  onclick
    get-widget-property:x:/../*/_event?value
      innerValue
      class
    sys42.show-code-window:x:/..
```

The *[sys42.show-code-window]* invocation above, is a helper event in System42, and allows you to inspect any section of your currently evaluated 
lambda object. We use it as a shortcut event, for seeing the results of our *[get-widget-property]* invocation. After evaluating the above code, 
in System42/executor, you should see something like this at the top of your main browser window.

```
onclick
  _event:xa886f5b
  get-widget-property
    xa886f5b
      innerValue:Foo
      class:bar
  sys42.show-code-window:x:/..
```

As you can see above, the *[get-widget-property]* returns the ID for your widget, and beneath the ID, each property requested as a "key"/"value" 
child node. Another interesting fact you see above, is that we use the "root iterator", still it displays only the *[onclick]* node. This is because
during the evaluation of your *[onclick]* event, the onclick node _is_ your root node. This is similar to how it works when you invoke a lambda object,
or dynamically created Active Event, using for instance *[eval]*. The rest of your lambda object, is not accessible at this point.

You can also retrieve multiple widget's properties in one call, by supplying an expression leading to multiple IDs. Consider this.

```
create-container-widget
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
        sys42.show-code-window:x:/..
```

The *[set-widget-property]* works similarly, except of course, instead of retrieving the properties, it changes them. Consider this.

```
create-container-widget
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
        set-widget-property:x:/../*/_widgets/*?name
          innerValue:Your second widget was clicked
          class:bar-after-update
```

Normally, you will only update a single widget's properties though. But for those rare occassions, where you for instance wish to update the CSS
class of multiple widgets in one go, being able to do such in a single go, is a nifty feature.

Notice, you can also use the *[set-widget-property]* to set a property which does not previously exist on your widget. This will simply add the property.
You can also use the *[delete-widget-property]* Active Event to entirely delete a property or attribute from your widget.
You can also change a widget's visibility, and even ID using the *[set-widget-property]* Active Event. But thes properties cannot be
removed. Only attributes rendered to the client as HTML attributes can be removed.

### Changing and retrieving a widget's Ajax events dynamically

The same way you can update and change properties of widgets, you can also change their lambda object for what occurs when some Ajax event is raised.
To do this, you would use the *[get-widget-ajax-event]* and the *[set-widget-ajax-event]* Active Events. Consider this code.

```
create-literal-widget
  parent:content
  position:0
  innerValue:Click me!
  onclick
    set-widget-property:x:/../*/_event?value
      innerValue:I was clicked, click me once more! It tickles!
    set-widget-ajax-event:x:/../*/_event?value
      onclick
        set-widget-property:x:/../*/_event?value
          innerValue:I was clicked one more time! Thank you!
```

The above code dynamically changes the p5.lambda that is evaluated as the widget is clicked, when you click it the first time.
The *[get-widget-ajax-event]* returns its existing lambda object. If you evaluate the following code.

```
create-literal-widget
  parent:content
  position:0
  innerValue:Click me!
  onclick
    set-widget-property:x:/../*/_event?value
      innerValue:I was clicked, click me once more! It tickles!
    get-widget-ajax-event:x:/../*/_event?value
      onclick
    sys42.show-code-window:x:/-
```

Then you will see the output format of *[get-widget-ajax-event]* looking something like the following.

```
get-widget-ajax-event
  x56592d0
    onclick
      _event:x56592d0
      set-widget-property:x:/../*/_event?value
        innerValue:I was clicked, click me once more! It tickles!
      get-widget-ajax-event:x:/../*/_event?value
        onclick
      sys42.show-code-window:x:/-
```

As you can see above, below the *[get-widget-ajax-event]*, you can find the ID of the widget requested. Then below that node, you can find
the name of the event requested. Inside of that node, you'll find the lambda being evaluated as the widget is clicked. This structure might
seem counter intuitive in the beginning. However, it allows you to return multiple events, for multiple widgets, in one go. Which is 
quite useful sometimes.

Hint!
You can also explicitly raise a widget's Ajax events from your own p5.lambda objects, by invoking *[raise-widget-ajax-event]*. To raise the *[onclick]*
event on three widgets for instance, you could use code similar to this (which won't evaluate without throwing exceptions, since these widgets does
not exist)

```
_widgets
  no1-widget-id
  no2-widget-id
  no3-widget-id
raise-widget-ajax-event:x:/-/*?name
  onclick

// Or to raise the "onmouseover" for a single widget
raise-widget-ajax-event:no4-widget-id
  onmouseover
```

Phosphorus will not discriminate in any ways between events dynamnically raised by the user clicking some object himself, or an event raised by your 
lambda in the manner shown above.

### Lambda events for widgets

A widget can also have dynamically declared Active Events associated with it. These are Active Events which are accessible, for any
context within your page, but only as long as the widget is existing on your page. This feature allows you to declare dynamically 
created Active Events, which only exists, as long as your widget exist. Which again makes it easier for your separate parts of your
page, to communicate with other parts of your page. Consider this code.

```
create-container-widget
  parent:content
  position:0
  events
    my-namespace.foo-event
      sys42.confirm-window
        _header:Howdy foo world
        _body:Watch the other label as you click OK!
        _onok
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

The lamba events are working identical to any other dynamically created Active Events, or lambda objects, and can take arguments, and return values
and nodes to the caller. They're simply Active Events, which only lives as long as your widget lives, and only for your page!

Also these lambda events can be dynamically changed, the same way you could with Ajax events, using the *[set-widget-lambda-event]* 
and *[get-widget-lambda-event]*. Both of these Active Events, works similarly to their Ajax events counterparts.

Hint!
You can also use *[list-widget-lambda-events]* and *[list-widget-ajax-events]* to inspect which events are declared for a specific widget.

### [oninit], initializaing your widgets

The *[oninit]* is a special type of event, which is only raised when your widget is initially created. It is useful for initialization and such
of your widget, but besides from its semantics, it works similarly to an Ajax event. Though none of its internals, not even its existence, is ever
rendered back to the client in any ways.

It can be useful for populating a "select" HTML widget, with "option" items, created dynamically from data in your database, for instance. But there
are many other useful scenarios for this event. Semantically, it behaves similar to an Ajax event, but it cannot be raised from the client. And it 
renders no additional data back to the client neither. When *[oninit]* fires, all other events and properties have already been created for your widget.
This allows you to invoke widget lambda events, associated with your widget, and retreieve their properties, from within your *[oninit]* lambda object.

Below is an example of *[oninit]* in action.

```
create-literal-widget
  parent:content
  position:0
  oninit
    set-widget-property:x:/../*/_event?value
      innerValue:Dynamically changed property during [oninit]
      element:h3
```

### Retrieving widgets

There are also several helper Active Events to retrieve widgets, according to some criteria. These are listed below.

* [get-parent-widget] - Returns the parent widget(s) of the specified widget(s)
* [get-children-widgets] - Returns the children widgets of the specified widget(s)
* [find-widget] - Returns the widgets that have the properties listed, with optionally, the values listed
* [find-widget-like] - Same as above, but doesn't require an "exact match", but is happy if the value "contains" the requested value
* [find-first-ancestor-widget] - Returns the first ancestor widget with the specifed properties and values
* [find-first-ancestor-widget-like] - Same as above, but is happy as long as the value "contains" the value(s) specified
* [list-widgets] - List all widgets that have IDs containing the specified string(s)
* [list-widgets-like] - Same as above, but is happy as long as the ID(s) "contains" the value(s) specified
* [widget-exist] - Yields true for each widget that exists matching the specified ID(s)

#### [get-parent-widget]

Returns the parent widget of one or more specified widget(s). Example given below.

```
create-container-widget:foo
  parent:content
  position:0
  widgets
    literal:bar1
      innerValue:bar1
    literal:bar2
      innerValue:bar2
get-parent-widget:x:/../0/**/literal?value
```

The above code, uses an expression leading to each value of each *[literal]* node within the *[widgets]* part of the *[create-container-widget]* 
invocation. This means that it will return the parent widget for both the "bar1" widget and the "bar2" widget, which happens to be the same. 
The output should look something like this, showing the type of widget as the name, and the ID of the parent widget as its value.

```
/* ... rest of code ... */
get-parent-widget
  bar1
    container:foo
  bar2
    container:foo
```

Notice how *[get-parent-widget]* optionally can take an expression as its argument. This means we have to return an additional "layer" before returning
the actual parent widget(s), since we need to return "parent widget to which widget" to the caller. If we had simply returned the parent as the first
node of *[get-parent-widget]*, we wouldn't know who this was a parent to, since we can supply multiple widgets as arguments. This is a general pattern
for many of the widget retrieval events.

#### [get-children-widgets]

Returns the children widgets of one or more specified widget(s). Example below.

```
create-container-widget:foo
  parent:content
  position:0
  widgets
    literal:bar1
      innerValue:bar1
    literal:bar2
      innerValue:bar2
get-children-widgets:foo
```

The above code uses a constant, instead of an expression as our previous example, and should return something like this.

```
/* ... rest of code ... */
get-children-widgets
  foo
    literal:bar1
    literal:bar2
```

Even though we do _not_ use an expression in the above example, we still return the children widgets in a similar structure as we did with our previous
example of *[get-parent-widget]*, which used an expression. This is to make sure we always return data to the caller in the same format. If we had
used an expression leading to multiple source widgets, instead of the constant of "foo", we would have had multiple return nodes beneath *[get-children-widgets]*
in our result.

Notice!
Most of these retrieval Active Events will actually throw an exception if the queried widget does not exist. If you supplied "does-not-exist" to 
your *[get-children-widgets]* invocation for instance, it would throw an exception.

#### [find-widget] and [find-widget-like]

These widgets does not take any arguments like values, besides from (optionally) the "root widget from where to start your search",
but require you to parametrize them with children nodes, having at least a name, and optionally a value. The name of the node, is some attribute 
that must exist on your widget, for it be returned as a "match". The value, is an optionally "value" for that attribute, which the widget must 
either have an exact match of (*[find-widget]*), or "contain" (for the *[find-widget-like]* event).

Let's see an example.

```
create-container-widget:foo
  parent:content
  position:0
  widgets
    literal:bar1
      innerValue:bar1
      class:foo-class
    literal:bar2
      innerValue:bar2
find-widget:foo
  innerValue:bar1
  class:foo-class
find-widget:foo
  innerValue:bar
find-widget-like:foo
  innerValue:bar
```

The above code has two invocations to *[find-widget]* and one to *[find-widget-like]*. The first invocation, requires an exact match for 
the *[innerValue]*, and the *[class]* attributes, being respectively "bar1", and "foo-class". This invocation will return only the "bar1" widget. 
The second invocation won't yield any matches, since it requires an exact match and no widgets have the innerValue of "bar" exactly. 
The third invocation will yield both widgets as its return value, since they both have an *[innerValue]* which _contains_ the text "bar".
The result will look something like this.

```
/* ... rest of code ... */
find-widget
  literal:bar1
find-widget
find-widget-like
  literal:bar1
  literal:bar2
```

Also these Active Events can take expressions as their arguments. However, the currently value iterated for the expression, will not be returned
as previously mentioned. Consider this code.

```
create-container-widget:foo1
  parent:content
  position:0
  widgets
    literal:bar1
      innerValue:bar1
      class:foo-class
    literal:bar2
      innerValue:bar2
create-container-widget:foo2
  parent:content
  position:0
  widgets
    literal:bar3
      innerValue:bar1
      class:foo-class
    literal:bar4
      innerValue:bar2
find-widget:x:/../*/[0,2]?value
  innerValue:bar1
```

Which of course result in this result.

```
/* ... rest of code ... */
find-widget
  literal:bar1
  literal:bar3
```

Notice, with the *[find-widget]* and the *[find-widget-like]* Active Events, the hierarchy is not as important as when you invoke *[get-parent-widget]*
and *[get-children-widgets]*. Hence it does not yield an "additional node", like our first to widget retrieval events did. Notice also that you do not
need to supply a value to these two Active Events. If you don't supply an value, they will both start searching at the "root system widget", which
is the widget with the ID of "cnt", which you can find physically declared in the markup of your Default.aspx page.

#### [find-first-ancestor-widget] and [find-first-ancestor-widget-like]

These two Active Events are similar to the *[find-widget]* events, except of course, instead of searching "downwards" in the hierarchy from the root
widget supplied, they search upwards in the ancestor chain for a match, from the currently iterated widget. These Active Events requires (for obvious 
reasons) the caller to actually supply a value, and will not have a "default" value as our previously mentioned widget retrieval Active Events did.
Example code given below.

```
create-container-widget:foo1
  parent:content
  position:0
  class:foo-class-1
  widgets
    container:bar1
      widgets
        literal:starting-widget-1
          innerValue:Foo bar
create-container-widget:foo2
  parent:content
  position:0
  class:foo-class-2
  widgets
    container:bar2
      widgets
        literal:starting-widget-2
          innerValue:Foo bar
find-first-ancestor-widget-like:x:/../**/literal?value
  class:foo-class
```

Which of course will yield the following result.

```
/* ... rest of code ... */
find-first-ancestor-widget-like
  container:foo1
  container:foo2
```

Notice, that also here, there is no "injection node" for the currently iterated expression value, since it is not as important as in 
the *[get-parent-widget]* and *[get-children-widgets]* events. If you for some reasons require to know the result, associated with the starting 
widget, you should break your invocations up, into multiple invocations, using for instance a *[for-each]* loop, or something similar.

#### [list-widgets] and [list-widgets-like]

These Active Events simply lists all widgets, with an ID matching some (optional) filter supplied. Notice, if you do not supply a filter, this Active
Event will yield every single widget in your page, starting from the root "cnt" widget, declared in your Default.aspx markup. Example code is
given below.

```
create-container-widget:test-foo1
  widgets
    literal:test-bar1
      innerValue:Bar 1
    literal:test-bar2
      innerValue:Bar 2

// Will only return "test-bar2"
list-widgets:test-bar2

// Will not return anything
list-widgets:test

// Will return all three widgets
list-widgets-like:test

// Will return only the two literals
_lit
  test-bar1
  test-bar2
list-widgets:x:/-/*?name
```

The above code should be fairly self explaining at this point ...

One point though, as in many of the other widget retrieval Active Events, is the fact that you are also getting the "type" of widget returned, as
the name of the node - While you get the ID of the widget, returned as the value - Which might be useful sometimes ...

#### [widget-exist]

This Active Event allows you to check for the existence of one or more specified widget(s). It can take either a constant, or an expression.
Below is an example.

```
create-container-widget:test-foo1
  widgets
    literal:test-bar1
      innerValue:Bar 1
    literal:test-bar2
      innerValue:Bar 2
widget-exist:test-bar2
```

Which will yield the following result.

```
/* ... rest of code ... */
widget-exist
  test-bar2:bool:true
```




