<%@ Page 
    Language="C#" 
    Inherits="phosphorus.ajax.samples.Default"
    Codebehind="Default.aspx.cs" %>

<!DOCTYPE html>
<html>
    <head>
    	<title>phosphorus.ajax examples</title>
        <link rel="stylesheet" type="text/css" href="media/main.css">
        <meta charset="utf-8">
    </head>
    <body>
        <form id="form1" runat="server" autocomplete="off">
            <div class="container">

                <h1>phosphorus.ajax examples</h1>

                <p>
                    the examples for phosphorus, can either be read sequentially, as a book, or you can use them as a reference guide

                <h2>hello world</h2>

                <p>
                    <pf:Literal
                        runat="server"
                        id="hello"
                        ElementType="strong"
                        RenderType="Default"
                        onclick="hello_onclick">click me</pf:Literal>

                <p>
                    you can change the element rendered through the <em>"ElementType"</em> property, and the rendering type through
                    <em>"RenderType"</em>.&nbsp;&nbsp;you can set any event handler or attribute you wish, by adding them directly
                    in markup, or using the index operator overload in your codebehind.&nbsp;&nbsp;for security reasons, all event handlers
                    must be marked with the <em>"WebMethod"</em> attribute.&nbsp;&nbsp;only changes will be returned from the server

                <ul>
                    <li><a href="Literal.aspx">literal example</a>
                    <li><a href="Container.aspx">container example</a>
                    <li><a href="Void.aspx">void example</a>
                    <li><a href="JavaScript.aspx">javascript example</a>
                    <li><a href="DynamicControls.aspx">dynamic controls example</a>
                </ul>

                <hr>

                <h2>the 5 pillars of phosphorus</h2>

                <p>
                    phosphorus builds on top of the following pillars

                <h3>secure</h3>

                <p>
                    javascript is insecure by design.&nbsp;&nbsp;phosphorus allows you to put all your business logic on your server

                <p>
                    <em>"with phosphorus five, you sleep like a baby at night"</em>

                <h3>lightweight</h3>

                <p>
                    phosphorus is lightweight.&nbsp;&nbsp;there is no unnecessary javascript, html or json rendered

                <p>
                    <em>"with phosphorus, your creations can grow into heaven"</em>

                <h3>beautiful</h3>

                <p>
                    phosphorus classes are easily understood, and the html and javascript is rendered according to best practices

                <p>
                    <em>"with phosphorus, you can build for the aeons"</em>

                <h3>flexible</h3>

                <p>
                    phosphorus allows you to override what you need to override, and leave the rest as it should be

                <p>
                    <em>"with phosphorus, you are the boss"</em>

                <h3>intuitive</h3>

                <p>
                    phosphorus is easy to understand, and contains no black magic concepts

                <p>
                    <em>"with phosphorus, your first hunch is probably right"</em>
            </div>
        </form>
    </body>
</html>
