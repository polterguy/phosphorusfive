<%@ Page 
    Language="C#" 
    Inherits="phosphorus.five.samples.Literal"
    Codebehind="Literal.aspx.cs" %>

<!DOCTYPE html>
<html>
    <head>
    	<title>literal example</title>
        <link rel="stylesheet" type="text/css" href="media/main.css">
        <meta charset="utf-8">
    </head>
    <body>
        <form id="form1" runat="server" autocomplete="off">
            <div class="container">

                <h1>literal example</h1>

                <p>
                    this example shows you how to create a literal widget

                <pf:Literal
                    runat="server"
                    id="element"
                    RenderType="NoClose"
                    ElementType="p"
                    onclick="element_onclick">click me</pf:Literal>

                <p>
                    phosphorus at its core, only contains three different widgets.&nbsp;&nbsp;the <em>"Literal"</em> widget is your widget of 
                    choice when you want to supply text or html to the client.&nbsp;&nbsp;the literal has the <em>"innerValue"</em> property,
                    which allows you to change its content, as a string of text, on the server

                <p>
                    onwards to the <a href="Container.aspx">container example</a>

            </div>
        </form>
    </body>
</html>
