Phosphorus Five
===============

Phosphorus Five is a web application framework for Mono and ASP.NET. Phosphorus 
is built around an entirely new axiom for system development based upon a new
design patter called *"Active Events"*. Active Events, and its off spring,
p5.lambda, a natural consequence of Active Events, opens up an entirely new 
doorway to architecting your software solutions.

p5 contains a managed Ajax library, allowing you to automatically map up the
bridge between the server and the client. It also contains p5.lambda, a 
*"non-programming language"*, allowing you to orchestrate your applications
building blocks together, in an extremely loosely manner. In addition, p5 contains
Hyperlisp, which is a tree structure file-format, allowing you to easily create
p5.lambda execution trees, and combine building blocks together, to form your 
end software.

The last and fifth piece in p5, is p5.lambda expressions, allowing for you to
extract and modify tree structures (such as p5.lambda nodes) using a 
Hyperdimensional Boolean Algebraic Graph Object Expression query language. The
above definition might be a mouthful for you, however, think; *"SQL for Tree 
Structures"*, and you've got the point.

All these building blocks combined, just so happens to largely solve all of your
web application software problems, making all the *"boring"* parts just happen,
leaving the fun parts to you. The end result being an extremely Agile software 
solution, facilitating for interchanging any building block as you maintain,
which results in you *"orchestrating"* or *"growing"* your software, as a conductor
or a gardener, rather than having to do all the nitty stuff yourself by hand. In
addition it facilitates for you to become extremely capable of *"reusing"* your
code, at an entirely new level, highly unlikely matched in any other frameworks
out there you've seen previously.

If you wish to use buzz words, and go all hype, then maybe *"fifth generation 
non-programming language"* might apply ... ;)

## Getting started with p5.lambda and Hyperlisp

p5.lambda is a *"non-programming language"* created on top of Active Events. At its core,
it is really nothing but loosely coupled Active Events, capable of executing
rudimentary programming language constructs, such as *"while"*, *"for-each"*, *"if"*
and so on. This makes p5.lambda extremely extendible, and allows for you to change
and/or add any keyword(s) you wish to the language, by creating your own Active Events.
This creates an excellent encironment for you to create your own *"domain-specific
language"* for your own applications.

p5.lambda is very easy to learn, since it doesn't contain more than ~15 keywords, 
and should be used in symbiosis with your own Active Events, to provide an environment
where you can *"orchestrate"* your applications together, by combining existing C#
logic together, to form an *"application"* based upon loosely coupled plugins and
modules.

p5.lambda can use any file format capable of constructing key/value/children nodes
as its *"language"*, such as XML or JSON, but by default, a file format called
*"Hyperlisp"* is being used. Below is a simple p5.lambda program written in Hyperlisp;

```
_x
  foo1:hello
  foo2:there
  foo3:stranger
  foo4:do you wish to become my friend?
for-each:x:/./_x/*/?node
  p5.console.write-line:"{0}:{1}"
    :x:/././__dp/#/?name
    :x:/././__dp/#/?value
```

The above example assumes a handler for *"p5.console.write-line"*, which is implemented
in the lambda.exe file (phoshorus.exe project), which allows for executing p5.lambda 
files and code directly from a command line shell.

If you create a file called *"hello.hl"* which you save in the same directory as you
have your *"lambda.exe"* file (your p5/lib folder), you can execute the above application
using *"mono lambda.exe -f hello.hl"* from a command line shell on linux or OSX. Drop 
the *"mono"* parts if you're on windows and you're using .Net.

Notice how each consecutive double-space in front of a line creates a new children
collection of nodes beneath the node above itself. For instance, the *"p5.console.write-line"*
statement above is a child node of the *"for-each"* Node. The above code perfectly 
translates into a Node structure with all *"foox"* being children nodes of the *"_x"*
Node, and so on. This just so happens to become a tree structure once parsed by the 
Hyperlisp parser. At the same time, it is also a linear/sequential data structure, due
to each new node, starting on a new line. This allows us to *"map"* a tree structure
out for the p5.lambda execution engine, using a linear and human readable approach,
far easier to read and maintain than for instance both XML and JSON.

Hyperlisp also supports most *"System.x"* types from .Net, such as *"_x:int:5"* 
turning the *_x* Node into having the integer value of "5", instead of its string 
representation. Hyperlisp is also extendible in its type system, allowing you to 
create handlers for your own types to be serialized into Hyperlisp. Which makes
Hyperlisp a *"better BSON than BSON"* one might argue.

Notice how there is no difference between *"data"* and *"code"* from a semantic point
of view in p5.lambda and Hyperlisp. There is also no difference between *"methods"* 
and *"keywords"* in p5.lambda. This might for the experienced developer seem like
a disadvantage, likely leading to unmaintainable code, and cluttered software 
solutions, but actually the exact opposite is true.

By unlearning everything you think you knew about software, and attack it from an
entirely new angle, you will actually discover how the *"everything is one"* 
approach in p5, opens up an entirely new doorway to thinking about software, far
superior to all previously learned conventions and best practice. For instance,
there are no *"design patterns"* in p5.lambda, and neither are any design patterns
necessary.

