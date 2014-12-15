phosphorus five
===============

phosphorus five is a web application framework for mono and asp.net.  phosphorus 
adds the fun back into creating web apps, and allows you to build software that 
can run unchanged for millenniums.  phosphorus is;

* secure
* lightweight
* beautiful
* flexible
* intuitive

#### secure

the web, and especially javascript, is insecure by design.  phosphorus fixes this, 
by making sure all your business logic stays on your server, where it is safe from 
hackers.  phosphorus also ties everything down by default, and makes your systems 
safe against intrusions

*"with phosphorus five, you sleep at night"*

#### lightweight

phosphorus is lightweight in all regards.  the javascript sent to the client is 
tiny, there is no unnecessary html rendered, the http traffic is tiny, and the 
server is not clogged with resource intensive functionality.  phosphorus solves 
the problems you need to solve, nothing more.  phosphorus is medicine against 
bloatware

*"with phosphorus five, your creations can grow into heaven"*

#### beautiful

phosphorus features beautiful code, and allows you to create beautiful code yourself.  
the class hierarchy is easy to understand.  the javascript and html rendered is easy 
to read, and conforms to all known standards.  phosphorus facilitates for you 
creating beautiful end results

*"with phosphorus five, your creations can be beautiful"*

#### flexible

phosphorus is highly flexible.  it allows you to easily create your own logic, 
overriding what you need to override, and not worry about the rest.  with phosphorus, 
you decide what html and javascript is being rendered back to the client and how 
your class hierarchy should be designed

*"with phosphorus five, you are the boss"*

#### intuitive

phosphorus is easy to understand, and contains few things you do not already know how 
to use.  it builds on top of asp.net, c#, html, javascript and json.  if you know c#, 
asp.net, and have used libraries such as jQuery or prototype.js before, then you 
don't need to learn anything new to get started

*"with phosphorus five, your first hunch is probably right"*

## getting started with phosphorus.ajax

create a reference to *"lib/phosphorus.ajax.dll"* in your asp.net web application

then modify your web.config, and make sure it has something like this inside its 
*"system.web"* section

```xml
<system.web>
  <pages>
    <controls>
      <add 
        assembly="phosphorus.ajax" 
        namespace="phosphorus.ajax.widgets" 
        tagPrefix="pf" />
    </controls>
  </pages>
</system.web>
```

then either inherit your page from AjaxPage, or implement the IAjaxPage interface, 
before you create a literal widget, by adding the code below in your .aspx markup

```xml
<pf:Literal
    runat="server"
    id="hello"
    Tag="strong"
    onclick="hello_onclick">
    click me
</pf:Literal>
```

then add the following code in your codebehind

```csharp
using phosphorus.ajax.core;
using pf = phosphorus.ajax.widgets;

/* ... */

[WebMethod]
protected void hello_onclick (pf.Literal sender, EventArgs e)
{
    // notice how you save a cast operation here ...
    sender.innerHTML = "hello world";
}

/* ... */
```

if you wish to have more samples for how to use phosphorus.ajax, you can check out the 
*"phosphorus.ajax.samples"* project by opening up the *"phosphorus.sln"* file

## the literal and container widgets

in phosphorus.ajax there is only two types of web controls.  there is the *"Literal"* 
class, and the *"Container"* class.  by cleverly combining these two classes however, 
you can create any html markup you wish

the **literal** widget is for controls that contains text or html, and allows you to 
change its content through the *"innerHTML"* property.  notice that the literal widget 
can have html elements inside of it, but these will be treated as client side html, 
and not possible to change on the server side, except by modifying the parent literal 
control.  everything inside of the beginning and the end of your literal widget in 
your .aspx markup will be treated as pure html, and not parsed as controls in any ways

the **container** widget can have child controls, which will be parsed in the .aspx 
markup as controls, and possible to reference on the server side, and modify 
in your server side code through its *"Controls"* collection.  everything inside of 
the beginning and the end of your container widget in your .aspx markup, will be 
treated as a web control

altough the comparison does not do justify the features of the phosphorus widgets, 
you can think of the literal widget as the *"Label"* equivalent, and the container 
widget as the *"Panel"* equivalent

#### modifying your widgets

