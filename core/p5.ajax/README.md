A Managed Ajax library for ASP.NET and C#
========

This project contains the main Ajax library for Phosphorus Five. This Ajax library can be consumed
either stand alone, in any ASP.NET Web Forms application, since its controls inherit from
_System.Web.UI.Control_ - Or indirectly, through using the [p5.web library](/plugins/p5.web/), 
allowing you to create controls with Active Events, using for instance Hyperlambda as your programming 
language.

The Hello World example application at the [root of this documentation](https://github.com/polterguy/phosphorusfive), 
actually consumes this library indirectly, by invoking the *[p5.web.widgets.create-literal]* Active Event, which internally
creates a _"Literal"_ widget for you, and injects it into your page.

In our documentation here, we will assume some C# knowledge, and create our examples, as _"pure"_ ASP.NET/C#
examples. If you wish to see it further abstracted, the way you'd probably normally use it, in combination 
with Hyperlambda, I encourage you to rather check out the documentation for [p5.web](/plugins/p5.web/).

## The trinity of widgets

In general there are only 3 Ajax controls, or _"widgets"_, as we usually refer to them as, giving you 100% 
control over your page's rendered HTML;

* Literal - An Ajax widget containing text, and/or HTML as its content through *[innerValue]*.
* Container - A _"panel"_ type of widget, having children widgets of itself through *[widgets]*.
* Void - An Ajax widget with neither children widgets, nor text/HTML content.

To create an attribute for your widget, simply use the subscript operator, with
the name being the attribute name, and its value being the value. To change the HTML 
element a widget is rendered with, simply change its `Element` property. An example of creating a `Literal` widget
with a CSS `class` attribute, an inner value containing some text, rendered with the _"address"_ HTML element, can be found below.

```csharp
var lit = new Literal ();
lit.innerValue = "John Doe, Cypress Hill 57, 98765 California, US"
lit.Element = "address";
lit ["class"] = "address-css-class";
```

The above two traits of p5.ajax, allows you to aquire 100% perfect control over what
HTML is rendered to your clients. And since the `Container` widget allows you to
persistently add and remove widgets from its children `Controls` collection, this allows
you to create any type of markup you wish, dynamically building up your website's HTML,
exactly as you need.

## Where the Ajax TreeView/DataGrid/"insert-your-complex-control-here"

p5.ajax does not have a _"Tree Ajax widget"_ or a _"DataGrid Ajax widget"_, simply
because it is not its responsibility to create such Controls/widgets. However, to
create such extension widgets, utilizing the 3 pre-existing widgets from p5.ajax,
would be very easy, and could easily be done, without even having to resort to JavaScript, if you wish.

In fact, such an Ajax TreeView widget actually exists in System42, created exclusively
using the 3 native widgets from p5.ajax. To see it in action, feel free to check out the documentation
for the [Ajax TreeView widget](/core/p5.webapp/system42/components/common-widgets/tree).

## No more keeping track of your controls

The Container widget, will automatically track its children widgets collection,
as long as you use the `CreatePersistentControl`, and `RemoveControlPersistent` methods
for adding and removing widgets from its `Controls` collection.

This means, that you do not need to re-create its widgets collection upon postbacks or callbacks,
since it'll keep track of whatever widgets it contains, at any specific point in time,
automatically for you.

You can add and remove any widgets you wish from a Container widget, as
long as you use the above mentioned methods. The widgets will be automatically
re-created upon every postback to your server. Which of course, for beginners in 
ASP.NET, entirely removes the burdon of having to keep track of which widgets 
have been previously created.

## Example usage

1. Create an empty ASP.NET Web Forms website project.
2. Create a reference to _"p5.ajax.dll"_ in your ASP.NET Web Forms application, and make sure 
you have a _"Default.aspx"_ page.
3. Modify your web.config, and make sure it has something like this inside its _"system.web"_ 
section, to recognize the p5.ajax controls.

```xml
<system.web>
  <pages>
    <controls>
      <add 
        assembly="p5.ajax" 
        namespace="p5.ajax.widgets" 
        tagPrefix="p5" />
    </controls>
  </pages>
</system.web>
```

Then inherit your page from AjaxPage, before you create a `Literal` widget, and a `Container` widget, 
by adding the code below into your Default.aspx, somewhere inside of its form declaration.

```xml
<p5:Literal
    runat="server"
    id="hello"
    Element="button"
    onclick="hello_onclick">
    Click me!
</p5:Literal>
<p5:Container
    runat="server"
    id="hello_ul"
    Visible="false"
    Element="ul" />
```

Add the following code in your C# codebehind.

```csharp
using p5.ajax.core;
using p5 = p5.ajax.widgets;

/* ... rest of class ... */

protected p5.Container hello_ul;

[WebMethod]
protected void hello_onclick (p5.Literal sender, EventArgs e)
{
    // Notice, no cast here ...
    sender.innerValue = "Hello World!";
    sender["style"] = "background-color:LightBlue;";

    // Dynamically adding a new "li" HTML element to our ul container.
    // Notice, if we want our Container control to take care of persistence
    // of our newly created child control, we must create it through the Container's
    // factory method, indirectly!
    var lit = hello_ul.CreatePersistentControl<Literal> ();
    lit.Element = "li";
    lit ["class"] = "some-class-value";
    lit.innerValue = "Item no; " + hello_ul.Controls.Count;

    // Making sure "ul" element becomes visible.
    hello_ul.Visible = true;
}

/* ... */
```

For security reasons, you must explicitly mark your server-side Ajax methods with the [WebMethod] attribute. 
Besides from that, they work similarly to how a server side event works in ASP.NET. Although, you can create
events for _anything_ in p5.ajax, as long as you start your attribute's name with _"on"_.

## Structure of p5.ajax

All 3 widgets described above, inherit from the `Widget` class. This class takes
care of attribute creation, deletion, and so on. And all attributes added to any control,
will be automatically remembered across postbacks.

Due to the automatic attribute serialization de-serialization, and stateful Container
controls, using p5.ajax is extremely easy. Simply add and/or change/remove any attribute
you wish from your controls, and have the underlaying library take care of the automatic 
changes being returned back to the client.

To add an attribute, or change its value, is as simple as this.

```csharp
// Notice, this does NOT render as "valid" HTML! But it proves the point!
myWidget ["some-attribute"] = "some-value";
```

To delete an attribute, simply use the `RemoveAttribute` method on your widget, and pass in 
the name of the attribute you wish to remove.

## Change is the only constant

In p5.ajax, everything can be changed during an Ajax request. As we've seen in previous
parts of this documentation, the Container widget keeps track of its children controls.
But also all other parts of a widget is automatically kept track of during execution
of your page. In fact, you can even change the element a widget is rendered with, 
dynamically - And even its ID - And p5.ajax will automatically take care of everything
that changes, and render the correct HTML/JSON back to the client.

Imagine you have a _"myWidget"_ on your page, which can be any of the 3 existing types
of widgets, and then you do something like this in one of your web methods.

```csharp
myWidget.Element = "div";
```

The above would change your widget's HTML element, and persist (remember) the change for 
every consecutive callback.

## JSON

p5.ajax uses JSON internally to return updates back to the client. This means, that among
other things, it rarely needs to re-render your controls, using _"partial rendering"_. You can 
also inject any new control, in a postback or Ajax callback, at any position in your 
page, as long as its parent control, is of type `Container`, and you use the appropriate factory 
method to create your controls.

The JSON parts of p5.ajax, means that the bandwidth consumption when consuming the library,
is ridiculously small, compared to frameworks built around _"partial rendering"_, which requires
some portion of the page, to be partially re-rendered.

This also makes the client-side JavaScript parts of P5 tiny compared to other Ajax frameworks.
In its release build, minified and GZipped, the entire JavaScript parts in p5.ajax, is actually 
less than 3KB in total.

## ViewState

The ViewState is bye, bye in p5.ajax. Or rather, to be accurate, it is kept, but on
the server. This means that the amount of data sent back and forth between your clients and your server, 
is probably less than a fraction of that which you are used to from other Ajax frameworks, such as 
ASP.NET Ajax, and similar frameworks.

The above comes with some consequences though, which is that the resource consumption on the
server, increases for each session object that connects to it simultaneously. To prevent a single
client from exhausting your server's resources entirely, p5.ajax stores only the last 5 used ViewState
page objects per session.

Out of the box, without tweaking the library, this means that it is best suited
for building single page web _"apps"_, where you don't have an extreme amount of simultaneous
users, and most users would only request one or two pages at the same time. As a general rule, 
I'd suggest using p5.ajax for building enterprise apps, without too many simultaneous users, 
and not for instance; Social media websites, such as Facebook or Twitter, for the above mentioned reasons.

Another consequence, is that (by default), only 5 simultaneous tabs are tolerated
at the same time, for each session object. If the user opens up a sixth tab,
and/or refreshes one of his pages more than 5 times, then the _"oldest"_ ViewState key
is invalidated, and the next Ajax request towards the server, with this key, will be rejected, 
due to a non-valid ViewState lookup. If this was not the case, a single
session (user), could exhaust your server's resources entirely, by simply refreshing his webpage,
thousands of times.

Have this in mind as you do your p5.ajax development. The library is best suited for
single page web apps, with complex and rich UI, doing lots of Ajax requests.
It is not as well suited for apps, where you have many tabs open at the same time,
or thousands of simultaneous users, such as for instance would often be the case 
with social media websites, such as StackOverflow, Facebook or Twitter etc ...

As a general rule, I encourage people to use it to build enterprise apps, while
use something else to build websites, requiring millions, of simultaneous users.
Although p5.ajax could be tweaked to handle also such scenarios.

The ViewState logic _can_ be overridden though, by implementing your own IAjaxPage,
and/or tweaking your web.config.

## More example code

To see an example of the p5.ajax library in use directly, without anything else but the
Ajax library itself, please check out the website project called [p5.ajax-samples](/samples/p5.ajax-samples/).
To test this project, make sure you set it as your startup project in Visual Studio, Xamarin or MonoDevelop, 
and start your debugger.

## Conclusion

The above traits of p5.ajax, meaning that it keeps its state, is a perfect match for [p5.web](/plugins/p5.web/).
This allows you to simply _"declare"_ your widget hierarchy, for then to in your own server side events, modify
any widget's state, including adding controls (widgets) to your page. Which results in a ridiculously simple
to understand development model, without compromising power in any ways.

In addition, by only having 3 different controls, allows you to easily create your own custom extensions, exactly
as you see fit. Instead of being forced to use an _"Ajax DataGrid"_ or an _"Ajax TreeView"_ as I for some reasons
feel for implementing it.

Building up complex and rich hierarchies of controls, rendered with the elements you wish, having the attributes
and/or events you see fit, is as easy as a walk in the park. And none of the code necessary for wiring up things 
becomes a burdon to you, since all Ajax functionality, and resource inclusions, simply _"happens automatically"_.

A testimonial towards this trait, is the fact of that even though p5.ajax depends upon ASP.NET, and is entirely
built on top of the _"ASP.NET Page Life cycle"_, you never even once have to worry about this, and simply add/remove/change
whatever you wish, in which ever server-side event you feel for doing such a thing.

When combining this with [p5.lambda](/plugins/p5.lambda/) and [p5.web](/plugins/p5.web/), development of really
rich UI and UX apps becomes so easy, that you would highly likely be very pleasently surprised as you dive into it.
