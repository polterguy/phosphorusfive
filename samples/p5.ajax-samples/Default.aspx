
<%@ Page 
    Language="C#" 
    Inherits="p5.samples.Default"
    Codebehind="Default.aspx.cs" %>

<!DOCTYPE html>
<html>
    <head>
    	<title>p5.ajax examples</title>
        <link rel="stylesheet" type="text/css" href="media/main.css">
        <meta charset="utf-8">
    </head>
    <body>
        <form id="form1" runat="server" autocomplete="off">
            <div class="container">

                <h1>p5.ajax examples</h1>

                <p>
                    These are the examples for p5.ajax, for those who wants to directly consume the ajax library in their own websites,
                    using ASP.NET and web forms. The examples for p5.ajax, can either be read sequentially, as a book, or you can use 
                    them as a reference guide. However, p5.ajax contains only three widgets, and less than 5KB of JavaScript, so teaching 
                    yourself p5.ajax, should easily be done in minutes.

                <p5:Literal
                    runat="server"
                    id="hello"
                    Element="h2"
                    RenderType="normal"
                    onclick="hello_onclick">Click me for hello world</p5:Literal>

                <p>
                    You can change the element rendered through the <em>"Element"</em> property, and the rendering type through
                    <em>"RenderType"</em>. You can set any event handler or attribute you wish, by adding them directly
                    in markup, or using the subscript operator overload in your codebehind. For security reasons, all event handlers
                    must be marked with the <em>"WebMethod"</em> attribute. Please notice, using your browser, how only changes
                    are returned from the server.

                <p>
                    The "Element" property of your widget, can be for instance <em>"p"</em>, <em>"div"</em> or any other legal HTML5
                    element. The "RenderType" defines how the element is rendered, and if it should be immediately closed, or force the
                    creation of an "end HTML tag". Or if it has no ending at all, such as is legal for the "p" HTML element, and some other
                    element types.

                <ul>
                    <li><a href="Literal.aspx">Literal widget example</a>
                    <li><a href="Container.aspx">Container widget example</a>
                    <li><a href="Void.aspx">Void widget example</a>
                    <li><a href="JavaScript.aspx">JavaScript example</a>
                    <li><a href="DynamicControls.aspx">Dynamic controls example (Container widget)</a>
                </ul>

                <p>
                    <a href="UnitTests.aspx">p5.ajax unit tests page</a>
            </div>
        </form>
    </body>
</html>
