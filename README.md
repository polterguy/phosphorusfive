Phosphorus-Five
===============

Phosphorus-Five is a web application framework for Mono and ASP.NET. Phosphorus 
adds the fun back into creating web apps, and allows you to build software that 
can run unchanged for millenniums. Phosphorus is;

* Secure
* Lightweight
* Beautiful
* Flexible
* Intuitive

#### Secure

The web, and especially javascript, is insecure by design. Phosphorus fixes this, 
by making sure all your business logic stays on your server, where it is safe from 
hackers. Phosphorus also ties everything down by default, and makes your systems 
safe against intrusions. Phosphorus also implements all security measures out of
the box, such as encryption of REST web service invocations, and allowing you to
send encrypted emails, etc.

*"Phosphorus makes you sleep like a baby"*

#### Lightweight

Phosphorus is lightweight in all regards. The javascript sent to the client is 
tiny, there is no unnecessary html rendered, the http traffic is tiny, and the 
server is not clogged with resource intensive functionality. Phosphorus solves 
the repetetive tasks you need to solve when you create software, nothing more.
Phosphorus is medicine against bloatware. This is the reason why it is named 
after a Homeopathic medicine in fact.

*"With Phosphorus, your solutions scales to the stars"*

#### Beautiful

Phosphorus features beautiful code, and allows you to create beautiful code yourself.  
The class hierarchy is easy to understand. The javascript and html rendered is easy 
to read, and conforms to all known standards. Phosphorus facilitates for you 
creating beautiful end results.

*"With Phosphorus, you create beautiful babies"*

#### Flexible

Phosphorus is highly flexible. It allows you to easily create your own logic, 
overriding what you need to override, and not worry about the rest. With Phosphorus, 
you decide what html and javascript is being rendered back to the client, and how 
your class hierarchy is designed.

*"With phosphorus, you are in charge"*

#### Intuitive

Phosphorus is easy to understand, and contains few things you do not already know how 
to use. It builds on top of ASP.NET, C#, html, javascript and json.  if you know C#, 
ASP.NET, and have used libraries such as jQuery or prototype.js before, then you 
really don't need to learn much new things to get started.

*"With phosphorus, your first hunch is probably right"*

## Getting started with phosphorus.ajax

Create a reference to *"lib/phosphorus.ajax.dll"* in your asp.net web application

Then modify your web.config, and make sure it has something like this inside its 
*"system.web"* section to recognize the Phosphorus.Ajax controls.

```xml
<system.web>
  <pages>
    <controls>
      <add 
        assembly="pf.ajax" 
        namespace="pf.ajax.widgets" 
        tagPrefix="pf" />
    </controls>
  </pages>
</system.web>
```

Then either inherit your page from AjaxPage, or implement the IAjaxPage interface, 
before you create a literal widget, by adding the code below in your .aspx markup.

```xml
<pf:Literal
    runat="server"
    id="hello"
    Tag="strong"
    onclick="hello_onclick">
    click me
</pf:Literal>
```

Then add the following code in your codebehind

```csharp
using pf.ajax.core;
using pf = pf.ajax.widgets;

/* ... rest of class ... */

[WebMethod]
protected void hello_onclick (pf.Literal sender, EventArgs e)
{
    // notice how you save a cast operation here ...
    sender.innerHTML = "hello world";
}

/* ... */
```

If you wish to have more samples for how to use phosphorus.ajax, you can check out the 
*"phosphorus.ajax.samples"* project by opening up the *"phosphorus.sln"* file.

## The literal and container widgets

In phosphorus.ajax there is only two types of web controls. There is the *"Literal"* 
class, and the *"Container"* class. By cleverly combining these two classes however, 
you can create any html markup you wish.

The **Literal** widget is for controls that contains text or html, and allows you to 
change its content through the *"innerHTML"* property. Notice that the literal widget 
can have html elements inside of it, but these will be treated as client side html, 
and not possible to change on the server side, except by modifying the html as text.
Everything inside of the beginning and the end of your literal widget in 
your .aspx markup will be treated as pure html/text, and not parsed as controls in
any ways.

