
<%@ Page 
    Language="C#" 
    Inherits="p5.samples.Literal"  
    ValidateRequest="false"
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
                    Below is an example of incrementally updating a textarea widget. Please inspect what goes over the wire, 
                    as you incrementally update its value.

                <p>
                    <p5:Literal
                        runat="server"
                        id="txt"
                        name="txt"
                        Element="textarea"
                        style="width:200px;height:100px;"
                        innerValue="Howdy world" />
                    <p5:Literal
                        runat="server"
                        id="btn"
                        Element="button"
                        innerValue="Append text"
                        onclick="btn_onclick" />
                    <p5:Literal
                        runat="server"
                        id="btn2"
                        Element="button"
                        innerValue="Remove text"
                        onclick="btn2_onclick" />
                    <p5:Literal
                        runat="server"
                        id="btn3"
                        Element="button"
                        innerValue="Set HTML"
                        onclick="btn3_onclick" />

                <p>
                    Onwards to the <a href="Container.aspx">container example</a>

            </div>
        </form>
    </body>
</html>
