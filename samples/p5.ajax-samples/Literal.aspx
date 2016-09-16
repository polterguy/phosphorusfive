
<%@ Page 
    Language="C#" 
    Inherits="p5.samples.Literal"
    Codebehind="Literal.aspx.cs" %>

<!DOCTYPE html>
<html>
    <head>
    	<title>Literal widget example</title>
        <link rel="stylesheet" type="text/css" href="media/main.css">
        <meta charset="utf-8">
    </head>
    <body>
        <form id="form1" runat="server" autocomplete="off">
            <div class="container">

                <h1>Literal widget example</h1>

                <p>
                    This example shows you how to create a literal widget

                <p>
                    <p5:Literal
                        runat="server"
                        id="element"
                        RenderType="normal"
                        Element="strong"
                        onclick="element_onclick">Click me</p5:Literal>

                <p>
                    p5.ajax, at its core, only contains three different widgets. The <em>"Literal"</em> widget is your widget of 
                    choice, when you want to supply text or html to the client. The literal widget has the <em>"innerValue"</em> property,
                    which allows you to change its content, as a string of text, from the server.

                <p>
                    Onwards to the <a href="Container.aspx">container example</a>

            </div>
        </form>
    </body>
</html>