The **Container** widget can have child controls, which will be parsed in the .aspx 
markup as controls, and possible to reference on the server side, and modify 
in your server side code through its *"Controls"* collection. Everything inside of 
the beginning and the end of your container widget in your .aspx markup, will be 
treated as web controls. A Container widget will automatically keep track of controls
dynamically added to it during execution of your page, and add these back up dynamically
during execution.

Altough the comparison does not do justify the features of the phosphorus widgets, 
you can think of the Literal widget as the *"Label"* equivalent, and the Container 
widget as the *"Panel"* equivalent. Whenever you need *static text* you should use
the **Literal** control, and whenever you need complex controls, having child
controls, you should use the **Container** control.

#### Modifying your widgets

The first thing you have to decide when creating a widget, is what html tag you wish 
to render it with. This is set through the *"Tag"* property of your widget. You can 
render any widget with any html tag you wish, but remember that you have to make sure 
what you're rendering is html compliant. Phosphorus.ajax supports the html5 standard 
100%, but it also supports the html500 standard, even though nobody knows how that 
looks like today. It is probably wise to stick to the html5 standard for now.

Adding attributes to your widgets is easily done by simply adding any attribute you 
wish, either directly in the markup of your .aspx page, or by using the index operator 
overload in c#. The framework will automatically take care of serializing your 
attributes correctly back to the client. Any attribute starting with *"on"* will
automatically be assumed to be a server side event reference, unless it is contains
characters not legal in C# method names, at which point it will be considered to
be a piece of javascript to be executed upon the given event being raised in the DOM.

Attribute values changed during execution of your page, will automatically be 
serialized back to the client, and kept track of. Only the actual changes will be 
serialized this way.

Below is an example of how to create a video html5 element using a literal widget;

```xml
<pf:Literal
    runat="server"
    id="video"
    Tag="video"
    width="640"
    onclick="video_click"
    controls>
    <source 
        src="http://download.blender.org/peach/trailer/trailer_1080p.ogg" 
        type="video/ogg" />
    your browser blows!
</pf:Literal>
```

You can modify or add any attribute you wish in the codebehind by using something 
like this, and the engine will automatically keep track of which items are dirty and 
needs to be sent back to the client.

```csharp
[WebMethod]
protected void video_click (Literal literal, EventArgs e)
{
    literal ["width"] = "1024";
}
```

You can modify any attribute you wish on your widgets, by using the index operator.  
Phosphorus.ajax will automatically keep track of what needs to be sent from the 
server to the client. Use the *"RemoveAttribute"* method to remove an attribute,
since setting an attribute value to null will not remove it, but keep the attribute
with a null value.

## Getting started with Active Events

Active Events are an alternative to OOP which facilitates for better encapsulation
and plugable software than traditional inheritance through its classes and interfaces.

Instead of inheriting from a class, and override methods from the base class, you
can directly override and replace any Active Event with any other Active Event.
This makes it easier for you to create plugins and modules that have no 
dependencies.

Below is an example of how you can create a static Active Event;

```csharp
using pf.core;

[ActiveEvent (Name = "foo")]
protected static void foo_method (ApplicationContext literal, ActiveEventArgs e)
{
    /* Do stuff with e.Args here.
       Notice that e.Args contains a reference to a Node, which
       allows you to pass in and return any number and/or types 
       of arguments you wish */
}
```

With Active Events you don't invoke the method directly, but rather indirectly,
through its *"Name"* property declared in your *"ActiveEvent"* attribute. This
means that you never have dependencies between the caller and the implementor of
your functions/methods/Active Events. This allows you to dynamically easily 
replace any functionality in your system, with other modules and pieces of 
functionality, without needing the caller and invoker to know anything about 
each other.