the first thing you have to decide when creating a widget, is what html tag you wish 
to render it with.  this is set through the *"Tag"* property of your widget.  you can 
render any widget with any html tag you wish, but remember, that you have to make sure 
what you're rendering is html compliant.  phosphorus.ajax supports the html5 standard 
100%, but it also supports the html500 standard, even though nobody knows how that 
looks like today, and it is probably wise to stick to the html5 standard for now

then you can start adding attributes to your widget.  this is done by simply adding 
any attribute you wish, either directly in the markup of your .aspx page, or by using 
the index operator overload in c#.  the framework will automatically take care of 
serializing your attributes correctly back to the client

below is an example of how to create a video html5 element using a literal widget

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

you can also modify or add any attribute you wish in the codebehind by using something 
like this, and the engine will automatically keep track of which items are dirty and 
needs to be sent back to the client

```csharp
[WebMethod]
protected void video_click (Literal literal, EventArgs e)
{
    literal ["width"] = "1024";
}
```

you can modify any attribute you wish on your widgets, by using the index operator.  
phosphorus.ajax will automatically keep track of what needs to be sent from the 
server to the client.  use the *"RemoveAttribute"* method to remove an attribute

## getting started with Active Events

Active Events are an alternative to OOP which facilitates for better encapsulation
and polymorphism than traditional inheritance through its classes and interfaces

instead of inheriting from a class, and override methods from the base class, you
can directly override and replace any Active Event with any other Active Event.
this makes it easier for you to create plugins and modules that have no 
dependencies

below is an example of how you can create an Active Event

```csharp
using phosphorus.core;

[ActiveEvent (Name = "foo")]
protected static void foo (ApplicationContext literal, ActiveEventArgs e)
{
    /* ... do stuff with e.Args ... */
}
```

with Active Events you don't invoke the method directly, but rather indirectly,
through its *"Name"* property declared in your *"ActiveEvent"* attribute. this
means that you never have dependencies between the caller and the implementor of
your functions/methods/Active Events. this allows you to dynamically replace any
functionality in your system, with other modules and pieces of functionality

to raise an Active Event, you use the *"Raise"* method on your ApplicationContext
object, passing in a Node, which will become the arguments passed into your Active
Event

```csharp
ApplicationContext ctx = Loader.Instance.CreateApplicationContext ();
Node node = new Node ();
ctx.Raise ("foo", node);
```

then from inside your Active Event, you can either extract arguments passed in
to your method, or return arguments, by modifying and retrieving values from your
Node. the Node class is actually a tree structure, capable of passing in any type
of argument you wish

you can dynamically load assemblies into your application, through the
*"Loader.Instance.LoadAssembly"* method to *inject* new Active Event handlers into
your application pool

## about pf.lambda

pf.lambda is a *"programming language"* created on top of Active Events. at its core,
it is really nothing but loosely coupled Active Events, capable of executing
rudimentary programming language constructs, such as *"while"*, *"for-each"*, *"if"*
and so on

pf.lambda is very easy to learn, since it doesn't contain more than ~15 keywords, 
and should be used in symbiosis with your own Active Events, to provide an environment
where you can *"orchestrate"* your applications together, by combining existing C#
logic together, to form an *"application"* based upon loosely coupled plugins and
modules

pf.lambda can use any file format capable of constructing key/value/children nodes
as its *"language"*, such as XML or JSON, but by default, a file format called
*"Hyperlisp"* is being used. below is a simple pf.lambda program written in Hyperlisp

```

for more information about phosphorus.five, check out its author's blog at;
http://magixilluminate.com/category/phosphorus/

_x
  foo1:bar1
  foo2:bar
  foo3:bar
for-each:@/./_x/*/?node
  pf.console.write-line:"{0}:{1}"
    :@/././__dp/#/?name
    :@/././__dp/#/?value
```

## license

phosphorus five is licensed to Mother Earth.  this means that every single living
man, woman and child in the World has an equal amount of ownership to the copyright
of the project.  one of its implications, is that if you sue any living human being
over the software in any ways, you loose all usage rights.  this also includes the
JavaScript portions of the library

The MIT License (MIT)

Copyright (c) 2014 Mother Earth, Jannah, Gaia

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

