p5.ajax, a managed Ajax library
========

Contains the main Ajax library for Phosphorus Five. This Ajax library can be consumed
either stand alone, in any ASP.NET Web Forms application, since its controls inherit from
_System.Web.UI.Control_, or indirectly, through using the _p5.web_ library, allowing
you to create controls with Active Events, using for instance Hyperlisp as your programming 
language.

In general there are only three controls, giving you 100% control over your page's HTML
structure;

* Container - A "panel" type of widget, having children widgets of itself
* Literal - A web control containing text, and/or HTML as its content
* Void - A web control with neither children controls, nor textual fragment content

To create a property for your widgets, simply use the subscript operator, with
the name being the property name, and its value being the value.

## Example usage in C#

Create a reference to *"lib/phosphorus.ajax.dll"* in your ASP.NET Web Forms application,
and make sure you have a "Default.aspx" page.

Then modify your web.config, and make sure it has something like this inside its 
*"system.web"* section to recognize the p5.ajax controls.

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

Then either inherit your page from AjaxPage, or implement the IAjaxPage interface, 
before you create a literal widget, by adding the code below in your .aspx markup.
The easiest is to inherit from AjaxPage directly.

```xml
<p5:Literal
    runat="server"
    id="hello"
    Element="strong"
    onclick="hello_onclick">
    click me
</p5:Literal>
<p5:Container
    runat="server"
    id="hello_ul"
    Element="ul"
    click me
</p5:Container>
```

Then add the following code in your codebehind

```csharp
using p5.ajax.core;
using p5.= p5.ajax.widgets;

/* ... rest of class ... */

p5.Container hello_ul;

[WebMethod]
protected void hello_onclick (p5.Literal sender, EventArgs e)
{
    // Notice, no cast here ...
    sender.innerValue = "hello world";
    sender["style"] = "background-color:LightBlue;";

    // Dynamically adding a new "li" HTML element to our ul container.
    // Notice, if we want our Container control to take care of persistence
    // of our newly created child control, we must create it through the Container's
    // factory method, indirectly!
    Literal lit = hello_ul.CreatePersistentControl<Literal> ();
    lit.Element = "li";
    lit["class"] = "some-class-value";
    lit.innerValue = "Item no; " + hello_ul.Controls.Count;
}

/* ... */
```

## Structure of p5.ajax

All three widgets described above, inherit from the "Widget" class. This class takes
care of property creation, deletion and so on. The "Container" class has a stateful
Controls hierarchy, allowing it to automatically serialize and de-serialize its children
collection upon postbacks. This allows you to add children controls to it, dynamically,
having the control "remember" its children controls upon consecutive callbacks.

This project shows you how to use only the Ajax library itself, without any of the 
Active Events and/or Hyperlisp parts of Phosphorus Five.

Due to the automatic attribute serialization de-serialization, and stateful Container
controls, using p5.ajax is dead simple. Simply add and/or change/remove any attribute
you wish from your controls, and have the underlaying library take care of the automatic 
changes being propagated back to the client.

p5.ajax uses JSON internally to serialize values back and forth, and rarely needs to
entirely re-render any controls, using "partial rendering". You can also inject any
new control, in a postback or Ajax callback, at any position in your page, as long
as its parent control is of type "Container".

## More example code

To see an example of the p5.ajax library in use directly, without anything else but the
Ajax library itself, please see the website project called "p5.ajax-samples".