To raise an Active Event, you use the *"Raise"* method on your ApplicationContext
object, passing in a Node, which will become the arguments passed into your Active
Event.

```csharp
ApplicationContext ctx = Loader.Instance.CreateApplicationContext ();
Node node = new Node ();
ctx.Raise ("foo", node);
```

Then from inside your Active Event, you can either extract arguments passed in
to your method, or return arguments, by modifying and retrieving values from your
Node. The Node class is actually a tree structure, capable of passing in any type
of argument you wish.

You can dynamically load assemblies into your application, through the
*"Loader.Instance.LoadAssembly"* method to *inject* new Active Event handlers into
your application pool.

If you have an instance Active Event (not static method), then you need to use the
*"RegisterListeningObject"* method on your *"ApplicationContext"* object for each 
instance of your class you wish to have notified when the Active Event is raised.

## About pf.lambda

pf.lambda is a *"programming language"* created on top of Active Events. At its core,
it is really nothing but loosely coupled Active Events, capable of executing
rudimentary programming language constructs, such as *"while"*, *"for-each"*, *"if"*
and so on. This makes pf.lambda extremely extendible, and allows for you to change
and add any keyword(s) you wish to the language, by creating your own Active Events.

pf.lambda is very easy to learn, since it doesn't contain more than ~15 keywords, 
and should be used in symbiosis with your own Active Events, to provide an environment
where you can *"orchestrate"* your applications together, by combining existing C#
logic together, to form an *"application"* based upon loosely coupled plugins and
modules.

pf.lambda can use any file format capable of constructing key/value/children nodes
as its *"language"*, such as XML or JSON, but by default, a file format called
*"Hyperlisp"* is being used. Below is a simple pf.lambda program written in Hyperlisp;

```
_x
  foo1:bar1
  foo2:bar
  foo3:bar
for-each:@/./_x/*/?node
  pf.console.write-line:"{0}:{1}"
    :@/././__dp/#/?name
    :@/././__dp/#/?value
```

The above example assumes a handler for *"pf.console.write-line"*, which is implemented
in the lambda.exe file (phoshorus.exe project), which allows for executing pf.lambda 
files and code directly from a command line shell.

Notice how each consecutive double-space in front of a line creates a new children
collection of nodes beneath the node above itself. For instance, the *"pf.console.write-line"*
statement above is a child node of the *"for-each"* Node. The above code perfectly 
translates into a Node structure with all *"foox"* being children nodes of the *"_x"*
Node, and so on.

Hyperlisp also supports most *"System.x"* types from .Net, such as *"_x:int:5"* 
turning the *_x* Node into having the integer value of "5", instead of its string 
representation. Hyperlisp is also extendible in its type system, allowing you to 
create handlers for your own types to be serialized into Hyperlisp.

## Building your own release

On Linux, building a release is easy, you simply choose "Release-Linux" as your project
configuration, rebuild your solution, for then to choose "Tools/Deploy to Web" by 
right-clicking "phosphorus.application-pool". To create a release, you must make sure 
you have "uglifyjs" installed on your system, since the Linux Release configuration 
of phosphorus.ajax depends upon uglifyjs to minify its javascript.

## Creating an Ajax widget hierarchy with pf.lambda

Below is an example of a piece of pf.lambda code that allows you to create an
Ajax widget, which once clicked, creates another Ajax widget, and injects it below
your first widget.

```
pf.web.create-widget:foo
  class:span-24 last
  widget:literal
  element:h1
  innerValue:"click me!"
  onclick
    pf.web.create-widget
      widget:literal
      element:p
      class:span-18 prepend-6 last
      innerValue:"I was created dynamically. Click me too!"
      after:foo
      onclick
        pf.web.set-widget-property:@/../*/_widget?value
          style:"background-color:LightBlue"
```

## License

The MIT License (MIT)

Copyright (c) 2014-2015 Thomas Hansen, isa.lightbringer@gmail.com

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
