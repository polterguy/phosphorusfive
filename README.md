phosphorus-five
===============

phosphorus five is a web application framework for mono and asp.net.  phosphorus 
adds the fun back into creating web apps.  phosphorus is;

* secure
* lightweight
* beautiful
* flexible
* intuitive

#### secure

the web, and especially javascript, is insecure by design.  phosphorus fixes this, 
by making sure all your business logic stay on your server.  phosphorus also ties 
everything down by default, and makes your systems safe against intrusions

*"with phosphorus five, you sleep at night"*

#### lightweight

phosphorus is lightweight in all regards.  the javascript sent to the client is 
tiny, there is no unnecessary html rendered, the http traffic is tiny, and the 
server is not clogged with expensive functionality.  phosphorus solves the problems 
that needs to be solved and nothing more

*"with phosphorus five, you grow into heaven"*

#### beautiful

phosphorus features beautiful code, and allows you to create beautiful code yourself.  
the class hierarchy is easy to understand.  the javascript and html rendered is easy 
to read, and conforms to all known standards.  phosphorus facilitates for you 
creating beautiful end results

*"with phosphorus five, you are beautiful"*

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

then modify your web.config, and make sure it has something like this inside of the 
system.web section

```xml
<pages>
  <controls>
    <add assembly="phosphorus.ajax" namespace="phosphorus.ajax.widgets" tagPrefix="pf" />
  </controls>
</pages>
```

then create a widget by adding up the code below into one of your .aspx pages

```asp
<pf:Literal
    runat="server"
    id="hello"
    Tag="strong"
    onclick="hello_onclick">
    click me
</pf:Literal>
```

and the following code in your codebehind

```csharp
protected void hello_onclick (pf.Literal sender, EventArgs e)
{
    sender.innerHTML = "hello world";
}
```

if you wish to have more samples for how to use phosphorus.ajax, you can check out the 
*"phosphorus.ajax.samples"* project by opening up the *"phosphorus.sln"* file