## Getting started with Active Events

Active Events are an alternative to OOP which facilitates for better encapsulation
and plugable software than traditional inheritance through its classes and interfaces.

Instead of inheriting from a class, and override methods from the base class, you
can directly override and replace any Active Event with any other Active Event.
This makes it easier for you to create plugins and modules that have no 
dependencies.

Below is an example of how you can create a static Active Event;

```csharp
using p5.core;

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

In addition, it allows you to change a method invocation the same way you'd 
change a simple C# string. This means you can store *"method hooks"* in your 
database, config file, allowing the user to type them in through a GUI, etc.

This feature just so happens to largely replace most features from Object 
Oriented Programming, with a much better alternative for acomplishing the 
same things, such as encapsulation and polymorphism. In addition, Active 
Events is at the core of p5.lambda, allowing for creation of a 
*"non-programming language"*, largely replacing 98% of all your code, with 
a much more flexible, maintainable and easy to understand construct, than 
your *"code"* such as C# and Java. Hyperlisp and p5.lambda is basically 
nothing but a thin abstraction on top of Active Events in fact.

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
your application.

If you have an instance Active Event (not static method), then you need to use the
*"RegisterListeningObject"* method on your *"ApplicationContext"* object for each 
instance of your class you wish to have notified when the Active Event is raised.

Although P5 is largely focused on web application development, there is nothing
stopping you from using p5.core, p5.exp, p5.lambda, and the rest of the plugins
in for instance a WinForms application, or any other type of app you can build using
Mono and/or .Net. Which allows you to use Active Events in for instance desktop
applications.

## Getting started with phosphorus.ajax

If you wish, you can of course use only p5.ajax in existing or new web forms apps.
Create a reference to *"lib/phosphorus.ajax.dll"* in your asp.net web application

Then modify your web.config, and make sure it has something like this inside its 
*"system.web"* section to recognize the Phosphorus.Ajax controls.

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

```xml
<p5:Literal
    runat="server"
    id="hello"
    Tag="strong"
    onclick="hello_onclick">
    click me
</p5:Literal>
```

Then add the following code in your codebehind

```csharp
using p5.ajax.core;
using p5 = p5.ajax.widgets;

/* ... rest of class ... */

[WebMethod]
protected void hello_onclick (p5.Literal sender, EventArgs e)
{
    // notice how you save a cast operation here ...
    sender.innerHTML = "hello world";
}

/* ... */
```

If you wish to have more samples for how to use phosphorus.ajax, you can check out the 
*"p5.ajax-samples"* project by opening up the *"p5.sln"* file.

## The literal, container and void widgets

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
dynamically added to it during execution of your page, and add these back up 
automatically during execution.

The **Void** widget cannot have neither content nor child controls, and is for HTML
elements such as *"input"* for instance, which has no content at all.

Altough the comparison does not do justify the features of the phosphorus widgets, 
you can think of the Literal widget as the *"Label"* equivalent, and the Container 
widget as the *"Panel"* equivalent. Whenever you need *static text* you should use
the **Literal** control, and whenever you need complex controls, having child
controls, you should use the **Container** control.

#### Modifying your widgets

The first thing you have to decide when creating a widget, is what html tag you wish 
to render it with. This is set through the *"Tag"* property of your widget. You can 
render any widget with any html tag you wish, but remember that you have to make sure 
what you're rendering is html compliant. p5.ajax supports the html5 standard 
100%, but it also supports the html500 standard, even though nobody knows how that 
looks like today.

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
<p5:Literal
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
</p5:Literal>
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
p5.ajax will automatically keep track of what needs to be sent from the 
server to the client. Use the *"RemoveAttribute"* method to remove an attribute,
since setting an attribute value to null will not remove it, but keep the attribute
with a null value.

## Building your own release

On Linux, building a release is easy, you simply choose "Release-Linux" as your project
configuration, rebuild your solution, for then to choose "Tools/Deploy to Web" by 
right-clicking "phosphorus.application-pool". To create a release, you must make sure 
you have "uglifyjs" installed on your system, since the Linux Release configuration 
of phosphorus.ajax depends upon uglifyjs to minify its javascript.

## Creating an Ajax widget hierarchy with p5.lambda

Below is an example of a piece of p5.lambda code that allows you to create an
Ajax widget, which once clicked, creates another Ajax widget, and injects it below
your first widget.

```
p5.web.create-widget:foo
  class:span-24 last
  widget:literal
  element:h1
  innerValue:"click me!"
  onclick
    p5.web.create-widget
      widget:literal
      element:p
      class:span-18 prepend-6 last
      innerValue:"I was created dynamically. Click me too!"
      after:foo
      onclick
        p5.web.set-widget-property:@/../*/_widget?value
          style:"background-color:LightBlue"
```

## License

The MIT License (MIT)

Copyright (c) 2014-2015 Thomas Hansen, phosphorusfive@gmail.com

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
